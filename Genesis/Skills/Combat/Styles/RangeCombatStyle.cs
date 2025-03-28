using ArcticRS.Actions;
using ArcticRS.Appearance;
using Genesis.Configuration;
using Genesis.Definitions;
using Genesis.Entities;
using Genesis.Model;
using Genesis.Movement;
using Genesis.Skills.Combat.Maxhit;

namespace Genesis.Skills.Combat;

public class RangedCombatStyle : ICombatStyle
{
    private static readonly Random _random = new();

    public bool CanAttack(Player player, Player target, int currentTick)
    {
        var weapon = player.CombatHelper.GetEquippedWeapon();
        var weaponData = WeaponBuilder.GetWeaponData(player, weapon.ItemId);

        return currentTick - player.CombatHelper.LastAttackTick >= weaponData.AttackSpeed;
    }

    public void Attack(Player player, Player target, int currentTick)
    {
        var weapon = player.CombatHelper.GetEquippedWeapon();
        var weaponData = WeaponBuilder.GetWeaponData(player, weapon.ItemId);

        var arrow = player.Equipment.GetItemInSlot(EquipmentSlot.Ammo);

        if (arrow == null) throw new InvalidOperationException("No ammunition equipped!");

        player.SetCurrentAnimation(weaponData.AttackerAnim);
        player.SetCurrentGfx(new Gfx(GameConstants.GetArrowPullbackGfx(arrow.ItemId), 90, 0));
        ProjectileCreator.CreateProjectile(player, target, GameConstants.GetArrowProjectile(arrow.ItemId), sY:100);

        var damage = CalculateDamage(player, target);

        target.ActionHandler.AddAction(new DamageAction(target, player, damage, CalculateDelay(player, target)));
        player.CombatHelper.UpdateAttackState(currentTick, weaponData);
    }

    public bool ValidateDistance(Player player, Player target)
    {
        var distance = MovementHelper.EuclideanDistance(player.Location.X, player.Location.Y, target.Location.X,
            target.Location.Y);

        if (distance > (int)CombatDistances.Range - 1)
        {
            player.PlayerMovementHandler.Reset();
            RSPathfinder.MeleeFollow(player, target);
            player.PlayerMovementHandler.Finish();
            player.PlayerMovementHandler.Process();
            player.PlayerMovementHandler.Reset();
        }
        
        if (distance <= 0)
        {
            player.PlayerMovementHandler.Reset();
            RSPathfinder.MeleeFollow(player, target);
            player.PlayerMovementHandler.Finish();
            player.PlayerMovementHandler.Process();
            player.PlayerMovementHandler.Reset();
        }

        if (distance <= (int)CombatDistances.Range && RSPathfinder.IsProjectilePathClear(player, player.Location.X, player.Location.Y,
                player.Location.Z,
                target.Location.X, target.Location.Y))
        {
            return true;
        }

        return false;
    }

    private Damage CalculateDamage(Player player, Player target)
    {
        var attackBonus = player.BonusManager.GetTotalForBonusType(BonusType.RangeAttack);
        var defenseBonus = target.BonusManager.GetTotalForBonusType(BonusType.RangeDefence);

        var attackRoll = (player.SkillManager.GetSkillLevel(SkillType.RANGED) + 8) * (attackBonus + 64);
        var defenseRoll = (target.SkillManager.GetSkillLevel(SkillType.DEFENCE) + 9) * (defenseBonus + 64);

        var hitChance = (double)attackRoll / (attackRoll + defenseRoll);
        if (_random.NextDouble() >= hitChance)
            return new Damage(DamageType.BLOCK, 0, null);

        int maxHit = MaxHitCalculator.GetRangeMaxHit(player);
        int damageValue = _random.Next(1, maxHit + 1);
        return new Damage(DamageType.HIT, damageValue, null);
    }

    private int CalculateDelay(Player player, Player target)
    {
        int distance = (int)MovementHelper.EuclideanDistance(player.Location.X, player.Location.Y, target.Location.X,
            target.Location.Y);
        return distance switch
        {
            1 or 2 => 2,
            >= 3 and <= 8 => 3,
            _ => 4
        };
    }
}