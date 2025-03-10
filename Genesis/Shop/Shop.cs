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

        int price = CalculateBuyPrice(shopItem.ItemId);
        int totalCost = price * quantity;

        if (player.InventoryItemContainer.GetItemCount(995) >= totalCost)
        {
            int transferred = InventorySystem.Transfer(
                source: Stock,
                destination: player.InventoryItemContainer,
                shopItem.ItemId,
                quantity);

            if (transferred > 0)
            {
                player.InventoryItemContainer.RemoveItem(995, totalCost);
                RefreshInterfaces(player);
            }
        }
    }

    public void SellItem(Player player, int slot, int quantity)
    {
        var playerItem = PlayerMirror.GetItem(slot);
        if (playerItem.IsEmpty) return;

        int price = CalculateSellPrice(playerItem.ItemId);
        int totalValue = price * quantity;

        // Transfer item from player to shop
        int transferred = InventorySystem.Transfer(
            source: player.InventoryItemContainer,
            destination: Stock,
            playerItem.ItemId,
            quantity);

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