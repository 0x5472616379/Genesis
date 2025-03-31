using Genesis.Entities.Player;

namespace Genesis.Client;

public class DisconnectInfo
{
    public DisconnectInfo(Player player, string reason)
    {
        Player = player;
        Reason = reason;
    }

    public Player Player { get; }
    public string Reason { get; }
}