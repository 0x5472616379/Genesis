namespace Genesis.Skills.Combat;

public class Damage
{
    public DamageType Type { get; set; }
    public int Amount { get; set; }

    public Damage(DamageType type, int amount)
    {
        Type = type;
        Amount = amount;
    }
}