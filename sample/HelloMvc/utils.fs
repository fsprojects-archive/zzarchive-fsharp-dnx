[<AutoOpen>]
module internal Utils

let inline debug () =
  if System.Diagnostics.Debugger.IsAttached then System.Diagnostics.Debugger.Break ()
  else System.Diagnostics.Debugger.Launch () |> ignore
