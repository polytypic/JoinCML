namespace JoinCML.Examples

open JoinCML

type Swap3WithLatch<'x>

module Swap3WithLatch =
  val create: int -> Swap3WithLatch<'x>
  val swap: Swap3WithLatch<'x> -> 'x -> Alt<option<'x * 'x>>
