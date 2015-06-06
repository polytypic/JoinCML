namespace JoinCML

type [<AbstractClass>] Alt<'x> =
  new () = {}
type Ch<'x> =
  inherit Alt<'x>
  new () = {}

module Ch =
  let give (xCh: Ch<'x>) (x: 'x) : Alt<unit> =
    failwith "XXX"
  let take (xCh: Ch<'x>) : Alt<'x> =
    failwith "XXX"

module Alt =
  let choice (xA1: Alt<'x>) (xA2: Alt<'x>) : Alt<'x> =
    failwith "XXX"
  let join (xA: Alt<'x>) (yA: Alt<'y>) : Alt<'x * 'y> =
    failwith "XXX"

  let withNack (uA2xAA: Alt<unit> -> #Alt<'x>) : Alt<'x> =
    failwith "XXX"

  let afterAsync (x2yA: 'x -> Async<'y>) (xA: Alt<'x>) : Alt<'y> =
    failwith "XXX"

  let sync (xA: Alt<'x>) : Async<'x> =
    failwith "XXX"
