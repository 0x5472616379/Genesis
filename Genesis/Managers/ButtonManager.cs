using ArcticRS.Actions;
using ArcticRS.Commands;
using Genesis.Configuration;
using Genesis.Constants;
using Genesis.Entities.Player;
using Genesis.Environment;
using Genesis.Skills.Combat;
using Genesis.Skills.Combat.Specials;

namespace Genesis.Managers;

public class ButtonManager
{
    private readonly Dictionary<ButtonId, Action<Player>> _buttonActions;
    private readonly Dictionary<int, FightMode> _fightModeMappings;


    public ButtonManager()
    {
        _buttonActions = new Dictionary<ButtonId, Action<Player>>
        {
            { ButtonId.VARROCK_TELEPORT, HandleVarrockTeleport }
        };

        _fightModeMappings = new Dictionary<int, FightMode>
        {
            // Accurate
            { 9125, FightMode.ACCURATE },
            { 6221, FightMode.ACCURATE },
            { 22228, FightMode.ACCURATE },
            { 48010, FightMode.ACCURATE },
            { 21200, FightMode.ACCURATE },
            { 1080, FightMode.ACCURATE },
            { 6168, FightMode.ACCURATE },
            { 6236, FightMode.ACCURATE },
            { 17102, FightMode.ACCURATE },
            { 8234, FightMode.ACCURATE },

            // Defensive
            { 9126, FightMode.DEFENSIVE },
            { 48008, FightMode.DEFENSIVE },
            { 21201, FightMode.DEFENSIVE },
            { 1078, FightMode.DEFENSIVE },
            { 6169, FightMode.DEFENSIVE },
            { 33019, FightMode.DEFENSIVE },
            { 18078, FightMode.DEFENSIVE },
            { 8235, FightMode.DEFENSIVE },
            { 22229, FightMode.DEFENSIVE },

            // Controlled
            { 9127, FightMode.CONTROLLED },
            { 48009, FightMode.CONTROLLED },
            { 33018, FightMode.CONTROLLED },
            { 6234, FightMode.CONTROLLED },
            { 6219, FightMode.CONTROLLED },
            { 18077, FightMode.CONTROLLED },
            { 18080, FightMode.CONTROLLED },
            { 18079, FightMode.CONTROLLED },
            { 17100, FightMode.CONTROLLED },

            // Aggressive
            { 9128, FightMode.AGGRESSIVE },
            { 6220, FightMode.AGGRESSIVE },
            { 22230, FightMode.AGGRESSIVE },
            { 21203, FightMode.AGGRESSIVE },
            { 21202, FightMode.AGGRESSIVE },
            { 1079, FightMode.AGGRESSIVE },
            { 6171, FightMode.AGGRESSIVE },
            { 6170, FightMode.AGGRESSIVE },
            { 33020, FightMode.AGGRESSIVE },
            { 6235, FightMode.AGGRESSIVE },
            { 17101, FightMode.AGGRESSIVE },
            { 8237, FightMode.AGGRESSIVE },
            { 8236, FightMode.AGGRESSIVE }
        };
    }

    public void HandleButtonClick(Player player, int buttonId)
    {
        // Check if the buttonId corresponds to a special attack
        if (_specialAttackMappings.TryGetValue(buttonId, out var specialMapping))
        {
            var (specialAttack, baseSpecBarId) = specialMapping;

            // Invert behavior: Check if the current special attack matches the clicked one
            if (player.CombatHelper.SpecialAttack != null &&
                player.CombatHelper.SpecialAttack.GetType() == specialAttack.GetType())
            {
                // Disable special attack if it matches the current one
                player.CombatHelper.SpecialAttack = null;
                Console.WriteLine("Special attack disabled.");
            }
            else
            {
                // Enable the selected special attack
                player.CombatHelper.SpecialAttack = specialAttack;
            }

            // Always update the special attack bar, even when disabling
            player.CombatHelper.UpdateSpecialAttack(baseSpecBarId);
            return;
        }

        // Handle other buttons not related to special attacks
        if (Enum.IsDefined(typeof(ButtonId), buttonId) &&
            _buttonActions.TryGetValue((ButtonId)buttonId, out var action))
        {
            action(player);
        }
        else
        {
            Console.WriteLine($"Unknown button ID: {buttonId}");
        }
    }

    private void HandleVarrockTeleport(Player player)
    {
        if (player.IsDelayed)
            return;

        TeleportCommand.NamedLocations.TryGetValue("Varrock", out var varrock);
        player.ActionHandler.AddAction(
            new TeleAction(player, new Location(varrock.Item1, varrock.Item2, varrock.Item3)));
    }

    private readonly Dictionary<int, (ISpecialAttack Attack, int BaseSpecBarId)> _specialAttackMappings = new()
    {
        //GameInterfaces.DragonDaggerDefaultSpecialBar
        {
            29138, (new DragonDaggerSpecialAttack(), GameInterfaces.DragonDaggerDefaultSpecialBar)
        }
    };
}