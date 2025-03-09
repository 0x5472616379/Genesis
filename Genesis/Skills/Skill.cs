using Genesis.Configuration;
using Genesis.Entities;

namespace Genesis.Skills;

public class Skill
{
    public SkillType SkillType { get; set; }

    //Represents the boosted level
    public int Level { get; set; } = 1;

    //Represents the current level
    public int Experience { get; set; }

    public int ExpToLevel { get; set; }

    public Skill(SkillType skillType)
    {
        SkillType = skillType;
    }

    // public void AddExperience(double experience)
    // {
    //     Experience += (int)experience;
    // }
    
    public bool AddExperience(int experienceToAdd, Player player, SkillData skillData)
    {
        int oldLevel = GetLevelForExperience(Experience, SkillManager.EXPERIENCE_TABLE);

        Experience = Math.Min(Experience + experienceToAdd, SkillManager.MAX_EXPERIENCE);

        int newLevel = GetLevelForExperience(Experience, SkillManager.EXPERIENCE_TABLE);

        // Check if the player leveled up
        if (newLevel > oldLevel)
        {
            Level = newLevel; 
            player.Session.PacketBuilder.SendTextToInterface($"@dbl@Congratulations, you just advanced a {skillData.SkillType.ToTitleCase()} level." , skillData.SecondaryId);
            player.Session.PacketBuilder.SendTextToInterface($"Your {skillData.SkillType.ToTitleCase()} level is now {Level}.", skillData.TertiaryId);
            player.Session.PacketBuilder.SendChatInterface(skillData.PrimaryId);
            player.Session.PacketBuilder.SendMessage($"Congratulations! You've reached level {Level} in {SkillType.ToTitleCase()}!");
            player.SetCurrentGfx(199);
            return true;
        }

        return false;
    }

    private int GetLevelForExperience(int experience, int[] experienceTable)
    {
        for (int level = 1; level < experienceTable.Length - 1; level++)
        {
            // Check experience is within bounds for this level
            if (experience >= experienceTable[level] && experience < experienceTable[level + 1])
            {
                return level;
            }
        }

        // If experience exceeds all thresholds, return the max level
        return experienceTable.Length - 1;
    }
}