namespace Genesis.Environment;

public class Location
{
    public Location(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
        // Update();
    }

    public int X { get; set; }

    public int Y { get; set; }

    public int Z { get; set; }

    public int CenterChunkX { get; set; }
    public int CenterChunkY { get; set; }
    public int RegionId { get; set; }
    public int OffsetChunkX { get; set; }
    public int OffsetChunkY { get; set; }
    public int BuildAreaStartX { get; set; }
    public int BuildAreaStartY { get; set; }
    public int PositionRelativeToOffsetChunkX => X - _cachedBuildAreaSWChunkX * 8;
    public int PositionRelativeToOffsetChunkY => Y - _cachedBuildAreaSWChunkY * 8;

    public bool ShouldGenerateNewBuildArea => PositionRelativeToOffsetChunkX < 16 ||
                                              PositionRelativeToOffsetChunkX >= 88 ||
                                              PositionRelativeToOffsetChunkY < 16 ||
                                              PositionRelativeToOffsetChunkY >= 88;

    private int _cachedCenterChunkX;
    private int _cachedCenterChunkY;
    
    private int _cachedBuildAreaSWChunkX;
    private int _cachedBuildAreaSWChunkY;
    
    private int _cachedBuildAreaStartX;
    private int _cachedBuildAreaStartY;

    public int CachedBuildAreaSwChunkX => _cachedBuildAreaSWChunkX;
    public int CachedBuildAreaSwChunkY => _cachedBuildAreaSWChunkY;
    public int CachedBuildAreaStartX => _cachedBuildAreaStartX;
    public int CachedBuildAreaStartY => _cachedBuildAreaStartY;
    public int CachedCenterChunkX => _cachedCenterChunkX;
    public int CachedCenterChunkY => _cachedCenterChunkY;
    

    public void Update()
    {
        if (ShouldGenerateNewBuildArea)
        {
            CenterChunkX = X >> 3;
            CenterChunkY = Y >> 3;
            RegionId = (((X >> 6) << 8) & 0xFF00) | ((Y >> 6) & 0xFF);
            OffsetChunkX = CenterChunkX - 6;
            OffsetChunkY = CenterChunkY - 6;
            BuildAreaStartX = OffsetChunkX << 3;
            BuildAreaStartY = OffsetChunkY << 3;
        }
    }

    public void Build()
    {
        RegionId = (((X >> 6) << 8) & 0xFF00) | ((Y >> 6) & 0xFF);
        
        CenterChunkX = X >> 3;
        CenterChunkY = Y >> 3;
        
        OffsetChunkX = CenterChunkX - 6;
        OffsetChunkY = CenterChunkY - 6;
        
        BuildAreaStartX = OffsetChunkX << 3;
        BuildAreaStartY = OffsetChunkY << 3;
        
        
        
        _cachedCenterChunkX = X >> 3;
        _cachedCenterChunkY = Y >> 3;
        
        _cachedBuildAreaSWChunkX = (X >> 3) - 6;
        _cachedBuildAreaSWChunkY = (Y >> 3) - 6;
        
        _cachedBuildAreaStartX = ((X >> 3) - 6) << 3;
        _cachedBuildAreaStartY = ((Y >> 3) - 6) << 3;
    }

    public bool IsWithinArea(Location entityLocation)
    {
        var delta = Delta(this, entityLocation);
        return delta.X <= 14 && delta.X >= -15 && delta.Y <= 14 && delta.Y >= -15 && entityLocation.Z == Z;
    }

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
        var messageParts = new List<string>
        {
            $"X: {X} Y: {Y}",
            $"CenterChunkX: {CenterChunkX} CenterChunkY: {CenterChunkY}",
            $"RegionId: {RegionId} OffsetChunkX: {OffsetChunkX} OffsetChunkY: {OffsetChunkY}",
            $"BuildAreaStartX: {BuildAreaStartX} BuildAreaStartY: {BuildAreaStartY}",
            $"PositionRelativeToOffsetChunkX: {PositionRelativeToOffsetChunkX} PositionRelativeToOffsetChunkY: {PositionRelativeToOffsetChunkY}",
            $"IsOutside: {ShouldGenerateNewBuildArea}"
        };

        return messageParts;
    }
}