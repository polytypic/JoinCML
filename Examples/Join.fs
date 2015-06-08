namespace JoinCML.Examples

// The examples in this file are translations of various examples based on
// join-calculus or JoCaml:
//
//   http://research.microsoft.com/en-us/um/people/fournet/papers/join-tutorial.pdf
//   http://jocaml.inria.fr/doc/concurrent.html
//
// JoinCML seems to be able to encode join-calculus with basic pattern matching
// and guards.  Also, message passing can remain synchronous, which means that
// the client side can use JoinCML combinators to further compose operations.
//
// Note that the examples in this file do not necessarily represent the best
// ways to use JoinCML, because JoinCML does not require representing internal
// server state via channel operations.

open JoinCML
open JoinCML.Examples

module Join =
  let def cs = Alt.choose cs |> forever |> Async.Start
  let Non = function None -> Some () | Some _ -> None
  let Som = function None -> None    | Some x -> Some x // aka `id`
  let ( *<<-+ ) gCh x = GuardedCh.give gCh x |> Alt.start
  let ( *? ) gCh m = gCh |> GuardedCh.pick m
  let ( *?= ) gCh c =
    gCh
    |> GuardedCh.pick *<| fun x ->
       if x = c then Some x else None

  module OnePlaceBuffer =
    type OnePlaceBuffer<'x> = {get: Alt<'x>; put: 'x -> Alt<unit>}
    let create () =
      let put = Ch ()
      let get = Ch ()
      let buf = GuardedCh ()
      buf *<<-+ None
      def [buf *? Non -&+ put ^-> fun x          -> buf *<<-+ Some x
           buf *? Som +&+ get ^-> fun (x, repCh) -> buf *<<-+ None; repCh *<-+ x]
      {get = get *<-+> id; put = Ch.give put}

  module Pies =
    type Filling = Apple | Rasperry | Cheese
    type Kind = Pie | Cake
    type Pies = {fill: Filling -> Alt<unit>; kind: Kind -> Alt<unit>}
    let pies () =
      let f = GuardedCh ()
      let k = GuardedCh ()
      def [(f *?= Apple <|> f *?= Rasperry) +&- k *?= Pie ^-> printfn "%A pie"
           (f *?= Rasperry <|> f *?= Cheese) +&- k *?= Cake ^-> printfn "%A cake"]
      {fill = GuardedCh.give f; kind = GuardedCh.give k}
