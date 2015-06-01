namespace JoinCML.Examples

open JoinCML

module Server =
  val serveAny: initial: 'state
             -> queryAlt: Alt<'query>
             -> nackOf: ('query -> #Alt<unit>)
             -> replyTo: ('query -> 'state -> Alt<unit> * 'state)
             -> Async<unit>
