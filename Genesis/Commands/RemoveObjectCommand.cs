using ArcticRS.Commands;
using ArcticRS.Constants;
using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Environment;

namespace Genesis.Commands;

public class RemoveObjectCommand : RSCommand
{
    protected override PlayerRights RequiredRights { get; } = PlayerRights.NORMAL;

    public RemoveObjectCommand(Player player, string[] args) : base(player, args)
    {
    }

    public override bool Validate()
    {
        return true;
    }

    public override void Invoke()
    {
        var treeLocations = new[]
        {
            new Location(3264, 3235, 0),
            new Location(3266, 3230, 0),
            new Location(3264, 3225, 0)
        };

        var regionUpdates = PrecomputeRegionalUpdates(treeLocations);
        SendRegionUpdatesToPlayers(regionUpdates);
    }


    private List<RegionUpdate> PrecomputeRegionalUpdates(Location[] treeLocations)
    {
        var regionUpdates = new List<RegionUpdate>();

        foreach (var treeLocation in treeLocations)
        {
            var wEntity = Region.GetObject(1276, treeLocation.X, treeLocation.Y, treeLocation.Z);
            if (wEntity == null)
            {
                Player.Session.PacketBuilder.SendMessage(
                    $"Object does not exist at location: X = {treeLocation.X}, Y = {treeLocation.Y}");
                continue;
            }

            var tStump = new WorldObject(1342, treeLocation.X, treeLocation.Y, treeLocation.Z, wEntity.Direction, 10,
                0);

            var relX = treeLocation.X - Player.Location.CachedBuildAreaStartX;
            var relY = treeLocation.Y - Player.Location.CachedBuildAreaStartY;

            var relZoneX = (byte)(relX & ~0x7);
            var relZoneY = (byte)(relY & ~0x7);

            var pos = EncodeRelativePosition(relX, relY);

            var modifiedEntity = CreateModifiedEntity(wEntity, tStump, treeLocation);

            var existingUpdate = regionUpdates.FirstOrDefault(u => u.RelZoneX == relZoneX && u.RelZoneY == relZoneY);
            if (existingUpdate == null)
            {
                var newUpdate = new RegionUpdate
                {
                    RelZoneX = relZoneX,
                    RelZoneY = relZoneY,
                    Entries = new List<RegionUpdateEntry>
                    {
                        new() { Position = pos, Entity = modifiedEntity }
                    }
                };
                regionUpdates.Add(newUpdate);
            }
            else
            {
                existingUpdate.Entries.Add(new RegionUpdateEntry { Position = pos, Entity = modifiedEntity });
            }
        }

        return regionUpdates;
    }

    private void SendRegionUpdatesToPlayers(List<RegionUpdate> regionUpdates)
    {
        foreach (var player in World.GetPlayers())
        {
            if (player == null) continue;

            foreach (var regionUpdate in regionUpdates)
            {
                player.Session.Writer.CreateFrameVarSizeWord(ServerOpCodes.REGION_UPDATE);
                player.Session.Writer.WriteByte(regionUpdate.RelZoneY);
                player.Session.Writer.WriteByteC(regionUpdate.RelZoneX);

                foreach (var entry in regionUpdate.Entries)
                {
                    WriteObjectAdd(player.Session.Writer, entry.Position, entry.Entity);
                }

                player.Session.Writer.EndFrameVarSizeWord();
            }
        }
    }

    private ModifiedEntity CreateModifiedEntity(WorldObject wEntity, WorldObject tStump, Location location)
    {
        return new ModifiedEntity
        {
            OriginalId = wEntity.Id,
            Id = tStump.Id,
            Type = tStump.Type,
            Face = tStump.Direction,
            Location = location,
            Delay = 20
        };
    }

    private byte EncodeRelativePosition(int relX, int relY)
    {
        var inZoneX = relX & 0x7;
        var inZoneY = relY & 0x7;
        return (byte)(((inZoneX & 0x7) << 4) | (inZoneY & 0x7));
    }

    private void WriteObjectAdd(RSStream writer, byte pos, ModifiedEntity entity)
    {
        writer.WriteByte((byte)ServerOpCodes.OBJ_ADD);
        writer.WriteByteA(pos);
        writer.WriteWordBigEndian(entity.Id);
        writer.WriteByteS((entity.Type << 2) | (entity.Face & 3));
    }
}

public class RegionUpdate
{
    public byte RelZoneX { get; set; }
    public byte RelZoneY { get; set; }
    public List<RegionUpdateEntry> Entries { get; set; } = new();
}

public class RegionUpdateEntry
{
    public byte Position { get; set; }
    public ModifiedEntity Entity { get; set; }
}