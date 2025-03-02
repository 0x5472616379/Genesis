using Genesis.Entities;

namespace Genesis.Environment;

public class EnvironmentBuilder
{
    public static Dictionary<ModifiedEntity, HashSet<int>> ModifiedEntities { get; set; } = new();

    static int cachedX = -1;
    static int cachedY = -1;
    
    public static void Process()
    {
        // Console.WriteLine($"EntityAmount: {ModifiedEntities.Count}");
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

                    int relX = modifiedEntity.Key.Location.X - player.Location.CachedBuildAreaStartX;
                    int relY = modifiedEntity.Key.Location.Y - player.Location.CachedBuildAreaStartY;
                    int relZoneX = relX & ~0x7;
                    int relZoneY = relY & ~0x7;
                    int inZoneX = relX & 0x7;
                    int inZoneY = relY & 0x7;

                    /* Has not been notified yet */
                    if (!modifiedEntity.Value.Contains(player.GetHashCode()))
                    {
                        player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);
                        player.Session.PacketBuilder.UpdateObject(inZoneX, inZoneY, modifiedEntity.Key);

                        Console.WriteLine("Sent Update");
                        
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

    // public static void Process1()
    // {
    //     // Example: Retrieving the player's current data (assume Player can be sourced)
    //     var players = World.GetPlayers();
    //     
    //     // Placeholder for the list of modified entities within the build area
    //     List<ModifiedEntity> modifiedEntitiesInBuildArea = new List<ModifiedEntity>();
    //     
    //     foreach (var player in players)
    //     {
    //         if (player == null) continue;
    //
    //         // Retrieve the build area boundaries
    //         int buildAreaStartX = player.Location.CachedBuildAreaStartX;
    //         int buildAreaStartY = player.Location.CachedBuildAreaStartY;
    //         int buildAreaEndX = buildAreaStartX + (13 << 3); // 13 chunks * 8 tiles per chunk
    //         int buildAreaEndY = buildAreaStartY + (13 << 3);
    //
    //
    //         // Iterate through all tiles in the defined build area
    //         for (int x = buildAreaStartX; x < buildAreaEndX; x++)
    //         {
    //             for (int y = buildAreaStartY; y < buildAreaEndY; y++)
    //             {
    //                 // Check if this entity exists in the ModifiedEntities list
    //                 foreach (var entry in ModifiedEntities)
    //                 {
    //                     // Retrieve entities from the world in the current tile position
    //                     var worldEntity = Region.GetExpectedObjectAt(entry.Key.OriginalId, x, y, player.Location.Z);
    //                     if (worldEntity == null) continue;
    //
    //                     bool isModifiedEntity = entry.Key.Location.X == x &&
    //                                             entry.Key.Location.Y == y &&
    //                                             entry.Key.Location.Z == player.Location.Z &&
    //                                             entry.Key.Id != worldEntity.Id;
    //
    //                     if (isModifiedEntity && !entry.Value.Contains(player.GetHashCode())) // Not yet notified
    //                     {
    //                         // Add to the list of modified entities
    //                         modifiedEntitiesInBuildArea.Add(entry.Key);
    //                     }
    //                 }
    //             }
    //         }
    //
    //         Console.WriteLine($"ModifiedEntitiesInBuildArea: {modifiedEntitiesInBuildArea.Count}");
    //         
    //         if (modifiedEntitiesInBuildArea.Count > 1)
    //         {
    //             
    //             int relX = player.Location.CachedAbsoluteCenterX - player.Location.CachedBuildAreaStartX;
    //             int relY = player.Location.CachedAbsoluteCenterY - player.Location.CachedBuildAreaStartY;
    //             int relZoneX = relX & ~0x7;
    //             int relZoneY = relY & ~0x7;
    //             int inZoneX = relX & 0x7;
    //             int inZoneY = relY & 0x7;
    //             
    //             player.Session.PacketBuilder.SendActiveRegion(player.Location.CachedBuildAreaStartX, 
    //                 player.Location.CachedBuildAreaStartY, 
    //                 player,
    //                 modifiedEntitiesInBuildArea);
    //         }
    //         modifiedEntitiesInBuildArea.Clear();
    //     }
    //
    //
    //     // // Now, decide which packet to send based on the number of modified entities found
    //     // if (modifiedEntitiesInBuildArea.Count > 1)
    //     // {
    //     //     // Send packet 60 + all entities
    //     //     SendPacket(60, modifiedEntitiesInBuildArea);
    //     // }
    //     // else if (modifiedEntitiesInBuildArea.Count == 1)
    //     // {
    //     //     // Send packet 80 + the single entity
    //     //     SendPacket(80, modifiedEntitiesInBuildArea[0]);
    //     // }
    //     //
    //     // // Helper function for sending packets (mock implementation)
    //     // void SendPacket(int packetCode, object data)
    //     // {
    //     //     // Example of how you'd send the "packet"; details would depend on your system
    //     //     Console.WriteLine($"Sending Packet Code: {packetCode}, Data: {data}");
    //     // }
    // }

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
                var worldEntity = Region.GetExpectedObjectAt(entity.OriginalId, entity.Location.X, entity.Location.Y,
                    player.Location.Z);
                if (worldEntity == null)
                    continue;

                var revertObject = new ModifiedEntity
                {
                    Id = entity.OriginalId,
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

                Console.WriteLine($"Reverted from: {worldEntity.Id} to: {revertObject.OriginalId}");
                Console.WriteLine($"Reverted from: {worldEntity.X}  to: {revertObject.Location.X}");
                Console.WriteLine($"Reverted from: {worldEntity.Y}  to: {revertObject.Location.Y}");

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
    public int OriginalId { get; set; }
    public int Id { get; set; }
    public Location Location { get; set; }
    public int Face { get; set; }
    public int Type { get; set; }
    public int Delay { get; set; }
}