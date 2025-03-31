using Genesis.Entities.Player;

namespace Genesis.Skills.Combat;

public interface ICombatStyle
{
    bool CanAttack(Player player, Player target, int currentTick);
    void Attack(Player player, Player target, int currentTick);
    bool ValidateDistance(Player player, Player target);
}
