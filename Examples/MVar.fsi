namespace JoinCML.Examples

open JoinCML

type MVar<'x> =
  new: unit -> MVar<'x>
  new: 'x -> MVar<'x>

module MVar =
  val take: MVar<'x> -> Alt<'x>
  val fill: MVar<'x> -> 'x -> Alt<unit>
