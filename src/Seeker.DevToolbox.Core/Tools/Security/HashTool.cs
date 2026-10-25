using System.Security.Cryptography;
using System.Text;

namespace Seeker.DevToolbox.Core.Tools.Security;

public enum HashKind
{
    Md5,
    Sha1,
    Sha256,
    Sha512
}

public static class HashTool
{
    public static string Compute(string text, HashKind kind)
    {
        ArgumentNullException.ThrowIfNull(text);
        return ToHex(HashData(Encoding.UTF8.GetBytes(text), kind));
    }

    public static string ComputeHmac(string text, string key, HashKind kind)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(key);

        var data = Encoding.UTF8.GetBytes(text);
        var keyBytes = Encoding.UTF8.GetBytes(key);

        return ToHex(HmacData(keyBytes, data, kind));
    }

    public static string ComputeFile(string path, HashKind kind)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        using var stream = File.OpenRead(path);

        var hash = kind switch
        {
            HashKind.Md5 => MD5.HashData(stream),
            HashKind.Sha1 => SHA1.HashData(stream),
            HashKind.Sha256 => SHA256.HashData(stream),
            HashKind.Sha512 => SHA512.HashData(stream),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

        return ToHex(hash);
    }

    private static byte[] HashData(byte[] data, HashKind kind) => kind switch
    {
        HashKind.Md5 => MD5.HashData(data),
        HashKind.Sha1 => SHA1.HashData(data),
        HashKind.Sha256 => SHA256.HashData(data),
        HashKind.Sha512 => SHA512.HashData(data),
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    private static byte[] HmacData(byte[] key, byte[] data, HashKind kind) => kind switch
    {
        HashKind.Md5 => HMACMD5.HashData(key, data),
        HashKind.Sha1 => HMACSHA1.HashData(key, data),
        HashKind.Sha256 => HMACSHA256.HashData(key, data),
        HashKind.Sha512 => HMACSHA512.HashData(key, data),
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    private static string ToHex(byte[] bytes) => Convert.ToHexString(bytes).ToLowerInvariant();
}
