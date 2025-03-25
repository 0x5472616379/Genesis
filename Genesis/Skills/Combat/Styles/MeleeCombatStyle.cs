using ArcticRS.Actions;
using Genesis.Definitions;
using Genesis.Entities;
using Genesis.Movement;
using Genesis.Skills.Combat.Maxhit;

namespace Genesis.Skills.Combat;

public class MeleeCombatStyle : ICombatStyle
{
    private static readonly Random _random = new();

    public void Attack(Player player, Player target, int currentTick)
    {
        var weapon = player.CombatHelper.GetEquippedWeapon();
        var weaponData = WeaponBuilder.GetWeaponData(player, weapon.ItemId);

        var specialHandler = player.CombatHelper.SpecialAttack;
        if (specialHandler != null && specialHandler.CanExecute(player))
        {
            specialHandler.Execute(player, target, currentTick, weaponData);
            return; // Skip regular attack logic
        }

        player.SetCurrentAnimation(weaponData.AttackerAnim);

        var damage = CalculateDamage(player, target);
        target.ActionHandler.AddAction(new DamageAction(target, damage, 0));

        player.CombatHelper.UpdateAttackState(currentTick, weaponData);
    }

    public bool CanAttack(Player player, Player target, int currentTick)
    {
        var weapon = player.CombatHelper.GetEquippedWeapon();
        var weaponData = WeaponBuilder.GetWeaponData(player, weapon.ItemId);

        return currentTick - player.CombatHelper.LastAttackTick >= weaponData.AttackSpeed;
    }

    public bool ValidateDistance(Player player, Player target)
    {
        var distance = MovementHelper.EuclideanDistance(player.Location.X, player.Location.Y, target.Location.X,
            target.Location.Y);

        if (distance >= 20)
            player.CombatHelper.ResetInteraction();

        RSPathfinder.MeleeFollow(player, target);
        player.PlayerMovementHandler.Finish();
        player.PlayerMovementHandler.Process();
        player.PlayerMovementHandler.Reset();

        var isDiagonal = MeleePathing.IsDiagonal(
            player.Location.X, player.Location.Y,
            target.Location.X, target.Location.Y);
        var isLongMeleeDistanceClear = MeleePathing.IsLongMeleeDistanceClear(
            player,
            player.Location.X, player.Location.Y, player.Location.Z,
            target.Location.X, target.Location.Y, 2
        );

        // player.Session.PacketBuilder.SendMessage($"IsDiagonal: {isDiagonal && distance <= 2}");
        // player.Session.PacketBuilder.SendMessage($"Clipping Clear: {isLongMeleeDistanceClear}");
        // player.Session.PacketBuilder.SendMessage($"Distance: {distance}");
        // player.Session.PacketBuilder.SendMessage($"WalkedThisTick: {player.PlayerMovementHandler.IsWalking}");

        var walked = player.PlayerMovementHandler.IsWalking;
        var ran = player.PlayerMovementHandler.IsRunning;

        var validDistance = ran ? 3 : walked ? 2 : 1;

        if (!isDiagonal && isLongMeleeDistanceClear && distance <= validDistance)
        {
            // player.PlayerMovementHandler.Reset();
            return true;
        }

        return false;
    }

    private Damage CalculateDamage(Player player, Player target)
    {
        var attackBonus = player.BonusManager.GetTotalForBonusType(BonusType.SlashAttack);
        var defenseBonus = target.BonusManager.GetTotalForBonusType(BonusType.SlashDefence);

        var attackRoll = (player.SkillManager.GetSkillLevel(SkillType.STRENGTH) + 8) * (attackBonus + 64);
        var defenseRoll = (target.SkillManager.GetSkillLevel(SkillType.DEFENCE) + 9) * (defenseBonus + 64);

        var hitChance = (double)attackRoll / (attackRoll + defenseRoll);
        var isHit = _random.NextDouble() < hitChance;

        if (!isHit) return new Damage(DamageType.BLOCK, 0, null);

        var maxHit = MaxHitCalculator.GetMeleeMaxHit(player);
        int damageValue = _random.Next(1, maxHit + 1);
        return new Damage(DamageType.HIT, damageValue, null);
    }
}