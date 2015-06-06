namespace JoinCML.Examples

open JoinCML

type Latch = {dec: Alt<unit>; is0: Alt<unit>}

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Latch =
  let create n =
    let is0 = Ch ()
    let dec = Ch ()
    let rec zero () = is0 *<- () |>>= zero
    and nonzero n =
      if 0 < n then dec *<- () |>>= fun () -> nonzero (n-1) else zero ()
    nonzero n |> Async.Start
    {is0 = is0; dec = dec}

  let dec c = c.dec
  let is0 c = c.is0
