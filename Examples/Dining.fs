namespace JoinCML.Examples

open System
open JoinCML

module Dining =
  let rec runPhilosopher (rnd: Random) name lhsFork rhsFork = async {
    while true do
      printfn "%s is hungry.  Taking forks..." name
      let! (lhsIdx, rhsIdx) =
        MVar.take lhsFork <&> MVar.take rhsFork |> Alt.sync
      printfn "%s got forks %d and %d.  Eating..." name lhsIdx rhsIdx
      do! Async.Sleep (rnd.Next (0, 1000))
      printfn "%s is done eating.  Releasing forks..." name
      do! MVar.fill lhsFork lhsIdx |> Alt.sync
      do! MVar.fill rhsFork rhsIdx |> Alt.sync
      printfn "%s is thinking..." name
      do! Async.Sleep (rnd.Next (0, 1000))
  }

  let run names =
    let forks = Array.init (Array.length names) MVar.createFull
    names
    |> Array.iteri (fun i name ->
       runPhilosopher (Random ()) name
         forks.[i] forks.[(i+1) % Array.length names]
       |> Async.Start)
