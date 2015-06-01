namespace JoinCML.Examples

// This example implements a basic solution to the dining philosophers problem
//
//   http://rosettacode.org/wiki/Dining_philosophers
//
// using JoinCML.  The implementation is very simple: we simply join the two
// alternatives corresponding to the actions of taking chopsticks:
//
//   MVar.take lhsStick +&+ MVar.take rhsStick
//
// The synchronization mechanism of JoinCML takes care of resolving ordering
// problems to implement the simultaneous synchronization of joined
// alternatives.

open System
open JoinCML

module Dining =
  let rec runPhilosopher (rnd: Random) name lhsStick rhsStick = async {
    while true do
      printfn "%s is hungry.  Taking chopsticks..." name
      let! (lhsIdx, rhsIdx) = MVar.take lhsStick +&+ MVar.take rhsStick

      printfn "%s got chopsticks %d and %d.  Eating..." name lhsIdx rhsIdx
      do! Async.Sleep (rnd.Next (0, 1000))

      printfn "%s is done eating.  Releasing chopsticks..." name
      do! MVar.fill lhsStick lhsIdx
      do! MVar.fill rhsStick rhsIdx

      printfn "%s is thinking..." name
      do! Async.Sleep (rnd.Next (0, 1000))
  }

  let run names =
    let sticks = Array.init (Array.length names) MVar.createFull
    names
    |> Array.iteri (fun i name ->
       runPhilosopher (Random ()) name
         sticks.[i] sticks.[(i+1) % Array.length names]
       |> Async.Start)
