using ArcticRS.Actions;
using ArcticRS.Appearance;
using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Definitions;
using Genesis.Entities;
using Genesis.Managers;
using Genesis.Skills.Combat;

namespace Genesis.Packets.Incoming;

public class EquipItemPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _itemId;
    private readonly int _fromIndex;
    private readonly int _interfaceId;


    public EquipItemPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;

        _itemId = _player.Session.Reader.ReadSignedWord();
        _fromIndex = _player.Session.Reader.ReadSignedWordA();
        _interfaceId = _player.Session.Reader.ReadSignedWordA();
        _player.CurrentInteraction = null;
        ResetInteraction();
    }

    public void Process()
    {
        if (_interfaceId == GameInterfaces.DefaultInventoryContainer)
        {
            _player.ActionHandler.AddAction(new EquipItemAction(_player, _fromIndex));
        }
    }

    public void ResetInteraction()
    {
        _player.CurrentInteraction = null;
        _player.SetFacingEntity(null);
    }
}