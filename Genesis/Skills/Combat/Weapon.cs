﻿using Genesis.Model;

namespace Genesis.Skills.Combat;

public class Weapon
{
    public int Id { get; set; }
    public int AttackSpeed { get; set; }
    public int AttackerAnim { get; set; }
    public Weapon(int id, int attackSpeed, int attackerAnim)
    {
        Id = id;
        AttackSpeed = attackSpeed;
        AttackerAnim = attackerAnim;
    }
}