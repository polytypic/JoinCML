namespace JoinCML

/// Represents a selective-joinable synchronous operation.
type [<AbstractClass>] Alt<'x> = class end

/// Represents a synchronous many-to-many channel.
type Ch<'x> =
  /// Inherit from `Alt<'x>` so that taking from a channel is implicit.
  inherit Alt<'x>

  /// Create a new channel.
  new: unit -> Ch<'x>

/// Operations on channels.
module Ch =
  /// Give message synchronously.
  val give: Ch<'x> -> 'x -> Alt<unit>

  /// Take message synchronously.
  val take: Ch<'x> -> Alt<'x>

/// Operations on alternatives.
module Alt =
  /// Exclusive choice of alternatives.
  val choice: Alt<'x> -> Alt<'x> -> Alt<'x>

  /// Simultaneous join of alternatives.
  val join: Alt<'x> -> Alt<'y> -> Alt<'x * 'y>

  /// Create alternative with negative acknowledgment.
  val withNack: (Alt<unit> -> #Alt<'x>) -> Alt<'x>

  /// Continue with async after commit.
  val afterAsync: ('x -> Async<'y>) -> Alt<'x> -> Alt<'y>

  /// Synchronize on alternative.
  val sync: Alt<'x> -> Async<'x>
