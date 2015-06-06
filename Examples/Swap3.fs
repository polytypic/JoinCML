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

type Swap3<'x> =
  val ch: Ch<'x * Ch<'x * 'x>>
  new () = {ch = Ch ()}

module Swap3 =
  let swap (s3: Swap3<_>) x0 =
        s3.ch *<-+> fun cCh -> (x0, cCh)
    <|> s3.ch +&+ s3.ch ^-> fun ((x1, cCh1), (x2, cCh2)) ->
          cCh1 *<+ (x2, x0)
          cCh2 *<+ (x0, x1)
          (x1, x2)
