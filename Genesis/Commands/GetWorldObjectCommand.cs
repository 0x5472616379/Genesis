using ArcticRS.Commands;
using ArcticRS.Constants;
using Genesis.Cache;
using Genesis.Entities.Player;
using Genesis.Environment;
using Genesis.Managers;

namespace Genesis.Commands;

public class GetWorldObjectCommand : RSCommand
{
    private int x = 0;
    private int y = 0;

    protected override PlayerRights RequiredRights { get; }
    public GetWorldObjectCommand(Player player, string[] args) : base(player, args)
    {
    }

    public override bool Validate()
    {
        x = int.Parse(Args[1]);
        y = int.Parse(Args[2]);

        return true;
    }

    public override void Invoke()
    {
        var worldEntity = Region.GetObjectAt(x, y, Player.Location.Z);
        if (worldEntity == null)
            return;
        
        Player.Session.PacketBuilder.SendMessage($"Object ID: {worldEntity.Id} at {x}, {y}");
        
    }
}