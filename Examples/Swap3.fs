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
//   sCh +&+ sCh
//
// The reason why JoinCML allows n-way rendezvous unlike CML is that the `join`
// (or `+&+`) combinator effectively allows joining an m-way rendezvous and a
// n-way rendezvous into a (m+n-1)-way rendezvous.  In this case we join two
// 2-way rendezvous into a 3-way rendezvous.

open JoinCML

type Swap3<'x> = Swap3 of Ch<'x * Ch<'x * 'x>>

module Swap3 =
  let create () = Swap3 (Ch.create ())

  let swap (Swap3 sCh) x0 =
    let client =
      sCh %<-~> fun cCh -> (x0, cCh)
    let leader =
      sCh +&+ sCh ^-> fun ((x1, cCh1), (x2, cCh2)) ->
                        cCh1 %<~ (x2, x0)
                        cCh2 %<~ (x0, x1)
                        (x1, x2)
    client <|> leader
