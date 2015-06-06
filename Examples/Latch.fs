namespace JoinCML.Examples

open JoinCML

type Latch =
  val dec: Ch<unit>
  val is0: Ch<unit>
  new n as l = {dec = Ch (); is0 = Ch ()} then
    if n < 0 then failwithf "Latch %d" n
    let rec zero () = l.is0 *<- () |>>= zero
    and nonzero = function 0 -> zero ()
                         | n -> l.dec *<- () |>>= fun () -> nonzero <| n-1
    nonzero n |> Async.Start

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Latch =
  let dec (l: Latch) = l.dec :> Alt<_>
  let is0 (l: Latch) = l.is0 :> Alt<_>
