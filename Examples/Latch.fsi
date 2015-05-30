namespace JoinCML.Examples

open JoinCML

type Latch

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Latch =
  val create: int -> Latch
  val dec: Latch -> Alt<unit>
  val is0: Latch -> Alt<unit>
