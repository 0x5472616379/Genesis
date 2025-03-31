using Genesis.Entities.Player;
using Genesis.Movement;

namespace Genesis.Interactions;

public class SingleDoorInteraction : RSInteraction
{
    private readonly int _ex;
    private readonly int _ey;
    private readonly int _ez;
    private readonly Player _player;
    public override int MaxDistance { get; } = 1;
    public override InteractingEntity Target { get; set; } = new();
    public SingleDoorInteraction(Action action, int ex, int ey, int ez, Player player)
    {
        _ex = ex;
        _ey = ey;
        _ez = ez;
        _player = player;
    }

    public override bool Execute()
    {
        _player.Session.PacketBuilder.SendMessage("Trying to execute..");
        if (CanExecute())
        {
            _player.Session.PacketBuilder.SendMessage("You interact with the door.");
            return true;
        }

        return false;
    }

    public override bool CanExecute()
    {
        var px = _player.Location.X;
        var py = _player.Location.Y;
        var pz = _player.Location.Z;

        var distance = MovementHelper.EuclideanDistance(px, py, _ex, _ey);
        return (Math.Abs(px - _ex) == 1 && py == _ey) || (Math.Abs(py - _ey) == 1 && px == _ex) && distance <= 1;
    }
}