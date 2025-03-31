using System.Text.Json;

namespace Genesis.Definitions.Items;

public class DataManager<T>
{
    private readonly Dictionary<int, T> dataById = [];

    public DataManager(string filePath, Func<T, int> idSelector)
    {
        var data = ParseJsonFile<List<T>>(filePath);
        foreach (var item in data)
        {
            int id = idSelector(item);
            dataById[id] = item;
        }
    }

    public T GetById(int id) =>
        (dataById.TryGetValue(id, out var value) ? value : default) ?? 
        throw new InvalidOperationException("Default value cannot be null.");

    private static readonly JsonSerializerOptions _serializerOptions = new(){ PropertyNameCaseInsensitive = true };
    private static TType ParseJsonFile<TType>(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<TType>(json, _serializerOptions) ?? 
                throw new InvalidDataException($"Invalid Data found when parsing file for type '{typeof(TType)}'!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing {filePath}: {ex.Message}");
            throw;
        }
    }
}
