namespace Genesis.Skills.Woodcutting;

public static class AxeData
{
    private static readonly Axe[] AxeList =
    {
        new("BRONZE_AXE", 1351, 1, 879, 1.00),  
        new("IRON_AXE", 1349, 1, 877, 1.00),    
        new("STEEL_AXE", 1353, 6, 875, 1.05),   
        new("BLACK_AXE", 1361, 11, 873, 1.07),  
        new("MITHRIL_AXE", 1355, 21, 871, 1.10),
        new("ADAMANT_AXE", 1357, 31, 869, 1.20),
        new("RUNE_AXE", 1359, 41, 867, 1.30),   
        new("DRAGON_AXE", 6739, 61, 2846, 1.50) 
    };

    private static readonly Dictionary<int, Axe> Axes = AxeList
        .ToDictionary(axe => axe.Id, axe => axe);

    public static Axe? GetAxe(int id) => Axes.TryGetValue(id, out var axe) ? axe : null;

    public record Axe(string Name, int Id, int RequiredLevel, int AnimationId, double Multiplier);
    
    public static List<int> GetAllAxeIds() => AxeList.Select(axe => axe.Id).ToList();
}