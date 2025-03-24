using ArcticRS.Appearance;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Skills;

namespace Genesis.Managers;

public class PlayerUpdateManager
{
    private static Player _player;
    private static readonly int _appearanceOffset = 0x100;
    private static readonly int _equipmentOffset = 0x200;
    private static RSStream PlayerUpdateBlock { get; set; }

    public static void Update()
    {
        for (int i = 0; i < World.GetPlayers().Length; i++)
        {
            var player = World.GetPlayers()[i];
            if (player == null) continue;

            PlayerUpdateBlock = new RSStream(new byte[5000]);

            _player = player;
            UpdateCurrentPlayerMovement();

            if (player.Flags != PlayerUpdateFlags.None)
                UpdatePlayerState(player, PlayerUpdateBlock);

            UpdateLocalPlayers();
            AddPlayersToLocalList();
            Finalize(PlayerUpdateBlock);
            _player.Session.Writer.EndFrameVarSizeWord();
        }
    }


    private static void Finalize(RSStream PlayerFlagUpdateBlock)
    {
        if (PlayerFlagUpdateBlock.CurrentOffset > 0)
        {
            _player.Session.Writer.FinishBitAccess();
            _player.Session.Writer.WriteBytes(PlayerFlagUpdateBlock.Buffer, PlayerFlagUpdateBlock.CurrentOffset,
                0);
        }
        else
        {
            _player.Session.Writer.FinishBitAccess();
        }
    }

    private static void AddPlayersToLocalList()
    {
        for (int i = 0; i < World.GetPlayers().Length; i++)
        {
            var player = World.GetPlayers()[i];
            if (player == null) continue;

            if (player.Session.Index == _player.Session.Index)
                continue;

            if (!_player.LocalPlayers.Contains(player) && player.Location.IsWithinArea(_player.Location))
            {
                /* Add the flag in order to render the local player */
                player.Flags |= PlayerUpdateFlags.Appearance;

                AddLocalPlayer(_player.Session.Writer, _player, player);
                UpdatePlayerState(player, PlayerUpdateBlock);
            }
        }

        /* Finished adding local players */
        _player.Session.Writer.WriteBits(11, 2047);
    }

    private static void AddLocalPlayer(RSStream writer, Player player, Player other)
    {
        writer.WriteBits(11, other.Session.Index);
        writer.WriteBits(1, 1); /* Observed */
        writer.WriteBits(1, 1); /* Teleported */

        var dx = other.Location.X - _player.Location.X;
        var dy = other.Location.Y - _player.Location.Y;

        writer.WriteBits(5, dy);
        writer.WriteBits(5, dx);

        // Console.WriteLine(
        //     $"Adding PlayerID: {other.Session.Index} To {player.Session.Index}'s LocalPlayerList at DeltaY: {other.Location.Y} - DeltaX: {other.Location.X}");
        player.AddLocalPlayer(other);
    }

    private static void UpdateLocalPlayers()
    {
        var count = _player.LocalPlayers.Count(x => x != null);
        _player.Session.Writer.WriteBits(8, count); // number of players to add

        for (int i = 0; i < _player.LocalPlayers.Length; i++)
        {
            var player = _player.LocalPlayers[i];
            if (player == null) continue;

            if (player.Location.IsWithinArea(_player.Location) && !player.PerformedTeleport)
            {
                UpdateLocalPlayerMovement(player, _player.Session.Writer);

                if (player.Flags != PlayerUpdateFlags.None) UpdatePlayerState(player, PlayerUpdateBlock);
            }
            else
            {
                RemovePlayer(player);
            }
        }
    }

    private static void UpdateLocalPlayerMovement(Player player, RSStream writer)
    {
        var pDir = player.PlayerMovementHandler.PrimaryDirection;
        var sDir = player.PlayerMovementHandler.SecondaryDirection;
        if (pDir != -1)
        {
            writer.WriteBits(1, 1);
            if (sDir != -1)
                WriteRun(writer, pDir, sDir, player.Flags != PlayerUpdateFlags.None);
            else
                WriteWalk(writer, pDir, player.Flags != PlayerUpdateFlags.None);
        }
        else
        {
            if (player.Flags != PlayerUpdateFlags.None)
            {
                writer.WriteBits(1, 1);
                writer.WriteBits(2, 0);
            }
            else
            {
                writer.WriteBits(1, 0);
            }
        }
    }

    private static void RemovePlayer(Player other)
    {
        _player.Session.Writer.WriteBits(1, 0);
        _player.RemoveLocalPlayer(other);
    }

