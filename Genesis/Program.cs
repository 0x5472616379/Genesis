using System.Diagnostics;
using Genesis;
using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Environment;
using Genesis.Shops;

var ifs = new IndexedFileSystem("../../../Data/cache", true);

Benchmark(() => { new ObjectDefinitionDecoder(ifs).Run(); }, "Loaded Objects in");
Benchmark(() => { RegionFactory.Load(ifs); }, "Loaded Regions in");
Benchmark(() => { new ItemDefinitionDecoder(ifs).Run(); }, "Decoded Item Definitions in");

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