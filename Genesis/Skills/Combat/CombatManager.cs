using Genesis.Entities;
using Genesis.Movement;

namespace Genesis.Skills.Combat;

public class CombatManager
{
    private readonly Player _player;
    public int RecentAttackTick { get; set; }
    public int WeaponSpeed { get; set; }
    
    public CombatManager(Player player)
    {
        _player = player;
    }

    public bool InValidProjectileDistance(Player target)
    {
        int targetX = target.Location.X;
        int targetY = target.Location.Y;
        int targetZ = target.Location.Z;
        
        var distance = MovementHelper.EuclideanDistance(_player.Location.X, _player.Location.Y,
            targetX, targetY);
        
        if (distance > 20)
        {
            _player.CurrentInteraction = null;
            _player.SetFacingEntity(null);
            _player.PlayerMovementHandler.Reset();
            return false;
        }
        
        if (distance > (int)CombatDistances.Magic)
        {
            _player.PlayerMovementHandler.Reset();
            RSPathfinder.MeleeFollow(_player, target);
            _player.PlayerMovementHandler.Finish();
            _player.PlayerMovementHandler.Process();
        }
        
        /* If same tile step away */
        if (distance <= 0)
        {
            _player.PlayerMovementHandler.Reset();
            RSPathfinder.MeleeFollow(_player, target);
            _player.PlayerMovementHandler.Finish();
            _player.PlayerMovementHandler.Process();
        }
        
        bool isValidDistance = distance <= (int)CombatDistances.Magic;
        // if (isValidDistance)
        // {
        //     _player.PlayerMovementHandler.Reset();
        //     _player.PlayerMovementHandler.Process();
        //     _player.PlayerMovementHandler.Finish();
        // }
        
        var projectilePathClear = RSPathfinder.IsProjectilePathClear(_player, _player.Location.X, _player.Location.Y, _player.Location.Z,
            targetX, targetY);
        
        return projectilePathClear && isValidDistance;
    }
    
    public bool InValidMeleeDistance(Player target)
    {
        int targetX = target.Location.X;
        int targetY = target.Location.Y;
        int targetZ = target.Location.Z;

        var distance = MovementHelper.EuclideanDistance(_player.Location.X, _player.Location.Y,
            targetX, targetY);
        

        if (distance > 20)
        {
            _player.CurrentInteraction = null;
            _player.SetFacingEntity(null);
            _player.PlayerMovementHandler.Reset();
            return false;
        }


        if (distance > (int)CombatDistances.Melee)
        {
            _player.PlayerMovementHandler.Reset();
            RSPathfinder.MeleeFollow(_player, target);
            _player.PlayerMovementHandler.Finish();
            _player.PlayerMovementHandler.Process();
        }

        /* If same tile step away */
        if (distance <= 0)
        {
            _player.PlayerMovementHandler.Reset();
            RSPathfinder.MeleeFollow(_player, target);
            _player.PlayerMovementHandler.Finish();
            _player.PlayerMovementHandler.Process();
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