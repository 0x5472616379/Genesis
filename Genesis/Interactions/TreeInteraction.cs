using Genesis.Entities;
using Genesis.Environment;

namespace Genesis.Interactions;

public class TreeInteraction : RSInteraction
{
    private readonly Player _player;
    private readonly Location _treeLocation;
    private readonly int _clipping;

    public TreeInteraction(Action action, Player player, int treeSize, int treeId, Location treeLocation,
        int clipping) : base(action)
    {
        _player = player;
        _treeLocation = treeLocation;
        _clipping = clipping;
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
        
        var reachedFacingObject = Region.reachedFacingObject(
            _player.Location.PositionRelativeToOffsetChunkX, 
            _player.Location.PositionRelativeToOffsetChunkY,
            treeRelX2,
            treeRelY2,
            2, 2, 0,
            _player.Location.X,
            _player.Location.Y, 0);

        _player.Session.PacketBuilder.SendMessage($"Reached Facing Object: {reachedFacingObject}");

        //var canMove = Region.canMove(_player.Location, _treeLocation, 2,2);
        //_player.Session.PacketBuilder.SendMessage($"CanReach Object: {canMove}");

        return false;
    }

    public override bool CanExecute()
    {
        throw new NotImplementedException();
    }
}