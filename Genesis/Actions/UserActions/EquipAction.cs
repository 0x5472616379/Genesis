using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Model;

namespace ArcticRS.Actions;

enum EquipState
{
    IDLE,
    EQUIPPING
}

// public class EquipAction : RSAction
// {
//     private readonly Player _player;
//     private readonly RSItem _item;
//     private readonly int _index;
//     private EquipState _state = EquipState.IDLE;
//     public override ActionCategory Category { get; set; }
//     public override Func<bool> CanExecute { get; set; }
//     public override Func<bool> Execute { get; set; }
//
//     public EquipAction(Player player, RSItem item, int index)
//     {
//         _player = player;
//         _item = item;
//         _index = index;
//         Execute = EquipItem;
//         CanExecute = CanEquip;
//     }
//
//     private bool EquipItem()
//     {
//         switch (_state)
//         {
//             case EquipState.IDLE:
//                 _state = EquipState.EQUIPPING;
//                 _player.EquipmentManager.Equip(new RSItem(_item.Id, _item.Amount, _index));
//                 _player.Session.PacketBuilder.UpdateSlot(_index, _item.Id, 1, GameInterfaces.DefaultInventoryContainer);
//                 return false;
//             case EquipState.EQUIPPING:
//                 return true;
//             default:
//                 throw new ArgumentOutOfRangeException();
//         }
//     }
//
//     private bool CanEquip() => true;
//     
// }