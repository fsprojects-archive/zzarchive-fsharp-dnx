using System.Collections.Immutable;

namespace FSharp.Dnx
{
    public class FSharpProjectInfo
    {
      public FSharpProjectInfo(bool autoDiscoverFsi, IImmutableList<string> files)
      {
        AutoDiscoverFsi = autoDiscoverFsi;
        Files = files;
      }
      
      public bool AutoDiscoverFsi { get; }
      public IImmutableList<string> Files { get; }
    }
}