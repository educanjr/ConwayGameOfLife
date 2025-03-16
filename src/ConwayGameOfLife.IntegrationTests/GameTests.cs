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
        Assert.Equal(state, parsedResponse.CurrentState);
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
        Assert.Equal(finalState, parsedResponse.CurrentState);
        Assert.Equal(2, parsedResponse.CurrentStep);
        Assert.True(parsedResponse.IsCompleted);
    }

    [Fact]
    public async Task GetCurrentBoardState_OninvalidId_ShouldReturn404()
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
}
