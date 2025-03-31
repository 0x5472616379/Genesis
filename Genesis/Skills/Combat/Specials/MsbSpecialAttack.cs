using ArcticRS.Actions;
using Genesis.Entities.Player;
using Genesis.Model;

namespace Genesis.Skills.Combat.Specials;

public class MsbSpecialAttack : ISpecialAttack
{
    public void Execute(Player player, Player target, int currentTick, Weapon weaponData)
    {
        player.SetCurrentAnimation(1062);
        player.SetCurrentGfx(new Gfx(252, 70, 0));

        var damage = CalculateDamage(player, target);
        target.ActionHandler.AddAction(new DamageAction(target, player, damage));
        target.ActionHandler.AddAction(new DoubleDamageAction(target, player, damage));

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
        return new Damage(DamageType.HIT, 1, null);
    }
}