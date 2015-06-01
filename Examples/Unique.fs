namespace JoinCML.Examples

open JoinCML

type Unique = {requestCh: Ch<Alt<unit> * Ch<int>>}

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Unique =
  let create () =
    let requestCh = Ch.create ()
    Server.serveAny 0 requestCh
       <| fun (nack, _) -> nack
       <| fun (_, replyCh) i -> (replyCh %<- i, i + 1)
    |> Async.Start
    {requestCh = requestCh}

  let unique u = u.requestCh %<~-> fun replyCh nack -> (nack, replyCh)
