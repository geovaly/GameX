# GameX
Solution to a technical exercise for a job application. 

Dear Reviewer,

I’m impressed by the person who created this exercise. That's why my goal was to create the best solution you had and you'll ever review: clean, creative, flexible, and decoupled in both code and architecture. I hope you enjoy reviewing it as much as I enjoyed building it!

# Framework

No SignalR, no MediatR, no external libraries? No problem! I built my own amazing framework named RequestResponseFramework. 

Solution Features:

- Request-Handler Pattern: Commands, Queries, and Events.

- Result Pattern: An elegant way to return exception alongside result, compatible with traditional exception throwing (See RequestExecutor).

- Polymorphic JSON Serialization/Deserialization for requests and responses.

- Middleware logic, similar to ASP.NET Core (MiddlewareExecutor).

- Client Connection support, essential for game servers.

- Context Passing support, such as a LoggedInContext for storing authentication data.

- Implementation of both WebSocketServer and WebSocketClient based on RequestExecutor.

- Logging Abstractions for decoupling Serilog.

- Handling concurrency issues using Semaphores, ConcurrentDictionary, and TaskCompletionSource.

# Architecture

Methodology: Domain-Driven Design (DDD)

Structure:

- 1st Level: Components - SuperPlay.GameX, RequestResponseFramework
- 2nd Level: Stacks - Backend, Frontend, and Shared
- 3rd Level: Layers - ApiLayer, ApplicationLayer, DomainLayer, PersistenceLayer, GenericLayer

I like to have a GenericLayer to encapsulate generic, non-business logic housed within the same component. The RequestResponseFramework began in this layer but evolved into a standalone framework, justifying its own dedicated component.

# Game

Name: GameX

Solution Features:

- Additional requests: GetMyPlayerQuery, LogoutCommand

- Unit of Work Pattern, for decoupling Entity Framework Core.

- Domain Primitives:  PlayerId, DeviceId, ResourceValue - encapsulating system primitives 

- Logging Abstractions for decoupling Serilog

- Composition Root Pattern, centralizing object graph composition using the dependency injection container.

- Application logic implemented via RequestHandlers and MiddlewareExecutors.

- Handling concurrency issues using locks and UnitOfWorkConcurrencyMiddlewareExecutor (  retry logic to handle concurrency issues during entity updates )

# Testing

3 ways:

- ConsoleGameClient: Manual Testing. Please run separate consoles for each connected player (ex when testing SendGift). You cannot have multiple players logged in simultaneously on the same console!
- GameServerTests: Standard automated testing (cluster tests)
- GameServerDslTests: Domain-specifc language automated testing (cluster tests)

I find cluster tests at the application level more valuable and flexible than isolated unit tests focused solely on RequestHandlers or MiddlewareExecutors.

## Why DSL Tests?
Domain-specifc language tests leverage the business domain language directly, remaining independent of the software interface. Though these tests require additional foundational code (GameServer.DslTests.Base), they enhance readability and maintainability. You can experiment with these tests in GameServer.DslTests > PlaygroundDslTests.cs.

# Conclusion

Software development is inherently subjective, with multiple valid approaches to solving the same problems. The solution presented reflects my best judgment in the absence of predefined team agreements.

Thank you for reviewing my work. I look forward to your feedback and I want to know you! :)
