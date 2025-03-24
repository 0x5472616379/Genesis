using ArcticRS.Actions;
using ArcticRS.Appearance;
using Genesis.Configuration;
using Genesis.Definitions;
using Genesis.Entities;
using Genesis.Model;
using Genesis.Movement;
using Genesis.Skills.Combat.Maxhit;

namespace Genesis.Skills.Combat;

public enum CombatStyle
{
    Melee,
    Ranged,
    Magic
}

public class CombatHelper
{
    private readonly Player _player;
    private static readonly Random _random = new();

    public CombatStyle CombatStyle { get; set; } = CombatStyle.Melee;
    
    public int LastAttackTick { get; private set; } = -1;
    public Weapon AttackedWith { get; private set; }

    public CombatHelper(Player player)
    {
        _player = player;
    }

    public bool Attack(Player target, int currentTick)
    {
        if (!ValidateCombatDistance(target, CombatStyle))
            return false;

        if (!CanAttack(currentTick))
            return false;

        var weapon = GetEquippedWeapon();
        var weaponData = GetWeaponData(weapon.ItemId);

        if (TryRangedAttack(target, currentTick, weapon, weaponData))
            return false;

        PerformMeleeAttack(target, currentTick, weaponData);
        return false;
    }

    private bool CanAttack(int currentTick)
    {
        return LastAttackTick == -1 || (currentTick - LastAttackTick) >= AttackedWith?.AttackSpeed;
    }

    private bool TryRangedAttack(Player target, int currentTick, ItemSlot weapon, Weapon weaponData)
    {
        return GameConstants.IsShortbow(weapon.ItemId) || GameConstants.IsLongbow(weapon.ItemId)
            ? HandleBowAttack(target, currentTick, weaponData)
            : GameConstants.IsThrowingKnife(weapon.ItemId)
                ? HandleThrowingAttack(target, currentTick, weaponData)
                : GameConstants.IsDart(weapon.ItemId)
                    ? HandleDartAttack(target, currentTick, weaponData)
                    : false;
    }

    public bool ValidateCombatDistance(Player target, CombatStyle style)
    {
        return style switch
        {
            CombatStyle.Ranged => ValidateRangedDistance(target),
            CombatStyle.Melee => ValidateMeleeDistance(target),
            _ => false
        };
    }
    
    private bool ValidateRangedDistance(Player target)
    {
        var distance = CalculateDistance(target);
        
        if (distance > (int)CombatDistances.Magic)
        {
            HandleDistanceTooFar(target);
            return false;
        }

        return HasClearProjectilePath(target);
    }

    private bool ValidateMeleeDistance(Player target)
    {
        var distance = CalculateDistance(target);
        
        if (distance > (int)CombatDistances.Melee)
        {
            HandleMeleeFollow(target);
            return false;
        }

        return IsValidMeleePosition(target);
    }
    
    private void HandleDistanceTooFar(Player target)
    {
        if (CalculateDistance(target) > 20)
        {
            ResetInteraction();
            return;
        }

        FindPathToTarget(target.Location.X, target.Location.Y);
    }

    private void HandleMeleeFollow(Player target)
    {
        RSPathfinder.MeleeFollow(_player, target);
        ProcessMovement();
    }

    private bool HasClearProjectilePath(Player target)
    {
        return RSPathfinder.IsProjectilePathClear(
            _player, 
            _player.Location.X, _player.Location.Y, _player.Location.Z,
            target.Location.X, target.Location.Y
        );
    }
    private void ResetInteraction()
    {
        _player.CurrentInteraction = null;
        _player.SetFacingEntity(null);
        _player.PlayerMovementHandler.Reset();
    }

    private void FindPathToTarget(int x, int y)
    {
        _player.PlayerMovementHandler.Reset();
        RSPathfinder.FindPath(_player, x, y, true, 1, 1);
        ProcessMovement();
    }

    private void ProcessMovement()
    {
        _player.PlayerMovementHandler.Finish();
        _player.PlayerMovementHandler.Process();
        _player.PlayerMovementHandler.Reset();
    }

    
    private bool IsValidMeleePosition(Player target)
    {
        return MeleePathing.IsLongMeleeDistanceClear(
            _player,
            _player.Location.X, _player.Location.Y, _player.Location.Z,
            target.Location.X, target.Location.Y, 2
        ) && !MeleePathing.IsDiagonal(
            _player.Location.X, _player.Location.Y,
            target.Location.X, target.Location.Y
        );
    }
    
    private void PerformMeleeAttack(Player target, int currentTick, Weapon weaponData)
    {
        CombatStyle = CombatStyle.Melee;
        // ValidateCombatDistance(target, CombatStyle.Melee);
        _player.SetCurrentAnimation(weaponData.AttackerAnim);
        QueueDamageAction(target);
        UpdateAttackState(currentTick, weaponData);
    }
    
