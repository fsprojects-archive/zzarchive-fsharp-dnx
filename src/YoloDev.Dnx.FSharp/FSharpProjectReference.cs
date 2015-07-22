using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Compilation;

namespace YoloDev.Dnx.FSharp
{
  public class FSharpProjectReference : IMetadataProjectReference
  {
    readonly CompilationContext _context;

    public FSharpProjectReference(CompilationContext context)
    {
      _context = context;
    }

    public string Name => _context.Name;
    public string ProjectPath => _context.ProjectPath;

    public DiagnosticResult GetDiagnostics()
    {
      var diagnostics = _context.Diagnostics;

      return CreateDiagnosticResult(success: true, diagnostics: diagnostics);
    }

    public IList<ISourceReference> GetSources()
    {
      return _context.SourceFiles
        .Select(FSharpSourceReference.Create)
        .Cast<ISourceReference>()
        .ToImmutableList();
    }

    public Assembly Load(IAssemblyLoadContext loadContext)
    {
      if (!_context.Success)
      {
        throw new FSharpCompilationException(_context.Diagnostics);
      }

      using (var assembly = new MemoryStream(_context.Assembly))
      {
        if (FSharpCompiler.SupportsPdbGeneration && _context.Pdb != null)
        {
          using (var pdb = new MemoryStream(_context.Pdb))
          {
            return loadContext.LoadStream(assembly, pdb);
          }
        }
        else
        {
          Logger.TraceWarning("PDB generation is not supported on this platform");
          return loadContext.LoadStream(assembly, null);
        }
      }
    }

    public void EmitReferenceAssembly(Stream stream)
    {
      if (!_context.Success)
      {
        throw new FSharpCompilationException(_context.Diagnostics);
      }

      using (var assembly = new MemoryStream(_context.Assembly))
        assembly.CopyTo(stream);
    }

    public DiagnosticResult EmitAssembly(string outputPath)
    {
      if (!_context.Success)
      {
        throw new FSharpCompilationException(_context.Diagnostics);
      }

      if (_context.Assembly == null)
        throw new InvalidOperationException($"Assembly {Name} is null");

      if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);

      WriteOut(outputPath, Name, ".dll", _context.Assembly);
      WriteOut(outputPath, Name, ".pdb", _context.Pdb);
      WriteOut(outputPath, Name, ".xml", _context.Xml);

      return CreateDiagnosticResult(success: _context.Success, diagnostics: _context.Diagnostics);
    }

    static DiagnosticResult CreateDiagnosticResult(bool success, IEnumerable<FSharpDiagnosticMessage> diagnostics)
    {
      return new FSharpDiagnosticResult(success, diagnostics);
    }

    private static void WriteOut(string folder, string name, string extension, byte[] data)
    {
      if (data == null) return;

      var path = Path.Combine(folder, name + extension);
      File.WriteAllBytes(path, data);
    }
  }
}
