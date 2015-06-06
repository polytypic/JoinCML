namespace JoinCML.Examples

open JoinCML

type MVar<'x> =
  inherit AltDelegate<'x>
  new: unit -> MVar<'x>
  new: 'x -> MVar<'x>

module MVar =
  val fill: MVar<'x> -> 'x -> Alt<unit>
