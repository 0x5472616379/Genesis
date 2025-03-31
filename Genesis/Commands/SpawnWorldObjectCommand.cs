using ArcticRS.Constants;
using Genesis.Entities.Player;
using Genesis.Environment;

namespace Genesis.Commands;

public class SpawnWorldObjectCommand : RSCommand
{
    private int _id;
    protected override PlayerRights RequiredRights => PlayerRights.ADMIN;
    
    public SpawnWorldObjectCommand(Player player, string[] args) : base(player, args)
    {
    }

    public override bool Validate()
    {
        _id = int.Parse(Args[1]);
        return true;
    }

    public override void Invoke()
    {
        var relX = Player.Location.X - Player.Location.CachedBuildAreaStartX;
        var relY = Player.Location.Y - Player.Location.CachedBuildAreaStartY;
        
        var relZoneX = (byte)(relX & ~0x7);
        var relZoneY = (byte)(relY & ~0x7);
        
        var inZoneX = relX & 0x7;
        var inZoneY = relY & 0x7;


        var entity = new ModifiedEntity
        {
            Id = _id,
            Location = new Location(Player.Location.X, Player.Location.Y, Player.Location.Z),
            Face = 0,
            Type = 10
        };
        
        Player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);
        Player.Session.PacketBuilder.UpdateObject(inZoneX, inZoneY, entity);
    }
}