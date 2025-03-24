namespace Genesis.Definitions;

public class ItemRequirements
{
    public int Id { get; set; }
    public int[] Requirements { get; set; } = Array.Empty<int>();
}