namespace HID;

internal static class DESIREDACCESS
{
    public const uint GENERIC_READ = 2147483648u;

    public const uint GENERIC_WRITE = 1073741824u;

    public const uint GENERIC_EXECUTE = 536870912u;

    public const uint GENERIC_ALL = 268435456u;

    public const uint FILE_SHARE_READ = 1u;

    public const uint FILE_SHARE_WRITE = 2u;
}