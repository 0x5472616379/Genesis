﻿using Genesis.Entities;
using Genesis.Managers;
using Genesis.Movement;
using ICSharpCode.SharpZipLib.Core;

namespace Genesis.Environment;

public class World
{
    private static Player[] Players = new Player[ServerConfig.MAX_PLAYERS];
    private static Entity[] NPCs = new Entity[ServerConfig.MAX_NPCS];

    public static void Process()
    {
        /* 1. Fetch Data */
        CollectPlayerPackets();
        ProcessPackets();

        /* 2. Pre-process state */
        PreProcessTick();

        /* 4. Process movement */
        ProcessMovement();

        /* 3. Process interactions (pathfinding setup) */
        ProcessPlayerInteractions();

        /* 5. Final interaction validation */
        FinalizeInteractions();

        /* 7. Client Visual Updates */
        PlayerUpdateManager.Update();

        /* 8. Flush and Reset */
        FlushAllPlayers();
        Reset();
    }

    private static void PreProcessTick()
    {
        foreach (var player in Players)
        {
            if (player == null) continue;
            player.PreProcessTick();
        }
    }

    public static void ProcessPlayerInteractions()
    {
        foreach (var player in Players)
        {
            if (player?.CurrentInteraction == null || player.NormalDelayTicks > 0) continue;

            int distance = MovementHelper.GameSquareDistance(
                player.Location.X, player.Location.Y,
                player.CurrentInteraction.Target.X, player.CurrentInteraction.Target.Y
            );

            if (distance > player.CurrentInteraction.MaxDistance && !player.PlayerMovementHandler.HasSteps)
            {
                player.PlayerMovementHandler.Reset();
                RSPathfinder.FindPath(player,
                    player.CurrentInteraction.Target.X,
                    player.CurrentInteraction.Target.Y,
                    true, 1, 1
                );
                player.PlayerMovementHandler.Finish();
            }
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

    public static void FinalizeInteractions()
    {
        foreach (var player in Players)
        {
            if (player?.CurrentInteraction == null || player.NormalDelayTicks > 0) continue;

            int distance = MovementHelper.GameSquareDistance(
                player.Location.X, player.Location.Y,
                player.CurrentInteraction.Target.X, player.CurrentInteraction.Target.Y
            );

            if (distance <= player.CurrentInteraction.MaxDistance)
            {
                if (player.CurrentInteraction.Execute())
                {
                    player.CurrentInteraction = null;
                }
            }
        }
    }

    // private static void ProcessPlayerInteractions()
    // {
    //     foreach (var player in Players)
    //     {
    //         if (player == null) continue;
    //         if (player.CurrentInteraction == null) continue;
    //
    //         if (player.CurrentInteraction.Execute())
    //             player.CurrentInteraction = null;
    //     }
    // }

    private static void RefreshPlayer()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == null) continue;

            Players[i].EquipmentManager.Refresh();
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

    public static Player? GetPlayerByHashCode(int hashCode)
    {
        return Players.FirstOrDefault(player => player != null && player.GetHashCode() == hashCode);
    }
}