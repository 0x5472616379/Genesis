﻿using Genesis.Entities;
using Genesis.Environment;

namespace Genesis.Movement;

public class MovementHandler
{
    private readonly Entity _entity;
    private readonly LinkedList<Waypoint> waypoints = new();
    private bool newWalkCmdIsRunning = false;

    public MovementHandler(Entity entity)
    {
        _entity = entity;
    }

    public int PrimaryDirection { get; set; } = -1;
    public int SecondaryDirection { get; set; } = -1;
    public bool DiscardMovementQueue { get; set; }
    public bool RunToggled { get; set; }

    public int TargetDestX { get; set; }
    public int TargetDestY { get; set; }

    public bool IsWalking { get; set; }
    public bool IsRunning { get; set; }

    public void Process()
    {
        if (DiscardMovementQueue)
        {
            Reset();
        }

        if (_entity.CurrentHealth <= 0)
            return;

        if (waypoints.Count == 0)
            return;

        var walkPoint = GetWalkPoint();
        var runPoint = GetRunPoint();

        if (walkPoint != null && walkPoint.Direction != -1)
        {
            /* Check Valid Combat Distance */

            // if (EarlyMovementResetCheck(_entity))
            // {
            //     _entity.MovementHandler.Reset();
            //     return;
            // }


            MoveToDirection(walkPoint.Direction);
            PrimaryDirection = walkPoint.Direction;
            IsWalking = true;
        }

        if (runPoint != null && runPoint.Direction != -1)
        {
            /* Check Valid Combat Distance */

            // if (EarlyMovementResetCheck(_entity))
            // {
            //     _entity.MovementHandler.Reset();
            //     return;
            // }

            MoveToDirection(runPoint.Direction);
            SecondaryDirection = runPoint.Direction;
            IsRunning = true;
        }

        if (_entity is Player player)
        {
            if (_entity.Location.ShouldGenerateNewBuildArea)
            {
                _entity.Location.Build();
                EnvironmentBuilder.UpdateBuildArea(player);
                player.Session.PacketBuilder.SendNewBuildAreaPacket();
            }
        }
    }

    private Waypoint GetRunPoint()
    {
        if (RunToggled)
            if (waypoints.First != null)
            {
                var runPoint = waypoints.First.Value;
                waypoints.RemoveFirst();
                return runPoint;
            }

        return null;
    }

    private Waypoint GetWalkPoint()
    {
        if (waypoints.First.Value != null)
        {
            var walkPoint = waypoints.First.Value;
            waypoints.RemoveFirst();
            return walkPoint;
        }

        return null;
    }


    public bool GetWalking => PrimaryDirection != -1;
    public bool GetRunning => SecondaryDirection != -1;

    private void MoveToDirection(int direction)
    {
        _entity.Location.Move(MovementHelper.DIRECTION_DELTA_X[direction], MovementHelper.DIRECTION_DELTA_Y[direction]);
    }

    public void WalkTo(Location location)
    {
        if (waypoints.Count == 0)
            Reset();

        var last = waypoints.Last.Value;
        var deltaX = location.X - last.X;
        var deltaY = location.Y - last.Y;
        var distanceDelta = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));

        if (distanceDelta >= 40)
            return;

        for (var i = 0; i < distanceDelta; i++)
        {
            if (deltaX != 0) deltaX -= Math.Sign(deltaX);
            if (deltaY != 0) deltaY -= Math.Sign(deltaY);
            AddStep(location.X - deltaX, location.Y - deltaY);
        }
    }

    public void AddToPath(Location location)
    {
        if (waypoints.Count == 0)
        {
            Reset();
        }

        var last = waypoints.Last.Value;
        var deltaX = location.X - last.X;
        var deltaY = location.Y - last.Y;
        var max = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));

        if (max > 40)
        {
            Console.WriteLine("ehm, stop that :^)");
            return;
        }

        for (var i = 0; i < max; i++)
        {
            if (deltaX < 0)
            {
                deltaX++;
            }
            else if (deltaX > 0)
            {
                deltaX--;
            }

            if (deltaY < 0)
            {
                deltaY++;
            }
            else if (deltaY > 0)
            {
                deltaY--;
            }

            AddStep(location.X - deltaX, location.Y - deltaY);
        }
    }

    private void AddStep(int x, int y)
    {
        if (waypoints.Count >= 100)
        {
            return;
        }

        var last = waypoints.Last.Value;
        var deltaX = x - last.X;
        var deltaY = y - last.Y;
        var direction = MovementHelper.GetDirection(deltaX, deltaY);

        if (direction > -1)
        {
            waypoints.AddLast(new Waypoint(x, y, direction));
        }
    }

    private bool EarlyMovementResetCheck(Entity entity)
    {
        // if (entity.MostRecentCombatTarget != null)
        // {
        //     return entity.CombatStrategy.IsValidDistance();
        // }

        return false;
    }

    public void Reset()
    {
        waypoints.Clear();
        var location = _entity.Location;
        waypoints.AddLast(new Waypoint(location.X, location.Y, -1));
    }

    public void Finish()
    {
        waypoints.RemoveFirst();
    }
}