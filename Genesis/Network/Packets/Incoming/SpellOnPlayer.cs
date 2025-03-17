﻿using Genesis.Entities;
using Genesis.Environment;
using Genesis.Interactions;
using Genesis.Model;
using Genesis.Skills.Combat;

namespace Genesis.Packets.Incoming;

public class SpellOnPlayer : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _index;
    private readonly int _spellId;

    public SpellOnPlayer(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
        _index = _player.Session.Reader.ReadSignedWordA();
        _spellId = _player.Session.Reader.ReadSignedWordBigEndian();
    }

    public void Process()
    {
        /* 12891 Ice barrage */

        if (_player.CurrentHealth <= 0)
        {
            return;
        }
        
        var weapon = new Weapon(_spellId, 5, 1979, null, new Gfx(369, 0, 0), 3);

        _player.Session.PacketBuilder.SendMessage($"PlayerIndex: {_index}");
        _player.Session.PacketBuilder.SendMessage($"SpellId: {_spellId}");
        _player.InteractingEntity = World.GetPlayers()[_index - 1];
        _player.CurrentInteraction = new SpellOnPlayerInteraction(_player, _player.InteractingEntity as Player, weapon);
    }
}