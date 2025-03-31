using ArcticRS.Constants;
using Genesis.Configuration;
using Genesis.Entities.Player;

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
        // Define the pattern of items to add to the inventory
        var itemPattern = new List<int> { 385, 3144, 2434 }; // Shark, Karambwan, Prayer Potion

        // Create the list of items to add to fill all 28 slots
        var itemsToAdd = new List<(int itemId, int quantity)>();
        
        for (int i = 0; i < 28; i++)
        {
            int itemId = itemPattern[i % itemPattern.Count]; // Cycle through the pattern
            itemsToAdd.Add((itemId, 1)); // Add 1 of each item
        }

        // Add items to the player's inventory
        foreach (var (itemId, quantity) in itemsToAdd)
            Player.Inventory.AddItem(itemId, quantity);
        
        // Refresh the inventory
        Player.Inventory.RefreshContainer(Player, GameInterfaces.DefaultInventoryContainer);
    }

    
    // public override void Invoke()
    // {
    //     var itemsToAdd = new List<(int itemId, int quantity)>
    //     {
    //         (385, 8),
    //         (3144, 1),
    //         (385, 1),
    //         (3144, 1),
    //         (385, 2),
    //         (3144, 1),
    //         (385, 1),
    //         (2434, 1),
    //         (385, 10)
    //     };
    //
    //     foreach (var (itemId, quantity) in itemsToAdd)
    //         Player.Inventory.AddItem(itemId, quantity);
    //     
    //     Player.Inventory.RefreshContainer(Player, GameInterfaces.DefaultInventoryContainer);
    // }
}