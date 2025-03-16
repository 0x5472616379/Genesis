using System.Net.Sockets;
using ArcticRS.Appearance;
using Genesis.Configuration;
using Genesis.Constants;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Managers;
using Genesis.Model;
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

        for (int i = 0; i < AxeData.GetAllAxeIds().Count; i++)
            player.InventoryItemContainer.AddItem(AxeData.GetAllAxeIds()[i], 1);

        player.BankItemContainer.AddItem(995, 2147483640);

        player.InventoryItemContainer.Refresh(player, GameInterfaces.DefaultInventoryContainer);

        // player.InventoryManager.RefreshInventory();
        // player.EquipmentManager.Refresh();


        // player.EquipmentManager.Equip(new RSItem(544, 1), EquipmentSlot.Chest);
        // player.EquipmentManager.Equip(new RSItem(542, 1), EquipmentSlot.Legs);
    }
}