using ConwayGameOfLife.Application.CommandAndQueries.Board.GetStep;
using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.Application.Repositories;
using Moq;
using FluentAssertions;
using ConwayGameOfLife.UnitTests.Utils;
using ConwayGameOfLife.Application.Common;

using BoardEntity = ConwayGameOfLife.Application.Entities.Board;

namespace ConwayGameOfLife.UnitTests.Application.CommandAndQueries.Board;
public class GetBoardStepQueryHandlerTests
{
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly GetBoardStepQueryHandler _handler;

    public GetBoardStepQueryHandlerTests()
    {
        _boardRepositoryMock = new Mock<IBoardRepository>();
        _handler = new GetBoardStepQueryHandler(_boardRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnExecution_WhenStepIsFinal()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var step = 2;

        var execution = new BoardExecution
        {
            Step = step,
            IsFinal = true,
            State = BoardState.FromJaggedArray(new[] { new[] { true, false }, new[] { false, false } }),
            Id = Guid.NewGuid(),
            BoardId = boardId
        };

        var board = new BoardEntity
        {
            Id = boardId,
            Name = "FinalStepBoard",
            InitialState = BoardState.FromJaggedArray(new[] { new[] { false, false }, new[] { true, false } }),
            Executions = new List<BoardExecution> { execution }
        };

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecution(boardId, (uint)step))
            .ReturnsAsync(board);

        var query = new GetBoardStepQuery(boardId, (uint)step);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.AssertSuccess();
        result.Value.CurrentStep.Should().Be(step);
        result.Value.IsCompleted.Should().BeTrue();
        result.Value.State.Should().Be(execution.State);
    }

    [Fact]
    public async Task Handle_ShouldReturnLatestStep_WhenStepIsNotFinal()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var step = 2;

        var execution = new BoardExecution
        {
            Step = step,
            IsFinal = false,
            State = BoardState.FromJaggedArray(new[] { new[] { true } }),
            Id = Guid.NewGuid(),
            BoardId = boardId
        };

        var board = new BoardEntity
        {
            Id = boardId,
            Name = "NonFinalStep",
            InitialState = BoardState.FromJaggedArray(new[] { new[] { false } }),
            Executions = new List<BoardExecution> { execution }
        };

        var latestExecution = new BoardExecution
        {
            Step = 4,
            IsFinal = true,
            State = BoardState.FromJaggedArray(new[] { new[] { false } }),
            Id = Guid.NewGuid(),
            BoardId = boardId
        };

        var boardWithLatest = new BoardEntity
        {
            Id = boardId,
            Name = "NonFinalStep",
            InitialState = board.InitialState,
            Executions = new List<BoardExecution> { latestExecution }
        };

        _boardRepositoryMock.Setup(r => r.GetBoardIncludingExecution(boardId, (uint)step)).ReturnsAsync(board);
        _boardRepositoryMock.Setup(r => r.GetBoardIncludingOnlyCurrentExecution(boardId)).ReturnsAsync(boardWithLatest);

        var query = new GetBoardStepQuery(boardId, (uint)step);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.AssertSuccess();
        result.Value.CurrentStep.Should().Be(4);
        result.Value.IsCompleted.Should().BeTrue();
        result.Value.State.GetStateHash().Should().Be(execution.State.GetStateHash()); // still returns requested step state
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenBoardIsNull()
    {
        var boardId = Guid.NewGuid();
        var query = new GetBoardStepQuery(boardId, 1);

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecution(boardId, 1))
            .ReturnsAsync((BoardEntity)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.AssertFailure(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenExecutionNotFoundInBoard()
    {
        var boardId = Guid.NewGuid();
        var board = new BoardEntity
        {
            Id = boardId,
            Name = "MissingStep",
            InitialState = BoardState.FromJaggedArray(new[] { new[] { false } }),
            Executions = new List<BoardExecution>()
        };

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecution(boardId, 1))
            .ReturnsAsync(board);

        var query = new GetBoardStepQuery(boardId, 1);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.AssertFailure(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenSecondBoardFetchFails()
    {
        var boardId = Guid.NewGuid();
        var step = 2;

        var execution = new BoardExecution
        {
            Step = step,
            IsFinal = false,
            State = BoardState.FromJaggedArray(new[] { new[] { true } }),
            Id = Guid.NewGuid(),
            BoardId = boardId
        };

        var board = new BoardEntity
        {
            Id = boardId,
            Executions = new List<BoardExecution> { execution },
            InitialState = BoardState.FromJaggedArray(new[] { new[] { false } })
        };

        _boardRepositoryMock.Setup(r => r.GetBoardIncludingExecution(boardId, (uint)step)).ReturnsAsync(board);
        _boardRepositoryMock.Setup(r => r.GetBoardIncludingOnlyCurrentExecution(boardId)).ReturnsAsync((BoardEntity)null!);

        var query = new GetBoardStepQuery(boardId, (uint)step);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.AssertFailure(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenLatestExecutionIsNull()
    {
        var boardId = Guid.NewGuid();
        var step = 2;

        var execution = new BoardExecution
        {
            Step = step,
            IsFinal = false,
            State = BoardState.FromJaggedArray(new[] { new[] { true } }),
            Id = Guid.NewGuid(),
            BoardId = boardId
        };

        var board = new BoardEntity
        {
            Id = boardId,
            Executions = new List<BoardExecution> { execution },
            InitialState = BoardState.FromJaggedArray(new[] { new[] { false } })
        };

        var emptyBoard = new BoardEntity
        {
            Id = boardId,
            Executions = new List<BoardExecution>(),
            InitialState = board.InitialState
        };

        _boardRepositoryMock.Setup(r => r.GetBoardIncludingExecution(boardId, (uint)step)).ReturnsAsync(board);
        _boardRepositoryMock.Setup(r => r.GetBoardIncludingOnlyCurrentExecution(boardId)).ReturnsAsync(emptyBoard);

        var query = new GetBoardStepQuery(boardId, (uint)step);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.AssertFailure(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_OnUnexpectedException()
    {
        var boardId = Guid.NewGuid();
        var query = new GetBoardStepQuery(boardId, 1);

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecution(boardId, 1))
            .ThrowsAsync(new InvalidOperationException("Something went wrong"));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.AssertFailure(ErrorCode.NotFound, "Something went wrong");
    }
}
