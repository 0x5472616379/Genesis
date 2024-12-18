using System.Net.Sockets;
using Genesis.Configuration;
using Genesis.Entities;
using Genesis.Environment;

namespace Genesis.Client;

public class NetworkSession
{
    private readonly Player _owner;

    private int _opCode = -1;
    private int _packetLength = -1;
    private FetchState _state = FetchState.READ_OPCODE;

    public NetworkSession(Player player)
    {
        _owner = player;
    }

    public int Index { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public TcpClient Socket { get; set; }
    public RSStream Reader { get; set; }
    public RSStream Writer { get; set; }

    public NetworkStream NetworkStream { get; set; }
    public SessionEncryption InEncryption { get; set; }
    public SessionEncryption OutEncryption { get; set; }

    public PacketBuilder PacketBuilder { get; set; }

    // public PacketStore PacketStore { get; set; } = new();

    public PacketCache PacketCache { get; set; } = new();

    public void Initialize(TcpClient client)
    {
        Socket = client;
        NetworkStream = client.GetStream();
        Reader = new RSStream(new byte[ServerConfig.BUFFER_SIZE]);
        Writer = new RSStream(new byte[ServerConfig.BUFFER_SIZE]);
        PacketBuilder = new PacketBuilder(_owner);
    }

    public void Fill(int count)
    {
        try
        {
            Reader.CurrentOffset = 0;
            NetworkStream.Read(Reader.Buffer, 0, count);
        }
        catch (IOException ex)
        {
            Disconnect(new DisconnectInfo(_owner, ex.Message));
        }
        catch (ObjectDisposedException e)
        {
            Disconnect(
                new DisconnectInfo(_owner, "The socket was unexpectedly closed. Exception message: " + e.Message));
        }
    }

    public void Fetch()
    {
        if (_state == FetchState.READ_OPCODE)
        {
            if (Socket.Available == 0) return;

            Fill(1);

            _opCode = (byte)(Reader.ReadUnsignedByte() - InEncryption.GetNextKey());
            _packetLength = GameConstants.INCOMING_SIZES[_opCode];

            _state = _packetLength switch
            {
                0 => FetchState.READ_OPCODE, // we should add this packet to the player even if its empty
                -1 => FetchState.READ_VAR_SIZE,
                _ => FetchState.READ_PAYLOAD
            };
        }

        if (_state == FetchState.READ_VAR_SIZE)
        {
            if (Socket.Available == 0) return;

            Fill(1);

            _packetLength = Reader.ReadUnsignedByte();
            _state = FetchState.READ_PAYLOAD;
        }

        if (_state != FetchState.READ_PAYLOAD) return;

        if (_packetLength > Socket.Available) return;

        Fill(_packetLength);
        Console.WriteLine($"[{_opCode}] Packet Received - Length: {_packetLength}");

         var packet = PacketFactory.CreateClientPacket(_opCode, new PacketParameters { OpCode = _opCode, Length = _packetLength, Player = _owner });
        
        PacketCache.Packets.Add(packet);
        
        _state = FetchState.READ_OPCODE;
    }

    public void Disconnect(DisconnectInfo disconnectInfo)
    {
        Socket.Close();
        World.RemovePlayer(disconnectInfo.Player);
        Console.WriteLine($"Client {Index} disconnected. Reason: {disconnectInfo.Reason}");
    }

    public void Flush()
    {
        try
        {
            NetworkStream.Write(Writer.Buffer, 0, Writer.CurrentOffset);
            Writer.CurrentOffset = 0;
        }
        catch (IOException ex)
        {
            Disconnect(new DisconnectInfo(_owner, ex.Message));
        }
        catch (ObjectDisposedException e)
        {
            Disconnect(
                new DisconnectInfo(_owner, "The socket was unexpectedly closed. Exception message: " + e.Message));
        }
    }

    public int Available()
    {
        return Socket.Available;
    }

    public void Close()
    {
        Socket.Close();
    }
}

internal enum FetchState
{
    READ_OPCODE = 0,
    READ_VAR_SIZE = 1,
    READ_PAYLOAD = 2
}