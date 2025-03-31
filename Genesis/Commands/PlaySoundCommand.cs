using System.Globalization;
using ArcticRS.Commands;
using ArcticRS.Constants;
using Genesis.Entities.Player;

namespace Genesis.Commands;

public class PlaySoundCommand : RSCommand
{
    private readonly Player _player;
    private int _id;

    public PlaySoundCommand(Player player, string[] args) : base(player, args)
    {
        _player = player;
    }

    protected override PlayerRights RequiredRights { get; }

    public override bool Validate()
    {
        if (Args.Length < 1)
        {
            Player.Session.PacketBuilder.SendMessage("Invalid syntax! Try ::play 1");
            return false;
        }

        if (!int.TryParse(Args[1], out _id))
        {
            Player.Session.PacketBuilder.SendMessage("Invalid sound ID! Try ::play 1");
            return false;
        }

        return true;
    }

    public override void Invoke()
    {
        Player.Session.PacketBuilder.SendSound(_id, 0, 0);
    }
}