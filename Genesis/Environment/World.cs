using Genesis.Entities;
using Genesis.Managers;

namespace Genesis.Environment;

public class World
{
    private static Player[] Players = new Player[ServerConfig.MAX_PLAYERS];
    private static Entity[] NPCs = new Entity[ServerConfig.MAX_NPCS];

    public static void Process()
    {
        /* 1. Process Actions / Interactions */

        /* 2. Fetch Data */
        CollectPlayerPackets();
        
        /* 3. Process World Updates (Spawn Ground Items etc.) */
        /* 4. Process NPC Movement */
        /* 5. Process Player Movement */
        /* 6. Combat */
        /* 7. Client Visual Updates */
        PlayerUpdateManager.Update();
        /* 8. Flush and Reset */
        FlushAllPlayers();
        Reset();
    }

    

    private static void CollectPlayerPackets()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == null) continue;
            Players[i].Session.Fetch();
        }
    }

    public static void AddPlayer(Entity player)
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == null)
            {
                Players[i] = (Player)player;
                break;
            }
        }
    }

    public static void RemovePlayer(Entity player)
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == player)
            {
                //override the slot of the player disconnecting and compress
                for (int j = i; j < Players.Length - 1; j++)
                    Players[j] = Players[j + 1];

                break;
            }
        }
    }

    private static void FlushAllPlayers()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == null) continue;
            Players[i].Session.Flush();
        }
    }

    private static void Reset()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == null) continue;
            Players[i].Reset();
        }
    }
    
    public static Player[] GetPlayers() => Players;
    public static int GetPlayerCount() => Players.Count(x => x != null);
}