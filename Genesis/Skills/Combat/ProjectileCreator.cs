using Genesis.Entities.Player;

namespace Genesis.Skills.Combat;

public class ProjectileCreator
{
    // Define your method here
    public static void CreateProjectile(Player player, Player target, int spotAnimId = 91, int delay = 40,
        int duration = 60, int peakPitch = 16, int arcSize = 64, int sY = 75, int dY = 1)
    {
        int pX = player.Location.X;
        int pY = player.Location.Y;
        int targetX = target.Location.X;
        int targetY = target.Location.Y;

        var relX = player.Location.X - player.Location.CachedBuildAreaStartX;
        var relY = player.Location.Y - player.Location.CachedBuildAreaStartY;

        var relZoneX = (byte)(relX & ~0x7);
        var relZoneY = (byte)(relY & ~0x7);

        var inZoneX = relX & 0x7;
        var inZoneY = relY & 0x7;

        int pos = (inZoneX << 4) | inZoneY;

        int deltaX = targetX - pX;
        int deltaZ = targetY - pY;

        int srcY = (sY + 64) / 4;
        int dstY = (dY + 64) / 4;

        player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);

        player.Session.PacketBuilder.SpawnProjectile(pos, (sbyte)deltaX, (sbyte)deltaZ,
            target.Session.Index, spotAnimId, srcY, dstY, delay,
            duration, peakPitch, arcSize
        );
    }
}