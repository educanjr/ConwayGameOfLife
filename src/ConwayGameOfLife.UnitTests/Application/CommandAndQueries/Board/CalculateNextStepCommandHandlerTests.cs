using ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateNextStep;
using ConwayGameOfLife.Application.Common;
using ConwayGameOfLife.Application.ConfigOptions;
using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.Application.Repositories;
using ConwayGameOfLife.UnitTests.Utils;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

using BoardEntity = ConwayGameOfLife.Application.Entities.Board;

namespace ConwayGameOfLife.UnitTests.Application.CommandAndQueries.Board;

public class CalculateNextStepCommandHandlerTests
{
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly GameRullerConfig _config;
    private readonly CalculateNextStepCommandHandler _handler;

    public CalculateNextStepCommandHandlerTests()
    {
        _boardRepositoryMock = new Mock<IBoardRepository>();
        _config = new GameRullerConfig { MaxExecutionsAllowed = 10 };
        var options = Options.Create(_config);
        _handler = new CalculateNextStepCommandHandler(_boardRepositoryMock.Object, options);
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenNextExecutionResolved()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var initialState = BoardState.FromJaggedArray(new[]
        {
            new[] { true, false },
            new[] { true, true }
        });

        var board = new BoardEntity
        {
            Id = boardId,
            Name = "Live Board",
            InitialState = initialState,
            Executions = new List<BoardExecution>()
        };

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecutions(boardId))
            .ReturnsAsync(board);

        _boardRepositoryMock
            .Setup(r => r.AddExecution(It.IsAny<BoardExecution>()))
            .ReturnsAsync(new BoardExecution())
            .Verifiable();

        var command = new CalculateNextStepCommand(boardId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertSuccess();
        result.Value.Id.Should().Be(board.Id);
        result.Value.Name.Should().Be(board.Name);
        result.Value.InitialState.GetStateHash().Should().Be(board.InitialState.GetStateHash());
        result.Value.CurrentStep.Should().BeGreaterThan(0);
        result.Value.State.Should().NotBeNull();
        _boardRepositoryMock.Verify(r => r.AddExecution(It.IsAny<BoardExecution>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenBoardIsNull()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecutions(boardId))
            .ReturnsAsync((BoardEntity)null!);

        var command = new CalculateNextStepCommand(boardId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertFailure(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_ReturnsRuleViolation_WhenExecutionLimitReached()
    {
        // Arrange
        var boardId = Guid.NewGuid();

        var board = new BoardEntity
        {
            Id = boardId,
            Name = "Dead Board",
            InitialState = BoardState.FromJaggedArray(new[]
            {
                new[] { false, false },
                new[] { false, false }
            }),
            Executions = new List<BoardExecution>
            {
                new BoardExecution
                {
                    Id = Guid.NewGuid(),
                    Step = _config.MaxExecutionsAllowed,
                    IsFinal = false,
                    State = BoardState.FromJaggedArray(new[]
                    {
                        new[] { false, false },
                        new[] { false, false }
                    })
                }
            }
        };

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecutions(boardId))
            .ReturnsAsync(board);

        var command = new CalculateNextStepCommand(boardId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertFailure(ErrorCode.ApplicationRuleViolation);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenUnhandledExceptionThrown()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecutions(boardId))
            .ThrowsAsync(new InvalidOperationException("DB connection failed"));

        var command = new CalculateNextStepCommand(boardId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertFailure(ErrorCode.NotFound, "DB connection failed");
    }
}
