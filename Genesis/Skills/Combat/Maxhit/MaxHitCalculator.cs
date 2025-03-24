using Genesis.Definitions;
using Genesis.Entities;
using Genesis.Managers;

namespace Genesis.Skills.Combat.Maxhit;

public class MaxHitCalculator
{
    public static int GetRangeMaxHit(Player player)
    {
        int rangedLevel = player.SkillManager.Skills[(int)SkillType.RANGED].Level;
        double boost = Boosts.RangePotion;
        double prayerBonus = PrayerBonuses.None;
        double attackStyle = AttackStyleBonuses.Accurate;
        double gearBonus = RangeModifiers.None;
        double voidModifier = VoidBonuses.FullVoid;

        int equipmentRangedStrength = player.BonusManager.GetTotalForBonusType(BonusType.RangeStrength);
        double specialBonus = 1.0;

        int effectiveRangedStrength = (int)(((rangedLevel + boost) * prayerBonus + attackStyle + 8) * voidModifier);
        int baseMaxHit = effectiveRangedStrength * (equipmentRangedStrength + 64) / 640;
        int maxHit = (int)(baseMaxHit * gearBonus * specialBonus);

        player.Session.PacketBuilder.SendMessage($"Effective Ranged Strength: {effectiveRangedStrength}");
        player.Session.PacketBuilder.SendMessage($"Maximum Hit: {maxHit}");
        return maxHit;
    }
}