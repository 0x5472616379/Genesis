using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Managers;
using Genesis.Movement;

namespace Genesis.Interactions;

public class PickupInteraction : RSInteraction
{
    private readonly int _ex;
    private readonly int _ey;
    private readonly int _ez;
    private readonly int _itemId;
    private readonly Player _player;
    public override int MaxDistance { get; } = 1;
    public override InteractingEntity Target { get; set; } = new();

    public PickupInteraction(int ex, int ey, int ez, int itemId, Player player)
    {
        _ex = ex;
        _ey = ey;
        _ez = ez;
        _itemId = itemId;
        _player = player;
    }

    public override bool Execute()
    {
        _player.Session.PacketBuilder.SendMessage("Trying to execute..");
        if (CanExecute())
        {
            _player.Session.PacketBuilder.SendMessage("You interact with the item.");
            var worldItem = WorldDropManager.ItemExists(_ex, _ey, _ez, _itemId);
            if (worldItem != null)
            {
                var added = _player.Inventory.TryPickupItem(worldItem.Id, worldItem.Amount);
                if (added.Success)
                {
                    WorldDropManager.RemoveDropAt(_ex, _ey, _ez, _itemId);
                    var invItem = _player.Inventory.GetItemAtIndex(added.Index);
                    _player.Inventory.RefreshSlot(_player, added.Index, worldItem.Id, invItem.Quantity,
                        GameInterfaces.DefaultInventoryContainer);
                }
            }

            return true;
        }

        return false;
    }

    public override bool CanExecute()
    {
        var px = _player.Location.X;
        var py = _player.Location.Y;
        var pz = _player.Location.Z;

        var distance = MovementHelper.EuclideanDistance(px, py, _ex, _ey);
        return distance == 0;
    }
}