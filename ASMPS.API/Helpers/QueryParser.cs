using System.Text.Json;

namespace ASMPS.API.Helpers;

/// <summary>
/// Парсер данных строки
/// </summary>
public class QueryParser
{
    public List<object>? ParseQueryToList(string jsonString)
    {
        var queryLists = JsonSerializer.Deserialize<List<object>>(jsonString);

        return queryLists;
    }
    
    public object? ParseQueryToObj(string jsonString)
    {
        var queryObject = JsonSerializer.Deserialize<object>(jsonString);

        return queryObject;
    }
}