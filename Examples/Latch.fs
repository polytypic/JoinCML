namespace JoinCML.Examples

open JoinCML

type Latch =
  val dec: Ch<unit>
  val is0: Ch<unit>
  new n as l = {dec = Ch (); is0 = Ch ()} then
    let rec zero () = l.is0 *<- () |>>= zero
    and nonzero n =
      if 0 < n then l.dec *<- () |>>= fun () -> nonzero <| n-1 else zero ()
    nonzero n |> Async.Start

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Latch =
  let dec (l: Latch) = l.dec :> Alt<_>
  let is0 (l: Latch) = l.is0 :> Alt<_>
