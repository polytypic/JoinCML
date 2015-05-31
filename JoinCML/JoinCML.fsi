namespace JoinCML

type Alt<'x>
type Ch<'x>

module Ch =
  val create: unit -> Ch<'x>
  val give: Ch<'x> -> 'x -> Alt<unit>
  val take: Ch<'x> -> Alt<'x>

module Alt =
  val choice: Alt<'x> -> Alt<'x> -> Alt<'x>
  val join: Alt<'x> -> Alt<'y> -> Alt<'x * 'y>

  val withNack: (Alt<unit> -> Alt<'x>) -> Alt<'x>

  val afterAsync: ('x -> Async<'y>) -> Alt<'x> -> Alt<'y>

  val sync: Alt<'x> -> Async<'x>

  // Non-primitives:

  val before: (unit -> Alt<'x>) -> Alt<'x>

  val once: 'x -> Alt<'x>
  val never<'x> : Alt<'x>

  val after: ('x -> 'y) -> Alt<'x> -> Alt<'y>

  val choose: seq<Alt<'x>> -> Alt<'x>

[<AutoOpen>]
module Convenience =
  val ( *<- ): Ch<'x> -> 'x -> Alt<unit>

  val ( ~~ ): Ch<'x> -> Alt<'x>

  val ( +<- ): Ch<'x> -> 'x -> unit

  val ( <|> ): Alt<'x> -> Alt<'x> -> Alt<'x>

  val ( <&> ): Alt<'x> -> Alt<'y> -> Alt<'x * 'y>
  val ( .&> ): Alt<_> -> Alt<'y> -> Alt<'y>
  val ( <&. ): Alt<'x> -> Alt<_> -> Alt<'x>
  val ( .&. ): Alt<_> -> Alt<_> -> Alt<unit>

  val ( <*> ): Alt<'x -> 'y> -> Alt<'x> -> Alt<'y>

  val ( >>= ): Async<'x> -> ('x -> Async<'y>) -> Async<'y>
  val result: 'x -> Async<'x>

  val ( |>>= ): Alt<'x> -> ('x -> Async<'y>) -> Async<'y>
