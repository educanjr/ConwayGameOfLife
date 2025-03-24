# Architecture Documentation: Conway's Game of Life (.NET 7 Implementation)

## Overview

This documentation describes the architecture for Conway's Game of Life implemented using .NET 7, leveraging Clean Architecture principles, CQRS with MediatR, Repository Pattern, and Entity Framework Core. The application is deployed using Docker Compose, with PostgreSQL as the persistence layer.

## Architectural Principles

### Clean Architecture
- Clear separation of concerns, promoting maintainability and scalability.
- Dependencies flow inward towards the Application layer, ensuring isolation of domain logic from external dependencies.

### CQRS (Command Query Responsibility Segregation)
- MediatR used to clearly separate read (Queries) and write (Commands) operations.

### Repository Pattern
- Abstracts data access logic, improving testability and encapsulation of data source specifics.

## Enforcing CQRS with Custom Interfaces

### Why Not Just Use MediatR's `IRequest`?

While MediatR's `IRequest` interface offers a great starting point for implementing CQRS, it lacks semantic expressiveness and can lead to mixed concerns across the codebase. In this implementation, custom interfaces like `ICommand`, `ICommand<T>`, `IQuery<T>`, and their corresponding handlers enforce clearer boundaries between commands and queries. This helps ensure the intent of each operation is explicit.

### Benefits of Custom CQRS Interfaces
- **Strong Semantics**: Distinguishes between commands and queries clearly at the type level.
- **Standardized Result Handling**: All handlers return a `ResultObject` or `ResultObject<T>`, aligning with the Result Object pattern.
- **Improved Consistency**: Promotes consistency in implementation across the entire application.
- **Encourages Best Practices**: Developers are nudged towards respecting CQRS principles by using purpose-specific interfaces.

### Example Usage
```csharp
// Command Definition
public sealed record CalculateFinalStepCommand(Guid Id) : ICommand<CalculateExecutionsDto>;

// Command Handler
internal sealed class CalculateFinalStepCommandHandler : ICommandHandler<CalculateFinalStepCommand, CalculateExecutionsDto>
{
    public async Task<ResultObject<CalculateExecutionsDto>> Handle(CalculateFinalStepCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // logic
        }
        catch (Exception ex)
        {
            return ex switch {
                ExecutionLimitReachedException => ResultObject.ApplicationRuleViolation<CalculateExecutionsDto>(ex.Message),
                _ => ResultObject.NotFound<CalculateExecutionsDto>(ex.Message),
            };
        }
    }
}
```

## Domain Entities with Encapsulated Behavior

### Embracing Domain-Driven Design (DDD)
This project follows the DDD philosophy by treating domain entities not just as data structures, but as behavior-rich objects that encapsulate domain logic. For example, the `Board` entity manages its own rules for evolving the state of the game, including methods like `ResolveNextExecution` and `ResolveFinalExecution`, and the `BoardState` entity includes the logic to compute Conway's rules.

### Alignment with SOLID Principles
Encapsulating behavior inside domain entities reinforces the **Single Responsibility Principle**:
- Each entity (e.g., `Board`, `BoardState`) is responsible for maintaining its state and domain behavior.
- Logic that belongs to the domain lives with the domain object rather than being scattered across services or handlers.

### Benefits of This Approach
- **High Cohesion**: Related data and behavior live together, improving clarity and discoverability.
- **Low Coupling**: Reduces the need for external services to coordinate domain logic.
- **Testability**: Makes entities easier to test in isolation.
- **Maintainability**: Enhancements to domain behavior are localized within the entity itself.

### Example: Board Logic Encapsulation
```csharp
public BoardExecution ResolveNextExecution(int maxExecutionsAllowed)
{
    var latestExecution = GetLatestExecution();
    var currentState = latestExecution?.State ?? InitialState;
    var currentStep = latestExecution?.Step ?? 0;
    var isCompleted = latestExecution?.IsFinal ?? false;

    if (isCompleted || currentStep >= maxExecutionsAllowed)
    {
        throw new ExecutionLimitReachedException();
    }

    var nextState = currentState.ComputeNextState();
    var nextStep = currentStep + 1;
    var isLastState = nextStep == maxExecutionsAllowed || IsLastState(nextState);

    var nextExecution = new BoardExecution
    {
        BoardId = Id,
        Step = nextStep,
        State = nextState,
        IsFinal = isLastState
    };

    AddExecutionToList(nextExecution);
    return nextExecution;
}
```

This domain-centric approach results in a more expressive and robust model, closely reflecting real-world concepts and interactions.

## Result Object Pattern

### Why Use Result Objects Over Exceptions?

