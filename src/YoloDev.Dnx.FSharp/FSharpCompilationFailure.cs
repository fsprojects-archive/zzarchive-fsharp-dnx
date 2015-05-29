using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.Framework.Runtime;

namespace YoloDev.Dnx.FSharp
{
  class FSharpCompilationFailure : ICompilationFailure
  {
    readonly string _filePath;
    readonly string _fileContent;
    readonly ImmutableList<FSharpCompilationMessage> _messages;

    public FSharpCompilationFailure(
      string filePath,
      IEnumerable<FSharpCompilationMessage> messages)
    {
      _filePath = filePath;
      _fileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : null;
      _messages = messages.ToImmutableList();
    }

    public string CompiledContent => null;
    public string SourceFilePath => _filePath;
    public string SourceFileContent => _fileContent;
    public IEnumerable<ICompilationMessage> Messages => _messages.Cast<ICompilationMessage>();
  }
}
