﻿using ArcticRS.Actions;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Managers;
using Genesis.Model;

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
    }

    public void Process()
    {
        if (!_player.Inventory.ContainsAt(_fromIndex, _itemId))
            return;
        
        var item = _player.Inventory.GetItemAtIndex(_fromIndex);
        
        if (_player.Equipment.TryEquip(_player, _itemId, item.Quantity, _fromIndex))
        {
            // _player.Inventory.RemoveAt(_fromIndex);
            _player.Inventory.RefreshContainer(_player, GameInterfaces.DefaultInventoryContainer);
            _player.Equipment.RefreshContainer(_player, GameInterfaces.EquipmentContainer);
        }
    }
}