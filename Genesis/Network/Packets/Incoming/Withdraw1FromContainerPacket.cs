using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Model;

namespace Genesis.Packets.Incoming;

public class Withdraw1FromContainerPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _fromContainer;
    private readonly int _from;
    private readonly int _itemId;
    private readonly int _amount;

    public Withdraw1FromContainerPacket(PacketParameters parameters)
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
        var item = ItemDefinition.Lookup(_itemId);

        /* Bank Inventory Container */
        if (_fromContainer == GameInterfaces.BankInventoryContainer)
        {
            InventorySystem.Transfer(_player.InventoryItemContainer, _player.BankItemContainer, _itemId, 1);
            _player.InventoryItemContainer.CopyToContainer(_player.BankInventoryItemContainer);

            _player.BankItemContainer.Refresh(_player, GameInterfaces.DefaultBankContainer);
            _player.BankInventoryItemContainer.Refresh(_player, GameInterfaces.BankInventoryContainer);
            _player.InventoryItemContainer.Refresh(_player, GameInterfaces.DefaultInventoryContainer);

        }

        if (_fromContainer == GameInterfaces.DefaultBankContainer)
        {
            InventorySystem.Transfer(_player.BankItemContainer, _player.InventoryItemContainer, _itemId, 1);
            _player.InventoryItemContainer.CopyToContainer(_player.BankInventoryItemContainer);
            
            _player.BankItemContainer.Refresh(_player, GameInterfaces.DefaultBankContainer);
            _player.BankInventoryItemContainer.Refresh(_player, GameInterfaces.BankInventoryContainer);
            _player.InventoryItemContainer.Refresh(_player, GameInterfaces.DefaultInventoryContainer);
        }
    }
}