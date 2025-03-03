using Genesis.Environment;
using Genesis.Movement;

namespace Genesis.Entities;

public abstract class Entity
{
    public Location Location { get; set; } = new(ServerConfig.SPAWN_LOCATION_X,
                                                 ServerConfig.SPAWN_LOCATION_Y,
                                                 ServerConfig.SPAWN_LOCATION_Z);

    public MovementHandler MovementHandler { get; set; }
    
    public int CurrentHealth { get; set; } = 10;
    public int CurrentGfx { get; set; }
    public int CurrentFaceX { get; set; }
    public int CurrentFaceY { get; set; }
    public int CurrentAnimation { get; set; }
    public abstract void SetCurrentAnimation(int animationId, int delay);
    public abstract void SetCurrentGfx(int gfx);
    public abstract void SetFaceX(int x);
    public abstract void SetFaceY(int y);
}