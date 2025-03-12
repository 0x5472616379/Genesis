using Genesis.Cache;
using Genesis.Entities;

namespace Genesis.Interactions;

public class MiningInteraction : RSInteraction
{
    private readonly Player _player;
    private readonly WorldObject _worldObject;

    public override int MaxDistance { get; } = 1;
    public override InteractingEntity Target { get; set; } = new();
    private const int ARRIVE_DELAY = 1;

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
        _player.SetFaceX(Target.X * 2 + _worldObject.GetSize()[0]);
        _player.SetFaceY(Target.Y * 2 + _worldObject.GetSize()[1]);
        
        if (!CanExecute()) return false;

        _player.SetCurrentAnimation(625);
        _player.NormalDelayTicks = 4; /* Exaggerated for testing */
        _player.Session.PacketBuilder.SendMessage("You swing your pick at the rock.");
        return true;
    }

    public override bool CanExecute()
    {
        if (_player.NormalDelayTicks > 0 || _player.ArriveDelayTicks > 0)
            return false;

        return true;
    }
}