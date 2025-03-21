using ArcticRS.Appearance;
using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Model;

namespace Genesis.Container;

public class EquipmentContainer : RSContainer
{
    public EquipmentContainer(int maxSize) : base(maxSize)
    {
        _slots = new List<ItemSlot>(Enumerable.Repeat(new ItemSlot(), maxSize));
    }

    public bool TryEquipItem(Player player, int inventoryIndex, EquipmentSlot slot)
    {
        var inventory = player.Inventory;

        /* 1. Get item reference and validate */
        var itemToEquip = inventory.GetItemAtIndex(inventoryIndex).Copy();
        if (itemToEquip.IsEmpty) return false;

        /* 2. Remove the item from inventory FIRST */
        if (!inventory.TryRemoveAt(inventoryIndex, itemToEquip.Quantity))
            return false;

        /* 3. Now handle conflicts with the now-empty slot available */
        if (!ResolveWeaponShieldConflicts(player, slot, itemToEquip.ItemId, inventoryIndex))
        {
            /* Rollback: Put item back in original slot */
            inventory.TryAddToSpecificIndex(inventoryIndex, itemToEquip.ItemId, itemToEquip.Quantity);
            return false;
        }

        /* 4. Handle stackables or perform swap */
        var targetSlot = GetItemInSlot(slot);
        if (!targetSlot.IsEmpty && targetSlot.ItemId == itemToEquip.ItemId && IsStackable(itemToEquip.ItemId))
        {
            return MergeStackable(player, inventoryIndex, slot, targetSlot, itemToEquip);
        }
        else
        {
            return PerformEquipmentSwap(player, slot, itemToEquip, inventoryIndex);
        }
    }

    private bool ResolveWeaponShieldConflicts(Player player, EquipmentSlot slot, int newItemId, int inventoryIndex)
    {
        if (slot == EquipmentSlot.Weapon && IsTwoHanded(newItemId))
        {
            /* List to track items we've unequipped for rollback */
            var unequippedItems = new List<(EquipmentSlot slot, ItemSlot item)>();

            /* 1. Unequip existing weapon (1h or current 2h) */
            var weaponItem = GetItemInSlot(EquipmentSlot.Weapon);
            if (!weaponItem.IsEmpty)
            {
                /* Try to unequip to original slot + 1 first, then fallback */
                bool weaponMoved = TryUnequipToInventory(player, EquipmentSlot.Weapon, inventoryIndex ) ||
                                   TryUnequipToInventory(player, EquipmentSlot.Weapon, -1); // -1 = any slot

                if (!weaponMoved)
                {
                    /* Rollback shield unequip if weapon failed */
                    foreach (var item in unequippedItems)
                    {
                        OverrideAtIndex(MapSlotToIndex(item.slot), item.item.ItemId, item.item.Quantity);
                        player.Inventory.RemoveItem(item.item.ItemId, item.item.Quantity);
                    }

                    return false;
                }
            }
            
            /* 2. Unequip shield first */
            var shieldItem = GetItemInSlot(EquipmentSlot.Shield);
            if (!shieldItem.IsEmpty)
            {
                if (!TryUnequipToInventory(player, EquipmentSlot.Shield, inventoryIndex))
                {
                    return false; /* Abort if shield can't be moved */
                }

                unequippedItems.Add((EquipmentSlot.Shield, shieldItem.Copy()));
            }

            return true;
        }
        /* Existing shield handling remains unchanged */
        else if (slot == EquipmentSlot.Shield)
        {
            var weaponSlot = GetItemInSlot(EquipmentSlot.Weapon);
            if (!weaponSlot.IsEmpty && IsTwoHanded(weaponSlot.ItemId) &&
                !TryUnequipToInventory(player, EquipmentSlot.Weapon, inventoryIndex))
            {
                return false;
            }

            return true;
        }

        return true;
    }


    private bool MergeStackable(Player player, int inventoryIndex, EquipmentSlot slot, ItemSlot targetSlot,
        ItemSlot newItem)
    {
        int maxAdd = int.MaxValue - targetSlot.Quantity;
        int toAdd = Math.Min(newItem.Quantity, maxAdd);

        if (toAdd <= 0) return false;

        targetSlot.Quantity += toAdd;
        RefreshSlot(player, (int)slot);

        /* Handle leftover quantity */
        if (toAdd < newItem.Quantity)
        {
            int remaining = newItem.Quantity - toAdd;
            player.Inventory.TryAddToSpecificIndex(inventoryIndex, newItem.ItemId, remaining);
        }

        return true;
    }

