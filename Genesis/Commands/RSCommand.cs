using ArcticRS.Constants;
using Genesis.Entities.Player;

namespace Genesis.Commands;

public abstract class RSCommand
{
    protected abstract PlayerRights RequiredRights { get; }
    public string[] Args { get; }
    public Player Player { get; }

    protected RSCommand(Player player, string[] args) => (Player, Args) = (player, args);

    public void Execute()
    {
        if (!HasRequiredRights())
        {
            Player.Session.PacketBuilder.SendMessage($"You do not have the rights to use the {Args[0]} command.");
            return;
        }

        if (Validate()) Invoke();
    }

    protected bool HasRequiredRights() => Player.Attributes.Rights >= RequiredRights;
    public abstract bool Validate();
    public abstract void Invoke();

}