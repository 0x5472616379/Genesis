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
    
    public bool AddExperience(int experienceToAdd)
    {
        // Store old level based on current experience
        int oldLevel = GetLevelForExperience(Experience, SkillManager.EXPERIENCE_TABLE);

        // Add the new experience
        Experience = Math.Min(Experience + experienceToAdd, SkillManager.MAX_EXPERIENCE);

        // Calculate new level based on updated experience
        int newLevel = GetLevelForExperience(Experience, SkillManager.EXPERIENCE_TABLE);

        // Check if the player leveled up
        if (newLevel > oldLevel)
        {
            Level = newLevel; // Update the displayed level
            return true; // Indicate a level-up occurred
        }

        return false; // No level-up
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