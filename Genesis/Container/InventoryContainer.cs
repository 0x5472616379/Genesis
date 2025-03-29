using Genesis.Cache;
using Genesis.Model;

namespace Genesis.Container;

public class InventoryContainer : RSContainer
{
    public InventoryContainer(int maxSize) : base(maxSize)
    {
    }

    public void ClearSlot(int index)
    {
        if (IsValidIndex(index))
        {
            _slots[index] = new ItemSlot() { ItemId = -1, Quantity = 0 }; // Resets to ItemId=0, Quantity=0
        }
    }

    public void Clear()
    {
        for (int i = 0; i < _maxSize; i++)
        {
            ClearSlot(i);
        }
    }
    
    public int AddItemToSpecificIndex(int index, int itemId, int quantity)
    {
        if (!IsValidIndex(index) || quantity <= 0) return 0;
    
        var slot = _slots[index];
        var itemDef = ItemDefinition.Lookup(itemId);
        bool isStackable = itemDef?.Stackable == true || itemDef?.IsNote() == true;

        // Case 1: Slot is empty
        if (slot.IsEmpty)
        {
            int addAmount = isStackable ? Math.Min(quantity, int.MaxValue) : 1;
            _slots[index] = new ItemSlot { ItemId = itemId, Quantity = addAmount };
            return addAmount;
        }

        // Case 2: Slot has same stackable item
        if (isStackable && slot.ItemId == itemId)
        {
            int maxAdd = int.MaxValue - slot.Quantity;
            int addAmount = Math.Min(quantity, maxAdd);
            slot.Quantity += addAmount;
            return addAmount;
        }

        // Case 3: Slot contains different item - can't add here
        return 0;
    }

    public bool TryAddToSpecificIndex(int index, int itemId, int quantity)
    {
        int added = AddItemToSpecificIndex(index, itemId, quantity);
        return added == quantity;
    }
    
    public List<ItemSlot> GetAllItems()
    {
        return _slots.Where(x => x.ItemId != 0 && x.ItemId != -1).ToList();
    }
    
}