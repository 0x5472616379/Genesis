using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Container;
using Genesis.Entities.Player;
using Genesis.Environment;
using Genesis.Model;

namespace Genesis.Shops;

public class Shop
{
    public string Name { get; }
    public RSContainer Stock { get; }
    // public Container PlayerMirror { get; }
    private int _shopInterfaceId;
    private int _inventoryInterfaceId;

    public Shop(string name, int shopInterfaceId, int inventoryInterfaceId, int stockSize = 40)
    {
        // Name = name;
        // Stock = new Container(stockSize, true);
        // // PlayerMirror = new Container(28, false); // Matches player inventory size
        // _shopInterfaceId = shopInterfaceId;
        // _inventoryInterfaceId = inventoryInterfaceId;

        InitializeDefaultStock();
    }

    private void InitializeDefaultStock()
    {
        Random random = new Random();
        // Stock.Clear();
        for (int i = 0; i < 10; i++)
        {
            var itemId = random.Next(1, 4000);
            var item = ItemDefinition.Lookup(itemId);
            if (item == null || item.Id == 995 || item.IsNote()) continue;

            Stock.AddItem(item.Id, random.Next(1, 100));
        }
    }

    public Shop OpenForPlayer(Player player)
    {
        // player.InventoryItemContainer.CopyToContainer(player.ShopInventoryItemContainer);
        //
        // RefreshInterfaces(player);
        //
        // player.Session.PacketBuilder.SendTextToInterface(Name, GameInterfaces.ShopWindowTitleInterface);
        // player.Session.PacketBuilder.SendInterface(_shopInterfaceId, _inventoryInterfaceId);
        return this;
    }

    public void BuyItem(Player player, int slot, int quantity)
    {
        // var shopItem = Stock.GetItem(slot);
        // if (shopItem.IsEmpty) return;
        //
        // int itemId = shopItem.ItemId;
        // int price = CalculateBuyPrice(itemId);
        // int playerCoins = player.InventoryItemContainer.GetItemCount(995);
        // int maxAffordable = playerCoins / price;
        //
        // if (maxAffordable == 0)
        // {
        //     player.Session.PacketBuilder.SendMessage("You don't have enough coins.");
        //     return;
        // }
        //
        // int availableStock = shopItem.Quantity;
        // int quantityToBuy = Math.Min(quantity, Math.Min(maxAffordable, availableStock));
        //
        // /* Check if the player has the exact amount of coins needed */
        // if (playerCoins == price * quantityToBuy)
        // {
        //     /* Remove the exact amount of coins first */
        //     int coinsRemoved = player.InventoryItemContainer.RemoveItem(995, price * quantityToBuy);
        //     if (coinsRemoved == price * quantityToBuy)
        //     {
        //         /* Attempt to transfer the item */
        //         int transferred = InventorySystem.Transfer(Stock, player.InventoryItemContainer, itemId, quantityToBuy);
        //         if (transferred > 0)
        //         {
        //             player.InventoryItemContainer.CopyToContainer(player.ShopInventoryItemContainer);
        //             RefreshInterfaces(player);
        //             player.Session.PacketBuilder.SendMessage($"Purchased {transferred} items.");
        //         }
        //         else
        //         {
        //             /* Transfer failed, refund the coins */
        //             player.InventoryItemContainer.AddItem(995, price * quantityToBuy);
        //             player.Session.PacketBuilder.SendMessage("Not enough space in inventory.");
        //         }
        //     }
        //     else
        //     {
        //         /* Failed to remove coins, abort */
        //         player.Session.PacketBuilder.SendMessage("Failed to remove coins.");
        //     }
        // }
        // else
        // {
        //     /* Player has more coins than needed, proceed with normal transfer */
        //     int transferred = InventorySystem.Transfer(Stock, player.InventoryItemContainer, itemId, quantityToBuy);
        //     if (transferred > 0)
        //     {
        //         int totalCost = transferred * price;
        //         player.InventoryItemContainer.RemoveItem(995, totalCost);
        //         player.InventoryItemContainer.CopyToContainer(player.ShopInventoryItemContainer);
        //         RefreshInterfaces(player);
        //         player.Session.PacketBuilder.SendMessage($"Purchased {transferred} items.");
        //     }
        //     else
        //     {
        //         player.Session.PacketBuilder.SendMessage("Not enough space in inventory.");
        //     }
        // }
        
        GlobalRefresh();

    }

    public void SellItem(Player player, int slot, int quantity)
    {
        // var playerItem = player.ShopInventoryItemContainer.GetItem(slot);
        // if (playerItem.IsEmpty) return;
        //
        //
        // /* Transfer item from player to shop */
        // int transferred = InventorySystem.Transfer(
        //     source: player.InventoryItemContainer,
        //     destination: Stock,
        //     playerItem.ItemId,
        //     quantity);
        //
        // int price = CalculateSellPrice(playerItem.ItemId);
        // int totalValue = price * transferred;
        //
        // player.Session.PacketBuilder.SendMessage("Transferred " + transferred + " items.");
        // /* Add coins to player */
        // if (transferred > 0)
        // {
        //     player.InventoryItemContainer.AddItem(995, totalValue);
        //     player.InventoryItemContainer.CopyToContainer(player.ShopInventoryItemContainer);
        //     RefreshInterfaces(player);
        // }

        GlobalRefresh();
    }

    private void RefreshInterfaces(Player player)
    {
        // Stock.Refresh(player, GameInterfaces.DefaultShopWindowContainer);
        // player.ShopInventoryItemContainer.Refresh(player, GameInterfaces.DefaultShopInventoryContainer);
        // player.InventoryItemContainer.Refresh(player, GameInterfaces.DefaultInventoryContainer);
    }

     public void GlobalRefresh()
     {
         // var players = World.GetPlayers().Where(x => x != null && x.OpenShop == this);
         // foreach (var player in players)
         //     player.OpenShop.Stock.Refresh(player, GameInterfaces.DefaultShopWindowContainer);
     }
    
    private int CalculateBuyPrice(int itemId)
    {
        return 10;
    }

    private int CalculateSellPrice(int itemId)
    {
        return 10;
    }

    public void Restock()
    {
        // foreach (var slot in Stock.GetItems)
        // {
        //     if (slot.Quantity < 10)
        //         slot.Quantity = 10;
        // }
    }
}