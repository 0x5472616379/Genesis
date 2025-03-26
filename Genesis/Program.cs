using System.Diagnostics;
using Genesis;
using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Definitions;
using Genesis.Environment;
using Genesis.Shops;

Directory.SetCurrentDirectory(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));

string DataDirectory = "./Data/cache";
var ifs = new IndexedFileSystem(DataDirectory, true);

Benchmark(() => { new ObjectDefinitionDecoder(ifs).Run(); }, "Loaded Objects in");
Benchmark(() => { RegionFactory.Load(ifs); }, "Loaded Regions in");
Benchmark(() => { new ItemDefinitionDecoder(ifs).Run(); }, "Decoded Item Definitions in");
Benchmark(ItemParser.Initialize, "Loaded Item Info");

RSServer server = new RSServer();
server.Run();




void Benchmark(Action action, string benchmarkText)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();

    action.Invoke();

    stopwatch.Stop();
    Console.WriteLine($"{benchmarkText}: {stopwatch.ElapsedMilliseconds}ms");
}