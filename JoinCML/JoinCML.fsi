namespace JoinCML

/// Represents a selective-joinable synchronous operation.
type [<AbstractClass>] Alt<'x> = class end

/// Represents a synchronous many-to-many channel.
type [<AbstractClass>] Ch<'x> = inherit Alt<'x>

/// Operations on channels.
module Ch =
  /// Create a new channel.
  val create: unit -> Ch<'x>

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

  // NOTE: Everything below this point is non-primitive.

  /// Perform action before alternative construction.
  val before: (unit -> #Alt<'x>) -> Alt<'x>

  /// Alternative enabled once with given value.
  val once: 'x -> Alt<'x>

  /// Alternative enabled always with given value.
  val always: 'x -> Alt<'x>

  /// Never enabled alternative.
  val never<'x> : Alt<'x>

  /// Exclusive choice.
  val choose: seq<Alt<'x>> -> Alt<'x>

/// Symbolic operators and top-level functions for concise expression.
[<AutoOpen>]
module Convenience =
  // NOTE: Operators below are listed from highest to lowest precedence.

  /// Give message synchronously.
  val ( *<- ): channel: Ch<'x> -> message: 'x -> Alt<unit>

  /// Send message asynchronously.
  val ( *<+ ): channel: Ch<'x> -> message: 'x -> unit

  /// Commit on query and await for reply.
  val ( *<-+> ): queryCh: Ch<'q>
              -> queryFromReplyCh: (Ch<'r> -> 'q)
              -> Alt<'r>

  /// Send query and commit on reply.
  val ( *<+-> ): queryCh: Ch<'q>
              -> queryFromReplyChAndNack: (Ch<'r> -> Alt<unit> -> 'q)
              -> Alt<'r>

  /// Join and keep both.
  val (+&+): fst: Alt<'x> -> snd: Alt<'y> -> Alt<'x * 'y>

  /// Join and keep second.
  val (-&+): fst: Alt<'x> -> snd: Alt<'y> -> Alt<'y>

  /// Join and keep first.
  val (+&-): fst: Alt<'x> -> snd: Alt<'y> -> Alt<'x>

  /// Join and keep neither.
  val (-&-): fst: Alt<'x> -> snd: Alt<'y> -> Alt<unit>

  /// Continue with async after commit.
  val (^=>): after: Alt<'x> -> action: ('x -> Async<'y>) -> Alt<'y>

  /// Continue with async after commit.
  val (^=>.): after: Alt<'x> -> action: Async<'y> -> Alt<'y>

  /// Continue with function after commit.
  val (^->): after: Alt<'x> -> action: ('x -> 'y) -> Alt<'y>

  /// Continue with value after commit.
  val (^->.): after: Alt<'x> -> value: 'y -> Alt<'y>

  /// Exclusive choice.
  val (<|>): eitherThis: Alt<'x> -> orThat: Alt<'x> -> Alt<'x>

  /// Join and apply.
  val (<*>): Alt<'x -> 'y> -> Alt<'x> -> Alt<'y>

  /// Sync and continue async.
  val (|>>=): Alt<'x> -> ('x -> Async<'y>) -> Async<'y>

  /// Async bind.
  val (>>=): Async<'x> -> ('x -> Async<'y>) -> Async<'y>

  /// Async return.
  val result: 'x -> Async<'x>

  /// Extensions for async builder.
  type AsyncBuilder with
    /// Synchronize alternative and bind result.
    member Bind: Alt<'x> * ('x -> Async<'y>) -> Async<'y>

    /// Synchronize alternative and return result.
    member ReturnFrom: Alt<'x> -> Async<'x>

