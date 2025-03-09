using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Movement;

namespace Genesis.Interactions;

public class BankInteraction : RSInteraction
{
    private readonly Player _player;
    private readonly WorldObject _worldObject;
    private readonly Random _random = new();

    public BankInteraction(Player player, WorldObject worldObject)
    {
        _player = player;
        _worldObject = worldObject;
    }

    public override bool Execute()
    {
        SetPlayerFacing();

        if (!CanExecute()) return false;

        _player.BankManager.CompressBank();
        _player.BankManager.RefreshInventory();
        _player.Session.PacketBuilder.SendInterface(GameInterfaces.BankWindowInterface,
            GameInterfaces.BankInventorySidebarInterface);

        return true;
    }

    public override bool CanExecute()
    {
        var isMoving = (_player.MovementHandler.IsWalking || _player.MovementHandler.IsRunning);
        if (isMoving)
            return false;

        var treeRelX2 = _worldObject.X - _player.Location.CachedBuildAreaStartX;
        var treeRelY2 = _worldObject.Y - _player.Location.CachedBuildAreaStartY;

        var region = Region.GetRegion(_player.Location.X, _player.Location.Y);
        var clip = region.GetClip(_player.Location.X, _player.Location.Y, _player.Location.Z);

        var reachedFacingObject = Region.ReachedObject(_player.Location.PositionRelativeToOffsetChunkX,
            _player.Location.PositionRelativeToOffsetChunkY,
            treeRelX2,
            treeRelY2,
            _worldObject.GetSize()[0],
            _worldObject.GetSize()[1],
            0, clip);

        if (!reachedFacingObject)
        {
            _player.MovementHandler.Reset();
            RSPathfinder.FindPath(_player, _player.MovementHandler.TargetDestX, _player.MovementHandler.TargetDestY,
                true, 1, 1);
            _player.MovementHandler.Finish();
            _player.MovementHandler.Process();
            return false;
        }

        return true;
    }

    private void SetPlayerFacing()
    {
        _player.SetFaceX(_worldObject.X * 2 + _worldObject.GetSize()[0]);
        _player.SetFaceY(_worldObject.Y * 2 + _worldObject.GetSize()[1]);
    }
}