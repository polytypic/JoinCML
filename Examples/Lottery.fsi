namespace JoinCML.Examples

open JoinCML

type Lottery

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Lottery =
  val create: numTickets: int -> Lottery
  val option: Lottery
           -> winner: Alt<'winner>
           -> Alt<option<'winner>>
