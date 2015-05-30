namespace JoinCML.Examples

open JoinCML

type Latch = {dec: Ch<unit>; is0: Ch<unit>}

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Latch =
  let create n =
    let is0 = Ch.create ()
    let dec = Ch.create ()
    let rec zero () = is0 *<- () |>>= zero
    and nonzero n =
      if 0 < n then dec *<- () |>>= fun () -> nonzero (n-1) else zero ()
    Async.Start (nonzero n)
    {is0 = is0; dec = dec}
  let dec c = ~~c.dec
  let is0 c = ~~c.is0
