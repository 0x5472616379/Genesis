using Genesis.Entities;
using Genesis.Environment;
using Genesis.Interactions;

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
        _player.Following = World.GetPlayers()[_index - 1];
        _player.InteractingEntity = World.GetPlayers()[_index - 1];
        _player.SetFacingEntity(_player.InteractingEntity);
        _player.CurrentInteraction = new PlayerAttackInteraction(_player, _player.InteractingEntity as Player);
    }
}