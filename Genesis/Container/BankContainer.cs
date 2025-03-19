namespace Genesis.Container;

public class BankContainer : RSContainer
{
    public override bool AlwaysStack { get; } = true;

    public BankContainer(int maxSize) : base(maxSize)
    {
    }
}