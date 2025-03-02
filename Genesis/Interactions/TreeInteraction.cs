using Genesis.Entities;
using Genesis.Environment;
using Genesis.Managers;
using Genesis.Movement;
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

    private int _tick;

    public TreeInteraction(Player player, WorldObject treeWorldObject, Location treeLocation, Tree tree)
    {
        _player = player;
        _treeWorldObject = treeWorldObject;
        _treeLocation = treeLocation;
        _tree = tree;
    }

    private bool _logRecentlyGathered = false; // Flag for tracking recent log gathering

    public override bool Execute()
    {
        /* 1281 Oak | 1276 Tree */

        if (!CanExecute()) return false;

        _player.SetFaceX(_treeLocation.X * 2 + _treeWorldObject.GetSize()[0]);
        _player.SetFaceY(_treeLocation.Y * 2 + _treeWorldObject.GetSize()[1]);

        if (_player.InventoryManager.GetItemCount() >= 28)
        {
            _player.Session.PacketBuilder.SendMessage("You don't have enough inventory space.");
            _player.ClearInteraction();
            _player.SetCurrentAnimation(-1);
            return false;
        }

        _tick++;

        if (_tick > 1)
            _player.Session.PacketBuilder.SendSound(471, 0, 10);

        _player.SetCurrentAnimation(875);


        if (!_logRecentlyGathered)
        {
            var random = new Random();
            if (random.Next(1, 6) == 1)
            {
                _player.InventoryManager.AddItem(_tree.LogId);
                _player.SkillManager.Skills[(int)SkillType.WOODCUTTING]
                    .AddExperience((int)_tree.Xp * ServerConfig.SKILL_BONUS_EXP);
                _player.SkillManager.RefreshSkill(SkillType.WOODCUTTING);

                _logRecentlyGathered = true;

                if (random.Next(1, 3) == 1)
                {
                    var stumpId = _tree.StumpId;
                    _player.SetCurrentAnimation(-1);
                    EnvironmentBuilder.Add(new ModifiedEntity
                    {
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
        else
        {
            _logRecentlyGathered = false;
        }

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

        return reachedFacingObject;
    }
}