namespace JoinCML.Examples

open JoinCML

type Lottery =
  val winner: Alt<unit>
  val loser: Alt<unit>
  new (numTickets: int) as l =
    if numTickets <= 0 then failwithf "Lottery %d" numTickets
    let lottery = Ch () in {winner = lottery *<- (); loser = lottery} then
    let rec mk op = function
      | 0 -> op
      | n -> mk <| op -&- l.winner <| n-1
    let op = mk l.loser <| numTickets-1
    forever op |> Async.Start

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Lottery =
  let option (l: Lottery) op =
        l.loser         ^->.None
    <|> l.winner -&+ op ^-> Some
