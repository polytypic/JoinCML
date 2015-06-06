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
    let rec full x =
          xI.fill ^=> fun _ ->
            // XXX log error
            full x
      <|> read ^=> fun replyCh ->
            replyCh *<+ x
            full x
       |> Alt.sync
    xI.fill ^=> full |> Alt.start

module IVar =
  let fill (xI: IVar<_>) x = xI.fill *<+ Choice1Of2 x
  let fillFailure (xI: IVar<_>) e = xI.fill *<+ Choice2Of2 e

type IVar<'x> with
  new (x: 'x)  as xI = IVar<'x> () then IVar.fill xI x
  new (e: exn) as xI = IVar<'x> () then IVar.fillFailure xI e
