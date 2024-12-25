namespace Genesis.Model;

public class RSItem
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public int Index { get; set; }
    public bool IsStackable { get; set; }

    public RSItem(int id, int amount, int index = -1, bool isStackable = false)
    {
        if (amount < 0) throw new ArgumentException("Amount cannot be negative.");
        
        Id = id;
        Amount = amount;
        Index = index;
        IsStackable = isStackable;
    }

    public void AddAmount(int amount)
    {
        if (amount < 0) throw new ArgumentException("Amount to add cannot be negative.");
        Amount += amount;
    }

    public void ReduceAmount(int amount)
    {
        if (amount < 0) throw new ArgumentException("Amount to remove cannot be negative.");
        if (amount > Amount) throw new InvalidOperationException("Cannot remove more than the current amount.");
        Amount -= amount;
    }
}