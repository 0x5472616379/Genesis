using Genesis.Entities;
using Genesis.Interactions;

namespace Genesis.Packets.Incoming;

public class PickupItemPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _x;
    private readonly int _objId;
    private readonly int _y;
    private readonly int _z;

    public PickupItemPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
        _y = _player.Session.Reader.ReadUnsignedWordBigEndian();
        _objId = _player.Session.Reader.ReadUnsignedWord();
        _x = _player.Session.Reader.ReadUnsignedWordBigEndian();
        _z = _player.Location.Z;
        _player.Session.PacketBuilder.SendMessage($"Interacting with object. {_objId} {_x} {_y} {_z}");
    }
    
    public void Process()
    {
        _player.CurrentInteraction = new PickupInteraction(_x, _y, _z, _objId, _player);
    }
}