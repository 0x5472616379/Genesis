using ArcticRS.Actions;
using Genesis.Configuration;
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
            var damage0 = player.CombatHelper.MeleeCombatStyle.CalculateDamage(player, target);
            var damage1 = player.CombatHelper.MeleeCombatStyle.CalculateDamage(player, target);
            
            target.ActionHandler.AddAction(new DamageAction(target, damage0));
            target.ActionHandler.AddAction(new DoubleDamageAction(target, damage1));
        }

        /* Update the player's special attack state */
        player.CombatHelper.SpecialAmount -= 2.5; /* DDS uses 25% energy */
        player.CombatHelper.LastAttackTick = currentTick;
        player.CombatHelper.UpdateAttackState(currentTick, weaponData);
        player.CombatHelper.SpecialAttack = null;
        player.CombatHelper.UpdateSpecialAttack(GameInterfaces.DragonDaggerDefaultSpecialBar);
    }

    public bool CanExecute(Player player)
    {
        if (player.CombatHelper.SpecialAmount >= 2.5)
        {
            return true;
        }

        player.Session.PacketBuilder.SendMessage("You don't have enough power left.");
        player.CombatHelper.SpecialAttack = null;
        player.CombatHelper.UpdateSpecialAttack(GameInterfaces.DragonDaggerDefaultSpecialBar);
        return false;
    }
}