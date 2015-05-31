namespace JoinCML.Examples

// This example implements a synchronous channel with guarded receive.  This is
// impossible to implement in CML.
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

type GuardedGive<'x> = {nack: Alt<unit>; value: 'x; replyCh: Ch<unit>}
type GuardedPick<'x> = {nack: Alt<unit>; guard: 'x -> option<Alt<unit>>}

type GuardedCh<'x> = {giveCh: Ch<GuardedGive<'x>>; pickCh: Ch<GuardedPick<'x>>}

module GuardedCh =
  let mkReqAlt (reqCh: Ch<'req>)
               (reqs: LinkedList<'req>)
               (nackOf: 'req -> Alt<unit>) =
    let nacksAlt =
      nodes reqs
      |> List.map (fun node ->
         nackOf node.Value
         |> Alt.after (fun () ->
            reqs.Remove node))
      |> Alt.choose
    let newReqAlt =
      ~~reqCh
      |> Alt.after (fun req -> reqs.AddLast (LinkedListNode<_> (req)))
    nacksAlt <|> newReqAlt

  let create () : GuardedCh<'x> =
    let giveCh = Ch.create ()
    let pickCh = Ch.create ()
    let gives = LinkedList<GuardedGive<'x>> ()
    let picks = LinkedList<GuardedPick<'x>> ()
    let rec server () =
      let reqAlts =
        (mkReqAlt giveCh gives <| fun r -> r.nack) <|>
        (mkReqAlt pickCh picks <| fun r -> r.nack)
      let ts = nodes picks
      let gs = nodes gives
      gs
      |> List.collect (fun giveNode ->
         let give = giveNode.Value
         ts
         |> List.choose (fun pickNode ->
            let pick = pickNode.Value
            pick.guard give.value
            |> Option.map (fun pickAlt ->
               give.replyCh *<- () <&> pickAlt
               |> Alt.after (fun ((), ()) ->
                  gives.Remove giveNode
                  picks.Remove pickNode))))
      |> subsets
      |> List.map (function
          | [] -> reqAlts
          | alt::alts ->
            alts
            |> List.foldFrom alt (fun all alt ->
               all <&> alt
               |> Alt.after ignore))
      |> Alt.choose
      |>>= server
    server () |> Async.Start
    {giveCh = giveCh; pickCh = pickCh}

  let give guardedCh value =
    Alt.requestWithNack guardedCh.giveCh <| fun nack replyCh ->
    {nack = nack; value = value; replyCh = replyCh}

  let pick guard guardedCh =
    Alt.requestWithNack guardedCh.pickCh <| fun nack replyCh ->
    {nack = nack; guard = guard >> Option.map (fun y -> replyCh *<- y)}