namespace JoinCML.Examples

open System.Collections.Generic
open JoinCML

module List =
  let foldFrom s f xs = List.fold f s xs

[<AutoOpen>]
module internal Util =
  let ( *<| ) f x = f x
  let ( *>> ) f g = f >> g

  let inline newLinkedListNode (x: 'x) = LinkedListNode<'x> (x)

  let nodes (lst: LinkedList<_>) =
    let rec lp ns = function
      | null -> ns
      | (n: LinkedListNode<_>) ->
        lp <| n::ns <| n.Next
    lp [] lst.First

  let rec powerset = function
    | [] -> [[]]
    | x::xs ->
      let xss = powerset xs
      xss
      |> List.foldFrom xss *<| fun xss xs -> (x::xs)::xss
