namespace JoinCML.Examples

open JoinCML

type Unique =
  inherit AltDelegate<int>
  new () =
    let queryCh = Ch ()
    {inherit AltDelegate<_> (queryCh *<+-> fun replyCh nack ->
                               (replyCh, nack))} then
    Server.serveAny 0 queryCh
     *<| fun (_, nack) -> nack
     *<| fun (replyCh, _) i -> (replyCh *<- i, i + 1)
    |> Async.Start
