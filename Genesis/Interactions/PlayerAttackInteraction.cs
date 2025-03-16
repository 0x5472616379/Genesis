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
            _target.ActionHandler.AddAction(new DamageAction(_target));
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
        
        int targetX = _target.Location.X;
        int targetY = _target.Location.Y;
        int targetZ = _target.Location.Z;

        var distance = MovementHelper.EuclideanDistance(_player.Location.X, _player.Location.Y,
            targetX, targetY);
        

        if (distance > 20)
        {
            // _player.Following = null;
            _player.CurrentInteraction = null;
            _player.SetFacingEntity(null);
            Target = null;
            _player.PlayerMovementHandler.Reset();
            return false;
        }
        

        if (_target != null)
        {
            if (distance > (int)combatDistance)
            {
                _player.PlayerMovementHandler.Reset();
                RSPathfinder.MeleeFollow(_player, _target);
                _player.PlayerMovementHandler.Finish();
                _player.PlayerMovementHandler.Process();
            }

            /* If same tile step away */
            if (distance <= 0)
            {
                _player.PlayerMovementHandler.Reset();
                RSPathfinder.MeleeFollow(_player, _target);
                _player.PlayerMovementHandler.Finish();
                _player.PlayerMovementHandler.Process();
            }
        }

        int moveDistance = 1;
        if (_player.PlayerMovementHandler.IsWalking)
            moveDistance = 2;
        if (_player.PlayerMovementHandler.IsRunning)
            moveDistance = 3;
        
        var projectilePathClear = MeleePathing.IsLongMeleeDistanceClear(_player, _player.Location.X, _player.Location.Y,
            _player.Location.Z, targetX, targetY, 2);

        bool isValidDistance = distance <= moveDistance;
        bool isDiagonal = MeleePathing.IsDiagonal(_player.Location.X, _player.Location.Y, targetX, targetY);
        
        _player.Session.PacketBuilder.SendMessage($"MoveDistance: {moveDistance}");
        _player.Session.PacketBuilder.SendMessage($"IsDiagonal: {isDiagonal}");
        _player.Session.PacketBuilder.SendMessage($"InValidDistance: {isValidDistance}");
        _player.Session.PacketBuilder.SendMessage("NoClipping: " + projectilePathClear);
        
        return isValidDistance && projectilePathClear && !isDiagonal;

    }
}