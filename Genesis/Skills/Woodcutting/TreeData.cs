namespace Genesis.Skills.Woodcutting;

static class TreeData
{
    private static readonly Tree[] TreeList =
    {
        new("NORMAL_TREE", new[] {1276, 1277, 1278, 1279, 1280, 1282, 1283, 1284, 1285, 1286, 1289, 1290, 1291, 1315, 1316, 1318, 1319, 1330, 1331, 1332, 1333, 1365, 1383, 1384, 2409, 3033, 3034, 3035, 3036, 3881, 3882, 3883, 5902, 5903, 5904},
            new[] {1719, 1720, 1721, 1721, 1721, 1722, 1722, 1722, 1723, 1727, 1727, 1728, 1729, 1741, 1742, 1743, 1743, 1744, 1744, 1744, 1732, 1725, 1727, 1745, 1721, 1719, 1721, 1722, 1726, 1747, 1747, 1747, 1722, 1727, 1727},
            1, 25, 1511, 1342, 75, 100),
        new("OAK_TREE", new[] {1281, 2037}, new[] {1739}, 15, 37.5, 1521, 1356, 17, 20),
        new("WILLOW_TREE", new[] {1308, 5551, 5552, 5553}, new[] {1736, 1737, 1737, 1738}, 30, 67.5, 1519, 7399, 17, 15),
        new("TEAK_TREE", new[] {9036}, new[] {2535}, 35, 85, 6333, 9037, 14, 20),
        new("MAPLE_TREE", new[] {1307, 4677}, new[] {1735}, 45, 100, 1517, 1343, 75, 15),
        new("HOLLOW_TREE", new[] {2289, 4060}, new[] {1749, 1750}, 45, 83, 3239, 2310, 59, 15),
        new("MAHOGANY_TREE", new[] {9034}, new[] {2534}, 50, 125, 6332, 9035, 80, 10),
        new("YEW_TREE", new[] {1309}, new[] {1740}, 60, 175, 1515, 7402, 138, 5),
        new("MAGIC_TREE", new[] {1306}, new[] {1734}, 75, 250, 1513, 7401, 270, 5),
        new("DRAMEN_TREE", new[] {1292}, null, 36, 0, 771, 1513, 59, 100)
    };

    
    
    private static readonly Dictionary<int, Tree> Trees = TreeList
        .SelectMany(tree => tree.Ids, (tree, id) => new { id, tree })
        .ToDictionary(x => x.id, x => x.tree);

    public static Tree? GetTree(int id) => Trees.TryGetValue(id, out var tree) ? tree : null;
}