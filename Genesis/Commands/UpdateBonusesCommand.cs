using ArcticRS.Constants;
using Genesis.Definitions.Items;
using Genesis.Entities.Player;

namespace Genesis.Commands;

public class UpdateBonusesCommand : RSCommand
{
    int[] bonusMapping = new int[]
    {
        0, 1, 2, 3, 4, // Attack Bonuses
        5, 6, 7, 8, 9, // Defence Bonuses
        10, // Strength
        13 // Prayer (Mapped from index 13 to 11 in the output)
    };


    protected override PlayerRights RequiredRights { get; } = PlayerRights.ADMIN;

    public UpdateBonusesCommand(Player player, string[] args) : base(player, args)
    {
    }

    public override bool Validate()
    {
        return true;
    }

    public override void Invoke()
    {
        Player.BonusManager.Reset();

        foreach (var itemslot in Player.Equipment._slots)
        {
            if (itemslot.ItemId == -1)
                continue;

            var itemBonuses = ItemParser.GetBonusesById(itemslot.ItemId).Bonuses;
            Player.BonusManager.CalculateBonuses(itemBonuses);
        }

        Player.BonusManager.UpdateBonus();
    }
}