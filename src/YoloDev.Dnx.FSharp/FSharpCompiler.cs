using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Caching;
using Microsoft.Framework.Runtime.Compilation;
using Microsoft.FSharp.Compiler;
using Microsoft.FSharp.Compiler.SimpleSourceCodeServices;
using YoloDev.Dnx.Json;

namespace YoloDev.Dnx.FSharp
{
  public class FSharpCompiler
  {
    const string PROJECT_FILE_NAME = "fsharp.project";

    static Lazy<bool> _supportsPdbGeneration = new Lazy<bool>(CheckPdbGenerationSupport);
    internal static bool SupportsPdbGeneration => _supportsPdbGeneration.Value;

    readonly ICache _cache;
    readonly ICacheContextAccessor _cacheContextAccessor;
    readonly INamedCacheDependencyProvider _namedDependencyProvider;
    readonly IAssemblyLoadContextFactory _loadContextFactory;
    readonly IFileWatcher _watcher;
    readonly IApplicationEnvironment _environment;
    readonly IServiceProvider _services;

    public FSharpCompiler(
      ICache cache,
      ICacheContextAccessor cacheContextAccessor,
      INamedCacheDependencyProvider namedDependencyProvider,
      IAssemblyLoadContextFactory loadContextFactory,
      IFileWatcher watcher,
      IApplicationEnvironment environment,
      IServiceProvider services)
    {
      _cache = cache;
      _cacheContextAccessor = cacheContextAccessor;
      _namedDependencyProvider = namedDependencyProvider;
      _loadContextFactory = loadContextFactory;
      _watcher = watcher;
      _environment = environment;
      _services = services;
    }

    public CompilationContext CompileProject(
      ICompilationProject project,
      ILibraryKey target,
      IEnumerable<IMetadataReference> incomingReferences,
      IEnumerable<ISourceReference> incomingSourceReferences,
      Func<IList<ResourceDescriptor>> resourcesResolver)
    {
      var path = project.ProjectDirectory;
      var name = project.Name;
      var fsproj = GetProjectInfo(path);

      _watcher.WatchProject(path);
      _watcher.WatchFile(project.ProjectFilePath);
      _watcher.WatchFile(fsproj.ProjectFilePath);
      foreach (var f in fsproj.Files)
        _watcher.WatchFile(f);

      if (_cacheContextAccessor.Current != null)
      {
        _cacheContextAccessor.Current.Monitor(new FileWriteTimeCacheDependency(fsproj.ProjectFilePath));

        // Monitor the trigger {projectName}_BuildOutputs
        var buildOutputsName = project.Name + "_BuildOutputs";

        _cacheContextAccessor.Current.Monitor(_namedDependencyProvider.GetNamedDependency(buildOutputsName));
        _cacheContextAccessor.Current.Monitor(_namedDependencyProvider.GetNamedDependency(project.Name + "_Dependencies"));
      }

      Logger.TraceInformation("[{0}]: Compiling '{1}'", GetType().Name, name);
      var sw = Stopwatch.StartNew();

      CompilationContext context;
      using (new ResolveHooker())
      using (var files = new TempFiles())
      {
        var outFileName = $"{name}.dll";
        var outDir = files.CreateDir();
        var outFile = Path.Combine(outDir, outFileName);
        var args = new List<string>();
        args.Add("fsc.exe");
        args.Add($"--out:{outFile}");
        args.Add("--target:library");
        args.Add("--noframework");
        args.Add("--optimize-");
        args.Add("--debug");
        if (SupportsPdbGeneration)
          args.Add($"--pdb:{Path.ChangeExtension(outFile, ".pdb")}");
        args.Add($"--doc:{Path.ChangeExtension(outFile, ".xml")}");

        foreach (var source in fsproj.Files)
          args.Add(source);

        // These are the metadata references being used by your project.
        // Everything in your project.json is resolved and normailzed here:
        // - Project references
        // - Package references are turned into the appropriate assemblies
        // Each IMetadaReference maps to an assembly
        foreach (var reference in incomingReferences)
        {
          string fileName = null;
          var projectRef = reference as IMetadataProjectReference;
          if (projectRef != null)
          {
            var dir = files.CreateDir();
            projectRef.EmitAssembly(dir);
            fileName = Path.Combine(dir, $"{projectRef.Name}.dll");
          }

          var fileRef = reference as IMetadataFileReference;
          if (fileRef != null)
            fileName = fileRef.Path;
          else if (fileName == null)
            throw new Exception($"Unknown reference type {reference.GetType()}");

          args.Add($"-r:{fileName}");
        }

        //Console.WriteLine(string.Join(Environment.NewLine, args));
        var scs = new SimpleSourceCodeServices();
        var result = scs.Compile(args.ToArray());
        var errors = result.Item1.Select(FSharpCompilationMessage.CompilationMessage);
        var resultCode = result.Item2;

        //System.Diagnostics.Debugger.Launch();
        MemoryStream assembly = null;
        MemoryStream pdb = null;
        MemoryStream xml = null;
        if (resultCode == 0)
        {
          assembly = new MemoryStream();
          xml = new MemoryStream();

          using (var fs = File.OpenRead(outFile))
            fs.CopyTo(assembly);

          var pdbFile = Path.ChangeExtension(outFile, ".pdb");
          if (File.Exists(pdbFile))
          {
            pdb = new MemoryStream();
            using (var fs = File.OpenRead(pdbFile))
              fs.CopyTo(pdb);
          }

          var xmlFile = Path.ChangeExtension(outFile, ".xml");
          if (File.Exists(xmlFile))
          {
            xml = new MemoryStream();
            using (var fs = File.OpenRead(xmlFile))
              fs.CopyTo(xml);
          }
        }

        context = new CompilationContext(
          project,
          fsproj,
          resultCode == 0,
          errors,
          assembly?.ToArray(),
          pdb?.ToArray(),
          xml?.ToArray());

        assembly?.Dispose();
        pdb?.Dispose();
        xml?.Dispose();
      }

      sw.Stop();
      Logger.TraceInformation("[{0}]: Compiled '{1}' in {2}ms", GetType().Name, name, sw.ElapsedMilliseconds);

      return context;
    }

