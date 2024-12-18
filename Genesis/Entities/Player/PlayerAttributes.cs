using ArcticRS.Appearance;
using ArcticRS.Constants;

namespace Genesis.Entities;

public class PlayerAttributes
{
    public Gender Gender { get; set; } = Gender.Male;
    public HeadIcon HeadIcon { get; set; } = HeadIcon.None;
    public PlayerRights Rights { get; set; } = PlayerRights.NORMAL;
    
    public TorsoType Torso { get; set; } = TorsoType.RegularShirt;
    public ArmType Arms { get; set; } = ArmType.ShortSleeveLessMuscles;
    public LegType Legs { get; set; } = LegType.RegularTightPants;
    public HandType Hands { get; set; } = HandType.NoCuffs;
    public FeetType Feet { get; set; } = FeetType.SmallerFeet;
    public HairType Hair { get; set; } = HairType.Spiky;
    public BeardType Beard { get; set; } = BeardType.LongBeard;
}