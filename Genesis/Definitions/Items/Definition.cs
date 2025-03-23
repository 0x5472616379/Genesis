namespace Genesis.Definitions;

public class Definition
{
    public double Weight { get; }
    public int[] Bonuses { get; }

    public Definition(ItemData item)
    {
        Weight = item.Weight;
        Bonuses = item.Bonuses?.GetBonuses();
    }

}