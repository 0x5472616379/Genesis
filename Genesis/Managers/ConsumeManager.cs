using ArcticRS.Actions;
using Genesis.Entities.Player;
using Genesis.Environment;

namespace Genesis.Managers;

public class ConsumeManager
{
    private enum LastConsumedType { None, Hard, Combo, Potion }

    private readonly Player _player;

    public static readonly HashSet<int> HardFoods = new() { 385, 391 };  // Shark, Manta
    public static readonly HashSet<int> ComboFoods = new() { 3144 };     // Karambwan
    public static readonly HashSet<int> Potions = new() { 2434, 3024 };  // Prayer Potion

    /* Tracking */
    private int _lastHardFoodTick = -1;
    private int _lastComboFoodTick = -1;
    private int _lastConsumedTick = -1;
    private LastConsumedType _lastConsumed = LastConsumedType.None;
    private const int EatDelay = 2;

    public ConsumeManager(Player player)
    {
        _player = player;
    }

    public void Consume(int consumableId, int fromInvIndex)
    {
        var currentTick = World.CurrentTick;
        bool isHardFood = HardFoods.Contains(consumableId);
        bool isComboFood = ComboFoods.Contains(consumableId);
        bool isPotion = Potions.Contains(consumableId);

        /* Reset state if global cooldown expired */
        if (currentTick - _lastConsumedTick >= EatDelay)
        {
            _lastConsumed = LastConsumedType.None;
        }

        if (!CanEat(currentTick, isHardFood, isComboFood, isPotion))
        {
            _player.Session.PacketBuilder.SendMessage("You can't eat that yet!");
            return;
        }

        /* Update specific cooldowns and state */
        if (isHardFood)
        {
            _lastHardFoodTick = currentTick;
            _lastConsumed = LastConsumedType.Hard;
        }
        else if (isComboFood)
        {
            _lastComboFoodTick = currentTick;
            _lastConsumed = LastConsumedType.Combo;
        }
        else if (isPotion)
        {
            _lastConsumed = LastConsumedType.Potion;
        }

        /* Update global consumption timestamp */
        _lastConsumedTick = currentTick;

        _player.ActionHandler.AddAction(new EatFoodAction(_player, GetHealAmount(consumableId), fromInvIndex, consumableId));
    }

    private bool CanEat(int currentTick, bool isHardFood, bool isComboFood, bool isPotion)
    {
        /* Potion Rules */
        if (isPotion)
        {
            // Block if last consumed was combo (unless global cooldown expired)
            return _lastConsumed != LastConsumedType.Combo || (currentTick - _lastConsumedTick >= EatDelay);
        }

        /* Hard Food Rules */
        if (isHardFood)
        {
            /* Block if hard food cooldown active */
            if (currentTick - _lastHardFoodTick < EatDelay)
                return false;

            /* Block if last consumed was potion/combo (unless global cooldown expired) */
            return _lastConsumed != LastConsumedType.Potion && 
                   _lastConsumed != LastConsumedType.Combo;
        }

        /* Combo Food Rules */
        if (isComboFood)
        {
            /* Always block if combo cooldown active (strict 2-tick delay between combo foods) */
            if (currentTick - _lastComboFoodTick < EatDelay)
                return false;

            /*
             * Allow combo if:
             * 1. Following hard/potion/none, OR
             * 2. Global cooldown expired (state reset)
             */
            return _lastConsumed == LastConsumedType.Hard ||
                   _lastConsumed == LastConsumedType.Potion ||
                   _lastConsumed == LastConsumedType.None;
        }

        return true;
    }

    private int GetHealAmount(int consumableId)
    {
        return consumableId switch
        {
            385 => 20,   // Shark
            391 => 22,   // Manta
            3144 => 18,  // Karambwan
            _ => 0
        };
    }
}