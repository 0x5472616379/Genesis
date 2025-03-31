namespace Genesis.Cache.Idx;

public class IndexedFileSystem : IDisposable
{
    private static FileStream _data;
    private readonly Dictionary<FileDescriptor, Archive> _cache = new(FileSystemConstants.ArchiveCount);
    private readonly FileStream[] _indexes;
    private readonly bool _readOnly;
    [Obsolete] private int[] _crcs = [];
    [Obsolete] private byte[] _crcTable = [];

    public IndexedFileSystem(string basePath, bool readOnly)
    {
        _readOnly = readOnly;

        var indexes = Directory.GetFiles(basePath, "main_file_cache.idx?", SearchOption.TopDirectoryOnly);
        _indexes = new FileStream[indexes.Length];
        var indexCount = 0;
        foreach (var index in indexes)
        {
            _indexes[indexCount++] = new(index, _readOnly ? FileMode.Open : FileMode.OpenOrCreate, _readOnly ? FileAccess.Read : FileAccess.ReadWrite);
        }

        if (indexCount <= 0) throw new FileNotFoundException($"No index file(s) present in {Path.GetFullPath(basePath)}.");

        var resourcesPath = Path.Combine(basePath, "main_file_cache.dat");
        if (File.Exists(resourcesPath) && !Directory.Exists(resourcesPath))
            _data = new FileStream(resourcesPath, _readOnly ? FileMode.Open : FileMode.OpenOrCreate, _readOnly ? FileAccess.Read : FileAccess.ReadWrite);
        else
            throw new FileNotFoundException("No data file present.");
    }

    public void Dispose()
    {
        _data?.Close();

        foreach (var index in _indexes) 
            index?.Close();
    }

    public Archive GetArchive(int type, int file)
    {
        var descriptor = new FileDescriptor(type, file);
        if (_cache.TryGetValue(descriptor, out var cached)) return cached;

        cached = Archive.Decode(GetFile(descriptor));
        lock (_cache)
        {
            _cache.Add(descriptor, cached);
        }

        return cached;
    }

    public MemoryStream GetFile(FileDescriptor descriptor)
    {
        var index = GetIndex(descriptor);
        var buffer = new byte[index.GetSize()];
        var position = index.GetBlock() * FileSystemConstants.BlockSize;
        var read = 0;
        var size = index.GetSize();
        var blocks = size / FileSystemConstants.ChunkSize;
        if (size % FileSystemConstants.ChunkSize != 0) blocks++;

        lock (_data)
        {
            for (var i = 0; i < blocks; i++)
            {
                var header = new byte[FileSystemConstants.HeaderSize];
                
                _data.Seek(position, SeekOrigin.Begin);
                _data.Read(header);

                position += FileSystemConstants.HeaderSize;

                var nextFile = header[0] << 8 | header[1];
                var curChunk = header[2] << 8 | header[3];
                var nextBlock = header[4] << 16 | header[5] << 8 | header[6];
                var nextType = header[7];

                if (i != curChunk) 
                    throw new InvalidOperationException("Chunk id mismatch.");

                var chunkSize = size - read;
                if (chunkSize > FileSystemConstants.ChunkSize) chunkSize = FileSystemConstants.ChunkSize;

                var chunk = new byte[chunkSize];
                
                _data.Seek(position, SeekOrigin.Begin);
                _data.Read(chunk);

                chunk.CopyTo(buffer, read);

                read += chunkSize;
                position = nextBlock * FileSystemConstants.BlockSize;

                // if we still have more data to read, check the validity of the header
                if (size > read)
                {
                    if (nextType != descriptor.GetDescriptorType() + 1) throw new InvalidOperationException("File type mismatch.");

                    if (nextFile != descriptor.GetFile()) throw new InvalidOperationException("File id mismatch.");
                }
            }
        }

        return new MemoryStream(buffer);
    }

    public MemoryStream GetFile(int type, int file) => 
        GetFile(new(type, file));

    private int GetFileCount(int type)
    {
        if (type < 0 || type >= _indexes.Length) throw new IndexOutOfRangeException("File type out of bounds.");

        var indexFile = _indexes[type];
        lock (indexFile)
        {
            return (int)(indexFile.Length / FileSystemConstants.IndexSize);
        }
    }

    private Index GetIndex(FileDescriptor descriptor)
    {
        var index = descriptor.GetDescriptorType();
        if (index < 0 || index >= _indexes.Length)
            throw new IndexOutOfRangeException("File descriptor type out of bounds.");

        var buffer = new byte[FileSystemConstants.IndexSize];
        var indexFile = _indexes[index];
        lock (indexFile)
        {
            long position = descriptor.GetFile() * FileSystemConstants.IndexSize;
            if (position >= 0 && indexFile.Length >= position + FileSystemConstants.IndexSize)
            {
                indexFile.Seek(position, SeekOrigin.Begin);
                indexFile.Read(buffer);
            }
            else
            {
                throw new FileNotFoundException("Could not find index.");
            }
        }

        return Index.Decode(buffer);
    }
}