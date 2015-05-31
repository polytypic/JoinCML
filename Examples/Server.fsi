namespace JoinCML.Examples

open JoinCML

module Server =
  val serveAny: initial: 'state
             -> requestAlt: Alt<'request>
             -> nackOf: ('request -> Alt<unit>)
             -> replyTo: ('request -> 'state -> Alt<unit> * 'state)
             -> Async<unit>
