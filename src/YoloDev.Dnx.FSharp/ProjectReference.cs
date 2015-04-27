using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Compilation;
using Microsoft.FSharp.Compiler;
using Microsoft.FSharp.Compiler.SimpleSourceCodeServices;

namespace YoloDev.Dnx.FSharp
{
  class FSharpMetadataProjectReference : IMetadataProjectReference
  {
    readonly ICompilationProject _project;
    readonly ILibraryKey _target;
    readonly Lazy<ILibraryExport> _export;
    readonly Lazy<IList<ResourceDescriptor>> _resources;
    readonly Lazy<FSharpCompilationResult> _compile;

    public FSharpMetadataProjectReference(
      ICompilationProject project,
      ILibraryKey target,
      Func<ILibraryExport> export,
      Func<IList<ResourceDescriptor>> resources,
      IFileWatcher watcher)
    {
      _project = project;
      _target = target;
      _export = new Lazy<ILibraryExport>(export);
      _resources = new Lazy<IList<ResourceDescriptor>>(resources);

      foreach (var source in _project.Files.SourceFiles)
        watcher.WatchFile(source);

      _compile = new Lazy<FSharpCompilationResult>(Emit);
    }

    public string Name => _project.Name;

    public string ProjectPath => _project.ProjectFilePath;

    public IList<ISourceReference> GetSources()
    {
      return _project.Files.SourceFiles
        .Select(f => (ISourceReference)new FSharpSourceReference(f))
        .ToList();
    }

    public IDiagnosticResult GetDiagnostics() => _compile.Value.Diagnostics;

    public void EmitReferenceAssembly(Stream stream)
    {
      _compile.Value.CopyAssembly(stream);
    }

    public IDiagnosticResult EmitAssembly(string outputPath)
    {
      var compile = _compile.Value;
      compile.EmitTo(outputPath, _project.Name);
      return compile.Diagnostics;
    }

    public System.Reflection.Assembly Load(IAssemblyLoadContext loadContext)
    {
      return _compile.Value.Load(loadContext);
    }

    private FSharpCompilationResult Emit()
    {
      using(var files = new TempFiles())
      {
        var sharedSources = _project.Files.SharedFiles;
        var sources = _project.Files.SourceFiles;

        if (sharedSources.Any())
        {
          return new FSharpCompilationResult(
            FSharpDiagnosticResult.Error(
              _project.ProjectFilePath,
              "The F# provider does not support shared sources"),
            null, null, null);
        }

        var outFileName = $"{_project.Name}.dll";
        var outDir = files.CreateDir();
        var outFile = Path.Combine(outDir, outFileName);
        var args = new List<string>();
        args.Add("fsc.exe");
        args.Add($"--out:{outFile}");
        args.Add($"--target:library");
        args.Add("--noframework");
        args.Add("--optimize-");

        // F# cares about order so assume that the files were listed in order
        foreach (var source in sources)
          args.Add(source);

        // These are the metadata references being used by your project.
        // Everything in your project.json is resolved and normailzed here:
        // - Project references
        // - Package references are turned into the appropriate assemblies
        // Each IMetadaReference maps to an assembly
        foreach (var reference in _export.Value.MetadataReferences)
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

        var scs = new SimpleSourceCodeServices();
        var result = scs.Compile(args.ToArray());
        var errors = result.Item1;
        var resultCode = result.Item2;

        //System.Diagnostics.Debugger.Launch();
        MemoryStream assembly = null;
        MemoryStream pdb = null;
        MemoryStream xml = null;
        if (resultCode == 0)
        {
          assembly = new MemoryStream();
          //pdb = new MemoryStream();
          //xml = new MemoryStream();

          using (var fs = File.OpenRead(outFile))
            fs.CopyTo(assembly);

          // TODO: Includ pdb and xml streams
        }

        return new FSharpCompilationResult(
          FSharpDiagnosticResult.CompilationResult(resultCode, errors),
          assembly,
          pdb,
          xml);
      }
    }
  }

  class FSharpSourceReference : ISourceReference
  {
    readonly string _path;

    public FSharpSourceReference(string path)
    {
      _path = path;
    }

    public string Name => _path;
    public string Path => _path;
  }

  class FSharpDiagnosticResult : IDiagnosticResult
  {
    readonly IImmutableList<ICompilationMessage> _messages;
    readonly bool? _success;

    public bool Success => _success.HasValue ? _success.Value : !_messages.Any(m => m.Severity == CompilationMessageSeverity.Error);

    public IEnumerable<ICompilationMessage> Diagnostics => _messages;

    public FSharpDiagnosticResult(IImmutableList<ICompilationMessage> messages)
    {
      _messages = messages;
      _success = null;
    }

