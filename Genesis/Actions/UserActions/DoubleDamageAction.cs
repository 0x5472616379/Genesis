using Genesis.Entities;
using Genesis.Environment;
using Genesis.Skills.Combat;

namespace ArcticRS.Actions;

public class DoubleDamageAction : RSAction
{
    private readonly Damage _damage1;
    private readonly Player _player;

    public DoubleDamageAction(Player player, Damage damage1, int delay = 0)
    {
        _player = player;
        _damage1 = damage1;
        
        Priority = ActionPriority.Forceful;
        ScheduledTick = World.CurrentTick + delay;
    }

    public override bool Execute()
    {
        _player.SetDoubleDamage(_damage1);
        return true;
    }
}