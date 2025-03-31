using Genesis.Entities.Player;
using Genesis.Model;

namespace Genesis.Skills.Combat.Specials;

public interface ISpecialAttack
{
    /// <summary>
    /// Executes the behavior of a special attack.
    /// </summary>
    /// <param name="player">The player executing the attack.</param>
    /// <param name="target">The target of the attack.</param>
    /// <param name="currentTick">The current game tick.</param>
    void Execute(Player player, Player target, int currentTick, Weapon weaponData);

    /// <summary>
    /// Checks whether the special attack can be executed in the current context.
    /// </summary>
    /// <param name="player">The player initiating the attack.</param>
    /// <returns>True if the attack can execute, false otherwise.</returns>
    bool CanExecute(Player player);
}
