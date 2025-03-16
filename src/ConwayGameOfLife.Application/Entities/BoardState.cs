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
}
