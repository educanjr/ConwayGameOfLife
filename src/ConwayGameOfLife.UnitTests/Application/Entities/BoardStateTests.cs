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
}