    private bool HandleBowAttack(Player target, int currentTick, Weapon weaponData)
    {
        CombatStyle = CombatStyle.Ranged;
        
        var arrow = _player.Equipment.GetItemInSlot(EquipmentSlot.Ammo);
        if (arrow == null) return false;

        // ValidateCombatDistance(target, CombatStyle.Ranged);
        
        SetupAttack(weaponData, GetArrowGraphics(arrow.ItemId));
        CreateProjectile(target, GetArrowProjectile(arrow.ItemId));
        QueueDamageAction(target, CalculateRangeDelay(target));
        UpdateAttackState(currentTick, weaponData);
        return true;
    }

    private bool HandleThrowingAttack(Player target, int currentTick, Weapon weapon)
    {
        CombatStyle = CombatStyle.Ranged;
        // ValidateCombatDistance(target, CombatStyle.Ranged);
        SetupAttack(GetWeaponData(weapon.Id), GetThrowingKnifeGraphics(weapon.Id));
        CreateProjectile(target, GetThrowingKnifeProjectile(weapon.Id));
        QueueDamageAction(target, CalculateThrowingDelay(target));
        UpdateAttackState(currentTick, GetWeaponData(weapon.Id));
        return true;
    }

    private bool HandleDartAttack(Player target, int currentTick, Weapon weapon)
    {
        CombatStyle = CombatStyle.Ranged;
        // ValidateCombatDistance(target, CombatStyle.Ranged);
        SetupAttack(GetWeaponData(weapon.Id), GetDartGraphics(weapon.Id));
        CreateProjectile(target, GetDartProjectile(weapon.Id));
        QueueDamageAction(target, 2); // Fixed dart delay
        UpdateAttackState(currentTick, GetWeaponData(weapon.Id));
        return true;
    }

    public Gfx GetArrowGraphics(int itemId)
    {
        return new Gfx(GameConstants.GetArrowPullbackGfx(itemId), 90, 0);
    }

    public int GetArrowProjectile(int itemId)
    {
        return GameConstants.GetArrowProjectile(itemId);
    }

    public Gfx GetThrowingKnifeGraphics(int itemId)
    {
        return new Gfx(GameConstants.GetThrowingKnifePullbackGfx(itemId), 90, 0);
    }

    public int GetThrowingKnifeProjectile(int itemId)
    {
        return GameConstants.GetThrowingKnifeProjectile(itemId);
    }

    public Gfx GetDartGraphics(int itemId)
    {
        return new Gfx(GameConstants.GetDartPullbackGfx(itemId), 90, 0);
    }

    public int GetDartProjectile(int itemId)
    {
        return GameConstants.GetDartProjectile(itemId);
    }

    private void SetupAttack(Weapon weaponData, Gfx graphics)
    {
        _player.SetCurrentAnimation(weaponData.AttackerAnim);
        _player.SetCurrentGfx(graphics);
    }

    private void CreateProjectile(Player target, int projectileId)
    {
        ProjectileCreator.CreateProjectile(_player, target, projectileId);
    }

    private void QueueDamageAction(Player target, int delay = 0)
    {
        var damage = CalculateDamage(target);
        target.ActionHandler.AddAction(new DamageAction(target, damage, delay));
    }

    private Damage CalculateDamage(Player target)
    {
        var attackBonus = _player.BonusManager.GetTotalForBonusType(BonusType.RangeAttack);
        var defenceBonus = target.BonusManager.GetTotalForBonusType(BonusType.RangeDefence);

        var playerRangeLevel = _player.SkillManager.Skills[(int)SkillType.RANGED].Level;
        var targetDefenceLevel = target.SkillManager.Skills[(int)SkillType.DEFENCE].Level;

        int attackRoll = (playerRangeLevel + 8) * (attackBonus + 64);
        int defenceRoll = (targetDefenceLevel + 9) * (defenceBonus + 64);
        double hitChance = (double)attackRoll / (attackRoll + defenceRoll);
        double hitChancePercentage = hitChance * 100;

        _player.Session.PacketBuilder.SendMessage("Hit Chance: " + hitChancePercentage + "%");

        bool isHit = _random.NextDouble() < hitChance;
        if (!isHit)
        {
            return new Damage(DamageType.BLOCK, 0, null);
        }

        int damageValue = _random.Next(1, MaxHitCalculator.GetRangeMaxHit(_player) + 1);
        return new Damage(DamageType.HIT, damageValue, null);
    }

    private void UpdateAttackState(int currentTick, Weapon weapon)
    {
        LastAttackTick = currentTick;
        AttackedWith = weapon;
    }

    private int CalculateThrowingDelay(Player target)
    {
        return (int)CalculateDistance(target);
    }

    private int CalculateRangeDelay(Player target)
    {
        int distance = (int)CalculateDistance(target);
        return distance switch
        {
            1 or 2 => 2,
            >= 3 and <= 8 => 3,
            _ => 4
        };
    }

    private double CalculateDistance(Player target)
    {
        return MovementHelper.EuclideanDistance(
            _player.Location.X, _player.Location.Y,
            target.Location.X, target.Location.Y
        );
    }

    private ItemSlot GetEquippedWeapon()
    {
        return _player.Equipment.GetItemInSlot(EquipmentSlot.Weapon);
    }

    private Weapon GetWeaponData(int itemId)
    {
        return WeaponBuilder.GetWeaponData(_player, itemId);
    }
}