using ArcticRS.Commands;
using ArcticRS.Constants;
using Genesis.Entities;

namespace Genesis.Commands;

public class ClearInventoryCommand : CommandBase
{
    protected override PlayerRights RequiredRights => PlayerRights.ADMIN;
    public ClearInventoryCommand(Player player, string[] args) : base(player, args)
    {
        
    }

    protected override string ValidateArgs()
    {
        return null;
    }

    protected override void Invoke()
    {
        Player.InventoryManager.Clear();
        Player.InventoryManager.RefreshInventory();
    }
}