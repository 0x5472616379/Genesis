namespace Genesis.Definitions.Items;

public static class ItemParser
{
    private static readonly DataManager<ItemBonuses> _bonusesManager;
    private static readonly DataManager<ItemRequirements> _requirementsManager;
    private static readonly DataManager<ItemAnimations> _animationsManager;
    private static readonly DataManager<ItemWeights> _weightsManager;
    private static readonly DataManager<ItemAlchs> _alchsManager;
    private static readonly DataManager<ItemSellableTradable> _sellableManager;

    private const string _dataDirectory = "./Data";
    private static bool _initialize;

    static ItemParser()
    {
        // Initialize data managers
        _bonusesManager = new(Path.Combine(_dataDirectory, "ItemBonuses.json")
            , item => item.Id);
        Console.WriteLine("Loaded item bonuses.");

        _requirementsManager = new(Path.Combine(_dataDirectory, "ItemRequirements.json")
            , item => item.Id);
        Console.WriteLine("Loaded item requirements.");

        _animationsManager = new(Path.Combine(_dataDirectory, "ItemAnimations.json")
            , item => item.Id);
        Console.WriteLine("Loaded item animations.");

        _weightsManager = new(Path.Combine(_dataDirectory, "ItemWeights.json")
            , item => item.Id);
        Console.WriteLine("Loaded item weights.");

        _alchsManager = new(Path.Combine(_dataDirectory, "ItemAlchs.json")
            , item => item.Id);
        Console.WriteLine("Loaded item alchemy values.");

        _sellableManager = new(
            Path.Combine(_dataDirectory, "ItemSellableTradable.json")
            , item => item.Id);
        Console.WriteLine("Loaded sellable and tradable statuses.");
    }

    public static void Initialize()
    {
        if (_initialize)
        {
            throw new InvalidOperationException($"Cannot call {nameof(Initialize)} after {nameof(Initialize)} has been called already.");
        }

        _initialize = true;
    }

    public static ItemBonuses GetBonusesById(int id) =>
        _bonusesManager.GetById(id);

    public static ItemRequirements GetRequirementsById(int id) =>
        _requirementsManager.GetById(id);

    public static ItemAnimations GetAnimationsById(int id) =>
        _animationsManager.GetById(id);

    public static ItemWeights GetWeightById(int id) =>
        _weightsManager.GetById(id);

    public static ItemAlchs GetAlchsById(int id) =>
        _alchsManager.GetById(id);

    public static ItemSellableTradable GetSellableInfoById(int id) =>
        _sellableManager.GetById(id);

    public static double GetBonusValue(int itemId, BonusType type) =>
        _bonusesManager.GetById(itemId).Bonuses.ElementAtOrDefault((int)type);
}