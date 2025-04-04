﻿using ArcticRS.Actions;
using Genesis.Entities.Player;
using Genesis.Environment;
using Genesis.Managers;
using Genesis.Network;

namespace Genesis.Packets.Incoming;

public class FirstItemOptionPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;

    private readonly int _containerId;
    private readonly int _index;
    private readonly int _itemId;

    public FirstItemOptionPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;

        _containerId = _player.Session.Reader.ReadSignedWordBigEndianA();
        _index = _player.Session.Reader.ReadSignedWordA();
        _itemId = _player.Session.Reader.ReadSignedWordBigEndian();
    }

    public void Process()
    {
        if (_player.CurrentHealth <= 0)
            return;

        Console.WriteLine(_containerId);
        Console.WriteLine(_index);
        Console.WriteLine(_itemId);

        if (_itemId == 526)
        {
            if (_player.ActionHandler.ActionPipeline.Any(x => x is BuryAction))
                return;

            _player.ActionHandler.AddAction(new BuryAction(_player));
        }
        
        if (ConsumeManager.HardFoods.Contains(_itemId) || ConsumeManager.ComboFoods.Contains(_itemId) 
                                                       || ConsumeManager.Potions.Contains(_itemId))
        {
            _player.ConsumeManager.Consume(_itemId, _index);
            return;
        }
    }
}