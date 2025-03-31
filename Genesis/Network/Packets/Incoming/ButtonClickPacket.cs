using Genesis.Entities.Player;
using Genesis.Managers;
using Genesis.Network;

namespace Genesis.Packets.Incoming;

public class ButtonClickPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _buttonId;
    
    private static readonly ButtonManager _buttonHandler = new();

    public ButtonClickPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
        
        var buttonData = _player.Session.Reader.ReadSignedWord(); //containerIndex
        _buttonId = ConversionExtension.HexToInt(_player.Session.Reader.Buffer, 0, _length);
        Console.WriteLine($"BUTTON DATA {buttonData}");
        Console.WriteLine($"BUTTON ID {_buttonId}");

    }
    
    public void Process()
    {
        _buttonHandler.HandleButtonClick(_player, _buttonId);
    }
}
