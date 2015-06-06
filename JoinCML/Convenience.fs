namespace JoinCML

open JoinCML

module Convenience =
  let result x = async.Return x
  let (>>=) xA x2yA = async.Bind (xA, x2yA)
  let (>>-) xA x2y = xA >>= (x2y >> result)

  let (^=>)  xA x2yA = Alt.afterAsync x2yA xA
  let (^=>.) xA   yA = xA ^=> fun _ -> yA
  let (^->)  xA x2y  = xA ^=> (x2y >> result)
  let (^->.) xA   y  = xA ^-> fun _ -> y

  module Alt =
    let start xA = xA |> Alt.sync |> Async.Start

    let wrapAbort abort xAlt = Alt.withNack <| fun nack ->
      nack ^-> abort |> start
      xAlt

    let prepare (u2xA: unit -> #Alt<'x>) : Alt<'x> =
      Alt.withNack (ignore >> u2xA)

    let once x =
      let xCh = Ch ()
      Ch.give xCh x |> start
      xCh :> Alt<_>

    let always x = prepare <| fun () -> once x

    let never<'x> = Ch () :> Alt<'x>

    let choose xAs = prepare <| fun () ->
      match Seq.toList xAs with
       | [] -> never
       | xA::xAs -> List.fold Alt.choice xA xAs

  module Ch =
    let take (xCh: Ch<_>) = xCh :> Alt<_>
    let send xCh x = Ch.give xCh x |> Alt.start

  let ( *<- ) xCh x = Ch.give xCh x
  let ( *<+ ) xCh x = Ch.send xCh x
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

  let timeOutMillis ms = Alt.withNack <| fun nack ->
    let uCh = Ch ()
    let tokenSrc = new System.Threading.CancellationTokenSource ()
    nack ^-> tokenSrc.Cancel |> Alt.start
    Async.Sleep ms >>- Ch.send uCh |> Async.Start
    uCh
