namespace Genesis.Skills.Runecrafting;

public record RunecraftingAltar(
    string Name,
    int Id,
    int RuneId,
    int RequiredLevel,
    double XpPerRune,
    int TalismanId,
    int TiaraId,
    int EntranceAltarId,
    Dictionary<int, int> Multipliers);
