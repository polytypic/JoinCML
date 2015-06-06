namespace JoinCML.Examples

open JoinCML

type Swap3<'x> =
  new: unit -> Swap3<'x>

module Swap3 =
  val swap: Swap3<'x> -> 'x -> Alt<'x * 'x>
