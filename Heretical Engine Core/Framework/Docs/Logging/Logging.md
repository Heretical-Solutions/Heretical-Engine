# [Work in progress]

---

# Logging

## TL;DR

- The [`ILogger`](ILogger.md) interface used here is NOT the same as Microsoft's ILogger or any of its variations that you may find in NuGet packages or repositories. Include with caution.
- Use [`ILogger`](ILogger.md) interface to extend the log messages with additional information (like log source), log messages to multiple sources (like printing them to console or writing to a log file), filter log messages, format them with RichText, etc. This is achieved by using decorators that implement the `ILogger` interface and pass down modified logs down the decorator chain.
- Use [`ILoggerBuilder`](ILoggerBuilder.md) to create a decorator chain of your choice and produce the resulting `ILogger` to be used in consumer classes.
- Use [`ILoggerResolver`](ILoggerResolver.md) to provide different consumer classes with different loggers. It can be useful for either feeding different logs to different channels/files or toggling which classes should actually log. If a class receives no logger from the resolver (variable is null), then it doesn't spend machine time 1) creating the string argument to log and 2) invoking the log function.
- Remember that concrete classes should receive instances of `ILogger`, while their factories should receive instances of `ILoggerResolver`. Classes should NOT ask a resolver for their dedicated loggers; their loggers should be provided to them by factories. Factories, on the other hand, should NOT use loggers that have already been resolved for some classes as a dependency for the class instances they create; they should ask logger resolvers to provide a new one.

## Implementations

### Wrappers