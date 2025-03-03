using Genesis.Cache;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Interactions;
using Genesis.Movement;
using Genesis.Skills.Woodcutting;

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
        var worldObject = GetWorldObject();
        if (worldObject == null) return;
        
        if (HandleTreeInteraction(worldObject)) return;
    }
    
    private WorldObject? GetWorldObject()
    {
        var worldObject = Region.GetObject(_objId, _x, _y, _z);
        if (worldObject == null)
        {
            _player.Session.PacketBuilder.SendMessage("Object does not exist.");
        }
        return worldObject;
    }
    
    private bool HandleTreeInteraction(WorldObject worldObject)
    {
        var tree = TreeData.GetTree(_objId);
        if (tree == null) return false;
        
        _player.SetFaceX(worldObject.X * 2 + worldObject.GetSize()[0]);
        _player.SetFaceY(worldObject.Y * 2 + worldObject.GetSize()[1]);
        
        _player.CurrentInterraction = new TreeInteraction(_player, worldObject, tree);
        return true;
    }
}