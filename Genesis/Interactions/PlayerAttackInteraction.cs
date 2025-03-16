using ArcticRS.Actions;
using Genesis.Entities;
using Genesis.Movement;
using Genesis.Skills.Combat;

namespace Genesis.Interactions;

public class PlayerAttackInteraction : RSInteraction
{
    private readonly Player _player;
    private Player _target;
    public override int MaxDistance { get; } = 1;
    public override InteractingEntity Target { get; set; } = new();

    private int currentTick = 0;
    private bool attackLoaded;

    private CombatDistances combatDistance = CombatDistances.Melee;

    public PlayerAttackInteraction(Player player, Player target)
    {
        _player = player;
        _target = target;
    }

    public override bool Execute()
    {
        if (!CanExecute()) return false;

        if (attackLoaded)
        {
            _player.SetCurrentAnimation(422);
            _target.ActionHandler.AddAction(new DamageAction(_target, null, 0));
            attackLoaded = false;
        }

        if (currentTick == 0 || currentTick % 4 == 0)
        {
            attackLoaded = true;
        }

        currentTick++;
        return false;
    }

    public override bool CanExecute()
    {
        if (_player.CurrentHealth <= 0)
        {
            return false;
        }

        if (_target.CurrentHealth <= 0)
        {
            _player.CurrentInteraction = null;
            _player.InteractingEntity = null;
            _target = null;
            _player.SetFacingEntity(null);
            _player.PlayerMovementHandler.Reset();
            return false;
        }
        
        return _player.CombatManager.InValidMeleeDistance(_target);
    }
}