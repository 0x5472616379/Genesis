using Genesis.Entities.Player;
using Genesis.Movement;

namespace Genesis.Interactions;

public class MoveInteraction : RSInteraction
{
    private readonly Player _player;
    public override int MaxDistance { get; } = 0;
    public override InteractingEntity Target { get; set; } = new();


    public MoveInteraction(Player player, int destX, int destY)
    {
        _player = player;
        Target.X = destX;
        Target.Y = destY;
        Target.Z = _player.Location.Z;
    }

    public override bool Execute()
    {
        if (!CanExecute()) return false;

        /* Movement does not require any action delay ticks like wc and mining */
        _player.NormalDelayTicks = 0;
        return true;
    }

    public override bool CanExecute()
    {
        /* Check for protected access by modal? */
        
        return true;
    }
}