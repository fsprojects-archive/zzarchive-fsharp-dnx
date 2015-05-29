using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Compilation;

namespace YoloDev.Dnx.FSharp
{
  class FSharpCompilationException : Exception, ICompilationException
  {
    readonly ImmutableList<FSharpCompilationFailure> _failures;

    public FSharpCompilationException(IEnumerable<FSharpCompilationMessage> diagnostics)
      : base(GetErrorMessage(diagnostics))
    {
      _failures = diagnostics.Where(d => d.Severity == CompilationMessageSeverity.Error)
                             .GroupBy(d => d.SourceFilePath, StringComparer.OrdinalIgnoreCase)
                             .Select(g => new FSharpCompilationFailure(g.Key, g))
                             .ToImmutableList();
    }

    public IEnumerable<ICompilationFailure> CompilationFailures => _failures.Cast<ICompilationFailure>();

    static string GetErrorMessage(IEnumerable<FSharpCompilationMessage> diagnostics)
      => string.Join(Environment.NewLine,
        diagnostics.Select(d => d.FormattedMessage));
  }
}
