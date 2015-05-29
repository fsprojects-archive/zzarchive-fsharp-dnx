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
      var export = referenceResolver();
      if (export == null)
      {
          return null;
      }

      var incomingReferences = export.MetadataReferences;

      return new FSharpMetadataProjectReference(
        project,
        target,
        incomingReferences,
        resourcesResolver,
        _watcher);
    }
  }
}
