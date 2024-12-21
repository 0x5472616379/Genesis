using Genesis.Entities;

namespace Genesis.Model;

public class RSContainer
{
    private readonly Player _player;
    public int Id { get; private set; }
    public RSItem[] Items { get; private set; }
    public int Capacity { get; private set; }

    public RSContainer(Player player, int capacity)
    {
        _player = player;
        Capacity = capacity;
        Items = Enumerable.Range(0, capacity)
                          .Select(_ => new RSItem(-1, 0))
                          .ToArray();
    }             

    public bool AddItem(RSItem item)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].Id == -1) // Empty slot or default initialized slot
            {
                Items[i] = item;
                return true;
            }
        }

        // No space available
        return false;
    }

    public bool RemoveItem(int itemId, int amount)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            var item = Items[i];
            if (item != null && item.Id == itemId)
            {
                if (item.Amount > amount)
                {
                    item.ReduceAmount(amount); // Reduce only the specified amount
                }
                else
                {
                    Items[i] = null; // Remove the item if the amount is exhausted
                }
                return true;
            }
        }

        // Item not found
        return false;
    }

    public bool UpdateItem(int index, RSItem updatedItem)
    {
        if (index < 0 || index >= Items.Length) return false; // Validate index
        Items[index] = updatedItem; // Replace item at the index
        return true;
    }

    public RSItem? GetItem(int itemId)
    {
        return Items.FirstOrDefault(i => i.Id == itemId);
    }

    public void TransferItemTo(RSContainer targetRsContainer, int itemId, int amount)
    {
        var item = GetItem(itemId);
        if (item == null || item.Amount < amount)
            throw new InvalidOperationException("Not enough items to transfer.");

        RemoveItem(itemId, amount);

        var newItem = new RSItem(itemId, amount);
        if (!targetRsContainer.AddItem(newItem))
        {
            AddItem(newItem);
            throw new InvalidOperationException("Target container full or transfer failed.");
        }
    }

    public void Refresh(int containerId)
    {
        _player.Session.PacketBuilder.RefreshContainer(Items, containerId);
    }
}