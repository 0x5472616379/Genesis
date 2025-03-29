using Genesis.Cache;
using Genesis.Entities;
using Genesis.Model;

namespace Genesis.Container;

public class RSContainer
{
    protected readonly int _maxSize;
    public List<ItemSlot> _slots;

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

    public bool TryAdd(int itemId, int quantity)
    {
        var result = AddItem(itemId, quantity);
        return result.Added == quantity;
    }

    public virtual bool ContainsAt(int index, int itemId, int minQuantity = 1)
    {
        // Validate index bounds first
        if (index < 0 || index >= _slots.Count)
            return false;

        ItemSlot slot = _slots[index];

        /* Check empty state and ID match */
        if (slot.IsEmpty || slot.ItemId != itemId)
            return false;

        /* Verify minimum quantity requirement */
        return slot.Quantity >= minQuantity;
    }

    protected virtual bool IsValidIndex(int index)
    {
        return index >= 0 && index < _slots.Count;
    }

    public (int Added, int Index) AddItem(int itemId, int quantity)
    {
        if (quantity <= 0) return (0, -1);

        var itemDef = ItemDefinition.Lookup(itemId);
        bool forceStack = AlwaysStack;
        bool naturalStack = itemDef?.Stackable == true || itemDef?.IsNote() == true;

        return (forceStack || naturalStack)
            ? AddStackableItem(itemId, quantity)
            : AddNonStackableItem(itemId, quantity);
    }

    private (int Added, int Index) AddStackableItem(int itemId, int quantity)
    {
        int remaining = quantity;
        int destIndex = -1;

        /* Add to existing stacks first */
        foreach (var slot in _slots)
        {
            if (slot.ItemId == itemId && slot.Quantity < int.MaxValue)
            {
                int available = int.MaxValue - slot.Quantity;
                int add = Math.Min(remaining, available);

                if (destIndex == -1) destIndex = _slots.IndexOf(slot);

                slot.Quantity += add;
                remaining -= add;
                // if (remaining == 0) break;
                return (quantity - remaining, destIndex);
            }
        }

        /* Add to new slots if needed */
        while (remaining > 0 && FreeSlots > 0)
        {
            var emptySlot = _slots.First(s => s.IsEmpty);
            int add = Math.Min(remaining, int.MaxValue);

            if (destIndex == -1) destIndex = _slots.IndexOf(emptySlot);

            emptySlot.ItemId = itemId;
            emptySlot.Quantity = add;
            remaining -= add;
        }

        return (quantity - remaining, destIndex);
    }

    private (int Added, int Index) AddNonStackableItem(int itemId, int quantity)
    {
        int destIndex = -1;
        int added = 0;

        foreach (var slot in _slots.Where(s => s.IsEmpty).Take(quantity))
        {
            slot.ItemId = itemId;
            slot.Quantity = 1;

            if (destIndex == -1) destIndex = _slots.IndexOf(slot);

            added++;
        }

        return (added, destIndex);
    }


    public (bool Success, int Index) TryPickupItem(int itemId, int quantity)
    {
        if (quantity <= 0)
            return (false, -1);

        var itemDef = ItemDefinition.Lookup(itemId);
        bool isStackable = AlwaysStack || (itemDef?.Stackable == true) || (itemDef?.IsNote() == true);

        return isStackable
            ? TryPickupStackableItem(itemId, quantity)
            : TryPickupNonStackableItem(itemId, quantity);
    }

    private (bool Success, int Index) TryPickupStackableItem(int itemId, int quantity)
    {
        // Check if we can add to existing stack
        var existingSlot = FindExistingStack(itemId);
        if (existingSlot != null)
        {
            int slotIndex = _slots.IndexOf(existingSlot);
            long potentialTotal = (long)existingSlot.Quantity + quantity;
            if (potentialTotal > int.MaxValue)
            {
                // Would exceed max stack size - don't add anything
                return (false, -1);
            }

            // We can safely add to existing stack
            existingSlot.Quantity += quantity;
            return (true, slotIndex);
        }

        // No existing stack - check if we have space for a new stack
        if (FreeSlots == 0)
        {
            return (false, -1);
        }

        // We have space - add new stack
        var emptySlot = _slots.First(s => s.IsEmpty);
        int newSlotIndex = _slots.IndexOf(emptySlot);
        emptySlot.ItemId = itemId;
        emptySlot.Quantity = quantity;
        return (true, newSlotIndex);
    }

