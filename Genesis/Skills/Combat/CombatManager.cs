using ArcticRS.Actions;
using ArcticRS.Appearance;
using Genesis.Configuration;
using Genesis.Entities.Player;
using Genesis.Environment;
using Genesis.Managers;
using Genesis.Model;
using Genesis.Movement;

namespace Genesis.Skills.Combat;

public class CombatManager
{
    private readonly Player _player;
    public int LastAttackTick { get; set; } = -1;
    public Weapon AttackedWith { get; set; }

    public CombatManager(Player player)
    {
        _player = player;
    }

    Random r = new();

    public bool Attack(Player target, int currentTick)
    {
        if (LastAttackTick == -1 || (currentTick - LastAttackTick) >= AttackedWith.AttackSpeed)
        {
            int dmg = r.Next(0, 2);
            var damage = new Damage(dmg == 0 ? DamageType.BLOCK : DamageType.HIT, dmg, null);

            /* Perform Attack */
            var equipped = _player.Equipment.GetItemInSlot(EquipmentSlot.Weapon);
            var weaponData = WeaponBuilder.GetWeaponData(_player, equipped.ItemId);

            if (GameConstants.IsShortbow(equipped.ItemId) || GameConstants.IsLongbow(equipped.ItemId))
            {
                var arrowEquipped = _player.Equipment.GetItemInSlot(EquipmentSlot.Ammo);
                _player.SetCurrentAnimation(weaponData.AttackerAnim);
                _player.SetCurrentGfx(new Gfx(GameConstants.GetArrowPullbackGfx(arrowEquipped.ItemId), 90, 0));
                ProjectileCreator.CreateProjectile(_player, target,
                    GameConstants.GetArrowProjectile(arrowEquipped.ItemId));
                AttackedWith = weaponData;
                LastAttackTick = currentTick;
                target.ActionHandler.AddAction(new DamageAction(target, _player, damage,
                    GetRangeDelay((int)GetDistanceToTarget(target.Location.X, target.Location.Y))));
                return false;
            }

            if (GameConstants.IsThrowingKnife(equipped.ItemId))
            {
                _player.SetCurrentAnimation(weaponData.AttackerAnim);
                _player.SetCurrentGfx(new Gfx(GameConstants.GetThrowingKnifePullbackGfx(equipped.ItemId), 90, 0));
                ProjectileCreator.CreateProjectile(_player, target,
                    GameConstants.GetThrowingKnifeProjectile(equipped.ItemId));
                AttackedWith = weaponData;
                LastAttackTick = currentTick;
                target.ActionHandler.AddAction(new DamageAction(target, _player, damage,
                    (int)GetDistanceToTarget(target.Location.X, target.Location.Y)));
                return false;
            }

            if (GameConstants.IsDart(equipped.ItemId))
            {
                _player.SetCurrentAnimation(weaponData.AttackerAnim);
                _player.SetCurrentGfx(new Gfx(GameConstants.GetDartPullbackGfx(equipped.ItemId), 90, 0));
                ProjectileCreator.CreateProjectile(_player, target, GameConstants.GetDartProjectile(equipped.ItemId));
                AttackedWith = weaponData;
                LastAttackTick = currentTick;
                target.ActionHandler.AddAction(new DamageAction(target, _player, damage, 2));
                return false;
            }

            /* Melee */
            _player.SetCurrentAnimation(weaponData.AttackerAnim);
            AttackedWith = weaponData;
            LastAttackTick = currentTick;
            target.ActionHandler.AddAction(new DamageAction(target, _player, damage));

            return false;
        }

        return false;
    }

