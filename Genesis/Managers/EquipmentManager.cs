using ArcticRS.Appearance;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Model;

namespace Genesis.Managers;

public class EquipmentItem
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public EquipmentSlot Slot { get; set; }

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
            { EquipmentSlot.Helmet, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Cape, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Amulet, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Weapon, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Chest, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Shield, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Legs, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Gloves, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Boots, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Ring, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Ammo, new EquipmentItem (-1, 0) }
        };
    }

    public Dictionary<EquipmentSlot, EquipmentItem> Equipment { get; set; } = new();

    public void Equip(RSItem rsItem)
    {
         var slot = GetEquipmentSlotById(rsItem.Id);
        
         if (Equipment.ContainsKey(slot))
             Unequip(slot);
        
         Equipment[slot] = new EquipmentItem(rsItem.Id, rsItem.Amount);
        
         // _player.InventoryManager.Remove(rsItem.Index, rsItem.Amount);
    }

    public void Unequip(EquipmentSlot slot)
    {
        if (Equipment.ContainsKey(slot))
            Equipment[slot] = new EquipmentItem(-1 ,0);
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


    public EquipmentSlot GetEquipmentSlotById(int Id)
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
            { EquipmentSlot.Helmet, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Cape, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Amulet, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Weapon, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Chest, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Shield, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Legs, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Gloves, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Boots, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Ring, new EquipmentItem (-1, 0) },
            { EquipmentSlot.Ammo, new EquipmentItem (-1, 0) }
        };
    }

    public int GetWeapon() => Equipment.TryGetValue(EquipmentSlot.Weapon, out var equippedWeapon) ? equippedWeapon.Id : -1;
    
     public void Refresh()
     {
         foreach (var entry in Equipment)
         {
             var equipmentSlot = entry.Key;
             var item = entry.Value;
             var Id = item.Id;
             var amount = item.Amount;
             _player.Session.PacketBuilder.UpdateSlot((int)equipmentSlot, Id, amount, GameInterfaces.EquipmentContainer);
         }
    
         _player.Flags |= PlayerUpdateFlags.Appearance;
     }
     
}