    static FSharpProjectInfo GetProjectInfo(string projectDirectory)
    {
      var path = Path.Combine(projectDirectory, PROJECT_FILE_NAME);
      if (!File.Exists(path))
      {
        throw new FileNotFoundException("FSharp project file not found", path);
      }

      JsonObject info;
      using (var fs = File.OpenRead(path))
      using (var rd = new StreamReader(fs))
      {
        var result = JsonDeserializer.Deserialize(rd).AsObject();
        if (result == null)
        {
          throw new InvalidOperationException($"{path} did not contain a valid fsharp project configuration");
        }

        info = result;
      }

      var autoDiscoverFsi = info.ValueAsBoolean("autoDiscoverFsi", true);
      var fileNames = info.ValueAsStringArray("files");

      var files = new List<string>();
      foreach (var f in fileNames)
      {
        var fullName = Path.Combine(projectDirectory, f);
        if (!File.Exists(fullName))
        {
          throw new FileNotFoundException("FSharp file {fullName} not found", fullName);
        }

        if (autoDiscoverFsi)
        {
          var fsiName = Path.ChangeExtension(fullName, ".fsi");
          if (File.Exists(fsiName))
          {
            files.Add(fsiName);
          }
        }

        files.Add(fullName);
      }

      return new FSharpProjectInfo(path, autoDiscoverFsi, files.ToImmutableList());
    }

    private static bool CheckPdbGenerationSupport()
    {
      try
      {
        if (PlatformHelper.IsMono)
        {
          return false;
        }

        // Check for the pdb writer component that roslyn uses to generate pdbs
        const string SymWriterGuid = "0AE2DEB0-F901-478b-BB9F-881EE8066788";

        var type = Marshal.GetTypeFromCLSID(new Guid(SymWriterGuid));

        if (type != null)
        {
          return Activator.CreateInstance(type) != null;
        }

        return false;
      }
      catch
      {
        return false;
      }
    }
  }

  class FSharpProjectInfo
  {
    public string ProjectFilePath { get; }
    public bool AutoDiscoverFsi { get; }
    public IImmutableList<string> Files { get; }

    public FSharpProjectInfo(
      string projectFilePath,
      bool autoDiscoverFsi,
      IImmutableList<string> files)
    {
      ProjectFilePath = projectFilePath;
      AutoDiscoverFsi = autoDiscoverFsi;
      Files = files;
    }
  }
}
