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
            "admin" => new SetAdminCommand(player, _commandArgs),
            "pos" => new PrintPositionCommand(player, _commandArgs),
            "tele" => new TeleportCommand(player, _commandArgs),
            _ => new NullCommand(player, _commandArgs)
        };
    }
}