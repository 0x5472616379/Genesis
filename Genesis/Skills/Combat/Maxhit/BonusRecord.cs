namespace Genesis.Skills.Combat.Maxhit;

public record BonusRecord<TBonus>(string Name, TBonus Bonus)
{
    public static implicit operator TBonus(BonusRecord<TBonus> record) => record.Bonus;
}