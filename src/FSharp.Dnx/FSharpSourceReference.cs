using System;
using Microsoft.Extensions.CompilationAbstractions;

namespace FSharp.Dnx
{
  class FSharpSourceReference : ISourceReference
  {
    readonly string _path;

    public FSharpSourceReference(string path)
    {
      _path = path;
    }

    public string Name => _path;
    public string Path => _path;

    public static FSharpSourceReference Create(string path)
      => new FSharpSourceReference(path);
  }
}
