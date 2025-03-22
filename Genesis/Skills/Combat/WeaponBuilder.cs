using Genesis.Cache;
using Genesis.Entities;
using Genesis.Managers;

namespace Genesis.Skills.Combat;

public class WeaponBuilder
{
    public static Weapon GetWeaponData(Player player, int itemId)
    {
        var def = ItemDefinition.Lookup(itemId);
        if (def == null) return null;

        if (itemId == -1)
            def.Name = "Unarmed";
        
        var weapon = new Weapon(itemId, (int)WeaponSpeedLookup.GetWeaponTicks(def.Name, player.FightMode), 
            player.AnimationManager.GetWeaponAnimation(itemId, (int)player.FightMode));
        
        return weapon;
    }
}