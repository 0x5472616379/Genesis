namespace Genesis.Interactions;

public abstract class RSInteraction
{
    private int _tick;
    private readonly Action _action;

    public RSInteraction(int tick, Action action)
    {
        _tick = tick;
        _action = action;
    }

    public abstract bool Execute();
    public abstract bool CanExecute();
}