namespace JoinCML.Examples

open JoinCML

type IVar<'x> =
  inherit AltDelegate<'x>
  val fill: Ch<Choice<'x, exn>>
  new () as xI =
    let read = Ch ()
    {inherit AltDelegate<_> (read *<-+> id ^-> function
                              | Choice1Of2 x -> x
                              | Choice2Of2 e -> raise e)
     fill = Ch ()} then
    let serve x =
      read ^-> fun replyCh ->
        replyCh *<-+ x
    let drain =
      xI.fill ^->. () // XXX log spurious fill error
    xI.fill ^-> fun x ->
        serve x |> forever |> Async.Start
        drain |> forever |> Async.Start
    |> Alt.start

module IVar =
  let fill (xI: IVar<_>) x = xI.fill *<-+ Choice1Of2 x
  let fillFailure (xI: IVar<_>) e = xI.fill *<-+ Choice2Of2 e

type IVar<'x> with
  new (x: 'x)  as xI = IVar<'x> () then IVar.fill xI x
  new (e: exn) as xI = IVar<'x> () then IVar.fillFailure xI e
