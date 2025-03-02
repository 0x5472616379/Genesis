using ArcticRS.Commands;
using Genesis.Cache;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Managers;

namespace Genesis.Commands;

public class GetWorldObjectCommand : CommandBase
{
    private int id = 0;
    private int x = 0;
    private int y = 0;

    public GetWorldObjectCommand(Player player, string[] args) : base(player, args)
    {
    }

    protected override string ValidateArgs()
    {
        id = int.Parse(Args[1]);
        x = int.Parse(Args[2]);
        y = int.Parse(Args[3]);

        return null;
    }

    protected override void Invoke()
    {
        
    }
}