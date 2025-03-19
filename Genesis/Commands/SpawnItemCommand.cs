using ArcticRS.Constants;
using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Model;

namespace Genesis.Commands;

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

        if (Args.Length > 2 && !int.TryParse(Args[2], out _amount))
        {
            Player.Session.PacketBuilder.SendMessage("Invalid item amount! Try ::item [id] [amount]");
            return false;
        }

        return true;
    }

    public override void Invoke()
    {
         var def = ItemDefinition.Lookup(_id);
         Player.Inventory.AddItem(_id, _amount);
         Player.Inventory.Refresh(Player, GameInterfaces.DefaultInventoryContainer);
    }
}