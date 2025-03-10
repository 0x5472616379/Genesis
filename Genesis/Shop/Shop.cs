using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Model;

namespace Genesis.Shop;

public class Shop
{
    public string Name { get; }
    public Container Stock { get; }
    public Container PlayerMirror { get; }
    private int _shopInterfaceId;
    private int _inventoryInterfaceId;

    public Shop(string name, int shopInterfaceId, int inventoryInterfaceId, int stockSize = 20)
    {
        Name = name;
        Stock = new Container(stockSize, true);
        PlayerMirror = new Container(28, false);  // Matches player inventory size
        _shopInterfaceId = shopInterfaceId;
        _inventoryInterfaceId = inventoryInterfaceId;
        
        InitializeDefaultStock();
    }

    private void InitializeDefaultStock()
    {
        Random random = new Random();
        Stock.Clear();
        for (int i = 0; i < 10; i++)
        {
            var itemId = random.Next(1, 4000);
            var item = ItemDefinition.Lookup(itemId);
            if (item == null || item.Id == 995 || item.IsNote()) continue;
        
            Stock.AddItem(item.Id, random.Next(1, 100));
        }
    }

    public void OpenForPlayer(Player player)
    {
        player.InventoryItemContainer.CopyToContainer(PlayerMirror);
        
        RefreshInterfaces(player);
        
        player.Session.PacketBuilder.SendTextToInterface(Name, GameInterfaces.ShopWindowTitleInterface);
        player.Session.PacketBuilder.SendInterface(_shopInterfaceId, _inventoryInterfaceId);
    }

    public void BuyItem(Player player, int slot, int quantity)
    {
        var shopItem = Stock.GetItem(slot);
        if (shopItem.IsEmpty) return;

        int itemId = shopItem.ItemId;
        int price = CalculateBuyPrice(itemId);
        int playerCoins = player.InventoryItemContainer.GetItemCount(995);
        int maxAffordable = playerCoins / price;

        if (maxAffordable == 0)
        {
            player.Session.PacketBuilder.SendMessage("Not enough coins.");
            return;
        }

        int quantityToBuy = Math.Min(quantity, maxAffordable);
        int transferred = InventorySystem.Transfer(Stock, player.InventoryItemContainer, itemId, quantityToBuy);

        // Handle cases where removing coins might free up inventory space
        if (transferred == 0)
        {
            // Check if buying 1 item by removing exact coins would free a slot
            bool hasExactCoins = player.InventoryItemContainer.ContainsSlotWithExactQuantity(995, price);
            if (hasExactCoins)
            {
                // Attempt to remove coins and transfer again
                int coinsRemoved = player.InventoryItemContainer.RemoveItem(995, price);
                if (coinsRemoved == price)
                {
                    transferred = InventorySystem.Transfer(Stock, player.InventoryItemContainer, itemId, 1);
                    if (transferred == 0)
                    {
                        // Transfer failed, refund coins
                        player.InventoryItemContainer.AddItem(995, price);
                    }
                }
            }
        }

        if (transferred > 0)
        {
            int totalCost = transferred * price;
            player.InventoryItemContainer.RemoveItem(995, totalCost);
            player.InventoryItemContainer.CopyToContainer(PlayerMirror);
            RefreshInterfaces(player);
            player.Session.PacketBuilder.SendMessage($"Transferred {transferred} items.");
        }
        else
        {
            player.Session.PacketBuilder.SendMessage("Not enough space in inventory.");
        }
    }

    public void SellItem(Player player, int slot, int quantity)
    {
        var playerItem = PlayerMirror.GetItem(slot);
        if (playerItem.IsEmpty) return;

        

        // Transfer item from player to shop
        int transferred = InventorySystem.Transfer(
            source: player.InventoryItemContainer,
            destination: Stock,
            playerItem.ItemId,
            quantity);
        
        int price = CalculateSellPrice(playerItem.ItemId);
        int totalValue = price * transferred;

        player.Session.PacketBuilder.SendMessage("Transferred " + transferred + " items.");
        // Add coins to player
        if (transferred > 0)
        {
            player.InventoryItemContainer.AddItem(995, totalValue);
            player.InventoryItemContainer.CopyToContainer(PlayerMirror);
            RefreshInterfaces(player);
        }
    }

    private void RefreshInterfaces(Player player)
    {
        Stock.Refresh(player, GameInterfaces.DefaultShopWindowContainer);
        PlayerMirror.Refresh(player, GameInterfaces.DefaultShopInventoryContainer);
        player.InventoryItemContainer.Refresh(player, GameInterfaces.DefaultInventoryContainer);
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
        foreach (var slot in Stock.GetItems)
        {
            if (slot.Quantity < 10)
                slot.Quantity = 10;
        }
    }
}