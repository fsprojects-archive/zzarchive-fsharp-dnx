using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.Dnx;
using Microsoft.Dnx.Compilation;

namespace FSharp.Dnx
{
  class FSharpCompilationFailure : CompilationFailure
  {
    public FSharpCompilationFailure(
      string sourceFilePath,
      IEnumerable<FSharpDiagnosticMessage> messages)
      : base(sourceFilePath, messages)
    {
    }

    public new IEnumerable<FSharpDiagnosticMessage> Messages => base.Messages.Cast<FSharpDiagnosticMessage>();
  }
}
