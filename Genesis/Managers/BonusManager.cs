using Genesis.Definitions;
using Genesis.Definitions.Items;
using Genesis.Entities.Player;

namespace Genesis.Managers;

public class BonusManager
{
    Player _player;
    int[] bonuses = new int[12];

    private String[] BONUS_NAMES =
    {
        "Stab", "Slash", "Crush", "Magic", "Range",
        "Stab", "Slash", "Crush", "Magic", "Range", 
        "Strength", "Prayer"
    };
    
    private readonly int[] BONUS_MAPPING = new int[]
    {
        0, 1, 2, 3, 4,  // Attack bonuses
        5, 6, 7, 8, 9,  // Defence bonuses
        10,             // Strength
        13              // Prayer
    };


    public BonusManager(Player player)
    {
        _player = player;
    }

    public int GetBonus(BonusType bonus)
    {
        return bonuses[(int)bonus];
    }

    public void AddBonus(BonusType bonus, int amount)
    {
        bonuses[(int)bonus] += amount; 
    }

    public void RemoveBonus(BonusType bonus, int amount)
    {
        bonuses[(int)bonus] -= amount;

        if (bonuses[(int)bonus] < 0)
            bonuses[(int)bonus] = 0;
    }

    public void CalculateBonuses(double[] itemBonuses)
    {
        // Reset current bonuses
        // Reset();

        // Map and add relevant bonuses
        foreach (var mapping in BONUS_MAPPING.Select((sourceIndex, targetIndex) => new { sourceIndex, targetIndex }))
        {
            AddBonus((BonusType)mapping.targetIndex, (int)itemBonuses[mapping.sourceIndex]);
        }
    }
    
    public void UpdateBonus()
    {
        for (int i = 0; i < bonuses.Length; i++)
        {
            int interfaceIndex = 1675 + i + (i >= 10 ? 1 : 0);
            string sign = bonuses[i] >= 0 ? "+" : "-";
            string text = $"{BONUS_NAMES[i]}: {sign}{Math.Abs(bonuses[i])}";

            _player.Session.PacketBuilder.SendTextToInterface(text, interfaceIndex);
        }
    }

    public void UpdateSpecificBonus(BonusType bonusType)
    {
        int index = (int)bonusType;

        // Determine the interface index, adjust for offset if necessary.
        int interfaceIndex = 1675 + index + (index >= 10 ? 1 : 0);

        // Create the bonus text with the appropriate sign.
        string sign = bonuses[index] >= 0 ? "+" : "-";
        string text = $"{BONUS_NAMES[index]}: {sign}{Math.Abs(bonuses[index])}";

        // Send the updated text to the interface.
        _player.Session.PacketBuilder.SendTextToInterface(text, interfaceIndex);
    }

    public int GetTotalForBonusType(BonusType bonusType)
    {
        int total = 0;

        foreach (var itemslot in _player.Equipment._slots)
        {
            if (itemslot.ItemId == -1) // Skip empty slots
                continue;

            // Get the bonuses for the current item and add the specific one to the total
            total += (int)ItemParser.GetBonusesById(itemslot.ItemId).Bonuses[(int)bonusType];
        }

        return total;
    }

    
    public void Reset()
    {
        for (int i = 0; i < bonuses.Length; i++)
        {
            bonuses[i] = 0;
        }
    }
}