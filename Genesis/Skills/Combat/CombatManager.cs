using ArcticRS.Actions;
using ArcticRS.Appearance;
using Genesis.Entities;
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
            /* Perform Attack */

            /* Get the current weapon equipped */
            var equipped = _player.Equipment.GetItemInSlot(EquipmentSlot.Weapon);
            var weapon = WeaponBuilder.GetWeapon(_player, equipped.ItemId);
            _player.SetCurrentAnimation(weapon.AttackerAnim);

            int dmg = r.Next(0, 31);
            var damage = new Damage(dmg == 0 ? DamageType.BLOCK : DamageType.HIT, dmg, null);

            ProjectileCreator.CreateProjectile(_player, target);
            
            AttackedWith = weapon;
            LastAttackTick = currentTick;
            target.ActionHandler.AddAction(new DamageAction(target, damage));
            return false;
        }

        return false;
    }

    public void SwitchWeapon(Weapon weapon)
    {
        AttackedWith = weapon;
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
}