using System.Collections.Generic;
using System.Linq;
using Microsoft.Dnx.Compilation;

namespace FSharp.Dnx
{
  public class CompilationContext
  {
    readonly CompilationProjectContext _project;

    internal CompilationContext(
      CompilationProjectContext project,
      FSharpProjectInfo projectInfo,
      bool success,
      IEnumerable<FSharpDiagnosticMessage> messages,
      byte[] assembly,
      byte[] pdb,
      byte[] xml)
    {
      _project = project;
      ProjectInfo = projectInfo;
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
    public FSharpProjectInfo ProjectInfo { get; }

    public byte[] Assembly { get; }
    public byte[] Pdb { get; }
    public byte[] Xml { get; }
  }
}
