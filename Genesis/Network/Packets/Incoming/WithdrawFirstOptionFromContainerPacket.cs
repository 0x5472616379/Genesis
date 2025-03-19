﻿using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Managers;
using Genesis.Model;

namespace Genesis.Packets.Incoming;

public class WithdrawFirstOptionFromContainerPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _fromContainer;
    private readonly int _fromIndex;
    private int _itemId;
    private readonly int _amount;

    public WithdrawFirstOptionFromContainerPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;

        _fromContainer = _player.Session.Reader.ReadUnsignedWordA();
        _fromIndex = _player.Session.Reader.ReadUnsignedWordA();
        _itemId = _player.Session.Reader.ReadUnsignedWordA();

        Console.WriteLine($"From Container: {_fromContainer}");
        Console.WriteLine($"FromIndex: {_fromIndex}");
        Console.WriteLine($"ItemId: {_itemId}");
    }

    public void Process()
    {
        var item = ItemDefinition.Lookup(_itemId);

        /* Bank Inventory Container */
        if (_fromContainer == GameInterfaces.BankInventoryContainer)
        {
            // InventorySystem.Transfer(_player.InventoryItemContainer, _player.BankItemContainer, _itemId, 1);
            // _player.InventoryItemContainer.CopyToContainer(_player.BankInventoryItemContainer);
            //
            // _player.BankItemContainer.Refresh(_player, GameInterfaces.DefaultBankContainer);
            // _player.BankInventoryItemContainer.Refresh(_player, GameInterfaces.BankInventoryContainer);
            // _player.InventoryItemContainer.Refresh(_player, GameInterfaces.DefaultInventoryContainer);

        }

        if (_fromContainer == GameInterfaces.DefaultBankContainer)
        {
            // var transfer =  InventorySystem.Transfer(_player.BankItemContainer, _player.InventoryItemContainer, _itemId, 1);
            // if (transfer <= 0)
            // {
            //     _player.Session.PacketBuilder.SendMessage("You don't have enough free inventory space to do that.");
            //     return;
            // }
            // _player.InventoryItemContainer.CopyToContainer(_player.BankInventoryItemContainer);
            //
            // _player.BankItemContainer.Refresh(_player, GameInterfaces.DefaultBankContainer);
            // _player.BankInventoryItemContainer.Refresh(_player, GameInterfaces.BankInventoryContainer);
            // _player.InventoryItemContainer.Refresh(_player, GameInterfaces.DefaultInventoryContainer);
        }
        
        if (_fromContainer == GameInterfaces.EquipmentContainer)
        {
            var slot = EquipmentManager.GetEquipmentSlotById(_itemId);
            if (_player.Equipment.TryUnequip(_player, slot))
            {
                _player.Equipment.Refresh(_player, GameInterfaces.EquipmentContainer);
                _player.Inventory.Refresh(_player, GameInterfaces.DefaultInventoryContainer);
            }
            
            // var equipped = _player.Equipment.TryEquip(_itemId);
            // if (equipped)
            // {
            //     _player.Inventory.RemoveAt(_fromIndex);
            // }
            // var transfer =  InventorySystem.Transfer(_player.BankItemContainer, _player.InventoryItemContainer, _itemId, 1);
            // if (transfer <= 0)
            // {
            //     _player.Session.PacketBuilder.SendMessage("You don't have enough free inventory space to do that.");
            //     return;
            // }
            // _player.InventoryItemContainer.CopyToContainer(_player.InventoryItemContainer);
            //
            // // _player.BankItemContainer.Refresh(_player, GameInterfaces.DefaultBankContainer);
            // // _player.BankInventoryItemContainer.Refresh(_player, GameInterfaces.BankInventoryContainer);
            
        }
        
        if (_fromContainer == GameInterfaces.DefaultShopWindowContainer)
        {
            var itemDef = ItemDefinition.Lookup(_itemId);
            if (itemDef == null) return;
            
            _player.Session.PacketBuilder.SendMessage($"{itemDef.Name}: currently costs 1gp.");
        }
        
        if (_fromContainer == GameInterfaces.DefaultShopInventoryContainer)
        {
            var itemDef = ItemDefinition.Lookup(_itemId);
            _player.Session.PacketBuilder.SendMessage($"{itemDef.Name}: shop will buy for 1gp.");
        }
    }
}