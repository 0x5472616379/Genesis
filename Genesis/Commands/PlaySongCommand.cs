using ArcticRS.Commands;
using Genesis.Entities;

namespace Genesis.Commands;

public class PlaySongCommand : CommandBase
{
    private readonly Player _player;
    private int _id;

    public PlaySongCommand(Player player, string[] args) : base(player, args)
    {
        _player = player;
    }

    protected override string ValidateArgs()
    {
        if (Args.Length < 1)
            return "Invalid syntax! Try ::song 1";

        if (!int.TryParse(Args[1], out _id))
            return "Invalid song ID! Try ::song 1";

        return null;
    }

    protected override void Invoke()
    {
        Player.Session.PacketBuilder.SendSong(_id);
    }
}