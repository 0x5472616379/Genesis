namespace Genesis.Skills
{

    public class SkillData
    {
        public SkillType SkillType { get; }
        public int PrimaryId { get; }
        public int SecondaryId { get; }
        public int TertiaryId { get; }
        public int InterfaceId1 { get; }
        public int InterfaceId2 { get; }
        public int DisplayId1 { get; }
        public int DisplayId2 { get; }

        public SkillData(SkillType skillType, int primaryId, int secondaryId, int tertiaryId,
                        int interfaceId1, int interfaceId2, int displayId1, int displayId2)
        {
            SkillType = skillType;
            PrimaryId = primaryId;
            SecondaryId = secondaryId;
            TertiaryId = tertiaryId;
            InterfaceId1 = interfaceId1;
            InterfaceId2 = interfaceId2;
            DisplayId1 = displayId1;
            DisplayId2 = displayId2;
        }
    }

    public static class SkillRepository
    {
        public static readonly List<SkillData> Skills = new()
        {
            new SkillData(SkillType.ATTACK, 6247, 6248, 6249, 4004, 4005, 4044, 4045),
            new SkillData(SkillType.DEFENCE, 6253, 6254, 6255, 4008, 4009, 4056, 4057),
            new SkillData(SkillType.STRENGTH, 6206, 6207, 6208, 4006, 4007, 4050, 4051),
            new SkillData(SkillType.HITPOINTS, 6216, 6217, 6218, 4016, 4017, 4080, 4081),
            new SkillData(SkillType.RANGED, 4443, 5453, 6114, 4010, 4011, 4062, 4063),
            new SkillData(SkillType.PRAYER, 6242, 6243, 6244, 4012, 4013, 4068, 4069),
            new SkillData(SkillType.MAGIC, 6211, 6212, 6213, 4014, 4015, 4074, 4075),
            new SkillData(SkillType.COOKING, 6226, 6227, 6228, 4034, 4035, 4134, 4135),
            new SkillData(SkillType.WOODCUTTING, 4272, 4273, 4274, 4038, 4039, 4146, 4147),
            new SkillData(SkillType.FLETCHING, 6231, 6232, 6233, 4026, 4027, 4110, 4111),
            new SkillData(SkillType.FISHING, 6258, 6259, 6260, 4032, 4033, 4128, 4129),
            new SkillData(SkillType.FIREMAKING, 4282, 4283, 4284, 4036, 4037, 4140, 4141),
            new SkillData(SkillType.CRAFTING, 6263, 6264, 6265, 4024, 4025, 4104, 4105),
            new SkillData(SkillType.SMITHING, 6221, 6222, 6223, 4030, 4031, 4122, 4123),
            new SkillData(SkillType.MINING, 4416, 4417, 4438, 4028, 4029, 4116, 4117),
            new SkillData(SkillType.HERBLORE, 6237, 6238, 6239, 4020, 4021, 4092, 4093),
            new SkillData(SkillType.AGILITY, 4277, 4278, 4279, 4018, 4019, 4086, 4087),
            new SkillData(SkillType.THIEVING, 4261, 4263, 4264, 4022, 4023, 4098, 4099),
            new SkillData(SkillType.SLAYER, 12122, 12123, 12124, 12166, 12167, 12171, 12172),
            new SkillData(SkillType.FARMING, 12122, 12123, 12124, 13926, 13927, 13921, 13922),
            new SkillData(SkillType.RUNECRAFTING, 4267, 4268, 4269, 4152, 4153, 4157, 4159)
        };

        public static SkillData GetSkill(SkillType skillType)
        {
            return Skills.FirstOrDefault(skill => skill.SkillType == skillType);
        }
    }
}