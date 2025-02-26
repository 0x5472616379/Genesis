using Genesis.Cache;
using Genesis.Entities;
using Genesis.Interactions;
using Genesis.Movement;

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

        /* Opening and closing a door requires being orthogonally adjacent (N, E, S, W) */

        if (_objId == 1531)
        {
            _player.Session.PacketBuilder.SendMessage($"Door Interaction: {_x} Y: {_y} Z: {_z} ObjId: {_objId}");
            _player.Session.PacketBuilder.SendMessage(
                $"Distance: {MovementHelper.EuclideanDistance(_player.Location.X, _player.Location.Y, _x, _y)}");
            // _player.Session.PacketBuilder.SendMessage($"Cardinal Adjacent: {IsCardinalAdjacent(_player.Location.X, _player.Location.Y, _x, _y)}");


            _player.CurrentInterraction = new SingleDoorInteraction(3,
                () => { _player.Session.PacketBuilder.SendMessage("Door Interaction"); }, _x, _y, _z, _player);
        }
    }
}