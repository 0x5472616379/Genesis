namespace Genesis.Skills.Woodcutting;

static class AxeData
{
    private static readonly Axe[] AxeList =
    {
        new("BRONZE_AXE", 1351, 1, 879),
        new("IRON_AXE", 1349, 1, 877),
        new("STEEL_AXE", 1353, 6, 875),
        new("BLACK_AXE", 1361, 11, 873),
        new("MITHRIL_AXE", 1355, 21, 871),
        new("ADAMANT_AXE", 1357, 31, 869),
        new("RUNE_AXE", 1359, 41, 867),
        new("DRAGON_AXE", 6739, 61, 2846)
    };

    private static readonly Dictionary<int, Axe> Axes = AxeList
        .ToDictionary(axe => axe.Id, axe => axe);

    public static Axe? GetAxe(int id) => Axes.TryGetValue(id, out var axe) ? axe : null;

    public record Axe(string Name, int Id, int RequiredLevel, int AnimationId);
}

