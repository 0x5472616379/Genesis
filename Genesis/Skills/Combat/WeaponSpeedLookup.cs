namespace Genesis.Skills.Combat;

public class WeaponSpeedLookup
{
    private static readonly Dictionary<string, WeaponInfo> WeaponData = new()
    {
        // Melee Weapons
        { "unarmed", new WeaponInfo(6, 4, 0, 0) },
        { "axe", new WeaponInfo(5, 5, 0, 0) },
        { "dagger", new WeaponInfo(6, 4, 0, 0) },
        { "sword", new WeaponInfo(6, 4, 0, 0) },
        { "scimitar", new WeaponInfo(6, 4, 0, 0) },
        { "claws", new WeaponInfo(6, 4, 0, 0) },
        { "pickaxe", new WeaponInfo(5, 5, 0, 0) },
        { "staff", new WeaponInfo(5, 5, 0, 0) },
        { "mace", new WeaponInfo(5, 5, 0, 0) },
        { "spear", new WeaponInfo(5, 5, 0, 0) },
        { "longsword", new WeaponInfo(5, 5, 0, 0) },
        { "warhammer", new WeaponInfo(4, 6, 0, 0) },
        { "battleaxe", new WeaponInfo(4, 6, 0, 0) },
        { "halberd", new WeaponInfo(3, 7, 0, 0) },
        { "2h", new WeaponInfo(3, 7, 0, 0) },
        { "ket-om", new WeaponInfo(3, 7, 0, 0) },

        // Ranged Weapons
        { "dart", new WeaponInfo(7, 3, 2, 0) },
        { "knife", new WeaponInfo(7, 3, 2, 0) },
        { "shortbow", new WeaponInfo(6, 4, 3, 0) },
        { "composite bow", new WeaponInfo(5, 5, 4, 0) },
        { "throwing axe", new WeaponInfo(5, 5, 4, 0) },
        { "longbow", new WeaponInfo(4, 6, 5, 0) },
        { "crossbow", new WeaponInfo(4, 6, 5, 0) },

        // Special Ranged Weapons
        { "toxic blowpipe", new WeaponInfo(6, 3, 2, 4) },
        { "hunters' crossbow", new WeaponInfo(6, 4, 3, 0) },
        { "toktz-xil-ul", new WeaponInfo(6, 4, 3, 0) },
        { "karil's crossbow", new WeaponInfo(6, 4, 3, 0) },
        { "crystal bow", new WeaponInfo(5, 5, 4, 0) },
        { "ogre bow", new WeaponInfo(3, 7, 6, 0) },
        { "dark bow", new WeaponInfo(2, 8, 7, 0) }
    };


    public static int? GetWeaponTicks(string weaponName, FightMode fightMode)
    {
        var weaponInfo = FindWeaponInfo(weaponName);
        if (weaponInfo == null) return null;

        // Determine tick value based on fight mode
        return fightMode switch
        {
            FightMode.ACCURATE => weaponInfo.DefaultTick,
            FightMode.AGGRESSIVE => weaponInfo.RapidTick > 0 ? weaponInfo.RapidTick : weaponInfo.DefaultTick,
            FightMode.CONTROLLED => weaponInfo.DefaultTick,
            FightMode.DEFENSIVE => weaponInfo.DefaultTick,
            _ => weaponInfo.DefaultTick // Fallback to default for unsupported modes
        };
    }

    public static string GetFormattedTicks(string weaponName, FightMode fightMode)
    {
        var weaponInfo = FindWeaponInfo(weaponName);
        if (weaponInfo == null)
        {
            return "Weapon not found.";
        }

        // Use the fight mode to determine which tick value is displayed
        var tickValue = GetWeaponTicks(weaponName, fightMode);

        return $"{weaponName} - Speed: {weaponInfo.Speed}, Ticks: {tickValue} ({fightMode} mode)";
    }

    private static WeaponInfo? FindWeaponInfo(string weaponName)
    {
        // Perform a case-insensitive partial match against the weapon names
        var match = WeaponData.FirstOrDefault(kvp =>
            weaponName.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase));

        return match.Value;
    }

    // Record to store weapon data
    public record WeaponInfo(int Speed, int DefaultTick, int RapidTick, int PvPTick);
}