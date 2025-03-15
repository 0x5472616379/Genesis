using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Movement;

namespace Genesis.Interactions;

public class BankInteraction : RSInteraction
{
    public override int MaxDistance { get; } = 1;
    private readonly Player _player;
    private readonly WorldObject _worldObject;
    private readonly Random _random = new();
    public override InteractingEntity Target { get; set; } = new();

    public BankInteraction(Player player, WorldObject worldObject)
    {
        _player = player;
        _worldObject = worldObject;
        Target.X = worldObject.X;
        Target.Y = worldObject.Y;
        Target.Z = _player.Location.Z;
    }


    public override bool Execute()
    {
        //SetPlayerFacing();

        if (!CanExecute()) return false;

        // _player.BankManager.CompressBank();

        _player.InventoryItemContainer.CopyToContainer(_player.BankInventoryItemContainer);
        _player.BankItemContainer.Refresh(_player, GameInterfaces.DefaultBankContainer);
        _player.InventoryItemContainer.Refresh(_player, GameInterfaces.DefaultInventoryContainer);
        _player.BankInventoryItemContainer.Refresh(_player, GameInterfaces.BankInventoryContainer);

        _player.Session.PacketBuilder.SendInterface(GameInterfaces.BankWindowInterface,
            GameInterfaces.BankInventorySidebarInterface);

        return true;
    }

    public override bool CanExecute()
    {
        // var isMoving = (_player.PlayerMovementHandler.IsWalking || _player.PlayerMovementHandler.IsRunning);
        // if (isMoving)
        //     return false;

        var treeRelX2 = _worldObject.X - _player.Location.CachedBuildAreaStartX;
        var treeRelY2 = _worldObject.Y - _player.Location.CachedBuildAreaStartY;
        //
        var region = Region.GetRegion(_player.Location.X, _player.Location.Y);
        var clip = region.GetClip(_player.Location.X, _player.Location.Y, _player.Location.Z);
        
        var reachedFacingObject = Region.ReachedObject(_player.Location.PositionRelativeToOffsetChunkX,
            _player.Location.PositionRelativeToOffsetChunkY,
            treeRelX2,
            treeRelY2,
            _worldObject.GetSize()[0],
            _worldObject.GetSize()[1],
            0, clip);

        // Proper game square distance check
        int distance = MovementHelper.GameSquareDistance(_player.Location.X, _player.Location.Y,
            _worldObject.X, _worldObject.Y);

        if (distance <= MaxDistance && !reachedFacingObject)
        {
            RSPathfinder.FindPath(_player, _worldObject.X, _worldObject.Y, true, 1, 1);
            _player.PlayerMovementHandler.Process();
            return false;
        }

        SetPlayerFacing();
        return true;
    }

    private void SetPlayerFacing()
    {
        _player.SetFaceX(_worldObject.X * 2 + _worldObject.GetSize()[0]);
        _player.SetFaceY(_worldObject.Y * 2 + _worldObject.GetSize()[1]);
    }
}