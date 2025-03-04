using Genesis.Entities;
using Genesis.Environment;

namespace Genesis.Packets.Incoming;

public class AttackPlayerPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _index;

    public AttackPlayerPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
        _index = _player.Session.Reader.ReadSignedWordBigEndian();
    }

    public void Process()
    {
        // _player.Session.PacketBuilder.SendMessage($"Trying to attack player with index: {_index}");
        // _player.Session.PacketBuilder.SendMessage($"Player server index: {_index - 1}");
        // _player.Session.PacketBuilder.SendMessage($"Player name: {World.GetPlayers()[_index - 1].Session.Username}");
        
        // _player.CurrentInterraction = new TreeInteraction(_player, worldObject, tree);
        _player.Following = World.GetPlayers()[_index - 1];
    }
}