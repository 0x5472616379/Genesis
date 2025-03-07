using ArcticRS.Commands;
using ArcticRS.Constants;
using Genesis.Entities;

namespace Genesis.Commands;

public class ClearInventoryCommand : RSCommand
{
    protected override PlayerRights RequiredRights => PlayerRights.ADMIN;
    public ClearInventoryCommand(Player player, string[] args) : base(player, args)
    {
        
    }

    public override bool Validate()
    {
        return true;
    }

    public override void Invoke()
    {
        Player.InventoryManager.Clear();

        Player.InventoryManager.AddItem(1462);
        Player.InventoryManager.AddItem(1436, 27);
        
        Player.InventoryManager.RefreshInventory();
    }
}