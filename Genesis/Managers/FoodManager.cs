using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Skills;

namespace Genesis.Managers;

public class FoodManager
{
    private readonly Player _player;
    
    public static readonly HashSet<int> HardFoods = new() { 385, 391 }; // Shark, Manta
    public static readonly HashSet<int> ComboFoods = new() { 3144 }; // Karambwan
    
    // Eating state
    private int _lastEatTick = -10;
    private bool _hardFoodConsumed = false;
    private bool _comboFoodConsumed = false;
    private int _queuedHardFoodId = -1;
    private int _queuedHardFoodSlot = -1;
    private int _queuedComboFoodId = -1;
    private int _queuedComboFoodSlot = -1;
    private bool _clickedHardFoodFirst = false;

    public FoodManager(Player player) => _player = player;

    public void QueueFood(int itemId, int slot)
    {
        bool isHard = HardFoods.Contains(itemId);
        bool isCombo = ComboFoods.Contains(itemId);

        if (!isHard && !isCombo) return;

        if (isHard)
        {
            /* Only queue hard food if no combo food was already clicked this tick */
            if (_queuedComboFoodId == -1)
            {
                _queuedHardFoodId = itemId;
                _queuedHardFoodSlot = slot;
                _clickedHardFoodFirst = true;
            }
        }
        else if (isCombo)
        {
            _queuedComboFoodId = itemId;
            _queuedComboFoodSlot = slot;
            /* If hard food was already queued, maintain the first-click flag */
            if (_queuedHardFoodId != -1) _clickedHardFoodFirst = true;
        }
    }

    public void ProcessFoodActions()
    {
        if (_player.CurrentHealth <= 0)
        {
            ClearQueues();
            return;
        }

        // Reset
        if (World.CurrentTick >= _lastEatTick + 3)
        {
            _hardFoodConsumed = false;
            _comboFoodConsumed = false;
        }

        // Case 1: We have both foods queued and hard food was clicked first
        if (_clickedHardFoodFirst && _queuedHardFoodId != -1 && _queuedComboFoodId != -1)
        {
            if (!_hardFoodConsumed && World.CurrentTick >= _lastEatTick + 3)
            {
                ConsumeFood(_queuedHardFoodId, _queuedHardFoodSlot);
                _queuedHardFoodId = -1;
                _hardFoodConsumed = true;
                _lastEatTick = World.CurrentTick;
                
                if (!_comboFoodConsumed)
                {
                    ConsumeFood(_queuedComboFoodId, _queuedComboFoodSlot);
                    _queuedComboFoodId = -1;
                    _comboFoodConsumed = true;
                }
            }
        }
        // Case 2: Standalone hard food
        else if (_queuedHardFoodId != -1 && !_hardFoodConsumed && World.CurrentTick >= _lastEatTick + 3)
        {
            ConsumeFood(_queuedHardFoodId, _queuedHardFoodSlot);
            _queuedHardFoodId = -1;
            _hardFoodConsumed = true;
            _lastEatTick = World.CurrentTick;
        }
        // Case 3: Standalone combo food (including when clicked before hard food)
        else if (_queuedComboFoodId != -1 && !_comboFoodConsumed && World.CurrentTick >= _lastEatTick + 3)
        {
            ConsumeFood(_queuedComboFoodId, _queuedComboFoodSlot);
            _queuedComboFoodId = -1;
            _comboFoodConsumed = true;
            _lastEatTick = World.CurrentTick;
            // Clear any hard food that was clicked after the combo food
            _queuedHardFoodId = -1;
        }

        // Reset first-click flag for next tick
        if (World.CurrentTick > _lastEatTick)
        {
            _clickedHardFoodFirst = false;
        }
    }

    private void ConsumeFood(int itemId, int slot)
    {
        int healAmount = GetHealAmount(itemId);
        
        _player.SetCurrentAnimation(829);
        _player.CurrentHealth = Math.Min(
            _player.CurrentHealth + healAmount,
            _player.SkillManager.Skills[(int)SkillType.HITPOINTS].Level);
        
        _player.SkillManager.RefreshSkill(SkillType.HITPOINTS);
        _player.Inventory.ClearSlot(slot);
        _player.Inventory.RefreshSlot(_player, slot, -1, 0, GameInterfaces.DefaultInventoryContainer);
        
        _player.Session.PacketBuilder.SendMessage($"You eat the {GetFoodName(itemId)}.");
    }

    private void ClearQueues()
    {
        _queuedHardFoodId = -1;
        _queuedHardFoodSlot = -1;
        _queuedComboFoodId = -1;
        _queuedComboFoodSlot = -1;
        _clickedHardFoodFirst = false;
    }

    private static int GetHealAmount(int itemId) => itemId switch
    {
        385 => 20,    // Shark
        391 => 22,    // Manta
        3144 => 18,   // Karambwan
        _ => 0
    };

    private static string GetFoodName(int itemId) => itemId switch
    {
        385 => "shark",
        391 => "manta ray",
        3144 => "karambwan",
        7228 => "anglerfish",
        _ => "food"
    };
}