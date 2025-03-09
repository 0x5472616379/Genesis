using ArcticRS.Commands;
using ArcticRS.Constants;
using Genesis.Entities;
using Genesis.Skills.Runecrafting;

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
        // Player.InventoryManager.Clear();
        //
        // RunecraftingAltarData.GetAllTalismanIds().ForEach(id => Player.InventoryManager.AddItem(id));
        // Player.InventoryManager.AddItem(1436, 17);
        //
        // Player.InventoryManager.RefreshInventory();
    }
}