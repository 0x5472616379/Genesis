using Genesis.Configuration;
using Genesis.Entities;

namespace Genesis.Packets.Incoming;

public class WithdrawSingleItemFromContainerPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _fromContainer;
    private readonly int _from;
    private readonly int _itemId;

    public WithdrawSingleItemFromContainerPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;

        _fromContainer = _player.Session.Reader.ReadUnsignedWordA();
        _from = _player.Session.Reader.ReadUnsignedWordA();
        _itemId = _player.Session.Reader.ReadUnsignedWordA();

        Console.WriteLine($"From Container: {_fromContainer}");
        Console.WriteLine($"FromIndex: {_from}");
        Console.WriteLine($"ItemId: {_itemId}");
    }
    
    public void Process()
    {
        /* Bank Inventory Container */
        if (_fromContainer == GameInterfaces.BankInventoryContainer)
        {
            if (_player.InventoryManager.GetItemAtIndex(_from) == null) return;
                
            /* Add To Bank Container */
            int amountAdded = _player.BankManager.AddItem(_itemId, 1);
            _player.InventoryManager.Remove(_from, amountAdded);
            _player.InventoryManager.RefreshInventory();
            _player.BankManager.RefreshInventory();
        }

        if (_fromContainer == GameInterfaces.DefaultBankContainer)
        {
            if (_player.BankManager.GetItemAtIndex(_from) == null) return;
            
            int amountAdded = _player.InventoryManager.AddItem(_itemId, 1);
            _player.BankManager.Remove(_from, amountAdded);
            _player.BankManager.RefreshBankContainer();
            _player.InventoryManager.RefreshInventory();
            _player.BankManager.RefreshInventory();
        }
    }
}