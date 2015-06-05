namespace JoinCML

type [<AbstractClass>] Alt<'x> = class
    internal new () = {}
  end
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

  // Non-primitives:

  let prepare (u2xA: unit -> #Alt<'x>) : Alt<'x> =
    withNack (ignore >> u2xA)

  let once x =
    let xCh = Ch ()
    Ch.give xCh x |> sync |> Async.Start
    Ch.take xCh

  let always x = prepare <| fun () -> once x

  let never<'x> : Alt<'x> = Ch () |> Ch.take

  let choose xAs = prepare <| fun () ->
    match Seq.toList xAs with
     | [] -> never
     | xA::xAs -> List.fold choice xA xAs

module Convenience =
  let (>>=) xA x2yA = async.Bind (xA, x2yA)
  let result x = async.Return x

  let (^=>)  xA x2yA = Alt.afterAsync x2yA xA
  let (^=>.) xA   yA = xA ^=> fun _ -> yA
  let (^->)  xA x2y  = xA ^=> (x2y >> result)
  let (^->.) xA   y  = xA ^-> fun _ -> y

  let ( *<- ) xCh x = Ch.give xCh x
  let ( *<+ ) xCh x = xCh *<- x |> Alt.sync |> Async.Start
  let ( *<+-> ) queryCh queryFromReplyChAndNack = Alt.withNack <| fun nack ->
    let replyCh = Ch ()
    queryCh *<+ queryFromReplyChAndNack replyCh nack
    replyCh
  let ( *<-+> ) queryCh queryFromReplyCh = Alt.prepare <| fun () ->
    let replyCh = Ch ()
    queryCh *<- queryFromReplyCh replyCh ^=>. Alt.sync replyCh

  let (<|>) xA1 xA2 = Alt.choice xA1 xA2

  let (+&+) xA yA = Alt.join xA yA
  let (-&+) xA yA = xA +&+ yA ^-> snd
  let (+&-) xA yA = xA +&+ yA ^-> fst
  let (-&-) xA yA = xA +&+ yA ^->. ()

  let (<*>) x2yA xA = x2yA +&+ xA ^-> fun (x2y, x) -> x2y x

  let (|>>=) xA x2yA = xA |> Alt.sync >>= x2yA

  type AsyncBuilder with
    member t.Bind (xA, x2yA) = xA |>>= x2yA
    member t.ReturnFrom xA = Alt.sync xA
