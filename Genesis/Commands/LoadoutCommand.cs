using ArcticRS.Constants;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Skills;

namespace Genesis.Commands;

public class LoadoutCommand : RSCommand
{
    private string _loadout;
    protected override PlayerRights RequiredRights { get; } = PlayerRights.ADMIN;

    public LoadoutCommand(Player player, string[] args) : base(player, args)
    {
    }

    public override bool Validate()
    {
        if (Args.Length < 1)
        {
            return false;
        }

        _loadout = Args[0];
        return true;
    }

    public override void Invoke()
    {
        GetLoadout(_loadout).Invoke();
    }

    public Action GetLoadout(string loadout) => loadout switch
    {
        "pure" => PureLoadout,
        _ => () => { Player.Session.PacketBuilder.SendMessage($"[{loadout}] Loadout not found."); }
    };

    void PureLoadout()
    {
        Player.SkillManager.Skills[(int)SkillType.ATTACK].SetLevel(60);
        Player.SkillManager.Skills[(int)SkillType.STRENGTH].SetLevel(99);
        Player.SkillManager.Skills[(int)SkillType.MAGIC].SetLevel(94);
        Player.SkillManager.Skills[(int)SkillType.RANGED].SetLevel(99);
        
        Player.CurrentHealth = 99;
        Player.SkillManager.Skills[(int)SkillType.HITPOINTS].SetLevel(99);
        
        Player.Inventory.Clear();
        Player.Equipment.ClearAll();
        Player.Inventory.AddItem(6107, 1); /* Ghost Robe Top*/
        Player.Inventory.AddItem(3842, 1); /* unholy book */
        Player.Inventory.AddItem(6570, 1); /* Fire cape */
        Player.Inventory.AddItem(2581, 1); /* Robin Hood Hat*/
        Player.Inventory.AddItem(2497, 1); /* Black dhide chaps */
        Player.Inventory.AddItem(5698, 1); /* Dds */
        Player.Inventory.AddItem(6737, 1); /* Bring */
        Player.Inventory.AddItem(2577, 1); /* Ranger Boots */
        Player.Inventory.AddItem(861, 1); /* MSB */
        Player.Inventory.AddItem(892, 1000); /* Rune Arrows */
        Player.Inventory.AddItem(6585, 1); /* Amulet of Fury */
        Player.Inventory.AddItem(2491, 1); /* Black dhide vambs */
        
        Player.Inventory.RefreshContainer(Player, GameInterfaces.DefaultInventoryContainer);
        Player.Equipment.RefreshContainer(Player, GameInterfaces.EquipmentContainer);
        Player.SkillManager.RefreshSkills();
        Player.Flags |= PlayerUpdateFlags.Appearance;
    }
}