namespace JoinCML.Examples

open JoinCML

type Unique =
  val queryCh: Ch<Ch<int> * Alt<unit>>
  new () as u = {queryCh = Ch ()} then
    Server.serveAny 0 u.queryCh
     *<| fun (_, nack) -> nack
     *<| fun (replyCh, _) i -> (replyCh *<- i, i + 1)
    |> Async.Start

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Unique =
  let unique (u: Unique) =
    u.queryCh *<+-> fun replyCh nack -> (replyCh, nack)
