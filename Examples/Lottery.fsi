namespace JoinCML.Examples

open JoinCML

type Lottery =
  new: numTickets: int -> Lottery

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Lottery =
  val option: Lottery
           -> winner: Alt<'winner>
           -> Alt<option<'winner>>
