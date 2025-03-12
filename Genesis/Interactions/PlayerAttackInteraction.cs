using Genesis.Entities;
using Genesis.Environment;
using Genesis.Movement;

namespace Genesis.Interactions;

public class PlayerAttackInteraction : RSInteraction
{
    private readonly Player _player;
    public override int MaxDistance { get; }
    public override InteractingEntity Target { get; set; } = new();
    public PlayerAttackInteraction(Player player)
    {
        _player = player;
    }

    private int currentTick = 0;
    private bool attackLoaded;

    public override bool Execute()
    {
        if (!CanExecute()) return false;

        if (attackLoaded)
        {
            _player.SetCurrentAnimation(1658);
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
        int targetX = _player.InteractingEntity.Location.X;
        int targetY = _player.InteractingEntity.Location.Y;
        int targetZ = _player.InteractingEntity.Location.Z;

        _player.PlayerMovementHandler.Reset();
        RSPathfinder.MeleeFollow(_player, _player.Following);
        _player.PlayerMovementHandler.Finish();
        _player.PlayerMovementHandler.Process();

        _player.Session.PacketBuilder.SendMessage("X: " + _player.Location.X + " Y: " + _player.Location.Y + "");

        var projectilePathClear = MeleePathing.IsLongMeleeDistanceClear(_player, _player.Location.X, _player.Location.Y,
            _player.Location.Z, targetX, targetY, 2);
        var distance = MovementHelper.EuclideanDistance(_player.Location.X, _player.Location.Y, targetX, targetY);
        int moveDistance = 1;
        if (_player.PlayerMovementHandler.IsWalking)
            moveDistance = 2;
        if (_player.PlayerMovementHandler.IsRunning)
            moveDistance = 3;


        bool isValidDistance = distance <= moveDistance;
        bool isDiagonal = MeleePathing.IsDiagonal(_player.Location.X, _player.Location.Y, targetX, targetY);

        _player.Session.PacketBuilder.SendMessage($"MoveDistance: {moveDistance}");
        _player.Session.PacketBuilder.SendMessage($"IsDiagonal: {isDiagonal}");
        _player.Session.PacketBuilder.SendMessage($"InValidDistance: {isValidDistance}");
        _player.Session.PacketBuilder.SendMessage("NoClipping: " + projectilePathClear);

        return isValidDistance && projectilePathClear && !isDiagonal;
    }
}