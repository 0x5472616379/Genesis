using Genesis.Entities.Player;
using Genesis.Network;

namespace Genesis;

public class PacketCache
{
    private readonly Player _owner;
    const int MAX_CLIENT_EVENTS = 50;
    const int MAX_USER_EVENTS = 10;

    int clientEventCount = 0;
    int userEventCount = 0;

    Queue<IPacket> UserPacketQueue = new();

    public PacketCache(Player owner)
    {
        _owner = owner;
    }

    public void Process()
    {
        while (UserPacketQueue.Count > 0)
        {
            var packet = UserPacketQueue.Dequeue();
            packet.Process();

            userEventCount--;
        }
    }

    public void Add(int opCode, IPacket packet)
    {
        if (IsUserEvent(opCode))
        {
            AddToUserEventQueue(packet);
        }
    }

    private bool IsClientEvent(int opCode)
    {
        return opCode switch
        {
            _ => false
        };
    }

    private bool IsUserEvent(int opCode)
    {
        return opCode switch
        {
            41 => true,
            
            98 => true,
            248 => true,
            164 => true,
            
            103 => true,
            122 => true,
            
            40 => true,
            130 => true,
            
            236 => true,
            
            73 => true,
            249 => true,
            39 => true,
            192 => true,
            214 => true,
            
            145 => true,
            117 => true,
            43 => true,
            129 => true,
            185 => true,
            
            132 => true,
            _ => false
        };
    }

    private void AddToClientEventQueue(int opCode, int length)
    {
        var packet = PacketFactory.CreateClientPacket(opCode, new PacketParameters
        {
            OpCode = opCode,
            Length = length,
            Player = _owner
        });

        // _owner.ClientEventQueue.Enqueue(packet);
    }

    private void AddToUserEventQueue(IPacket packet)
    {
        if (userEventCount >= MAX_USER_EVENTS)
        {
            _owner.Session.PacketBuilder.SendMessage($"User event limit reached for player {_owner.Session.Username}. Ignoring packet.");
            return;
        }

        // Enqueue the packet if it passes the check
        UserPacketQueue.Enqueue(packet);
        userEventCount++;

        // _owner.Session.PacketBuilder.SendMessage("Queued Packet");
    }
}