using ArcticRS.Appearance;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Skills.Combat;

namespace Genesis.Interactions;

public class PlayerAttackInteraction : RSInteraction
{
    private readonly Player _player;
    private Player _target;
    private readonly Weapon _weapon;
    public override int MaxDistance { get; } = 1;
    public override InteractingEntity Target { get; set; } = new();

    private int currentTick = 0;
    private bool attackLoaded;

    private CombatDistances combatDistance = CombatDistances.Melee;

    public PlayerAttackInteraction(Player player, Player target)
    {
        _player = player;
        _target = target;
    }

    public override bool Execute()
    {
        if (!CanExecute()) return false;
        return _player.CombatManager.Attack(_target, World.CurrentTick);
    }

    public override bool CanExecute()
    {
        // var weaponId = _player.Equipment.GetItemInSlot(EquipmentSlot.Weapon).ItemId;
        //
        // var UsingBow = GameConstants.IsShortbow(weaponId) || GameConstants.IsLongbow(weaponId)
        //                                                        || GameConstants.IsDart(weaponId)
        //                                                        || GameConstants.IsThrowingKnife(weaponId);
        //
        // if (_player.CurrentHealth <= 0)
        // {
        //     return false;
        // }
        //
        // if (_target.CurrentHealth <= 0)
        // {
        //     _player.CurrentInteraction = null;
        //     _player.InteractingEntity = null;
        //     _target = null;
        //     _player.SetFacingEntity(null);
        //     _player.PlayerMovementHandler.Reset();
        //     return false;
        // }
        //
        // if (UsingBow)
        // {
        //     if (_player.CombatManager.InValidProjectileDistance(_target))
        //     {
        //         _player.PlayerMovementHandler.Reset();
        //         return true;
        //     }
        // }
        // else
        // {
        //     if (_player.CombatManager.InValidMeleeDistance(_target))
        //     {
        //         _player.PlayerMovementHandler.Reset();
        //         return true;
        //     }
        // }

        return true;
    }
}