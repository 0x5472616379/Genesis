using Genesis.Entities;

namespace ArcticRS.Actions;

public class EatAction : RSAction
{
    private readonly Player _player;

    public override ActionCategory Category { get; set; } = ActionCategory.EAT;
    public override Func<bool> CanExecute { get; set; }
    public override Func<bool> Execute { get; set; }

    public EatAction(Player player)
    {
        _player = player;
        CanExecute = CanEat;
        Execute = Invoke;
    }

    private bool Invoke()
    {
        _player.SetCurrentAnimation(829);
        _player.Session.PacketBuilder.SendMessage("You eat a salmon.");
        // _player.PacketBuilder.SendMessage($"Eat On Tick: {World.CurrentTick}");
        return true;
    }

    private bool CanEat()
    {
        return true;
    }
}