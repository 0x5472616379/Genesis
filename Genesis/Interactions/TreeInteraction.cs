using Genesis.Cache;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Managers;
using Genesis.Model;
using Genesis.Movement;
using Genesis.Skills;
using Genesis.Skills.Woodcutting;

namespace Genesis.Interactions;

public class TreeInteraction : RSInteraction
{
    private readonly Player _player;
    private readonly Objects _treeObject;
    private readonly Location _treeLocation;
    private readonly Tree _tree;
    private readonly int _clipping;

    private int _tick;

    public TreeInteraction(Action action, Player player, Objects treeObject, Location treeLocation, Tree tree) :
        base(action)
    {
        _player = player;
        _treeObject = treeObject;
        _treeLocation = treeLocation;
        _tree = tree;
    }

    public override bool Execute()
    {
        if (!CanExecute()) return false;
        
        _tick++;
        
        if (_tick > 1)
            _player.Session.PacketBuilder.SendSound(471, 0, 10);
        _player.SetCurrentAnimation(875);

        if (_tick < 4)
            return false;

        _player.SetCurrentAnimation(-1);
        _player.InventoryManager.AddItem(_tree.LogId);
        _player.SkillManager.Skills[(int)SkillType.WOODCUTTING].AddExperience((int)_tree.Xp * ServerConfig.SKILL_BONUS_EXP);
        _player.SkillManager.RefreshSkill(SkillType.WOODCUTTING);
        return true;
    }

    public override bool CanExecute()
    {
        var treeRelX2 = _treeLocation.X - _player.Location.CachedBuildAreaStartX;
        var treeRelY2 = _treeLocation.Y - _player.Location.CachedBuildAreaStartY;

        var region = Region.GetRegion(_player.Location.X, _player.Location.Y);
        var clip = region.GetClip(_player.Location.X, _player.Location.Y, _player.Location.Z);

        var reachedFacingObject = Region.reachedFacingObject(
            _player.Location.PositionRelativeToOffsetChunkX,
            _player.Location.PositionRelativeToOffsetChunkY,
            treeRelX2,
            treeRelY2,
            _treeObject.GetSize()[0], _treeObject.GetSize()[1], 0,
            _player.Location.X,
            _player.Location.Y, clip);

        // _player.Session.PacketBuilder.SendMessage($"Reached Facing Object: {reachedFacingObject}");
        return reachedFacingObject;
    }
}