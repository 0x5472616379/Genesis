using ArcticRS.Commands;
using Genesis.Commands;
using Genesis.Entities;

namespace ArcticRS.Factories;

public class CommandFactory
{
    public static RSCommand CreateCommand(string[] _commandArgs, Player player)
    {
        var commandName = _commandArgs[0];
        return commandName switch
        {
            "tele" => new TeleportCommand(player, _commandArgs),
            "gfx" => new PlayGfxCommand(player, _commandArgs),
            "getobj" => new GetWorldObjectCommand(player, _commandArgs),
            "worldobj" => new SpawnWorldObjectCommand(player, _commandArgs),
            "anim" => new PlayAnimationCommand(player, _commandArgs),
            "sound" => new PlaySoundCommand(player, _commandArgs),
            "pos" => new PrintPositionCommand(player, _commandArgs),
            "remove" => new RemoveObjectCommand(player, _commandArgs),
            "admin" => new SetAdminCommand(player, _commandArgs),
            "spawn" => new SpawnGroundItemCommand(player, _commandArgs),
            "item" => new SpawnItemCommand(player, _commandArgs),
            "removeitem" => new RemoveItemCommand(player, _commandArgs),
            "clear" => new ClearInventoryCommand(player, _commandArgs),
            "colors" => new TestColorsCommand(player, _commandArgs),
            "loadbank" => new LoadBankCommand(player, _commandArgs),
            "clearbank" => new ClearBankCommand(player, _commandArgs),
            _ => new NullCommand(player, _commandArgs)
        };
    }
}