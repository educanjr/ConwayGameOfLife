# 🌀 Conway's Game of Life - .NET 7 Implementation

## 📖 About the Project

This is a **.NET 7** implementation of **Conway's Game of Life**, following **Clean Architecture** principles.  
It uses **CQRS with MediatR**, the **Repository Pattern**, and **EF Core** for data access.  
The system is containerized using **Docker Compose**, with **PostgreSQL** as the database.

---

## 🎮 Game Rules

Conway's Game of Life is a **zero-player cellular automaton**, meaning its evolution is determined by its initial state.  
Each cell in a **2D grid** can be **alive (`true`) or dead (`false`)** and follows these rules:

1. **Underpopulation:** A live cell with fewer than **2** neighbors dies.  
2. **Survival:** A live cell with **2 or 3** neighbors stays alive.  
3. **Overpopulation:** A live cell with more than **3** neighbors dies.  
4. **Reproduction:** A dead cell with **exactly 3** live neighbors becomes alive.  

The simulation progresses by applying these rules to all cells **simultaneously**.

---

## 📂 Project Structure

The solution follows Clean Architecture, with clearly defined layers and strong domain entities. Since the application is relatively small, a dedicated Domain project was not added, but domain-specific logic is encapsulated within the Application layer.

```
ConwayGameOfLife.sln
│
├── 📂 ConwayGameOfLife.App           # 🚀 Startup & service orchestrator
│   ├── Program.cs                     # App entry point
│   ├── Configuration                  # App configuration
│   ├── OptionsSetup                   # Options from AppSettings
│
├── 📂 ConwayGameOfLife.Application    # 🧠 Business logic & domain definitions
│   ├── Abstractions                    # Application layer abstractions and contracts
│   ├── CommandAndQueries               # Use cases (CQRS commands/queries with MediatR)
│   ├── Entities                        # Domain entities & aggregates
│   ├── Repositories                    # Repositories contracts
│   ├── Exceptions                      # Custom Exceptions
│   ├── Dtos                            # Domain transfer options
│   ├── ConfigOptions                   # Object models for configuration options
│   ├── Common                          # Common objects and patterns (like Result Object pattern implementation)
│
├── 📂 ConwayGameOfLife.Data           # 💾 EF Core database setup
│   ├── DbContext                        # Entity Framework Core database context
│   ├── Configurations                   # DB Entity Framework Core configuration files for Entitiy Types
│   ├── Repositories                     # Basic Repository pattern implementation
│   ├── Migrations                       # EF Core database migrations
│   ├── Abstractions                     # Data ayer abstractions (like Base Repository)
│   ├── DbContextFactory                 # Entity Framework Core database context factory
│
├── 📂 ConwayGameOfLife.Web            # 🎨 Presentation layer (Controllers & API)
│   ├── Controllers                      # API Controllers
│   ├── Contracts                        # API request/response DTOs
│   ├── Middleware                       # API Middlewares 
│   ├── Abstractions                     # Presentation layer abstractions (like Base Controller)
│
├── 📂 ConwayGameOfLife.IntegrationTests  # 🔬 Integration tests with TestContainers
│
├── 📂 ConwayGameOfLife.ArchitectureTests # 🔍 Architecture tests (ensuring correct layer dependencies)
│
└── docker-compose.yml                  # 🐳 Docker setup (API + PostgreSQL DB)
```

---

## ⚡ Technologies Used
✅ **.NET 7** - Core framework  
✅ **ASP.NET Core Web API** - Presentation layer  
✅ **Entity Framework Core** - ORM for database access  
✅ **MediatR** - CQRS (Command Query Responsibility Segregation)  
✅ **PostgreSQL** - Relational database  
✅ **Docker Compose** - Containerized application  
✅ **TestContainers** - Integration testing  

---

## 🛠️ How to Build & Run with Docker

### **1️⃣ Clone the Repository**
```sh
git clone https://github.com/your-username/ConwayGameOfLife.git
cd ConwayGameOfLife
```

### **2️⃣ Build & Start the App**
Use **Docker Compose** to build and run the entire system (API + PostgreSQL):
```sh
docker-compose up --build
```

