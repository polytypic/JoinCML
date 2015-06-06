namespace JoinCML.Examples

open JoinCML

type Latch =
  new: int -> Latch

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Latch =
  val dec: Latch -> Alt<unit>
  val is0: Latch -> Alt<unit>
