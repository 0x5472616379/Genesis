using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Entities;

namespace Genesis.Managers;

public class InventoryManager
{
    private readonly Player _player;

    public const int MAX_SLOTS = 28;
    private List<Item> InventoryItems = new(new Item[MAX_SLOTS]);

    public InventoryManager(Player player)
    {
        _player = player;
    }

    public int AddItem(int itemId, int amount = 1)
    {
        var itemDefinition = ItemDefinition.Lookup(itemId);
        if (itemDefinition.Id == -1)
        {
            _player.Session.PacketBuilder.SendMessage("Item not found!");
            return 0;
        }

        amount = Math.Min(amount, int.MaxValue);
        int addedAmount = 0;

        if (itemDefinition.IsStackable())
        {
            var existingItemIndex = GetItemIndex(itemId);
            if (existingItemIndex == -1)
            {
                if (NotifyInventoryFull()) return 0;

                if (AddToEmptySlot(new Item(itemId, amount, stackable: true)))
                {
                    addedAmount = amount;
                }
                else
                {
                    _player.Session.PacketBuilder.SendMessage("Failed to add item to inventory!");
                }
            }
            else
            {
                var existingItem = GetItemAtIndex(existingItemIndex);
                var totalAmount = existingItem.Amount + amount;

                var newAmount = Math.Min(totalAmount, int.MaxValue);
                addedAmount = newAmount - existingItem.Amount;
                existingItem.Amount = newAmount;
            }
        }
        else
        {
            if (NotifyInventoryFull()) return 0;

            int freeSlots = MAX_SLOTS - GetItemCount();
            int itemsToAdd = Math.Min(amount, freeSlots);

            if (itemsToAdd <= 0)
            {
                _player.Session.PacketBuilder.SendMessage("Not enough space in inventory for that many items!");
                return 0;
            }

            for (int i = 0; i < itemsToAdd; i++)
            {
                if (AddToEmptySlot(new Item(itemId, amount: 1, stackable: false)))
                {
                    addedAmount++;
                }
                else
                {
                    _player.Session.PacketBuilder.SendMessage("Failed to add some items to inventory.");
                    break;
                }
            }
        }

        return addedAmount;
    }

    private bool NotifyInventoryFull()
    {
        if (GetItemCount() >= MAX_SLOTS)
        {
            _player.Session.PacketBuilder.SendMessage("Inventory is full!");
            return true;
        }

        return false;
    }

    private bool AddToEmptySlot(Item newItem)
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (InventoryItems[i] == null)
            {
                InventoryItems[i] = newItem;
                return true;
            }
        }

        return false;
    }

    public void Remove(int index, int amount = 1)
    {
        if (index < 0 || index >= InventoryItems.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        var item = InventoryItems[index];
        if (item is null)
            throw new InvalidOperationException("No item exists at the specified index.");

        if (item.Amount > amount)
            item.Amount -= amount;
        else
            InventoryItems[index] = null;
    }
    
    public void Clear()
    {
        InventoryItems = new List<Item>(new Item[MAX_SLOTS]);
    }

    public int GetItemIndex(int itemId) => InventoryItems.FindIndex(i => i != null && i.Id == itemId);
    public Item GetItemAtIndex(int index) => InventoryItems[index];

    public void RefreshInventory()
    {
        _player.Session.PacketBuilder.RefreshContainer(InventoryItems, GameInterfaces.DefaultInventoryContainer,
            GetItemCount());
    }

    public int GetItemCount() => InventoryItems.Count(i => i != null);
    public List<Item> GetItems() => InventoryItems.Where(x => x != null).ToList();
}

public class Item
{
    public int Id { get; }
    public int Amount { get; set; }
    public bool Stackable { get; }

    public Item(int id, int amount = 1, bool stackable = false)
    {
        Id = id;
        Amount = amount;
        Stackable = stackable;
    }
}