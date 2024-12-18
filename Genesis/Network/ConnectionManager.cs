using System.Net;
using System.Net.Sockets;
using Genesis.Client;
using Genesis.Entities;
using Genesis.Environment;

namespace Genesis;

public class ConnectionManager
{
    private const int MaxClientsPerCycle = 10;
    private static TcpListener _listener;

    public static void Initialize()
    {
        _listener = new TcpListener(IPAddress.Any, ServerConfig.PORT);
        _listener.Start(50);
    }

    public static void AcceptClients()
    {
        for (int i = 0; i < MaxClientsPerCycle; i++)
        {
            if (!_listener.Pending())
                break;
            
            /* Build Player */
            BuildPlayer();
        }
    }

    private static void BuildPlayer()
    {
        var client = _listener.AcceptTcpClient();
        ServerLogger.IncomingConnectionMessage(client);

        var player = ClientManager.InitializeClient(client);

        if (player.Session.Available() < 2)
        {
            RejectLogin(player);
            return;
        }
            
        if (LoginManager.Handshake(player))
        {
            ClientManager.AssignAvailablePlayerSlot(player);
            ClientManager.Login(player);
        }
        else
        {
            player.Session.Disconnect(new DisconnectInfo(player, "Invalid handshake!"));
        }
    }

    private static void RejectLogin(Player player)
    {
        player.Session.Close();
        World.RemovePlayer(player);
        Console.WriteLine("Connection Rejected.");
    }
}