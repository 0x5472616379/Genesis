﻿using Genesis.Entities;
using Genesis.Environment;

namespace ArcticRS.Actions;

public class RespawnAction : RSAction
{
    private readonly Player _player;
    private RespawnPhase _currentPhase;

    public RespawnAction(Player player)
    {
        _player = player;
        Priority = ActionPriority.Forceful;
        ScheduledTick = World.CurrentTick + 1;
    }

    public override bool Execute()
    {
        switch (_currentPhase)
        {
            case RespawnPhase.Fall: // Start respawn process
                StartRespawn();
                ScheduleNext(3);
                _currentPhase = RespawnPhase.Spawn;
                return false;

            case RespawnPhase.Spawn: // Perform actual teleport and spawn
                FinalizePlayerSpawn();
                return true;

            default:
                return false;
        }
    }

    private void FinalizePlayerSpawn()
    {
        _player.Location.X = 3222;
        _player.Location.Y = 3218;
        _player.Location.Z = 0;

        _player.PerformedTeleport = true;
        _player.Location.Build();
        _player.Session.PacketBuilder.SendNewBuildAreaPacket();
        EnvironmentBuilder.UpdateBuildArea(_player);
        _player.SetCurrentAnimation(-1);
        _player.CurrentHealth = 10;
        _currentPhase = RespawnPhase.Spawn;
    }

    private void StartRespawn()
    {
        _player.SetCurrentAnimation(836);
        _player.PlayerMovementHandler.Reset();
    }

    private void ScheduleNext(int ticks)
    {
        ScheduledTick = World.CurrentTick + ticks;
    }

    private enum RespawnPhase
    {
        Fall,
        Spawn
    }
}