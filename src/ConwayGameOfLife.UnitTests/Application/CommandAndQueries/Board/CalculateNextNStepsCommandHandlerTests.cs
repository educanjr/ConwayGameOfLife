using ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateNextNSteps;
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

public class CalculateNextNStepsCommandHandlerTests
{
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly GameRullerConfig _config;
    private readonly CalculateNextNStepsCommandHandler _handler;

    public CalculateNextNStepsCommandHandlerTests()
    {
        _boardRepositoryMock = new Mock<IBoardRepository>();
        _config = new GameRullerConfig { MaxExecutionsAllowed = 10 };
        var options = Options.Create(_config);
        _handler = new CalculateNextNStepsCommandHandler(_boardRepositoryMock.Object, options);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenBoardResolvesStepsCorrectly()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var command = new CalculateNextNStepsCommand(boardId, Steps: 3);

        var initialState = BoardState.FromJaggedArray(new[]
        {
            new[] { false, true, false },
            new[] { false, true, false },
            new[] { false, true, false }
        });

        var board = new BoardEntity
        {
            Id = boardId,
            Name = "Test Board",
            InitialState = initialState,
            Executions = new List<BoardExecution>()
        };

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecutions(boardId))
            .ReturnsAsync(board);

        _boardRepositoryMock
            .Setup(r => r.AddExecutionsRange(It.IsAny<IList<BoardExecution>>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertSuccess();
        result.Value.Id.Should().Be(boardId);
        result.Value.CurrentStep.Should().BeGreaterThan(0);
        result.Value.CalculatedSteps.Should().BeGreaterThan(0);
        result.Value.InitialState.GetStateHash().Should().Be(initialState.GetStateHash());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenBoardIsNull()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var command = new CalculateNextNStepsCommand(boardId, Steps: 3);

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecutions(boardId))
            .ReturnsAsync((BoardEntity)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertFailure(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnRuleViolation_WhenExecutionLimitIsReached()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var command = new CalculateNextNStepsCommand(boardId, Steps: 3);

        var board = new BoardEntity
        {
            Id = boardId,
            Name = "TooManySteps",
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

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertFailure(ErrorCode.ApplicationRuleViolation);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenUnhandledExceptionIsThrown()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var command = new CalculateNextNStepsCommand(boardId, Steps: 2);

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecutions(boardId))
            .ThrowsAsync(new InvalidOperationException("Unexpected"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertFailure(ErrorCode.NotFound, "Unexpected");
    }
}
