using Genesis.Cache;
using Genesis.Entities;
using Genesis.Managers;

namespace Genesis.Environment;

public class Location
{
    public Location(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public int X { get; set; }

    public int Y { get; set; }

    public int Z { get; set; }

    public bool ShouldGenerateNewBuildArea => PositionRelativeToOffsetChunkX < 16 ||
                                              PositionRelativeToOffsetChunkX >= 88 ||
                                              PositionRelativeToOffsetChunkY < 16 ||
                                              PositionRelativeToOffsetChunkY >= 88;

    public int PositionRelativeToOffsetChunkX => X - _cachedBuildAreaSWChunkX * 8;
    public int PositionRelativeToOffsetChunkY => Y - _cachedBuildAreaSWChunkY * 8;
    public int RegionId { get; set; }

    public int CachedAbsoluteCenterX => _cachedAbsoluteCenterX;
    public int CachedAbsoluteCenterY => _cachedAbsoluteCenterY;

    public int CachedBuildAreaSwChunkX => _cachedBuildAreaSWChunkX;
    public int CachedBuildAreaSwChunkY => _cachedBuildAreaSWChunkY;
    public int CachedBuildAreaStartX => _cachedBuildAreaStartX;
    public int CachedBuildAreaStartY => _cachedBuildAreaStartY;

    public int CachedCenterChunkX => _cachedCenterChunkX;
    public int CachedCenterChunkY => _cachedCenterChunkY;

    private int _cachedCenterChunkX;
    private int _cachedCenterChunkY;

    private int _cachedBuildAreaSWChunkX;
    private int _cachedBuildAreaSWChunkY;

    private int _cachedBuildAreaStartX;
    private int _cachedBuildAreaStartY;

    //Offsets
    private int _cachedAbsoluteCenterX;
    private int _cachedAbsoluteCenterY;


    public void Build()
    {
        RegionId = (((X >> 6) << 8) & 0xFF00) | ((Y >> 6) & 0xFF);

        _cachedAbsoluteCenterX = X;
        _cachedAbsoluteCenterY = Y;

        _cachedCenterChunkX = _cachedAbsoluteCenterX >> 3;
        _cachedCenterChunkY = _cachedAbsoluteCenterY >> 3;

        _cachedBuildAreaSWChunkX = (_cachedAbsoluteCenterX >> 3) - 6;
        _cachedBuildAreaSWChunkY = (_cachedAbsoluteCenterY >> 3) - 6;

        _cachedBuildAreaStartX = ((_cachedAbsoluteCenterX >> 3) - 6) << 3;
        _cachedBuildAreaStartY = ((_cachedAbsoluteCenterY >> 3) - 6) << 3;
    }

    public bool IsWithinArea(Location entityLocation)
    {
        var delta = Delta(this, entityLocation);
        return delta.X <= 14 && delta.X >= -15 && delta.Y <= 14 && delta.Y >= -15 && entityLocation.Z == Z;
    }

    // public void RefreshObjects(Player player)
    // {
    //     // Determine build area bounds
    //     int buildAreaStartX = CachedBuildAreaStartX;
    //     int buildAreaStartY = CachedBuildAreaStartY;
    //     int buildAreaEndX = buildAreaStartX + (13 << 3); // 13 chunks * 8 tiles per chunk
    //     int buildAreaEndY = buildAreaStartY + (13 << 3);
    //
    //     // Iterate through tiles in the defined build area
    //     for (int tileX = buildAreaStartX; tileX < buildAreaEndX; tileX++)
    //     {
    //         for (int tileY = buildAreaStartY; tileY < buildAreaEndY; tileY++)
    //         {
    //             // Retrieve object at the tile position
    //             var gameObject = Region.GetObjectAt(tileX, tileY, Z);
    //             if (gameObject == null)
    //                 continue;
    //
    //             // Handle conflicts in respawning objects
    //             foreach (var respawningObject in WorldObjectManager.Respawning)
    //             {
    //                 var respawnObj = respawningObject.ReplaceWith;
    //
    //                 if (WorldObjectManager.IsConflict(respawnObj, gameObject))
    //                 {
    //                     WorldObjectManager.HandleConflict(respawnObj, tileX, tileY, player);
    //                     WorldObjectManager.Notify(respawningObject, player);
    //                 }
    //             }
    //         }
    //     }
    // }

    public static Location Delta(Location a, Location b)
    {
        return new Location(b.X - a.X, b.Y - a.Y, b.Z - a.Z);
    }

    internal void Move(int amountX, int amountY)
    {
        X += amountX;
        Y += amountY;
    }

    public List<string> ToStringParts()
    {
        var clipData = Region.GetClipping(X, Y, Z);
        var messageParts = new List<string>
        {
            $"CachedCenterX: {CachedAbsoluteCenterX} CachedCenterY: {CachedAbsoluteCenterY}",
            $"CenterChunkX: {CachedCenterChunkX} CenterChunkY: {CachedCenterChunkY}",
            $"RegionId: {RegionId} OffsetChunkX: {CachedBuildAreaSwChunkX} OffsetChunkY: {CachedBuildAreaSwChunkY}",
            $"BuildAreaStartX: {CachedBuildAreaStartX} BuildAreaStartY: {CachedBuildAreaStartY}",
            $"PositionRelativeToOffsetChunkX: {PositionRelativeToOffsetChunkX} PositionRelativeToOffsetChunkY: {PositionRelativeToOffsetChunkY}",
            $"IsOutside: {ShouldGenerateNewBuildArea}",
            $"ClipData: [{clipData}]"
        };

        return messageParts;
    }
}