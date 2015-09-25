using Microsoft.Dnx;
using Microsoft.Dnx.Runtime;
using Microsoft.FSharp.Compiler;

namespace YoloDev.Dnx.FSharp
{
  public class FSharpDiagnosticMessage : DiagnosticMessage
  {
    public FSharpDiagnosticMessage(
      string errorCode,
      int startColumn,
      int startLine,
      string filePath,
      string message,
      DiagnosticMessageSeverity severity)
      : base(errorCode, message, filePath, severity, startLine, startColumn)
    {
    }

    internal static FSharpDiagnosticMessage Error(string projectPath, string error)
    {
      return new FSharpDiagnosticMessage("YOLO01", 1, 1, projectPath, error, DiagnosticMessageSeverity.Error);
    }

    internal static FSharpDiagnosticMessage CompilationMessage(FSharpErrorInfo message)
    {
      var severity = message.Severity.IsError ?
        DiagnosticMessageSeverity.Error :
        DiagnosticMessageSeverity.Warning;

      return new FSharpDiagnosticMessage(
        message.Subcategory,
        message.StartColumn,
        message.StartLineAlternate,
        message.FileName,
        message.Message,
        severity);
    }
  }
}
