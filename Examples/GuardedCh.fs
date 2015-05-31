namespace JoinCML.Examples

open System.Collections.Generic
open JoinCML

type GuardedGive<'x> = {nack: Alt<unit>; value: 'x; replyCh: Ch<unit>}
type GuardedTake<'x> = {nack: Alt<unit>; guard: 'x -> bool; replyCh: Ch<'x>}

type GuardedCh<'x> = {giveCh: Ch<GuardedGive<'x>>; takeCh: Ch<GuardedTake<'x>>}

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
    let takeCh = Ch.create ()
    let gives = LinkedList<GuardedGive<'x>> ()
    let takes = LinkedList<GuardedTake<'x>> ()
    let rec server () =
      let reqAlts =
        (mkReqAlt giveCh gives <| fun r -> r.nack) <|>
        (mkReqAlt takeCh takes <| fun r -> r.nack)
      let ts = nodes takes
      let gs = nodes gives
      gs
      |> List.collect (fun giveNode ->
         let give = giveNode.Value
         ts
         |> List.choose (fun takeNode ->
            let take = takeNode.Value
            if take.guard give.value then
              give.replyCh *<- () <&>
              take.replyCh *<- give.value
              |> Alt.after (fun ((), ()) ->
                 gives.Remove giveNode
                 takes.Remove takeNode)
              |> Some
            else
              None))
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
    {giveCh = giveCh; takeCh = takeCh}

  let give guardedCh value =
    Alt.requestWithNack guardedCh.giveCh <| fun nack replyCh ->
    {nack = nack; value = value; replyCh = replyCh}

  let take guardedCh guard =
    Alt.requestWithNack guardedCh.takeCh <| fun nack replyCh ->
    {nack = nack; guard = guard; replyCh = replyCh}
