using Genesis.Configuration;
using Genesis.Entities.Player;
using Genesis.Environment;
using Genesis.Skills;

namespace ArcticRS.Actions;

public class EatComboFoodAction : RSAction
{
    private readonly Player _player;
    private readonly int _health;
    private readonly int _index;
    private readonly int _itemId;
    private EatState _currentState;

    public EatComboFoodAction(Player player, int health, int index, int itemId)
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

        switch (_currentState)
        {
            case EatState.Eat:
                StartEating();
                ScheduleNext(2);
                _currentState = EatState.Delay;
                return false;

            case EatState.Delay:
                return true;

            default:
                return true;
        }
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

    private void ScheduleNext(int ticks)
    {
        ScheduledTick = World.CurrentTick + ticks; // Schedule the action for the next phase
    }

    private enum EatState
    {
        Eat, // Represents the initial dig action
        Delay // Represents the action of burying the bones
    }
}