using Genesis.Entities;
using Genesis.Environment;
using Genesis.Model;

namespace Genesis.Managers;
/// <summary>
/// Inefficient, needs work more on an "Area" basis.
/// </summary>
public class WorldDrop
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int Delay { get; set; } = 20;
    public Player VisibleTo { get; set; }
    public HashSet<int> NotifiedPlayerIds { get; set; } = new();
    public HashSet<int> PlayersNeedingRemoval { get; set; } = new();
}

public static class WorldDropManager
{
    public static List<WorldDrop> WorldDrops { get; set; } = new();

    public static void Process()
    {
        foreach (var drop in WorldDrops.ToList())
        {
            if (drop.Delay <= 0)
            {
                /* Notify ALL players who ever saw this drop to remove it */
                foreach (var playerId in drop.PlayersNeedingRemoval.ToList())
                {
                    var player = World.GetPlayers().FirstOrDefault(x => x.Session.Index == playerId);
                    if (player != null)
                    {
                        RemoveDropForPlayer(drop, player);
                    }
                }
                WorldDrops.Remove(drop);
                continue;
            }

            var location = new Location(drop.X, drop.Y, drop.Z);
            var nearbyPlayers = GetPlayersWithinLocation(location).ToList();

            /* Add new players to notification list */
            foreach (var player in nearbyPlayers)
            {
                /* Skip if restricted to a specific player */
                if (drop.VisibleTo != null && player != drop.VisibleTo)
                    continue;

                if (!drop.NotifiedPlayerIds.Contains(player.Session.Index))
                {
                    SendDropToPlayer(drop, player);
                    drop.NotifiedPlayerIds.Add(player.Session.Index);
                    
                    /* Mark for future removal */
                    drop.PlayersNeedingRemoval.Add(player.Session.Index);
                }
            }

            /* Make visible to all players after delay reaches 10 */
            if (drop.Delay <= 10 && drop.VisibleTo != null)
            {
                drop.VisibleTo = null;
                foreach (var player in nearbyPlayers)
                {
                    if (!drop.NotifiedPlayerIds.Contains(player.Session.Index))
                    {
                        SendDropToPlayer(drop, player);
                        drop.NotifiedPlayerIds.Add(player.Session.Index);
                        drop.PlayersNeedingRemoval.Add(player.Session.Index);
                    }
                }
            }

            drop.Delay--;
        }
    }

    private static void SendDropToPlayer(WorldDrop drop, Player player)
    {
        var item = new Item(drop.Id, drop.Amount);
        int relX = drop.X - player.Location.CachedBuildAreaStartX;
        int relY = drop.Y - player.Location.CachedBuildAreaStartY;
        byte relZoneX = (byte)(relX & ~0x7);
        byte relZoneY = (byte)(relY & ~0x7);
        int inZoneX = relX & 0x7;
        int inZoneY = relY & 0x7;

        player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);
        player.Session.PacketBuilder.SendGroundItem(item, inZoneX, inZoneY);
    }

    private static void RemoveDropForPlayer(WorldDrop drop, Player player)
    {
         var item = new Item(drop.Id, drop.Amount);
         int relX = drop.X - player.Location.CachedBuildAreaStartX;
         int relY = drop.Y - player.Location.CachedBuildAreaStartY;
         byte relZoneX = (byte)(relX & ~0x7);
         byte relZoneY = (byte)(relY & ~0x7);
         int inZoneX = relX & 0x7;
         int inZoneY = relY & 0x7;
        
         player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);
         player.Session.PacketBuilder.RemoveGroundItem(inZoneX, inZoneY, item.Id);
    }

    public static void AddDrop(WorldDrop drop)
    {
        WorldDrops.Add(drop);
    }
    
    public static IEnumerable<Player> GetPlayersWithinLocation(Location location)
    {
        return World.GetPlayers().Where(player => player != null && player.Location.IsWithinArea(location));
    }
    
    public static bool RemoveDropAt(int x, int y, int z, int id)
    {
        // Find the drop to remove based on x, y, z, and id
        var dropToRemove = WorldDrops.FirstOrDefault(drop => drop.X == x && drop.Y == y && drop.Z == z && drop.Id == id);

        if (dropToRemove != null)
        {
            // Notify all players who might still see it
            foreach (var playerId in dropToRemove.PlayersNeedingRemoval.ToList())
            {
                var player = World.GetPlayers().FirstOrDefault(p => p.Session.Index == playerId);
                if (player != null)
                {
                    RemoveDropForPlayer(dropToRemove, player);
                }
            }
            
            // Remove the drop from the world
            WorldDrops.Remove(dropToRemove);
            
            return true; // Successfully removed
        }

        return false; // No matching drop found
    }

    public static WorldDrop ItemExists(int x, int y, int z, int id)
    {
        var item = WorldDrops.FirstOrDefault(drop => drop.X == x && drop.Y == y && drop.Z == z && drop.Id == id);
        return item;
    }

}