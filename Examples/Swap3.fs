namespace JoinCML.Examples

open JoinCML

type Swap3<'x> = Swap3 of Ch<'x * Ch<'x * 'x>>

module Swap3 =
  let create () = Swap3 (Ch.create ())

  let swap (Swap3 sCh: Swap3<_>) x0 =
    let client = Alt.before <| fun () ->
      let cCh = Ch.create ()
      sCh *<- (x0, cCh)
      |> Alt.afterAsync (fun () ->
         Alt.sync ~~cCh)
    let leader =
      ~~sCh <&> ~~sCh
      |> Alt.afterAsync (fun ((x1, cCh1), (x2, cCh2)) ->
         cCh1 *<- (x2, x0) <&> cCh2 *<- (x0, x1)
         |> Alt.after (fun _ ->
            (x1, x2))
         |> Alt.sync)
    client <|> leader