    private static void UpdatePlayerState(Player player, RSStream playerFlagUpdateBlock)
    {
        var mask = player.Flags;

        if ((int)mask >= 0x100)
        {
            // Add only the 0x40 (bit 7 in first byte) to signal two-byte flag
            mask |= (PlayerUpdateFlags)(0x40); 

            playerFlagUpdateBlock.WriteByte((byte)((int)mask & 0xFF)); // Write lower byte
            playerFlagUpdateBlock.WriteByte((byte)((int)mask >> 8));  // Write upper byte
        }
        else
        {
            playerFlagUpdateBlock.WriteByte((byte)mask); // Single byte for lower masks
        }

        if ((mask & PlayerUpdateFlags.Graphics) != 0) AppendGraphics(player, playerFlagUpdateBlock);
        if ((mask & PlayerUpdateFlags.Animation) != 0) AppendAnimation(player, playerFlagUpdateBlock);
        if ((mask & PlayerUpdateFlags.InteractingEntity) != 0) AppendNPCInteract(player, playerFlagUpdateBlock);
        if ((mask & PlayerUpdateFlags.Appearance) != 0) AppendAppearance(player, playerFlagUpdateBlock);
        if ((mask & PlayerUpdateFlags.FaceDirection) != 0) AppendInteractingEntity(player, playerFlagUpdateBlock);
        if ((mask & PlayerUpdateFlags.SingleHit) != 0) AppendSingleHit(player, playerFlagUpdateBlock);
        if ((mask & PlayerUpdateFlags.DoubleHit) != 0) AppendDoubleHit(player, playerFlagUpdateBlock);
    }


    private static void AppendGraphics(Player player, RSStream playerFlagUpdateBlock)
    {
        playerFlagUpdateBlock.WriteWordBigEndian(player.CurrentGfx.Id);
        playerFlagUpdateBlock.WriteWord(player.CurrentGfx.Height); //0:on - 1:above
        playerFlagUpdateBlock.WriteWord(player.CurrentGfx.Delay); //delay
    }

    private static void AppendAnimation(Player player, RSStream playerFlagUpdateBlock)
    {
        playerFlagUpdateBlock.WriteWordBigEndian(player.CurrentAnimation);
        playerFlagUpdateBlock.WriteByteC(0); //delay is x * client tick (20ms)
    }

    private static void AppendSingleHit(Player player, RSStream playerFlagUpdateBlock)
    {
        playerFlagUpdateBlock.WriteByte((byte)player.RecentDamage.Amount); //hitDamage
        playerFlagUpdateBlock.WriteByteA((byte)player.RecentDamage.Type); //hitType
        playerFlagUpdateBlock.WriteByteC(player.CurrentHealth); //currentHealth
        playerFlagUpdateBlock.WriteByte(player.SkillManager.Skills[(int)SkillType.HITPOINTS].Level); //maxHealth
    }
    
    private static void AppendDoubleHit(Player player, RSStream playerFlagUpdateBlock)
    {
        playerFlagUpdateBlock.WriteByte((byte)player.RecentDamage1.Amount); //hitDamage
        playerFlagUpdateBlock.WriteByteS((byte)player.RecentDamage1.Type); //hitType
        playerFlagUpdateBlock.WriteByte(player.CurrentHealth); //currentHealth
        playerFlagUpdateBlock.WriteByteC(player.SkillManager.Skills[(int)SkillType.HITPOINTS].Level); //maxHealth
    }

    private static void AppendNPCInteract(Player player, RSStream updatetempBlock)
    {
        if (player.InteractingEntity is Player target)
        {
            updatetempBlock.WriteWordBigEndian(target.Session.Index + 32768);
        }
        // else if (player.InteractingEntity is NPC npc)
        // {
        //     updatetempBlock.WriteWordBigEndian(npc.Index);
        // }
        else
        {
            updatetempBlock.WriteWordBigEndian(0x00FFFF);
        }
    }

