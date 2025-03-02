using Genesis.Cache;
using Genesis.Entities;
using Genesis.Environment;

namespace Genesis.Managers;

public class WorldObjectManager
{
    // public static List<RespawnObject> Respawning { get; set; } = new();
    // public static Dictionary<RespawnObject, HashSet<int>> notifiedClients = new();
    //
    // public static List<RespawnObject> toRemove { get; set; } = new();
    //
    // public static void Process()
    // {
    //     foreach (var worldObject in Respawning)
    //     {
    //         if (worldObject.Delay >= 0)
    //         {
    //             if (!notifiedClients.TryGetValue(worldObject, out var clientsNotified))
    //             {
    //                 clientsNotified = new HashSet<int>();
    //                 notifiedClients[worldObject] = clientsNotified;
    //             }
    //
    //             var players = World.GetPlayers();
    //             var playersToRemove = new List<int>();
    //
    //             foreach (var clientId in clientsNotified)
    //             {
    //                 var player = players.FirstOrDefault(p => p?.GetHashCode() == clientId);
    //                 if (player == null || !player.Location.IsWithinArea(
    //                         new Location(worldObject.ReplaceWith.X, worldObject.ReplaceWith.Y,
    //                             worldObject.ReplaceWith.Height)))
    //                 {
    //                     playersToRemove.Add(clientId);
    //                 }
    //             }
    //
    //             foreach (var clientId in playersToRemove)
    //             {
    //                 clientsNotified.Remove(clientId);
    //             }
    //
    //             for (int i = 0; i < players.Length; i++)
    //             {
    //                 var player = players[i];
    //
    //                 if (player == null)
    //                     continue;
    //
    //                 if (player.Location.IsWithinArea(new Location(worldObject.ReplaceWith.X,
    //                         worldObject.ReplaceWith.Y,
    //                         worldObject.ReplaceWith.Height)))
    //                 {
    //                     if (!clientsNotified.Contains(player.GetHashCode()))
    //                     {
    //                         clientsNotified.Add(player.GetHashCode());
    //                         player.Session.PacketBuilder.SendMessage("Ehm");
    //
    //                         var relX = worldObject.ReplaceWith.X - player.Location.CachedBuildAreaStartX;
    //                         var relY = worldObject.ReplaceWith.Y - player.Location.CachedBuildAreaStartY;
    //
    //                         var relZoneX = relX & ~0x7;
    //                         var relZoneY = relY & ~0x7;
    //
    //                         var inZoneX = relX & 0x7;
    //                         var inZoneY = relY & 0x7;
    //
    //                         player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);
    //                         player.Session.PacketBuilder.UpdateObject(inZoneX, inZoneY, worldObject.ReplaceWith);
    //                     }
    //                 }
    //             }
    //         }
    //
    //         worldObject.Delay--;
    //         if (worldObject.Delay <= 0)
    //         {
    //             toRemove.Add(worldObject);
    //         }
    //     }
    //
    //     // Process to remove respawned objects and put back originals
    //     foreach (var worldObject in toRemove)
    //     {
    //         // Respawn Original Object
    //         var players = World.GetPlayers();
    //         foreach (var player in players)
    //         {
    //             if (player == null)
    //                 continue;
    //
    //             if (player.Location.IsWithinArea(new Location(worldObject.Original.X, worldObject.Original.Y, worldObject.Original.Height)))
    //             {
    //                 var relX = worldObject.Original.X - player.Location.CachedBuildAreaStartX;
    //                 var relY = worldObject.Original.Y - player.Location.CachedBuildAreaStartY;
    //
    //                 var relZoneX = relX & ~0x7;
    //                 var relZoneY = relY & ~0x7;
    //
    //                 var inZoneX = relX & 0x7;
    //                 var inZoneY = relY & 0x7;
    //
    //                 // Notify player about the original object being restored
    //                 //player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);
    //                 //player.Session.PacketBuilder.UpdateObject(inZoneX, inZoneY, worldObject.Original);
    //             }
    //         }
    //
    //         // Remove from respawning list
    //         Respawning.Remove(worldObject);
    //         notifiedClients.Remove(worldObject);
    //     }
    //
    //     // Clear the removed objects list
    //     toRemove.Clear();
    //
    // }
    //
    // public static void Add(RespawnObject obj)
    // {
    //     Respawning.Add(obj);
    // }
    //
    // public static void Notify(RespawnObject obj, Player player)
    // {
    //     if (notifiedClients.TryGetValue(obj, out var clientsNotified))
    //     {
    //         clientsNotified.Add(player.GetHashCode());
    //     }
    // }
    //
    // public static bool IsConflict(WorldObject obj1, WorldObject obj2)
    // {
    //     return obj1.X == obj2.X && obj1.Y == obj2.Y && obj1.Height == obj2.Height && obj1.Id != obj2.Id;
    // }
    //
    // public static void HandleConflict(WorldObject respawnObj, int tileX, int tileY, Player player)
    // {
    //     Console.WriteLine($"Conflict found at ({tileX}, {tileY}): " +
    //                       $"Existing Object ID: {Region.GetObjectAt(tileX, tileY, respawnObj.Height)?.Id}, Respawning Object ID: {respawnObj.Id}");
    //
    //     int relX = respawnObj.X - player.Location.CachedBuildAreaStartX;
    //     int relY = respawnObj.Y - player.Location.CachedBuildAreaStartY;
    //
    //     int relZoneX = relX & ~0x7;
    //     int relZoneY = relY & ~0x7;
    //
    //     int inZoneX = relX & 0x7;
    //     int inZoneY = relY & 0x7;
    //
    //     player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);
    //     player.Session.PacketBuilder.UpdateObject(inZoneX, inZoneY, respawnObj);
    // }
}

// public class RespawnObject
// {
//     public WorldObject Original { get; set; }
//     public WorldObject ReplaceWith { get; set; }
//     public int Delay { get; set; }
// }