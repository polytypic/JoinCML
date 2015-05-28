namespace JoinCML.Examples

// This example implements a 3-way swap as an alternative.  It is impossible to
// express 3-way swap as an event in the original CML.
//
// This implementation is almost the same as with Transactional Events (TE):
//
//   http://www.cs.rit.edu/~mtf/research/tx-events/ICFP06/icfp06.pdf
//
// The key difference between the TE version and this JoinCML version is that
// the leader in the TE implementation takes two messages from two clients
// sequentially:
//
//   do { (x2, replyCh2) <- recvEvt ch
//      ; (x3, replyCh3) <- recvEvt ch
//
// while the JoinCML implementation takes two messages from two clients
// simultaneously:
//
//   ~~sCh <&> ~~sCh
//
// The reason why JoinCML allows n-way rendezvous unlike CML is that the `join`
// (or `<&>`) combinator effectively allows joining an n-way rendezvous and a
// 2-way rendezvous into a (n+1)-way rendezvous.

open JoinCML

type Swap3<'x> = Swap3 of Ch<'x * Ch<'x * 'x>>

module Swap3 =
  let create () = Swap3 (Ch.create ())

  let swap (Swap3 sCh: Swap3<_>) x0 =
    let client = Alt.before <| fun () ->
      let cCh = Ch.create ()
      sCh *<- (x0, cCh)
      |> Alt.afterAsync (fun () ->
         Alt.sync ~~cCh)
    let leader =
      ~~sCh <&> ~~sCh
      |> Alt.afterAsync (fun ((x1, cCh1), (x2, cCh2)) ->
         cCh1 *<- (x2, x0) <&> cCh2 *<- (x0, x1)
         |> Alt.after (fun _ ->
            (x1, x2))
         |> Alt.sync)
    client <|> leader
