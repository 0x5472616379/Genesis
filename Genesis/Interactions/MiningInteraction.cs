using Genesis.Cache;
using Genesis.Entities;
using Genesis.Movement;

namespace Genesis.Interactions;

public class MiningInteraction : RSInteraction
{
    private readonly Player _player;
    private readonly WorldObject _worldObject;

    public override int MaxDistance { get; } = 1;
    public override InteractingEntity Target { get; set; } = new();

    public MiningInteraction(Player player, WorldObject worldObject)
    {
        _player = player;
        _worldObject = worldObject;
        Target.X = worldObject.X;
        Target.Y = worldObject.Y;
        Target.Z = _player.Location.Z;
    }

    public override bool Execute()
    {
        if (!CanExecute()) return false;

        _player.SetCurrentAnimation(625);
        _player.Session.PacketBuilder.SendMessage("You swing your pick at the rock.");
        return true;
    }

    public override bool CanExecute()
    {
        if (_player.CurrentInteraction != null && 
            (_player.MovedThisTick || _player.MovedLastTick) && 
            MovementHelper.GameSquareDistance(_player.Location.X, _player.Location.Y, _player.CurrentInteraction.Target.X, _player.CurrentInteraction.Target.Y) <= 1)
        {
            _player.ArriveDelayTicks = 1;
        }
        
        if (_player.NormalDelayTicks > 0 || _player.ArriveDelayTicks > 0)
            return false;
     
        
        // Proper game square distance check
        int distance = MovementHelper.GameSquareDistance(_player.Location.X, _player.Location.Y,
                                                         _worldObject.X, _worldObject.Y);

        return distance <= MaxDistance;
    }
}