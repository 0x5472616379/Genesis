using Genesis.Cache;
using Genesis.Cache.Idx;
using Genesis.Util;
using System.Collections.Frozen;

namespace Genesis.Environment;

public static class RegionFactory
{
    private static FrozenDictionary<int, Region>? _regions;
    public static FrozenDictionary<int, Region> GetRegions() => _regions ?? throw new InvalidOperationException($"Cannot access class before calling {nameof(Load)} first.");

    public static void Load(IndexedFileSystem fs)
    {
        var archive = Archive.Decode(fs.GetFile(new FileDescriptor(0, 5)));
        var entry = archive.GetEntry("map_index");
        var buffer = new MemoryStream(entry.GetBuffer());

        var size = buffer.Length / 7;
        // regions = new Region[size];
        var regionIds = new int[size];
        var mapGroundFileIds = new int[size];
        var mapObjectsFileIds = new int[size];
        var isMembers = new bool[size];

        for (var i = 0; i < size; i++)
        {
            regionIds[i] = buffer.ReadInt16BE();
            mapGroundFileIds[i] = buffer.ReadInt16BE();
            mapObjectsFileIds[i] = buffer.ReadInt16BE();
            isMembers[i] = buffer.ReadByte() == 0;
        }

        var regions = new Dictionary<int, Region>();
        for (var i = 0; i < size; i++) regions.Add(regionIds[i], new Region(regionIds[i], isMembers[i]));
        _regions = regions.ToFrozenDictionary();

        for (var i = 0; i < size; i++)
        {
            using var file1Stream = new MemoryStream();
            using var file2Stream = new MemoryStream();
            CompressionUtil.InflateToStream(fs.GetFile(4, mapObjectsFileIds[i]), file1Stream);
            CompressionUtil.InflateToStream(fs.GetFile(4, mapGroundFileIds[i]), file2Stream);

            try
            {
                LoadMaps(regionIds[i], file1Stream, file2Stream);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading map region: {regionIds[i]}\n{e.Message}");
            }
        }
    }

    private static void LoadMaps(int regionId, MemoryStream str1, MemoryStream str2)
    {
        var regionX = (regionId >> 8) * 64; // Region ID is bitshifted to get X position
        var regionY = (regionId & 0xff) * 64; // Region ID is bitshifted and AND'd against 0xff to get Y position
        var positionArray = new int[4 * 64 * 64];
        var someArrayBoolean = new bool[4 * 64 * 64];

        for (var localz = 0; localz < 4; localz++)
        {
            for (var localx = 0; localx < 64; localx++)
            {
                for (var localy = 0; localy < 64; localy++)
                {
                    while (true)
                    {
                        var v = str2.ReadByte();
                        if (v == 0) break;

                        if (v == 1)
                        {
                            str2.Skip(1);
                            break;
                        }

                        var pos = (localz * 64 * 64) + (localx * 64) + localy;
                        if (v == 2) 
                            someArrayBoolean[pos] = true;

                        if (v <= 49)
                            str2.Skip(1);
                        else if (v <= 81)
                            positionArray[pos] = v - 49;
                    }
                }
            }
        }


        for (var localz = 0; localz < 4; localz++)
        {
            for (var localx = 0; localx < 64; localx++)
            {
                for (var localy = 0; localy < 64; localy++)
                {
                    var pos = (localz * 64 * 64) + (localx * 64) + localy;
                    if ((positionArray[pos] & 1) == 1)
                    {
                        var height = localz;
                        if ((positionArray[(1 * 64 * 64) + (localx * 64) + localy] & 2) == 2) height--;
                        if (height >= 0 && height <= 3)
                        {
                            if (someArrayBoolean[pos])
                                Region.AddClipping(regionX + localx, regionY + localy, height, 264 + 0x0002000);
                            else
                                Region.AddClipping(regionX + localx, regionY + localy, height, 256 + 0x0002000);
                        }
                    }
                }
            }
        }


        uint objectId = 0;
        uint incr;
        while ((incr = str1.GetUSmart()) != 0)
        {
            objectId += incr - 1;
            uint location = 0;
            uint incr2;
            while ((incr2 = str1.GetUSmart()) != 0)
            {
                location += incr2 - 1;
                var objectX = (location >> 6) & 0x3f;
                var objectY = location & 0x3f;
                var objectHeight = location >> 12;
                var objectData = str1.GetUByte();
                var type = objectData >> 2;
                var direction = objectData & 0x3;
                if (objectX < 0 || objectX >= 64 || objectY < 0 || objectY >= 64)
                    continue; //Checks the object position is not outside the bounds of a region (0-64)

                if ((positionArray[(1 * 3 * 4) + (objectX * 4) + objectY] & 2) == 2) 
                    objectHeight--;

                if (regionX + objectX == 3238 && regionY + objectY == 3224) { } // ??

                if (objectHeight >= 0 && objectHeight <= 3)
                    Region.AddObject((int)objectId, (int)(regionX + objectX), (int)(regionY + objectY), (int)objectHeight, type, direction);
            }
        }
    }
}