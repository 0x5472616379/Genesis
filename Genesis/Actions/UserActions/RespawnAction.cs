using Genesis;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Managers;
using Genesis.Skills;

namespace ArcticRS.Actions;

public class RespawnAction : RSAction
{
    private readonly Player _player;
    private RespawnPhase _currentPhase;

    private bool _droppedItems = false;
    private int _fromX;
    private int _fromY;
    private int _fromZ;

    public RespawnAction(Player player)
    {
        _player = player;
        Priority = ActionPriority.Forceful;
        ScheduledTick = World.CurrentTick + 2;
    }

    public override bool Execute()
    {
        Console.WriteLine("Execute: RespawnAction");
        switch (_currentPhase)
        {
            case RespawnPhase.Fall: // Start respawn process
                StartRespawn();
                ScheduleNext(3);
                _currentPhase = RespawnPhase.Spawn;
                _fromX = _player.Location.X;
                _fromY = _player.Location.Y;
                _fromZ = _player.Location.Z;
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
        _player.Location.X = 3100; //3222
        _player.Location.Y = 3830; //3218
        _player.Location.Z = 0;
        _player.PerformedTeleport = true;
        _player.Location.Build();
        _player.Session.PacketBuilder.SendNewBuildAreaPacket();
        EnvironmentBuilder.UpdateBuildArea(_player);
        _player.SetCurrentAnimation(-1);
        _player.CurrentHealth = _player.SkillManager.Skills[(int)SkillType.HITPOINTS].Level;
        _player.NormalDelayTicks = 1;
        _player.SkillManager.RefreshSkill(SkillType.HITPOINTS);

        if (!_droppedItems)
        {
            DropItems();

            _droppedItems = true;
        }

        _player.Inventory.Clear();
        _player.Equipment.ClearAll();
        _player.Equipment.RefreshContainer(_player, GameInterfaces.EquipmentContainer);
        
        _player.DamageTable = new Dictionary<int, int>();
    }

    private void StartRespawn()
    {
        _player.SetCurrentAnimation(836);
        _player.PlayerMovementHandler.Reset();
        _player.CurrentInteraction = null;
        _player.SetFacingEntity(null);
    }

    private void ScheduleNext(int ticks)
    {
        ScheduledTick = World.CurrentTick + ticks;
    }

    private enum RespawnPhase
    {
        Fall,
        DropItems,
        Spawn,
    }

    private void DropItems()
    {
        int playerIdx = _player.DamageTable
            .OrderByDescending(kvp => kvp.Value) // Order by value in descending order
            .First().Key;

        var player = World.GetPlayers().FirstOrDefault(x => x.Session.Index == playerIdx);

        var inventoryItems = _player.Inventory.GetAllItems();
        var equipmentItems = _player.Equipment.GetAllItems();

        foreach (var item in inventoryItems)
        {
            WorldDropManager.AddDrop(new WorldDrop
            {
                Id = item.ItemId,
                Amount = item.Quantity,
                X = _fromX,
                Y = _fromY,
                Z = _fromZ,
                Delay = 30,
                VisibleTo = player
            });
        }

        foreach (var item in equipmentItems)
        {
            WorldDropManager.AddDrop(new WorldDrop
            {
                Id = item.ItemId,
                Amount = item.Quantity,
                X = _fromX,
                Y = _fromY,
                Z = _fromZ,
                Delay = 30,
                VisibleTo = player
            });
        }

        WorldDropManager.AddDrop(new WorldDrop
        {
            Id = 526,
            Amount = 1,
            X = _fromX,
            Y = _fromY,
            Z = _fromZ,
            Delay = 30,
            VisibleTo = player
        });
    }
}