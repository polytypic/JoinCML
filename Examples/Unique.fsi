namespace JoinCML.Examples

open JoinCML

type Unique =
  inherit AltDelegate<int>
  new: unit -> Unique
