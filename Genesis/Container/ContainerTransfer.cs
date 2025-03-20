using Genesis.Cache;

namespace Genesis.Container;

public static class ContainerTransfer
{
    public static int Transfer(RSContainer source, RSContainer destination, int itemId, int amount)
    {
        if (amount <= 0) return 0;

        // Handle noted item conversion
        int destinationItemId = ShouldConvertToUnnoted(destination, itemId) 
            ? ConvertToUnnoted(itemId) 
            : itemId;

        // Calculate actual transfer amounts
        int available = source.GetItemCount(itemId);
        int removable = Math.Min(available, amount);
        int addable = CalculateAddable(destination, destinationItemId, removable);

        if (addable <= 0) return 0;

        // Perform the transfer
        int removed = source.RemoveItem(itemId, addable);
        int added = destination.AddItem(destinationItemId, removed);

        // Handle partial transfers
        if (removed > added)
            source.AddItem(itemId, removed - added);

        return added;
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