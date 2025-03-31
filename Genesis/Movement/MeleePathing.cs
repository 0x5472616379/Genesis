using Genesis.Entities;
using Genesis.Entities.Player;
using Genesis.Environment;

namespace Genesis.Movement;

public class MeleePathing
{
    private const int DEFALT_PATH_LENGTH = 4000;

    public static bool IsDiagonal(int x0, int y0, int x1, int y1)
    {
        int deltaX = Math.Abs(x1 - x0);
        int deltaY = Math.Abs(y1 - y0);

        return deltaX > 0 && deltaY > 0;
    }
    
    public static bool IsLongMeleeDistanceClear(Player player, int x0, int y0, int z, int x1, int y1, int maxDistance)
    {
        var deltaX = x1 - x0;
        var deltaY = y1 - y0;

        if (deltaX + deltaY > maxDistance) return false;
        
        double error = 0;
        var deltaError = Math.Abs(
            deltaY / (deltaX == 0 ? deltaY : (double)deltaX));

        var x = x0;
        var y = y0;

        var pX = x;
        var pY = y;

        var incrX = x0 < x1;
        var incrY = y0 < y1;

        if (!IsMeleeAccessable(player, x1, y1))
        {
            return false;
        }

        while (true)
        {
            if (x != x1) x += incrX ? 1 : -1;

            if (y != y1)
            {
                error += deltaError;

                if (error >= 0.5)
                {
                    y += incrY ? 1 : -1;
                    error -= 1;
                }
            }

            if (!Meleeable(x, y, z, pX, pY)) return false;

            if (incrX && incrY
                      && x >= x1 && y >= y1)
                break;
            if (!incrX && !incrY
                       && x <= x1 && y <= y1)
                break;
            if (!incrX && incrY
                       && x <= x1 && y >= y1)
                break;
            if (incrX && !incrY
                      && x >= x1 && y <= y1)
                break;

            pX = x;
            pY = y;
        }

        return true;
    }

    private static bool Meleeable(int x, int y, int z, int px, int py)
    {
        if (x == px && y == py) return true;

        var delta1 = Location.Delta(new Location(x, y, z), new Location(px, py, z));
        var delta2 = Location.Delta(new Location(px, py, z), new Location(x, y, z));

        var dir = MovementHelper.GetDirection(delta1.X, delta1.Y);
        var dir2 = MovementHelper.GetDirection(delta2.X, delta2.Y);

        if (dir == -1 || dir2 == -1) return false;

        return (Region.CanMove(x, y, z, (Direction)dir) && Region.CanMove(px, py, z, (Direction)dir2));
    }

    public static bool IsMeleeAccessable(Entity character, int destX, int destY)
    {
        if (destX == character.Location.PositionRelativeToOffsetChunkX &&
            destY == character.Location.PositionRelativeToOffsetChunkY)
        {
            if (character is Player player) player.Session.PacketBuilder.SendMessage("ERROR!");

            return false;
        }

        var height = character.Location.Z;
        destX = destX - 8 * character.Location.CachedBuildAreaSwChunkX;
        destY = destY - 8 * character.Location.CachedBuildAreaSwChunkY;

        var via = new int[104][];
        var cost = new int[104][];

        var tileQueueX = new List<int>();
        var tileQueueY = new List<int>();

        for (var i = 0; i < 104; i++)
        {
            via[i] = new int[104];
            cost[i] = new int[104];
        }

        for (var xx = 0; xx < 104; xx++)
        for (var yy = 0; yy < 104; yy++)
            cost[xx][yy] = 99999999;

        var curX = character.Location.PositionRelativeToOffsetChunkX;
        var curY = character.Location.PositionRelativeToOffsetChunkY;

        via[curX][curY] = 99;
        cost[curX][curY] = 0;

        var tail = 0;
        tileQueueX.Add(curX);
        tileQueueY.Add(curY);
        var foundPath = false;

        while (tail != tileQueueX.Count && tileQueueX.Count < DEFALT_PATH_LENGTH)
        {
            curX = tileQueueX.ElementAt(tail);
            curY = tileQueueY.ElementAt(tail);
            var curAbsX = character.Location.CachedBuildAreaSwChunkX * 8 + curX;
            var curAbsY = character.Location.CachedBuildAreaSwChunkY * 8 + curY;

            if (curX == destX && curY == destY)
            {
                foundPath = true;
                break;
            }

            /* Combat Exit Strategy */
            // if (character.MostRecentCombatTarget != null)
            // {
            //     if (xLength != 0 && yLength != 0 && Region.canInteract(destX, destY, curAbsX, curAbsY, curX, curY,xLength, yLength, 0))
            //     {
            //         foundPath = true;
            //         break;
            //     }
            // }

            tail = (tail + 1) % DEFALT_PATH_LENGTH;
            var thisCost = cost[curX][curY] + 1;
            if (curY > 0 && via[curX][curY - 1] == 0 &&
                (Region.GetClipping(curAbsX, curAbsY - 1, height) & 0x1280102) == 0)
            {
                tileQueueX.Add(curX);
                tileQueueY.Add(curY - 1);
                via[curX][curY - 1] = 1;
                cost[curX][curY - 1] = thisCost;
            }

            if (curX > 0
                && via[curX - 1][curY] == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY, height) & 0x1280108) == 0)
            {
                tileQueueX.Add(curX - 1);
                tileQueueY.Add(curY);
                via[curX - 1][curY] = 2;
                cost[curX - 1][curY] = thisCost;
            }

