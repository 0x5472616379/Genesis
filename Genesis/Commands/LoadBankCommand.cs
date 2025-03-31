using ArcticRS.Constants;
using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Entities.Player;

namespace Genesis.Commands;

public class LoadBankCommand : RSCommand
{
    private readonly Player _player;
    protected override PlayerRights RequiredRights { get; } = PlayerRights.ADMIN;
    
    Random _random = new Random();
    
    public LoadBankCommand(Player player, string[] args) : base(player, args)
    {
        _player = player;
    }

    public override bool Validate()
    {
        return true;
    }

    public override void Invoke()
    {
        // _player.BankItemContainer.Clear();
        // //
        // /* Coins */
        // _player.BankItemContainer.AddItem(995, int.MaxValue);
        //
        // /* Rares */
        // _player.BankItemContainer.AddItem(962, 10);
        //
        // for (int i = 0; i < 6; i++)
        // {
        //     var itemId = 1038 + (i * 2);
        //     _player.BankItemContainer.AddItem(itemId, 10);
        // }
        //
        // _player.BankItemContainer.AddItem(981, 10);
        // _player.BankItemContainer.AddItem(1050, 10);
        // _player.BankItemContainer.AddItem(1959, 10);
        //
        // _player.BankItemContainer.AddItem(1053, 5);
        // _player.BankItemContainer.AddItem(1055, 5);
        // _player.BankItemContainer.AddItem(1057, 5);
        //
        // _player.BankItemContainer.AddItem(2577, 2);
        // _player.BankItemContainer.AddItem(2581, 2);
        //
        //
        // /* Runes */
        // for (int i = 0; i < 13; i++)
        // {
        //     var itemid = 554;
        //     _player.BankItemContainer.AddItem(itemid + i, 100000000);
        // }
        //
        // /* Bones */
        // _player.BankItemContainer.AddItem(536, 100000);
        // _player.BankItemContainer.AddItem(534, 100000);
        // _player.BankItemContainer.AddItem(532, 100000);
        //
        // /* Potions */
        // _player.BankItemContainer.AddItem(2434, 1000);
        // _player.BankItemContainer.AddItem(2452, 1000);
        // _player.BankItemContainer.AddItem(2444, 1000);
        // _player.BankItemContainer.AddItem(2436, 1000);
        // _player.BankItemContainer.AddItem(2440, 1000);
        // _player.BankItemContainer.AddItem(2442, 1000);
        // _player.BankItemContainer.AddItem(385, 1000);
        // _player.BankItemContainer.AddItem(3144, 1000);
        //
        // /* Herbs */
        // for (int i = 0; i < 11; i++)
        // {
        //     var itemid = 199;
        //     _player.BankItemContainer.AddItem(itemid + (i * 2), new Random().Next(1000, 5001));
        // }
        //
        // /* Longbow */
        // _player.BankItemContainer.AddItem(845, new Random().Next(5000, 7001));
        // _player.BankItemContainer.AddItem(847, new Random().Next(5000, 7001));
        // _player.BankItemContainer.AddItem(851, new Random().Next(5000, 7001));
        // _player.BankItemContainer.AddItem(855, new Random().Next(5000, 7001));
        // _player.BankItemContainer.AddItem(859, new Random().Next(5000, 7001));
        //
        // /* Arrows */
        // for (int i = 0; i < 6; i++)
        // {
        //     var itemid = 882;
        //     _player.BankItemContainer.AddItem(itemid + (i * 2), new Random().Next(5000, 9001));
        // }
        //
        // _player.BankItemContainer.AddItem(53, new Random().Next(5000, 9001));
        // _player.BankItemContainer.AddItem(52, new Random().Next(15000, 25001));
        //
        // /* Gear */
        //
        // _player.BankItemContainer.AddItem(4716, 5);
        // _player.BankItemContainer.AddItem(4718, 5);
        //
        // _player.BankItemContainer.AddItem(3140, new Random().Next(2, 8));
        // _player.BankItemContainer.AddItem(1187, new Random().Next(5, 10));
        // _player.BankItemContainer.AddItem(4087, new Random().Next(2, 8));
        // _player.BankItemContainer.AddItem(3751, new Random().Next(5, 15));
        //
        // _player.BankItemContainer.AddItem(4131, new Random().Next(5, 10));
        // _player.BankItemContainer.AddItem(1580, new Random().Next(5, 10));
        //
        //
        // _player.BankItemContainer.AddItem(4720, 5);
        // _player.BankItemContainer.AddItem(4722, 5);
        //
        // _player.BankItemContainer.AddItem(1377, new Random().Next(5, 10));
        // _player.BankItemContainer.AddItem(1215, new Random().Next(5, 10));
        // _player.BankItemContainer.AddItem(4151, new Random().Next(5, 10));
        // _player.BankItemContainer.AddItem(4224, 1);
        // _player.BankItemContainer.AddItem(4212, 1);
        // _player.BankItemContainer.AddItem(1052, 1);
        //
        // /* Fillers */
        // var random = new Random();
        // while (_player.BankItemContainer.FreeSlots > 1)
        // {
        //     var itemId = random.Next(1, 4000);
        //     var item = ItemDefinition.Lookup(itemId);
        //     if (item == null || item.Id == 995 || item.IsNote()) continue;
        //
        //     _player.BankItemContainer.AddItem(item.Id, _random.Next(1, 100));
        // }
        //
        // _player.BankItemContainer.AddItem(732, 100000);
        //
        // // _player.BankItemContainer.CopyInventory(_player.InventoryManager.GetAllItemsIncNull());
        // _player.BankItemContainer.Refresh(_player, GameInterfaces.DefaultBankContainer);
    }
}