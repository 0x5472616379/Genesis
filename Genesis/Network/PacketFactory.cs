namespace Genesis;

public interface IPacket
{
    void Process();
}

public static class PacketFactory
{
    public static IPacket CreateClientPacket(int opcode, PacketParameters parameters)
    {
        switch (opcode)
        {
            default:
                Console.WriteLine($"No packet class implementation for opcode {opcode}.");
                return null;
        }
    }
}