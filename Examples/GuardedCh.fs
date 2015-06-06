namespace JoinCML.Examples

// This example implements a synchronous channel with guarded receive.  This is
// impossible to implement in CML.  This implementation approach also does not
// work with Conjoined Events:
//
//   https://kar.kent.ac.uk/33878/1/Conjoined.pdf
//
// as in the Haskell CHP library:
//
//   http://www.cs.kent.ac.uk/projects/ofa/chp/
//
// because Conjoined Events/CHP does not include `withNack`, which seems
// essential for supporting user defined synchronization abstractions.  Note
// that it should not be difficult to implement a layer implementing events with
// nack support as "poc.cml" shows for Clojure core.async:
//
//   https://github.com/VesaKarvonen/poc.cml
//
// The basic idea behind the implementation is simple.  The internal server of
// the guarded channel maintains lists of both givers and takers.  At each step
// the server tries to match givers to takers.  The powerset of joins of
// matching pairs is offered for synchronization.
//
// The existence of a guarded channel abstraction essentially shows that
// join-calculus with pattern matching can be implemented in JoinCML.

open System.Collections.Generic
open JoinCML

type GuardedGive<'x> = {value: 'x; replyCh: Ch<unit>; nack: Alt<unit>}
type GuardedPick<'x> = {guard: 'x -> option<Alt<unit>>; nack: Alt<unit>}

type GuardedCh<'x> =
  val giveCh: Ch<GuardedGive<'x>>
  val pickCh: Ch<GuardedPick<'x>>
  new () as gCh = {giveCh = Ch (); pickCh = Ch ()} then
    let mkQueryAlt (queryCh: Ch<'q>)
                   (queries: LinkedList<'q>)
                   (nackOf: 'q -> Alt<unit>) =
          nodes queries
          |> List.map *<| fun node ->
               nackOf node.Value ^-> fun () -> queries.Remove node
          |> Alt.choose
      <|> queryCh ^-> newLinkedListNode *>> queries.AddLast
    let gives = LinkedList<GuardedGive<'x>> ()
    let picks = LinkedList<GuardedPick<'x>> ()
    let rec server () =
      let queryAlts =
            mkQueryAlt gCh.giveCh gives *<| fun r -> r.nack
        <|> mkQueryAlt gCh.pickCh picks *<| fun r -> r.nack
      let pickNodes = nodes picks
      let giveNodes = nodes gives
      giveNodes
      |> List.collect *<| fun giveNode ->
           let give = giveNode.Value
           pickNodes
           |> List.choose *<| fun pickNode ->
                let pick = pickNode.Value
                pick.guard give.value
                |> Option.map *<| fun pickAlt ->
                    give.replyCh *<- () -&- pickAlt ^-> fun () ->
                      gives.Remove giveNode
                      picks.Remove pickNode
      |> powerset
      |> List.map *<| function
          | [] -> queryAlts
          | alt::alts -> List.fold (-&-) alt alts
      |> Alt.choose
      |>>= server
    server () |> Async.Start

module GuardedCh =

  let give (guardedCh: GuardedCh<_>) value =
    guardedCh.giveCh *<+-> fun replyCh nack ->
      {value = value; replyCh = replyCh; nack = nack}

  let pick guard (guardedCh: GuardedCh<_>) =
    guardedCh.pickCh *<+-> fun replyCh nack ->
      {guard = guard >> Option.map *<| Ch.give replyCh; nack = nack}
