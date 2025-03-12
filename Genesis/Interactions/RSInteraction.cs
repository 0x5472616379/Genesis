namespace Genesis.Interactions;

public abstract class RSInteraction
{
    public abstract int MaxDistance { get; }
    public abstract InteractingEntity Target { get; set; }
    public abstract bool Execute();
    public abstract bool CanExecute();
}

public class InteractingEntity
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
}