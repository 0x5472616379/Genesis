using System.Net.Sockets;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Model;

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
        player.Session.PacketBuilder.SendPlayerStatus();
        player.Session.PacketBuilder.SendSidebarInterface(0,  GameInterfaces.WeaponInterface);
        player.Session.PacketBuilder.SendSidebarInterface(1,  GameInterfaces.SkillInterface);
        player.Session.PacketBuilder.SendSidebarInterface(2,  GameInterfaces.QuestInterface);
        player.Session.PacketBuilder.SendSidebarInterface(3,  GameInterfaces.InventoryInterface);
        player.Session.PacketBuilder.SendSidebarInterface(4,  GameInterfaces.EquipmentInterface);
        player.Session.PacketBuilder.SendSidebarInterface(5,  GameInterfaces.PrayerInterface);
        player.Session.PacketBuilder.SendSidebarInterface(6,  GameInterfaces.NormalMagicInterface);
        player.Session.PacketBuilder.SendSidebarInterface(8,  GameInterfaces.FriendsInterface);
        player.Session.PacketBuilder.SendSidebarInterface(9,  GameInterfaces.IgnoreInterface);
        player.Session.PacketBuilder.SendSidebarInterface(10, GameInterfaces.LogoutInterface);
        player.Session.PacketBuilder.SendSidebarInterface(11, GameInterfaces.SettingsInterface);
        player.Session.PacketBuilder.SendSidebarInterface(12, GameInterfaces.PlayerControlsInterface);
        player.Session.PacketBuilder.SendSidebarInterface(13, GameInterfaces.MusicInterface);
        
        for (int i = 0; i <= 3; i++)
            player.Inventory.AddItem(new RSItem(526, 1));
        
        for (int i = 4; i <= 7; i++)
            player.Inventory.AddItem(new RSItem(379, 1));
        
        player.Inventory.Refresh(GameInterfaces.DefaultInventoryContainer);
    }
}