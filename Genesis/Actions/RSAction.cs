namespace ArcticRS.Actions;

public abstract class RSAction
{
    private int _currentDelayTicks = 0; // Tracks elapsed ticks for the current delay
    private int _requiredDelayTicks = 0; // Tracks how many ticks are required for the delay to complete

    public abstract ActionCategory Category { get; set; }
    public abstract Func<bool> CanExecute { get; set; }
    public abstract Func<bool> Execute { get; set; }

    public bool ExecuteAction()
    {
        if (CanExecute())
        {
            // Check if delay is active and still running
            if (_currentDelayTicks < _requiredDelayTicks)
            {
                _currentDelayTicks++;
                return false; // Action is still "delayed"
            }

            // Reset the delay before proceeding to actual execution
            _currentDelayTicks = 0;
            _requiredDelayTicks = 0;

            // Run the action's execution logic
            return Execute();
        }

        return false;
    }

    // Helper to set a delay for the action
    protected void SetDelay(int ticks)
    {
        _requiredDelayTicks = ticks; // Specify how many ticks to delay
        _currentDelayTicks = 0;      // Reset current ticks
    }

    // Helper to check if a delay is currently active
    protected bool IsDelayed() => _currentDelayTicks < _requiredDelayTicks;
}
