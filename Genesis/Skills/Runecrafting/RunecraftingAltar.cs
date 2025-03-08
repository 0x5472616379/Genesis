namespace Genesis.Skills.Runecrafting;

public record RunecraftingAltar(
    string Name,
    int Id,
    int RuneId,
    int RequiredLevel,
    double XpPerRune,
    int TalismanId,
    int TiaraId,
    Dictionary<int, int> Multipliers);
