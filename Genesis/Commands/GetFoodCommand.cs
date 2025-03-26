using ArcticRS.Constants;
using Genesis.Configuration;
using Genesis.Entities;

namespace Genesis.Commands;

public class GetFoodCommand : RSCommand
{
    protected override PlayerRights RequiredRights { get; }

    public GetFoodCommand(Player player, string[] args) : base(player, args)
    {
    }

    public override bool Validate()
    {
        return true;
    }

    public override void Invoke()
    {
        var itemsToAdd = new List<(int itemId, int quantity)>
        {
            (385, 8),
            (3144, 1),
            (385, 1),
            (3144, 1),
            (385, 2),
            (3144, 1),
            (385, 1),
            (3144, 1),
            (385, 10)
        };

        foreach (var (itemId, quantity) in itemsToAdd)
            Player.Inventory.AddItem(itemId, quantity);
        
        Player.Inventory.RefreshContainer(Player, GameInterfaces.DefaultInventoryContainer);
    }
}