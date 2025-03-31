using ArcticRS.Constants;
using Genesis.Commands;
using Genesis.Entities.Player;

namespace ArcticRS.Commands;

public class SetAdminCommand : RSCommand
{
    protected override PlayerRights RequiredRights { get; } = PlayerRights.NORMAL;
    public SetAdminCommand(Player player, string[] args) : base(player, args)
    {
    }

    public override bool Validate()
    {
        return true;
    }

    public override void Invoke()
    {
        Player.Attributes.Rights = PlayerRights.ADMIN;
        Player.Session.PacketBuilder.SendMessage("You're now an admin!");
    }
}