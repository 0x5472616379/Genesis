using Genesis.Entities;

namespace Genesis.Skills.Combat;

public class ProjectileCreator
{
    // Define your method here
    public static void CreateProjectile(Player player, Player target, int spotAnimId = 91, int delay = 50,
        int duration = 70, int peakPitch = 16, int arcSize = 64)
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

        int srcY = (1 + 64) / 4;
        int dstY = (1 + 64) / 4;

        player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);

        player.Session.PacketBuilder.SpawnProjectile(pos, (sbyte)deltaX, (sbyte)deltaZ,
            target.Session.Index, spotAnimId, srcY, dstY, delay,
            duration, peakPitch, arcSize
        );
    }
}