    private bool PerformEquipmentSwap(Player player, EquipmentSlot slot, ItemSlot newItem, int inventoryIndex)
    {
        var oldItem = GetItemInSlot(slot).Copy();

        /* Try to add old item to the original inventory slot */
        if (!oldItem.IsEmpty)
        {
            var added = player.Inventory.AddItemToSpecificIndex(inventoryIndex, oldItem.ItemId, oldItem.Quantity);
            if (added < oldItem.Quantity)
            {
                /* Rollback entire operation */
                player.Inventory.TryAddToSpecificIndex(inventoryIndex, newItem.ItemId, newItem.Quantity);
                return false;
            }
        }

        /* Equip the new item */
        OverrideAtIndex(MapSlotToIndex(slot), newItem.ItemId, newItem.Quantity);
        RefreshSlot(player, (int)slot);

        /* Update animations if weapon */
        if (slot == EquipmentSlot.Weapon)
        {
            player.AnimationManager.SetAnimations(string.Empty, newItem.ItemId);
        }

        return true;
    }

    private bool TryUnequipToInventory(Player player, EquipmentSlot slot, int preferredIndex)
    {
        var item = GetItemInSlot(slot);
        if (item.IsEmpty) return true;

        /* 1. Try preferred index first */
        if (preferredIndex >= 0 && preferredIndex < player.Inventory._slots.Count)
        {
            int added = player.Inventory.AddItemToSpecificIndex(preferredIndex, item.ItemId, item.Quantity);
            if (added == item.Quantity)
            {
                ClearSlot(slot);
                RefreshSlot(player, (int)slot);
                return true;
            }
        }

        /* 2. Fallback to normal inventory addition */
        var result = player.Inventory.AddItem(item.ItemId, item.Quantity);
        if (result.Added == item.Quantity)
        {
            ClearSlot(slot);
            RefreshSlot(player, (int)slot);
            return true;
        }

        /* 3. Partial add cleanup */
        if (result.Added > 0)
        {
            player.Inventory.RemoveItem(item.ItemId, result.Added);
        }

        return false;
    }

    public bool TryUnequip(Player player, EquipmentSlot slot)
    {
        var item = GetItemInSlot(slot);
        if (item.IsEmpty) return true;

        var added = player.Inventory.AddItem(item.ItemId, item.Quantity);
        if (added.Added == item.Quantity)
        {
            ClearSlot(slot);
            RefreshSlot(player, (int)slot);
            return true;
        }

        return false;
    }

    private void ClearSlot(EquipmentSlot slot)
    {
        var index = MapSlotToIndex(slot);
        if (index >= 0 && index < _slots.Count)
        {
            _slots[index] = new ItemSlot();
        }
    }

    private bool IsTwoHanded(int itemId)
    {
        var itemDef = ItemDefinition.Lookup(itemId);
        string[] keywords =
        {
            "2h", "bow", "maul", "claw", "halberd", "salamander", "spear",
            "greataxe", "hammers", "ket-om", "flail"
        };
        return keywords.Any(k => itemDef.Name.Contains(k, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsStackable(int itemId)
    {
        var def = ItemDefinition.Lookup(itemId);
        return def?.Stackable == true || def?.IsNote() == true;
    }

    public void RefreshSlot(Player player, int slotIndex)
    {
        /* Convert client slot index to container index */
        var containerIndex = MapClientSlotToContainerIndex(slotIndex);

        /* Validate container index */
        if (containerIndex < 0 || containerIndex >= _slots.Count)
            return;

        var item = GetItemAtIndex(containerIndex);
        player.Session.PacketBuilder.UpdateSlot(
            slotIndex, /* Send original client slot index back */
            item.ItemId,
            item.Quantity,
            GameInterfaces.EquipmentContainer
        );
    }

    public ItemSlot GetItemInSlot(EquipmentSlot slot)
    {
        var index = MapSlotToIndex(slot);
        return index >= 0 && index < _slots.Count ? _slots[index] : null;
    }

    private int MapSlotToIndex(EquipmentSlot slot) => slot switch
    {
        EquipmentSlot.Helmet => 0,
        EquipmentSlot.Cape => 1,
        EquipmentSlot.Amulet => 2,
        EquipmentSlot.Weapon => 3,
        EquipmentSlot.Chest => 4,
        EquipmentSlot.Shield => 5,
        EquipmentSlot.Legs => 6,
        EquipmentSlot.Gloves => 7,
        EquipmentSlot.Boots => 8,
        EquipmentSlot.Ring => 9,
        EquipmentSlot.Ammo => 10,
        _ => -1
    };

    private int MapClientSlotToContainerIndex(int clientSlotIndex)
    {
        return clientSlotIndex switch
        {
            0 => 0, // Helmet
            1 => 1, // Cape
            2 => 2, // Amulet
            3 => 3, // Weapon
            4 => 4, // Chest
            5 => 5, // Shield
            7 => 6, // Legs
            9 => 7, // Gloves
            10 => 8, // Boots
            12 => 9, // Ring
            13 => 10, // Ammo
            _ => -1 // Invalid
        };
    }
}