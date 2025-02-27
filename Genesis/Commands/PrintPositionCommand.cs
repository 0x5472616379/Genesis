using ArcticRS.Constants;
using Genesis.Entities;
using Genesis.Environment;

namespace ArcticRS.Commands;

public class PrintPositionCommand : CommandBase
{
    private readonly bool _debug;

    public PrintPositionCommand(Player player, string[] args) : base(player, args)
    {
        _debug = args.Length > 1 && args[1].ToLower() == "debug";
    }

    protected override PlayerRights RequiredRights => PlayerRights.NORMAL;
    
    protected override string ValidateArgs()
    {
        return null;
    }

    protected override void Invoke()
    {
        var location = Player.Location;
        if (!_debug)
        {
            Player.Session.PacketBuilder.SendMessage($"X: {location.X} Y: {location.Y} Z: {location.Z}");
            return;
        }

        foreach (var part in Player.Location.ToStringParts())
        {
            Player.Session.PacketBuilder.SendMessage(part);
        }
        
        Player.Session.PacketBuilder.SendMessage("========");
        
        Player.Session.PacketBuilder.SendMessage($"Current X: {Player.Location.X}");
        Player.Session.PacketBuilder.SendMessage($"Current Y: {Player.Location.Y}");
        
        /* Not used for much more than cache layout */
        var MapRegion = (((Player.Location.X >> 6) << 8) & 0xFF00) | ((Player.Location.Y >> 6) & 0xFF);
        Player.Session.PacketBuilder.SendMessage($"Current Map Region: {MapRegion}");
        
        //Current ChunkX/Y
        var currentChunkX = Player.Location.X >> 3;
        var currentChunkY = Player.Location.Y >> 3;
        
        var offsetChunkX = (Player.Location.X >> 3) - 6;
        var offsetChunkY = (Player.Location.Y >> 3) - 6;
        
        Player.Session.PacketBuilder.SendMessage($"Current ChunkX: {currentChunkX}");
        Player.Session.PacketBuilder.SendMessage($"Current ChunkY: {currentChunkY}");
        
        Player.Session.PacketBuilder.SendMessage($"Current OffsetChunkX: {currentChunkX}");
        Player.Session.PacketBuilder.SendMessage($"Current OffsetChunkY: {currentChunkY}");
        
        Player.Session.PacketBuilder.SendMessage($"Chunk Relative To Current OffsetChunkX: {offsetChunkX}");
        Player.Session.PacketBuilder.SendMessage($"Chunk Relative To Current OffsetChunkY: {offsetChunkY}");

        Player.Session.PacketBuilder.SendMessage($"Pos Relative To OffsetChunkX: {Player.Location.X - offsetChunkX * 8}");
        Player.Session.PacketBuilder.SendMessage($"Pos Relative To OffsetChunkY: {Player.Location.Y - offsetChunkY * 8}");
        
        Player.Session.PacketBuilder.SendMessage($"Pos Relative To CachedOffsetChunkX: {Player.Location.X - Player.Location.CachedBuildAreaSwChunkX * 8}");
        Player.Session.PacketBuilder.SendMessage($"Pos Relative To CachedOffsetChunkY: {Player.Location.Y - Player.Location.CachedBuildAreaSwChunkY * 8}");
        
        Player.Session.PacketBuilder.SendMessage($"Cached OffsetChunkX: {Player.Location.CachedBuildAreaSwChunkX}");
        Player.Session.PacketBuilder.SendMessage($"Cached OffsetChunkY: {Player.Location.CachedBuildAreaSwChunkY}");
        
        Player.Session.PacketBuilder.SendMessage($"Pos Relative To Cached OffsetChunkX: {Player.Location.X - Player.Location.CachedBuildAreaSwChunkX * 8}");
        Player.Session.PacketBuilder.SendMessage($"Pos Relative To Cached OffsetChunkY: {Player.Location.Y - Player.Location.CachedBuildAreaSwChunkY * 8}");
        
        
        Player.Session.PacketBuilder.SendMessage($"AbsX Relative To BA-Ax: {Player.Location.X - Player.Location.CachedBuildAreaStartX}");
        Player.Session.PacketBuilder.SendMessage($"AbsY Relative To BA-Ay: {Player.Location.Y - Player.Location.CachedBuildAreaStartY}");

        var clipData = Region.GetClipping(Player.Location.X, Player.Location.Y, Player.Location.Z);
        Player.Session.PacketBuilder.SendMessage($"ClipData: {clipData}");
    }
}