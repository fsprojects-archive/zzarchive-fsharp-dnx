using System.Collections.Generic;
using System.Linq;
using Microsoft.Dnx.Compilation;

namespace FSharp.Dnx
{
  public class CompilationContext
  {
    private readonly CompilationProjectContext _project;

    internal CompilationContext(
      CompilationProjectContext project,
      IEnumerable<string> sourceFiles,
      bool success,
      IEnumerable<FSharpDiagnosticMessage> messages,
      byte[] assembly,
      byte[] pdb,
      byte[] xml)
    {
      _project = project;
      SourceFiles = sourceFiles;
      Success = success;
      Diagnostics = messages.ToList();
      Assembly = assembly;
      Pdb = pdb;
      Xml = xml;
    }

    public string Name => _project.Target.Name;
    public string ProjectPath => _project.ProjectFilePath;
    public bool Success { get; }
    public IEnumerable<FSharpDiagnosticMessage> Diagnostics { get; }
    public IEnumerable<string> SourceFiles { get; }

    public byte[] Assembly { get; }
    public byte[] Pdb { get; }
    public byte[] Xml { get; }
  }
}
