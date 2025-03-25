using ConwayGameOfLife.Application.Entities;
using FluentAssertions;

namespace ConwayGameOfLife.UnitTests.Application.Entities;

public class BoardStateTests
{
    [Fact]
    public void FromJaggedArray_ShouldConvertCorrectly()
    {
        // Arrange
        bool[][] jagged = new[]
        {
            new[] { true, false },
            new[] { false, true }
        };

        // Act
        var boardState = BoardState.FromJaggedArray(jagged);

        // Assert
        boardState.State.GetLength(0).Should().Be(2);
        boardState.State.GetLength(1).Should().Be(2);
        boardState.State[0, 0].Should().BeTrue();
        boardState.State[0, 1].Should().BeFalse();
        boardState.State[1, 0].Should().BeFalse();
        boardState.State[1, 1].Should().BeTrue();
    }

    [Fact]
    public void ToJaggedArrayState_ShouldConvertBackCorrectly()
    {
        // Arrange
        var originalJagged = new[]
        {
            new[] { true, false },
            new[] { false, true }
        };

        var boardState = BoardState.FromJaggedArray(originalJagged);

        // Act
        var resultJagged = boardState.ToJaggedArrayState();

        // Assert
        resultJagged.Should().BeEquivalentTo(originalJagged);
    }

    [Fact]
    public void GetStateHash_ShouldReturnSameHash_ForSameState()
    {
        // Arrange
        var matrix1 = BoardState.FromJaggedArray(new[]
        {
            new[] { true, false },
            new[] { false, true }
        });

        var matrix2 = BoardState.FromJaggedArray(new[]
        {
            new[] { true, false },
            new[] { false, true }
        });

        // Act
        var hash1 = matrix1.GetStateHash();
        var hash2 = matrix2.GetStateHash();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void GetStateHash_ShouldReturnDifferentHash_ForDifferentStates()
    {
        // Arrange
        var matrix1 = BoardState.FromJaggedArray(new[]
        {
            new[] { true, false },
            new[] { false, true }
        });

        var matrix2 = BoardState.FromJaggedArray(new[]
        {
            new[] { false, true },
            new[] { true, false }
        });

        // Act
        var hash1 = matrix1.GetStateHash();
        var hash2 = matrix2.GetStateHash();

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void ComputeNextState_ShouldUseSequentialLogic_ForSmallBoard()
    {
        // Arrange
        var smallBoard = GenerateBoard(rows: 10, cols: 10, fillEveryNthCellAlive: 3); // 100 cells < 2500

        // Act
        var next = smallBoard.ComputeNextState();

        // Assert
        next.Should().NotBeNull();
        next.State.GetLength(0).Should().Be(10);
        next.State.GetLength(1).Should().Be(10);
    }

    [Fact]
    public void ComputeNextState_ShouldUseParallelLogic_ForLargeBoard()
    {
        // Arrange
        var largeBoard = GenerateBoard(rows: 60, cols: 60, fillEveryNthCellAlive: 4); // 3600 cells > 2500

        // Act
        var next = largeBoard.ComputeNextState();

        // Assert
        next.Should().NotBeNull();
        next.State.GetLength(0).Should().Be(60);
        next.State.GetLength(1).Should().Be(60);
    }

    [Fact]
    public void ComputeNextState_ShouldBeDeterministic_ForSameBoard()
    {
        // Arrange
        var board = GenerateBoard(rows: 20, cols: 20, fillEveryNthCellAlive: 5); // Small enough for sequential

        // Act
        var state1 = board.ComputeNextState().GetStateHash();
        var state2 = board.ComputeNextState().GetStateHash(); // Run again

        // Assert
        state1.Should().Be(state2);
    }

    private static BoardState GenerateBoard(int rows, int cols, int fillEveryNthCellAlive)
    {
        var jagged = new bool[rows][];
        int counter = 0;

        for (int i = 0; i < rows; i++)
        {
            jagged[i] = new bool[cols];
            for (int j = 0; j < cols; j++)
            {
                jagged[i][j] = counter++ % fillEveryNthCellAlive == 0;
            }
        }

        return BoardState.FromJaggedArray(jagged);
    }
}
