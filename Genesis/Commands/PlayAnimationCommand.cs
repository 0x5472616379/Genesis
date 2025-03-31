using ArcticRS.Commands;
using ArcticRS.Constants;
using Genesis.Entities.Player;

namespace Genesis.Commands;

public class PlayAnimationCommand : RSCommand
{
    private int _id;
    public PlayAnimationCommand(Player player, string[] args) : base(player, args)
    {
    }

    protected override PlayerRights RequiredRights { get; }

    public override bool Validate()
    {
        if (Args.Length < 1)
        {
            Player.Session.PacketBuilder.SendMessage("Invalid syntax! Try ::anim 1");
            return false;
        }

        if (!int.TryParse(Args[1], out _id))
        {
            Player.Session.PacketBuilder.SendMessage("Invalid animation ID! Try ::anim 1");
            return false;
        }

        return true;
    }

    public override void Invoke()
    {
        Player.SetCurrentAnimation(_id);
    }
}