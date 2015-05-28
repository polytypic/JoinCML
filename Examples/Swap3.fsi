namespace JoinCML.Examples

open JoinCML

type Swap3<'x>

module Swap3 =
  val create: unit -> Swap3<'x>
  val swap: Swap3<'x> -> 'x -> Alt<'x * 'x>
