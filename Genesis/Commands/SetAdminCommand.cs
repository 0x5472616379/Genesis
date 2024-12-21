using ArcticRS.Constants;
using Genesis.Entities;

namespace ArcticRS.Commands;

public class SetAdminCommand : CommandBase
{
    public SetAdminCommand(Player player, string[] args) : base(player, args)
    {
    }

    protected override string ValidateArgs()
    {
        return null;
    }

    protected override void Invoke()
    {
        Player.Attributes.Rights = PlayerRights.ADMIN;
        Player.Session.PacketBuilder.SendMessage("You're now an admin!");
    }
}