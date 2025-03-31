using Genesis.Entities.Player;
using Genesis.Environment;
using Genesis.Skills.Combat;

namespace ArcticRS.Actions;

public class DoubleDamageAction : RSAction
{
    private readonly Damage _damage1;
    private readonly Player _toPlayer;
    private readonly Player _fromPlayer;

    public DoubleDamageAction(Player toPlayer, Player fromPlayer, Damage damage1, int delay = 0)
    {
        _toPlayer = toPlayer;
        _fromPlayer = fromPlayer;
        _damage1 = damage1;
        Priority = ActionPriority.Forceful;
        ScheduledTick = World.CurrentTick + delay;
        int key = fromPlayer.Session.Index;
        int damage = damage1.Amount;

        // Check if the key exists in the dictionary
        if (toPlayer.DamageTable.ContainsKey(key))
        {
            // Increment the existing value
            toPlayer.DamageTable[key] += damage;
        }
        else
        {
            // Add the new key-value pair if the key does not exist
            toPlayer.DamageTable.Add(key, damage);
        }
    }

    public override bool Execute()
    {
        _toPlayer.SetDoubleDamage(_damage1);
        return true;
    }
}