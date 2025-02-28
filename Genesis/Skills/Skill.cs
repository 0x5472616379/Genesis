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

    public void AddExperience(int experience)
    {
        Experience += experience;
    }
}