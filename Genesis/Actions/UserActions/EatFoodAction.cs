using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Skills;

namespace ArcticRS.Actions;

public class EatFoodAction : RSAction
{
    private readonly Player _player;
    private readonly int _health;
    private readonly int _index;
    private readonly int _itemId;

    public EatFoodAction(Player player, int health, int index, int itemId)
    {
        _player = player;
        _health = health;
        _index = index;
        _itemId = itemId;
        Priority = ActionPriority.Forceful;
        ScheduledTick = World.CurrentTick;
    }

    public override bool Execute()
    {
        if (_player.CurrentHealth <= 0)
            return true;

        StartEating();
        
        return true;
    }

    private void StartEating()
    {
        _player.SetCurrentAnimation(829);
        var newHealth = Math.Min(_player.CurrentHealth + _health,
            _player.SkillManager.Skills[(int)SkillType.HITPOINTS].Level);
        _player.CurrentHealth = newHealth;
        _player.SkillManager.RefreshSkill(SkillType.HITPOINTS);
        _player.Session.PacketBuilder.SendMessage("You eat the food.");
        _player.Inventory.ClearSlot(_index);
        _player.Inventory.RefreshSlot(_player, _index, -1, 0, GameInterfaces.DefaultInventoryContainer);
    }
}