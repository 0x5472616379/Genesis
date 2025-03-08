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
            /* Regular Walk */
            case 98:
            case 164:
            case 248:
                return new WalkPacket(parameters);
            /* Walk To Interaction Object */
            case 41:
                return new EquipItemPacket(parameters);
            case 103:
                return new PlayerCommandPacket(parameters);
            case 122:
                return new FirstItemOptionPacket(parameters);
            case 132:
                return new InteractFirstOptionPacket(parameters);
            case 73:
                return new AttackPlayerPacket(parameters);
            case 39:
                return new FollowPlayerPacket(parameters);
            case 40:
                return new DialogueClickPacket(parameters);

            default:
                Console.WriteLine($"No packet class implementation for opcode {opcode}.");
                return null;
        }
    }
}