namespace JoinCML.Examples.JoinCalculusStyle

// The examples in this file are translations of various examples based on
// join-calculus.  JoinCML seems to be able to encode core join-calculus where
// arbitrary patterns or guards are not used.
//
// It seems that it would be possible to extend JoinCML to allow arbitrary
// guards, but that would require adding a new type and guard combinator so that
// guards could be added and combined to be evaluated just before post
// synchronization actions.
//
// Note that the examples in this file do not necessarily represent the best
// ways to use JoinCML.  For example, the `OnePlaceBuffer` implemented below is
// essentially a weaker variation of `MVar`.

open JoinCML

/// This module implements a core asynchonous join-calculus using JoinCML.
[<AutoOpen>]
module Join =
  type AsyncCh<'x> = AsyncCh of Ch<'x>
  let asyncCh () = AsyncCh <| Ch.create ()
  let (<~) (AsyncCh c) x = c +<- x
  let (!!) (AsyncCh c) = ~~c

  type SyncCh<'x, 'y> = AsyncCh<'x * AsyncCh<'y>>
  let syncCh () : SyncCh<'x, 'y> = asyncCh ()
  let call (x2y: SyncCh<'x, 'y>) (x: 'x) : Async<'y> = async {
    let yCh = asyncCh ()
    x2y <~ (x, yCh)
    return! Alt.sync !!yCh
  }

  let (|~>) j f = j |> Alt.after f

  let join = function
    | [] -> ()
    | clause::clauses ->
      let alt = List.fold (<|>) clause clauses
      let rec forever () = alt |>>= forever
      forever () |> Async.Start

module OnePlaceBuffer =
  type T<'x> = {get: unit -> Async<'x>; put: 'x -> Async<unit>}

  let create () =
    let put = syncCh ()
    let get = syncCh ()
    let none = asyncCh ()
    let some = asyncCh ()
    none <~ ()
    join [!!none <&> !!put |~> fun ((), (x, putR)) -> some <~ x ; putR <~ ()
          !!some <&> !!get |~> fun (x, ((), getR)) -> none <~ () ; getR <~ x]
    {put = call put; get = call get}
