using ConwayGameOfLife.Application.Entities;
using System.Diagnostics;
using Xunit.Abstractions;

namespace ConwayGameOfLife.UnitTests.Application.Entities;

public class BoardStatePerformanceTests : IDisposable
{
    private readonly ITestOutputHelper _output;

    public BoardStatePerformanceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public void Dispose() { }


    [Theory]
    [InlineData(10)]    // 10x10
    [InlineData(30)]    // 30x30
    [InlineData(50)]    // 50x50
    [InlineData(100)]   // 100x100
    [InlineData(200)]   // 200x200
    [InlineData(300)]   // 300x300
    [InlineData(500)]   // 500x500
    [InlineData(1500)]  // 1500x1500
    public void ComputeNextState_Performance_Test(int size)
    {
        var state = new bool[size, size];
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                state[i, j] = (i + j) % 2 == 0; // seed pattern

        var boardState = new BoardState { State = state };

        var stopwatch = Stopwatch.StartNew();
        var next = boardState.ComputeNextState();
        stopwatch.Stop();

        var elapsedMs = stopwatch.ElapsedMilliseconds;
        var elapsedTicks = stopwatch.ElapsedTicks;
        Assert.NotNull(next);

        _output.WriteLine($"{size}x{size}: {elapsedMs} ms, {elapsedTicks}");
    }
}
