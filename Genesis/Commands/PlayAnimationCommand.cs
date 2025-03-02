using ArcticRS.Commands;
using Genesis.Entities;

namespace Genesis.Commands;

public class PlayAnimationCommand : CommandBase
{
    private int _id;
    public PlayAnimationCommand(Player player, string[] args) : base(player, args)
    {
    }

    protected override string ValidateArgs()
    {
        if (Args.Length < 1)
            return "Invalid syntax! Try ::anim 1";

        if (!int.TryParse(Args[1], out _id))
            return "Invalid song ID! Try ::anim 1";

        return null;
    }

    protected override void Invoke()
    {
        Player.SetCurrentAnimation(_id);
    }
}