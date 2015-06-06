namespace JoinCML.Examples

open JoinCML

type GuardedCh<'x> =
  new: unit -> GuardedCh<'x>

module GuardedCh =
  val give: GuardedCh<'x> -> 'x -> Alt<unit>
  val pick: ('x -> option<'y>) -> GuardedCh<'x> -> Alt<'y>
  val take: GuardedCh<'x> -> Alt<'x>
