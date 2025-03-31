using Genesis.Network.Packets.Incoming;
using Genesis.Packets.Incoming;

namespace Genesis.Network;

public interface IPacket
{
    void Process();
}

public static class PacketFactory
{
    internal static bool ThrowOnUnknownPacket = false;
    public static IPacket? CreateClientPacket(int opcode, PacketParameters parameters) =>
        opcode switch
        {
            3 => new WindowFocusPacket(parameters),
            /* Regular Walk */
            98 or 164 or 248 => new WalkPacket(parameters),
            /* Walk To Interaction Object */
            41 => new EquipItemPacket(parameters),
            103 => new PlayerCommandPacket(parameters),
            122 => new FirstItemOptionPacket(parameters),
            132 => new InteractFirstOptionPacket(parameters),
            73 => new AttackPlayerPacket(parameters),
            249 => new SpellOnPlayer(parameters),
            39 => new FollowPlayerPacket(parameters),
            40 => new DialogueClickPacket(parameters),
            130 => new CloseWindowPacket(parameters),
            192 => new ItemOnWorldObjectPacket(parameters),
            236 => new PickupItemPacket(parameters),
            117 => new WithdrawSecondOptionFromContainerPacket(parameters),
            43 => new WithdrawThirdFromContainerPacket(parameters),
            129 => new WithdrawFourthFromContainerPacket(parameters),
            145 => new WithdrawFirstOptionFromContainerPacket(parameters),
            214 => new MoveItemInContainerPacket(parameters),
            185 => new ButtonClickPacket(parameters),
            210 => new MapRebuildAckPacket(parameters),
            241 => new UnusedPacket(), // new MouseInputPacket(parameters),
            _ => ThrowOnUnknownPacket ? throw new InvalidOperationException($"Unrecognized opcode found: {opcode}.") : new UnusedPacket(),
        };
}