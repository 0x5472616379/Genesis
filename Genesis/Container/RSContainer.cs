using Genesis.Cache;
using Genesis.Entities;
using Genesis.Model;

namespace Genesis.Container;

public class RSContainer
{
    protected readonly int _maxSize;
    protected List<ItemSlot> _slots;
    
    public virtual bool AlwaysStack => false;
    public int FreeSlots => _slots.Count(s => s.IsEmpty);

    public RSContainer(int maxSize)
    {
        _maxSize = maxSize;
        InitializeSlots();
    }

    protected void InitializeSlots()
    {
        _slots = new List<ItemSlot>();
        for (int i = 0; i < _maxSize; i++)
            _slots.Add(new ItemSlot());
    }

    public void OverrideAtIndex(int index, int itemId, int quantity)
    {
        _slots[index] = new ItemSlot
        {
            ItemId = itemId,
            Quantity = quantity
        };
    }
    
    public virtual bool ContainsAt(int index, int itemId, int minQuantity = 1)
    {
        // Validate index bounds first
        if (index < 0 || index >= _slots.Count)
            return false;

        ItemSlot slot = _slots[index];
        
        // Check empty state and ID match
        if (slot.IsEmpty || slot.ItemId != itemId)
            return false;

        // Verify minimum quantity requirement
        return slot.Quantity >= minQuantity;
    }

    protected virtual bool IsValidIndex(int index)
    {
        return index >= 0 && index < _slots.Count;
    }
    
    public virtual int AddItem(int itemId, int quantity)
    {
        if (quantity <= 0) return 0;

        var itemDef = ItemDefinition.Lookup(itemId);
        bool forceStack = AlwaysStack;
        bool naturalStack = itemDef.Stackable || itemDef.IsNote();
        bool isStackable = forceStack || naturalStack;

        return isStackable 
            ? AddStackableItem(itemId, quantity, itemDef) 
            : AddNonStackableItem(itemId, quantity);
    }

    private int AddStackableItem(int itemId, int quantity, ItemDefinition def)
    {
        int remaining = quantity;

        // Try existing stacks
        foreach (var slot in _slots.Where(s => s.ItemId == itemId && s.Quantity < int.MaxValue))
        {
            int available = int.MaxValue - slot.Quantity;
            int add = Math.Min(remaining, available);
            slot.Quantity += add;
            remaining -= add;
            if (remaining == 0) break;
        }

        // Use empty slots if needed
        while (remaining > 0 && FreeSlots > 0)
        {
            var emptySlot = _slots.First(s => s.IsEmpty);
            int add = Math.Min(remaining, int.MaxValue);
            emptySlot.ItemId = itemId;
            emptySlot.Quantity = add;
            remaining -= add;
        }

        return quantity - remaining;
    }

    private int AddNonStackableItem(int itemId, int quantity)
    {
        int remaining = quantity;
        int emptySlots = FreeSlots;
        int canAdd = Math.Min(remaining, emptySlots);

        foreach (var slot in _slots.Where(s => s.IsEmpty).Take(canAdd))
        {
            slot.ItemId = itemId;
            slot.Quantity = 1;
            remaining--;
        }

        return quantity - remaining;
    }
    
    public virtual int RemoveAt(int index, int quantity = int.MaxValue)
    {
        if (index < 0 || index >= _slots.Count)
            return 0;

        ItemSlot slot = _slots[index];
        if (slot.IsEmpty || quantity <= 0)
            return 0;

        bool isStackable = AlwaysStack || 
                           (ItemDefinition.Lookup(slot.ItemId)?.Stackable ?? false) ||
                           (ItemDefinition.Lookup(slot.ItemId)?.IsNote() ?? false);

        int removeAmount = isStackable 
            ? Math.Min(quantity, slot.Quantity)
            : 1; // Non-stackables always remove 1

        slot.Quantity -= removeAmount;
        
        if (slot.Quantity <= 0)
            slot.Clear();

        return removeAmount;
    }

    public void Swap(int index1, int index2)
    {
        if (index1 < 0 || index1 >= _slots.Count || index2 < 0 || index2 >= _slots.Count)
            throw new ArgumentOutOfRangeException();

        (_slots[index1], _slots[index2]) = (_slots[index2], _slots[index1]);
    }

    public ItemSlot GetItemAtIndex(int index) => _slots[index];
    
    public void Refresh(Player player, int interfaceId)
    {
        player.Session.PacketBuilder.RefreshContainer(_slots, interfaceId, _maxSize);
    }
}