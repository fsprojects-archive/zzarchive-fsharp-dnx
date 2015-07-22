using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Framework.Runtime.Compilation;

namespace YoloDev.Dnx.FSharp
{
  public class CompilationContext
  {
    readonly CompilationProjectContext _project;
    readonly FSharpProjectInfo _fsproj;
    readonly bool _success;
    readonly List<FSharpDiagnosticMessage> _messages;
    readonly byte[] _assembly;
    readonly byte[] _pdb;
    readonly byte[] _xml;

    internal CompilationContext(
      CompilationProjectContext project,
      FSharpProjectInfo fsproj,
      bool success,
      IEnumerable<FSharpDiagnosticMessage> messages,
      byte[] assembly,
      byte[] pdb,
      byte[] xml)
    {
      _project = project;
      _fsproj = fsproj;
      _success = success;
      _messages = messages.ToList();
      _assembly = assembly;
      _pdb = pdb;
      _xml = xml;
    }

    public string Name => _project.Target.Name;
    public string ProjectPath => _project.ProjectFilePath;
    public bool Success => _success;
    public IEnumerable<FSharpDiagnosticMessage> Diagnostics => _messages;
    public IEnumerable<string> SourceFiles => _fsproj.Files;

    public byte[] Assembly => _assembly;
    public byte[] Pdb => _pdb;
    public byte[] Xml => _xml;
  }
}
