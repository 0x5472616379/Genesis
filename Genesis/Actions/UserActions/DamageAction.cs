using Genesis.Entities;
using Genesis.Model;
using Genesis.Skills.Combat;

namespace ArcticRS.Actions;

public class DamageAction : RSAction
{
    private readonly Player _player;
    private readonly Weapon _weapon;

    public DamageAction(Player player, Weapon weapon, int tick)
    {
        _player = player;
        _weapon = weapon;
        Priority = ActionPriority.Forceful;
        ScheduledTick = tick;
    }

    public Weapon GetWeapon()
    {
        return _weapon;
    }
    
    public override bool Execute()
    {
        _player.SetDamage(_weapon.Damage, DamageType.HIT, _weapon);
        return true;
    }
}