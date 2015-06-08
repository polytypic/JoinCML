namespace JoinCML

open JoinCML

/// Symbolic operators and top-level functions for concise expression.
[<AutoOpen>]
module Convenience =
  // NOTE: Operators below are listed from highest to lowest precedence.

  /// Give message synchronously.
  val ( *<- ): channel: Ch<'x> -> message: 'x -> Alt<unit>

  /// Send message asynchronously.
  val ( *<-+ ): channel: Ch<'x> -> message: 'x -> unit

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

  /// Async map.
  val (>>-): Async<'x> -> ('x -> 'y) -> Async<'y>

  /// Async return.
  val result: 'x -> Async<'x>

  /// Extensions for async builder.
  type AsyncBuilder with
    /// Synchronize alternative and bind result.
    member Bind: Alt<'x> * ('x -> Async<'y>) -> Async<'y>

    /// Synchronize alternative and return result.
    member ReturnFrom: Alt<'x> -> Async<'x>

  /// Additional operations on channels.
  module Ch =
    /// Take message synchronously.
    val take: Ch<'x> -> Alt<'x>

    /// Send message asynchronously.
    val send: Ch<'x> -> 'x -> unit

  /// Additional operations on alternatives.
  module Alt =
    /// Start async thread to synchronize alternative.
    val start: Alt<unit> -> unit

    /// Create alternative with abort action.
    val wrapAbort: action: (unit -> unit) -> Alt<'x> -> Alt<'x>

    /// Create alternative just before synchronization.
    val prepare: (unit -> #Alt<'x>) -> Alt<'x>

    /// Enabled once with given value.
    val once: 'x -> Alt<'x>

    /// Enabled always with given value.
    val always: 'x -> Alt<'x>

    /// Enabled always with unit value.
    val unit: Alt<unit>

    /// Never enabled.
    val never<'x> : Alt<'x>

    /// Exclusive choice.
    val choose: seq<Alt<'x>> -> Alt<'x>

  /// Create alternative that becomes available after given time.
  val timeOutMillis: int -> Alt<unit>
