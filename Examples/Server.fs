namespace JoinCML.Examples

open System.Collections.Generic
open JoinCML

module Server =
  let serveAny initial queryAlt nackOf replyTo : Async<unit> =
    let queries = LinkedList<'query> ()
    let rec loop state =
      let newQueryAlt =
        queryAlt ^-> newLinkedListNode *>> queries.AddLast
      let nodes = nodes queries
      let nacksAlt =
        nodes
        |> List.map *<| fun node ->
             nackOf node.Value ^-> fun () -> queries.Remove node
        |> Alt.choose
      powerset nodes
      |> List.map *<| function
          | [] ->
            (newQueryAlt <|> nacksAlt) ^->. state
          | node::nodes as subset ->
            nodes
            |> List.foldFrom *<| replyTo node.Value state
                *<| fun (replyAlt, state) node ->
                      let (replyAlt', state) = replyTo node.Value state
                      (replyAlt -&- replyAlt', state)
            |> fun (replyAlt, state) ->
                 replyAlt ^-> fun () ->
                   subset |> List.iter queries.Remove
                   state
      |> Alt.choose
      |>>= loop
    loop initial
