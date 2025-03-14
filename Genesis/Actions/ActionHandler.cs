using System.Security.Cryptography;
using Genesis.Entities;
using Genesis.Environment;

namespace ArcticRS.Actions;

public class ActionHandler(Player player)
{
    public List<RSAction> ActionPipeline { get; } = new();

    public void ProcessActionPipeline()
    {
        // Pre-process forceful actions
        if (ActionPipeline.Any(a => a.Priority == ActionPriority.Forceful))
        {
            // CloseAllInterfaces();
            ActionPipeline.RemoveAll(a => a.Priority == ActionPriority.Interruptible);
        }

        bool processedAny;
        do
        {
            processedAny = false;
            for (int i = 0; i < ActionPipeline.Count; i++)
            {
                var action = ActionPipeline[i];

                // Skip actions scheduled for future ticks
                if (World.CurrentTick < action.ScheduledTick) continue;

                // Skip if player is delayed and action isn't Unstoppable
                if (player.IsDelayed && action.Priority != ActionPriority.Unstoppable)
                    continue;

                // Handle interface checks for Standard priority
                // if (action.Priority == ActionPriority.Standard && player.HasOpenInterface)
                //     continue;

                // Handle interface closing for Forceful/Unstoppable
                if (action.Priority is ActionPriority.Forceful or ActionPriority.Unstoppable)
                {
                    // player.CloseAllInterfaces();
                }

                try
                {
                    bool wasDelayed = player.IsDelayed;
                    bool actionCompleted = false;

                    // Execute the action
                    actionCompleted = action.Execute();

                    // Remove from pipeline if completed
                    if (actionCompleted)
                    {
                        ActionPipeline.RemoveAt(i);
                        i--; // Adjust index after removal
                    }

                    // Handle new delays set DURING execution
                    if (!wasDelayed && player.IsDelayed)
                    {
                        // Remove only Interruptible actions (Weak)
                        ActionPipeline.RemoveAll(a =>
                            a.Priority == ActionPriority.Interruptible);

                        // Don't return - continue processing Unstoppable
                        if (ActionPipeline.Any(a =>
                                a.Priority == ActionPriority.Unstoppable))
                        {
                            i = -1; // Restart loop for remaining actions
                            continue;
                        }

                        return;
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue processing other actions
                    // Logger.Error($"Action failed: {ex.Message}");
                    ActionPipeline.RemoveAt(i);
                    i--;
                }
            }
        } while (processedAny);
    }

    public void AddAction(RSAction action)
    {
        ActionPipeline.Add(action);
    }
}