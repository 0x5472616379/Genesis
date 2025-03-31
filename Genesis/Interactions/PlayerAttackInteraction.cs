using ArcticRS.Appearance;
using Genesis.Configuration;
using Genesis.Entities.Player;
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
        return _player.CombatHelper.Attack(_target, World.CurrentTick);
    }

    public override bool CanExecute()
    {
        return true;
    }
}