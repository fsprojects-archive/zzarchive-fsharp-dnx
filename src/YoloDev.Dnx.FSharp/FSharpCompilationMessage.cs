using Microsoft.Framework.Runtime;
using Microsoft.FSharp.Compiler;

namespace YoloDev.Dnx.FSharp
{
  public class FSharpCompilationMessage : ICompilationMessage
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

    internal static FSharpCompilationMessage Error(string projectPath, string error)
    {
      return new FSharpCompilationMessage(1, 1, 1, 1, projectPath, error, CompilationMessageSeverity.Error);
    }

    internal static FSharpCompilationMessage CompilationMessage(FSharpErrorInfo message)
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
}
