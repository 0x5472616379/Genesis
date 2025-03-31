using ArcticRS.Actions;
using ArcticRS.Constants;
using Genesis.Commands;
using Genesis.Entities.Player;
using Genesis.Environment;

namespace ArcticRS.Commands;

public class TeleportCommand : RSCommand
{
    private int _x, _y, _z;

    public static readonly Dictionary<string, (int, int, int)> NamedLocations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Big_water_fall"] = (2534, 3511, 0),
        ["Very_back_of_wild"] = (3100, 3957, 0),
        ["Varrock_east_bank"] = (3250, 3423, 0),
        ["Varrock"] = (3210, 3424, 0),
        ["Falador"] = (2964, 3378, 0),
        ["Lumbridge"] = (3222, 3218, 0),
        ["Camelot"] = (2757, 3477, 0),
        ["East_Ardougne"] = (2662, 3305, 0),
        ["West_Ardougne"] = (2529, 3307, 0),
        ["King_Lathas_Training_Grounds"] = (2516, 3369, 0),
        ["Tree_Gnome_Stronghold"] = (2461, 3443, 0),
        ["Al_Kharid"] = (3293, 3174, 0),
        ["Shantays_Pass"] = (3304, 3116, 0),
        ["Kalphite_Lair"] = (3226, 3107, 0),
        ["Pyramid"] = (3233, 2902, 0),
        ["Pollnivneach"] = (3359, 2910, 0),
        ["Menaphos/Sophanem"] = (3274, 2784, 0),
        ["Yanille"] = (2606, 3093, 0),
        ["Gul'Tanoth"] = (2516, 3044, 0),
        ["Tutorial_Island"] = (3094, 3107, 0),
        ["Barbarian_Village"] = (3082, 3420, 0),
        ["Entrana"] = (2834, 3335, 0),
        ["Heroes_Guild"] = (2902, 3510, 0),
        ["Rangers_Guild"] = (2658, 3439, 0),
        ["Coal_Trucks"] = (2582, 3481, 0),
        ["Goblin_Village"] = (2956, 3506, 0),
        ["Druids_Circle"] = (2926, 3482, 0),
        ["Burthorpe"] = (2926, 3559, 0),
        ["White_wolf_mountain"] = (2848, 3498, 0),
        ["Catherby"] = (2813, 3447, 0),
        ["Seers_village"] = (2708, 3492, 0),
        ["Fishing_guild"] = (2603, 3414, 0),
        ["Barbarian_Agility_Course"] = (2541, 3546, 0),
        ["Prifddinas"] = (2242, 3278, 0),
        ["Elf_camp_(Tirannwn)"] = (2197, 3252, 0),
        ["Isafdar_(Tirannwn)"] = (2241, 3238, 0),
        ["Duel_arena"] = (3360, 3213, 0),
        ["Hill_giants"] = (3111, 9836, 0),
        ["Desert_mining_camp"] = (3286, 3023, 0),
        ["Bedabin_camp"] = (3171, 3026, 0),
        ["Bandit_camp"] = (3176, 2987, 0),
        ["Sophanem"] = (3305, 2755, 0),
        ["Ruins_of_Uzer"] = (3490, 3090, 0),
        ["Mort'ton"] = (3489, 3288, 0),
        ["Canifis"] = (3506, 3496, 0),
        ["Port_Phasmatys"] = (3687, 3502, 0),
        ["Fenkenstrain's_castle"] = (3550, 3548, 0),
        ["Dig_site"] = (3354, 3402, 0),
        ["Exam_centre"] = (3354, 3344, 0),
        ["Edgeville"] = (3093, 3493, 0),
        ["Crafting_guild"] = (2933, 3285, 0),
        ["Port_Sarim"] = (3023, 3208, 0),
        ["Rimmington"] = (2957, 3214, 0),
        ["Draynor_village"] = (3093, 3244, 0),
        ["Fight_arena"] = (2585, 3150, 0),
        ["Tree_gnome_village"] = (2525, 3167, 0),
        ["Port_Khazard"] = (2665, 3161, 0),
        ["Monastery"] = (3051, 3490, 0),
        ["Karamja"] = (2948, 3147, 0),
        ["Crandor"] = (2851, 3238, 0),
        ["King_Black_Dragon_Lair"] = (2717, 9816, 0),
        ["KQ_Lair"] = (3487, 9493, 0),
        ["Underground_pass_level_2"] = (2337, 9798, 0),
        ["Weird_water_place"] = (2676, 3008, 0),
        ["Members_Karajama"] = (2815, 3182, 0),
        ["Grave_island"] = (3504, 3575, 0),
        ["Underground_pass_level_1"] = (2495, 9715, 0),
        ["Mage_Arena"] = (3107, 3937, 0),
        ["Ape_Atoll"] = (2755, 2784, 0),
        ["Jungle_demon_area_bottom_level"] = (2714, 9183, 0),
        ["Jungle_demon_area_top_level_(use_Burst_of_Strength)"] = (2703, 9178, 0),
        ["Wilderness_Agility_Course"] = (3003, 3934, 0),
        ["Wizard_tower"] = (3110, 3158, 0),
        ["Keldagram"] = (2937, 9999, 0),
        ["Agility_arena_on_Karamja"] = (2761, 9557, 0),
        ["Metal_dragon_dungeon_(back)"] = (2713, 9459, 0),
        ["Metal_dragon_dungeon_(front)"] = (2713, 9564, 0),
        ["Inside_the_desert_treasure_pyramid"] = (3233, 9315, 0),
        ["Tree_gnome_hanger"] = (2390, 9886, 0),
        ["Fishing_platform"] = (2782, 3273, 0),
        ["Boat_crash_island_place"] = (2795, 3321, 0),
        ["Legends_quest_jungle_dungeon"] = (2772, 9341, 0),
        ["TzHaar"] = (2480, 5175, 0),
        ["Essence_mine"] = (2911, 4832, 0),
        ["Morings_end_part_1_mine"] = (2044, 4649, 0),
        ["Dwarf_cut_scene_meeting_area"] = (2035, 4529, 0),
        ["New_KBD_lair"] = (2273, 4695, 0),
        ["Elven_forest_cut_scene_area"] = (2309, 4597, 0),
        ["Gold_mine_rock"] = (2358, 4959, 0),
        ["White_knight_tasks_area"] = (2443, 4956, 0),
        ["Rouge_maze_(Press_Burst_of_Strength)"] = (3050, 5071, 0),
        ["The_shadow_guys_dungeon"] = (2738, 5081, 0),
        ["Barrows_tunnel"] = (3568, 9695, 0),
        ["Barrows_chest"] = (3551, 9694, 0),
        ["CW_bank"] = (2441, 3090, 0),
        ["CW_center"] = (2400, 3103, 0),
        ["Blue_dragons"] = (2910, 9801, 0),
        ["Black_demons"] = (2860, 9775, 0),
        ["Hell_Hounds"] = (2867, 9844, 0),
        ["Black_dragons"] = (2829, 9826, 0),
        ["Chaos_rune_crafting_alter"] = (2269, 4843, 0),
        ["Legends_dungeon_prt2"] = (2375, 4705, 0),
        ["Mage_arena_dungeon"] = (2519, 4719, 0),
        ["Evil_Bobs_island_'scaperune'"] = (2525, 4776, 0),
        ["Distant_kingdom_(alive)"] = (2576, 4655, 0),
        ["Distant_kingdom_(dead)"] = (2803, 4723, 0),
        ["Forester_random_event"] = (2602, 4775, 0),
        ["Alt_area"] = (2527, 4547, 0),
        ["Fremmy_maze"] = (2652, 9999, 0),
        
