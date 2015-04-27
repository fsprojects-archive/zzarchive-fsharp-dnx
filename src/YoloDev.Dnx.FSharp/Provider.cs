using System;
using System.Collections.Generic;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Compilation;

namespace YoloDev.Dnx.FSharp
{
  public class FSharpProjectReferenceProvider : IProjectCompiler
  {
    readonly IFileWatcher _watcher;

    public FSharpProjectReferenceProvider(IFileWatcher watcher)
    {
      _watcher = watcher;
    }

    public IMetadataProjectReference CompileProject(
      ICompilationProject project,
      ILibraryKey target,
      Func<ILibraryExport> referenceResolver,
      Func<IList<ResourceDescriptor>> resourcesResolver)
    {
      return new FSharpMetadataProjectReference(
        project,
        target,
        referenceResolver,
        resourcesResolver,
        _watcher);
    }
  }
}
