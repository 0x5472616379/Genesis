using Genesis.Entities;
using Genesis.Movement;

namespace Genesis.Interactions;

public class MoveInteraction : RSInteraction
{
    private readonly Player _player;
    private int x;
    private int y;

    public MoveInteraction(Player player)
    {
        _player = player;
        x = _player.MovementHandler.TargetDestX;
        y = _player.MovementHandler.TargetDestY;
    }

    public override bool Execute()
    {
        if (!CanExecute()) return false;

        return true;
    }

    public override bool CanExecute()
    {
        if (_player.Location.X != x || _player.Location.Y != y)
        {
            _player.MovementHandler.Reset();
            RSPathfinder.FindPath(_player, x, y, true, 1, 1);
            _player.MovementHandler.Finish();
            _player.MovementHandler.Process();
            _player.Session.PacketBuilder.SendMessage("X: " + _player.Location.X + " Y: " + _player.Location.Y + "");
            return false;
        }


        return true;
    }
}