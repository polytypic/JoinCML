namespace JoinCML

type [<AbstractClass>] Alt<'x> = class end
type [<AbstractClass>] Ch<'x> = inherit Alt<'x>

module Ch =
  let create () : Ch<'x> =
    failwith "XXX"
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

  // Non-primitives:

  let before (u2xA: unit -> #Alt<'x>) : Alt<'x> =
    withNack (ignore >> u2xA)

  let once (x: 'x) : Alt<'x> =
    let xCh = Ch.create ()
    Ch.give xCh x |> sync |> Async.Start
    Ch.take xCh
    
  let never<'x> : Alt<'x> = Ch.create () |> Ch.take

  let choose xAs = before <| fun () ->
    match Seq.toList xAs with
     | [] -> never
     | xA::xAs -> List.fold choice xA xAs

module Convenience =
  let (-->) x xCh = Ch.give xCh x
  let (-~>) x xCh = x --> xCh |> Alt.sync |> Async.Start
  let (|>~) xA x2yA = Alt.afterAsync x2yA xA
  let (|>-) xA x2y = xA |>~ (x2y >> async.Return)
  let (|>=) xA y = xA |>- fun _ -> y
  let (<|>) xA1 xA2 = Alt.choice xA1 xA2
  let (<&>) xA yA = Alt.join xA yA
  let (.&>) xA yA = Alt.join xA yA |>- fun (_, y) -> y
  let (<&.) xA yA = Alt.join xA yA |>- fun (x, _) -> x
  let (.&.) xA yA = Alt.join xA yA |>- fun (_, _) -> ()
  let (<*>) x2yA xA = Alt.join x2yA xA |>- fun (x2y, x) -> x2y x
  let (>>=) xA x2yA = async.Bind (xA, x2yA)
  let result x = async.Return x
  let (|>>=) xA x2yA = xA |> Alt.sync >>= x2yA
