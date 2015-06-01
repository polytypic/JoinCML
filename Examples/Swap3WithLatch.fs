namespace JoinCML.Examples

open JoinCML

type Swap3WithLatch<'x> = {sw3: Swap3<'x>; lot: Lottery; lat: Latch}

module Swap3WithLatch =
  let create n =
    {sw3 = Swap3.create ()
     lot = Lottery.create 3
     lat = Latch.create n}

  let swap s x =
        Latch.is0 s.lat ^=> None
    <|> Lottery.option s.lot (Latch.dec s.lat) -&+ Swap3.swap s.sw3 x ^-> Some
