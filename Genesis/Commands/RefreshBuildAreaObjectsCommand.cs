using ArcticRS.Commands;
using Genesis.Cache;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Managers;

namespace Genesis.Commands;

public class RefreshBuildAreaObjectsCommand : CommandBase
{
    public RefreshBuildAreaObjectsCommand(Player player, string[] args) : base(player, args)
    {
    }

    protected override string ValidateArgs()
    {
        return null;
    }

    protected override void Invoke()
    {
        
    }
}