    public FSharpDiagnosticResult(bool success, IImmutableList<ICompilationMessage> messages)
    {
      _messages = messages;
      _success = success;
    }

    internal static IDiagnosticResult Error(string projectPath, string errorMessage)
    {
      var message = (ICompilationMessage)FSharpCompilationMessage.Error(projectPath, errorMessage);
      return new FSharpDiagnosticResult(ImmutableList.Create(message));
    }

    internal static IDiagnosticResult CompilationResult(int resultCode, IEnumerable<FSharpErrorInfo> errors)
    {
      var success = resultCode == 0;
      return new FSharpDiagnosticResult(
        success,
        errors.Select(FSharpCompilationMessage.CompilationMessage).ToImmutableList());
    }
  }

  class FSharpCompilationMessage : ICompilationMessage
  {
    const string MESSAGE_TEMPLATE = "{0}({1},{2}): {3}: {4}";

    readonly int _startColumn;
    readonly int _startLine;
    readonly int _endColumn;
    readonly int _endLine;
    readonly string _sourceFilePath;
    readonly string _message;
    readonly CompilationMessageSeverity _severity;

    public int StartColumn => _startColumn;
    public int StartLine => _startLine;
    public int EndColumn => _endColumn;
    public int EndLine => _endLine;
    public string SourceFilePath => _sourceFilePath;
    public string Message => _message;
    public CompilationMessageSeverity Severity => _severity;
    public string FormattedMessage => string.Format(MESSAGE_TEMPLATE, SourceFilePath, StartLine, StartColumn, Severity.ToString().ToLowerInvariant(), Message);

    public FSharpCompilationMessage(
      int startColumn,
      int startLine,
      int endColumn,
      int endLine,
      string sourceFilePath,
      string message,
      CompilationMessageSeverity severity)
    {
      _startColumn = startColumn;
      _startLine = startLine;
      _endColumn = endColumn;
      _endLine = endLine;
      _sourceFilePath = sourceFilePath;
      _message = message;
      _severity = severity;
    }

    internal static ICompilationMessage Error(string projectPath, string error)
    {
      return new FSharpCompilationMessage(1, 1, 1, 1, projectPath, error, CompilationMessageSeverity.Error);
    }

    internal static ICompilationMessage CompilationMessage(FSharpErrorInfo message)
    {
      var severity = message.Severity.IsError ?
        CompilationMessageSeverity.Error :
        CompilationMessageSeverity.Warning;

      return new FSharpCompilationMessage(
        message.StartColumn,
        message.StartLineAlternate,
        message.EndColumn,
        message.EndLineAlternate,
        message.FileName,
        message.Message,
        severity);
    }
  }

  class FSharpCompilationResult
  {
    readonly MemoryStream _assembly;
    readonly MemoryStream _pdb;
    readonly MemoryStream _xml;
    readonly IDiagnosticResult _diagnostics;

    public FSharpCompilationResult(
      IDiagnosticResult diagnostics,
      MemoryStream assembly,
      MemoryStream pdb,
      MemoryStream xml)
    {
      _diagnostics = diagnostics;
      _pdb = pdb;
      _xml = xml;
      _assembly = assembly;
    }

    public IDiagnosticResult Diagnostics => _diagnostics;

    public void CopyAssembly(Stream stream)
    {
      _assembly.Seek(0, SeekOrigin.Begin);
      _assembly.CopyTo(stream);
    }

    public void EmitTo(string dir, string name)
    {
      if (!Directory.Exists(dir))
        Directory.CreateDirectory(dir);

      var assemblyName = Path.Combine(dir, $"{name}.dll");
      using (var fs = File.Open(assemblyName, FileMode.OpenOrCreate, FileAccess.Write))
        CopyAssembly(fs);

      // TODO: Handle pdb and xml
    }

    public System.Reflection.Assembly Load(IAssemblyLoadContext loadContext)
    {
      _assembly.Seek(0, SeekOrigin.Begin);
      return loadContext.LoadStream(_assembly, null);
    }
  }

  class TempFiles : IDisposable
  {
    readonly List<string> _files;
    readonly List<string> _dirs;

    public TempFiles()
    {
      _files = new List<string>();
      _dirs = new List<string>();
    }

    public Stream CreateStream(out string path)
    {
      path = Path.GetTempFileName();
      _files.Add(path);
      return File.Open(path, FileMode.Truncate, FileAccess.Write);
    }

    public string CreateDir()
    {
      var path = Path.GetTempFileName();
      File.Delete(path);
      Directory.CreateDirectory(path);
      _dirs.Add(path);
      return path;
    }

    public void Dispose()
    {
      foreach (var file in _files)
        if (File.Exists(file))
          File.Delete(file);

      foreach (var dir in _dirs)
        if (Directory.Exists(dir))
          Directory.Delete(dir, true);
    }
  }
}
