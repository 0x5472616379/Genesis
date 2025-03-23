using Genesis.Entities;

namespace Genesis.Skills.Combat.Maxhit;

public class MaxHitCalculator
{
    public static void GetRangeMaxHit(Player player)
    {
        int rangedLevel = player.SkillManager.Skills[(int)SkillType.RANGED].Level;
        double boost = Boosts.RangePotion;
        double prayerBonus = PrayerBonuses.EagleEye;
        double attackStyle = AttackStyleBonuses.Accurate;
        double gearBonus = RangeModifiers.None;
        double voidModifier = VoidBonuses.FullVoid;
        
        int equipmentRangedStrength = 100;
        double specialBonus = 1.0;

        int effectiveRangedStrength = (int)(((rangedLevel + boost) * prayerBonus + attackStyle + 8) * voidModifier);
        int baseMaxHit = effectiveRangedStrength * (equipmentRangedStrength + 64) / 640;
        int maxHit = (int)(baseMaxHit * gearBonus * specialBonus);

        Console.WriteLine($"Effective Ranged Strength: {effectiveRangedStrength}");
        Console.WriteLine($"Maximum Hit: {maxHit}");
    }
}