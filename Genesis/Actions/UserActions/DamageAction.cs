using Genesis.Entities;
using Genesis.Environment;
using Genesis.Skills.Combat;

namespace ArcticRS.Actions;

public class DamageAction : RSAction
{
    private readonly Damage _damage;
    private readonly Player _toPlayer;
    private readonly Player _fromPlayer;

    public DamageAction(Player toPlayer, Player fromPlayer, Damage damage, int delay = 0)
    {
        _damage = damage;
        _toPlayer = toPlayer;
        _fromPlayer = fromPlayer;
        Priority = ActionPriority.Forceful;
        ScheduledTick = World.CurrentTick + delay;
        
        
        int key = fromPlayer.Session.Index;
        int dmg = _damage.Amount;
        
        // Check if the key exists in the dictionary
        if (toPlayer.DamageTable.ContainsKey(key))
        {
            // Increment the existing value
            toPlayer.DamageTable[key] += dmg;
        }
        else
        {
            // Add the new key-value pair if the key does not exist
            toPlayer.DamageTable.Add(key, dmg);
        }
    }

    public override bool Execute()
    {
        _toPlayer.SetDamage(_damage);
        return true;
    }
}