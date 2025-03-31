using ArcticRS.Constants;
using Genesis.Cache;
using Genesis.Commands;
using Genesis.Configuration;
using Genesis.Entities.Player;

public class SpawnItemCommand : RSCommand
{
    protected override PlayerRights RequiredRights => PlayerRights.ADMIN;

    private int _id;
    private int _amount = 1;

    public SpawnItemCommand(Player player, string[] args) : base(player, args)
    {
    }

    public override bool Validate()
    {
        if (Args.Length < 2)
        {
            Player.Session.PacketBuilder.SendMessage("Invalid syntax! Try ::item 1");
            return false;
        }

        if (!int.TryParse(Args[1], out _id))
        {
            Player.Session.PacketBuilder.SendMessage("Invalid item ID! Try ::item 1");
            return false;
        }

        if (Args.Length > 2)
        {
            // Attempt parsing _amount while checking if the input might be larger than int.MaxValue
            if (!long.TryParse(Args[2], out long parsedAmount))
            {
                Player.Session.PacketBuilder.SendMessage("Invalid item amount! Try ::item [id] [amount]");
                return false;
            }

            // If the value exceeds int.MaxValue, clamp it to the maximum size of an integer
            _amount = parsedAmount > int.MaxValue ? int.MaxValue : (int)parsedAmount;
        }

        return true;
    }

    public override void Invoke()
    {
        var def = ItemDefinition.Lookup(_id);
        Player.Inventory.AddItem(_id, _amount);
        Player.Inventory.RefreshContainer(Player, GameInterfaces.DefaultInventoryContainer);
    }
}