### **3️⃣ Access the API**
Once running, the API is available at:
- **Swagger UI:** [http://localhost:7006/swagger](http://localhost:7006/swagger)  
- **API Base URL:** `http://localhost:7006/api`

---

## 📡 API Endpoints
The API provides multiple endpoints for interacting with the Game of Life simulation. A **Postman collection** is available at the root of the repository for testing these endpoints. 
The application is seeded with **two boards**:
- **Blinker (3x3) Oscillator** -> `3fedc2a6-9743-4b8c-8087-c34cd0e383ad`
- **Glider Gun (10x10)** -> `2229a2ca-3e77-4637-91e1-06e66630068b`

📌 **Note:** Responses will be simplified in this document.

### **1️⃣ Get Current Board State**
Retrieves the current state of a board.

**Request:**
```http
GET /api/v1/Game/{Blinker_Id}
```

**Example Response:**
```json
{
  "name": "Blinker 3x3",
  "state": [
    [false, false, false],
    [true,  true,  true ],
    [false, false, false]
  ]
}
```

### **2️⃣ Calculate the Next State of a Board**
Applies **one step of evolution** from current status based on Conway’s rules. If this endpoint is called after board reaches a stable state or the max iterations are exceeded an error is returned.

**Request:**
```http
PATCH /api/v1/Game/{Blinker_Id}/next
```

**Example Response:**
```json
{
  "state": [
    [false, true, false],
    [false, true, false],
    [false, true, false]
  ]
}
```

### **3️⃣ Calculate Multiple Steps Ahead**
Computes **a given number of steps** in advance from current state. Applies **evolutions** until board reach to the **final stable state** or the **last possible state**, based on the maximun ammount of executions allowed.

**Request:**
```http
PATCH /api/v1/Game/{Blinker_Id}/next/{steps}
```

**Example for 2 Steps:**
```json
{
  "state": [
    [false, false, false],
    [true,  true,  true ],
    [false, false, false]
  ]
}
```
📌 The **Blinker** oscillates between two states every step.

### **4️⃣ Get the Final State**
Applies all required **step of evolution** based on Conway’s rules. Until board reach to the **final stable state** or the **last possible state**, based on the maximun ammount of executions allowed.

**Request:**
```http
GET /api/v1/Game/{Blinker_Id}/final
```

**Response:**
Retrieves the **final stable state** or the **last possible state**, based on max executions allowed, of the board.


---

## 🏗️ CQRS with MediatR
The project uses **CQRS (Command Query Responsibility Segregation)** via **MediatR**.  
- **Commands:** Used for modifying data (`RegisterBoardCommand`, `CalculateNextStepCommand`, `CalculateNextNStepsCommand`, `CalculateFinalStepCommand`).  
- **Queries:** Used for retrieving data (`GetCurrentBoardQuery`, `GetBoardStepQuery`).  

### **Example Command**
```csharp
public record CalculateNextStepCommand(Guid Id) : ICommand<BoardStateDto>;

public class CalculateNextStepCommandHandler : ICommandHandler<CalculateNextStepCommand, BoardStateDto>
{
    private readonly IBoardRepository _boardRepository;

    public CalculateNextStateHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<BoardStateDto> Handle(CalculateNextStateCommand request, CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetBoardIncludingExecutions(request.BoardId);
        var nextExecution = board.ResolveNextExecution();
        await _boardRepository.AddExecution(nextExecution);
        return new BoardStateDto(
            Id: board.Id,
            Name: board.Name,
            InitialState: board.InitialState,
            CurrentStep: nextExecution.Step,
            IsCompleted: nextExecution.IsFinal,
            State: nextExecution.State);
    }
}
```

---

## ✅ Running Tests
### **1️⃣ Integration Tests (TestContainers)**
Run integration tests using **xUnit & TestContainers**:
```sh
dotnet test ConwayGameOfLife.IntegrationTests
```

### **2️⃣ Architecture Tests**
Ensure **Clean Architecture principles** are followed:
```sh
dotnet test ConwayGameOfLife.ArchitectureTests
```

---

## 📜 License
This project is licensed under the **MIT License**. Feel free to modify and use it.

---

## 🚀 Contributing
Contributions are welcome! If you have improvements or bug fixes, feel free to **open a PR**.

---

## 🎯 Summary
✅ Conway's Game of Life **implemented in .NET 7**  
✅ **CQRS (MediatR) + Repository Pattern** for structured architecture  
✅ **PostgreSQL database** with **EF Core**  
✅ **Docker Compose** for containerized deployment  
✅ **Integration tests with TestContainers**  
✅ **Clean Architecture with separation of concerns**
