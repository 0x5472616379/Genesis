using Genesis.Cache;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Interactions;
using Genesis.Skills.Runecrafting;
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
        _player.Session.PacketBuilder.SendMessage($"Interacting with object. {_objId} {_x} {_y} {_z}");
    }

    /* Invoked from World.cs.ProcessPackets(); */
    public void Process()
    {
        var worldObject = GetWorldObject();
        if (worldObject == null) return;

        HandleMiningInteraction(worldObject);
        HandleBankInteraction(worldObject);
    }

    private bool HandleMiningInteraction(WorldObject worldObject)
    {
        if (worldObject.Id == 2093 || worldObject.Id == 2092 || worldObject.Id == 2091 || worldObject.Id == 2090)
        {
            _player.SetFaceX(_x * 2 + worldObject.GetSize()[0]);
            _player.SetFaceY(_y * 2 + worldObject.GetSize()[1]);
            _player.CurrentInteraction = new MiningInteraction(_player, worldObject);
            return true;
        }

        return false;
    }

    private bool HandleBankInteraction(WorldObject worldObject)
    {
        if (worldObject.Id != 2213)
        {
            return false;
        }

        _player.CurrentInteraction = new BankInteraction(_player, worldObject);
        return true;
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

    private bool HandleRunecraftingInteraction(WorldObject worldObject)
    {
        var altar = RunecraftingAltarData.GetAltar(worldObject.Id);
        if (altar == null)
            return false;

        _player.CurrentInteraction = new RunecraftingInteraction(_player, worldObject);
        return true;
    }


    private bool HandleTreeInteraction(WorldObject worldObject)
    {
        var tree = TreeData.GetTree(_objId);
        if (tree == null) return false;

        _player.SetFaceX(worldObject.X * 2 + worldObject.GetSize()[0]);
        _player.SetFaceY(worldObject.Y * 2 + worldObject.GetSize()[1]);

        _player.CurrentInteraction = new TreeInteraction(_player, worldObject, tree);
        return true;
    }
}