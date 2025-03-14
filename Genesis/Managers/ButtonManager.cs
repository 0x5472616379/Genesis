using ArcticRS.Actions;
using ArcticRS.Commands;
using Genesis.Constants;
using Genesis.Entities;
using Genesis.Environment;

namespace Genesis.Managers;

public class ButtonManager
{
    private readonly Dictionary<ButtonId, Action<Player>> _buttonActions;

    public ButtonManager()
    {
        _buttonActions = new Dictionary<ButtonId, Action<Player>>
        {
            { ButtonId.VARROCK_TELEPORT, HandleVarrockTeleport }
        };
    }
    
    public void HandleButtonClick(Player player, int buttonId)
    {
        if (player.IsDelayed)
            return;

        if (Enum.IsDefined(typeof(ButtonId), buttonId) && _buttonActions.TryGetValue((ButtonId)buttonId, out var action))
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
        TeleportCommand.NamedLocations.TryGetValue("Varrock", out var varrock);
        player.ActionHandler.AddAction(new TeleAction(player, new Location(varrock.Item1, varrock.Item2, varrock.Item3)));
    }
}