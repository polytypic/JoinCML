namespace JoinCML.Examples

open JoinCML

type Latch =
  inherit AltDelegate<unit>
  val dec: Ch<unit>
  new n as l =
    let is0 = Ch () in
    {inherit AltDelegate<_> (is0); dec = Ch ()} then
    if n < 0 then failwithf "Latch %d" n
    let rec zero () = is0 *<- () |>>= zero
    and nonzero = function 0 -> zero ()
                         | n -> l.dec *<- () |>>= fun () -> nonzero <| n-1
    nonzero n |> Async.Start

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Latch =
  let dec (l: Latch) = l.dec :> Alt<_>
