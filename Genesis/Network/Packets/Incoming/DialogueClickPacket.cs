using Genesis.Entities.Player;
using Genesis.Network;

namespace Genesis.Packets.Incoming;

public class DialogueClickPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _interfaceId;

    public DialogueClickPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;

        _interfaceId = _player.Session.Reader.ReadSignedWord();
    }

    public void Process()
    {
        if (_player.DialogueManager.NextDialogue == -1)
            _player.Session.PacketBuilder.ClearAllInterfaces();
    }
}