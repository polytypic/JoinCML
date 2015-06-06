namespace JoinCML.Examples

open JoinCML

type Unique = {queryCh: Ch<Ch<int> * Alt<unit>>}

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Unique =
  let create () =
    let queryCh = Ch ()
    Server.serveAny 0 queryCh
       <| fun (_, nack) -> nack
       <| fun (replyCh, _) i -> (replyCh *<- i, i + 1)
    |> Async.Start
    {queryCh = queryCh}

  let unique u = u.queryCh *<+-> fun replyCh nack -> (replyCh, nack)
