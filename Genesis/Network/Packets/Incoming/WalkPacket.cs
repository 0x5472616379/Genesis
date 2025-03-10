﻿using Genesis.Entities;
using Genesis.Interactions;
using Genesis.Movement;

namespace Genesis.Packets.Incoming;

public class WalkPacket : IPacket
{
    private int _destX;
    private int _destY;
    private readonly int _firstStepX;
    private readonly int _firstStepY;
    private readonly int _length;
    private readonly int _opCode;
    private readonly int[,] _path;
    private readonly Player _player;
    private readonly bool _running;
    private readonly int _waypoints;

    public WalkPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opCode = parameters.OpCode;
        _length = parameters.Length;

        _destX = -1;
        _destY = -1;

        if (_opCode == 248)
            _length -= 14;

        _waypoints = (_length - 5) / 2;
        _path = new int[_waypoints, 2];

        _firstStepX = _player.Session.Reader.ReadSignedWordBigEndianA();
        for (var i = 0; i < _waypoints; i++)
        {
            _path[i, 0] = (sbyte)_player.Session.Reader.ReadUnsignedByte();
            _path[i, 1] = (sbyte)_player.Session.Reader.ReadUnsignedByte();
        }

        _firstStepY = _player.Session.Reader.ReadSignedWordBigEndian();
        _running = _player.Session.Reader.ReadSignedByteC() == 1;

        _player.SetFacingEntity(null);
        _player.MovementHandler.Reset();
        _player.Session.PacketBuilder.ClearAllInterfaces();
    }

    public void Process()
    {
        if (_player.CurrentHealth <= 0)
            return;

       

        // _player.FollowingEntity = null;
        // _player.SetInteractingEntity(null);
        // _player.MostRecentCombatTarget = null;
        // _player.FollowingEntity = null;

        _player.MovementHandler.RunToggled = _running;

        _destX = _firstStepX;
        _destY = _firstStepY;

        for (var i = 0; i < _waypoints; i++)
        {
            _path[i, 0] += _firstStepX;
            _path[i, 1] += _firstStepY;

            _destX = _path[i, 0];
            _destY = _path[i, 1];
        }

        _player.MovementHandler.TargetDestX = _destX;
        _player.MovementHandler.TargetDestY = _destY;
        
        if (_opCode == 98 || _opCode == 73)
            return;
        
        _player.ClearInteraction();
        _player.Following = null;
        
        // RSPathfinder.FindPath(_player, _destX, _destY, true, 1, 1);
        // _player.MovementHandler.Finish();
        
        _player.CurrentInterraction = new MoveInteraction(_player);
    }
}