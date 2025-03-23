using System.Text.Json;

namespace Genesis.Definitions;

public class ItemDefinitions
{
    private static Dictionary<int, Definition> definitions = new();
    private static readonly int[] EMPTY_BONUSES = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    public static int[] GetBonus(int id)
    {
        if (definitions.TryGetValue(id, out var def) && def.Bonuses != null)
        {
            return def.Bonuses;
        }

        return EMPTY_BONUSES;
    }

    public static double GetWeight(int id)
    {
        return definitions.TryGetValue(id, out var def) ? def.Weight : 0.0;
    }

    public static void Load()
    {
        try
        {
            string json = File.ReadAllText("./ItemDefinitions.json");
            var items = JsonSerializer.Deserialize<ItemData[]>(json);

            if (items != null)
            {
                foreach (var item in items)
                {
                    definitions[item.Id] = new Definition(item);
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("items.json: file not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}