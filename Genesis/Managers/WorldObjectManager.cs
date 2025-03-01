namespace Genesis.Managers;

public class WorldObjectManager
{
    public List<WorldObject> DESPAWN { get; set; }
}

public class WorldObject
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int Face { get; set; }
    public int Id { get; set; }
    public int Type { get; set; }
}