// using Genesis.Cache;
// using Genesis.Configuration;
// using Genesis.Entities;
// using Genesis.Model;
// using System.Linq;
//
// namespace Genesis.Managers;
//
// public class BankManager
// {
//     private readonly Player _player;
//
//     private List<Item> BankItems = new(new Item[ServerConfig.BANK_SIZE]);
//
//     public BankManager(Player player)
//     {
//         _player = player;
//     }
//
//     public int AddItem(int itemId, int amount = 1)
//     {
//         var itemDefinition = ItemDefinition.Lookup(itemId);
//         if (itemDefinition.Id == -1)
//         {
//             _player.Session.PacketBuilder.SendMessage("Item not found!");
//             return 0;
//         }
//
//         amount = Math.Min(amount, int.MaxValue);
//         int addedAmount = 0;
//
//         // Treat all items as stackable
//         var existingItemIndex = GetItemIndex(itemId);
//
//         // If item does not already exist in BankItems
//         if (existingItemIndex == -1)
//         {
//             if (NotifyInventoryFull()) return 0;
//
//             // Create a new stack
//             var newItem = new Item(itemId, amount, stackable: true);
//             if (AddToEmptySlot(newItem))
//             {
//                 addedAmount = amount;
//             }
//             else
//             {
//                 _player.Session.PacketBuilder.SendMessage("Failed to add item to inventory!");
//             }
//         }
//         else
//         {
//             Item existingItem = GetItemAtIndex(existingItemIndex);
//
//             // Check if adding the amount will exceed int.MaxValue
//             if (existingItem.Amount > int.MaxValue - amount)
//             {
//                 // Notify or handle that we cannot add the new amount
//                 _player.Session.PacketBuilder.SendMessage("You can't add more of that to the bank.");
//                 return 0;
//             }
//
//             // Otherwise, safely add the item amount
//             existingItem.Amount += amount;
//             RefreshInventory();
//             return amount;
//         }
//
//         return addedAmount;
//     }
//
//     public int GetAvailableSpaceForId(Item item)
//     {
//         var existingItem = BankItems.FirstOrDefault(x => x.Id == item.Id);
//
//         if (item.Stackable)
//         {
//             if (existingItem != null)
//             {
//                 if (item.Stackable)
//                     return int.MaxValue - item.Amount;
//             }
//             else
//             {
//                 if (GetFirstAvailableSlot() != -1)
//                 {
//                     return int.MaxValue;
//                 }
//
//                 return 0;
//             }
//         }
//
//
//         return 0;
//     }
//
//     private bool NotifyInventoryFull()
//     {
//         if (GetItemCount() >= ServerConfig.BANK_SIZE)
//         {
//             _player.Session.PacketBuilder.SendMessage("Bank is full!");
//             return true;
//         }
//
//         return false;
//     }
//
//     private bool AddToEmptySlot(Item newItem)
//     {
//         for (int i = 0; i < ServerConfig.BANK_SIZE; i++)
//         {
//             if (BankItems[i] == null)
//             {
//                 BankItems[i] = newItem;
//                 return true;
//             }
//         }
//
//         return false;
//     }
//
//     public void Remove(int index, int amount = 1)
//     {
//         if (index < 0 || index >= BankItems.Count)
//             throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
//
//         var item = BankItems[index];
//         if (item is null)
//             throw new InvalidOperationException("No item exists at the specified index.");
//
//         if (item.Amount > amount)
//             item.Amount -= amount;
//         else
//             BankItems[index] = null;
//     }
//
//     public int RemoveItems(int itemId, int amountToRemove)
//     {
//         if (amountToRemove <= 0)
//             return 0;
//
//         // Check free slots in the inventory
//         int freeSlots = _player.InventoryManager.GetAllItemsIncNull().Count(x => x == null);
//         int remainingToAdd = amountToRemove;
//
//         var itemDefinition = ItemDefinition.Lookup(itemId);
//         if (itemDefinition != null && itemDefinition.IsStackable())
//         {
//             // If the item is stackable, check if it can be added to an existing stack
//             var existingStack = _player.InventoryManager.GetItems().FirstOrDefault(x => x.Id == itemId);
//             if (existingStack != null)
//             {
//                 int stackableSpace = int.MaxValue - existingStack.Amount;
//                 remainingToAdd = Math.Max(0, remainingToAdd - stackableSpace);
//             }
//         }
//
//         // Calculate free slots needed in addition to existing stacks
//         if (remainingToAdd > 0 && !itemDefinition.IsStackable())
//         {
//             int requiredSlots = remainingToAdd;
//             if (requiredSlots > freeSlots)
//             {
//                 remainingToAdd -= (requiredSlots - freeSlots);
//             }
//         }
//
//         // If there's no space to add any items, return 0
//         if (remainingToAdd <= 0)
//         {
//             _player.Session.PacketBuilder.SendMessage("You can't add more of that to the bank.");
//             return 0;
//         }
//
//         // Remove items from the bank and add as much as we can
//         int removedCount = 0;
//
//         for (int i = 0; i < BankItems.Count && removedCount < remainingToAdd; i++)
//         {
//             var item = BankItems[i];
//             if (item?.Id == itemId)
//             {
//                 if (item.Amount + removedCount > remainingToAdd)
//                 {
//                     int toRemove = remainingToAdd - removedCount;
//                     item.Amount -= toRemove;
//                     removedCount += toRemove;
//                 }
//                 else
//                 {
//                     removedCount += item.Amount;
//                     BankItems[i] = null;
//                 }
//             }
//         }
//
//         return removedCount;
//     }
//
//     public int RemoveItemsWithId(int itemId)
//     {
//         var indicesToRemove = BankItems
//             .Select((item, index) => new { Item = item, Index = index })
//             .Where(x => x.Item != null && x.Item.Id == itemId)
//             .Select(x => x.Index)
//             .ToList();
//
//         foreach (var index in indicesToRemove)
//         {
//             BankItems[index] = null;
//         }
//
//         return indicesToRemove.Count;
//     }
//
//     public bool SwapItems(int from, int to)
//     {
//         if (from < 0 || from >= ServerConfig.BANK_SIZE || to < 0 || to >= ServerConfig.BANK_SIZE)
//         {
//             _player.Session.PacketBuilder.SendMessage($"Invalid indexes: {from} or {to}.");
//             return false;
//         }
//
//         var item1 = BankItems[from];
//         var item2 = BankItems[to];
//
//         BankItems[from] = item2;
//         BankItems[to] = item1;
//
//         RefreshBankContainer();
//         return true;
//     }
//
//     public void Clear()
//     {
//         BankItems = new List<Item>(new Item[ServerConfig.BANK_SIZE]);
//     }
//
//     public int GetItemIndex(int itemId) => BankItems.FindIndex(i => i != null && i.Id == itemId);
//     public Item GetItemAtIndex(int index) => BankItems[index];
//     public bool HasItem(int itemId) => GetItemIndex(itemId) != -1;
//     public int GetFirstAvailableSlot() => BankItems.FindIndex(x => x == null);
//
//     public void CopyInventory(List<Item> items)
//     {
//         _player.Session.PacketBuilder.RefreshContainer(items, GameInterfaces.BankInventoryContainer,
//             ServerConfig.INVENTORY_SIZE);
//     }
//
//     public void RefreshBankContainer()
//     {
//         _player.Session.PacketBuilder.RefreshContainer(BankItems, GameInterfaces.DefaultBankContainer,
//             ServerConfig.BANK_SIZE);
//     }
//
//     public void RefreshInventory()
//     {
//         CopyInventory(_player.InventoryManager.GetAllItemsIncNull());
//         _player.Session.PacketBuilder.RefreshContainer(BankItems, GameInterfaces.DefaultBankContainer,
//             ServerConfig.BANK_SIZE);
//     }
//
//     public void CompressBank()
//     {
//         int idx = 0;
//
//         for (int i = 0; i < BankItems.Count; i++)
//         {
//             if (BankItems[i] != null && BankItems[i].Id != -1)
//             {
//                 BankItems[idx] = BankItems[i];
//                 idx++;
//             }
//         }
//
//         for (int j = idx; j < BankItems.Count; j++)
//         {
//             BankItems[j] = null;
//         }
//     }
//
//     public int GetItemCount() => BankItems.Count(i => i != null);
//     public List<Item> GetItems() => BankItems.Where(x => x != null).ToList();
// }