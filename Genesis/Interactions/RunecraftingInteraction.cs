using Genesis.Cache;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Movement;

namespace Genesis.Interactions;

public class RunecraftingInteraction : RSInteraction
{
    private readonly Player _player;
    private readonly WorldObject _runecraftingAltar;
    private int _tick;

    public RunecraftingInteraction(Player player, WorldObject runecraftingAltar)
    {
        _player = player;
        _runecraftingAltar = runecraftingAltar;
    }

    public override bool Execute()
    {
        _player.SetFaceX(_runecraftingAltar.X * 2 + _runecraftingAltar.GetSize()[0]);
        _player.SetFaceY(_runecraftingAltar.Y * 2 + _runecraftingAltar.GetSize()[1]);

        if (!CanExecute()) return false;
        _tick++;

        if (_tick == 1)
        {
            _player.SetCurrentGfx(186);
            _player.SetCurrentAnimation(791);
            _player.Session.PacketBuilder.SendSound(481, 0, 10);
            
            if (_runecraftingAltar.Id == 2486)
            {
                var removed = _player.InventoryManager.RemoveItemsWithId(1436);
                _player.InventoryManager.AddItem(561, removed);
                _player.InventoryManager.RefreshInventory();
            }

            return true;
        }

        return false;
    }

    public override bool CanExecute()
    {
        var isMoving = (_player.MovementHandler.IsWalking || _player.MovementHandler.IsRunning);
        if (isMoving)
        {
            return false;
        }

        var treeRelX2 = _runecraftingAltar.X - _player.Location.CachedBuildAreaStartX;
        var treeRelY2 = _runecraftingAltar.Y - _player.Location.CachedBuildAreaStartY;

        var region = Region.GetRegion(_player.Location.X, _player.Location.Y);
        var clip = region.GetClip(_player.Location.X, _player.Location.Y, _player.Location.Z);

        _player.Session.PacketBuilder.SendMessage($"Clip: {clip}");

        var reachedFacingObject = Region.ReachedObject(
            _player.Location.PositionRelativeToOffsetChunkX,
            _player.Location.PositionRelativeToOffsetChunkY,
            treeRelX2,
            treeRelY2,
            _runecraftingAltar.GetSize()[0],
            _runecraftingAltar.GetSize()[1],
            0, clip);

        if (!reachedFacingObject)
        {
            _player.MovementHandler.Reset();

            RSPathfinder.WalkToObject(_player, new Location(_player.MovementHandler.TargetDestX,
                _player.MovementHandler.TargetDestY,
                _runecraftingAltar.Height));

            _player.MovementHandler.Finish();

            _player.MovementHandler.Process();
            return false;
        }

        return true;
    }
}