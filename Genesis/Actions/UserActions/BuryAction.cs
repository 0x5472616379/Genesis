using Genesis.Entities.Player;
using Genesis.Environment;

namespace ArcticRS.Actions;

public class BuryAction : RSAction
{
    private readonly Player _player;
    private BuryState _currentState;

    public BuryAction(Player player)
    {
        _player = player;
        Priority = ActionPriority.Forceful;
        ScheduledTick = World.CurrentTick;
        _currentState = BuryState.Digging; // Start with the initial state
    }

    public override bool Execute()
    {
        _player.CurrentInteraction = null;
        _player.PlayerMovementHandler.Reset();
        
        switch (_currentState)
        {
            case BuryState.Digging: // Initial digging phase
                StartDigging();
                ScheduleNext(1); // Schedule burying phase after 2 ticks
                _currentState = BuryState.Burying;
                return false;

            case BuryState.Burying: // Burying phase
                PerformBurying();
                return true; // Mark for removal after completion

            default:
                return true;
        }
    }

    private void StartDigging()
    {
        _player.SetCurrentAnimation(827); // Play digging animation
        _player.Session.PacketBuilder.SendMessage("You dig a hole in the ground...");
    }

    private void PerformBurying()
    {
        _player.Session.PacketBuilder.SendMessage("You bury the bones.");
    }

    private void ScheduleNext(int ticks)
    {
        ScheduledTick = World.CurrentTick + ticks; // Schedule the action for the next phase
    }

    private enum BuryState
    {
        Digging,   // Represents the initial dig action
        Burying    // Represents the action of burying the bones
    }
}


// public class BuryAction : RSAction
// {
//     private readonly Player _player;
//
//     public override ActionCategory Category { get; set; } = ActionCategory.BURY;
//     public override Func<bool> CanExecute { get; set; }
//     public override Func<bool> Execute { get; set; }
//
//     private BuryState _state = BuryState.Digging;
//
//     public BuryAction(Player player)
//     {
//         _player = player;
//         CanExecute = CanBury;
//         Execute = Invoke;
//     }
//
//     private bool Invoke()
//     {
//         switch (_state)
//         {
//             case BuryState.Digging:
//                 _player.SetCurrentAnimation(827);
//                 _player.Session.PacketBuilder.SendMessage("You dig a hole in the ground...");
//                 _state = BuryState.Burying;
//                 return false;
//
//             case BuryState.Burying:
//                 _player.Session.PacketBuilder.SendMessage("You bury the bones.");
//                 return true;
//
//             default:
//                 throw new InvalidOperationException($"Unhandled state: {_state}");
//         }
//     }
//
//     private bool CanBury()
//     {
//         return true;
//     }
// }