    public bool InValidProjectileDistance(Player target)
    {
        int targetX = target.Location.X;
        int targetY = target.Location.Y;
        int targetZ = target.Location.Z;

        var distance = MovementHelper.EuclideanDistance(_player.Location.X, _player.Location.Y, targetX, targetY);

        if (distance > 20)
        {
            _player.CurrentInteraction = null;
            _player.SetFacingEntity(null);
            _player.PlayerMovementHandler.Reset();
            return false;
        }

        if (distance > (int)CombatDistances.Magic)
        {
            _player.PlayerMovementHandler.Reset();
            RSPathfinder.FindPath(_player, targetX, targetY, true, 1, 1);
            _player.PlayerMovementHandler.Finish();
            _player.PlayerMovementHandler.Process();
            _player.PlayerMovementHandler.Reset();
            return false;
        }

        /* If same tile step away */
        if (distance <= 0)
        {
            _player.PlayerMovementHandler.Reset();
            RSPathfinder.FindPath(_player, targetX, targetY, true, 1, 1);
            _player.PlayerMovementHandler.Finish();
            _player.PlayerMovementHandler.Process();
            _player.PlayerMovementHandler.Reset();
            return false;
        }

        bool isValidDistance = distance <= (int)CombatDistances.Magic;

        var projectilePathClear = RSPathfinder.IsProjectilePathClear(_player, _player.Location.X, _player.Location.Y,
            _player.Location.Z,
            targetX, targetY);
        if (!projectilePathClear)
        {
            _player.PlayerMovementHandler.Reset();
            RSPathfinder.FindPath(_player, targetX, targetY, true, 1, 1);
            _player.PlayerMovementHandler.Finish();
            _player.PlayerMovementHandler.Process();
            _player.PlayerMovementHandler.Reset();
            return false;
        }

        return projectilePathClear && isValidDistance;
    }

    public bool InValidMeleeDistance(Player target)
    {
        int targetX = target.Location.X;
        int targetY = target.Location.Y;
        int targetZ = target.Location.Z;

        var distance = MovementHelper.EuclideanDistance(_player.Location.X, _player.Location.Y,
            targetX, targetY);


        if (distance > 20)
        {
            _player.CurrentInteraction = null;
            _player.SetFacingEntity(null);
            _player.PlayerMovementHandler.Reset();
            return false;
        }


        if (distance > (int)CombatDistances.Melee)
        {
            _player.PlayerMovementHandler.Reset();
            RSPathfinder.MeleeFollow(_player, target);
            _player.PlayerMovementHandler.Finish();
            _player.PlayerMovementHandler.Process();
            _player.PlayerMovementHandler.Reset();
        }

        /* If same tile step away */
        if (distance <= 0)
        {
            _player.PlayerMovementHandler.Reset();
            RSPathfinder.MeleeFollow(_player, target);
            _player.PlayerMovementHandler.Finish();
            _player.PlayerMovementHandler.Process();
            _player.PlayerMovementHandler.Reset();
        }

        int moveDistance = 1;
        if (_player.PlayerMovementHandler.IsWalking)
            moveDistance = 2;
        if (_player.PlayerMovementHandler.IsRunning)
            moveDistance = 3;

        var projectilePathClear = MeleePathing.IsLongMeleeDistanceClear(_player, _player.Location.X, _player.Location.Y,
            _player.Location.Z, targetX, targetY, 2);

        bool isValidDistance = distance <= moveDistance;
        bool isDiagonal = MeleePathing.IsDiagonal(_player.Location.X, _player.Location.Y, targetX, targetY);

        _player.Session.PacketBuilder.SendMessage($"MoveDistance: {moveDistance}");
        _player.Session.PacketBuilder.SendMessage($"IsDiagonal: {isDiagonal}");
        _player.Session.PacketBuilder.SendMessage($"InValidDistance: {isValidDistance}");
        _player.Session.PacketBuilder.SendMessage("NoClipping: " + projectilePathClear);

        return isValidDistance && projectilePathClear && !isDiagonal;
    }

    private int GetRangeDelay(int distance) => distance switch
    {
        1 => 2,
        2 => 2,
        >= 3 and <= 8 => 3,
        >= 9 => 4,
        _ => 2
    };

    double GetDistanceToTarget(int targetX, int targetY)
    {
        return MovementHelper.EuclideanDistance(_player.Location.X, _player.Location.Y,
            targetX, targetY);
    }
}