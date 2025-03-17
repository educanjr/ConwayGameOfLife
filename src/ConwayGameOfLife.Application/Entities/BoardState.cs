using System.Security.Cryptography;
using System.Text;

namespace ConwayGameOfLife.Application.Entities;

public class BoardState
{
    public bool[,] State { get; set; }

    public static BoardState FromJaggedArray(bool[][] jaggedState)
    {
        int rows = jaggedState.Length;
        int cols = jaggedState[0].Length;
        bool[,] array2D = new bool[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                array2D[i, j] = jaggedState[i][j];
            }
        }

        return new()
        {
            State = array2D,
        };
    }

    public bool[][] ToJaggedArrayState()
    {
        int rows = State.GetLength(0);
        int cols = State.GetLength(1);

        bool[][] jaggedArray = new bool[rows][];

        for (int i = 0; i < rows; i++)
        {
            jaggedArray[i] = new bool[cols]; // Initialize each row
            for (int j = 0; j < cols; j++)
            {
                jaggedArray[i][j] = State[i, j];
            }
        }

        return jaggedArray;
    }

    public BoardState ComputeNextState()
    {
        int rows = State.GetLength(0);
        int cols = State.GetLength(1);

        int totalCells = rows * cols;
        int parallelThreshold = 5000;

        //Since each cell's next state is independent of others, we can use Parallel.For to calculate the next state
        var nextState = totalCells < parallelThreshold ? 
            ComputeNextStateSequential(State) : //Use sequential loop on small boards (Avoids parallel overhead)
            ComputeNextStateParallel(State);

        return new()
        {
            State = nextState
        };
    }

    private static bool[,] ComputeNextStateSequential(bool[,] board)
    {
        var  rows = board.GetLength(0);
        var  cols = board.GetLength(1);
        var nextState = new bool[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int liveNeighbors = CountLiveNeighbors(board, r, c);
                nextState[r, c] = ApplyConwayRules(board[r, c], liveNeighbors);
            }
        }

        return nextState;
    }

    private static bool[,] ComputeNextStateParallel(bool[,] board)
    {
        var rows = board.GetLength(0);
        var cols = board.GetLength(1);
        var nextState = new bool[rows, cols];

        Parallel.For(0, rows, r =>
        {
            for (int c = 0; c < cols; c++)
            {
                var liveNeighbors = CountLiveNeighbors(board, r, c);
                nextState[r, c] = ApplyConwayRules(board[r, c], liveNeighbors);
            }
        });

        return nextState;
    }

    private static int CountLiveNeighbors(bool[,] board, int row, int col)
    {
        var rows = board.GetLength(0);
        var cols = board.GetLength(1);
        var liveCount = 0;

        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;

                var nr = row + dr;
                var nc = col + dc;

                if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && board[nr, nc])
                    liveCount++;
            }
        }

        return liveCount;
    }

    private static bool ApplyConwayRules(bool isAlive, int liveNeighbors) =>
        isAlive ? liveNeighbors == 2 || liveNeighbors == 3 : liveNeighbors == 3;

    public string GetStateHash()
    {
        var rows = State.GetLength(0);
        var cols = State.GetLength(1);
        StringBuilder sb = new StringBuilder(rows * cols);

        //Flatten board state into a string representation
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                sb.Append(State[r, c] ? '1' : '0');
            }
        }

        //Hash the board state to create a unique identifier
        using SHA256 sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
        return Convert.ToBase64String(hashBytes);
    }
}
