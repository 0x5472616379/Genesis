using ArcticRS.Constants;
using Genesis.Entities;

namespace ArcticRS.Commands;

public abstract class CommandBase
{
    protected Player Player { get; }
    protected string[] Args { get; }

    protected CommandBase(Player player, string[] args)
    {
        Player = player;
        Args = args;
    }

    public void Execute()
    {
        if (!HasRequiredRights())
        {
            Player.Session.PacketBuilder.SendMessage($"You do not have the rights to use the {Args[0]} command.");
            return;
        }

        var validationError = ValidateArgs();
        if (!string.IsNullOrEmpty(validationError))
        {
            Player.Session.PacketBuilder.SendMessage($"{validationError}");
            return;
        }

        try
        {
            Invoke();
        }
        catch (Exception ex)
        {
            Player.Session.PacketBuilder.SendMessage($"Error executing command: {ex.Message}");
        }
    }

    protected virtual PlayerRights RequiredRights => PlayerRights.NORMAL;
    protected virtual bool HasRequiredRights() => Player.Attributes.Rights >= RequiredRights;

    protected abstract string ValidateArgs();
    protected abstract void Invoke();
}