namespace HelloFSharp

open Microsoft.Extensions.PlatformAbstractions

type Program (env: IApplicationEnvironment) =
  member x.Main () =
    printfn "Hello from F#, running on %s v%s" env.RuntimeFramework.Identifier (env.RuntimeFramework.Version.ToString ())
