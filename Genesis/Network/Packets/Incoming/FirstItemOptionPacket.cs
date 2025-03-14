using ArcticRS.Actions;
using Genesis.Entities;
using Genesis.Environment;

namespace Genesis.Packets.Incoming;

public class FirstItemOptionPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;

    private readonly int _containerId;
    private readonly int _index;
    private readonly int _itemId;

    public FirstItemOptionPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;

        _containerId = _player.Session.Reader.ReadSignedWordBigEndianA();
        _index = _player.Session.Reader.ReadSignedWordA();
        _itemId = _player.Session.Reader.ReadSignedWordBigEndian();
    }

    public void Process()
    {
        Console.WriteLine(_containerId);
        Console.WriteLine(_index);
        Console.WriteLine(_itemId);


        // if (_itemId == 526)
        // {
        //     if (_player.ActionHandler.ContainsActionOfCategory(ActionCategory.BURY))
        //         return;
        //     
        //     _player.ActionHandler.AddAction(new BuryAction(_player));
        // }
        //
        // if (_itemId == 379)
        // {
        //     if (_player.ActionHandler.ContainsActionOfCategory(ActionCategory.EAT))
        //         return;
        //     
        //     _player.ActionHandler.AddAction(new EatAction(_player));
        //     // _player.PacketBuilder.SendMessage($"Queued Eat On Tick: {World.CurrentTick}");
        // }
    }
}