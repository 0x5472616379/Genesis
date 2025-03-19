using ArcticRS.Appearance;
using Genesis.Cache;
using Genesis.Entities;

namespace Genesis.Managers;

public class AnimationManager
{
    public int Stand { get; set; } = 0x328;
    public int StandTurn { get; set; } = 0x337;
    public int Walk { get; set; } = 0x333;
    public int Turn180 { get; set; } = 0x334;
    public int Turn90CW { get; set; } = 0x335;
    public int Turn90CCW { get; set; } = 0x336;
    public int Run { get; set; } = 0x338;

    private readonly int DefaultStand = 0x328;
    private readonly int DefaultWalk = 0x333;
    private readonly int DefaultRun = 0x338;

    public AnimationManager()
    {
        // Initialize to defaults
        Stand = DefaultStand;
        Walk = DefaultWalk;
        Run = DefaultRun;
    }

    public void SetAnimations(string weaponName, int weaponId)
    {
        // Cache defaults to use throughout the method
        int defaultStand = DefaultStand;
        int defaultWalk = DefaultWalk;
        int defaultRun = DefaultRun;

        // Normalize weapon name
        weaponName = string.IsNullOrEmpty(weaponName) ? ItemDefinition.Lookup(weaponId)?.Name.ToLower() ?? string.Empty : weaponName.ToLower();

        // Update animations using a switch expression
        (Stand, Walk, Run) = weaponName switch
        {
            string name when name.Contains("halberd") || name.Contains("guthan") => (809, 1146, 1210),
            string name when name.Contains("dharok") => (0x811, 0x67F, 0x680),
            string name when name.Contains("ahrim") => (809, 1146, 1210),
            string name when name.Contains("verac") => (1832, 1830, 1831),
            string name when name.Contains("wand") || name.Contains("staff") => (809, 1146, 1210),
            string name when name.Contains("karil") => (2074, 2076, 2077),
            string name when name.Contains("2h sword") => (2561, 2562, 2563),
            string name when name.Contains("bow") => (808, 819, 824),

            // Weapon ID-specific cases
            _ => weaponId switch
            {
                4151 => (1832, 1660, 1661), // Whip
                6528 => (0x811, 2064, 1664), // Obsidian Maul
                4153 => (1662, 1663, 1664), // Granite Maul
                11694 or 11696 or 11730 or 11698 or 11700 => (4300, 4306, 4305), // Godswords
                1305 => (809, defaultWalk, defaultRun), // Dragon Longsword (only Stand changes)
                
                // Default case: reset to original default values
                _ => (defaultStand, defaultWalk, defaultRun)
            }
        };
    }



    public int GetWeaponAnimation(int weaponId, int fightMode)
    {
        var weaponName = ItemDefinition.Lookup(weaponId)?.Name.ToLower() ?? string.Empty;

        if (string.IsNullOrEmpty(weaponName))
            return 422;

        return (weaponName, weaponId) switch
        {
            // Name-based animations
            (var name, _) when name.Contains("knife") || name.Contains("dart") || name.Contains("javelin") ||
                               name.Contains("thrownaxe") => 806,
            (var name, _) when name.Contains("halberd") => fightMode == 2 ? 440 : 412,
            (var name, _) when name.Contains("pickaxe") => fightMode == 2
                ? (Random.Shared.Next(2) == 0 ? 400 : 401)
                : 395,
            (var name, _) when name.Contains("dragon dagger") => 402,
            (var name, _) when name.Contains("scimitar") => 451,
            (var name, _) when name.Contains("2h sword") || name.Contains("godsword") || name.Contains("aradomin sword")
                => fightMode == 4 ? 406 : 407,
            (var name, _) when name.Contains("longsword") => fightMode == 3 ? 412 : 451,
            (var name, _) when name.Contains("sword") || name.EndsWith("dagger") => fightMode == 3 ? 451 : 412,
            (var name, _) when name.Contains("karil") => 2075,
            (var name, _) when name.Contains("bow") && !name.Contains("'bow") => 426,
            (var name, _) when name.Contains("'bow") => 4230,

            // ID-based animations
            (_, 6522) => 2614,
            (_, 4153) => 1665, // Granite Maul
            (_, 4726) => 2080, // Guthan
            (_, 4747) => 0x814, // Torag
            (_, 4718) => 2067, // Dharok
            (_, 4710) => 406, // Ahrim
            (_, 4755) => 2062, // Verac
            (_, 4734) => 2075, // Karil
            (_, 4151) => 1658,
            (_, 6528) => 2661,

            // Default animation
            _ => 422
        };
    }

    public int GetBlockEmote(int weaponId, int shieldId)
    {
        var weaponName = ItemDefinition.Lookup(weaponId)?.Name.ToLower() ?? string.Empty;
        var shieldName = ItemDefinition.Lookup(shieldId)?.Name.ToLower() ?? string.Empty;

        if (string.IsNullOrEmpty(weaponName) && string.IsNullOrEmpty(shieldName))
            return 404;

        return (shieldName, weaponName, shieldId, weaponId) switch
        {
            /* Shields */
            (var shield, _, >= 8844 and <= 8850, _) => 4177, // Special shields
            ("shield", _, _, _) => 1156, // Generic shields
            (_, var weapon, _, _) when weapon.Contains("staff") => 420, // Staff block animation

            /* Weapon ID-based block animations */
            (_, _, _, 4755) => 2063, // Verac
            (_, _, _, 4153) => 1666, // Granite Maul
            (_, _, _, 4151) => 1659, // Abyssal Whip
            (_, _, _, 11694 or 11696 or 11698 or 11700 or 11730) => -1, // Godswords

            _ => 404
        };
    }

    public List<int> GetAnimations()
    {
        return new List<int>
        {
            Stand,
            StandTurn,
            Walk,
            Turn180,
            Turn90CW,
            Turn90CCW,
            Run
        };
    }
}