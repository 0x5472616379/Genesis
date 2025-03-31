using ArcticRS.Commands;
using ArcticRS.Constants;
using Genesis.Entities.Player;

namespace Genesis.Commands;

public class NullCommand : RSCommand
{
    protected override PlayerRights RequiredRights => PlayerRights.NORMAL;
    
    private readonly string[] _args;
    public NullCommand(Player player, string[] args) : base(player, args)
    {
        _args = args;
    }

    public override bool Validate()
    {
        return true;
    }

    public override void Invoke()
    {
        Player.Session.PacketBuilder.SendMessage($"Command {_args[0]} not recognized.");
    }
}