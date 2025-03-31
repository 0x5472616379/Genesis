using System.IO.Compression;
using ICSharpCode.SharpZipLib.BZip2;

namespace Genesis.Util;

/// <summary>
///     A utility class for performing compression/decompression.
/// </summary>
public static class CompressionUtil
{
    /// <summary>
    ///     Bzip2s the specified array, removing the header.
    /// </summary>
    /// <param name="uncompressed">The uncompressed array.</param>
    /// <returns>The compressed array.</returns>
    /// <exception cref="IOException">If there is an error compressing the array.</exception>
    public static byte[] Bzip2(byte[] uncompressed)
    {
        using var memStream = new MemoryStream();
        using var os = new BZip2OutputStream(memStream);

        os.Write(uncompressed);

        var compressed = memStream.ToArray();
        return compressed[4..]; // Strip the header
    }

    private static ReadOnlySpan<byte> BZipHeader() => [ (byte)'B', (byte)'Z', (byte)'h', (byte)'1'];

    /// <summary>
    ///     Debzip2s the compressed array and places the result into the decompressed array.
    /// </summary>
    /// <param name="compressed">The compressed array, <strong>without</strong> the header.</param>
    /// <param name="decompressed">The decompressed array.</param>
    /// <exception cref="IOException">If there is an error decompressing the array.</exception>
    public static void Debzip2(byte[] compressed, byte[] decompressed)
    {
        var newCompressed = new byte[compressed.Length + 4];
        BZipHeader().CopyTo(newCompressed);
        Array.Copy(compressed, 0, newCompressed, 4, compressed.Length);

        using var memStream = new MemoryStream(newCompressed);
        using var isStream = new BZip2InputStream(memStream);
        
        isStream.Read(decompressed);
    }

    /// <summary>
    ///     Degzips the compressed array and places the results into the decompressed array.
    /// </summary>
    /// <param name="compressed">The compressed array.</param>
    /// <param name="decompressed">The decompressed array.</param>
    /// <exception cref="IOException">If an I/O error occurs.</exception>
    public static void Degzip(byte[] compressed, byte[] decompressed)
    {
        using var memStream = new MemoryStream(compressed);
        using var isStream = new GZipStream(memStream, CompressionMode.Decompress);
        
        isStream.Read(decompressed);
    }

    public static byte[] Degzip(byte[] compressed)
    {
        using var ms = new MemoryStream(compressed);
        using var gzs = new GZipStream(ms, CompressionMode.Decompress);
        using var result = new MemoryStream();
        
        var buffer = new byte[1024];
        int read;
        while ((read = gzs.Read(buffer)) > 0) 
            result.Write(buffer, 0, read);

        return result.ToArray();
    }

    /// <summary>
    /// Inflates a GZip deflated stream.
    /// </summary>
    /// <param name="deflatedStream">Input deflated stream</param>
    /// <param name="inflatedStream">Inflate stream buffer to write to. Position is reset to 0 when function returns.</param>
    /// <returns>The number of bytes written to the inflated stream.</returns>
    public static long InflateToStream(in MemoryStream deflatedStream, in MemoryStream inflatedStream)
    {
        using var gzs = new GZipStream(deflatedStream, CompressionMode.Decompress);

        var buffer = new byte[1024];
        int read;
        while ((read = gzs.Read(buffer)) > 0)
            inflatedStream.Write(buffer, 0, read);

        var len = inflatedStream.Position;
        inflatedStream.Position = 0;

        return len;
    }
}