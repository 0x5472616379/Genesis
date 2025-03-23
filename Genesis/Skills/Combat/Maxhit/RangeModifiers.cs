namespace Genesis.Skills.Combat.Maxhit;

public static class RangeModifiers
{
    public static readonly BonusRecord<double> None = new("None", 1);
    public static readonly BonusRecord<double> SlayerTask = new("Slayer Task", 1.65);
    public static readonly BonusRecord<double> SalveAmulet = new("Salve Amulet", 1.167);
}