        ["Air_rune_altar"] = (2841, 4829, 0),
        ["Mind_rune_altar"] = (2793, 4828, 0),
        ["Water_rune_altar"] = (2725, 4832, 0),
        ["Earth_rune_altar"] = (2655, 4830, 0),
        ["Fire_rune_altar"] = (2575, 4848, 0),
        ["Body_rune_altar"] = (2523, 4826, 0),
        ["Cosmic_rune_altar"] = (2162, 4833, 0),
        ["Chaos_rune_altar"] = (2281, 4837, 0),
        ["Nature_rune_altar"] = (2400, 4835, 0),
        ["Law_rune_altar"] = (2464, 4818, 0),
        ["Death_rune_altar"] = (2208, 4830, 0),
        
        ["Cosmic_alter"] = (2162, 4833, 0),
        ["Middle_of_under_ground_pass"] = (2168, 4726, 0),
        ["Quiz_random_event"] = (1952, 4768, 0),
        ["Trawler_boat"] = (2013, 4820, 0),
        ["West_dragons"] = (2976, 3615, 0),
        ["Sunk_trawler_mini_game_boat"] = (1951, 4825, 0),
        ["Trawler_boat_empty_of_water"] = (1886, 4830, 0),
        ["Mime_stage"] = (2008, 4762, 0),
        ["Draynor"] = (2130, 4913, 0),
        ["Game_room"] = (2196, 4961, 0),
        ["Mind_alter"] = (2796, 4818, 0),
        ["Air_Alter"] = (2845, 4832, 0),
        ["Water_Alter"] = (2713, 4836, 0),
        ["Earth_Alter"] = (2660, 4839, 0),
        ["Fire_Alter"] = (2584, 4836, 0),
        ["Body_Alter"] = (2527, 4833, 0),
        ["Law_Alter"] = (2464, 4834, 0),
        ["Nature_Alter"] = (2398, 4841, 0),
        ["Gas_hole"] = (2464, 4782, 0),
        ["Edge_of_maze"] = (2885, 4550, 0),
        ["Center_of_maze"] = (2912, 4576, 0),
        ["Falador_mine"] = (3038, 9800, 0),
        ["drill_deamon_camp"] = (3157, 4822, 0),
        ["HAM_camp"] = (3165, 9629, 0),
        ["Pyramid_under_Sophanem"] = (3277, 9171, 0),
        ["Juna_the_snake"] = (3251, 9517, 2),
        ["castle_wars"] = (2379, 9489, 0),
        ["castle_wars1"] = (2422, 9525, 0),
        ["Ice_Lair"] = (2867, 9955, 0),
        ["Slayer_Tower"] = (3429, 3538, 0),
        ["Brimhaven_Dungeon"] = (2710, 9466, 0),
        ["Ice_Path"] = (2856, 3812, 0),
        ["Gnome_Agility_Training"] = (2480, 3437, 0),
        ["Pyramid_Plunder_room"] = (3281, 2765, 0),
        ["Duel_Arena"] = (3345, 3251, 0),
        ["King_Black_Dragon_Cave"] = (2717, 9808, 0),
        ["Rogues_Den"] = (3044, 4973, 1)
    };

    public TeleportCommand(Player player, string[] args) : base(player, args)
    {
    }

    protected override PlayerRights RequiredRights => PlayerRights.ADMIN;

    public override bool Validate()
    {
        if (Args.Length < 2)
        {
            Player.Session.PacketBuilder.SendMessage("Usage: ::teleport [location] or ::teleport [x] [y] [z]");
            return false;
        }

        if (Args.Length == 2 && NamedLocations.TryGetValue(Args[1].ToLower(), out var namedLocation))
        {
            (_x, _y, _z) = namedLocation;
        }
        else
        {
            if (Args.Length < 4 || !int.TryParse(Args[1], out _x) ||
                                   !int.TryParse(Args[2], out _y) ||
                                   !int.TryParse(Args[3], out _z))
            {
                Player.Session.PacketBuilder.SendMessage("Invalid location!");
                return false;
            }
        }

        return true;
    }

    public override void Invoke()
    {
        Player.ActionHandler.AddAction(new TeleAction(Player, new Location(_x, _y, _z)));
    }
}