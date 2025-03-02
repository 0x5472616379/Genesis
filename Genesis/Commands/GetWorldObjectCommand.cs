using ArcticRS.Commands;
using Genesis.Cache;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Managers;

namespace Genesis.Commands;

public class GetWorldObjectCommand : CommandBase
{
    private int id = 0;
    private int x = 0;
    private int y = 0;

    public GetWorldObjectCommand(Player player, string[] args) : base(player, args)
    {
    }

    protected override string ValidateArgs()
    {
        x = int.Parse(Args[1]);
        y = int.Parse(Args[2]);
        // id = int.Parse(Args[3]);

        return null;
    }

    protected override void Invoke()
    {
        var worldEntity = Region.GetObjectAt(x, y, Player.Location.Z);
        if (worldEntity == null)
            return;
        
        Player.Session.PacketBuilder.SendMessage($"Object ID: {worldEntity.Id} at {x}, {y}");
        
    }
}