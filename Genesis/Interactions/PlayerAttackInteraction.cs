using ArcticRS.Actions;
using Genesis.Entities;
using Genesis.Movement;
using Genesis.Skills.Combat;

namespace Genesis.Interactions;

public class PlayerAttackInteraction : RSInteraction
{
    private readonly Player _player;
    private readonly Player _target;
    public override int MaxDistance { get; } = 1;
    public override InteractingEntity Target { get; set; } = new();

    private int currentTick = 0;
    private bool attackLoaded;

    private CombatDistances combatDistance = CombatDistances.Melee;

    public PlayerAttackInteraction(Player player, Player target)
    {
        _player = player;
        _target = target;
        Target.X = target.Location.X;
        Target.Y = target.Location.Y;
        Target.Z = target.Location.Z;
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
        if (_target.CurrentHealth <= 0)
        {
            _player.CurrentInteraction = null;
            _player.SetFacingEntity(null);
            Target = null;
            return false;
        }

        int targetX = _player.InteractingEntity.Location.X;
        int targetY = _player.InteractingEntity.Location.Y;
        int targetZ = _player.InteractingEntity.Location.Z;

        var distance = MovementHelper.EuclideanDistance(_player.Location.X, _player.Location.Y,
            targetX, targetY);

        if (_player.Following != null)
        {
            if (distance > (int)combatDistance)
            {
                _player.PlayerMovementHandler.Reset();
                RSPathfinder.MeleeFollow(_player, _player.Following);
                _player.PlayerMovementHandler.Finish();
                _player.PlayerMovementHandler.Process();
            }

            /* If same tile step away */
            if (distance <= 0)
            {
                _player.PlayerMovementHandler.Reset();
                RSPathfinder.MeleeFollow(_player, _player.Following);
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

        // return true;

        // _player.PlayerMovementHandler.Reset();
        // // RSPathfinder.MeleeWalk(_player, _player.Following.Location);
        // RSPathfinder.FindPath(_player, _player.InteractingEntity.Location.X, _player.InteractingEntity.Location.Y, true,
        //     1, 1);
        // _player.PlayerMovementHandler.Finish();
        // _player.PlayerMovementHandler.Process();
        // _player.PlayerMovementHandler.Reset();
        // return false;

        // int targetX = _player.InteractingEntity.Location.X;
        // int targetY = _player.InteractingEntity.Location.Y;
        // int targetZ = _player.InteractingEntity.Location.Z;
        //
        // _player.Session.PacketBuilder.SendMessage("X: " + _player.Location.X + " Y: " + _player.Location.Y + "");
        //
        // var projectilePathClear = MeleePathing.IsLongMeleeDistanceClear(_player, _player.Location.X, _player.Location.Y,
        //     _player.Location.Z, targetX, targetY, 2);
        //
        // var distance = MovementHelper.EuclideanDistance(_player.Location.X, _player.Location.Y, targetX, targetY);
        // int moveDistance = 1;
        // if (_player.PlayerMovementHandler.IsWalking)
        //     moveDistance = 2;
        // if (_player.PlayerMovementHandler.IsRunning)
        //     moveDistance = 3;
        //
        //
        // bool isValidDistance = distance <= moveDistance;
        // bool isDiagonal = MeleePathing.IsDiagonal(_player.Location.X, _player.Location.Y, targetX, targetY);
        //
        // if (!isValidDistance || !projectilePathClear || isDiagonal)
        // {
        //     // _player.PlayerMovementHandler.Finish();
        //     // _player.PlayerMovementHandler.Reset();
        //     // // RSPathfinder.MeleeWalk(_player, _player.Following.Location);
        //     // RSPathfinder.FindPath(_player, _player.InteractingEntity.Location.X, _player.InteractingEntity.Location.Y, true,
        //     //     1, 1);
        //     // _player.PlayerMovementHandler.Finish();
        //     // _player.PlayerMovementHandler.Process();
        //     // _player.PlayerMovementHandler.Reset();
        //     return false;
        // }
        //
        // _player.Session.PacketBuilder.SendMessage($"MoveDistance: {moveDistance}");
        // _player.Session.PacketBuilder.SendMessage($"IsDiagonal: {isDiagonal}");
        // _player.Session.PacketBuilder.SendMessage($"InValidDistance: {isValidDistance}");
        // _player.Session.PacketBuilder.SendMessage("NoClipping: " + projectilePathClear);
        //
        // return isValidDistance && projectilePathClear && !isDiagonal;
    }
}