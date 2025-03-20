using ArcticRS.Appearance;
using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Managers;
using Genesis.Model;

namespace Genesis.Container;

public class EquipmentContainer : RSContainer
{
    public EquipmentContainer() : base(14)
    {
    }

    // public override int AddItem(int itemId, int quantity)
    // {
    //     // Equipment needs special handling - use TryEquip instead
    //     throw new InvalidOperationException("Use TryEquip method for equipment");
    // }
    

    public bool TryEquip(Player player, int itemId, int amount, int fromIndex)
    {
        var itemDef = ItemDefinition.Lookup(itemId);
        if (string.IsNullOrEmpty(itemDef.Name)) return false;

        var slot = EquipmentManager.GetEquipmentSlotById(itemId);
        int index = MapSlotToIndex(slot);
        if (index == -1) return false;

        if (slot == EquipmentSlot.Weapon)
        {
            player.AnimationManager.SetAnimations(string.Empty, itemId);
        }
        
        // if (itemDef.Name.ToLower().Contains("2h"))
        // {
        //     var shieldSlot = EquipmentSlot.Shield;
        //     if (!TryUnequip(player, shieldSlot))
        //         return false;
        // }

        var existingItem = _slots[MapSlotToIndex(slot)];
        var id = existingItem.ItemId;
        var am = existingItem.Quantity;
        if (!existingItem.IsEmpty)
        {
            // if (!TryUnequip(player, slot))
            //     return false;
            ClearSlot(slot);
            player.Inventory.OverrideAtIndex(fromIndex, id, am);
        }
        else
        {
            player.Inventory.RemoveAt(fromIndex);
        }

        _slots[index] = new ItemSlot { ItemId = itemId, Quantity = amount };
        
        player.Session.PacketBuilder.UpdateSlot((int)slot, itemId, amount, GameInterfaces.EquipmentContainer);
        player.Flags |= PlayerUpdateFlags.Appearance;

        return true;
    }

    public void ClearSlot(EquipmentSlot slot)
    {
        int index = MapSlotToIndex(slot);
        if (IsValidIndex(index)) _slots[index] = new ItemSlot();
    }

    public bool TryUnequip(Player player, EquipmentSlot slot)
    {
        int index = MapSlotToIndex(slot);
        if (index == -1 || _slots[index].IsEmpty)
            return false;

        // Get the item being unequipped
        var item = _slots[index];

        // Try to add to inventory
        if (player.Inventory.AddItem(item.ItemId, item.Quantity).Added > 0)
        {
            RemoveAt(MapSlotToIndex(slot));

            // Update client
            // player.Session.SendEquipmentUpdate(slot);
            // player.UpdateAppearance();
            // player.UpdateBonuses();

            // Handle unequip effects
            // var def = ItemDefinition.Lookup(item.ItemId);
            // def?.UnequipEffect?.Activate(player);

            player.Session.PacketBuilder.UpdateSlot((int)slot, _slots[index].ItemId, _slots[index].Quantity,
                GameInterfaces.EquipmentContainer);
            
            player.Flags |= PlayerUpdateFlags.Appearance;
            return true;
        }

        player.Session.PacketBuilder.SendMessage("Not enough space in your inventory!");
        return false;
    }

    private int MapSlotToIndex(EquipmentSlot slot) => slot switch
    {
        EquipmentSlot.Helmet => 0,
        EquipmentSlot.Cape => 1,
        EquipmentSlot.Amulet => 2,
        EquipmentSlot.Weapon => 3,
        EquipmentSlot.Chest => 4,
        EquipmentSlot.Shield => 5,
        EquipmentSlot.Legs => 7,
        EquipmentSlot.Gloves => 9,
        EquipmentSlot.Boots => 10,
        EquipmentSlot.Ring => 12,
        EquipmentSlot.Ammo => 13,
        _ => -1
    };

    public EquipmentItem GetItem(EquipmentSlot slot)
    {
        int index = MapSlotToIndex(slot);
        if (index == -1 || _slots[index].IsEmpty)
            return new EquipmentItem(-1, 0);

        var itemSlot = _slots[index];

        return new EquipmentItem(itemSlot.ItemId, itemSlot.Quantity);
    }

    private bool IsValidForSlot(ItemDefinition def, EquipmentSlot slot)
    {
        // Implement your equipment validation logic
        //def.EquipmentSlots.Contains(slot);
        return true;
    }
}