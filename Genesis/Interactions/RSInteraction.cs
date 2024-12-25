namespace Genesis.Interactions;

public class RSInteraction
{
    private int _tick;
    private readonly Action _action;
    public bool CanExecute { get; set; } = true;

    public RSInteraction(int tick, Action action)
    {
        _tick = tick;
        _action = action;
    }
    
    
    public bool Execute()
    {
        if (!CanExecute)
            return false;

        if (_tick <= 0)
        {
            _action.Invoke();
            return true;
        }
        
        _tick--;
        return false;
    }
}