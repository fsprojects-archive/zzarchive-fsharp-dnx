using System;
using System.Collections.Generic;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Caching;
using Microsoft.Framework.Runtime.Compilation;

namespace YoloDev.Dnx.FSharp
{
  public class FSharpProjectCompiler : IProjectCompiler
  {
    readonly FSharpCompiler _compiler;

    public FSharpProjectCompiler(
      ICache cache,
      ICacheContextAccessor cacheContextAccessor,
      INamedCacheDependencyProvider namedCacheProvider,
      IAssemblyLoadContextFactory loadContextFactory,
      IFileWatcher watcher,
      IApplicationEnvironment environment,
      IServiceProvider services)
    {
      _compiler = new FSharpCompiler(
        cache,
        cacheContextAccessor,
        namedCacheProvider,
        loadContextFactory,
        watcher,
        environment,
        services);
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
      var incomingSourceReferences = export.SourceReferences;

      var compilationContext = _compiler.CompileProject(
        project,
        target,
        incomingReferences,
        incomingSourceReferences,
        resourcesResolver);

      if (compilationContext == null)
      {
        return null;
      }

      // Project reference
      return new FSharpProjectReference(compilationContext);
    }
  }
}
