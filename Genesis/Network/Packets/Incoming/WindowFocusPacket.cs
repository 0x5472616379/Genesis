namespace Genesis.Network.Packets.Incoming;

internal sealed class WindowFocusPacket : IPacket
{
    public WindowFocusPacket(PacketParameters parameters)
    {
        var player = parameters.Player;
        var focused = player.Session.Reader.ReadByte() == 0;
    }

    public void Process() { }
}
