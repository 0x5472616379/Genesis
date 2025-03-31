using System.Globalization;
using ArcticRS.Constants;
using Genesis.Entities.Player;
using Genesis.Model;

namespace Genesis.Commands;

public class PlayGfxCommand : RSCommand
{
    protected override PlayerRights RequiredRights { get; } = PlayerRights.ADMIN;

    private readonly Player _player;
    private short _id;

    public PlayGfxCommand(Player player, string[] args) : base(player, args)
    {
        _player = player;
    }

    public override bool Validate()
    {
        if (Args.Length < 1)
        {
            Player.Session.PacketBuilder.SendMessage("Invalid syntax! Try ::gfx 1");
            return false;
        }

        if (!short.TryParse(Args[1], out _id))
        {
            Player.Session.PacketBuilder.SendMessage("Invalid gfx ID! Try ::gfx 1");
            return false;
        }

        return true;
    }

    public override void Invoke()
    {
        _player.SetCurrentGfx(new Gfx(_id, 150, 0));
        // _player.SetCurrentAnimation(199);
    }
}