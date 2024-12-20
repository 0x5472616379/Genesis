using System.Net.Sockets;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Environment;

namespace Genesis.Client;

public class ClientManager
{
    public static Player InitializeClient(TcpClient tcpClient)
    {
        var player = new Player();
        player.Session.Initialize(tcpClient);
        return player;
    }

    public static void AssignAvailablePlayerSlot(Player player)
    {
        if (World.GetPlayerCount() >= ServerConfig.MAX_PLAYERS)
        {
            Console.WriteLine($"Server is full! Disconnecting {player.Session.Socket.Client.RemoteEndPoint}.");
            player.Session.Disconnect(new DisconnectInfo(player, "Server is full!"));
            throw new Exception("Server is full!");
        }

        World.AddPlayer(player);
        player.Session.Index = World.GetPlayerCount();
        Console.WriteLine($"Incoming connection has been assigned to player {player.Session.Username}!");
    }

    public static void Login(Player player)
    {
        player.Session.PacketBuilder.BuildNewBuildAreaPacket();
        player.Flags = PlayerUpdateFlags.Appearance;
        player.PerformedTeleport = true;
        player.Session.PacketBuilder.DisplayWelcomeScreen();
    }
}