﻿using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.Application.Exceptions;
using FluentAssertions;

namespace ConwayGameOfLife.UnitTests.Application.Entities;

public class BoardTests
{
    [Fact]
    public void GetLatestExecution_ShouldReturnNull_WhenNoExecutionsExist()
    {
        var board = new Board();

        board.GetLatestExecution().Should().BeNull();
    }

    [Fact]
    public void GetLatestExecution_ShouldReturnHighestStepExecution()
    {
        var expectedId = Guid.NewGuid();
        var board = new Board
        {
            Executions = new List<BoardExecution>
            {
                new() { Id = Guid.NewGuid(), Step = 1 },
                new() { Id = expectedId, Step = 3 },
                new() { Id = Guid.NewGuid(), Step = 2 }
            }
        };

        var result = board.GetLatestExecution();
        result!.Step.Should().Be(3);
        result!.Id.Should().Be(expectedId);
    }

    [Fact]
    public void GetExecution_ShouldReturnMatchingStep()
    {
        var expectedId = Guid.NewGuid();
        var board = new Board
        {
            Executions = new List<BoardExecution>
            {
                new() { Id = Guid.NewGuid(), Step = 2 },
                new() { Id = expectedId, Step = 5 }
            }
        };

        var result = board.GetExecution(5);
        result.Should().NotBeNull();
        result!.Step.Should().Be(5);
        result!.Id.Should().Be(expectedId);
    }

    [Fact]
    public void GetExecution_ShouldReturnNull_WhenStepDoesNotExist()
    {
        var board = new Board
        {
            Executions = new List<BoardExecution>
            {
                new() { Step = 2 }
            }
        };

        board.GetExecution(99).Should().BeNull();
    }

    [Fact]
    public void Should_ComputeFirstExecution_WhenNoExecutionsExist()
    {
        // Arrange
        var board = new Board
        {
            InitialState = CreateBlinker()
        };

        // Act
        var result = board.ResolveNextExecution(maxExecutionsAllowed: 10);

        // Assert
        result.Step.Should().Be(1);
        result.State.Should().NotBeNull();
        board.Executions.Should().HaveCount(1);
    }

    [Fact]
    public void Should_Throw_When_LatestExecutionIsFinal()
    {
        // Arrange
        var board = new Board
        {
            Executions = new List<BoardExecution>
            {
                new()
                {
                    Step = 5,
                    IsFinal = true,
                    State = CreateBlinker()
                }
            }
        };

        // Act
        Action act = () => board.ResolveNextExecution(maxExecutionsAllowed: 10);

        // Assert
        act.Should().Throw<ExecutionLimitReachedException>();
    }

    [Fact]
    public void Should_Throw_When_MaxExecutionReached()
    {
        // Arrange
        var board = new Board
        {
            Executions = new List<BoardExecution>
            {
                new()
                {
                    Step = 10,
                    IsFinal = false,
                    State = CreateBlinker()
                }
            }
        };

        // Act
        Action act = () => board.ResolveNextExecution(maxExecutionsAllowed: 10);

        // Assert
        act.Should().Throw<ExecutionLimitReachedException>();
    }

    [Fact]
    public void Should_MarkAsFinal_IfNextStateEqualsInitialState()
    {
        // Arrange
        var state = CreateBlinker();

        var board = new Board
        {
            InitialState = state,
            Executions = new List<BoardExecution>
            {
                new()
                {
                    Step = 1,
                    IsFinal = false,
                    State = state.ComputeNextState()
                }
            }
        };

        // Act
        var result = board.ResolveNextExecution(maxExecutionsAllowed: 10);

        // Assert
        result.IsFinal.Should().BeTrue();
    }

    [Fact]
    public void Should_MarkAsFinal_IfNextStateMatchesPreviousExecution()
    {
        // Arrange
        var state = CreateBlinker();
        var repeatingState = state.ComputeNextState();

        var board = new Board
        {
            InitialState = state,
            Executions = new List<BoardExecution>
            {
                new() { Step = 1, State = state, IsFinal = false },
                new() { Step = 2, State = repeatingState, IsFinal = false }
            }
        };

        // Act
        var result = board.ResolveNextExecution(maxExecutionsAllowed: 10);

        // Assert
        result.Step.Should().Be(3);
        result.IsFinal.Should().BeTrue();
    }

    [Fact]
    public void Should_ContinueWithNextStep_IfWithinLimit_AndNotFinal()
    {
        // Arrange
        var board = new Board
        {
            InitialState = CreateBlinker(),
            Executions = new List<BoardExecution>
            {
                new() { Step = 1, State = CreateBlinker(), IsFinal = false }
            }
        };

        // Act
        var result = board.ResolveNextExecution(maxExecutionsAllowed: 10);

        // Assert
        result.Step.Should().Be(2);
        result.IsFinal.Should().BeFalse();
    }


