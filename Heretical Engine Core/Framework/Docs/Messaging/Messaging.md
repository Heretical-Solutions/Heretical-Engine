# [Work in progress]

---

# Messaging

## TL;DR

- [`IMessageSender`](IMessageSender.md) allows to pop, write and send an `IMessage` message instance
- [`IMessageReceiver`](IMessageReceiver.md) allows to subscribe to or unsubscribe from certain message types
- Both `IMessageSender` and `IMessageReceiver` are implemented by message busses
- The `IMessageReceiver` interface is designed to allow subscription holders to un/subscribe their subscriptions to the bus but not to call its message sending methods
- The `IMessageSender` interface is designed to allow message writers to send messages but not to call bus's un/subscribe methods.

## What are message busses for