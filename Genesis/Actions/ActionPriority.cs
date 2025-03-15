namespace ArcticRS.Actions;


/// <summary>
/// Describes the priority levels for actions and their behavior in the queue.
/// </summary>
public enum ActionPriority 
{
    /// <summary>
    /// Interruptible ("Weak"):
    /// - Removed from the queue if any strong scripts are present before processing.
    /// - Removed on interruptions, including:
    ///     - Interacting with an entity or clicking on a game square.
    ///     - Interacting with an item in the inventory.
    ///     - Unequipping an item.
    ///     - Opening or closing an interface.
    ///     - Dragging items in the inventory.
    /// - Any action that closes an interface also clears all weak scripts.
    /// </summary>
    Interruptible,   // "Weak"

    /// <summary>
    /// Standard ("Normal"):
    /// - Skipped during execution if the player has a modal interface open at the time.
    /// </summary>
    Standard,        // "Normal"

    /// <summary>
    /// Forceful ("Strong"):
    /// - Removes all weak scripts from the queue before being processed.
    /// - Closes any modal interface prior to execution.
    /// </summary>
    Forceful,        // "Strong"

    /// <summary>
    /// Unstoppable ("Soft"):
    /// - Cannot be paused or interrupted and will always execute as long as the timer allows.
    /// - Closes any modal interface prior to execution.
    /// </summary>
    Unstoppable      // "Soft"
}