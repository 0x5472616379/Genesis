using Genesis.Model;

namespace Genesis.Skills.Combat;

public class Damage
{
    public DamageType Type { get; set; }
    public int Amount { get; set; }
    public Gfx Gfx { get; set; } /* Gfx applied to receiver on hit */

    public Damage(DamageType type, int amount, Gfx gfx)
    {
        Gfx = gfx;
        Type = type;
        Amount = amount;
    }
}