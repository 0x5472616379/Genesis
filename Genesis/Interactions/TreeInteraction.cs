using Genesis.Cache;
using Genesis.Entities;
using Genesis.Environment;

namespace Genesis.Interactions;

public class TreeInteraction : RSInteraction
{
    private readonly Player _player;
    private readonly Objects _treeObject;
    private readonly Location _treeLocation;
    private readonly int _clipping;

    public TreeInteraction(Action action, Player player, Objects treeObject, Location treeLocation) : base(action)
    {
        _player = player;
        _treeObject = treeObject;
        _treeLocation = treeLocation;
    }

    public override bool Execute()
    {
        var treeRelX2 = _treeLocation.X - _player.Location.CachedBuildAreaStartX;
        var treeRelY2 = _treeLocation.Y - _player.Location.CachedBuildAreaStartY;
        
        var region = Region.GetRegion(_player.Location.X, _player.Location.Y);
        var clip = region.GetClip(_player.Location.X, _player.Location.Y, _player.Location.Z);
        
        var reachedFacingObject = Region.reachedFacingObject(
            _player.Location.PositionRelativeToOffsetChunkX, 
            _player.Location.PositionRelativeToOffsetChunkY,
            treeRelX2,
            treeRelY2,
            _treeObject.GetSize()[0], _treeObject.GetSize()[1], 0,
            _player.Location.X,
            _player.Location.Y, clip);

        _player.Session.PacketBuilder.SendMessage($"Reached Facing Object: {reachedFacingObject}");

        return false;
    }

    public override bool CanExecute()
    {
        throw new NotImplementedException();
    }
}