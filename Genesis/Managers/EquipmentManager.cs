using ArcticRS.Appearance;
using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Model;

namespace Genesis.Managers;

public class EquipmentItem
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public bool IsEmpty => Id == -1 || Amount <= 0;

    public EquipmentItem(int id, int amount)
    {
        Id = id;
        Amount = amount;
    }
}

public class EquipmentManager
{
    private readonly Player _player;

    public EquipmentManager(Player player)
    {
        _player = player;
        Equipment = new Dictionary<EquipmentSlot, EquipmentItem>
        {
            { EquipmentSlot.Helmet, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Cape, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Amulet, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Weapon, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Chest, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Shield, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Legs, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Gloves, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Boots, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Ring, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Ammo, new EquipmentItem(-1, 0) }
        };
    }

    public Dictionary<EquipmentSlot, EquipmentItem> Equipment { get; set; } = new();

    public bool TryEquipItem(int inventoryIndex)
    {
        var inventory = _player.Inventory;
        var item = inventory.GetItemAtIndex(inventoryIndex);

        // 1. Validate item exists
        if (item.IsEmpty || item.ItemId == -1)
            return false;

        // 2. Get target slot
        var slot = GetEquipmentSlotById(item.ItemId);
        if (slot == EquipmentSlot.None)
            return false;

        // 3. Handle two-handed/shield conflicts
        HandleWeaponShieldConflicts(slot, item.ItemId);

        // 4. Get current equipped item
        var currentEquipped = GetItem(slot);

        // 5. Handle stackables
        if (currentEquipped != null && currentEquipped.Id == item.ItemId && IsStackable(item.ItemId))
        {
            return MergeStack(item, inventoryIndex, slot);
        }

        // 6. Perform swap
        return SwapItems(inventoryIndex, slot, item);
    }

    private void HandleWeaponShieldConflicts(EquipmentSlot slot, int newItemId)
    {
        // If equipping 2h weapon
        if (slot == EquipmentSlot.Weapon && IsTwoHanded(newItemId))
        {
            var shield = GetItem(EquipmentSlot.Shield);
            if (!shield.IsEmpty)
                UnequipToInventory(EquipmentSlot.Shield);
        }
        // If equipping shield
        else if (slot == EquipmentSlot.Shield)
        {
            var weapon = GetItem(EquipmentSlot.Weapon);
            if (!weapon.IsEmpty && IsTwoHanded(weapon.Id))
                UnequipToInventory(EquipmentSlot.Weapon);
        }
    }

    private bool MergeStack(ItemSlot item, int inventoryIndex, EquipmentSlot slot)
    {
        var current = GetItem(slot);
        int maxAdd = int.MaxValue - current.Amount;
        int toAdd = Math.Min(item.Quantity, maxAdd);

        if (toAdd <= 0) return false;

        // Remove from inventory
        int removed = _player.Inventory.RemoveAt(inventoryIndex, toAdd);
        if (removed == 0) return false;

        // Add to equipment
        current.Amount += removed;
        RefreshSlot(slot);
        return true;
    }

    private bool SwapItems(int inventoryIndex, EquipmentSlot slot, ItemSlot newItem)
    {
        var oldItem = GetItem(slot);

        // 1. Remove new item from inventory
        if (!_player.Inventory.TryRemoveAt(inventoryIndex, newItem.Quantity))
            return false;

        // 2. Add old item to inventory
        if (!oldItem.IsEmpty)
        {
            var added = _player.Inventory.AddItem(oldItem.Id, oldItem.Amount);
            if (added.Added != oldItem.Amount)
            {
                // Rollback: Put new item back
                _player.Inventory.TryAdd(newItem.ItemId, newItem.Quantity);
                return false;
            }
        }

        // 3. Equip new item
        EquipItem(slot, newItem.ItemId, newItem.Quantity);
        RefreshSlot(slot);
        return true;
    }

    private void UnequipToInventory(EquipmentSlot slot)
    {
        var item = GetItem(slot);
        if (item.IsEmpty) return;

        if (_player.Inventory.AddItem(item.Id, item.Amount).Added == item.Amount)
        {
            Unequip(slot);
            RefreshSlot(slot);
        }
    }

