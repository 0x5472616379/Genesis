using Genesis.Entities;

namespace Genesis.Skills;

public class SkillManager
{
    private readonly Player _player;
    public static int MAX_EXPERIENCE = 200000000;
    public static int MIN_EXPERIENCE = 0;
    private const int SKILL_COUNT = 21;

    public int CombatLevel { get; set; } = 3;

    public static int[] EXPERIENCE_TABLE =
    [
        0, 0, 83, 174, 276, 388, 512, 650, 801, 969, 1154, 1358, 1584, 1833, 2107, 2411, 2746, 3115, 3523, 3973,
        4470, 5018, 5624, 6291, 7028, 7842, 8740, 9730, 10824, 12031, 13363, 14833, 16456, 18247, 20224, 22406,
        24815, 27473, 30408, 33648, 37224, 41171, 45529, 50339, 55649, 61512, 67983, 75127, 83014, 91721, 101333,
        111945, 123660, 136594, 150872, 166636, 184040, 203254, 224466, 247886, 273742, 302288, 333804, 368599,
        407015, 449428, 496254, 547953, 605032, 668051, 737627, 814445, 899257, 992895, 1096278, 1210421, 1336443,
        1475581, 1629200, 1798808, 1986068, 2192818, 2421087, 2673114, 2951373, 3258594, 3597792, 3972294, 4385776,
        4842295, 5346332, 5902831, 6517253, 7195629, 7944614, 8771558, 9684577, 10692629, 11805606, 13034431
    ];

    // Skill[] _skills = new Skill[SKILL_COUNT];

    public Skill[] Skills { get; set; } = new Skill[SKILL_COUNT];

    public SkillManager(Player player)
    {
        _player = player;

        for (int i = 0; i < Skills.Length; i++)
        {
            if ((SkillType)i == SkillType.HITPOINTS)
            {
                Skills[i] = new Skill((SkillType)i);
                Skills[i].Level = 99;
                Skills[i].Experience = EXPERIENCE_TABLE[99];
                _player.CurrentHealth = 99;

                continue;
            }

            if ((SkillType)i == SkillType.MAGIC)
            {
                Skills[i] = new Skill((SkillType)i);
                Skills[i].Level = 99;
                Skills[i].Experience = EXPERIENCE_TABLE[99];

                continue;
            }

            Skills[i] = new Skill((SkillType)i);
            Skills[i].Level = 1;
            Skills[i].Experience = EXPERIENCE_TABLE[1];
        }

        CombatLevel = GetCombatLevel(Skills[(int)SkillType.ATTACK].Level,
            Skills[(int)SkillType.STRENGTH].Level,
            Skills[(int)SkillType.MAGIC].Level,
            Skills[(int)SkillType.RANGED].Level,
            Skills[(int)SkillType.DEFENCE].Level,
            Skills[(int)SkillType.HITPOINTS].Level,
            Skills[(int)SkillType.PRAYER].Level);
    }

    public void RefreshSkills()
    {
        for (int i = 0; i < SKILL_COUNT; i++)
        {
            var skill = Skills[i];
            _player.Session.PacketBuilder.SendSkillUpdate(i, skill.Experience, skill.Level);
        }
    }

    public void RefreshSkill(SkillType type)
    {
        var skill = Skills[(int)type];
        _player.Session.PacketBuilder.SendSkillUpdate((int)type, skill.Experience, _player.CurrentHealth);
    }

    public int GetSkillLevel(SkillType type)
    {
        return Skills[(int)type].Level;
    }
    
    public static int GetCombatLevel(double atk, double str, double mag, double rng, double def, double hp, double pry)
    {
        // Source: http://runescape.wikia.com/wiki/Combat_level#History
        double k = 1.3d * Math.Max(atk + str, Math.Max(2 * mag, 2 * rng));
        double total = k + def + hp + Math.Floor(0.5d * pry);
        return (int)Math.Floor(total / 4);
    }
}