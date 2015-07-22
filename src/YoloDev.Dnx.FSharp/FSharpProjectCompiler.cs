using System;
using System.Collections.Generic;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Caching;
using Microsoft.Framework.Runtime.Compilation;
using Microsoft.Framework.Runtime.Infrastructure;

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
      CompilationProjectContext projectContext,
      Func<LibraryExport> referenceResolver,
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
        projectContext,
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