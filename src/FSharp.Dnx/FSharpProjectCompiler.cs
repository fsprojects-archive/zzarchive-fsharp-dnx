using System;
using System.Collections.Generic;
using Microsoft.Dnx.Compilation;
using Microsoft.Dnx.Compilation.Caching;
using Microsoft.Extensions.PlatformAbstractions;

namespace FSharp.Dnx
{
  public class FSharpProjectCompiler : IProjectCompiler
  {
    readonly FSharpCompiler _compiler;

    public FSharpProjectCompiler(
      ICache cache,
      ICacheContextAccessor cacheContextAccessor,
      INamedCacheDependencyProvider namedCacheProvider,
      IApplicationEnvironment environment,
      IServiceProvider services)
    {
      _compiler = new FSharpCompiler(
        cache,
        cacheContextAccessor,
        namedCacheProvider,
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
