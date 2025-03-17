using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.Web.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ConwayGameOfLife.IntegrationTests;

public class GameTests : BaseIntegrationTest
{
    public GameTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateBoard_OnCreatedBoard_ShouldReturn201()
    {
        var request = new CreateBoardRequest(
            Name: "My Test Board",
            State: new bool[][]
            {
                new bool[] { true, false, true },
                new bool[] { false, true, true },
                new bool[] { false, false, true },
            });

        var response = await PostAsync("api/v1/Game", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var parsedResponse = await ParseResponse<Guid>(response);
        Assert.NotEqual(Guid.Empty, parsedResponse);

        var board = DbContext.Boards
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == parsedResponse);
        Assert.NotNull(board);
    }


    [Fact]
    public async Task CreateBoard_OnIncorrectState_ShouldReturn400()
    {
        var request = new CreateBoardRequest(
            Name: "My Test Board",
            State: new bool[][]
            {
                new bool[] { true, false, true },
                new bool[] { true, true },
                new bool[] { false, false, true },
            });

        var response = await PostAsync("api/v1/Game", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("")] //Empty name
    [InlineData("ts")] //Too Short
    public async Task CreateBoard_OnIncorrectName_ShouldReturn400(string name)
    {
        var request = new CreateBoardRequest(
            Name: name,
            State: new bool[][]
            {
                new bool[] { true, false, true },
                new bool[] { true, true },
                new bool[] { false, false, true },
            });

        var response = await PostAsync("api/v1/Game", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentBoardState_OnNewBoard_WithValidId_ShouldReturn200()
    {
        var state = new bool[][]
        {
            new bool[] { true, false, true },
            new bool[] { true, true, true },
            new bool[] { false, false, true },
        };
        var board = new Board
        {
            Name = "Test Get Board",
            InitialState = BoardState.FromJaggedArray(state)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response = await GetAsync($"api/v1/Game/{board.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var parsedResponse = await ParseResponse<CurrentBoardStateResponse>(response);
        Assert.NotEqual(Guid.Empty, parsedResponse.Id);
        Assert.Equal(board.Id, parsedResponse.Id);
        Assert.Equal(board.Name, parsedResponse.Name);
        Assert.Equal(state, parsedResponse.InitialState);
        Assert.Equal(state, parsedResponse.State);
        Assert.Equal(0, parsedResponse.CurrentStep);
        Assert.False(parsedResponse.IsCompleted);
    }

    [Fact]
    public async Task GetCurrentBoardState_OnCompletedBoard_WithValidId_ShouldReturn200()
    {
        var initialState = new bool[][]
        {
            new bool[] { true, false, true },
            new bool[] { true, true, true },
            new bool[] { false, false, true },
        };

        var finalState = new bool[][]
        {
            new bool[] { false, false, true },
            new bool[] { true, false, true },
            new bool[] { false, false, false },
        };

        var board = new Board
        {
            Name = "Test Get Board",
            InitialState = BoardState.FromJaggedArray(initialState)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        var boardExecution = new BoardExecution
        {
            IsFinal = true,
            State = BoardState.FromJaggedArray(finalState),
            Step = 2,
            BoardId = board.Id,
        };

        await DbContext.BoardExecutions.AddAsync(boardExecution);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response = await GetAsync($"api/v1/Game/{board.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var parsedResponse = await ParseResponse<CurrentBoardStateResponse>(response);
        Assert.NotEqual(Guid.Empty, parsedResponse.Id);
        Assert.Equal(board.Id, parsedResponse.Id);
        Assert.Equal(board.Name, parsedResponse.Name);
        Assert.Equal(initialState, parsedResponse.InitialState);
        Assert.Equal(finalState, parsedResponse.State);
        Assert.Equal(2, parsedResponse.CurrentStep);
        Assert.True(parsedResponse.IsCompleted);
    }

    [Fact]
    public async Task GetCurrentBoardState_OnInvalidId_ShouldReturn404()
    {
        var state = new bool[][]
        {
            new bool[] { true, false, true },
            new bool[] { true, true, true },
            new bool[] { false, false, true },
        };
        var board = new Board
        {
            Name = "Test Get Board",
            InitialState = BoardState.FromJaggedArray(state)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response = await GetAsync($"api/v1/Game/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetBoardState_OnLastStep_WithValidId_AndStep_ShouldReturn200()
    {
        var initialState = new bool[][]
        {
            new bool[] { true, false, true },
            new bool[] { true, true, true },
            new bool[] { false, false, true },
        };

        var finalState = new bool[][]
        {
            new bool[] { false, false, true },
            new bool[] { true, false, true },
            new bool[] { false, false, false },
        };

        var board = new Board
        {
            Name = "Test Get Board",
            InitialState = BoardState.FromJaggedArray(initialState)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        var boardExecution = new BoardExecution
        {
            IsFinal = true,
            State = BoardState.FromJaggedArray(finalState),
            Step = 2,
            BoardId = board.Id,
        };

        await DbContext.BoardExecutions.AddAsync(boardExecution);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response = await GetAsync($"api/v1/Game/{board.Id}/steps/{boardExecution.Step}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var parsedResponse = await ParseResponse<BoardStateResponse>(response);
        Assert.NotEqual(Guid.Empty, parsedResponse.Id);
        Assert.Equal(board.Id, parsedResponse.Id);
        Assert.Equal(board.Name, parsedResponse.Name);
        Assert.Equal(initialState, parsedResponse.InitialState);
        Assert.Equal(finalState, parsedResponse.State);
        Assert.Equal(2, parsedResponse.CurrentStep);
        Assert.True(parsedResponse.IsCompleted);
    }

    [Fact]
    public async Task GetBoardState_OnNoLastStep_WithValidId_AndStep_ShouldReturn200()
    {
        var initialState = new bool[][]
        {
            new bool[] { true, false, true },
            new bool[] { true, true, true },
            new bool[] { false, false, true },
        };

        var requestedState = new bool[][]
        {
            new bool[] { false, false, false },
            new bool[] { true, true, true },
            new bool[] { true, false, false },
        };

        var finalState = new bool[][]
        {
            new bool[] { false, false, true },
            new bool[] { true, false, true },
            new bool[] { false, false, false },
        };

        var board = new Board
        {
            Name = "Test Get Board",
            InitialState = BoardState.FromJaggedArray(initialState)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        var boardExecution = new BoardExecution
        {
            IsFinal = false,
            State = BoardState.FromJaggedArray(requestedState),
            Step = 2,
            BoardId = board.Id,
        };

        await DbContext.BoardExecutions.AddAsync(boardExecution);
        Assert.NotEqual(Guid.Empty, board.Id);

        var finalBoardExecution = new BoardExecution
        {
            IsFinal = true,
            State = BoardState.FromJaggedArray(finalState),
            Step = 5,
            BoardId = board.Id,
        };

        await DbContext.BoardExecutions.AddAsync(finalBoardExecution);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response = await GetAsync($"api/v1/Game/{board.Id}/steps/{boardExecution.Step}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var parsedResponse = await ParseResponse<BoardStateResponse>(response);
        Assert.NotEqual(Guid.Empty, parsedResponse.Id);
        Assert.Equal(board.Id, parsedResponse.Id);
        Assert.Equal(board.Name, parsedResponse.Name);
        Assert.Equal(initialState, parsedResponse.InitialState);
        Assert.Equal(requestedState, parsedResponse.State);
        Assert.Equal(5, parsedResponse.CurrentStep);
        Assert.True(parsedResponse.IsCompleted);
    }

    [Fact]
    public async Task GetBoardState_OnInvalidId_ShouldReturn404()
    {
        var state = new bool[][]
        {
            new bool[] { true, false, true },
            new bool[] { true, true, true },
            new bool[] { false, false, true },
        };
        var board = new Board
        {
            Name = "Test Get Board",
            InitialState = BoardState.FromJaggedArray(state)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response = await GetAsync($"api/v1/Game/{Guid.NewGuid()}/steps/2");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetBoardState_OnInvalidStep_ShouldReturn404()
    {
        var state = new bool[][]
        {
            new bool[] { true, false, true },
            new bool[] { true, true, true },
            new bool[] { false, false, true },
        };
        var board = new Board
        {
            Name = "Test Get Board",
            InitialState = BoardState.FromJaggedArray(state)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response = await GetAsync($"api/v1/Game/{board.Id}/steps/2");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CalculateBoardNextState_OnInvalidId_ShouldReturn404()
    {
        var state = new bool[][]
        {
            new bool[] { true, false, true },
            new bool[] { true, true, true },
            new bool[] { false, false, true },
        };
        var board = new Board
        {
            Name = "Test Next Board Step",
            InitialState = BoardState.FromJaggedArray(state)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response = await PatchAsync($"api/v1/Game/{Guid.NewGuid()}/next");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CalculateBoardNextState_OnBlinkerSchema_OnValidId_ShouldReturn200()
    {
        var state = new bool[][]
        {
            new bool[] { false, false, false },
            new bool[] { true, true, true },
            new bool[] { false, false, false }
        };
        var board = new Board
        {
            Name = "Test Next Board Step Blinker Schema",
            InitialState = BoardState.FromJaggedArray(state)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response1 = await PatchAsync($"api/v1/Game/{board.Id}/next");

        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

        var firstRunExpectedState = new bool[][]
        {
            new bool[] { false, true, false },
            new bool[] { false, true, false },
            new bool[] { false, true, false }
        };
        var parsedResponse1 = await ParseResponse<CurrentBoardStateResponse>(response1);

        Assert.NotEqual(Guid.Empty, parsedResponse1.Id);
        Assert.Equal(board.Id, parsedResponse1.Id);
        Assert.Equal(board.Name, parsedResponse1.Name);
        Assert.Equal(state, parsedResponse1.InitialState);
        Assert.Equal(firstRunExpectedState, parsedResponse1.State);
        Assert.Equal(1, parsedResponse1.CurrentStep);
        Assert.False(parsedResponse1.IsCompleted);

        var response2 = await PatchAsync($"api/v1/Game/{board.Id}/next");

        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

        var secondRunExpectedState = new bool[][]
        {
            new bool[] { false, false, false },
            new bool[] { true, true, true },
            new bool[] { false, false, false }
        };
        var parsedResponse2 = await ParseResponse<CurrentBoardStateResponse>(response2);

        Assert.NotEqual(Guid.Empty, parsedResponse2.Id);
        Assert.Equal(board.Id, parsedResponse2.Id);
        Assert.Equal(board.Name, parsedResponse2.Name);
        Assert.Equal(state, parsedResponse2.InitialState);
        Assert.Equal(secondRunExpectedState, parsedResponse2.State);
        Assert.Equal(2, parsedResponse2.CurrentStep);
        Assert.True(parsedResponse2.IsCompleted);

        var loadBoard = await DbContext.Boards
            .AsNoTracking()
            .Where(x => x.Id == board.Id)
            .Include(x => x.Executions)
            .FirstOrDefaultAsync();
        Assert.NotNull(loadBoard);
        Assert.NotNull(loadBoard.Executions);
        Assert.NotEmpty(loadBoard.Executions);
        Assert.Equal(2, loadBoard.Executions.Count);
        Assert.Equal(1, loadBoard.Executions.Count(x => x.IsFinal));
    }

    [Fact]
    public async Task CalculateBoardNextState_OnBlinkerSchema_OnValidId_WithNoValidCalculations_ShouldReturn409()
    {
        var state = new bool[][]
        {
            new bool[] { false, false, false },
            new bool[] { true, true, true },
            new bool[] { false, false, false }
        };
        var board = new Board
        {
            Name = "Test Next Board Step Blinker Schema",
            InitialState = BoardState.FromJaggedArray(state)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response1 = await PatchAsync($"api/v1/Game/{board.Id}/next");

        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

        var firstRunExpectedState = new bool[][]
        {
            new bool[] { false, true, false },
            new bool[] { false, true, false },
            new bool[] { false, true, false }
        };
        var parsedResponse1 = await ParseResponse<CurrentBoardStateResponse>(response1);

        Assert.NotEqual(Guid.Empty, parsedResponse1.Id);
        Assert.Equal(board.Id, parsedResponse1.Id);
        Assert.Equal(board.Name, parsedResponse1.Name);
        Assert.Equal(state, parsedResponse1.InitialState);
        Assert.Equal(firstRunExpectedState, parsedResponse1.State);
        Assert.Equal(1, parsedResponse1.CurrentStep);
        Assert.False(parsedResponse1.IsCompleted);

        var response2 = await PatchAsync($"api/v1/Game/{board.Id}/next");

        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

        var secondRunExpectedState = new bool[][]
        {
            new bool[] { false, false, false },
            new bool[] { true, true, true },
            new bool[] { false, false, false }
        };
        var parsedResponse2 = await ParseResponse<CurrentBoardStateResponse>(response2);

        Assert.NotEqual(Guid.Empty, parsedResponse2.Id);
        Assert.Equal(board.Id, parsedResponse2.Id);
        Assert.Equal(board.Name, parsedResponse2.Name);
        Assert.Equal(state, parsedResponse2.InitialState);
        Assert.Equal(secondRunExpectedState, parsedResponse2.State);
        Assert.Equal(2, parsedResponse2.CurrentStep);
        Assert.True(parsedResponse2.IsCompleted);

        var response3 = await PatchAsync($"api/v1/Game/{board.Id}/next");

        Assert.Equal(HttpStatusCode.Conflict, response3.StatusCode);

        var loadBoard = await DbContext.Boards
            .AsNoTracking()
            .Where(x => x.Id == board.Id)
            .Include(x => x.Executions)
            .FirstOrDefaultAsync();
        Assert.NotNull(loadBoard);
        Assert.NotNull(loadBoard.Executions);
        Assert.NotEmpty(loadBoard.Executions);
        Assert.Equal(2, loadBoard.Executions.Count);
        Assert.Equal(1, loadBoard.Executions.Count(x => x.IsFinal));
    }

    [Fact]
    public async Task GetBoardFinalState_OnInvalidId_ShouldReturn404()
    {
        var state = new bool[][]
        {
            new bool[] { true, false, true },
            new bool[] { true, true, true },
            new bool[] { false, false, true },
        };
        var board = new Board
        {
            Name = "Test Next Board Step",
            InitialState = BoardState.FromJaggedArray(state)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response = await GetAsync($"api/v1/Game/{Guid.NewGuid()}/final");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetBoardFinalState_OnBlinkerSchema_OnValidId_ShouldReturn200()
    {
        var state = new bool[][]
        {
            new bool[] { false, false, false },
            new bool[] { true, true, true },
            new bool[] { false, false, false }
        };
        var board = new Board
        {
            Name = "Test Next Board Step Blinker Schema",
            InitialState = BoardState.FromJaggedArray(state)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response = await GetAsync($"api/v1/Game/{board.Id}/final");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var finalExpectedState = new bool[][]
        {
            new bool[] { false, false, false },
            new bool[] { true, true, true },
            new bool[] { false, false, false }
        };
        var parsedResponse = await ParseResponse<CalculatedBoardStateResponse>(response);

        Assert.NotEqual(Guid.Empty, parsedResponse.Id);
        Assert.Equal(board.Id, parsedResponse.Id);
        Assert.Equal(board.Name, parsedResponse.Name);
        Assert.Equal(state, parsedResponse.InitialState);
        Assert.Equal(finalExpectedState, parsedResponse.State);
        Assert.Equal(2, parsedResponse.CurrentStep);
        Assert.True(parsedResponse.IsCompleted);
        Assert.Equal(2, parsedResponse.CalculatedSteps);

        var loadBoard = await DbContext.Boards
            .AsNoTracking()
            .Where(x => x.Id == board.Id)
            .Include(x => x.Executions)
            .FirstOrDefaultAsync();
        Assert.NotNull(loadBoard);
        Assert.NotNull(loadBoard.Executions);
        Assert.NotEmpty(loadBoard.Executions);
        Assert.Equal(2, loadBoard.Executions.Count);
        Assert.Equal(1, loadBoard.Executions.Count(x => x.IsFinal));
    }

    [Fact]
    public async Task CalculateBoardNextStepsState_OnInvalidId_ShouldReturn404()
    {
        var state = new bool[][]
        {
            new bool[] { true, false, true },
            new bool[] { true, true, true },
            new bool[] { false, false, true },
        };
        var board = new Board
        {
            Name = "Test Next Board Step",
            InitialState = BoardState.FromJaggedArray(state)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response = await PatchAsync($"api/v1/Game/{Guid.NewGuid()}/next/2");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CalculateBoardNextStepsState_OnBlinkerSchema_OnValidId_WhenMoreStepsThanAllowedProvided_ShouldReturn200()
    {
        var state = new bool[][]
        {
            new bool[] { false, false, false },
            new bool[] { true, true, true },
            new bool[] { false, false, false }
        };
        var board = new Board
        {
            Name = "Test Next Board Step Blinker Schema",
            InitialState = BoardState.FromJaggedArray(state)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response = await PatchAsync($"api/v1/Game/{board.Id}/next/10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var finalExpectedState = new bool[][]
        {
            new bool[] { false, false, false },
            new bool[] { true, true, true },
            new bool[] { false, false, false }
        };
        var parsedResponse = await ParseResponse<CalculatedBoardStateResponse>(response);

        Assert.NotEqual(Guid.Empty, parsedResponse.Id);
        Assert.Equal(board.Id, parsedResponse.Id);
        Assert.Equal(board.Name, parsedResponse.Name);
        Assert.Equal(state, parsedResponse.InitialState);
        Assert.Equal(finalExpectedState, parsedResponse.State);
        Assert.Equal(2, parsedResponse.CurrentStep);
        Assert.Equal(2, parsedResponse.CalculatedSteps);
        Assert.True(parsedResponse.IsCompleted);

        var loadBoard = await DbContext.Boards
            .AsNoTracking()
            .Where(x => x.Id == board.Id)
            .Include(x => x.Executions)
            .FirstOrDefaultAsync();
        Assert.NotNull(loadBoard);
        Assert.NotNull(loadBoard.Executions);
        Assert.NotEmpty(loadBoard.Executions);
        Assert.Equal(2, loadBoard.Executions.Count);
        Assert.Equal(1, loadBoard.Executions.Count(x => x.IsFinal));
    }

    [Fact]
    public async Task CalculateBoardNextStepsState_OnBlinkerSchema_OnValidId_WhenLessStepsThanAllowedProvided_ShouldReturn200()
    {
        var state = new bool[][]
        {
            new bool[] { false, false, false },
            new bool[] { true, true, true },
            new bool[] { false, false, false }
        };
        var board = new Board
        {
            Name = "Test Next Board Step Blinker Schema",
            InitialState = BoardState.FromJaggedArray(state)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response = await PatchAsync($"api/v1/Game/{board.Id}/next/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var finalExpectedState = new bool[][]
        {
            new bool[] { false, true, false },
            new bool[] { false, true, false },
            new bool[] { false, true, false }
        };
        var parsedResponse = await ParseResponse<CalculatedBoardStateResponse>(response);

        Assert.NotEqual(Guid.Empty, parsedResponse.Id);
        Assert.Equal(board.Id, parsedResponse.Id);
        Assert.Equal(board.Name, parsedResponse.Name);
        Assert.Equal(state, parsedResponse.InitialState);
        Assert.Equal(finalExpectedState, parsedResponse.State);
        Assert.Equal(1, parsedResponse.CurrentStep);
        Assert.Equal(1, parsedResponse.CalculatedSteps);
        Assert.False(parsedResponse.IsCompleted);

        var loadBoard = await DbContext.Boards
            .AsNoTracking()
            .Where(x => x.Id == board.Id)
            .Include(x => x.Executions)
            .FirstOrDefaultAsync();
        Assert.NotNull(loadBoard);
        Assert.NotNull(loadBoard.Executions);
        Assert.NotEmpty(loadBoard.Executions);
        Assert.Equal(1, loadBoard.Executions.Count);
    }

    [Fact]
    public async Task CalculateBoardNextStepsState_OnLowerSteps_ShouldReturn400()
    {
        var state = new bool[][]
        {
            new bool[] { true, false, true },
            new bool[] { true, true, true },
            new bool[] { false, false, true },
        };
        var board = new Board
        {
            Name = "Test Next Board Step",
            InitialState = BoardState.FromJaggedArray(state)
        };

        await DbContext.Boards.AddAsync(board);
        Assert.NotEqual(Guid.Empty, board.Id);

        await DbContext.SaveChangesAsync();

        var response = await PatchAsync($"api/v1/Game/{Guid.NewGuid()}/next/0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
