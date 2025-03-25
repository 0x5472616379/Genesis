using ArcticRS.Appearance;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Model;
using Genesis.Skills.Combat.Specials;

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
    public SpecialAttack SpecialAttack { get; set; } = null;
    public int SpecialAmount { get; set; } = 10;

    public int LastAttackTick { get; private set; } = -1;
    public Weapon AttackedWith { get; private set; }

    private ICombatStyle _combatStyle;

    public RangedCombatStyle RangedCombatStyle { get; set; } = new();
    public MeleeCombatStyle MeleeCombatStyle { get; set; } = new();

    public CombatHelper(Player player)
    {
        _player = player;
        SetCombatStyle(CombatStyle.Melee);
    }

    public bool Attack(Player target, int currentTick)
    {
        if (target.CurrentHealth <= 0 || _player.CurrentHealth <= 0)
        {
            ResetInteraction();
            return false;
        }
        
        ComputeAttackStyle();

        if (!_combatStyle.ValidateDistance(_player, target)) return false;
        if (!_combatStyle.CanAttack(_player, target, currentTick)) return false;

        _combatStyle.Attack(_player, target, currentTick);
        return false;
    }

    public void SetCombatStyle(CombatStyle combatStyle)
    {
        _combatStyle = combatStyle switch
        {
            CombatStyle.Melee => new MeleeCombatStyle(),
            CombatStyle.Ranged => new RangedCombatStyle(),
            // CombatStyle.Magic => new MageCombatStyle(),
            _ => throw new ArgumentException("Unknown combat style: " + combatStyle)
        };
    }

    public void ResetInteraction()
    {
        _player.CurrentInteraction = null;
        _player.SetFacingEntity(null);
        _player.PlayerMovementHandler.Reset();
    }

    public void UpdateAttackState(int currentTick, Weapon weapon)
    {
        LastAttackTick = currentTick;
        AttackedWith = weapon;
    }

    public void UpdateSpecialAttack()
    {
        if (SpecialAttack != null)
        {
            _player.Session.PacketBuilder.DisplayHiddenInterface(0, 7549); /* 7561 Spec bar */
            for (int i = 0; i < 10; i++)
            {
                _player.Session.PacketBuilder.SendInterfaceOffset(i < SpecialAmount ? 500 : 0, 0, 7551 + i);
            }

            _player.Session.PacketBuilder.SendTextToInterface(GetSpecialBarText(), 7561);
        }
        else
        {
            _player.Session.PacketBuilder.DisplayHiddenInterface(0, 7549); /* 7561 Spec bar */
            for (int i = 0; i < 10; i++)
            {
                _player.Session.PacketBuilder.SendInterfaceOffset(i < SpecialAmount ? 500 : 0, 0, 7551 + i);
            }

            DisableSpecialBarText(7561);
        }
    }

    private string GetSpecialBarText()
    {
        string[] letters = { "S P", " E ", "C I ", "A L ", " A ", "T T", " A ", "C ", "K " };
        string textToDisplay = "";

        for (int i = 0; i < letters.Length; i++)
        {
            textToDisplay += SpecialAmount >= i + 2 ? "@yel@" + letters[i] : "@bla@" + letters[i];
        }

        return textToDisplay;
    }

    private void DisableSpecialBarText(int SpecBarId)
    {
        _player.Session.PacketBuilder.SendTextToInterface("@bla@S P E C I A L  A T T A C K", SpecBarId);
    }

    private void ComputeAttackStyle()
    {
        var equippedId = GetEquippedWeapon().ItemId;
        if (GameConstants.IsShortbow(equippedId) || GameConstants.IsLongbow(equippedId)
                                                 || GameConstants.IsThrowingKnife(equippedId)
                                                 || GameConstants.IsDart(equippedId)
                                                 || GameConstants.IsCrossbow(equippedId))
        {
            _combatStyle = RangedCombatStyle;
        }
        else
        {
            _combatStyle = MeleeCombatStyle;
        }
    }

    public ItemSlot GetEquippedWeapon()
    {
        return _player.Equipment.GetItemInSlot(EquipmentSlot.Weapon);
    }
}