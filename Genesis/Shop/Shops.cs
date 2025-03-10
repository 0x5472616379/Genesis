using Genesis.Configuration;

namespace Genesis.Shop;

public class Shops
{
    public static Shop GeneralStore = new Shop("Lumbridge General Store", GameInterfaces.DefaultShopWindowInterface, GameInterfaces.DefaultShopInventoryInterface);
}