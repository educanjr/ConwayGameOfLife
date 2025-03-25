using ConwayGameOfLife.Application.CommandAndQueries.Board.Register;
using ConwayGameOfLife.Application.Common;
using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.Application.Repositories;
using ConwayGameOfLife.UnitTests.Utils;
using Moq;

using BoardEntity = ConwayGameOfLife.Application.Entities.Board;

namespace ConwayGameOfLife.UnitTests.Application.CommandAndQueries.Board;

public class RegisterBoardCommandHandlerTests
{
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly RegisterBoardCommandHandler _handler;

    public RegisterBoardCommandHandlerTests()
    {
        _boardRepositoryMock = new Mock<IBoardRepository>();
        _handler = new RegisterBoardCommandHandler(_boardRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenBoardIsCreated()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var state = BoardState.FromJaggedArray(new[]
        {
            new[] { true, false },
            new[] { false, true }
        });

        var command = new RegisterBoardCommand("Test Board", state);

        _boardRepositoryMock
            .Setup(r => r.RegisterBoard(command.Name, command.State))
            .ReturnsAsync(new BoardEntity
            {
                Id = boardId,
                Name = command.Name,
                InitialState = state,
                Executions = new List<BoardExecution>()
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertSuccess();

        _boardRepositoryMock
            .Verify(r => r.RegisterBoard(command.Name, command.State), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenExceptionIsThrown()
    {
        // Arrange
        var command = new RegisterBoardCommand("Failing Board", BoardState.FromJaggedArray(new[]
        {
            new[] { false }
        }));

        _boardRepositoryMock
            .Setup(r => r.RegisterBoard(It.IsAny<string>(), It.IsAny<BoardState>()))
            .ThrowsAsync(new InvalidOperationException("Database insert failed"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AssertFailure(ErrorCode.InternalError, "Database insert failed");
    }
}
