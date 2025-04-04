﻿using Genesis.Configuration;
using Genesis.Entities.Player;
using Genesis.Network;

namespace Genesis.Packets.Incoming;

public class CloseWindowPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;

    public CloseWindowPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
    }

    public void Process()
    {
        _player.OpenShop = null;
        // _player.Inventory.RefreshContainer(_player, GameInterfaces.DefaultInventoryContainer);
    }
}