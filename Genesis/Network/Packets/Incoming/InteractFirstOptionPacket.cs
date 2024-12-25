using Genesis.Entities;
using Genesis.Interactions;

namespace Genesis.Packets.Incoming;

public class InteractFirstOptionPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _x;
    private readonly int _objId;
    private readonly int _y;
    private readonly int _z;

    public InteractFirstOptionPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
        _x = _player.Session.Reader.ReadUnsignedWordBigEndianA();
        _objId = _player.Session.Reader.ReadUnsignedWord();
        _y = _player.Session.Reader.ReadSignedWordA();
        _z = _player.Location.Z;
    }

    public void Process()
    {
        _player.Session.PacketBuilder.SendMessage($"Interact First Option: {_objId}");
        if (_objId == 1530)
        {
            _player.CurrentInterraction = new RSInteraction(5, () => { _player.Session.PacketBuilder.SendMessage("Hello, World!"); });
        }
        
    }
}