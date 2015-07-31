using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Dnx;
using Microsoft.Dnx.Compilation;
using Microsoft.FSharp.Compiler;
using Microsoft.FSharp.Compiler.SimpleSourceCodeServices;

namespace YoloDev.Dnx.FSharp
{
  // HUGE UGLY HACK. NEEDS TO BE REMOVED.
  class ResolveHooker : IDisposable
  {
    readonly object l = new object();
    bool inner = false;

    public ResolveHooker()
    {
      AppDomain.CurrentDomain.AssemblyResolve += HandleResolve;
    }

    public void Dispose()
    {
      AppDomain.CurrentDomain.AssemblyResolve -= HandleResolve;
    }

    private System.Reflection.Assembly HandleResolve(object sender, ResolveEventArgs args)
    {
      if (args.Name.StartsWith("FSharp.Core", StringComparison.OrdinalIgnoreCase))
      {
        lock (l)
        {
          if (inner)
          {
            return null;
          }

          inner = true;
        }

        try
        {
          return System.Reflection.Assembly.Load("FSharp.Core");
        }
        finally
        {
          lock(l)
          {
            inner = false;
          }
        }
      }

      return null;
    }
  }
}
