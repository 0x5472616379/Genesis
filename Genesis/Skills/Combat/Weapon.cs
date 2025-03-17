using Genesis.Model;

namespace Genesis.Skills.Combat;

public class Weapon
{
    public int Id { get; set; }
    public int AttackSpeed { get; set; }
    public int AttackerAnim { get; set; }
    public Gfx AttackerGfx { get; set; }
    public Gfx TargetGfx { get; set; }
    public int Delay { get; set; }

    public Weapon(int id, int attackSpeed, int attackerAnim, Gfx attackerGfx, Gfx targetGfx, int delay)
    {
        Id = id;
        AttackSpeed = attackSpeed;
        AttackerAnim = attackerAnim;
        AttackerGfx = attackerGfx;
        TargetGfx = targetGfx;
        Delay = delay;
    }
}