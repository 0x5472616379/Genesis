using ArcticRS.Appearance;
using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Container;
using Genesis.Definitions;
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
        // var item = ItemDefinition.Lookup(_itemId);

        if (_fromContainer == GameInterfaces.BankInventoryContainer)
        {
            var inventorySlot = _player.Inventory.GetFirstItem(_itemId);
            if (inventorySlot == null) return;

            var index = _player.Inventory.GetIndexOfItemId(inventorySlot.ItemId);

            var transferResult = ContainerTransfer.Transfer(
                _player.Inventory,
                _player.BankContainer,
                _itemId,
                1
            );

            if (transferResult.Amount <= 0) return;

            /* Handle bank slot refresh */
            var updatedBankSlot = _player.BankContainer.GetItemAtIndex(transferResult.DestinationIndex);
            int displayBankItemId = updatedBankSlot.ItemId == 0 ? -1 : updatedBankSlot.ItemId;

            _player.BankContainer.RefreshSlot(
                _player,
                transferResult.DestinationIndex,
                displayBankItemId,
                updatedBankSlot.Quantity,
                GameInterfaces.DefaultBankContainer
            );


            /* Handle inventory slot refresh */
            var updatedInventorySlot = _player.Inventory.GetItemAtIndex(index);
            int displayInventoryItemId = updatedInventorySlot.ItemId == 0 ? -1 : updatedInventorySlot.ItemId;


            var refreshInterfaces = new[]
            {
                GameInterfaces.BankInventoryContainer,
                GameInterfaces.DefaultInventoryContainer
            };

            foreach (var interfaceId in refreshInterfaces)
            {
                _player.Inventory.RefreshSlot(
                    _player,
                    index,
                    displayInventoryItemId,
                    updatedInventorySlot.Quantity,
                    interfaceId
                );
            }
        }

        if (_fromContainer == GameInterfaces.DefaultBankContainer)
        {
            var bankItem = _player.BankContainer.GetItemAtIndex(_fromIndex);
            if (bankItem == null) return;

            var transferResult = ContainerTransfer.Transfer(
                _player.BankContainer,
                _player.Inventory,
                _itemId,
                1
            );

            if (transferResult.Amount > 0 && transferResult.DestinationIndex != -1)
            {
                var inventorySlot = _player.Inventory.GetItemAtIndex(transferResult.DestinationIndex);

                /* Refresh inventory interfaces */
                foreach (var interfaceId in new[]
                         {
                             GameInterfaces.BankInventoryContainer,
                             GameInterfaces.DefaultInventoryContainer
                         })
                {
                    _player.Inventory.RefreshSlot(
                        _player,
                        transferResult.DestinationIndex,
                        inventorySlot.ItemId,
                        inventorySlot.Quantity,
                        interfaceId
                    );
                }

                // Refresh bank slot with proper display rules
                var updatedBankSlot = _player.BankContainer.GetItemAtIndex(_fromIndex);
                int displayItemId = updatedBankSlot.ItemId == 0 ? -1 : updatedBankSlot.ItemId;

                _player.BankContainer.RefreshSlot(
                    _player,
                    _fromIndex,
                    displayItemId, // Now properly handles 0 -> -1 conversion
                    updatedBankSlot.Quantity,
                    GameInterfaces.DefaultBankContainer
                );
            }
        }

        if (_fromContainer == GameInterfaces.EquipmentContainer)
        {
            int containerIndex = _player.Equipment.MapClientSlotToContainerIndex(_fromIndex);
            if (containerIndex == -1) return;

            var equipmentSlot = _player.Equipment.GetItemAtIndex(containerIndex);
            if (equipmentSlot.IsEmpty) return;

            int amountToRemove = equipmentSlot.Quantity; // Or read from packet if partial unequip
            var addResult = _player.Inventory.AddItem(equipmentSlot.ItemId, amountToRemove);

            if (addResult.Added > 0)
            {
                // Update equipment slot
                equipmentSlot.Quantity -= addResult.Added;
                bool needsClear = equipmentSlot.Quantity <= 0;

                if (needsClear)
                {
                    _player.Equipment.ClearSlot(EquipmentManager.GetEquipmentSlotById(_itemId));
                }
                else
                {
                    _player.Equipment.OverrideAtIndex(
                        containerIndex,
                        equipmentSlot.ItemId,
                        equipmentSlot.Quantity
                    );
                }

                // Refresh equipment slot
                _player.Equipment.RefreshSlot(
                    _player,
                    _fromIndex,
                    needsClear ? -1 : equipmentSlot.ItemId,
                    needsClear ? 0 : equipmentSlot.Quantity,
                    GameInterfaces.EquipmentContainer
                );

                // Refresh SPECIFIC inventory slot that received the items
                if (addResult.Index != -1)
                {
                    var inventoryItem = _player.Inventory.GetItemAtIndex(addResult.Index);
                    _player.Inventory.RefreshSlot(
                        _player,
                        addResult.Index,
                        inventoryItem.ItemId,
                        inventoryItem.Quantity,
                        GameInterfaces.DefaultInventoryContainer
                    );
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
                
                _player.Flags |= PlayerUpdateFlags.Appearance;
            }
        }
    }
}