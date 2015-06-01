namespace JoinCML

type [<AbstractClass>] Alt<'x> = class end
type [<AbstractClass>] Ch<'x> = inherit Alt<'x>

module Ch =
  val create: unit -> Ch<'x>
  val give: Ch<'x> -> 'x -> Alt<unit>
  val take: Ch<'x> -> Alt<'x>

module Alt =
  val choice: Alt<'x> -> Alt<'x> -> Alt<'x>
  val join: Alt<'x> -> Alt<'y> -> Alt<'x * 'y>

  val withNack: (Alt<unit> -> #Alt<'x>) -> Alt<'x>

  val afterAsync: ('x -> Async<'y>) -> Alt<'x> -> Alt<'y>

  val sync: Alt<'x> -> Async<'x>

  // Non-primitives:

  val before: (unit -> #Alt<'x>) -> Alt<'x>

  val once: 'x -> Alt<'x>
  val never<'x> : Alt<'x>

  val choose: seq<Alt<'x>> -> Alt<'x>

[<AutoOpen>]
module Convenience =
  val (-->): 'x -> Ch<'x> -> Alt<unit>

  val (-~>): 'x -> Ch<'x> -> unit

  val (<-~>): Ch<'q> -> (Ch<'r>              -> 'q) -> Alt<'r>
  val (<~->): Ch<'q> -> (Ch<'r> -> Alt<unit> -> 'q) -> Alt<'r>

  val (|>~): Alt<'x> -> ('x -> Async<'y>) -> Alt<'y>
  val (|>-): Alt<'x> -> ('x ->       'y ) -> Alt<'y>
  val (|>=): Alt<'x> ->              'y   -> Alt<'y>

  val (<|>): Alt<'x> -> Alt<'x> -> Alt<'x>

  val (<&>): Alt<'x> -> Alt<'y> -> Alt<'x * 'y>
  val (.&>): Alt<_>  -> Alt<'y> -> Alt<'y>
  val (<&.): Alt<'x> -> Alt<_>  -> Alt<'x>
  val (.&.): Alt<_>  -> Alt<_>  -> Alt<unit>

  val (<*>): Alt<'x -> 'y> -> Alt<'x> -> Alt<'y>

  val (>>=): Async<'x> -> ('x -> Async<'y>) -> Async<'y>
  val result: 'x -> Async<'x>

  val (|>>=): Alt<'x> -> ('x -> Async<'y>) -> Async<'y>
