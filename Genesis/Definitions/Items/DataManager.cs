using System.Text.Json;

namespace Genesis.Definitions;

public class DataManager<T>
{
    private readonly Dictionary<int, T> dataById = new();

    public DataManager(string filePath, Func<T, int> idSelector)
    {
        var data = ParseJsonFile<List<T>>(filePath);
        if (data != null)
        {
            foreach (var item in data)
            {
                int id = idSelector(item);
                dataById[id] = item;
            }
        }
    }

    public T GetById(int id)
    {
        return dataById.TryGetValue(id, out var value) ? value : default;
    }

    private static TType ParseJsonFile<TType>(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<TType>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing {filePath}: {ex.Message}");
            throw;
        }
    }
}
