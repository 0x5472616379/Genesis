using System.Globalization;
using ArcticRS.Commands;
using Genesis.Entities;

namespace Genesis.Commands;

public class PlaySoundCommand : CommandBase
{
    private readonly Player _player;
    private int _id;

    public PlaySoundCommand(Player player, string[] args) : base(player, args)
    {
        _player = player;
    }

    protected override string ValidateArgs()
    {
        if (Args.Length < 1)
            return "Invalid syntax! Try ::play 1";

        if (!int.TryParse(Args[1], out _id))
            return "Invalid sound ID! Try ::play 1";

        return null;
    }

    protected override void Invoke()
    {
        Player.Session.PacketBuilder.SendSound(_id, 0, 0);
    }
}