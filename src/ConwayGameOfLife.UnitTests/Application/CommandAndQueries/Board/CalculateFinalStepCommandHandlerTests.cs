using ConwayGameOfLife.Application.CommandAndQueries.Board.CalculateFinalStep;
using ConwayGameOfLife.Application.Common;
using ConwayGameOfLife.Application.ConfigOptions;
using ConwayGameOfLife.Application.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using BoardEntity = ConwayGameOfLife.Application.Entities.Board;
using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.UnitTests.Utils;

namespace ConwayGameOfLife.UnitTests.Application.CommandAndQueries.Board;

public class CalculateFinalStepCommandHandlerTests
{
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly GameRullerConfig _config;
    private readonly CalculateFinalStepCommandHandler _handler;

    public CalculateFinalStepCommandHandlerTests()
    {
        _boardRepositoryMock = new Mock<IBoardRepository>();
        _config = new GameRullerConfig { MaxExecutionsAllowed = 5 };
        var options = Options.Create(_config);
        _handler = new CalculateFinalStepCommandHandler(_boardRepositoryMock.Object, options);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_AndCallAddExecutions_WhenBoardIsResolvedToFinalState()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var initialState = BoardState.FromJaggedArray(new[]
        {
            new[] { false, true, false },
            new[] { false, true, false },
            new[] { false, true, false }
        });

        var board = new BoardEntity
        {
            Id = boardId,
            Name = "Oscillator",
            InitialState = initialState,
            Executions = new List<BoardExecution>()
        };

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecutions(boardId))
            .ReturnsAsync(board);

        _boardRepositoryMock
            .Setup(r => r.AddExecutionsRange(It.IsAny<IList<BoardExecution>>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var command = new CalculateFinalStepCommand(boardId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertSuccess();
        result.Value.Id.Should().Be(boardId);
        result.Value.Name.Should().Be("Oscillator");
        result.Value.InitialState.GetStateHash().Should().Be(initialState.GetStateHash());
        result.Value.CalculatedSteps.Should().BeGreaterThan(0);
        result.Value.CurrentStep.Should().BeLessThanOrEqualTo(_config.MaxExecutionsAllowed);

        _boardRepositoryMock
            .Verify(r => r.AddExecutionsRange(It.IsAny<IList<BoardExecution>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundResult_WhenBoardDoesNotExist()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecutions(boardId))
            .ReturnsAsync((BoardEntity)null!);

        var command = new CalculateFinalStepCommand(boardId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertFailure(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnRuleViolation_WhenExecutionLimitIsReachedImmediately()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var initialState = BoardState.FromJaggedArray(new[]
        {
            new[] { false, false },
            new[] { false, false }
        });

        var board = new BoardEntity
        {
            Id = boardId,
            Name = "Empty",
            InitialState = initialState,
            Executions = new List<BoardExecution>
            {
                new() {
                    Id = Guid.NewGuid(),
                    Step = _config.MaxExecutionsAllowed,
                    IsFinal = false,
                    State = initialState
                }
            }
        };

        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecutions(boardId))
            .ReturnsAsync(board);

        var command = new CalculateFinalStepCommand(boardId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertFailure(ErrorCode.ApplicationRuleViolation);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_OnUnexpectedException()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        _boardRepositoryMock
            .Setup(r => r.GetBoardIncludingExecutions(boardId))
            .ThrowsAsync(new InvalidOperationException("Unexpected DB failure"));

        var command = new CalculateFinalStepCommand(boardId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertFailure(ErrorCode.NotFound, "Unexpected DB failure");
    }
}
