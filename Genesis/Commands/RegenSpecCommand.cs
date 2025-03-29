using ArcticRS.Constants;
using Genesis.Configuration;
using Genesis.Entities;

namespace Genesis.Commands;

public class RegenSpecCommand : RSCommand
{
    protected override PlayerRights RequiredRights { get; }
    public RegenSpecCommand(Player player, string[] args) : base(player, args)
    {
    }

    public override bool Validate()
    {
        return true;
    }

    public override void Invoke()
    {
        Player.CombatHelper.SpecialAmount = 10;
        Player.CombatHelper.UpdateSpecialAttack(GameInterfaces.WhipDefaultSpecialBar);
        Player.CombatHelper.UpdateSpecialAttack(GameInterfaces.MsbDefaultSpecialBar);
        Player.CombatHelper.UpdateSpecialAttack(GameInterfaces.DragonScimitarDefaultSpecialBar);
        Player.CombatHelper.UpdateSpecialAttack(GameInterfaces.DragonHalberdDefaultSpecialBar);
        Player.CombatHelper.UpdateSpecialAttack(GameInterfaces.DragonBattleAxeDefaultSpecialBar);
        Player.CombatHelper.UpdateSpecialAttack(GameInterfaces.GraniteMaulDefaultSpecialBar);
        Player.CombatHelper.UpdateSpecialAttack(GameInterfaces.DragonSpearDefaultSpecialBar);
        Player.CombatHelper.UpdateSpecialAttack(GameInterfaces.DragonDaggerDefaultSpecialBar);
        Player.CombatHelper.UpdateSpecialAttack(GameInterfaces.DragonMaceDefaultSpecialBar);
    }
}