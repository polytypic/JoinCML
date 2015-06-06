namespace JoinCML.Examples

open JoinCML

type Swap3WithLatch<'x> =
  new: int -> Swap3WithLatch<'x>

module Swap3WithLatch =
  val swap: Swap3WithLatch<'x> -> 'x -> Alt<option<'x * 'x>>
