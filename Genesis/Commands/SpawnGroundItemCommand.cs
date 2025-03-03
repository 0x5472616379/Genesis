using ArcticRS.Constants;
using Genesis.Entities;
using Genesis.Managers;

namespace Genesis.Commands;

public class SpawnGroundItemCommand : RSCommand
{
    protected override PlayerRights RequiredRights => PlayerRights.ADMIN;

    public SpawnGroundItemCommand(Player player, string[] args) : base(player, args)
    {
    }

    public override bool Validate()
    {
        return true;
    }

    public override void Invoke()
    {
        var item = new Item(995, 10000);

        var relX = Player.Location.X - Player.Location.CachedBuildAreaStartX;
        var relY = Player.Location.Y - Player.Location.CachedBuildAreaStartY;

        var relZoneX = (byte)(relX & ~0x7);
        var relZoneY = (byte)(relY & ~0x7);

        var inZoneX = relX & 0x7;
        var inZoneY = relY & 0x7;

        /* Chunk to update in */
        Player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);
        Player.Session.PacketBuilder.SendGroundItem(item, inZoneX, inZoneY);
    }
}