using Genesis.Entities;
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
        // MessageTickCount();

        /* 1. Fetch Data */
        CollectPlayerPackets();
        ProcessPackets();

        /* Process Regular Movement */
        // ProcessPlayerMovement();

        /* If we did follow someone then process again for this player */
        // foreach (var player in Players) 
        // {
        //     if (player == null) continue;
        //     if (player.Following != null)
        //     {
        //         player.MovementHandler.Reset();
        //         RSPathfinder.MeleeFollow(player, player.Following);
        //         player.MovementHandler.Finish();
        //
        //         player.MovementHandler.Process();
        //     }
        // }
        

        // ProcessPlayerMovement();
        
        ProcessActions();
        ProcessPlayerInteractions();
        EnvironmentBuilder.Process();
        
        foreach (var player in Players)
        {
            if (player == null) continue;
            var isMoving = (player.MovementHandler.IsWalking || player.MovementHandler.IsRunning);
            // player.Session.PacketBuilder.SendMessage($"IsMoving: {isMoving}");
            player.Session.PacketBuilder.SendMessage($"PosX: {player.Location.X} PosY: {player.Location.Y}");
        }

        /* Refresh */
        RefreshPlayer();

        /* 7. Client Visual Updates */
        PlayerUpdateManager.Update();

        /* 8. Flush and Reset */
        FlushAllPlayers();
        Reset();
    }

    private static void ProcessPlayerInteractions()
    {
        foreach (var player in Players)
        {
            if (player == null) continue;
            if (player.CurrentInterraction == null) continue;

            if (player.CurrentInterraction.Execute())
                player.CurrentInterraction = null;
        }
    }

    private static void ProcessPlayerMovement()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == null) continue;
            if (Players[i].CurrentInterraction != null || Players[i].Following != null) continue;
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

    public static Player? GetPlayerByHashCode(int hashCode)
    {
        return Players.FirstOrDefault(player => player != null && player.GetHashCode() == hashCode);
    }
}