using System.Net.Sockets;
using ArcticRS.Appearance;
using Genesis.Configuration;
using Genesis.Constants;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Managers;
using Genesis.Model;
using Genesis.Skills;
using Genesis.Skills.Woodcutting;

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
        player.Location.Build();
        // player.Location.RefreshObjects(player);
        EnvironmentBuilder.UpdateBuildArea(player);
        player.Session.PacketBuilder.SendNewBuildAreaPacket();
        player.Flags = PlayerUpdateFlags.Appearance;
        player.PerformedTeleport = true;

        player.Session.PacketBuilder.DisplayWelcomeScreen();
        player.Session.PacketBuilder.SendPlayerStatus();

        player.Session.PacketBuilder.SendInteractionOption("Attack", 3, true);
        player.Session.PacketBuilder.SendInteractionOption("Follow", 5, false);
        player.Session.PacketBuilder.SendInteractionOption("Trade with", 4, false);

        player.Session.PacketBuilder.SendSidebarInterface(0, GameInterfaces.WeaponInterface);
        player.Session.PacketBuilder.SendSidebarInterface(1, GameInterfaces.SkillInterface);
        player.Session.PacketBuilder.SendSidebarInterface(2, GameInterfaces.QuestInterface);
        player.Session.PacketBuilder.SendSidebarInterface(3, GameInterfaces.InventoryInterface);
        player.Session.PacketBuilder.SendSidebarInterface(4, GameInterfaces.EquipmentInterface);
        player.Session.PacketBuilder.SendSidebarInterface(5, GameInterfaces.PrayerInterface);
        player.Session.PacketBuilder.SendSidebarInterface(6, GameInterfaces.AncientMagiksInterface);
        player.Session.PacketBuilder.SendSidebarInterface(8, GameInterfaces.FriendsInterface);
        player.Session.PacketBuilder.SendSidebarInterface(9, GameInterfaces.IgnoreInterface);
        player.Session.PacketBuilder.SendSidebarInterface(10, GameInterfaces.LogoutInterface);
        player.Session.PacketBuilder.SendSidebarInterface(11, GameInterfaces.SettingsInterface);
        player.Session.PacketBuilder.SendSidebarInterface(12, GameInterfaces.PlayerControlsInterface);
        player.Session.PacketBuilder.SendSidebarInterface(13, GameInterfaces.MusicInterface);
        player.Session.PacketBuilder.SendFriendListStatus(FriendListStatus.LOADED);
        player.SkillManager.RefreshSkills();

        // for (int i = 0; i <= 3; i++)
        //     player.InventoryManager.AddItem(526);
        //
        // for (int i = 4; i <= 7; i++)
        //     player.InventoryManager.AddItem(379);
        //
        // player.InventoryManager.AddItem(544);
        // player.InventoryManager.AddItem(542);
        
        LoadPVPGear(player);
        // LoadTestItems(player);
         for (int i = 0; i < AxeData.GetAllAxeIds().Count; i++)
             player.Inventory.AddItem(AxeData.GetAllAxeIds()[i], 1);
        
        // player.InventoryItemContainer.AddItem(555, 1500);
        // player.InventoryItemContainer.AddItem(560, 550);
        // player.InventoryItemContainer.AddItem(565, 1000);
        
        // player.BankContainer.AddItem(995, 2147483640);
        // player.BankItemContainer.AddItem(385, 1000);
        player.Inventory.Refresh(player, GameInterfaces.DefaultInventoryContainer);
    }

    static void LoadPVPGear(Player player)
    {
         player.Inventory.AddItem(6107, 1); /* Ghost Robe Top*/
         player.Inventory.AddItem(4675, 1); /* Ancient Staff */
         player.Inventory.AddItem(2581, 1); /* Robin Hood Hat*/
         player.Inventory.AddItem(6737, 1); /* Bring */
         player.Inventory.AddItem(2497, 1); /* Black dhide chaps */
         player.Inventory.AddItem(3842, 1); /* unholy book */
         player.Inventory.AddItem(6585, 1); /* Amulet of Fury */
         player.Inventory.AddItem(5698, 1); /* Dds */
         player.Inventory.AddItem(861,  1); /* MSB */
         player.Inventory.AddItem(892, 1000); /* Rune Arrows */
         player.Inventory.AddItem(3105, 1); /* Climbing Boots */
         player.Inventory.AddItem(6570, 1); /* Fire cape */
         player.Inventory.AddItem(2491, 1); /* Black dhide vambs */
         player.Inventory.AddItem(4587, 1); /* Dscim */
        
    }
    
    static void LoadTestItems(Player player)
    {
        // player.InventoryItemContainer.AddItem(526, 1); /* Bones */
        // player.InventoryItemContainer.AddItem(385, 1); /* Shark */
        // player.InventoryItemContainer.AddItem(3144, 1); /* Karambwan */
    }
}