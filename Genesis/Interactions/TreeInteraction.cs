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
        var offsetChunkX = (_player.Location.X >> 3) - 6;
        var offsetChunkY = (_player.Location.Y >> 3) - 6;
        var x = _player.Location.X - offsetChunkX * 8;
        var y = _player.Location.Y - offsetChunkY * 8;
        //
        //
        var treeoffsetChunkX = (_treeLocation.X >> 3) - 6;
        var treeoffsetChunkY = (_treeLocation.Y >> 3) - 6;
        var treex = _treeLocation.X - treeoffsetChunkX * 8;
        var treey = _treeLocation.Y - treeoffsetChunkY * 8;
        //
        // var clipping = Region.GetClipping(_player.Location.X, _player.Location.Y, _player.Location.Z);
        // Console.WriteLine($"ClipData: {clipping}");
        //
        // var region = Region.GetRegion(_treeLocation.X, _treeLocation.Y);
        // var clip = region.GetClip(_treeLocation.X, _treeLocation.Y, 0);
        // Console.WriteLine($"NClipData: {clip}");
        //
        
        var treeRelX = _treeLocation.X - _player.Location.CachedBuildAreaSwChunkX;
        var treeRelY = _treeLocation.Y - _player.Location.CachedBuildAreaSwChunkY;
        
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