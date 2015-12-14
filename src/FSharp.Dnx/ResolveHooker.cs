using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.CompilationAbstractions;
using Microsoft.FSharp.Compiler;
using Microsoft.FSharp.Compiler.SimpleSourceCodeServices;
using Microsoft.Dnx.Runtime;

namespace FSharp.Dnx
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
      if (args.Name.StartsWith("FSharp.Core,", StringComparison.OrdinalIgnoreCase))
      {

#if DEBUG
        Logger.TraceInformation("[{0}]: HandleResolve '{1}'", GetType().Name, args.Name);
#endif

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
