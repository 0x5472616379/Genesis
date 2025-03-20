using Genesis.Cache;

namespace Genesis.Container;

public struct TransferResult
{
    public int Amount;
    public int DestinationIndex;
}

public static class ContainerTransfer
{
    public static TransferResult Transfer(RSContainer source, RSContainer destination, 
        int itemId, int amount)
    {
        var result = new TransferResult();
    
        if (amount <= 0) return result;

        int destinationItemId = ShouldConvertToUnnoted(destination, itemId) 
            ? ConvertToUnnoted(itemId) 
            : itemId;

        int removable = Math.Min(source.GetItemCount(itemId), amount);
        int addable = CalculateAddable(destination, destinationItemId, removable);

        if (addable <= 0) return result;

        int removed = source.RemoveItem(itemId, addable);
        var (added, destIndex) = destination.AddItem(destinationItemId, removed);

        result.Amount = added;
        result.DestinationIndex = destIndex;

        if (removed > added)
            source.AddItem(itemId, removed - added);

        return result;
    }

    private static int CalculateAddable(RSContainer container, int itemId, int desired)
    {
        if (container.AlwaysStack) // Bank container
        {
            // Find existing slot for this item
            var existingSlot = container._slots.FirstOrDefault(s => s.ItemId == itemId);

            if (existingSlot != null)
            {
                // Can only add to existing stack
                int availableSpace = int.MaxValue - existingSlot.Quantity;
                return Math.Min(desired, availableSpace);
            }
            else
            {
                // Need at least 1 free slot to add new item
                if (container.FreeSlots == 0) return 0;

                // New slot can hold full stack
                return Math.Min(desired, int.MaxValue);
            }
        }
        else
        {
            // Regular container logic (inventory/etc)
            var def = ItemDefinition.Lookup(itemId);
            bool stackable = def?.Stackable == true || def?.IsNote() == true;

            return stackable
                ? Math.Min(desired, container.GetStackableAddable(itemId))
                : Math.Min(desired, container.FreeSlots);
        }
    }

    private static bool ShouldConvertToUnnoted(RSContainer destination, int itemId)
    {
        return destination is BankContainer && ItemDefinition.Lookup(itemId)?.IsNote() == true;
    }

    private static int ConvertToUnnoted(int notedItemId) => notedItemId - 1;
}