    [Fact]
    public void Should_ComputeNExecutions_WhenNoExecutionsExist()
    {
        // Arrange
        var board = new Board
        {
            InitialState = CreateBlinker()
        };

        // Act
        var result = board.ResolveNextExecution(executionsToResolve: 3, maxExecutionsAllowed: 10);

        // Assert
        result.Step.Should().BeGreaterThan(0);
        board.Executions.Should().HaveCountLessThanOrEqualTo(3);
    }

    [Fact]
    public void Should_ContinueFromLatestExecution()
    {
        // Arrange
        var state = CreateBlinker();
        var board = new Board
        {
            InitialState = state,
            Executions = new List<BoardExecution>
            {
                new() { Step = 2, State = state.ComputeNextState(), IsFinal = false }
            }
        };

        // Act
        var result = board.ResolveNextExecution(2, 10);

        // Assert
        result.Step.Should().BeGreaterThan(2);
        board.Executions.Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public void Should_StopEarly_IfFinalStateIsReached()
    {
        // Arrange
        var board = new Board
        {
            InitialState = CreateBlinker()
        };

        // Act
        var result = board.ResolveNextExecution(10, 10); // try to compute more than needed

        // Assert
        result.IsFinal.Should().BeTrue();
        board.Executions!.Count.Should().BeLessThanOrEqualTo(10);
    }

    [Fact]
    public void Should_Throw_IfTargetStepExceedsLimit()
    {
        // Arrange
        var board = new Board
        {
            Executions = new List<BoardExecution>
            {
                new() { Step = 9, IsFinal = false, State = CreateBlinker() }
            }
        };

        // Act
        Action act = () => board.ResolveNextExecution(executionsToResolve: 3, maxExecutionsAllowed: 10);

        // Assert
        act.Should().Throw<ExecutionLimitReachedException>();
    }

    [Fact]
    public void Should_NotAddMoreThanNExecutions()
    {
        // Arrange
        var board = new Board
        {
            InitialState = CreateBlinker()
        };

        // Act
        var result = board.ResolveNextExecution(executionsToResolve: 2, maxExecutionsAllowed: 10);

        // Assert
        board.Executions!.Count.Should().BeLessThanOrEqualTo(2);
    }

    [Fact]
    public void Should_ComputeInitialPlusN_WhenNoExecutionsExist()
    {
        // Arrange
        var board = new Board
        {
            InitialState = CreateBlinker()
        };

        // Act
        var result = board.ResolveNextExecution(3, 10);

        // Assert
        result.Step.Should().BeGreaterThanOrEqualTo(2); //Blinker repeats every 2 states
        board.Executions!.Count.Should().BeLessThanOrEqualTo(2);
    }

    [Fact]
    public void Should_ComputeFromInitialState_WhenNoExecutionsExist()
    {
        // Arrange
        var board = new Board
        {
            InitialState = CreateBlinker()
        };

        // Act
        var finalExecution = board.ResolveFinalExecution(10);

        // Assert
        finalExecution.Step.Should().Be(2); // Blinker repeats at step 2
        finalExecution.IsFinal.Should().BeTrue();
        board.Executions!.Count.Should().Be(2);
    }

    [Fact]
    public void Should_NotCompute_WhenLatestExecutionIsAlreadyFinal()
    {
        // Arrange
        var board = new Board
        {
            Executions = new List<BoardExecution>
            {
                new()
                {
                    Step = 3,
                    IsFinal = true,
                    State = CreateBlinker()
                }
            }
        };

        // Act
        var finalExecution = board.ResolveFinalExecution(10);

        // Assert
        finalExecution.Step.Should().Be(3);
        finalExecution.IsFinal.Should().BeTrue();
        board.Executions!.Count.Should().Be(1);
    }

    [Fact]
    public void Should_ThrowExecutionLimitReachedException_WhenMaxExecutionsReached()
    {
        // Arrange
        var board = new Board
        {
            InitialState = CreateBlinker()
        };

        // Act
        Action act = () => board.ResolveFinalExecution(maxExecutionsAllowed: 1);

        // Assert
        act.Should().Throw<ExecutionLimitReachedException>();
    }

    [Fact]
    public void Should_ReachFinalState_WhenCycleDetected()
    {
        // Arrange
        var board = new Board
        {
            InitialState = CreateBlinker()
        };

        // Act
        var finalExecution = board.ResolveFinalExecution(10);

        // Assert
        finalExecution.Step.Should().Be(2); // Cycle detected
        finalExecution.IsFinal.Should().BeTrue();
        board.Executions!.Count.Should().Be(2);
    }

    [Fact]
    public void Should_CalculateUntilFinalOrMaxReached()
    {
        // Arrange
        var board = new Board
        {
            InitialState = CreateBlinker()
        };

        // Act
        var finalExecution = board.ResolveFinalExecution(5);

        // Assert
        finalExecution.Step.Should().BeLessThanOrEqualTo(5);
        finalExecution.IsFinal.Should().BeTrue();
    }

    private static BoardState CreateBlinker()
    {
        return BoardState.FromJaggedArray(new[]
        {
            new[] { false, true, false },
            new[] { false, true, false },
            new[] { false, true, false }
        });
    }
}
