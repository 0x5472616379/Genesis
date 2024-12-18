using Genesis.Environment;

namespace Genesis.Entities;

public abstract class Entity
{
    public Location Location { get; set; } = new(ServerConfig.SPAWN_LOCATION_X,
                                                 ServerConfig.SPAWN_LOCATION_Y,
                                                 ServerConfig.SPAWN_LOCATION_Z);

    public int CurrentHealth { get; set; } = 10;
    public int CurrentGfx { get; set; }
    public int CurrentAnimation { get; set; }
}