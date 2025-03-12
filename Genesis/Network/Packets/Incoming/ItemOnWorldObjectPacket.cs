using Genesis.Entities;
using Genesis.Interactions;
using Genesis.Movement;

namespace Genesis.Packets.Incoming;

public class ItemOnWorldObjectPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    
    private readonly int _offset;
    private readonly int _interfaceId;
    private readonly int _interfaceSlot;
    private readonly int _y;
    private readonly int _x;
    private readonly int _selectedObjectId;
    
    private readonly WorldInteractObject _worldObject;

    public ItemOnWorldObjectPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;

        _worldObject = new WorldInteractObject
        {
            InterfaceId = _player.Session.Reader.ReadSignedWord(),
            WorldLocDataBits = _player.Session.Reader.ReadSignedWordBigEndian(),
            Y = _player.Session.Reader.ReadSignedWordBigEndianA(),
            InterfaceSlot = _player.Session.Reader.ReadSignedWordBigEndian(),
            X = _player.Session.Reader.ReadSignedWordBigEndianA(),
            SelectedObjectId = _player.Session.Reader.ReadUnsignedWord()
        };
        
        int bitset = _worldObject.WorldLocDataBits;
        
        /* Check if non-interactable */
        bool isInteractable = (bitset & 0x80000000) == 0; // If bit 31 is set, it's NOT interactable.

        /* Remove constants to isolate the packed components */
        bitset &= 0x3FFFFFFF; // Mask out the top 2 bits (0xC0000000) from bitset.

        int x = bitset & 0x7F; // Lowest 7 bits for `x`
        int z = (bitset >> 7) & 0x7F; // Next 7 bits for `z`
        int locID = (bitset >> 14) & 0x3FFF; // Next 14 bits for `locID`
    }

    public void Process()
    {
        _player.CurrentInteraction = new ItemOnWorldObjectInteraction(_player, _worldObject);
    }
}

public class WorldInteractObject
{
    public int InterfaceId { get; set; }
    public int WorldLocDataBits { get; set; }
    public int Y { get; set; }
    public int InterfaceSlot { get; set; }
    public int X { get; set; }
    public int SelectedObjectId { get; set; }
}