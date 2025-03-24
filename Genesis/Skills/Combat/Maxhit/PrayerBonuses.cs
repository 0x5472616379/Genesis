namespace Genesis.Skills.Combat.Maxhit;

public static class PrayerBonuses
{
    public static readonly BonusRecord<double> None = new("None", 1);
    public static readonly BonusRecord<double> SharpEye = new("Sharp Eye", 1.05);
    public static readonly BonusRecord<double> HawkEye = new("Hawk Eye", 1.10);
    public static readonly BonusRecord<double> EagleEye = new("Eagle Eye", 1.15);
}