namespace JoinCML.Examples

open JoinCML

type MVar<'x> = MVar of Ch<'x>

module MVar =
  let rec full x c = c *<- x |>>= fun () -> empty c
  and empty c = ~~c |>>= fun x -> full x c

  let start state =
    let c = Ch.create ()
    state c |> Async.Start
    MVar c

  let create () = start empty
  let createFull x = start (full x)

  let take (MVar c) = ~~c
  let fill (MVar c) x = c *<- x