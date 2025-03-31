using ArcticRS.Constants;
using Genesis.Entities.Player;

namespace Genesis.Commands;

public class ClearBankCommand : RSCommand
{
    protected override PlayerRights RequiredRights { get; }
    public ClearBankCommand(Player player, string[] args) : base(player, args)
    {
    }

    public override bool Validate()
    {
        return true;
    }

    public override void Invoke()
    {
        // Player.BankItemContainer.Clear();
    }
}