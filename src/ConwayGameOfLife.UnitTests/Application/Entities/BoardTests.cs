using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.Application.Exceptions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
