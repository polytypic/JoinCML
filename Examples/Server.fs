namespace JoinCML.Examples

open System.Collections.Generic
open JoinCML

module Server =
  let serveAny (initial: 'state)
               (requestAlt: Alt<'request>)
               (nackOf: 'request -> Alt<unit>)
               (replyTo: 'request -> 'state -> Alt<unit> * 'state) : Async<unit> =
    let requests = LinkedList<'request> ()
    let rec loop state =
      let newRequestAlt =
        requestAlt
        |> Alt.after (fun request ->
           requests.AddLast (LinkedListNode<_> (request))
           state)
      let nodes = nodes requests
      let nacksAlt =
        nodes
        |> List.map (fun node ->
           nackOf node.Value
           |> Alt.after (fun () ->
              requests.Remove node
              state))
        |> Alt.choose
      subsets nodes
      |> List.map (function
          | [] ->
            newRequestAlt <|> nacksAlt
          | (node::nodes) as subset ->
            nodes
            |> List.foldFrom (replyTo node.Value state) (fun (replyAlt, state) node ->
               let (replyAlt', state) = replyTo node.Value state
               (replyAlt <&> replyAlt' |> Alt.after ignore,
                state))
            |> fun (replyAlt, state) ->
                 replyAlt
                 |> Alt.after (fun () ->
                    subset |> List.iter requests.Remove
                    state))
      |> Alt.choose |>>= loop
    loop initial
