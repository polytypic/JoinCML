namespace JoinCML.Examples

open JoinCML

type Unique

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Unique =
  val create: unit -> Unique
  val unique: Unique -> Alt<int>
