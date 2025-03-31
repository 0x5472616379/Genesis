using ArcticRS.Appearance;
using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Entities.Player;

namespace Genesis.Managers;

public class WeaponInterfaceManager
{
    public static void Refresh(Player player)
    {
        // player.SpecialAttack.UsingSpecial = false;
        var weaponId = player.Equipment.GetItemInSlot(EquipmentSlot.Weapon).ItemId;
        var itemDefinition = ItemDefinition.Lookup(weaponId);
        if (itemDefinition != null)
        {
            if (weaponId == -1)
            {
                itemDefinition.Name = "Unarmed";
            }
            /* Update Weapon Interface */
            var matchedData = GameConstants.WeaponInterfaceData
                .FirstOrDefault(kvp => itemDefinition.Name.ToLower().Contains(kvp.Key.ToLower()))
                .Value;

            // If no matched data was found, try to get the "Other" data
            if (matchedData == null)
            {
                GameConstants.WeaponInterfaceData.TryGetValue("Other", out matchedData);
            }
            if (matchedData != null)
            {
                player.Session.PacketBuilder.SendSidebarInterface(0, matchedData.MainFrameId);
                player.Session.PacketBuilder.SendItemToInterface(weaponId, matchedData.Zoom, matchedData.IconFrameId);
                player.Session.PacketBuilder.SendTextToInterface(itemDefinition.Name, matchedData.TextFrameId);
            }
            
            /* Add Special Attack Bar */
            // AddSpecialBar(weaponId, player);
            
        }
    }

    // public static void DisplaySpecialAmount(Player player, int barId)
    // {
    //     var specAmount = 10;
    //     for (int i = 10; i >= 1; i--)
    //     {
    //         player.Session.PacketBuilder.SendInterfaceOffset(player.SpecialAttack.SpecialAmount >= i ? 500 : 0, 0, --barId);
    //     }
    // }
    //
    // public static void AddSpecialBar(int weapon, Player player)
    // {
    //     Dictionary<int, (int mainFrame, int frameId, int barId)> weaponMap = new Dictionary<int, (int, int, int)>
    //     {
    //         { 4151, (0, 12323, 12335) },
    //         { 6541, (0, 12323, 12335) },
    //         { 6543, (0, 12323, 12335) },
    //         { 6545, (0, 12323, 12335) },
    //         { 6547, (0, 12323, 12335) },
    //         { 6549, (0, 12323, 12335) },
    //         { 6551, (0, 12323, 12335) },
    //         { 14484, (0, 7599, 7611) },
    //         { 859, (0, 7549, 7561) },
    //         { 861, (0, 7549, 7561) },
    //         { 11235, (0, 7549, 7561) },
    //         { 15017, (0, 7549, 7561) },
    //         { 4587, (0, 7599, 7611) },
    //         { 3204, (0, 8493, 8505) },
    //         { 1377, (0, 7499, 7511) },
    //         { 4153, (0, 7474, 7486) },
    //         { 1249, (0, 7674, 7686) },
    //         { 1215, (0, 7574, 7586) },
    //         { 1231, (0, 7574, 7586) },
    //         { 5680, (0, 7574, 7586) },
    //         { 5698, (0, 7574, 7586) },
    //         { 1305, (0, 7574, 7586) },
    //         { 1434, (0, 7624, 7636) }
    //     };
    //
    //      if (weaponMap.TryGetValue(weapon, out var parameters))
    //      {
    //          player.Session.PacketBuilder.DisplayHiddenInterface(parameters.mainFrame, parameters.frameId);
    //          player.Session.SpecialAttack.SpecBarId = parameters.barId;
    //          DisplaySpecialAmount(player, parameters.barId);
    //          //player.SpecialAttack.UpdateSpecialBar();
    //      }
    //      else
    //      {
    //          //player.PacketSender.DisplayHiddenInterface(1, 7624);
    //          //player.PacketSender.DisplayHiddenInterface(1, 7474);
    //          //player.PacketSender.DisplayHiddenInterface(1, 7499);
    //          //player.PacketSender.DisplayHiddenInterface(1, 7549);
    //          //player.PacketSender.DisplayHiddenInterface(1, 7574);
    //          //player.PacketSender.DisplayHiddenInterface(1, 7599);
    //          //player.PacketSender.DisplayHiddenInterface(1, 8493);
    //          //player.PacketSender.DisplayHiddenInterface(1, 12323);
    //      }
    // }
}