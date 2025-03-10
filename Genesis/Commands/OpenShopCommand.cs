using ArcticRS.Constants;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Shop;

namespace Genesis.Commands;

public class OpenShopCommand : RSCommand
{
    public OpenShopCommand(Player player, string[] args) : base(player, args)
    {
    }

    protected override PlayerRights RequiredRights { get; }

    public override bool Validate()
    {
        return true;
    }

    public override void Invoke()
    {
        Shops.GeneralStore.OpenForPlayer(Player);
        // Player.InventoryItemContainer.CopyToContainer(Player.ShopInventoryItemContainer);
        //
        // Player.ShopItemContainer.Refresh(Player, GameInterfaces.DefaultBankContainer);
        //
        // Player.InventoryItemContainer.Refresh(Player, GameInterfaces.DefaultInventoryContainer);
        // Player.ShopInventoryItemContainer.Refresh(Player, GameInterfaces.DefaultShopInventoryContainer);
        //
        // Player.Session.PacketBuilder.SendTextToInterface("Lumbridge General Store", GameInterfaces.ShopWindowTitleInterface);
        // Player.Session.PacketBuilder.SendInterface(GameInterfaces.DefaultShopWindowInterface, GameInterfaces.DefaultShopInventoryInterface);
    }
}