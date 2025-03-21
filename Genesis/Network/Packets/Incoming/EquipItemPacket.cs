﻿using ArcticRS.Appearance;
using Genesis.Cache;
using Genesis.Configuration;
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
    }

    public void Process()
    {
        if (_interfaceId == GameInterfaces.DefaultInventoryContainer)
        {
            var item = _player.Inventory.GetItemAtIndex(_fromIndex);
            if (item.IsEmpty) return;

            var slot = EquipmentManager.GetEquipmentSlotById(item.ItemId);
            if (slot == EquipmentSlot.None) return;

            if (_player.Equipment.TryEquipItem(_player, _fromIndex, slot))
            {
                if (slot == EquipmentSlot.Weapon)
                {
                    WeaponInterfaceManager.Refresh(_player);
                    // WeaponSpeedLookup.GetWeaponInfo(ItemDefinition.Lookup(_itemId).Name);
                }

                _player.Inventory.RefreshContainer(_player, GameInterfaces.DefaultInventoryContainer);
                _player.Flags |= PlayerUpdateFlags.Appearance;
            }
        }
    }
}