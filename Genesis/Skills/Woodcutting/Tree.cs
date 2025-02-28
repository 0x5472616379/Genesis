namespace Genesis.Skills.Woodcutting;



public record Tree(string Name,
    int[] Ids,
    int[]? Entities,
    int Level,
    double Xp,
    int LogId,
    int StumpId,
    int RespawnTime,
    int DecayChance);