    private static void AppendAppearance(Player player, RSStream playerFlagUpdateBlock)
    {
        var updateBlockBuffer = new RSStream(new byte[256]);
        updateBlockBuffer.WriteByte((byte)player.Attributes.Gender);
        updateBlockBuffer.WriteByte((byte)player.Attributes.HeadIcon); // Skull Icon

        WriteHelmet(updateBlockBuffer, player);
        WriteCape(updateBlockBuffer, player);
        WriteAmulet(updateBlockBuffer, player);
        WriteWeapon(updateBlockBuffer, player);
        WriteBody(updateBlockBuffer, player);
        WriteShield(updateBlockBuffer, player);
        WriteArms(updateBlockBuffer, player);
        WriteLegs(updateBlockBuffer, player);

        WriteHair(updateBlockBuffer, player);
        WriteHands(updateBlockBuffer, player);
        WriteFeet(updateBlockBuffer, player);
        WriteBeard(updateBlockBuffer, player);

        WritePlayerColors(updateBlockBuffer, player);
        WriteMovementAnimations(updateBlockBuffer, player);

        updateBlockBuffer.WriteQWord(player.Session.Username.ToLong());
        updateBlockBuffer.WriteByte(player.SkillManager.CombatLevel);
        updateBlockBuffer.WriteWord(0);

        playerFlagUpdateBlock.WriteByteC(updateBlockBuffer.CurrentOffset);
        playerFlagUpdateBlock.WriteBytes(updateBlockBuffer.Buffer, updateBlockBuffer.CurrentOffset, 0);
    }

    private static void AppendInteractingEntity(Player player, RSStream playerFlagUpdateBlock)
    {
        playerFlagUpdateBlock.WriteWordBigEndianA(player.CurrentFaceX);
        playerFlagUpdateBlock.WriteWordBigEndian(player.CurrentFaceY);
    }

    private static void UpdateCurrentPlayerMovement()
    {
        _player.Session.Writer.CreateFrameVarSizeWord(ServerOpCodes.PLAYER_UPDATE);
        _player.Session.Writer.InitBitAccess();
        if (_player.PerformedTeleport)
            WritePositionUpdate(_player);
        else
            WriteMove(_player);
    }

    private static void WritePositionUpdate(Player player)
    {
        player.Session.Writer.WriteBits(1, 1); // set to true if updating this Player
        player.Session.Writer.WriteBits(2, 3); // updateType - 3=jump to pos (teleport / spawn)

        // the following applies to type 3 only
        player.Session.Writer.WriteBits(2, player.Location.Z); // height level (0-3)
        player.Session.Writer.WriteBits(1, 1); // set to true, if discarding walking queue (after teleport e.g.)
        player.Session.Writer.WriteBits(1,
            player.Flags != PlayerUpdateFlags.None ? 1 : 0); // UpdateRequired aka does come with UpdateFlags
        player.Session.Writer.WriteBits(7, player.Location.PositionRelativeToOffsetChunkY); // y-position
        player.Session.Writer.WriteBits(7, player.Location.PositionRelativeToOffsetChunkX); // x-position
    }

    private static void WriteMove(Player player)
    {
        var pDir = player.PlayerMovementHandler.PrimaryDirection;
        var sDir = player.PlayerMovementHandler.SecondaryDirection;

        if (pDir != -1)
        {
            player.Session.Writer.WriteBits(1, 1);
            if (sDir != -1)
                WriteRun(player.Session.Writer, pDir, sDir, player.Flags != PlayerUpdateFlags.None);
            else
                WriteWalk(player.Session.Writer, pDir, player.Flags != PlayerUpdateFlags.None);
        }
        else
        {
            if (player.Flags != PlayerUpdateFlags.None)
            {
                player.Session.Writer.WriteBits(1, 1);
                WriteUpdateStand(player);
            }
            else
            {
                WriteIdleStand(player);
            }
        }
    }

    private static void WriteRun(RSStream writer, int pDir, int sDir, bool updateRequired)
    {
        writer.WriteBits(2, 2); // 2 - running.

        // Append the actual sector.
        writer.WriteBits(3, pDir);
        writer.WriteBits(3, sDir);
        writer.WriteBits(1, updateRequired ? 1 : 0);
    }

    private static void WriteWalk(RSStream writer, int pDir, bool updateRequired)
    {
        writer.WriteBits(2, 1); // 1 - walking.

        // Append the actual sector.
        writer.WriteBits(3, pDir);
        writer.WriteBits(1, updateRequired ? 1 : 0);
    }

    private static void WriteUpdateStand(Player player)
    {
        player.Session.Writer.WriteBits(2, 0);
    }

    private static void WriteIdleStand(Player player)
    {
        player.Session.Writer.WriteBits(1, 0);
    }

    private static void WriteBeard(RSStream stream, Player player)
    {
        var beard = player.Attributes.Beard;

        if (beard != 0) //|| GameConstants.IsFullHelm(player.Equipment.GetItem(EquipmentSlot.Helmet).Id) || GameConstants.IsFullMask(player.Equipment.GetItem(EquipmentSlot.Helmet).Id)
            stream.WriteWord(_appearanceOffset + (int)beard);
        else
            stream.WriteByte(0);
    }

