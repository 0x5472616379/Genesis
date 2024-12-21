using Genesis.Environment;

namespace Genesis;

public class ServerConfig
{
    public static int TICK_RATE = 600;
    public static int PORT = 43594;
    public static int MAX_PLAYERS = 2048;
    public static int MAX_NPCS = 8192;
    public static int BUFFER_SIZE = 4096;
    public const int PACKET_FETCH_LIMIT = 50;

    public static int SPAWN_LOCATION_X = 3512;
    public static int SPAWN_LOCATION_Y = 3480;
    public static int SPAWN_LOCATION_Z = 0;
}