using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Dnx;
using Microsoft.Dnx.Compilation;
using Microsoft.Dnx.Runtime;

namespace YoloDev.Dnx.FSharp
{
  class FSharpCompilationException : Exception, ICompilationException
  {
    readonly ImmutableList<FSharpCompilationFailure> _failures;

    public FSharpCompilationException(IEnumerable<FSharpDiagnosticMessage> diagnostics)
      : base(GetErrorMessage(diagnostics))
    {
      _failures = diagnostics.Where(d => d.Severity == DiagnosticMessageSeverity.Error)
                             .GroupBy(d => d.SourceFilePath, StringComparer.OrdinalIgnoreCase)
                             .Select(g => new FSharpCompilationFailure(g.Key, g))
                             .ToImmutableList();
    }

    public IEnumerable<CompilationFailure> CompilationFailures => _failures.Cast<CompilationFailure>();

    static string GetErrorMessage(IEnumerable<FSharpDiagnosticMessage> diagnostics)
      => string.Join(Environment.NewLine,
        diagnostics.Select(d => d.FormattedMessage));
  }
}
