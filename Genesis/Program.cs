using System.Diagnostics;
using Genesis;
using Genesis.Cache;
using Genesis.Cache.Idx;
using Genesis.Cache.Items;
using Genesis.Definitions.Items;
using Genesis.Environment;

string DataDirectory = "./Data/cache";
var ifs = new IndexedFileSystem(DataDirectory, true);

Benchmark(() => { ObjectDefinitionDecoder.Load(ifs); }, "Loaded Objects in");
Benchmark(() => { RegionFactory.Load(ifs); }, "Loaded Regions in");
Benchmark(() => { ItemDefinitionDecoder.Load(ifs); }, "Decoded Item Definitions in");
Benchmark(ItemParser.Initialize, "Loaded Item Info");

new RSServer().Run();

static void Benchmark(Action action, string benchmarkText)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();

    action.Invoke();

    Console.WriteLine($"{benchmarkText}: {stopwatch.ElapsedMilliseconds}ms");
    stopwatch.Stop();
}
