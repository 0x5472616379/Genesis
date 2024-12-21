using ArcticRS.Commands;
using ArcticRS.Constants;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Model;

namespace Genesis.Commands;

public class SpawnItemCommand : CommandBase
{
    private int _id;
    private int _amount = 1;


    public SpawnItemCommand(Player player, string[] args) : base(player, args)
    {
    }

    protected override PlayerRights RequiredRights => PlayerRights.ADMIN;

    protected override string ValidateArgs()
    {
        if (Args.Length < 2)
        {
            return "Invalid syntax! Try ::item 1";
        }

        if (!int.TryParse(Args[1], out _id))
        {
            return "Invalid item ID! Try ::item 1";
        }

        if (Args.Length > 2 && !int.TryParse(Args[2], out _amount))
        {
            return "Invalid item amount! Try ::item [id] [amount]";
        }

        return null;
    }

    protected override void Invoke()
    {
        Player.Inventory.AddItem(new RSItem(_id, _amount));
        Player.Inventory.Refresh(GameInterfaces.DefaultInventoryContainer);
    }
}