using Genesis.Entities;
using Genesis.Environment;
using Genesis.Skills;

namespace ArcticRS.Actions;

public class EatFoodAction : RSAction
{
    private readonly Player _player;
    private readonly int _health;
    private EatState _currentState;

    public EatFoodAction(Player player, int health)
    {
        _player = player;
        _health = health;
        Priority = ActionPriority.Forceful;
        ScheduledTick = World.CurrentTick;
    }

    public override bool Execute()
    {
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