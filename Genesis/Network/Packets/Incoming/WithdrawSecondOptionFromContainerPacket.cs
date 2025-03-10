using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Model;
using Genesis.Shop;

namespace Genesis.Packets.Incoming;

public class WithdrawSecondOptionFromContainerPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _fromContainer;
    private readonly int _itemId;
    private readonly int _from;
    private readonly int _amount;

    public WithdrawSecondOptionFromContainerPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
        
        _fromContainer = _player.Session.Reader.ReadUnsignedWordBigEndianA();
        _itemId = _player.Session.Reader.ReadUnsignedWordBigEndianA();
        _from = _player.Session.Reader.ReadUnsignedWordBigEndian();

        Console.WriteLine($"From Container: {_fromContainer}");
        Console.WriteLine($"FromIndex: {_itemId}");
        Console.WriteLine($"ItemId: {_from}");
    }
    
    public void Process()
    {
        /* Bank Inventory Container */
        if (_fromContainer == GameInterfaces.BankInventoryContainer)
        {
            InventorySystem.Transfer(_player.InventoryItemContainer, _player.BankItemContainer, _itemId, 5);
            _player.InventoryItemContainer.CopyToContainer(_player.BankInventoryItemContainer);

            _player.BankItemContainer.Refresh(_player, GameInterfaces.DefaultBankContainer);
            _player.BankInventoryItemContainer.Refresh(_player, GameInterfaces.BankInventoryContainer);
            _player.InventoryItemContainer.Refresh(_player, GameInterfaces.DefaultInventoryContainer);

        }

        if (_fromContainer == GameInterfaces.DefaultBankContainer)
        {
            InventorySystem.Transfer(_player.BankItemContainer, _player.InventoryItemContainer, _itemId, 5);
            _player.InventoryItemContainer.CopyToContainer(_player.BankInventoryItemContainer);
            
            _player.BankItemContainer.Refresh(_player, GameInterfaces.DefaultBankContainer);
            _player.BankInventoryItemContainer.Refresh(_player, GameInterfaces.BankInventoryContainer);
            _player.InventoryItemContainer.Refresh(_player, GameInterfaces.DefaultInventoryContainer);
        }
        
        if (_fromContainer == GameInterfaces.DefaultShopWindowContainer)
        {
            Shops.GeneralStore.BuyItem(_player, _from, 1);
        }
        
        if (_fromContainer == GameInterfaces.DefaultShopInventoryContainer)
        {
            Shops.GeneralStore.SellItem(_player, _from, 1);
        }
    }
}