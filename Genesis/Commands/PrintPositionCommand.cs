using ArcticRS.Constants;
using Genesis.Commands;
using Genesis.Entities;
using Genesis.Environment;

namespace ArcticRS.Commands;

public class PrintPositionCommand : RSCommand
{
    private readonly bool _debug;
    protected override PlayerRights RequiredRights => PlayerRights.NORMAL;

    public PrintPositionCommand(Player player, string[] args) : base(player, args)
    {
        _debug = args.Length > 1 && args[1].ToLower() == "debug";
    }

    public override bool Validate()
    {
        return true;
    }

    public override void Invoke()
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

        for (int idx = 0; idx < World.GetPlayers().Length; idx++)
        {
            var player = World.GetPlayers()[idx];
            if (player == null) continue;
            if (player == Player)
            {
                Player.Session.PacketBuilder.SendMessage($"My Index: {idx}");
                break;
            }
        }
    }
}