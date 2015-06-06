namespace JoinCML.Examples

open JoinCML

type Latch =
  inherit AltDelegate<unit>
  new: int -> Latch

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Latch =
  val dec: Latch -> Alt<unit>
