using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Model;

namespace Genesis.Managers;

public class InventoryManager
{
    private readonly Player _player;
    
    RSItem[] DefaultInventory { get; set; } = Enumerable.Range(0, ServerConfig.INVENTORY_SIZE)
                                                        .Select(index => new RSItem(-1, 0, index))
                                                        .ToArray();
    
    RSItem[] BankInventory { get; set; } = Enumerable.Range(0, ServerConfig.INVENTORY_SIZE)
                                                     .Select(index => new RSItem(-1, 0, index))
                                                     .ToArray();

    public InventoryManager(Player player)
    {
        _player = player;
    }

    public bool AddItem(RSItem item)
    {
        if (item.IsStackable)
        {
            for (int i = 0; i < ServerConfig.BANK_SIZE; i++)
            {
                if (DefaultInventory[i].Id == item.Id)
                {
                    long potentialTotal = (long)DefaultInventory[i].Amount + item.Amount;
                    if (potentialTotal > int.MaxValue)
                    {
                        int amountThatCanBeAdded = int.MaxValue - DefaultInventory[i].Amount;
                    
                        // Update both DefaultInventory and BankInventory
                        DefaultInventory[i].Amount = int.MaxValue;
                        BankInventory[i].Amount = int.MaxValue;
                    
                        item.Amount -= amountThatCanBeAdded;
                    
                        // Continue processing the remaining amount
                        return AddItem(item);
                    }

                    DefaultInventory[i].Amount += item.Amount;
                    BankInventory[i].Amount += item.Amount;
                    
                    item.Id = -1;
                    return true;
                }
            }
        }

        // For non-stackable items or new stack slots
        for (int i = 0; i < ServerConfig.BANK_SIZE; i++)
        {
            if (DefaultInventory[i].Id == -1)
            {
                DefaultInventory[i] = new RSItem(item.Id, item.Amount, i);
                BankInventory[i] = new RSItem(item.Id, item.Amount, i);
            
                item.Id = -1;
                return true;
            }
        }

        _player.Session.PacketBuilder.SendMessage("Inventory is full.");
        return false;
    }

    public void RefreshInventory()
    {
        _player.Session.PacketBuilder.RefreshContainer(DefaultInventory, GameInterfaces.DefaultInventoryContainer);
    }
    

    public void Remove(int index, int amount)
    {
        DefaultInventory[index].Amount -= amount;

        if (DefaultInventory[index].Amount == 0)
            DefaultInventory[index].Id = -1;
    }
}