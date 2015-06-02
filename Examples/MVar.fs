namespace JoinCML.Examples

// This implements a Concurrent Haskell style MVar:
//
//   https://hackage.haskell.org/package/base-4.8.0.0/docs/Control-Concurrent-MVar.html
//
// rather than a CML style mvar:
//
//   http://cml.cs.uchicago.edu/pages/sync-var.html
//
// The difference between the two is that Concurrent Haskell style MVar does not
// treat an attempt to put to a full MVar as an error.
//
// MVars in JoinCML are more expressive than in both Concurrent Haskell and CML,
// because operations on MVars can be used in both choices and joins.
//
// The implementation is very simple.  A process maintains the state of the MVar
// and responds only to either `take` or `fill` operations.
//
// The same implementation also works in CML, but CML events cannot be used in
// joins.  This means that, for example, an operation to take two values from
// two different MVars cannot be formed compositionally in CML and would need to
// be implemented as a separate primitive operation.

open JoinCML

type MVar<'x> = {fill: Ch<'x>; take: Ch<'x>}

module MVar =
  let rec full x mv = mv.take *<- x |>>= fun () -> empty mv
  and empty mv = mv.fill |>>= fun x -> full x mv

  let start state =
    let mv = {fill = Ch.create (); take = Ch.create ()}
    state mv |> Async.Start
    mv

  let create () = start empty
  let createFull x = start (full x)

  let take mv = mv.take :> Alt<_>
  let fill mv x = mv.fill *<- x
