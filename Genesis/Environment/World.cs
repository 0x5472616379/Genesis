using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Managers;

namespace Genesis.Environment;

public class World
{
    private static Player[] Players = new Player[ServerConfig.MAX_PLAYERS];
    private static Entity[] NPCs = new Entity[ServerConfig.MAX_NPCS];
    public static int CurrentTick = 0;

    public static void Process()
    {
        /* 1. Fetch Data */
        CollectPlayerPackets();
        ProcessPackets();

        WorldDropManager.Process();
        
        /* 2. Pre-process state */
        PreProcessTick();

        /* 3 Actions, Movement, Interactions */
        BulkPlayerProcess();

        /* 5. Client Visual Updates */
        PlayerUpdateManager.Update();

        /* 6. Flush */
        FlushAllPlayers();

        /* 7. Reset */
        Reset();

        
        
        CurrentTick++;
    }

    private static void BulkPlayerProcess()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            var player = Players[i];
            if (player == null) continue;

            /* Movement */
            player.ProcessMovement();
            
            /* Actions (Queues) */
            player.ActionHandler.ProcessActionPipeline();

            /* Interactions */
            if (player.CurrentInteraction != null)
            {
                /* Within Range of the CurrentInteraction */
                // if (player.CurrentInteraction.CanExecute())
                // {
                if (player.CurrentInteraction.Execute())
                {
                    player.CurrentInteraction = null;
                }
                // }
            }
        }
    }


    private static void PreProcessTick()
    {
        foreach (var player in Players)
        {
            if (player == null) continue;
            player.PreProcessTick();
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

    public static Player? GetPlayerByHashCode(int hashCode)
    {
        return Players.FirstOrDefault(player => player != null && player.GetHashCode() == hashCode);
    }
}