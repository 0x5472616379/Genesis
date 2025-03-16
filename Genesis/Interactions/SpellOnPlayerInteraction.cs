using ArcticRS.Actions;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Model;

namespace Genesis.Interactions;

public class SpellOnPlayerInteraction : RSInteraction
{
    private readonly Player _player;
    private readonly Player _target;
    public override int MaxDistance { get; } = 1;
    public override InteractingEntity Target { get; set; }

    public SpellOnPlayerInteraction(Player player, Player target)
    {
        _player = player;
        _target = target;
    }

    public override bool Execute()
    {
        if (!CanExecute()) return false;

        _player.SetCurrentAnimation(1979);
        _target.ActionHandler.AddAction(new DamageAction(_target, new Gfx { Id = 369 }, World.CurrentTick + 2));
        return true;
    }

    public override bool CanExecute()
    {
        if (_player.CombatManager.InValidProjectileDistance(_target))
        {
            _player.PlayerMovementHandler.Reset();
            return true;
        }

        return false;
    }
}