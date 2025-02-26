using Genesis.Entities;
using Genesis.Managers;

namespace Genesis.Environment;

public class World
{
    private static Player[] Players = new Player[ServerConfig.MAX_PLAYERS];
    private static Entity[] NPCs = new Entity[ServerConfig.MAX_NPCS];

    private static int TICK_COUNT = 0;

    public static void Process()
    {
        // MessageTickCount();

        /* 1. Fetch Data */
        CollectPlayerPackets();
        ProcessPackets();

        /* 2. Process Actions / Interactions */
        ProcessActions();

        /* Process Interactions */
        foreach (var player in Players)
        {
            if (player == null) continue;
            if (player.CurrentInterraction == null) continue;
            
            if (player.CurrentInterraction.Execute())
                player.CurrentInterraction = null;
        }
        
        /* 3. Process World Updates (Spawn Ground Items etc.) */
        /* 4. Process NPC Movement */
        /* 5. Process Player Movement */
        ProcessPlayerMovement();

        /* 6. Combat */

        /* Refresh */
        RefreshPlayer();

        /* 7. Client Visual Updates */
        PlayerUpdateManager.Update();

        

        /* 8. Flush and Reset */
        FlushAllPlayers();
        Reset();

        if (TICK_COUNT >= 5)
            TICK_COUNT = 0;

        TICK_COUNT++;
    }

    private static void MessageTickCount()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == null) continue;

            Players[i].Session.PacketBuilder.SendMessage($"Tick: {TICK_COUNT}");
        }
    }


    private static void ProcessPlayerMovement()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == null) continue;
            Players[i].MovementHandler.Process();
        }
    }

    private static void RefreshPlayer()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == null) continue;

            Players[i].EquipmentManager.Refresh();
            Players[i].InventoryManager.RefreshInventory();
        }
    }


    private static void CollectPlayerPackets()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == null) continue;

            for (var n = 0; n < ServerConfig.PACKET_FETCH_LIMIT; n++)
                Players[i].Session.Fetch();
        }
    }

    private static void ProcessPackets()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == null) continue;
            Players[i].Session.PacketCache.Process();
        }
    }

    private static void ProcessActions()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == null) continue;
            Players[i].ActionHandler.ProcessActions();
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