using ArcticRS.Commands;
using Genesis.Cache;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Managers;

namespace Genesis.Commands;

public class RemoveObjectCommand : CommandBase
{
    public RemoveObjectCommand(Player player, string[] args) : base(player, args)
    {
    }

    protected override string ValidateArgs()
    {
        return null;
    }

    protected override void Invoke()
    {
        var dasiesLocation = new Location(3196, 3203, 0);

        var gameObject = Region.GetObject(1189, dasiesLocation.X, dasiesLocation.Y, dasiesLocation.Z);
        if (gameObject == null)
        {
            Player.Session.PacketBuilder.SendMessage("Object does not exist.");
            return;
        }

        var treeStrump = new WorldObject(7399, dasiesLocation.X, dasiesLocation.Y, dasiesLocation.Z, gameObject.Direction, 10, 0);

        var relX = dasiesLocation.X - Player.Location.CachedBuildAreaStartX;
        var relY = dasiesLocation.Y - Player.Location.CachedBuildAreaStartY;

        var relZoneX = relX & ~0x7;
        var relZoneY = relY & ~0x7;

        var inZoneX = relX & 0x7;
        var inZoneY = relY & 0x7;


        Player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);
        Player.Session.PacketBuilder.UpdateObject(inZoneX, inZoneY, treeStrump);
        Player.Session.PacketBuilder.SendMessage("Tried to remove object.");
    }
}