namespace Genesis.Network.Packets.Incoming;

internal sealed class MapRebuildAckPacket : IPacket
{
    public MapRebuildAckPacket(PacketParameters parameters)
    {
        var player = parameters.Player;
        var ack = player.Session.Reader.ReadDWord();
        if (ack != 0x3F008EDD)
        {
            player.Session.Disconnect(new(player, "Invalid Request"));
        }
    }


    public void Process() { }
}