using Genesis.Cache;
using Genesis.Entities;

namespace Genesis.Model;

public class ItemSlot
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }

    public bool IsEmpty => ItemId == 0 || Quantity <= 0;
}

public class Container
{
    private readonly int _maxSize;
    private List<ItemSlot> _slots;
    public bool IsBank { get; }

    public Container(int maxSize, bool isBank)
    {
        _maxSize = maxSize;
        IsBank = isBank;

        _slots = new List<ItemSlot>();
        for (int i = 0; i < _maxSize; i++)
        {
            _slots.Add(new ItemSlot());
        }
    }

    public int FreeSlots => _slots.Count(s => s.IsEmpty);

    public ItemSlot GetItem(int index)
    {
        if (index < 0 || index >= _slots.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        return _slots[index];
    }

    public void Swap(int index1, int index2)
    {
        if (index1 < 0 || index1 >= _slots.Count || index2 < 0 || index2 >= _slots.Count)
            throw new ArgumentOutOfRangeException();

        (_slots[index1], _slots[index2]) = (_slots[index2], _slots[index1]);
    }

    public int AddItem(int itemId, int quantity)
    {
        if (quantity <= 0)
            return 0;

        int remaining = quantity;

        bool isStackable = IsBank || (ItemDefinition.Lookup(itemId).Stackable);

        if (isStackable)
        {
            // Try to add to existing slots first
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i].ItemId == itemId && _slots[i].Quantity < int.MaxValue)
                {
                    int availableSpace = int.MaxValue - _slots[i].Quantity;
                    int addAmount = Math.Min(remaining, availableSpace);
                    _slots[i] = new ItemSlot { ItemId = itemId, Quantity = _slots[i].Quantity + addAmount };
                    remaining -= addAmount;
                    if (remaining == 0)
                        break;
                }
            }

            // Add to empty slots if remaining
            while (remaining > 0 && FreeSlots > 0)
            {
                int emptyIndex = _slots.FindIndex(s => s.IsEmpty);
                if (emptyIndex == -1)
                    break;

                int addAmount = Math.Min(remaining, int.MaxValue);
                _slots[emptyIndex] = new ItemSlot { ItemId = itemId, Quantity = addAmount };
                remaining -= addAmount;
            }
        }
        else
        {
            // Non-stackable: each item takes a slot
            int possibleToAdd = Math.Min(remaining, FreeSlots);
            int added = 0;

            for (int i = 0; i < _slots.Count && added < possibleToAdd; i++)
            {
                if (_slots[i].IsEmpty)
                {
                    _slots[i] = new ItemSlot { ItemId = itemId, Quantity = 1 };
                    added++;
                }
            }

            remaining -= added;
        }

        return quantity - remaining;
    }

    public int RemoveItem(int itemId, int quantity)
    {
        if (quantity <= 0)
            return 0;

        int remaining = quantity;

        bool isStackable = IsBank || ((ItemDefinition.Lookup(itemId).Stackable));

        if (isStackable)
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i].ItemId == itemId)
                {
                    if (_slots[i].Quantity <= remaining)
                    {
                        remaining -= _slots[i].Quantity;
                        _slots[i] = new ItemSlot(); // Clear slot
                    }
                    else
                    {
                        _slots[i] = new ItemSlot { ItemId = itemId, Quantity = _slots[i].Quantity - remaining };
                        remaining = 0;
                    }

                    if (remaining == 0)
                        break;
                }
            }
        }
        else
        {
            // Non-stackable: remove individual slots
            int removed = 0;
            for (int i = 0; i < _slots.Count && removed < quantity; i++)
            {
                if (_slots[i].ItemId == itemId)
                {
                    _slots[i] = new ItemSlot();
                    removed++;
                }
            }

            remaining -= removed;
        }

        return quantity - remaining;
    }

    public int GetItemCount(int itemId)
    {
        bool isStackable = IsBank || (ItemDefinition.Lookup(itemId).Stackable);
        if (isStackable)
        {
            return _slots.Where(s => s.ItemId == itemId).Sum(s => s.Quantity);
        }
        else
        {
            return _slots.Count(s => s.ItemId == itemId);
        }
    }

    public int GetAddableQuantity(int itemId, int desiredAmount)
    {
        if (IsBank)
        {
            var existingSlot = _slots.FirstOrDefault(s => s.ItemId == itemId);
            if (existingSlot != null && !existingSlot.IsEmpty)
            {
                int availableSpace = int.MaxValue - existingSlot.Quantity;
                return Math.Min(desiredAmount, availableSpace);
            }
            else
            {
                return FreeSlots > 0 ? Math.Min(desiredAmount, int.MaxValue) : 0;
            }
        }
        else
        {
            bool isStackable = (ItemDefinition.Lookup(itemId).Stackable);
            if (isStackable)
            {
                var existingSlot = _slots.FirstOrDefault(s => s.ItemId == itemId);
                if (existingSlot != null && !existingSlot.IsEmpty)
                {
                    int availableSpace = int.MaxValue - existingSlot.Quantity;
                    return Math.Min(desiredAmount, availableSpace);
                }
                else
                {
                    return FreeSlots >= 1 ? Math.Min(desiredAmount, int.MaxValue) : 0;
                }
            }
            else
            {
                return Math.Min(desiredAmount, FreeSlots);
            }
        }
    }

    public void CopyToContainer(Container targetContainer)
    {
        if (targetContainer == null)
            throw new ArgumentNullException(nameof(targetContainer), "Target container cannot be null.");

        targetContainer.Clear();

        for (int i = 0; i < _maxSize; i++)
        {
            var item = _slots[i];
            targetContainer._slots[i] = item == null
                ? null
                : new ItemSlot
                {
                    ItemId = item.ItemId,
                    Quantity = item.Quantity
                };
        }
    }

    public void Clear()
    {
        _slots = new List<ItemSlot>();
        for (int i = 0; i < _maxSize; i++)
        {
            _slots.Add(new ItemSlot());
        }
    }

    public void Refresh(Player player, int interfaceId)
    {
        player.Session.PacketBuilder.RefreshContainer(_slots, interfaceId, _maxSize);
    }

    public int Count => _slots.Count;
    public List<ItemSlot> GetItems => _slots;
    public bool ContainsItemId(int id) => _slots.Any(s => s.ItemId == id);
    public int RemoveAllById(int id)
    {
        int count = 0;
        foreach (var slot in _slots.Where(s => s.ItemId == id))
        {
            slot.ItemId = 0;
            slot.Quantity = 0;
            count++;
        }
        return count;
    }
}

public class InventorySystem
{
    public static int Transfer(Container source, Container destination, int itemId, int amount)
    {
        int available = source.GetItemCount(itemId);
        if (available <= 0)
            return 0;

        int possibleFromSource = Math.Min(available, amount);
        int addableToDest = destination.GetAddableQuantity(itemId, possibleFromSource);

        int transferAmount = Math.Min(possibleFromSource, addableToDest);
        if (transferAmount <= 0)
            return 0;

        int removed = source.RemoveItem(itemId, transferAmount);
        int added = destination.AddItem(itemId, removed);

        if (added < removed)
        {
            // Attempt to return leftover to source if destination couldn't accept all
            source.AddItem(itemId, removed - added);
        }

        return added;
    }
}