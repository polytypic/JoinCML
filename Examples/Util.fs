namespace JoinCML.Examples

open System.Collections.Generic
open JoinCML

module List =
  let foldFrom s f xs = List.fold f s xs

[<AutoOpen>]
module internal Util =
  let stable x _ = x

  let newLinkedListNode (x: 'x) = LinkedListNode<'x> (x)

  let nodes (lst: LinkedList<_>) =
    let rec lp ns = function
      | null -> ns
      | (n: LinkedListNode<_>) ->
        lp (n::ns) n.Next
    lp [] lst.First

  let rec powerset = function
    | [] -> [[]]
    | x::xs ->
      let xss = powerset xs
      List.fold (fun xss xs -> (x::xs)::xss) xss xss

module Alt =
  let requestWithNack requestCh mkReq = Alt.withNack <| fun nack ->
    let replyCh = Ch.create ()
    requestCh +<- mkReq nack replyCh
    ~~replyCh
