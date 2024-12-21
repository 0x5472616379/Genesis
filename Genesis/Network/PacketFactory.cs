using Genesis.Packets.Incoming;

namespace Genesis;

public interface IPacket
{
    void Process();
}

public static class PacketFactory
{
    public static IPacket? CreateClientPacket(int opcode, PacketParameters parameters)
    {
        switch (opcode)
        {
            case 103:
                return new PlayerCommandPacket(parameters);
            case 122:
                return new FirstOptionInteractObject(parameters);
            default:
                Console.WriteLine($"No packet class implementation for opcode {opcode}.");
                return null;
        }
    }
}