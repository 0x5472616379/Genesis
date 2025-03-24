using Genesis.Model;

namespace Genesis.Skills.Combat.Specials;

public class SpecialAttack
{
    public int Cost { get; set; }
    public Gfx Gfx { get; set; }
    public int Animation { get; set; }

    public SpecialAttack(int cost, Gfx gfx, int animation)
    {
        Cost = cost;
        Gfx = gfx;
        Animation = animation;
    }
}

public class SpecialAttacks
{
    public static SpecialAttack MAGIC_SHORTBOW = new(5, new Gfx(256, 90, 0), 1074);
}