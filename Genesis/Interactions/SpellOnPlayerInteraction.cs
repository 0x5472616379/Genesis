using ArcticRS.Actions;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Model;
using Genesis.Skills.Combat;

namespace Genesis.Interactions;

public class SpellOnPlayerInteraction : RSInteraction
{
    private readonly Player _player;
    private readonly Player _target;
    private readonly Weapon _weapon;
    public override int MaxDistance { get; } = 1;
    public override InteractingEntity Target { get; set; }

    public SpellOnPlayerInteraction(Player player, Player target, Weapon weapon)
    {
        _player = player;
        _target = target;
        _weapon = weapon;
    }

    public override bool Execute()
    {
        if (!CanExecute()) return false;

        return _player.CombatManager.Attack(_target, World.CurrentTick, _weapon);
    }

    public override bool CanExecute()
    {
        if (_target.CurrentHealth <= 0)
        {
            _player.CurrentInteraction = null;
            _player.InteractingEntity = null;
            _player.SetFacingEntity(null);
            _player.PlayerMovementHandler.Reset();
            return false;
        }
        
        if (_player.CombatManager.InValidProjectileDistance(_target))
        {
            _player.PlayerMovementHandler.Reset();
            return true;
        }

        return false;
    }
}