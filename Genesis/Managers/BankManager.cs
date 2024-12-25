using Genesis.Entities;
using Genesis.Model;

namespace Genesis.Managers;

public class BankManager
{
    private readonly Player _player;
    
    RSItem[] BankItems { get; set; } = Enumerable.Range(0, ServerConfig.BANK_SIZE)
                                                 .Select(index => new RSItem(-1, 0, index))
                                                 .ToArray();

    public BankManager(Player player)
    {
        _player = player;
    }

    public bool AddItem(RSItem item)
    {
        if (item.IsStackable)
        {
            for (int i = 0; i < ServerConfig.BANK_SIZE; i++)
            {
                if (BankItems[i].Id == item.Id)
                {
                    long potentialTotal = (long)BankItems[i].Amount + item.Amount;
                    if (potentialTotal > int.MaxValue)
                    {
                        int amountThatCanBeAdded = int.MaxValue - BankItems[i].Amount;
                        BankItems[i].Amount = int.MaxValue;
                        item.Amount -= amountThatCanBeAdded;
                        
                        return AddItem(item);
                    }
                    else
                    {
                        BankItems[i].Amount += item.Amount;
                        item.Id = -1;
                        return true;
                    }
                }
            }
        }

        for (int i = 0; i < ServerConfig.BANK_SIZE; i++)
        {
            if (BankItems[i].Id == -1)
            {
                BankItems[i] = item;
                item.Id = -1;
                return true;
            }
        }

        return false;
    }
}