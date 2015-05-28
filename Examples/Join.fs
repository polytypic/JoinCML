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

/// This module implements a variation of core asynchronous join-calculus using
/// JoinCML.
[<AutoOpen>]
module Join =
  type AsyncCh<'x> =
    | AsyncCh of Ch<'x>
    static member (~+.) (AsyncCh c) = ~~c |> Alt.after (fun x x2y -> x2y x)
    static member (~-.) (AsyncCh c) = ~~c |> Alt.after (fun _ y -> y)
  let asyncCh () = AsyncCh <| Ch.create ()

  let (<~) (AsyncCh c) x = c +<- x

  let (.&.) lhs rhs =
    lhs <&> rhs |> Alt.after (fun (xyz2yz, yz2z) xyz -> yz2z (xyz2yz xyz))
  let (|~>) xy2y xy =
    xy2y |> Alt.after (fun xy2y -> xy2y xy)

  type SyncCh<'x, 'y> =
    | SyncCh of Ch<'x * Ch<'y>>
    static member (~+.) (SyncCh c) =
      ~~c |> Alt.after (fun (x, r) xr2y -> xr2y x r)
    static member (~-.) (SyncCh c) =
      ~~c |> Alt.after (fun (_, r) r2y -> r2y r)

  let syncCh () : SyncCh<'x, 'y> = SyncCh <| Ch.create ()
  let call (SyncCh x2y: SyncCh<'x, 'y>) (x: 'x) : Async<'y> = async {
    let yCh = Ch.create ()
    x2y +<- (x, yCh)
    return! ~~yCh |> Alt.sync
  }
  let returnTo r y = r +<- y

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
    join [-.none .&. +.put |~> fun x putR -> some <~ x; returnTo putR ()
          +.some .&. -.get |~> fun x getR -> none <~ (); returnTo getR x]
    {put = call put; get = call get}
