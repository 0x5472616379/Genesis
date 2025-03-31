namespace Genesis.Cache;

public readonly struct FileDescriptor(int type, int file)
{
    private readonly int _file = file;
    private readonly int _type = type;

    public readonly int GetFile() => _file;

    public readonly int GetDescriptorType() => _type;

    public readonly override int GetHashCode() => _file * FileSystemConstants.ArchiveCount + _type;

    public readonly override bool Equals(object? obj) =>
        obj switch
        {
            FileDescriptor other => _type == other._type && _file == other._file,
            _ => false
        };
    public static bool operator ==(FileDescriptor left, FileDescriptor right) => left.Equals(right);
    public static bool operator !=(FileDescriptor left, FileDescriptor right) => !(left == right);
}