using Genesis.Entities;

namespace Genesis.Environment;

public class EnvironmentBuilder
{
    public static Dictionary<ModifiedEntity, HashSet<int>> ModifiedEntities { get; set; } = new();

    public static void Process()
    {
        foreach (var modifiedEntity in ModifiedEntities)
        {
            if (modifiedEntity.Key.Delay > 0)
            {
                /* These are the ones that were notified of a change */
                /* Since they're far enough away to not notice the update we can remove them */
                /* So that if they were to come close enough again, they would see the update */
                var distantPlayers = GetDistantPlayers(modifiedEntity.Key.Location);
                foreach (var player in distantPlayers)
                {
                    modifiedEntity.Value.Remove(player.GetHashCode());
                }
                
                /* Add the ones that are within the area to ModifiedEntities.Values */
                var nearbyPlayers = GetPlayersWithinLocation(modifiedEntity.Key.Location);

                foreach (var player in nearbyPlayers)
                {
                    var worldEntity = Region.GetObjectAt(modifiedEntity.Key.Location.X, modifiedEntity.Key.Location.Y,
                        player.Location.Z);
                    if (worldEntity == null)
                        continue;

                    /* Has not been notified yet */
                    if (!modifiedEntity.Value.Contains(player.GetHashCode()))
                    {
                        int relX = modifiedEntity.Key.Location.X - player.Location.CachedBuildAreaStartX;
                        int relY = modifiedEntity.Key.Location.Y - player.Location.CachedBuildAreaStartY;
                        int relZoneX = relX & ~0x7;
                        int relZoneY = relY & ~0x7;
                        int inZoneX = relX & 0x7;
                        int inZoneY = relY & 0x7;

                        player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);
                        player.Session.PacketBuilder.UpdateObject(inZoneX, inZoneY, modifiedEntity.Key);

                        /* Flag player as processed */
                        modifiedEntity.Value.Add(player.GetHashCode());
                    }
                }
                modifiedEntity.Key.Delay--;
                if (modifiedEntity.Key.Delay <= 0)
                {
                    Revert();
                }
            }
        }
    }

    public static void UpdateBuildArea(Player player)
    {
        int buildAreaStartX = player.Location.CachedBuildAreaStartX;
        int buildAreaStartY = player.Location.CachedBuildAreaStartY;
        int buildAreaEndX = buildAreaStartX + (13 << 3); // 13 chunks * 8 tiles per chunk
        int buildAreaEndY = buildAreaStartY + (13 << 3);

        for (int x = buildAreaStartX; x < buildAreaEndX; x++)
        {
            for (int y = buildAreaStartY; y < buildAreaEndY; y++)
            {
                var worldEntity = Region.GetObjectAt(x, y, player.Location.Z);
                if (worldEntity == null)
                    continue;

                foreach (var entity in ModifiedEntities)
                {
                    var locX = entity.Key.Location.X;
                    var locY = entity.Key.Location.Y;
                    var locZ = entity.Key.Location.Z;

                    /* If entity at location has been modified */
                    if (worldEntity.X == locX && worldEntity.Y == locY
                                              && worldEntity.Height == locZ
                                              && worldEntity.Id != entity.Key.Id)
                    {
                        var nearbyPlayers = GetPlayersWithinLocation(entity.Key.Location);
                        foreach (var nearbyPlayer in nearbyPlayers)
                        {
                            if (!entity.Value.Contains(nearbyPlayer.GetHashCode()))
                            {
                                int relX = entity.Key.Location.X - player.Location.CachedBuildAreaStartX;
                                int relY = entity.Key.Location.Y - player.Location.CachedBuildAreaStartY;
                                int relZoneX = relX & ~0x7;
                                int relZoneY = relY & ~0x7;
                                int inZoneX = relX & 0x7;
                                int inZoneY = relY & 0x7;

                                player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);
                                player.Session.PacketBuilder.UpdateObject(inZoneX, inZoneY, entity.Key);

                                /* Flag player as processed */
                                entity.Value.Add(player.GetHashCode());
                            }
                        }
                    }
                }
            }
        }
    }

    public static void Add(ModifiedEntity entity)
    {
        ModifiedEntities.Add(entity, new HashSet<int>());
    }

    public static void Revert()
    {
        foreach (var entity in ModifiedEntities.Keys.ToList())
        {
            if (entity.Delay > 0)
                continue;

            var nearbyPlayers = GetPlayersWithinLocation(entity.Location);

            foreach (var player in nearbyPlayers)
            {
                var worldEntity = Region.GetObjectAt(entity.Location.X, entity.Location.Y, player.Location.Z);
                if (worldEntity == null)
                    continue;

                var revertObject = new ModifiedEntity
                {
                    Id = worldEntity.Id,
                    Delay = -1,
                    Face = worldEntity.Direction,
                    Type = worldEntity.Type,
                    Location = new Location(entity.Location.X, entity.Location.Y, entity.Location.Z)
                };

                int relX = entity.Location.X - player.Location.CachedBuildAreaStartX;
                int relY = entity.Location.Y - player.Location.CachedBuildAreaStartY;
                int relZoneX = relX & ~0x7;
                int relZoneY = relY & ~0x7;
                int inZoneX = relX & 0x7;
                int inZoneY = relY & 0x7;

                player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);
                player.Session.PacketBuilder.UpdateObject(inZoneX, inZoneY, revertObject);
            }

            ModifiedEntities.Remove(entity);
        }
    }

    public static IEnumerable<Player> GetPlayersWithinLocation(Location location)
    {
        return World.GetPlayers().Where(player => player != null && player.Location.IsWithinArea(location));
    }

    public static IEnumerable<Player> GetDistantPlayers(Location location)
    {
        var allPlayerIds = ModifiedEntities.Values.SelectMany(hashSet => hashSet);
        var allPlayers = allPlayerIds.Select(playerId => World.GetPlayerByHashCode(playerId));

        return allPlayers.Where(player => player != null && !player.Location.IsWithinArea(location));
    }
}

public class ModifiedEntity
{
    public int Id { get; set; }
    public Location Location { get; set; }
    public int Face { get; set; }
    public int Type { get; set; }
    public int Delay { get; set; }
}