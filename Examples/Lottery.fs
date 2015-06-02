namespace JoinCML.Examples

open JoinCML

type Lottery = {winner: Alt<unit>; loser: Alt<unit>}

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Lottery =
  let create n =
    assert (0 < n)
    let lottery = Ch.create ()
    let winner = lottery *<- ()
    let loser = lottery
    let rec mk op = function
      | 0 -> op
      | n -> mk (op -&- winner) (n-1)
    let op = mk loser (n-1)
    let rec forever () = op |>>= forever
    forever () |> Async.Start
    {winner = winner; loser = loser}

  let option l op =
        l.loser         ^->.None
    <|> l.winner -&+ op ^-> Some
