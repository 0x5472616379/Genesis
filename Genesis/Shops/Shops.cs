using Genesis.Configuration;

namespace Genesis.Shops;

public class ShopsContainer
{
    public static Shop GeneralStore = new Shop("Lumbridge General Store", GameInterfaces.DefaultShopWindowInterface, GameInterfaces.DefaultShopInventoryInterface);
}