using Genesis.Entities;
using Genesis.Model;
using Genesis.Skills.Combat;

namespace ArcticRS.Actions;

public class DamageAction : RSAction
{
    private readonly Player _player;
    private readonly Gfx _gfx;

    public DamageAction(Player player, Gfx gfx, int tick)
    {
        _player = player;
        _gfx = gfx;
        Priority = ActionPriority.Forceful;
        ScheduledTick = tick;
    }
    
    public override bool Execute()
    {
        _player.SetDamage(1, DamageType.HIT, _gfx);
        return true;
    }
}