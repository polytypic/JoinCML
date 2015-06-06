namespace JoinCML.Examples

open JoinCML

type Swap3WithLatch<'x> =
  val sw3: Swap3<'x>
  val lot: Lottery
  val lat: Latch
  new n = {sw3 = Swap3 (); lot = Lottery 3; lat = Latch n}

module Swap3WithLatch =
  let swap (s: Swap3WithLatch<_>) x =
        s.lat ^->. None
    <|> Lottery.option s.lot *<| Latch.dec s.lat -&+ Swap3.swap s.sw3 x ^-> Some
