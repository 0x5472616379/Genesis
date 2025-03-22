using ArcticRS.Actions;
using ArcticRS.Appearance;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Managers;
using Genesis.Model;
using Genesis.Movement;

namespace Genesis.Skills.Combat;

public class CombatHelper
{
    private readonly Player _player;
    private static readonly Random _random = new();

    public int LastAttackTick { get; private set; } = -1;
    public Weapon AttackedWith { get; private set; }

    public CombatHelper(Player player)
    {
        _player = player;
    }

    public bool Attack(Player target, int currentTick)
    {
        if (!CanAttack(currentTick))
            return false;

        var weapon = GetEquippedWeapon();
        var weaponData = GetWeaponData(weapon.ItemId);

        if (TryRangedAttack(target, currentTick, weapon, weaponData))
            return false;

        // PerformMeleeAttack(target, currentTick, weaponData);
        return false;
    }

    private bool CanAttack(int currentTick)
    {
        return LastAttackTick == -1 || (currentTick - LastAttackTick) >= AttackedWith?.AttackSpeed;
    }

    private bool TryRangedAttack(Player target, int currentTick, ItemSlot weapon, Weapon weaponData)
    {
        return GameConstants.IsShortbow(weapon.ItemId) || GameConstants.IsLongbow(weapon.ItemId) ? HandleBowAttack(target, currentTick, weaponData) 
             : GameConstants.IsThrowingKnife(weapon.ItemId) ? HandleThrowingAttack(target, currentTick, weaponData) 
             : GameConstants.IsDart(weapon.ItemId) ? HandleDartAttack(target, currentTick, weaponData) 
             : false;

        // GameConstants.IsThrowingKnife(weapon.ItemId) ? HandleThrowingAttack(target, currentTick, weapon) :
        //     GameConstants.IsDart(weapon.ItemId) ? HandleDartAttack(target, currentTick, weapon) :
    }

    private bool HandleBowAttack(Player target, int currentTick, Weapon weaponData)
    {
        var arrow = _player.Equipment.GetItemInSlot(EquipmentSlot.Ammo);
        if (arrow == null) return false;

        SetupAttack(weaponData, GetArrowGraphics(arrow.ItemId));
        CreateProjectile(target, GetArrowProjectile(arrow.ItemId));
        QueueDamageAction(target, CalculateRangeDelay(target));
        UpdateAttackState(currentTick, weaponData);
        return true;
    }

    private bool HandleThrowingAttack(Player target, int currentTick, Weapon weapon)
    {
        SetupAttack(GetWeaponData(weapon.Id), GetThrowingKnifeGraphics(weapon.Id));
        CreateProjectile(target, GetThrowingKnifeProjectile(weapon.Id));
        QueueDamageAction(target, CalculateThrowingDelay(target));
        UpdateAttackState(currentTick, GetWeaponData(weapon.Id));
        return true;
    }

    private bool HandleDartAttack(Player target, int currentTick, Weapon weapon)
    {
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
        var damage = CalculateDamage();
        target.ActionHandler.AddAction(new DamageAction(target, damage, delay));
    }

    private Damage CalculateDamage()
    {
        int damageValue = _random.Next(0, 2);
        return new Damage(damageValue == 0 ? DamageType.BLOCK : DamageType.HIT, damageValue, null);
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