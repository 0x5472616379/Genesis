using Genesis.Entities.Player;

namespace Genesis;

public class PacketParameters
{
    public int OpCode { get; set; }
    public int Length { get; set; }
    public Player Player { get; set; }
}