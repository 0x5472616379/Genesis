using System.Globalization;
using ArcticRS.Constants;
using Genesis.Configuration;
using Genesis.Entities;

namespace Genesis.Commands;

public class RemoveItemCommand : RSCommand
{
    private int _index;

    public RemoveItemCommand(Player player, string[] args) : base(player, args)
    {
    }

    protected override PlayerRights RequiredRights { get; }
    public override bool Validate()
    {
        if (Args.Length < 2)
        {
            Player.Session.PacketBuilder.SendMessage("Invalid syntax! Try ::remove 0 - 27");
            return false;
        }

        if (!int.TryParse(Args[1], out _index))
        {
            Player.Session.PacketBuilder.SendMessage("Invalid inventory index! Try ::remove 0 - 27");
            return false;
        }

        return true;
    }

    public override void Invoke()
    {
        // Player.InventoryItemContainer.Remove(_index, 5);
        // Player.InventoryItemContainer.Refresh(Player, GameInterfaces.DefaultInventoryContainer);
    }
}