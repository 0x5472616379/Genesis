using ArcticRS.Actions;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Movement;
using Genesis.Skills.Combat;

namespace Genesis.Interactions;

public class PlayerAttackInteraction : RSInteraction
{
    private readonly Player _player;
    private Player _target;
    private readonly Weapon _weapon;
    public override int MaxDistance { get; } = 1;
    public override InteractingEntity Target { get; set; } = new();

    private int currentTick = 0;
    private bool attackLoaded;

    private CombatDistances combatDistance = CombatDistances.Melee;

    private bool UsingBow = false;

    public PlayerAttackInteraction(Player player, Player target, Weapon weapon)
    {
        _player = player;
        _target = target;
        _weapon = weapon;
    }

    public override bool Execute()
    {
        if (!CanExecute()) return false;
        _weapon.Damage = 1;

        if (UsingBow)
            _weapon.AttackerAnim = 426;
        else
            _weapon.AttackerAnim = 422;

        return _player.CombatManager.Attack(_target, World.CurrentTick, _weapon);

        // if (attackLoaded)
        // {
        //     _player.SetCurrentAnimation(422);
        //     _target.ActionHandler.AddAction(new DamageAction(_target, null, 0));
        //     attackLoaded = false;
        // }
        //
        // if (currentTick == 0 || currentTick % 4 == 0)
        // {
        //     attackLoaded = true;
        // }
        //
        // currentTick++;
        // return false;
    }

    public override bool CanExecute()
    {
        UsingBow = false;
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

        if (UsingBow)
        {
            if (_player.CombatManager.InValidProjectileDistance(_target))
            {
                _player.PlayerMovementHandler.Reset();
                return true;
            }
        }
        else
        {
            if (_player.CombatManager.InValidMeleeDistance(_target))
            {
                _player.PlayerMovementHandler.Reset();
                return true;
            }
        }

        _player.PlayerMovementHandler.Reset();
        RSPathfinder.FindPath(_player, _target.Location.X, _target.Location.Y, true, 1, 1);
        _player.PlayerMovementHandler.Finish();
        _player.PlayerMovementHandler.Process();
        _player.PlayerMovementHandler.Reset();
        
        return false;
    }
}