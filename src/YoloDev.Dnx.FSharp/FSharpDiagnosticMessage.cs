using Microsoft.Framework.Runtime;
using Microsoft.FSharp.Compiler;

namespace YoloDev.Dnx.FSharp
{
  public class FSharpDiagnosticMessage : DiagnosticMessage
  {
    public FSharpDiagnosticMessage(
      int startColumn,
      int startLine,
      int endColumn,
      int endLine,
      string filePath,
      string message,
      DiagnosticMessageSeverity severity)
      : base(message, $"{filePath}({startLine},{startColumn}): {severity.ToString().ToLowerInvariant()}: {message}", filePath, severity, startLine, startColumn, endLine, endColumn)
    {
    }

    internal static FSharpDiagnosticMessage Error(string projectPath, string error)
    {
      return new FSharpDiagnosticMessage(1, 1, 1, 1, projectPath, error, DiagnosticMessageSeverity.Error);
    }

    internal static FSharpDiagnosticMessage CompilationMessage(FSharpErrorInfo message)
    {
      var severity = message.Severity.IsError ?
        DiagnosticMessageSeverity.Error :
        DiagnosticMessageSeverity.Warning;

      return new FSharpDiagnosticMessage(
        message.StartColumn,
        message.StartLineAlternate,
        message.EndColumn,
        message.EndLineAlternate,
        message.FileName,
        message.Message,
        severity);
    }
  }
}
