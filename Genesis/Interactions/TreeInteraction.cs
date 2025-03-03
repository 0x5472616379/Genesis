using Genesis.Entities;
using Genesis.Environment;
using Genesis.Skills;
using Genesis.Skills.Woodcutting;
using WorldObject = Genesis.Cache.WorldObject;

namespace Genesis.Interactions;

public class TreeInteraction : RSInteraction
{
    private readonly Player _player;
    private readonly WorldObject _treeWorldObject;
    private readonly Location _treeLocation;
    private readonly Tree _tree;
    private readonly int _clipping;

    private int axeAnimationId;

    private int _tick;

    public TreeInteraction(Player player, WorldObject treeWorldObject, Location treeLocation, Tree tree)
    {
        _player = player;
        _treeWorldObject = treeWorldObject;
        _treeLocation = treeLocation;
        _tree = tree;
    }

    private int _lastLogGatheredTick = -1;

    public override bool Execute()
    {
        if (!CanExecute()) return false;

        _player.SetFaceX(_treeLocation.X * 2 + _treeWorldObject.GetSize()[0]);
        _player.SetFaceY(_treeLocation.Y * 2 + _treeWorldObject.GetSize()[1]);


        if (_tick > 0)
            _player.SetCurrentAnimation(axeAnimationId);

        if (_tick > 1)
            _player.Session.PacketBuilder.SendSound(471, 0, 10);

        if (_tick > 2)
        {
            if (_lastLogGatheredTick == -1 || _tick - _lastLogGatheredTick > 2)
            {
                _player.SetCurrentAnimation(axeAnimationId);
                var random = new Random();
                if (random.Next(1, 3) == 1)
                {
                    _player.InventoryManager.AddItem(_tree.LogId);
                    _player.SkillManager.Skills[(int)SkillType.WOODCUTTING]
                        .AddExperience((int)_tree.Xp * ServerConfig.SKILL_BONUS_EXP);
                    _player.SkillManager.RefreshSkill(SkillType.WOODCUTTING);

                    _lastLogGatheredTick = _tick;

                    if (random.Next(1, 8) == 1)
                    {
                        var stumpId = _tree.StumpId;
                        _player.SetCurrentAnimation(-1);
                        EnvironmentBuilder.Add(new ModifiedEntity
                        {
                            OriginalId = _treeWorldObject.Id,
                            Id = stumpId,
                            Type = _treeWorldObject.Type,
                            Face = _treeWorldObject.Direction,
                            Location = new Location(_treeLocation.X, _treeLocation.Y, _treeLocation.Z),
                            Delay = 20
                        });

                        return true;
                    }
                }
            }
        }


        _tick++;
        return false;
    }

    public override bool CanExecute()
    {
        var treeRelX2 = _treeLocation.X - _player.Location.CachedBuildAreaStartX;
        var treeRelY2 = _treeLocation.Y - _player.Location.CachedBuildAreaStartY;

        var region = Region.GetRegion(_player.Location.X, _player.Location.Y);
        var clip = region.GetClip(_player.Location.X, _player.Location.Y, _player.Location.Z);

        var reachedFacingObject = Region.ReachedObject(
            _player.Location.PositionRelativeToOffsetChunkX,
            _player.Location.PositionRelativeToOffsetChunkY,
            treeRelX2,
            treeRelY2,
            _treeWorldObject.GetSize()[0],
            _treeWorldObject.GetSize()[1],
            0,
            _player.Location.X,
            _player.Location.Y, clip);

        if (!reachedFacingObject)
            return false;

        var playerWoodcuttingLevel = _player.SkillManager.Skills[(int)SkillType.WOODCUTTING].Level;

        if (!HasRequiredWoodcuttingLevel(playerWoodcuttingLevel) || !HasEnoughInventorySpace())
            return false;

        if (!TryUseEquippedAxe(playerWoodcuttingLevel) && !TryUseInventoryAxe(playerWoodcuttingLevel))
        {
            _player.Session.PacketBuilder.SendMessage("You need a usable axe equipped or in your inventory to cut this tree.");
            _player.ClearInteraction();
            return false;
        }

        return true;
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
        return true;
    }
}