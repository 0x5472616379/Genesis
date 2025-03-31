using Genesis.Entities.Player;
using Genesis.Environment;
using Genesis.Skills.Combat;

namespace ArcticRS.Actions;

public class SendMsbGfxAction : RSAction
{
    private readonly Player _player;
    private readonly Player _target;
    private ProjectileStage _currentState = ProjectileStage.FirstProjectile;

    public SendMsbGfxAction(Player player, Player target)
    {
        _player = player;
        _target = target;
        Priority = ActionPriority.Unstoppable;
    }

    public override bool Execute()
    {
        if (_player.CurrentHealth <= 0)
            return true;

        switch (_currentState)
        {
            case ProjectileStage.FirstProjectile:
                SendFirstProjectile();
                return true;

            default:
                return true;
        }
    }

    private void SendFirstProjectile()
    {
        ProjectileCreator.CreateProjectile(_player, _target, 249, delay: 15, duration: 35, sY: 100);
    }
    
    private void SendSecondProjectile()
    {
        ProjectileCreator.CreateProjectile(_player, _target, 249);
    }

    private void ScheduleNext(int ticks)
    {
        ScheduledTick = World.CurrentTick + ticks; // Schedule the action for the next phase
    }

    private enum ProjectileStage
    {
        FirstProjectile,
        SecondProjectile
    }
}