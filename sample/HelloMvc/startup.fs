namespace HelloMvc

open System.Reflection
open Microsoft.AspNet.Builder
open Microsoft.AspNet.Hosting
open Microsoft.AspNet.FileProviders
open Microsoft.AspNet.Mvc.Razor
open Microsoft.Extensions.DependencyInjection

type Startup() =

  // Set up application services
  member public x.ConfigureServices (services: IServiceCollection) =
    services.AddMvc () |> ignore

    services.Configure(fun (options: RazorViewEngineOptions) ->
        // Base namespace matches the resources added to the assembly from the EmbeddedResources folder.
        options.FileProvider <- new EmbeddedFileProvider(
            (x.GetType().GetTypeInfo().Assembly),
            "HelloMvc.EmbeddedResources")
    ) |> ignore

    ()

  // Configure pipeline
  member public x.Configure (app: IApplicationBuilder) =
    //loggerFactory.AddConsole (fun (name, logLevel) -> true)
    app.UseDeveloperExceptionPage () |> ignore

    app.UseStaticFiles () |> ignore

    app.UseMvc (fun routes ->
        routes.MapRoute (name = "default", template = "{controller=Home}/{action=Index}/{id?}")
        |> ignore)
    |> ignore

type Program() =

    // Set up entry point
    static member Main(args) =
        let hostingConfiguration = WebApplicationConfiguration.GetDefault args
        let application =
            WebApplicationBuilder()
               .UseConfiguration(hostingConfiguration)
                .UseStartup<Startup>()
                .Build()

        application.Run()
