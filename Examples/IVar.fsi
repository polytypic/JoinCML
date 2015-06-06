namespace JoinCML.Examples

open JoinCML

type IVar<'x> =
  inherit AltDelegate<'x>
  new: unit -> IVar<'x>
  new: 'x -> IVar<'x>

module IVar =
  val fill: IVar<'x> -> 'x -> unit
  val fillFailure: IVar<'x> -> exn -> unit
