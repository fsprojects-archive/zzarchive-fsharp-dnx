using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Compilation;
using Microsoft.FSharp.Compiler;

namespace YoloDev.Dnx.FSharp
{
  class FSharpDiagnosticResult : IDiagnosticResult
  {
    readonly IImmutableList<FSharpCompilationMessage> _messages;
    readonly bool? _success;

    public bool Success => _success.HasValue ? _success.Value : !_messages.Any(m => m.Severity == CompilationMessageSeverity.Error);

    public IEnumerable<ICompilationMessage> Diagnostics => _messages.Cast<ICompilationMessage>();
    public IImmutableList<FSharpCompilationMessage> Messages => _messages;

    public FSharpDiagnosticResult(IEnumerable<FSharpCompilationMessage> messages)
    {
      _messages = messages.ToImmutableList();
      _success = null;
    }

    public FSharpDiagnosticResult(bool success, IEnumerable<FSharpCompilationMessage> messages)
    {
      _messages = messages.ToImmutableList();
      _success = success;
    }

    internal static FSharpDiagnosticResult Error(string projectPath, string errorMessage)
    {
      var message = FSharpCompilationMessage.Error(projectPath, errorMessage);
      return new FSharpDiagnosticResult(ImmutableList.Create(message));
    }

    internal static FSharpDiagnosticResult CompilationResult(int resultCode, IEnumerable<FSharpErrorInfo> errors)
    {
      var success = resultCode == 0;
      return new FSharpDiagnosticResult(
        success,
        errors.Select(FSharpCompilationMessage.CompilationMessage).ToImmutableList());
    }
  }
}
