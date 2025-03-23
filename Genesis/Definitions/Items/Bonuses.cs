using System.Text.Json.Serialization;

namespace Genesis.Definitions;

public class Bonuses
{
    [JsonPropertyName("attackStab")]
    public int AttackStab { get; set; } = 0;

    [JsonPropertyName("attackSlash")]
    public int AttackSlash { get; set; } = 0;

    [JsonPropertyName("attackCrush")]
    public int AttackCrush { get; set; } = 0;

    [JsonPropertyName("attackMagic")]
    public int AttackMagic { get; set; } = 0;

    [JsonPropertyName("attackRange")]
    public int AttackRange { get; set; } = 0;

    [JsonPropertyName("defenceStab")]
    public int DefenceStab { get; set; } = 0;

    [JsonPropertyName("defenceSlash")]
    public int DefenceSlash { get; set; } = 0;

    [JsonPropertyName("defenceCrush")]
    public int DefenceCrush { get; set; } = 0;

    [JsonPropertyName("defenceMagic")]
    public int DefenceMagic { get; set; } = 0;

    [JsonPropertyName("defenceRange")]
    public int DefenceRange { get; set; } = 0;

    [JsonPropertyName("strengthBonus")]
    public int StrengthBonus { get; set; } = 0;

    [JsonPropertyName("prayerBonus")]
    public int PrayerBonus { get; set; } = 0;

    public int[] GetBonuses()
    {
        return new int[]
        {
            AttackStab, AttackSlash, AttackCrush, AttackMagic, AttackRange,
            DefenceStab, DefenceSlash, DefenceCrush, DefenceMagic, DefenceRange,
            StrengthBonus, PrayerBonus
        };
    }
}