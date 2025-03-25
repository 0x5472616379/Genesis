using ArcticRS.Actions;
using Genesis.Entities;
using Genesis.Model;

namespace Genesis.Skills.Combat.Specials;

public class DragonDaggerSpecialAttack : ISpecialAttack
{
    public void Execute(Player player, Player target, int currentTick, Weapon weaponData)
    {
        player.SetCurrentAnimation(1062);
        player.SetCurrentGfx(new Gfx(252, 70, 0));

        for (int i = 0; i < 2; i++)
        {
            var damage = CalculateDamage(player, target);
            target.ActionHandler.AddAction(new DamageAction(target, damage));
            target.ActionHandler.AddAction(new DoubleDamageAction(target, damage));
        }

        // Update the player's special attack state
        player.CombatHelper.SpecialAmount -= 2.5; // DDS uses 25% energy
        player.CombatHelper.LastAttackTick = currentTick;
        player.CombatHelper.UpdateAttackState(currentTick, weaponData);
    }

    public bool CanExecute(Player player)
    {
        return player.CombatHelper.SpecialAmount >= 2.5;
    }

    private Damage CalculateDamage(Player player, Player target)
    {
        return new Damage(DamageType.HIT,1, null);
    }
}