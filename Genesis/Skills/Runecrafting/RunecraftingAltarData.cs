namespace Genesis.Skills.Runecrafting;

public static class RunecraftingAltarData
{
    private static readonly RunecraftingAltar[] AltarList =
    {
        new("AIR_ALTAR", 2478, 556, 1, 5.0, 1438, 5527, new Dictionary<int, int>
        {
            { 1, 1 }, { 11, 2 }, { 22, 3 }, { 33, 4 }, { 44, 5 },
            { 55, 6 }, { 66, 7 }, { 77, 8 }, { 88, 9 }, { 99, 10 }
        }),
        new("MIND_ALTAR", 2479, 558, 2, 5.5, 1448, 5529, new Dictionary<int, int>
        {
            { 2, 1 }, { 14, 2 }, { 28, 3 }, { 42, 4 }, { 56, 5 },
            { 70, 6 }, { 84, 7 }, { 98, 8 }
        }),
        new("WATER_ALTAR", 2480, 555, 5, 6.0, 1444, 5531, new Dictionary<int, int>
        {
            { 5, 1 }, { 19, 2 }, { 38, 3 }, { 57, 4 }, { 76, 5 },
            { 95, 6 }
        }),
        new("EARTH_ALTAR", 2481, 557, 9, 6.5, 1440, 5533, new Dictionary<int, int>
        {
            { 9, 1 }, { 26, 2 }, { 52, 3 }, { 78, 4 }, { 104, 5 }
        }),
        new("FIRE_ALTAR", 2482, 554, 14, 7.0, 1442, 5537, new Dictionary<int, int>
        {
            { 14, 1 }, { 35, 2 }, { 70, 3 }
        }),
        new("BODY_ALTAR", 2483, 559, 20, 7.5, 1446, 5533, new Dictionary<int, int>
        {
            { 20, 1 }, { 46, 2 }, { 92, 3 }
        }),
        new("COSMIC_ALTAR", 2484, 564, 27, 8.0, 1454, 5539, new Dictionary<int, int>
        {
            { 27, 1 }, { 59, 2 }
        }),
        new("CHAOS_ALTAR", 2487, 562, 35, 8.5, 1452, 5543, new Dictionary<int, int>
        {
            { 35, 1 }, { 74, 2 }
        }),
        new("NATURE_ALTAR", 2486, 561, 44, 9.0, 1462, 5541, new Dictionary<int, int>
        {
            { 44, 1 }, { 91, 2 }
        }),
        new("LAW_ALTAR", 2485, 563, 54, 9.5, 1458, 5545, new Dictionary<int, int>
        {
            { 54, 1 }, { 95, 2 }
        }),
        new("DEATH_ALTAR", 2488, 560, 65, 10.0, 1456, 5547, new Dictionary<int, int>
        {
            { 65, 1 }, { 99, 2 }
        })
    };

    public static int GetMultiplierForLevel(Dictionary<int, int> multipliers, int level)
    {
        int multiplier = 1;

        foreach (var entry in multipliers)
        {
            if (level >= entry.Key) multiplier = entry.Value;
            else break;
        }

        return multiplier;
    }

    private static readonly Dictionary<int, RunecraftingAltar> Altars = AltarList
        .ToDictionary(altar => altar.Id, altar => altar);

    public static RunecraftingAltar? GetAltar(int id) => Altars.TryGetValue(id, out var altar) ? altar : null;

    public static List<int> GetAllAltarIds() => AltarList.Select(altar => altar.Id).ToList();

    public static List<int> GetAllTalismanIds() => AltarList.Select(altar => altar.TalismanId).ToList();

    public static List<int> GetAllTiaraIds() => AltarList.Select(altar => altar.TiaraId).ToList();
}