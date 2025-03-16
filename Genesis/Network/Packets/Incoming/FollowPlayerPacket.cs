using Genesis.Entities;
using Genesis.Environment;

namespace Genesis.Packets.Incoming;

public class FollowPlayerPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _index;

    public FollowPlayerPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
        _index = _player.Session.Reader.ReadSignedWordBigEndian();
    }
    
    public void Process()
    {
        var player =  World.GetPlayers().Where(x => x.Session.Index == _index).FirstOrDefault();
        if (player == null) return;

        // _player.Following = player;
        // _player.Session.PacketBuilder.SendMessage($"Following player with index: {_index}");
        // _player.SetFacingEntity(_player.Following);
    }
}