The Result Object pattern provides a structured way to handle operation outcomes, distinguishing clearly between successful and failed operations without relying on exceptions. Unlike exceptions, Result Objects make failure scenarios explicit in the method's contract, which significantly improves readability, clarity, and predictability of the code. 

### Impact on Maintainability
- **Explicit Error Handling**: Developers are forced to handle potential errors explicitly, avoiding hidden or unhandled exceptions at runtime.
- **Clearer Business Logic**: The Result Object communicates intent clearly, allowing developers to easily understand success and failure pathways in the code.
- **Improved Debugging**: Errors returned through Result Objects contain structured and informative error messages, enhancing debugging and reducing maintenance overhead.
- **Predictable Flow**: Results enable predictable application flow, as all possible outcomes (success, failure, validation errors, etc.) are explicitly stated and managed.

## Custom Data Mapping

### Custom Mappers vs. AutoMapper or Mapster
Using custom mappers (like the `DataConverters` class) instead of mapping libraries such as AutoMapper or Mapster offers greater control, clarity, and debuggability:
- **Explicitness**: Custom mappers explicitly define mappings, reducing hidden magic and unexpected behavior.
- **Improved Debugging**: Easier to debug and step through the code due to its straightforward nature.
- **Performance**: Custom mappers can offer optimized performance since they do exactly what is necessary without overhead.

### Presentation Layer DTOs
Having separate DTOs for the presentation layer (Contracts) instead of directly using application-layer DTOs is beneficial because:
- **Encapsulation of Concerns**: Clearly separates the presentation concerns from the business logic.
- **Versioning Flexibility**: Facilitates API evolution by isolating changes within the presentation layer.
- **Security and Validation**: Provides a place to manage validation and security checks specific to the API layer.

### Scalability of Data Mapping
As the project grows, maintaining all mappings within a single file (such as the `DataConverters` class) can become challenging due to increased complexity and reduced readability. A recommended approach would be to adopt a structured strategy, such as creating individual mapper classes or profiles for each domain entity or operation.

#### Example Recommendation:
```csharp
public interface IMapper<TSource, TDestination>
{
    TDestination Map(TSource source);
}

public class BoardStateMapper : IMapper<BoardStateDto, CurrentBoardStateResponse>
{
    public CurrentBoardStateResponse Map(BoardStateDto source) =>
        new CurrentBoardStateResponse(
            Id: source.Id,
            Name: source.Name,
            InitialState: source.InitialState.ToJaggedArrayState(),
            CurrentStep: source.CurrentStep,
            IsCompleted: source.IsCompleted,
            State: source.State.ToJaggedArrayState());
}
```

## Architecture Layers

### 1. Presentation Layer (ConwayGameOfLife.Web)
- Handles API interactions.
- Contains Controllers, DTOs (Contracts), Middleware, and abstractions (Base Controllers).
- Uses Swagger UI for API documentation and interaction.

### 2. Application Layer (ConwayGameOfLife.Application)
- Houses business logic, domain entities, commands, queries, exceptions, DTOs, and configurations.
- MediatR orchestrates command and query handlers.
- Implements the Result Object pattern for structured response handling.

### 3. Data Layer (ConwayGameOfLife.Data)
- Manages persistence using Entity Framework Core.
- Contains DbContext, configurations for entities, migration scripts, and repository implementations.
- Provides abstractions for repositories and DbContextFactory for controlled DB context creation.

### 4. Infrastructure
- Docker Compose is used for containerization, connecting application containers to PostgreSQL.
- TestContainers are used for integration testing, ensuring environment parity.

### 5. Testing Layers
- Integration tests (ConwayGameOfLife.IntegrationTests) ensure system reliability.
- Architecture tests (ConwayGameOfLife.ArchitectureTests) validate adherence to architectural rules.

## Deployment Architecture

### Containerized Deployment (Docker Compose)

- **API Container**: ASP.NET Core Web API, accessible via port 7006.
- **Database Container**: PostgreSQL database, accessible via port 5433.
- Containers communicate through a dedicated network (`conwaygame.devnetwork`).

## API Design & Interaction

- RESTful API following CRUD operations.
- Endpoints defined clearly for board state management (current state, next state, multiple steps, and final state).
- JSON structured responses.

## Technologies Used

- **Framework:** .NET 7
- **Web:** ASP.NET Core Web API
- **ORM:** Entity Framework Core
- **CQRS:** MediatR
- **Database:** PostgreSQL
- **Containerization:** Docker Compose
- **Testing:** xUnit, TestContainers

## Conclusion

This implementation adheres to robust software architecture practices ensuring clarity, scalability, and maintainability. It leverages Docker to provide reproducible environments, simplifying deployment and scaling.

