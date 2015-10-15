using System.Collections.Immutable;

namespace FSharp.Dnx
{
    public class FSharpProjectInfo
    {
      public FSharpProjectInfo(IImmutableList<string> files)
      {
        Files = files;
      }
      
      public IImmutableList<string> Files { get; }
    }
}