    private static void WriteFeet(RSStream stream, Player player)
    {
        var Id = player.Equipment.GetItemInSlot(EquipmentSlot.Boots).ItemId;
        var feetId = player.Attributes.Feet;
        if (Id > -1)
            stream.WriteWord(_equipmentOffset + Id);
        else
            stream.WriteWord(_appearanceOffset + (int)feetId);
    }

    private static void WriteHands(RSStream stream, Player player)
    {
        var Id = player.Equipment.GetItemInSlot(EquipmentSlot.Gloves).ItemId;
        var handsId = player.Attributes.Hands;
        if (Id > -1)
            stream.WriteWord(_equipmentOffset + Id);
        else
            stream.WriteWord(_appearanceOffset + (int)handsId);
    }

    private static void WriteHair(RSStream stream, Player player)
    {
        var isFullHelmOrMask = GameConstants.IsFullHelm(player.Equipment.GetItemInSlot(EquipmentSlot.Helmet).ItemId) ||
                               GameConstants.IsFullMask(player.Equipment.GetItemInSlot(EquipmentSlot.Helmet).ItemId);
        if (!isFullHelmOrMask)
        {
            var hair = player.Attributes.Hair;
            stream.WriteWord(_appearanceOffset + (int)hair);
        }
        else
        {
            stream.WriteByte(0);
        }
    }

    private static void WriteLegs(RSStream stream, Player player)
    {
        var Id = player.Equipment.GetItemInSlot(EquipmentSlot.Legs).ItemId;
        var legsId = player.Attributes.Legs;
        if (Id > -1)
            stream.WriteWord(_equipmentOffset + Id);
        else
            stream.WriteWord(_appearanceOffset + (int)legsId);
    }

    private static void WriteShield(RSStream stream, Player player)
    {
        var Id = player.Equipment.GetItemInSlot(EquipmentSlot.Shield).ItemId;
        if (Id > -1)
            stream.WriteWord(_equipmentOffset + Id);
        else
            stream.WriteByte(0);
    }

    private static void WriteBody(RSStream stream, Player player)
    {
        var Id = player.Equipment.GetItemInSlot(EquipmentSlot.Chest).ItemId;
        var torsoId = player.Attributes.Torso;
        if (Id > -1)
            stream.WriteWord(_equipmentOffset + Id);
        else
            stream.WriteWord(_appearanceOffset + (int)torsoId);
    }

    private static void WriteWeapon(RSStream stream, Player player)
    {
        if (player.Equipment.GetItemInSlot(EquipmentSlot.Weapon) == null)
        {
            stream.WriteByte(0);
            return;
        }

        var Id = player.Equipment.GetItemInSlot(EquipmentSlot.Weapon).ItemId;
        if (Id > -1)
            stream.WriteWord(_equipmentOffset + Id);
        else
            stream.WriteByte(0);
    }

    private static void WriteAmulet(RSStream stream, Player player)
    {
        var Id = player.Equipment.GetItemInSlot(EquipmentSlot.Amulet).ItemId;
        if (Id > -1)
            stream.WriteWord(_equipmentOffset + Id);
        else
            stream.WriteByte(0);
    }

    private static void WriteCape(RSStream stream, Player player)
    {
        var Id = player.Equipment.GetItemInSlot(EquipmentSlot.Cape).ItemId;
        if (Id > -1)
            stream.WriteWord(_equipmentOffset + Id);
        else
            stream.WriteByte(0);
    }

    private static void WriteHelmet(RSStream stream, Player player)
    {
        var Id = player.Equipment.GetItemInSlot(EquipmentSlot.Helmet).ItemId;
        if (Id > -1)
            stream.WriteWord(_equipmentOffset + Id);
        else
            stream.WriteByte(0);
    }

    private static void WriteArms(RSStream stream, Player player)
    {
        var isFullBody = GameConstants.IsFullBody(player.Equipment.GetItemInSlot(EquipmentSlot.Chest).ItemId);
        if (!isFullBody)
        {
            var arms = player.Attributes.Arms;
            stream.WriteWord(_appearanceOffset + (int)arms);
        }
        else
        {
            stream.WriteByte(0);
        }
    }

    private static void WritePlayerColors(RSStream stream, Player player)
    {
        for (var i = 0; i < 5; i++) stream.WriteByte(player.ColorManager.GetColors()[i]);
    }

    private static void WriteMovementAnimations(RSStream stream, Player player)
    {
        foreach (var animation in player.AnimationManager.GetAnimations())
            stream.WriteWord(animation);
    }
}