using System.Text;

namespace Genesis;

/**
 * A utility class which contains byte buffer related utility methods.
 */
public static class BufferUtil
{
    public const int StringTerminator = 10;
    private static byte[] _buffer = new byte[sizeof(int)];

    public static int ReadSmart(MemoryStream stream)
    {
        var peek = stream.ReadByte();
        if (peek >= 128)
            return (short)(((peek << 8) & 0xFF00) | stream.ReadByte());
        return peek;
    }

    public static string ReadString(BinaryReader reader)
    {
        var builder = new StringBuilder();
        char character;
        while ((character = (char)reader.ReadByte()) != StringTerminator) builder.Append(character);
        return builder.ToString();
    }


    public static int ReadUnsignedMedium(MemoryStream stream)
    {
        return (stream.ReadByte() << 16) | (stream.ReadByte() << 8) | stream.ReadByte();
    }

    public static short ReadInt16BE(this MemoryStream stream)
    {
        stream.Read(_buffer, 0, 2);
        return (short)((_buffer[0] << 8) | _buffer[1]);
    }

    public static int ReadInt32BE(this MemoryStream stream)
    {
        stream.Read(_buffer, 0, 4);
        return (_buffer[0] << 24) | (_buffer[1] << 16) | (_buffer[2] << 8) | _buffer[3];
    }

    public static short ReadInt16BigEndian(this BinaryReader binaryReader)
    {
        binaryReader.Read(_buffer, 0, 2);
        var value = (short)((_buffer[0] << 8) | _buffer[1]);
        return value;
    }

    public static void Skip(this MemoryStream ms, int count)
    {
        ms.Seek(count, SeekOrigin.Current);
    }


    public static int GetUByte(this MemoryStream stream)
    {
        return stream.ReadByte() & 0xFF;
    }

    public static uint GetUShort(this MemoryStream stream)
    {
        var b1 = stream.ReadByte();
        var b2 = stream.ReadByte();
        return (uint)((b1 << 8) + b2);
    }

    public static uint GetUSmart(this MemoryStream stream)
    {
        var i = stream.ReadByte();
        if (i < 128) return (uint)i;

        stream.Position--;
        return stream.GetUShort() - 32768;
    }
}