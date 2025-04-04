﻿using ArcticRS.Constants;
using Genesis.Entities.Player;
using Genesis.Managers;
using Genesis.Model;

namespace Genesis.Commands;

public class SpawnGroundItemCommand : RSCommand
{
    protected override PlayerRights RequiredRights => PlayerRights.ADMIN;

    public SpawnGroundItemCommand(Player player, string[] args) : base(player, args)
    {
    }

    public override bool Validate()
    {
        return true;
    }

    public override void Invoke()
    {
         // var item = new Item(995, 10000);
         //
         // var relX = Player.Location.X - Player.Location.CachedBuildAreaStartX;
         // var relY = Player.Location.Y - Player.Location.CachedBuildAreaStartY;
         //
         // var relZoneX = (byte)(relX & ~0x7);
         // var relZoneY = (byte)(relY & ~0x7);
         //
         // var inZoneX = relX & 0x7;
         // var inZoneY = relY & 0x7;
         //
         // /* Chunk to update in */
         // Player.Session.PacketBuilder.SendActiveChunk(relZoneX, relZoneY);
         // Player.Session.PacketBuilder.SendGroundItem(item, inZoneX, inZoneY);
         
         WorldDropManager.AddDrop(new WorldDrop
         {
             Id = 995,
             Amount = 10000,
             Delay = 50,
             X = Player.Location.X,
             Y = Player.Location.Y,
             Z = Player.Location.Z
         });
    }
}