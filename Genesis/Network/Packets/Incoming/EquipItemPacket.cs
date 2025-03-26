﻿using ArcticRS.Appearance;
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

    private readonly Dictionary<int, int> _weaponSpecialBars = new()
    {
        { 4151, GameInterfaces.WhipDefaultSpecialBar },
        { 861, GameInterfaces.MsbDefaultSpecialBar },
        { 859, GameInterfaces.MsbDefaultSpecialBar },
        { 1215, GameInterfaces.DragonDaggerDefaultSpecialBar },
        { 1231, GameInterfaces.DragonDaggerDefaultSpecialBar },
        { 5680, GameInterfaces.DragonDaggerDefaultSpecialBar },
        { 5698, GameInterfaces.DragonDaggerDefaultSpecialBar },
        { 4587, GameInterfaces.DragonScimitarDefaultSpecialBar }
    };


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
                    _player.CombatHelper.SpecialAttack = null;
                    if (_weaponSpecialBars.TryGetValue(_itemId, out int specialBarInterface))
                    {
                        _player.Session.PacketBuilder.DisplayHiddenInterface(0, specialBarInterface);
                        _player.CombatHelper.UpdateSpecialAttack(specialBarInterface);
                    }
                }

                _player.BonusManager.Reset();

                foreach (var itemslot in _player.Equipment._slots)
                {
                    if (itemslot.ItemId == -1)
                        continue;

                    var itemBonuses = ItemParser.GetBonusesById(itemslot.ItemId).Bonuses;
                    _player.BonusManager.CalculateBonuses(itemBonuses);
                }

                _player.BonusManager.UpdateBonus();

                _player.Inventory.RefreshContainer(_player, GameInterfaces.DefaultInventoryContainer);
                _player.Flags |= PlayerUpdateFlags.Appearance;
            }
        }
    }
}