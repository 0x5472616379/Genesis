using Genesis.Entities;
using Genesis.Environment;
using Genesis.Movement;

namespace Genesis.Interactions;

public class PlayerAttackInteraction : RSInteraction
{
    private readonly Player _player;

    public PlayerAttackInteraction(Player player)
    {
        _player = player;
    }

    private int currentTick = 0;
    private bool attackLoaded;

    public override bool Execute()
    {
        if (!CanExecute()) return false;

        if (attackLoaded)
        {
            _player.SetCurrentAnimation(1658);
            attackLoaded = false;
        }

        if (currentTick == 0 || currentTick % 4 == 0)
        {
            attackLoaded = true;
        }


        currentTick++;
        return false;
    }

    public override bool CanExecute()
    {
        var isMoving = (_player.MovementHandler.IsWalking || _player.MovementHandler.IsRunning);
        if (isMoving) return false;

        Console.WriteLine("IsMoving: " + isMoving);

        return false;
        // var projectilePathClear = MeleePathing.IsLongMeleeDistanceClear(_player, _player.Location.X, _player.Location.Y,
        //     _player.Location.Z,
        //     _player.InteractingEntity.Location.X, _player.InteractingEntity.Location.Y, 1);
        //
        // var isDiagonal = MeleePathing.IsDiagonal(_player.Location.X, _player.Location.Y,
        //     _player.InteractingEntity.Location.X, _player.InteractingEntity.Location.Y);
        //
        // var distance = MovementHelper.EuclideanDistance(_player.Location.X, _player.Location.Y,
        //     _player.InteractingEntity.Location.X, _player.InteractingEntity.Location.Y);
        //
        // if (distance == 0)
        // {
        //     _player.MovementHandler.Reset();
        //     RSPathfinder.MeleeWalk(_player, new Location(_player.InteractingEntity.Location.X, _player.InteractingEntity.Location.Y, 0));
        //     _player.MovementHandler.Finish();
        //     _player.MovementHandler.Process();
        //     return true;
        // }
        //
        // if(distance > 1 || !projectilePathClear)
        // {
        //     _player.MovementHandler.Reset();
        //
        //     RSPathfinder.FindPath(_player, _player.InteractingEntity.Location.X, _player.InteractingEntity.Location.Y, true, 0,0);
        //
        //     _player.MovementHandler.Finish();
        //     _player.MovementHandler.Process();
        //     
        //     return true;
        // }
        //
        //
        // _player.Session.PacketBuilder.SendMessage("Long Path Clear: " + projectilePathClear);
        // _player.Session.PacketBuilder.SendMessage("Diagonal: " + isDiagonal);
        // _player.Session.PacketBuilder.SendMessage("PIS: " + (projectilePathClear && !isDiagonal));
        //
        // return projectilePathClear && !isDiagonal;
    }

    private int meleeDistance()
    {
        int meleeDistance = 1;
        if (_player.MovementHandler.IsWalking || _player.MovementHandler.IsRunning)
        {
            int extraSpace = _player.InteractingEntity.MovementHandler.IsRunning ? 2
                : _player.InteractingEntity.MovementHandler.IsWalking ? 1 : 0;
            meleeDistance = 1 + extraSpace;
        }

        return meleeDistance;
    }

    public static bool IsValidMeleeClipping(Entity attacker, Entity target)
    {
        Location[] attackerTiles = TileControl.GetTiles(attacker);
        Location[] targetTiles = TileControl.GetTiles(target);

        int extraSpace = 0;
        if (attacker.MovementHandler.IsWalking || attacker.MovementHandler.IsRunning)
        {
            extraSpace = target.MovementHandler.IsRunning ? 2 : target.MovementHandler.IsWalking ? 1 : 0;
        }

        foreach (Location attackerTile in attackerTiles)
        {
            foreach (Location targetTile in targetTiles)
            {
                string adjacentDirection = GetAdjacentDirection(attackerTile, targetTile, extraSpace);

                if (adjacentDirection != null)
                {
                    if (!IsDirectionBlockedForAttacker(attackerTile.X, attackerTile.Y, attackerTile.Z,
                            adjacentDirection))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static bool IsDirectionBlockedForAttacker(int x, int y, int z, string direction)
    {
        switch (direction)
        {
            case "North":
                return Region.BlockedNorth(x, y, z);
            case "East":
                return Region.BlockedEast(x, y, z);
            case "South":
                return Region.BlockedSouth(x, y, z);
            case "West":
                return Region.BlockedWest(x, y, z);
            default:
                return false;
        }
    }

    public static string GetAdjacentDirection(Location tileOne, Location tileTwo, int extraSpace)
    {
        if (tileTwo.Y <= tileOne.Y + 1 + extraSpace && tileTwo.Y >= tileOne.Y + 1 && tileTwo.X == tileOne.X)
            return "North";
        if (tileTwo.Y == tileOne.Y && tileTwo.X <= tileOne.X + 1 + extraSpace && tileTwo.X >= tileOne.X + 1)
            return "East";
        if (tileTwo.Y >= tileOne.Y - 1 - extraSpace && tileTwo.Y <= tileOne.Y - 1 && tileTwo.X == tileOne.X)
            return "South";
        if (tileTwo.Y == tileOne.Y && tileTwo.X >= tileOne.X - 1 - extraSpace && tileTwo.X <= tileOne.X - 1)
            return "West";

        return null;
    }
}

public class TileControl
{
    public static Location Generate(int x, int y, int z)
    {
        return new Location(x, y, z);
    }

    /// <summary>
    /// Gets the tiles that the entity occupies.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static Location[] GetTiles(Entity entity)
    {
        int size = 1;
        int tileCount = 0;

        Location[] tiles = new Location[size * size];

        if (tiles.Length == 1)
        {
            tiles[0] = Generate(entity.Location.X, entity.Location.Y, entity.Location.Z);
        }
        else
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    tiles[tileCount++] = Generate(entity.Location.X + x, entity.Location.Y + y, entity.Location.Z);
                }
            }
        }

        return tiles;
    }

    public static int CalculateDistance(Location location, Entity other)
    {
        int X = Math.Abs(location.X - other.Location.X);
        int Y = Math.Abs(location.Y - other.Location.Y);
        return X > Y ? X : Y;
    }

    public static Location CurrentLocation(Entity entity)
    {
        return entity.Location;
    }
}