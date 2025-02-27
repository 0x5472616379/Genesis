using ArcticRS.Constants;
using Genesis.Entities;
using Genesis.Environment;

namespace ArcticRS.Commands;

public class PrintPositionCommand : CommandBase
{
    private readonly bool _debug;

    public PrintPositionCommand(Player player, string[] args) : base(player, args)
    {
        _debug = args.Length > 1 && args[1].ToLower() == "debug";
    }

    protected override PlayerRights RequiredRights => PlayerRights.NORMAL;
    
    protected override string ValidateArgs()
    {
        return null;
    }

    protected override void Invoke()
    {
        var location = Player.Location;
        if (!_debug)
        {
            Player.Session.PacketBuilder.SendMessage($"X: {location.X} Y: {location.Y} Z: {location.Z}");
            return;
        }

        foreach (var part in Player.Location.ToStringParts())
        {
            Player.Session.PacketBuilder.SendMessage(part);
        }
    }
}