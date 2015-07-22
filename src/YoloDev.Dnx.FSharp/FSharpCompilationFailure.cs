using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Compilation;

namespace YoloDev.Dnx.FSharp
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
