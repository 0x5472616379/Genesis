namespace Genesis.Skills.Combat.Maxhit;

public static class Boosts
{
    public static readonly BonusRecord<double> None = new("None", 1);
    public static readonly BonusRecord<double> RangePotion = new("Range Potion", 15);
    public static readonly BonusRecord<double> StrengthPotion = new("Strength Potion", 5);
    public static readonly BonusRecord<double> SuperStrengthPotion = new("Super Strength Potion", 15);
}