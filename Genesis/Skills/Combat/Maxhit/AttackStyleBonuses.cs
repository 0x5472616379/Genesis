namespace Genesis.Skills.Combat.Maxhit;

public static class AttackStyleBonuses
{
    public static readonly BonusRecord<double> RangeAccurate = new("Range Accurate", 3);
    public static readonly BonusRecord<double> RangeAggressive = new("Range Aggressive", 1);
    public static readonly BonusRecord<double> RangeControlled = new("Range Controlled", 1);
    public static readonly BonusRecord<double> RangeDefensive = new("Range Defensive", 1);
    
    public static readonly BonusRecord<double> MeleeAccurate = new("Melee Accurate", 0);
    public static readonly BonusRecord<double> MeleeAggressive = new("Melee Aggressive", 3);
    public static readonly BonusRecord<double> MeleeControlled = new("Melee Controlled", 1);
    public static readonly BonusRecord<double> MeleeDefensive = new("Melee Defensive", 0);
}
