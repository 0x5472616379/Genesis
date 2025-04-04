﻿using ArcticRS.Appearance;
using Genesis.Entities.Player;
using Genesis.Environment;
using Genesis.Interactions;
using Genesis.Model;
using Genesis.Network;
using Genesis.Skills.Combat;

namespace Genesis.Packets.Incoming;

public class AttackPlayerPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _index;

    public AttackPlayerPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
        _index = _player.Session.Reader.ReadSignedWordBigEndian();
    }

    public void Process()
    {
        if (_player.CurrentHealth <= 0)
        {
            return;
        }
        
        _player.PlayerMovementHandler.Reset();
        
        // _player.Following = World.GetPlayers()[_index - 1];
        _player.InteractingEntity = World.GetPlayers()[_index - 1];
        _player.SetFacingEntity(_player.InteractingEntity);
        _player.CurrentInteraction = new PlayerAttackInteraction(_player, _player.InteractingEntity as Player);
    }
}