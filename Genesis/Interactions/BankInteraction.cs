using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Movement;

namespace Genesis.Interactions;

public class BankInteraction : RSInteraction
{
    public override int MaxDistance { get; } = 1;
    private readonly Player _player;
    private readonly WorldObject _worldObject;
    private readonly Random _random = new();
    public override InteractingEntity Target { get; set; } = new();

    public BankInteraction(Player player, WorldObject worldObject)
    {
        _player = player;
        _worldObject = worldObject;
        Target.X = worldObject.X;
        Target.Y = worldObject.Y;
        Target.Z = _player.Location.Z;
    }


    public override bool Execute()
    {
        //SetPlayerFacing();

        if (!CanExecute()) return false;

        // _player.BankManager.CompressBank();

        // _player.InventoryItemContainer.CopyToContainer(_player.BankInventoryItemContainer);
        // _player.BankItemContainer.Refresh(_player, GameInterfaces.DefaultBankContainer);
        // _player.InventoryItemContainer.Refresh(_player, GameInterfaces.DefaultInventoryContainer);
        // _player.BankInventoryItemContainer.Refresh(_player, GameInterfaces.BankInventoryContainer);

        _player.BankContainer.RefreshContainer(_player, GameInterfaces.DefaultBankContainer);
        _player.Inventory.RefreshContainer(_player, GameInterfaces.BankInventoryContainer);
        
        _player.Session.PacketBuilder.SendInterface(GameInterfaces.BankWindowInterface,
            GameInterfaces.BankInventorySidebarInterface);

        return true;
    }

    public override bool CanExecute()
    {
        var distance = DistanceToObject(_player.Location.X, _player.Location.Y, _worldObject.X, _worldObject.Y,
            _worldObject.GetSize()[0], _worldObject.GetSize()[1]);
        _player.Session.PacketBuilder.SendMessage($"Distance: {distance}");

        if (_player.CurrentInteraction != null &&
            (_player.MovedThisTick || _player.MovedLastTick) &&
            distance <= 1)
        {
            // _player.ArriveDelayTicks = 1;
        }

        if (_player.NormalDelayTicks > 0 || _player.ArriveDelayTicks > 0)
            return false;

        if (distance <= MaxDistance)
        {
            SetPlayerFacing();
            return true;
        }

        return false;
    }

    private void SetPlayerFacing()
    {
        _player.SetFaceX(_worldObject.X * 2 + _worldObject.GetSize()[0]);
        _player.SetFaceY(_worldObject.Y * 2 + _worldObject.GetSize()[1]);
    }

    public static double DistanceToObject(int px, int py, int ox, int oy, int width, int height)
    {
        int minX = ox;
        int maxX = ox + width - 1;
        int minY = oy;
        int maxY = oy + height - 1;

        int dx = Math.Max(0, Math.Max(minX - px, px - maxX));
        int dy = Math.Max(0, Math.Max(minY - py, py - maxY));

        return Math.Sqrt(dx * dx + dy * dy);
    }
}