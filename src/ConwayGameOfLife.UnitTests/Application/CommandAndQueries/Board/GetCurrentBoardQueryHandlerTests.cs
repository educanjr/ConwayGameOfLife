using ConwayGameOfLife.Application.CommandAndQueries.Board.GetCurrent;
using ConwayGameOfLife.Application.Common;
using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.Application.Repositories;
using ConwayGameOfLife.UnitTests.Utils;
using FluentAssertions;
using Moq;

using BoardEntity = ConwayGameOfLife.Application.Entities.Board;

namespace ConwayGameOfLife.UnitTests.Application.CommandAndQueries.Board;

public class GetCurrentBoardQueryHandlerTests
{
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly GetCurrentBoardQueryHandler _handler;

    public GetCurrentBoardQueryHandlerTests()
    {
        _boardRepositoryMock = new Mock<IBoardRepository>();
        _handler = new GetCurrentBoardQueryHandler(_boardRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsBoardState_WhenCurrentExecutionExists()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var latestExecution = new BoardExecution
        {
            Id = Guid.NewGuid(),
            Step = 3,
            IsFinal = true,
            State = BoardState.FromJaggedArray(new[]
            {
                new[] { true, false },
                new[] { false, true }
            }),
            BoardId = boardId
        };

        var board = new BoardEntity
        {
            Id = boardId,
            Name = "Live Board",
            InitialState = BoardState.FromJaggedArray(new[]
            {
                new[] { false, false },
                new[] { false, false }
            }),
            Executions = new List<BoardExecution> { latestExecution }
        };

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingOnlyCurrentExecution(boardId))
            .ReturnsAsync(board);

        var command = new GetCurrentBoardQuery(boardId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertSuccess();
        result.Value!.Id.Should().Be(boardId);
        result.Value.CurrentStep.Should().Be(3);
        result.Value.IsCompleted.Should().BeTrue();
        result.Value.State.GetStateHash().Should().Be(latestExecution.State.GetStateHash());
    }

    [Fact]
    public async Task Handle_ReturnsInitialState_WhenNoExecutionsExist()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var initialState = BoardState.FromJaggedArray(new[]
        {
            new[] { true, true },
            new[] { false, false }
        });

        var board = new BoardEntity
        {
            Id = boardId,
            Name = "Initial Only",
            InitialState = initialState,
            Executions = new List<BoardExecution>() // no executions
        };

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingOnlyCurrentExecution(boardId))
            .ReturnsAsync(board);

        var command = new GetCurrentBoardQuery(boardId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertSuccess();
        result.Value!.CurrentStep.Should().Be(0);
        result.Value.IsCompleted.Should().BeFalse();
        result.Value.State.GetStateHash().Should().Be(initialState.GetStateHash());
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenBoardIsNull()
    {
        // Arrange
        var boardId = Guid.NewGuid();

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingOnlyCurrentExecution(boardId))
            .ReturnsAsync((BoardEntity)null!);

        var command = new GetCurrentBoardQuery(boardId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertFailure(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenExceptionThrown()
    {
        // Arrange
        var boardId = Guid.NewGuid();

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingOnlyCurrentExecution(boardId))
            .ThrowsAsync(new InvalidOperationException("Unexpected"));

        var command = new GetCurrentBoardQuery(boardId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertFailure(ErrorCode.NotFound, "Unexpected");
    }
}
