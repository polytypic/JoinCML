# JoinCML

A research project to create an extension of CML with a `join` combinator.

The core of CML-style events, or alternatives, boils down to two types:

```fsharp
type Alt<'x>
type Ch<'x>
```

And a set of combinators:

```fsharp
module Ch =
  val create: unit -> Ch<'x>
  val give: Ch<'x> -> 'x -> Alt<unit>
  val take: Ch<'x> -> Alt<'x>

module Alt =
  val choice: Alt<'x> -> Alt<'x> -> Alt<'x>
  val withNack: (Alt<unit> -> Alt<'x>) -> Alt<'x>
  val afterAsync: ('x -> Async<'y>) -> Alt<'x> -> Alt<'y>
  val sync: Alt<'x> -> Async<'x>
```

The idea of
[Transactional Events](http://www.cs.rit.edu/~mtf/research/tx-events/ICFP06/icfp06.pdf)
(TE) is to extend CML-style events to a monad (with plus).  The result is a
highly expressive synchronization mechanism with some surprising and interesting
[Programming Idioms](http://arxiv.org/pdf/1002.0936.pdf).

The idea of JoinCML is to extend CML-style alternatives just far enough to form
an applicative functor.  More specifically by adding a single new combinator:

```fsharp
module Alt =
  val join: Alt<'x> -> Alt<'y> -> Alt<'x * 'y>
```

Informally, the semantics is that the two given alternatives must both be
available simultaneously in order for the alternative to complete.

Using `join` one can implement the `<*>` operation of applicative functors as:

```fsharp
let (<*>) x2yA xA =
  Alt.join x2yA xA
  |> Alt.after (fun (x2y, x) ->
     x2y x)
```

Unlike TE, JoinCML essentially retains all of CML as a subset, including
negative acknowledgments and the semantics of having only a single commit point.
See the full [JoinCML.fsi](JoinCML/JoinCML.fsi) signature including many
convenience operations.


Unsurprisingly, it appears that basically the same idea has already been
discovered earlier in the form of
[Conjoined Events](https://kar.kent.ac.uk/33878/1/Conjoined.pdf).  Thanks to
Matthew Fluet for finding the paper!

This project seeks to answer the following questions:

* What can JoinCML express that CML cannot express?
* What cannot JoinCML express that TE can express?
* Does JoinCML have favorable properties when compared to TE?
* Can JoinCML be implemented efficiently?
* What sort of programming idioms work with JoinCML?

Here are some working hypotheses:

* CML ⊊ JoinCML ⊊ TE.  That is, JoinCML is strictly more expressive than CML and
  TE is strictly more expressive than JoinCML.
* JoinCML allows n-way rendezvous to be implemented for any n determined before
  the synchronization.  TE allows the n to be determined during synchronization.
  CML only allows 2-way rendezvous.
* JoinCML subsumes core
  [join-calculus](http://research.microsoft.com/en-us/um/people/fournet/papers/join-tutorial.pdf).
* JoinCML is significantly less expensive to implement than TE, because
  synchronization does not require evaluating events step-by-step during
  synchronization.
* JoinCML is better suited to impure languages than TE, because synchronization
  does not require running arbitrary code and thus there is no danger of
  performing side-effects that cannot be rolled back.

One way to gain some additional intuition is that JoinCML alternatives can be
rewritten to a [DNF](http://en.wikipedia.org/wiki/Disjunctive_normal_form)-style
normal form that roughly looks like

```fsharp
(p11 <&> ...) <|> (p21 <&> ...) <|> ...
```

when we use `<|>` for `choice` and `<&>` for `join` and the `pij` are operations
on channels.  Ordinary CML doesn't have `<&>` aka `join`, so in ordinary CML the
normal form is just

```fsharp
p1 <|> p2 <|> ...
```

In both of the above, the resulting normal forms are finite.  TE does not have
such a normal form.  It is even possible to express a pair of processes using TE
that exchange messages indefinitely without completing a transaction.

At this point JoinCML is vaporware, but I definitely plan to try find an
efficient implementation.  (Of course, most of JoinCML can be trivially
implemented in TE.)

Some examples have been drafted:

* 3-way swap channel in [Swap3.fsi](Examples/Swap3.fsi) and
  [Swap3.fs](Examples/Swap3.fs).
* Dining Philosophers [Dining.fs](Examples/Dining.fs) using `MVar`s defined in
  [MVar.fsi](Examples/MVar.fsi) and [MVar.fs](Examples/MVar.fs).
* Synchronous channel with guarded receive in
  [GuardedCh.fsi](Examples/GuardedCh.fsi) and
  [GuardedCh.fs](Examples/GuardedCh.fs).
* Translations of examples based on join-calculus in
  [Join.fs](Examples/Join.fs).

Contributions and collaborators are welcome!  In particular, if you have
interesting concurrent programming problems to solve, it would be interesting to
try to express solutions to them using JoinCML.
