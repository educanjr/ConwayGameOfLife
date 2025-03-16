using ConwayGameOfLife.Application.Entities;
using Newtonsoft.Json;

namespace ConwayGameOfLife.Data.Utils;

internal static class DataConversionHelper
{
    public static string SerializeBoardState(BoardState state) => 
        JsonConvert.SerializeObject(state.ToJaggedArrayState());
    
    public static BoardState DeserializeBoardState(string state)
    {
        var jaggedSatate = JsonConvert.DeserializeObject<bool[][]>(state);
        return jaggedSatate is null ? default! :
            BoardState.FromJaggedArray(jaggedSatate);
    }
}
