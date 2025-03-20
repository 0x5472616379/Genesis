using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Container;
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

        if (_fromContainer == GameInterfaces.BankInventoryContainer)
        {
            var inventorySlot = _player.Inventory.GetItemAtIndex(_fromIndex);
            if (inventorySlot == null) return;

            int transferred = ContainerTransfer.Transfer(_player.Inventory, _player.BankContainer, _itemId, 1);
            if (transferred <= 0) return;

            /* Refresh bank slot */
            int bankIndex = _player.BankContainer.GetIndexOfItemId(_itemId);
            var bankSlot = _player.BankContainer.GetItemAtIndex(bankIndex);
            _player.BankContainer.RefreshSlot(
                _player, 
                bankIndex, 
                bankSlot.ItemId, 
                bankSlot.Quantity, 
                GameInterfaces.DefaultBankContainer
            );

            /* Determine display values */
            int displayItemId = inventorySlot.ItemId == 0 ? -1 : inventorySlot.ItemId;
            int slotIndex = _fromIndex;
            int quantity = inventorySlot.Quantity;

            /* Refresh inventory interfaces */
            var refreshInterfaces = new[] { 
                GameInterfaces.BankInventoryContainer, 
                GameInterfaces.DefaultInventoryContainer 
            };

            foreach (var interfaceId in refreshInterfaces)
            {
                _player.Inventory.RefreshSlot(
                    _player, 
                    slotIndex, 
                    displayItemId, 
                    quantity, 
                    interfaceId
                );
            }
        }

        if (_fromContainer == GameInterfaces.DefaultBankContainer)
        {
            var bankSlot = _player.BankContainer.GetItemAtIndex(_fromIndex);
            if (bankSlot == null) return;

            int transferred = ContainerTransfer.Transfer(_player.BankContainer, _player.Inventory, _itemId, 1);
            if (transferred <= 0) return;

            /* Refresh bank slot */
            int updatedBankQty = _player.BankContainer.GetItemAtIndex(_fromIndex).Quantity;
            int displayBankItemId = updatedBankQty > 0 ? _itemId : -1;
            _player.BankContainer.RefreshSlot(
                _player,
                _fromIndex,
                displayBankItemId,
                updatedBankQty,
                GameInterfaces.DefaultBankContainer
            );

            /* Refresh inventory interfaces */
            int inventoryIndex = _player.Inventory.GetIndexOfItemId(_itemId);
            // if (inventoryIndex == -1) inventoryIndex = _player.Inventory.FirstFreeSlot();

            var inventorySlot = _player.Inventory.GetItemAtIndex(inventoryIndex);
            var refreshInterfaces = new[] {
                GameInterfaces.BankInventoryContainer,
                GameInterfaces.DefaultInventoryContainer
            };

            foreach (var interfaceId in refreshInterfaces)
            {
                _player.Inventory.RefreshSlot(
                    _player,
                    inventoryIndex,
                    inventorySlot.ItemId,
                    inventorySlot.Quantity,
                    interfaceId
                );
            }
        }

        if (_fromContainer == GameInterfaces.EquipmentContainer)
        {
            var slot = EquipmentManager.GetEquipmentSlotById(_itemId);
            if (_player.Equipment.TryUnequip(_player, slot))
            {
                _player.Equipment.RefreshContainer(_player, GameInterfaces.EquipmentContainer);
                _player.Inventory.RefreshContainer(_player, GameInterfaces.DefaultInventoryContainer);
            }
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