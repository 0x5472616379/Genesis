namespace Genesis.Model;

public class Gfx
{
    public short Id { get; set; }
    public short Height { get; set; }
    public short Delay { get; set; }
    
    public Gfx(short id, short height, short delay)
    {
        Id = id;
        Height = height;
        Delay = delay;
    }

}