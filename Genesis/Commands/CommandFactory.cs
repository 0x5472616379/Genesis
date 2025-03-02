using ArcticRS.Commands;
using Genesis.Commands;
using Genesis.Entities;

namespace ArcticRS.Factories;

public class CommandFactory
{
    public static CommandBase CreateCommand(string[] _commandArgs, Player player)
    {
        var commandName = _commandArgs[0];
        return commandName switch
        {
            "item" => new SpawnItemCommand(player, _commandArgs),
            "admin" => new SetAdminCommand(player, _commandArgs),
            "pos" => new PrintPositionCommand(player, _commandArgs),
            "tele" => new TeleportCommand(player, _commandArgs),
            "play" => new PlaySoundCommand(player, _commandArgs),
            "song" => new PlaySongCommand(player, _commandArgs),
            "clear" => new ClearInventoryCommand(player, _commandArgs),
            "remove" => new RemoveObjectCommand(player, _commandArgs),
            "anim" => new PlayAnimationCommand(player, _commandArgs),
            "obj" => new GetWorldObjectCommand(player, _commandArgs),
            "refresh" => new RefreshBuildAreaObjectsCommand(player, _commandArgs),
            _ => new NullCommand(player, _commandArgs)
        };
    }
}