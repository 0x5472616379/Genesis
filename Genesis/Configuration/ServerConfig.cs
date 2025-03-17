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
    
    public const int SKILL_BONUS_EXP = 100;
    public const int COMBAT_BONUS_EXP = 1;

    public static int SPAWN_LOCATION_X = 3223;
    public static int SPAWN_LOCATION_Y = 3218;
    public static int SPAWN_LOCATION_Z = 0;
    
    public static int ITEM_LIMIT = 15000;

    public static int BANK_SIZE = 352;
    public static int INVENTORY_SIZE = 28;

    public static bool ADDED_REGION_OBJECTS = false;
    
    
}