using Genesis.Cache;
using Genesis.Environment;

namespace Genesis.Managers;

public class WorldObjectManager
{
    public List<RespawnObject> Respawning { get; set; } = new();
    private Dictionary<RespawnObject, HashSet<int>> notifiedClients = new();

    public void Process()
    {
        foreach (var worldObject in Respawning)
        {
            /* If the object is despawned */
            if (worldObject.Delay >= 0)
            {
                for (int i = 0; i < World.GetPlayers().Length; i++)
                {
                    var player = World.GetPlayers()[i];
                    
                    if (World.GetPlayers()[i] == null)
                        continue;

                    
                    if (World.GetPlayers()[i].Location.IsWithinArea(new Location(worldObject.WorldObject.X, 
                                                                                 worldObject.WorldObject.Y, 
                                                                                 worldObject.WorldObject.Height)))
                    {
                        if (!notifiedClients.TryGetValue(worldObject, out var clientsNotified))
                        {
                            clientsNotified = new HashSet<int>();
                            notifiedClients[worldObject] = clientsNotified;
                        }

                        if (!clientsNotified.Contains(player.GetHashCode()))
                        {
                            /* Send the packet to the client */
                            // NotifyClient(worldObject, client);

                            // Mark client as notified
                            clientsNotified.Add(player.GetHashCode());
                        }

                        
                    }
                    
                }
            }
        }
    }

    public void Add(RespawnObject obj)
    {
        Respawning.Add(obj);
    }
}

public class RespawnObject
{
    public WorldObject WorldObject { get; set; }
    public int Delay { get; set; }
}