using Genesis.Entities;
using Genesis.Environment;

namespace ArcticRS.Actions;

enum TeleportState
{
    Initiate,    // The teleport process is starting
    Teleporting, // During teleport (e.g., animations, effects)
    Arrived      // Teleportation is complete
}

// public class TeleportAction : RSAction
// {
//     private readonly Player _player;
//     private readonly Location _toLocation;
//     private readonly bool _isInstant;
//     public override ActionCategory Category { get; set; }
//     public override Func<bool> CanExecute { get; set; }
//     public override Func<bool> Execute { get; set; }
//
//     private TeleportState _currentState = TeleportState.Initiate; // Initial state
//
//     public TeleportAction(Player player, Location toLocation, bool isInstant = false)
//     {
//         _player = player;
//         _toLocation = toLocation;
//         _isInstant = isInstant;
//         CanExecute = CanTeleport;
//         Execute = Invoke;
//     }
//
//     private bool Invoke()
//     {
//         _player.PlayerMovementHandler.Reset();
//         if (_isInstant)
//         {
//             _player.Location.X = _toLocation.X;
//             _player.Location.Y = _toLocation.Y;
//             _player.Location.Z = _toLocation.Z;
//             _player.PerformedTeleport = true;
//
//             _player.Location.Build();
//             _player.Session.PacketBuilder.SendNewBuildAreaPacket();
//             EnvironmentBuilder.UpdateBuildArea(_player);
//             _player.PlayerMovementHandler.DiscardMovementQueue = true;
//             return true;
//         }
//         
//         switch (_currentState)
//         {
//             case TeleportState.Initiate:
//                 // Delay logic is automatically handled by RSAction's SetDelay/IsDelayed
//                 if (IsDelayed()) return false; // Wait for the current delay to complete
//                 
//                 // Begin the teleport animation and graphics
//                 _player.SetCurrentGfx(301);       // Start graphic effect for teleport
//                 _player.SetCurrentAnimation(714); // Start teleport animation
//
//                 // Set a delay of 3 ticks before transitioning
//                 SetDelay(2);
//                 _currentState = TeleportState.Teleporting; // Move to the next state
//                 return false; // Still in progress
//
//             case TeleportState.Teleporting:
//                 // Teleport the player to the target location
//                 _player.Location.X = _toLocation.X;
//                 _player.Location.Y = _toLocation.Y;
//                 _player.Location.Z = _toLocation.Z;
//                 _player.PerformedTeleport = true;
//
//                 _player.Location.Build();
//                 _player.Session.PacketBuilder.SendNewBuildAreaPacket();
//                 // _player.Location.RefreshObjects(_player);
//                 EnvironmentBuilder.UpdateBuildArea(_player);
//                 _player.PlayerMovementHandler.DiscardMovementQueue = true; // Clear movement queue
//                 
//                 _currentState = TeleportState.Arrived; // Transition to the final state
//                 return false; // Still in progress
//
//             case TeleportState.Arrived:
//                 // Notify the player of arrival
//                 _player.Session.PacketBuilder.SendMessage("You have arrived at your destination.");
//                 _player.SetCurrentAnimation(715); // Stop the player's animation
//                 
//                 return true; // Indicate teleportation is complete
//
//             default:
//                 throw new InvalidOperationException($"Unhandled state: {_currentState}");
//         }
//     }
//
//     private bool CanTeleport()
//     {
//         return true;
//     }
// }