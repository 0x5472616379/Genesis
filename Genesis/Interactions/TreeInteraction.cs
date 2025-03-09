using Genesis.Constants;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Movement;
using Genesis.Skills;
using Genesis.Skills.Woodcutting;
using WorldObject = Genesis.Cache.WorldObject;

namespace Genesis.Interactions;

public class TreeInteraction : RSInteraction
{
    private readonly Player _player;
    private readonly WorldObject _treeWorldObject;
    private readonly Tree _tree;
    private readonly int _clipping;

    private int axeAnimationId;

    private AxeData.Axe EquippedAxe = null;

    private int _tick;

    public TreeInteraction(Player player, WorldObject treeWorldObject, Tree tree)
    {
        _player = player;
        _treeWorldObject = treeWorldObject;
        _tree = tree;
    }

    private int _lastLogGatheredTick = -1;
    private int sleepTick = 0;
    private bool didWalk = false;

    public override bool Execute()
    {
        _player.SetFaceX(_treeWorldObject.X * 2 + _treeWorldObject.GetSize()[0]);
        _player.SetFaceY(_treeWorldObject.Y * 2 + _treeWorldObject.GetSize()[1]);

        if (!CanExecute()) return false;
        _tick++;

        if (_tick > 1)
            _player.SetCurrentAnimation(axeAnimationId);

        if (_tick > 2)
            _player.Session.PacketBuilder.SendSound(471, 0, 10);

        if (_tick > 3)
        {
            if (_lastLogGatheredTick == -1 || _tick - _lastLogGatheredTick > 2)
            {
                _player.SetCurrentAnimation(axeAnimationId);
                var random = new Random();
                if (random.Next(1, 100) < GetSuccessRate(_tree, EquippedAxe)) //GetSuccessRate(_tree, EquippedAxe)
                {
                    _player.InventoryManager.AddItem(_tree.LogId);
                    _player.InventoryManager.RefreshInventory();
                    _player.SkillManager.Skills[(int)SkillType.WOODCUTTING]
                        .AddExperience((int)_tree.Xp * ServerConfig.SKILL_BONUS_EXP, _player, SkillRepository.GetSkill(SkillType.WOODCUTTING));
                    _player.SkillManager.RefreshSkill(SkillType.WOODCUTTING);

                    _lastLogGatheredTick = _tick;

                    if (random.Next(0, 100) < _tree.DecayChance)
                    {
                        var stumpId = _tree.StumpId;
                        _player.SetCurrentAnimation(-1);
                        EnvironmentBuilder.Add(new ModifiedEntity
                        {
                            OriginalId = _treeWorldObject.Id,
                            Id = stumpId,
                            Type = _treeWorldObject.Type,
                            Face = _treeWorldObject.Direction,
                            Location = new Location(_treeWorldObject.X, _treeWorldObject.Y, _treeWorldObject.Height),
                            Delay = _tree.RespawnTime
                        });

                        return true;
                    }
                }
            }
        }


        return false;
    }

    public override bool CanExecute()
    {
        var isMoving = (_player.MovementHandler.IsWalking || _player.MovementHandler.IsRunning);
        if (isMoving)
        {
            didWalk = false;
            return false;
        }

        var treeRelX2 = _treeWorldObject.X - _player.Location.CachedBuildAreaStartX;
        var treeRelY2 = _treeWorldObject.Y - _player.Location.CachedBuildAreaStartY;

        var region = Region.GetRegion(_player.Location.X, _player.Location.Y);
        var clip = region.GetClip(_player.Location.X, _player.Location.Y, _player.Location.Z);

        var reachedFacingObject = Region.ReachedObject(
            _player.Location.PositionRelativeToOffsetChunkX,
            _player.Location.PositionRelativeToOffsetChunkY,
            treeRelX2,
            treeRelY2,
            _treeWorldObject.GetSize()[0],
            _treeWorldObject.GetSize()[1],
            0, clip);

        if (!reachedFacingObject)
        {
            _player.MovementHandler.Reset();

            RSPathfinder.WalkToObject(_player, new Location(_player.MovementHandler.TargetDestX,
                _player.MovementHandler.TargetDestY,
                _treeWorldObject.Height));

            _player.MovementHandler.Finish();

            _player.MovementHandler.Process();
            didWalk = true;
            return false;
        }

        var playerWoodcuttingLevel = _player.SkillManager.Skills[(int)SkillType.WOODCUTTING].Level;

        if (!HasRequiredWoodcuttingLevel(playerWoodcuttingLevel) || !HasEnoughInventorySpace())
            return false;

        if (!TryUseEquippedAxe(playerWoodcuttingLevel) && !TryUseInventoryAxe(playerWoodcuttingLevel))
        {
            _player.Session.PacketBuilder.SendMessage(
                "You need a usable axe equipped or in your inventory to cut this tree.");
            _player.ClearInteraction();
            return false;
        }

        // if (_player.MovementHandler.IsRunning)
        // {
        //     if (sleepTick < 1)
        //     {
        //         sleepTick++;
        //         return false;
        //     }
        // }
        //
        // if (_player.MovementHandler.IsWalking)
        // {
        //     if (sleepTick < 2)
        //     {
        //         sleepTick++;
        //         return false;
        //     }
        // }
        //
        // sleepTick = 0;
        return true;
    }

    public static double GetSuccessRate(Tree tree, AxeData.Axe axe)
    {
        return tree.SuccessYield * axe.Multiplier;
    }


    private bool HasRequiredWoodcuttingLevel(int playerLevel)
    {
        if (playerLevel < _tree.Level)
        {
            _player.Session.PacketBuilder.SendMessage(
                $"You need a Woodcutting level of {_tree.Level} to cut this tree.");
            _player.ClearInteraction();
            return false;
        }

        return true;
    }

    private bool HasEnoughInventorySpace()
    {
        if (_player.InventoryManager.GetItemCount() >= 28)
        {
            _player.Session.PacketBuilder.SendMessage("You don't have enough inventory space.");
            _player.ClearInteraction();
            return false;
        }

        return true;
    }

    private bool TryUseEquippedAxe(int playerLevel)
    {
        var weaponId = _player.EquipmentManager.GetWeapon();
        var equippedAxe = AxeData.GetAxe(weaponId);

        if (equippedAxe != null && playerLevel >= equippedAxe.RequiredLevel)
        {
            axeAnimationId = equippedAxe.AnimationId;
            EquippedAxe = equippedAxe;
            return true;
        }

        return false;
    }

    private bool TryUseInventoryAxe(int playerLevel)
    {
        var inventory = _player.InventoryManager.GetItems();
        var usableAxe = inventory
            .Select(item => AxeData.GetAxe(item.Id))
            .Where(axe => axe != null && playerLevel >= axe.RequiredLevel)
            .OrderByDescending(axe => axe!.RequiredLevel)
            .FirstOrDefault();

        if (usableAxe == null)
        {
            _player.ClearInteraction();
            return false;
        }

        axeAnimationId = usableAxe.AnimationId;
        EquippedAxe = usableAxe;
        return true;
    }
}