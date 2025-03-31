using ArcticRS.Constants;
using Genesis.Entities.Player;
using Genesis.Skills;

namespace Genesis.Commands;

public class SetLevelCommand : RSCommand
{
    private int _id;
    private int _level;

    protected override PlayerRights RequiredRights => PlayerRights.ADMIN; // Adjust as needed
    public SetLevelCommand(Player player, string[] args) : base(player, args)
    {
    }

    public override bool Validate()
    {
        if (Args.Length < 3)
        {
            Player.Session.PacketBuilder.SendMessage("Invalid syntax! Try ::setlvl <skill_id> <level>");
            return false;
        }

        if (!int.TryParse(Args[1], out _id) || _id < 0 || _id >= Enum.GetValues(typeof(SkillType)).Length)
        {
            Player.Session.PacketBuilder.SendMessage("Invalid skill ID! Use a valid skill ID (e.g., 0 for Attack, 1 for Defence).");
            return false;
        }

        if (!int.TryParse(Args[2], out _level) || _level < 1 || _level > 99) // Assuming level range is from 1 to 99
        {
            Player.Session.PacketBuilder.SendMessage("Invalid level! Skill levels must be between 1 and 99.");
            return false;
        }

        return true;
    }

    public override void Invoke()
    {
        // Set the skill level for the specified skill
        SkillType skill = (SkillType)_id; // Convert skill ID to the corresponding SkillType enum
        Player.SkillManager.Skills[(int)skill].SetLevel(_level);

        Player.SkillManager.RefreshSkills();
        // Optionally, send feedback to the player
        Player.Session.PacketBuilder.SendMessage($"Successfully set {skill} level to {_level}.");
    }
}