    public void EquipItem(EquipmentSlot slot, int itemId, int quantity)
    {
        if (Equipment.ContainsKey(slot))
        {
            Equipment[slot] = new EquipmentItem(itemId, quantity);
        
            // Update weapon animations if equipping weapon
            if (slot == EquipmentSlot.Weapon)
            {
                _player.AnimationManager.SetAnimations(string.Empty, itemId);
            }
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
        return keywords.Any(keyword => itemDef.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsStackable(int itemId)
    {
        var def = ItemDefinition.Lookup(itemId);
        return def?.Stackable == true || def?.IsNote() == true;
    }

    private void RefreshSlot(EquipmentSlot slot)
    {
        // var item = GetItem(slot);
        // _player.Session.PacketBuilder.SendEquipmentUpdate(
        //     (int)slot,
        //     item.Id,
        //     item.Amount
        // );
    }

    public void Unequip(EquipmentSlot slot)
    {
        if (Equipment.ContainsKey(slot))
            Equipment[slot] = new EquipmentItem(-1, 0);
    }

    public EquipmentItem GetItem(EquipmentSlot slot)
    {
        if (Equipment.ContainsKey(slot))
            return Equipment[slot];

        return null;
    }

    public bool HasBowEquipped()
    {
        if (GetItem(EquipmentSlot.Weapon) == null)
            return false;

        return Array.Exists(GameConstants.BOWS, i => i == GetItem(EquipmentSlot.Weapon).Id);
    }

    public static EquipmentSlot GetEquipmentSlotById(int Id)
    {
        if (GameConstants.IsItemInArray(Id, GameConstants.BOWS) ||
            GameConstants.IsItemInArray(Id, GameConstants.OTHER_RANGE_WEAPONS))
            return EquipmentSlot.Weapon;
        if (GameConstants.IsItemInArray(Id, GameConstants.ARROWS))
            return EquipmentSlot.Ammo;
        if (GameConstants.IsItemInArray(Id, GameConstants.capes))
            return EquipmentSlot.Cape;
        if (GameConstants.IsItemInArray(Id, GameConstants.boots))
            return EquipmentSlot.Boots;
        if (GameConstants.IsItemInArray(Id, GameConstants.gloves))
            return EquipmentSlot.Gloves;
        if (GameConstants.IsItemInArray(Id, GameConstants.shields))
            return EquipmentSlot.Shield;
        if (GameConstants.IsItemInArray(Id, GameConstants.hats))
            return EquipmentSlot.Helmet;
        if (GameConstants.IsItemInArray(Id, GameConstants.amulets))
            return EquipmentSlot.Amulet;
        if (GameConstants.IsItemInArray(Id, GameConstants.rings))
            return EquipmentSlot.Ring;
        if (GameConstants.IsItemInArray(Id, GameConstants.body))
            return EquipmentSlot.Chest;
        if (GameConstants.IsItemInArray(Id, GameConstants.legs))
            return EquipmentSlot.Legs;
        
        // Default to Weapon if no match
        return EquipmentSlot.Weapon;
    }

    public void Clear()
    {
        Equipment = new Dictionary<EquipmentSlot, EquipmentItem>
        {
            { EquipmentSlot.Helmet, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Cape, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Amulet, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Weapon, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Chest, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Shield, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Legs, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Gloves, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Boots, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Ring, new EquipmentItem(-1, 0) },
            { EquipmentSlot.Ammo, new EquipmentItem(-1, 0) }
        };
    }

    public int GetWeapon() =>
        Equipment.TryGetValue(EquipmentSlot.Weapon, out var equippedWeapon) ? equippedWeapon.Id : -1;

    private int MapEquipmentSlotToContainerIndex(EquipmentSlot slot)
    {
        return slot switch
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
    }

    public void Equip(RSItem rsItem)
    {
        var slot = GetEquipmentSlotById(rsItem.Id);

        if (slot == EquipmentSlot.Weapon)
        {
            _player.AnimationManager.SetAnimations(string.Empty, rsItem.Id);
        }

        if (Equipment.ContainsKey(slot))
            Unequip(slot);

        Equipment[slot] = new EquipmentItem(rsItem.Id, rsItem.Amount);
    }
}