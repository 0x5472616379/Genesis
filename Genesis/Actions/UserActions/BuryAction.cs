using Genesis.Entities;

namespace ArcticRS.Actions;

enum BuryState
{
    Digging,   // Represents the initial dig action
    Burying    // Represents the action of burying the bones
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