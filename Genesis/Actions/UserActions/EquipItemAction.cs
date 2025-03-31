using ArcticRS.Appearance;
using Genesis.Configuration;
using Genesis.Definitions.Items;
using Genesis.Entities.Player;
using Genesis.Managers;
using Genesis.Model;

namespace ArcticRS.Actions;

public class EquipItemAction : RSAction
{
    private readonly Player _player;
    private readonly int _index;

    private readonly Dictionary<int, int> _weaponSpecialBars = new()
    {
        { 4151, GameInterfaces.WhipDefaultSpecialBar },
        { 861, GameInterfaces.MsbDefaultSpecialBar },
        { 859, GameInterfaces.MsbDefaultSpecialBar },
        { 1215, GameInterfaces.DragonDaggerDefaultSpecialBar },
        { 1231, GameInterfaces.DragonDaggerDefaultSpecialBar },
        { 5680, GameInterfaces.DragonDaggerDefaultSpecialBar },
        { 5698, GameInterfaces.DragonDaggerDefaultSpecialBar },
        { 4587, GameInterfaces.DragonScimitarDefaultSpecialBar }
    };

    public EquipItemAction(Player player, int index)
    {
        Priority = ActionPriority.Forceful;
        _player = player;
        _index = index;
    }

    public override bool Execute()
    {
        var item = _player.Inventory.GetItemAtIndex(_index);
        if (item.IsEmpty) return true;

        var slot = EquipmentManager.GetEquipmentSlotById(item.ItemId);
        if (slot == EquipmentSlot.None) return true;

        var itemId = item.ItemId;
        if (_player.Equipment.TryEquipItem(_player, _index, slot))
        {
            if (slot == EquipmentSlot.Weapon)
            {
                WeaponInterfaceManager.Refresh(_player);
                _player.CombatHelper.SpecialAttack = null;
                if (_weaponSpecialBars.TryGetValue(itemId, out int specialBarInterface))
                {
                    _player.Session.PacketBuilder.DisplayHiddenInterface(0, specialBarInterface);
                    _player.CombatHelper.UpdateSpecialAttack(specialBarInterface);
                }
                else
                {
                    _player.Session.PacketBuilder.DisplayHiddenInterface(1, GameInterfaces.WhipDefaultSpecialBar);
                    _player.Session.PacketBuilder.DisplayHiddenInterface(1, GameInterfaces.MsbDefaultSpecialBar);
                    _player.Session.PacketBuilder.DisplayHiddenInterface(1,
                        GameInterfaces.DragonScimitarDefaultSpecialBar);
                    _player.Session.PacketBuilder.DisplayHiddenInterface(1,
                        GameInterfaces.DragonHalberdDefaultSpecialBar);
                    _player.Session.PacketBuilder.DisplayHiddenInterface(1,
                        GameInterfaces.DragonBattleAxeDefaultSpecialBar);
                    _player.Session.PacketBuilder.DisplayHiddenInterface(1,
                        GameInterfaces.GraniteMaulDefaultSpecialBar);
                    _player.Session.PacketBuilder.DisplayHiddenInterface(1,
                        GameInterfaces.DragonSpearDefaultSpecialBar);
                    _player.Session.PacketBuilder.DisplayHiddenInterface(1,
                        GameInterfaces.DragonDaggerDefaultSpecialBar);
                    _player.Session.PacketBuilder.DisplayHiddenInterface(1, GameInterfaces.DragonMaceDefaultSpecialBar);
                }
            }

            _player.BonusManager.Reset();

            foreach (var itemslot in _player.Equipment._slots)
            {
                if (itemslot.ItemId == -1)
                    continue;

                var itemBonuses = ItemParser.GetBonusesById(itemslot.ItemId).Bonuses;
                _player.BonusManager.CalculateBonuses(itemBonuses);
            }

            _player.BonusManager.UpdateBonus();

            // _player.Inventory.RefreshContainer(_player, GameInterfaces.DefaultInventoryContainer);
            // _player.Flags |= PlayerUpdateFlags.Appearance;
            _player.EquippedItem = true;
        }

        return true;
    }
}