    private (bool Success, int Index) TryPickupNonStackableItem(int itemId, int quantity)
    {
        // For non-stackable items, quantity represents number of items to add
        if (FreeSlots < quantity)
        {
            return (false, -1);
        }

        // We have enough space - add all items
        int firstAddedIndex = -1;
        foreach (var slot in _slots.Where(s => s.IsEmpty).Take(quantity))
        {
            int currentIndex = _slots.IndexOf(slot);
            if (firstAddedIndex == -1)
            {
                firstAddedIndex = currentIndex;
            }

            slot.ItemId = itemId;
            slot.Quantity = 1;
        }

        return (true, firstAddedIndex);
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
            : 1; /* Non-stackables always remove 1 */

        slot.Quantity -= removeAmount;

        if (slot.Quantity <= 0)
            slot.Clear();

        return removeAmount;
    }

    public virtual int RemoveItem(int itemId, int quantity)
    {
        if (quantity <= 0) return 0;

        int remaining = quantity;
        var itemDef = ItemDefinition.Lookup(itemId);
        bool isStackable = AlwaysStack || (itemDef?.Stackable == true) || (itemDef?.IsNote() == true);

        if (isStackable)
        {
            /* Remove from all stacks until quantity is met */
            foreach (var slot in _slots.Where(s => s.ItemId == itemId).ToList())
            {
                if (remaining <= 0) break;

                int removeAmount = Math.Min(remaining, slot.Quantity);
                slot.Quantity -= removeAmount;
                remaining -= removeAmount;

                if (slot.Quantity <= 0) slot.Clear();
            }
        }
        else
        {
            /* Remove individual items until quantity is met */
            int removed = 0;
            foreach (var slot in _slots.Where(s => s.ItemId == itemId).ToList())
            {
                if (removed >= quantity) break;

                slot.Clear();
                removed++;
            }

            remaining -= removed;
        }

        return quantity - remaining;
    }

    public void Swap(int index1, int index2)
    {
        if (index1 < 0 || index1 >= _slots.Count || index2 < 0 || index2 >= _slots.Count)
            throw new ArgumentOutOfRangeException();

        (_slots[index1], _slots[index2]) = (_slots[index2], _slots[index1]);
    }

    public ItemSlot GetItemAtIndex(int index) => _slots[index];
    public ItemSlot GetFirstItem(int itemId) => _slots.FirstOrDefault(x => x.ItemId == itemId);

    public int GetItemCount(int itemId)
    {
        return _slots.Where(s => s.ItemId == itemId)
            .Sum(s => AlwaysStack || ItemDefinition.Lookup(itemId)?.Stackable == true
                ? s.Quantity
                : 1);
    }

    public ItemSlot FindExistingStack(int itemId)
    {
        return _slots.FirstOrDefault(s => s.ItemId == itemId && s.Quantity < int.MaxValue);
    }

    public bool TryRemoveAt(int index, int quantity)
    {
        int removed = RemoveAt(index, quantity);
        return removed == quantity;
    }

    public int GetStackableAddable(int itemId)
    {
        /* Find existing stack for this item */
        var existingSlot = _slots.FirstOrDefault(s => s.ItemId == itemId);

        if (existingSlot != null)
        {
            /* Can only add to existing stack */
            return int.MaxValue - existingSlot.Quantity;
        }
        else
        {
            /* Need at least 1 free slot to create new stack */
            return FreeSlots > 0 ? int.MaxValue : 0;
        }
    }

    public void RefreshContainer(Player player, int interfaceId)
    {
        player.Session.PacketBuilder.RefreshContainer(_slots, interfaceId, _maxSize);
    }

    public void RefreshSlot(Player player, int toIndex, int itemId, int amount, int interfaceId)
    {
        player.Session.PacketBuilder.UpdateSlot(toIndex, itemId, amount, interfaceId);
    }

    public int GetIndexOfItemId(int itemId)
    {
        return _slots.FindIndex(s => s.ItemId == itemId);
    }
}