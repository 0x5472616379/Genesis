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
        _listener = new TcpListener(IPAddress.Loopback, ServerConfig.PORT);
        _listener.Start(200);
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

    // Define these variables at a static level to track the grid positions
    private static int _currentX = 0; // tracks the x-coordinate within the column
    private static int _currentY = 0; // tracks the y-coordinate within the grid
    private const int GridStartX = 3208; // Base X-coordinate
    private const int GridStartY = 3421; // Base Y-coordinate
    private const int GridWidth = 14; // Grid width (104 cells)

    
    private static void BuildPlayer()
    {
        var client = _listener.AcceptTcpClient();
        ServerLogger.IncomingConnectionMessage(client);

        var player = ClientManager.InitializeClient(client);

        var av = player.Session.Available();
        if (av < 2)
        {
            RejectLogin(player);
            return;
        }
        
        if (LoginManager.Handshake(player))
        {
            ClientManager.AssignAvailablePlayerSlot(player);
            if (player.IsBot)
            {
                player.Location = GetNextGridLocation();
            }

            ClientManager.Login(player);
        }
        else
        {
            player.Session.Disconnect(new DisconnectInfo(player, "Invalid handshake!"));
        }
    }
    
    private static Location GetNextGridLocation()
    {
        // Compute the current grid position (x, y)
        var location = new Location(GridStartX + _currentX, GridStartY + _currentY, 0);

        // Increment the X position for the next player
        _currentX++;

        // If X exceeds GridWidth, reset X to 0 and move to the next Y row
        if (_currentX >= GridWidth)
        {
            _currentX = 0;
            _currentY++; // Proceed to the next row
        }

        return location;
    }


    private static void RejectLogin(Player player)
    {
        player.Session.Close();
        World.RemovePlayer(player);
        Console.WriteLine("Connection Rejected.");
    }
}