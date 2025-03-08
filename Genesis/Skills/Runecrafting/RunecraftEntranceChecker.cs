using ArcticRS.Actions;
using ArcticRS.Commands;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Packets.Incoming;

namespace Genesis.Skills.Runecrafting;

public class RunecraftEntranceChecker
{
    public static bool TryHandleRunecraftingInteraction(Player player, WorldInteractObject worldObject)
    {
        if (RunecraftingAltarData.AltarEntranceMap.TryGetValue(worldObject.WorldLocDataBits, out var altarInfo))
        {
            if (worldObject.SelectedObjectId == altarInfo.TalismanId)
            {
                if (TeleportCommand.NamedLocations.TryGetValue(altarInfo.NamedLocation, out var namedLocation))
                {
                    player.ActionHandler.AddAction(new TeleportAction(
                        player,
                        new Location(namedLocation.Item1, namedLocation.Item2, namedLocation.Item3), true));
                    return true;
                }

                player.Session.PacketBuilder.SendMessage("I'm not quite sure where this takes me.");
                return false;
            }
        }

        return false;
    }
}