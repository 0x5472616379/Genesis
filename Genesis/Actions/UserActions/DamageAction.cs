using Genesis.Entities;
using Genesis.Environment;
using Genesis.Skills.Combat;

namespace ArcticRS.Actions;

public class DamageAction : RSAction
{
    private readonly Damage _damage;
    private readonly Player _player;

    public DamageAction(Player player, Damage damage, int delay = 0)
    {
        _damage = damage;
        _player = player;
        Priority = ActionPriority.Forceful;
        ScheduledTick = World.CurrentTick + delay;
    }

    public override bool Execute()
    {
        _player.SetDamage(_damage);
        return true;
    }
}