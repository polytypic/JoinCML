namespace JoinCML.Examples

open JoinCML

type Unique = {requestCh: Ch<Alt<unit> * Ch<int>>}

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Unique =
  open Server

  let create () =
    let requestCh = Ch.create ()
    serveAny 0 ~~requestCh
       <| fun (nack, _) -> nack
       <| fun (_, replyCh) i -> (replyCh *<- i, i + 1)
    |> Async.Start
    {requestCh = requestCh}

  let unique u = Alt.withNack <| fun nack ->
    let replyCh = Ch.create ()
    u.requestCh +<- (nack, replyCh)
    ~~replyCh
