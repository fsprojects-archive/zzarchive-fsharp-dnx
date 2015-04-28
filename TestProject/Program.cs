using System.IO;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Compilation;

namespace TestProject
{
  public class Program
  {
    const string PATH = @"C:\test";

    readonly ILibraryManager _mgr;

    public Program(ILibraryManager mgr)
    {
      _mgr = mgr;
    }

    public void Main()
    {
      var library = _mgr.GetAllExports("TestProject");
      var reference = library.MetadataReferences[0] as IMetadataProjectReference;
      if (!Directory.Exists(PATH))
        Directory.CreateDirectory(PATH);
      reference.EmitAssembly(PATH);
      reference.EmitAssembly(PATH);
    }
  }
}
