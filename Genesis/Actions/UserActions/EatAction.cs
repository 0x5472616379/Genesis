using Genesis.Entities;

namespace ArcticRS.Actions;

enum EatState
{
    IDLE,
    EATING
}

// public class EatAction : RSAction
// {
//     private readonly Player _player;
//
//     public override ActionCategory Category { get; set; } = ActionCategory.EAT;
//     public override Func<bool> CanExecute { get; set; }
//     public override Func<bool> Execute { get; set; }
//
//     private EatState _currentState;
//
//     public EatAction(Player player)
//     {
//         _player = player;
//         CanExecute = CanEat;
//         Execute = Invoke;
//     }
//
//     private bool Invoke()
//     {
//         switch (_currentState)
//         {
//             case EatState.IDLE:
//                 _player.SetCurrentAnimation(829);
//                 _player.Session.PacketBuilder.SendMessage("You start eating a salmon.");
//
//                 SetDelay(1);
//                 _currentState = EatState.EATING;
//                 return false;
//
//             case EatState.EATING:
//                 if (IsDelayed()) return false;
//
//                 _player.Session.PacketBuilder.SendMessage("You finish eating the salmon.");
//
//                 _currentState = EatState.IDLE;
//                 return true;
//
//             default:
//                 throw new InvalidOperationException($"Unhandled state: {_currentState}");
//         }
//     }
//
//     private bool CanEat()
//     {
//         return true;
//     }
// }