namespace HelloMvc

open System
open Microsoft.AspNet.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.AspNet.Hosting

type Startup(env: IHostingEnvironment) =

  // Set up application services
  member public x.ConfigureServices (services: IServiceCollection) =
    let mvcBuilder = services.AddMvc ()

    let viewAssemblies = 
      [ "HelloMvc.Views" ]
      |> List.map Reflection.Assembly.Load
      |> Array.ofList
    
    mvcBuilder.AddPrecompiledRazorViews viewAssemblies |> ignore
    //Microsoft.Extensions.DependencyInjection.MvcRazorMvcBuilderExtensions.AddPrecompiledRazorViews (mvcBuilder, System.Reflection.Assembly.Load "HelloMvc.Views") |> ignore
    ()

  // Configure pipeline
  member public x.Configure (app: IApplicationBuilder, loggerFactory: ILoggerFactory) =
    //loggerFactory.AddConsole (fun (name, logLevel) -> true)
    app.UseDeveloperExceptionPage () |> ignore

    app.UseStaticFiles () |> ignore

    app.UseMvc (fun routes ->
      routes.MapRoute (name = "default", template = "{controller=Home}/{action=Index}/{id?}") |> ignore

      ()) |> ignore