            if (curY < 104 - 1
                && via[curX][curY + 1] == 0
                && (Region.GetClipping(curAbsX, curAbsY + 1, height) & 0x1280120) == 0)
            {
                tileQueueX.Add(curX);
                tileQueueY.Add(curY + 1);
                via[curX][curY + 1] = 4;
                cost[curX][curY + 1] = thisCost;
            }

            if (curX < 104 - 1 && via[curX + 1][curY] == 0 &&
                (Region.GetClipping(curAbsX + 1, curAbsY, height) & 0x1280180) == 0)
            {
                tileQueueX.Add(curX + 1);
                tileQueueY.Add(curY);
                via[curX + 1][curY] = 8;
                cost[curX + 1][curY] = thisCost;
            }

            if (curX > 0
                && curY > 0
                && via[curX - 1][curY - 1] == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY - 1, height) & 0x128010e) == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY, height) & 0x1280108) == 0
                && (Region.GetClipping(curAbsX, curAbsY - 1, height) & 0x1280102) == 0)
            {
                tileQueueX.Add(curX - 1);
                tileQueueY.Add(curY - 1);
                via[curX - 1][curY - 1] = 3;
                cost[curX - 1][curY - 1] = thisCost;
            }

            if (curX > 0
                && curY < 104 - 1
                && via[curX - 1][curY + 1] == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY + 1, height) & 0x1280138) == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY, height) & 0x1280108) == 0
                && (Region.GetClipping(curAbsX, curAbsY + 1, height) & 0x1280120) == 0)
            {
                tileQueueX.Add(curX - 1);
                tileQueueY.Add(curY + 1);
                via[curX - 1][curY + 1] = 6;
                cost[curX - 1][curY + 1] = thisCost;
            }

            if (curX < 104 - 1
                && curY > 0
                && via[curX + 1][curY - 1] == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY - 1, height) & 0x1280183) == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY, height) & 0x1280180) == 0
                && (Region.GetClipping(curAbsX, curAbsY - 1, height) & 0x1280102) == 0)
            {
                tileQueueX.Add(curX + 1);
                tileQueueY.Add(curY - 1);
                via[curX + 1][curY - 1] = 9;
                cost[curX + 1][curY - 1] = thisCost;
            }

            if (curX < 104 - 1
                && curY < 104 - 1
                && via[curX + 1][curY + 1] == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY + 1, height) & 0x12801e0) == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY, height) & 0x1280180) == 0
                && (Region.GetClipping(curAbsX, curAbsY + 1, height) & 0x1280120) == 0)
            {
                tileQueueX.Add(curX + 1);
                tileQueueY.Add(curY + 1);
                via[curX + 1][curY + 1] = 12;
                cost[curX + 1][curY + 1] = thisCost;
            }
        }

        return foundPath;
    }
}