using Genesis.Entities;
using Genesis.Managers;
using Genesis.Movement;
using ICSharpCode.SharpZipLib.Core;

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

        /* 2. Pre-process state */
        PreProcessTick();

        ProcessActions();
        
        /* 3. Process movement */
        ProcessMovement();

        /* 4. Final interaction validation */
        FinalizeInteractions();

        /* 5. Client Visual Updates */
        PlayerUpdateManager.Update();

        /* 6. Flush */
        FlushAllPlayers();
        
        /* 7. Reset */
        Reset();
        
        CurrentTick++;
    }

    private static void ProcessActions()
    {
        foreach (var player in Players)
        {
            if (player == null) continue;
            player.ActionHandler.ProcessActionPipeline();
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

    private static void ProcessMovement()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == null) continue;

            Players[i].ProcessMovement();
        }
    }

    private static void FinalizeInteractions()
    {
        foreach (var player in Players)
        {
            if (player == null || player.CurrentInteraction == null) continue;
        
            // Block during ANY delay
            if (player.NormalDelayTicks > 0 || player.ArriveDelayTicks > 0)
                continue;

            // Distance check
            int distance = MovementHelper.GameSquareDistance(
                player.Location.X, player.Location.Y,
                player.CurrentInteraction.Target.X, player.CurrentInteraction.Target.Y);

            if (distance <= player.CurrentInteraction.MaxDistance)
            {
                if (player.CurrentInteraction.Execute())
                {
                    player.CurrentInteraction = null;
                }
            }
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