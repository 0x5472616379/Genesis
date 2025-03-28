﻿namespace Genesis.Configuration;

public class GameInterfaces
{
    /* Container: When clicking / moving items */
    /* Interface: What's visible */
    
    /* Inventory */
    public static int EquipmentContainer { get; } = 1688;
    
    /* Default */
    public static int DefaultInventoryContainer { get; } = 3214;
    
    /* Bank */
    public static int DefaultBankContainer { get; } = 5382;
    public static int BankInventoryContainer { get; } = 5064;
    public static int BankWindowInterface { get; } = 5292;
    public static int BankInventorySidebarInterface { get; } = 5063;

    /* Shop */
    public static int ShopWindowTitleInterface { get; } = 3901;
    public static int DefaultShopWindowInterface { get; } = 3824;
    public static int DefaultShopWindowContainer { get; } = 3900;
    public static int DefaultShopInventoryInterface { get; } = 3822;
    public static int DefaultShopInventoryContainer { get; } = 3823;
    
    public static int WeaponInterface { get; } = 2423;
    public static int SkillInterface { get; } = 3917;
    public static int QuestInterface { get; } = 638;
    public static int InventoryInterface { get; } = 3213;
    public static int EquipmentInterface { get; } = 1644;
    public static int PrayerInterface { get; } = 5608;
    public static int NormalMagicInterface { get; } = 1151;
    public static int AncientMagiksInterface { get; } = 12855;
    public static int FriendsInterface { get; } = 5065;
    public static int IgnoreInterface { get; } = 5715;
    public static int LogoutInterface { get; } = 2449;
    public static int SettingsInterface { get; } = 4445;
    public static int PlayerControlsInterface { get; } = 147;
    public static int MusicInterface { get; } = 6299;
    
    public static int WhipDefaultSpecialBar { get; } = 12323;
    public static int MsbDefaultSpecialBar { get; } = 7549;
    public static int DragonScimitarDefaultSpecialBar { get; } = 7599;
    public static int DragonHalberdDefaultSpecialBar { get; } = 8493;
    public static int DragonBattleAxeDefaultSpecialBar { get; } = 7499;
    public static int GraniteMaulDefaultSpecialBar { get; } = 7474;
    public static int DragonSpearDefaultSpecialBar { get; } = 7674;
    public static int DragonDaggerDefaultSpecialBar { get; } = 7574;
    public static int DragonMaceDefaultSpecialBar { get; } = 7624;
}