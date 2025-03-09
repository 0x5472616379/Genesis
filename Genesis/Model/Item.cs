namespace Genesis.Model;

public class Item
{
    public int Id { get; }
    public int Amount { get; set; }
    public bool Stackable { get; }

    public Item(int id, int amount = 1, bool stackable = false)
    {
        Id = id;
        Amount = amount;
        Stackable = stackable;
    }
}