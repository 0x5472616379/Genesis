using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Model;

namespace Genesis.Packets.Incoming;

public class WithdrawAllFromContainerPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _fromContainer;
    private readonly int _itemId;
    private readonly int _from;
    private readonly int _amount;

    public WithdrawAllFromContainerPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
        
        _from = _player.Session.Reader.ReadUnsignedWordA();
        _fromContainer = _player.Session.Reader.ReadUnsignedWord();
        _itemId = _player.Session.Reader.ReadUnsignedWordA();

        Console.WriteLine($"From Container: {_fromContainer}");
        Console.WriteLine($"FromIndex: {_itemId}");
        Console.WriteLine($"ItemId: {_from}");
    }
    
    public void Process()
    {
        /* Bank Inventory Container */
        if (_fromContainer == GameInterfaces.BankInventoryContainer)
        {
            InventorySystem.Transfer(_player.InventoryItemContainer, _player.BankItemContainer, _itemId, int.MaxValue);
            _player.InventoryItemContainer.CopyToContainer(_player.BankInventoryItemContainer);

            _player.BankItemContainer.Refresh(_player, GameInterfaces.DefaultBankContainer);
            _player.BankInventoryItemContainer.Refresh(_player, GameInterfaces.BankInventoryContainer);
            _player.InventoryItemContainer.Refresh(_player, GameInterfaces.DefaultInventoryContainer);

        }

        if (_fromContainer == GameInterfaces.DefaultBankContainer)
        {
            InventorySystem.Transfer(_player.BankItemContainer, _player.InventoryItemContainer, _itemId, int.MaxValue);
            _player.InventoryItemContainer.CopyToContainer(_player.BankInventoryItemContainer);
            
            _player.BankItemContainer.Refresh(_player, GameInterfaces.DefaultBankContainer);
            _player.BankInventoryItemContainer.Refresh(_player, GameInterfaces.BankInventoryContainer);
            _player.InventoryItemContainer.Refresh(_player, GameInterfaces.DefaultInventoryContainer);
        }
    }
}