namespace ArcticRS.Actions;

public abstract class RSAction
{
    public ActionPriority Priority { get; protected set; }
    public int ScheduledTick { get; set; }
    public abstract bool Execute(); // Returns true when completed
}
