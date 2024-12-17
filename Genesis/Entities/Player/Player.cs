using Genesis.Client;

namespace Genesis.Entities;

public class Player : Entity
{
    public NetworkSession Session { get; set; }

    public Player()
    {
        Session = new NetworkSession(this);
    }
}