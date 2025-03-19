﻿using Genesis.Configuration;
using Genesis.Entities;

namespace Genesis.Packets.Incoming;

public class MoveItemInContainerPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _containerId;
    private readonly byte _insertionMode;
    private readonly int _from;
    private readonly int _to;

    public MoveItemInContainerPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;

        _containerId = _player.Session.Reader.ReadUnsignedWordBigEndianA();
        _insertionMode = _player.Session.Reader.ReadSignedByteC();
        _from = _player.Session.Reader.ReadSignedWordBigEndianA();
        _to = _player.Session.Reader.ReadSignedWordBigEndian();
    }

    public void Process()
    {
        Console.WriteLine($"ContainerId: {_containerId}");
        Console.WriteLine($"InsertionMode: {_insertionMode}");
        Console.WriteLine($"From: {_from}");
        Console.WriteLine($"To: {_to}");

        if (_containerId == GameInterfaces.DefaultInventoryContainer)
        {
            // _player.InventoryItemContainer.Swap(_from, _to);
        }

        if (_containerId == GameInterfaces.DefaultBankContainer)
        {
            // _player.BankItemContainer.Swap(_from, _to);
        }

        if (_containerId == GameInterfaces.BankInventoryContainer)
        {
            // _player.BankInventoryItemContainer.Swap(_from, _to);
            // _player.InventoryItemContainer.Swap(_from, _to);
            // _player.InventoryItemContainer.Refresh(_player, GameInterfaces.DefaultInventoryContainer);
        }
    }
}