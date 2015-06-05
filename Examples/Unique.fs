namespace JoinCML.Examples

open JoinCML

type Unique = {queryCh: Ch<Alt<unit> * Ch<int>>}

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Unique =
  let create () =
    let queryCh = Ch ()
    Server.serveAny 0 queryCh
       <| fun (nack, _) -> nack
       <| fun (_, replyCh) i -> (replyCh *<- i, i + 1)
    |> Async.Start
    {queryCh = queryCh}

  let unique u = u.queryCh *<+-> fun replyCh nack -> (nack, replyCh)
