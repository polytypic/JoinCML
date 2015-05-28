namespace JoinCML.Examples

open JoinCML

type MVar<'x>

module MVar =
  val create: unit -> MVar<'x>
  val createFull: 'x -> MVar<'x>
  val take: MVar<'x> -> Alt<'x>
  val fill: MVar<'x> -> 'x -> Alt<unit>
