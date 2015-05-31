namespace JoinCML.Examples

open JoinCML

type GuardedCh<'x>

module GuardedCh =
  val create: unit -> GuardedCh<'x>
  val give: GuardedCh<'x> -> 'x -> Alt<unit>
  val pick: ('x -> option<'y>) -> GuardedCh<'x> -> Alt<'y>
