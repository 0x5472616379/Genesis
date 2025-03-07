using System.Globalization;
using ArcticRS.Constants;
using Genesis.Entities;

namespace Genesis.Commands;

public class PlayGfxCommand : RSCommand
{
    protected override PlayerRights RequiredRights { get; } = PlayerRights.ADMIN;

    private readonly Player _player;
    private int _id;

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

        if (!int.TryParse(Args[1], out _id))
        {
            Player.Session.PacketBuilder.SendMessage("Invalid gfx ID! Try ::gfx 1");
            return false;
        }

        return true;
    }

    public override void Invoke()
    {
        _player.SetCurrentGfx(_id);
        _player.SetCurrentAnimation(791);
    }
}