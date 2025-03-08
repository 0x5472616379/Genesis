using ArcticRS.Constants;
using Genesis.Entities;

namespace Genesis.Commands;

public class TestColorsCommand : RSCommand
{
    protected override PlayerRights RequiredRights { get; } = PlayerRights.ADMIN;

    public TestColorsCommand(Player player, string[] args) : base(player, args)
    {
    }

    public override bool Validate()
    {
        return true;
    }

    public override void Invoke()
    {
        string test = "@str@@red@Re@gre@Gr@blu@Bl@yel@Ye@end@@cya@Cy@mag@Ma@whi@Wh@bla@Bl@lre@LR@dre@DR@dbl@DB@or1@O1@or2@O2@or3@O3@gr1@G1@gr2@G2@gr3@G3";
        Player.Session.PacketBuilder.SendMessage(test);
    }
}