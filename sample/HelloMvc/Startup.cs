using System;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.AspNet.Hosting;

namespace HelloMvc.CS
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseErrorPage();
      app.UseStaticFiles();
      app.UseMvc(routes => {
        routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
