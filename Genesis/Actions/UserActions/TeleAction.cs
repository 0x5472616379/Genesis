using Genesis.Entities;
using Genesis.Environment;

namespace ArcticRS.Actions;

public class TeleAction : RSAction
{
    private readonly Player _player;
    private readonly Location _destination;
    private TeleportPhase _currentPhase;

    public TeleAction(Player player, Location destination)
    {
        _player = player;
        _destination = destination;
        Priority = ActionPriority.Unstoppable;
        ScheduledTick = World.CurrentTick;
        _currentPhase = TeleportPhase.Initiate;
    }

    public override bool Execute()
    {
        switch (_currentPhase)
        {
            case TeleportPhase.Initiate: // Start teleport process
                StartTeleport();
                ScheduleNext(2);
                _currentPhase = TeleportPhase.Teleport;
                return false;

            case TeleportPhase.Teleport: // Perform actual teleport
                MovePlayer();
                ScheduleNext(1);
                _currentPhase = TeleportPhase.Complete;
                return false;

            case TeleportPhase.Complete: // Finalize teleport process
                CompleteTeleport();
                return true; // Mark for removal from queue

            default:
                return true;
        }
    }

    private void StartTeleport()
    {
        _player.SetCurrentAnimation(714);
        _player.SetCurrentGfx(301);
        _player.PlayerMovementHandler.Reset();
        _player.NormalDelayTicks = 4; /* Should be 3 according to docs, but 4 looks and feels better */
    }

    private void MovePlayer()
    {
        _player.Location.X = _destination.X;
        _player.Location.Y = _destination.Y;
        _player.Location.Z = _destination.Z;
        _player.PerformedTeleport = true;
        _player.Location.Build();
        _player.Session.PacketBuilder.SendNewBuildAreaPacket();
        EnvironmentBuilder.UpdateBuildArea(_player);
    }

    private void CompleteTeleport()
    {
        _player.SetCurrentAnimation(-1);
        _player.SetCurrentGfx(-1);
        _player.Session.PacketBuilder.SendMessage("You arrive at your location.");
    }

    private void ScheduleNext(int ticks)
    {
        ScheduledTick = World.CurrentTick + ticks;
    }

    private enum TeleportPhase
    {
        Initiate, 
        Teleport,
        Complete
    }
}