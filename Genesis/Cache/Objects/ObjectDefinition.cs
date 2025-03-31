namespace Genesis.Cache.Objects;

public class ObjectDefinition(int id)
{
    public int Id { get; } = id;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool Wall { get; set; }
    public bool IsInteractive { get; set; }
    public bool IsObstructive { get; set; }
    public bool IsSolid { get; set; } = true;
    public bool IsImpenetrable { get; set; } = true;
    public bool IsClipped { get; set; } = true;
    public int Width { get; set; } = 1;
    public int Length { get; set; } = 1;
    public int? Face { get; set; }
    public string[]? MenuActions { get; set; }

    public static ObjectDefinition[] Definitions { get; private set; } = [];

    public static int Count => Definitions.Length;

    public static void Init(ObjectDefinition[] defs)
    {
        Definitions = defs;

        for (var i = 0; i < defs.Length; i++)
            if (defs[i].Id != i)
                throw new Exception("Item definition id mismatch.");
    }

    public static ObjectDefinition Lookup(int id)
    {
        if (id >= Definitions.Length) 
            throw new IndexOutOfRangeException("Id out of bounds.");

        return Definitions[id];
    }
}