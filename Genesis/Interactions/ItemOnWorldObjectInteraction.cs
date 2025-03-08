using ArcticRS.Actions;
using ArcticRS.Commands;
using Genesis.Cache;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Movement;
using Genesis.Packets.Incoming;
using Genesis.Skills.Runecrafting;

namespace Genesis.Interactions;

public class ItemOnWorldObjectInteraction : RSInteraction
{
    private readonly Player _player;
    private readonly WorldInteractObject _worldObject;
    private readonly WorldObject? _interactWith;

    public ItemOnWorldObjectInteraction(Player player, WorldInteractObject worldObject)
    {
        _player = player;
        _worldObject = worldObject;
        _interactWith = GetWorldObject();
    }

    public override bool Execute()
    {
        if (!CanExecute()) return false;
        
        if (RunecraftEntranceChecker.TryHandleRunecraftingInteraction(_player, _worldObject)) return true;

        _player.Session.PacketBuilder.SendMessage("Nothing interesting happens.");
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
            _interactWith.GetSize()[0],
            _interactWith.GetSize()[1],
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

    private WorldObject? GetWorldObject()
    {
        var worldObject = Region.GetObject(_worldObject.WorldLocDataBits, _worldObject.X, _worldObject.Y, _player.Location.Z);
        if (worldObject == null)
        {
            _player.Session.PacketBuilder.SendMessage("Object does not exist.");
        }

        return worldObject;
    }
}