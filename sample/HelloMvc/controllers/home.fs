namespace HelloMvc.Controllers

open Microsoft.AspNet.Mvc

type public HomeController() =
  inherit Controller()

  member public x.Index () =
    //x.Content "Hello from F#"
    x.View ()
