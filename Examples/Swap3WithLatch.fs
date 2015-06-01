namespace JoinCML.Examples

open JoinCML

type Swap3WithLatch<'x> = {sw3: Swap3<'x>; lot: Lottery; lat: Latch}

module Swap3WithLatch =
  let create n =
    {sw3 = Swap3.create ()
     lot = Lottery.create 3
     lat = Latch.create n}
  let swap {sw3 = sw3; lot = lot; lat = lat} x =
    Latch.is0 lat                                           ^=> None <|>
    Swap3.swap sw3 x +&- Lottery.option lot (Latch.dec lat) ^-> Some
