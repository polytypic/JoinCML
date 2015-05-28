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

/// This module implements or encodes a variation of core asynchronous
/// join-calculus using JoinCML.
[<AutoOpen>]
module Join =
  /// Represents an asynchronous channel.
  type AsyncCh<'x> =
    | AsyncCh of Ch<'x>

    /// Join pattern to take and bind a value from the channel.
    static member (~+.) (AsyncCh c) = ~~c |> Alt.after (fun x x2y -> x2y x)

    /// Join pattern to take and ignore a value from the channel.
    static member (~-.) (AsyncCh c) = ~~c |> Alt.after (fun _ y -> y)

  /// Creates a new asynchronous channel.
  let asyncCh () = AsyncCh <| Ch.create ()

  /// Sends a value to the asynchronous channel.
  let (<~) (AsyncCh c) x = c +<- x

  /// Join of two join patterns.
  let (.&.) lhs rhs =
    lhs <&> rhs |> Alt.after (fun (xyz2yz, yz2z) xyz -> yz2z (xyz2yz xyz))

  /// Action of join pattern.
  let (|~>) xy2y xy =
    xy2y |> Alt.after (fun xy2y -> xy2y xy)

  // NOTE: The combinators `+.`, `-.`, `.&.` and `|~>` basically use a form of
  // functional unparsing (http://www.brics.dk/RS/98/12/BRICS-RS-98-12.pdf) to
  // implement a composable notation that allows one to select which values to
  // bind, `+.`, and which values to ignore, `-.`.

  /// Represents a synchronous or a call-reply channel.
  type SyncCh<'x, 'y> =
    | SyncCh of Ch<'x * Ch<'y>>

    /// Join pattern to take and bind a value and the reply address from the
    /// channel.
    static member (~+.) (SyncCh c) =
      ~~c |> Alt.after (fun (x, r) xr2y -> xr2y x r)

    /// Join pattern to take and ignore a value, but bind the reply address from
    /// the channel.
    static member (~-.) (SyncCh c) =
      ~~c |> Alt.after (fun (_, r) r2y -> r2y r)

  /// Creates a call-reply channel.
  let syncCh () : SyncCh<'x, 'y> = SyncCh <| Ch.create ()

  /// Call and wait for reply.
  let call (SyncCh x2y: SyncCh<'x, 'y>) (x: 'x) : Async<'y> = async {
    let yCh = Ch.create ()
    x2y +<- (x, yCh)
    return! ~~yCh |> Alt.sync
  }

  /// Reply to a call.
  let replyTo r y = r +<- y

  /// Spawns a process to repeatedly match a set of join patterns.
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
    join [-.none .&. +.put |~> fun x putR -> some <~ x; replyTo putR ()
          +.some .&. -.get |~> fun x getR -> none <~ (); replyTo getR x]
    {put = call put; get = call get}
