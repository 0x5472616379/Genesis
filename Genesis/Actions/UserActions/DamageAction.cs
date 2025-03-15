using Genesis.Entities;
using Genesis.Skills.Combat;

namespace ArcticRS.Actions;

public class DamageAction : RSAction
{
    private readonly Player _player;

    public DamageAction(Player player)
    {
        _player = player;
        Priority = ActionPriority.Forceful;
    }
    
    public override bool Execute()
    {
        _player.SetDamage(2, DamageType.HIT);
        return true;
    }
}