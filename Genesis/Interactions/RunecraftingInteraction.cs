﻿using ArcticRS.Appearance;
using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Constants;
using Genesis.Entities.Player;
using Genesis.Environment;
using Genesis.Model;
using Genesis.Movement;
using Genesis.Skills;
using Genesis.Skills.Runecrafting;

namespace Genesis.Interactions;

public class RunecraftingInteraction : RSInteraction
{
    private readonly Player _player;
    private readonly WorldObject _runecraftingAltar;
    private int _tick;
    private readonly Skill _skill;
    public override int MaxDistance { get; }
    public override InteractingEntity Target { get; set; } = new();
    public RunecraftingInteraction(Player player, WorldObject runecraftingAltar)
    {
        _player = player;
        _runecraftingAltar = runecraftingAltar;
        _skill = _player.SkillManager.Skills[(int)SkillType.RUNECRAFTING];
    }

    public override bool Execute()
    {
        SetPlayerFacing();

        if (!CanExecute()) return false;

        _tick++;

        if (_tick == 1)
        {
            // var altar = RunecraftingAltarData.GetAltar(_runecraftingAltar.Id);
            // if (altar == null)
            // {
            //     _player.Session.PacketBuilder.SendMessage("Unknown altar.");
            //     return false;
            // }
            //
            // if (_skill.Level < altar.RequiredLevel)
            // {
            //     _player.Session.PacketBuilder.SendMessage("You need a Runecrafting level of " + altar.RequiredLevel +
            //                                               " to craft this rune.");
            //     return false;
            // }
            //
            // var hasTalisman = _player.InventoryItemContainer.ContainsItemId(altar.TalismanId);
            // var tiara = _player.EquipmentManager.GetItem(EquipmentSlot.Helmet)?.Id;
            // if (!hasTalisman && tiara != altar.TiaraId)
            // {
            //     _player.Session.PacketBuilder.SendMessage("You need to wear the required tiara or bring a talisman.");
            //     return false;
            // }
            //
            // var removedEssenceCount = _player.InventoryItemContainer.RemoveAllById(1436);
            // if (removedEssenceCount <= 0)
            // {
            //     _player.Session.PacketBuilder.SendMessage("You don't have enough rune essence.");
            //     return false;
            // }
            //
            // int multiplier = RunecraftingAltarData.GetMultiplierForLevel(altar.Multipliers, _skill.Level);
            //
            //  int totalRunes = removedEssenceCount * multiplier;
            //
            //  _player.InventoryItemContainer.AddItem(altar.RuneId, totalRunes);
            //  _player.InventoryItemContainer.Refresh(_player, GameInterfaces.DefaultInventoryContainer);
            //
            // PlayRunecraftingEffects();
            //
            // _skill.AddExperience((int)(removedEssenceCount * altar.XpPerRune) * ServerConfig.SKILL_BONUS_EXP, _player, SkillRepository.GetSkill(SkillType.RUNECRAFTING));
            // _player.SkillManager.RefreshSkill(SkillType.RUNECRAFTING);

            return true;
        }

        return false;
    }

    private void SetPlayerFacing()
    {
        _player.SetFaceX(_runecraftingAltar.X * 2 + _runecraftingAltar.GetSize()[0]);
        _player.SetFaceY(_runecraftingAltar.Y * 2 + _runecraftingAltar.GetSize()[1]);
    }

    private void PlayRunecraftingEffects()
    {
        _player.SetCurrentGfx(new Gfx(186,1,0));
        _player.SetCurrentAnimation(791);
        _player.Session.PacketBuilder.SendSound(481, 0, 10);
    }

    public override bool CanExecute()
    {
        var isMoving = (_player.PlayerMovementHandler.IsWalking || _player.PlayerMovementHandler.IsRunning);
        if (isMoving)
        {
            return false;
        }

        var treeRelX2 = _runecraftingAltar.X - _player.Location.CachedBuildAreaStartX;
        var treeRelY2 = _runecraftingAltar.Y - _player.Location.CachedBuildAreaStartY;

        var region = Region.GetRegion(_player.Location.X, _player.Location.Y);
        var clip = region.GetClip(_player.Location.X, _player.Location.Y, _player.Location.Z);

        var reachedFacingObject = Region.ReachedObject(_player.Location.PositionRelativeToOffsetChunkX,
            _player.Location.PositionRelativeToOffsetChunkY,
            treeRelX2,
            treeRelY2,
            _runecraftingAltar.GetSize()[0],
            _runecraftingAltar.GetSize()[1],
            0, clip);

        if (!reachedFacingObject)
        {
            _player.PlayerMovementHandler.Reset();

            RSPathfinder.WalkToObject(_player, new Location(_player.PlayerMovementHandler.TargetDestX,
                _player.PlayerMovementHandler.TargetDestY,
                _runecraftingAltar.Height));

            _player.PlayerMovementHandler.Finish();

            _player.PlayerMovementHandler.Process();
            return false;
        }

        return true;
    }
}