using ArcticRS.Actions;
using Genesis.Entities;
using Genesis.Environment;

namespace Genesis.Managers;

public class ConsumeManager
{
    private enum LastConsumedType { None, Hard, Combo, Potion }

    private readonly Player _player;

    public static readonly HashSet<int> HardFoods = new() { 385, 391 };      // Shark, Manta Ray
    public static readonly HashSet<int> ComboFoods = new() { 3144 };         // Karambwan
    public static readonly HashSet<int> Potions = new() { 2434, 3024 };      // Prayer Potion

    /* Tracking */
    private int _lastHardFoodTick = -1;
    private int _lastComboFoodTick = -1;
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

        if (!CanEat(currentTick, isHardFood, isComboFood, isPotion))
        {
            _player.Session.PacketBuilder.SendMessage("You can't eat that yet!");
            return;
        }

        /* Update tracking */
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

        _player.ActionHandler.AddAction(new EatFoodAction(_player, GetHealAmount(consumableId), fromInvIndex, consumableId));
    }

    private bool CanEat(int currentTick, bool isHardFood, bool isComboFood, bool isPotion)
    {
        /* Potions are always allowed */
        if (isPotion) return true;

        /* Hard Food Rules */
        if (isHardFood)
        {
            /* Block if last consumed was a potion */
            if (_lastConsumed == LastConsumedType.Potion)
                return false;

            /* Block if eaten within 2 ticks */
            if (currentTick - _lastHardFoodTick < EatDelay)
                return false;
        }

        /* Combo Food Rules */
        if (isComboFood)
        {
            /* Block if eaten within 2 ticks */
            if (currentTick - _lastComboFoodTick < EatDelay)
                return false;

            /* Allow if last consumed was Hard, Potion, or None */
            if (_lastConsumed != LastConsumedType.Hard && 
                _lastConsumed != LastConsumedType.Potion && 
                _lastConsumed != LastConsumedType.None)
                return false;
        }

        return true;
    }

    private int GetHealAmount(int consumableId)
    {
        return consumableId switch
        {
            385 => 20,   // Shark
            391 => 22,   // Manta Ray
            3144 => 18,  // Karambwan
            _ => 0
        };
    }
}