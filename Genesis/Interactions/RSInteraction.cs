namespace Genesis.Interactions;

public abstract class RSInteraction
{
    private int _tick;
    private readonly Action _action;

    public RSInteraction(Action action, int tick = 0)
    {
        _tick = tick;
        _action = action;
    }

    public abstract bool Execute();
    public abstract bool CanExecute();
}