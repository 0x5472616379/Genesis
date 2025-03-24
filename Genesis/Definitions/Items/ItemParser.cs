using System.Text.Json;

namespace Genesis.Definitions;

public static class ItemParser
{
    private static DataManager<ItemBonuses> _bonusesManager;
    private static DataManager<ItemRequirements> _requirementsManager;
    private static DataManager<ItemAnimations> _animationsManager;
    private static DataManager<ItemWeights> _weightsManager;
    private static DataManager<ItemAlchs> _alchsManager;
    private static DataManager<ItemSellableTradable> _sellableManager;

    private static bool _isInitialized = false;

    public static string RootPath { get; set; } = "../../../Data";
    private static string DataDirectory => "../../../Data";

    public static void Initialize()
    {
        if (_isInitialized)
        {
            Console.WriteLine("ItemParser is already initialized.");
            return;
        }

        // Initialize data managers
        _bonusesManager = new DataManager<ItemBonuses>(Path.Combine(DataDirectory, "ItemBonuses.json")
            , item => item.Id);
        Console.WriteLine("Loaded item bonuses.");

        _requirementsManager = new DataManager<ItemRequirements>(Path.Combine(DataDirectory, "ItemRequirements.json")
            , item => item.Id);
        Console.WriteLine("Loaded item requirements.");

        _animationsManager = new DataManager<ItemAnimations>(Path.Combine(DataDirectory, "ItemAnimations.json")
            , item => item.Id);
        Console.WriteLine("Loaded item animations.");

        _weightsManager = new DataManager<ItemWeights>(Path.Combine(DataDirectory, "ItemWeights.json")
            , item => item.Id);
        Console.WriteLine("Loaded item weights.");

        _alchsManager = new DataManager<ItemAlchs>(Path.Combine(DataDirectory, "ItemAlchs.json")
            , item => item.Id);
        Console.WriteLine("Loaded item alchemy values.");

        _sellableManager = new DataManager<ItemSellableTradable>(
            Path.Combine(DataDirectory, "ItemSellableTradable.json")
            , item => item.Id);
        Console.WriteLine("Loaded sellable and tradable statuses.");

        _isInitialized = true;
    }

    public static ItemBonuses GetBonusesById(int id)
    {
        EnsureInitialized();
        return _bonusesManager.GetById(id);
    }

    public static ItemRequirements GetRequirementsById(int id)
    {
        EnsureInitialized();
        return _requirementsManager.GetById(id);
    }

    public static ItemAnimations GetAnimationsById(int id)
    {
        EnsureInitialized();
        return _animationsManager.GetById(id);
    }

    public static ItemWeights GetWeightById(int id)
    {
        EnsureInitialized();
        return _weightsManager.GetById(id);
    }

    public static ItemAlchs GetAlchsById(int id)
    {
        EnsureInitialized();
        return _alchsManager.GetById(id);
    }

    public static ItemSellableTradable GetSellableInfoById(int id)
    {
        EnsureInitialized();
        return _sellableManager.GetById(id);
    }

    public static double GetBonusValue(int itemId, BonusType type)
    {
        return _bonusesManager.GetById(itemId).Bonuses.ElementAtOrDefault((int)type);
    }

    private static void EnsureInitialized()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("ItemParser has not been initialized. Call Initialize() first.");
    }
}