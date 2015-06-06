namespace JoinCML.Examples

open JoinCML

type Unique =
  new: unit -> Unique

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Unique =
  val unique: Unique -> Alt<int>
