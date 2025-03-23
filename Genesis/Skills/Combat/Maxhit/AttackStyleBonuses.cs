namespace Genesis.Skills.Combat.Maxhit;

public static class AttackStyleBonuses
{
    public static readonly BonusRecord<double> Accurate = new("Accurate", 3);
    public static readonly BonusRecord<double> Aggressive = new("Agressive", 1);
    public static readonly BonusRecord<double> Controlled = new("Controlled", 1);
    public static readonly BonusRecord<double> Defensive = new("Defensive", 1);
}