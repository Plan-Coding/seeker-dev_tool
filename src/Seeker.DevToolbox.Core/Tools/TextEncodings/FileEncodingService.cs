using System.Text;

namespace Seeker.DevToolbox.Core.Tools.TextEncodings;

public sealed record EncodingDetection(string Id, string DisplayName, bool HasBom);

public sealed record FileTextResult(
    string Text,
    EncodingDetection Detection,
    long FileSize,
    bool Truncated);

public static class FileEncodingService
{
    public static EncodingDetection Detect(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
        {
            return new EncodingDetection("utf-8", "UTF-8 (BOM)", true);
        }

        if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
        {
            return new EncodingDetection("utf-16-le", "UTF-16 LE (BOM)", true);
        }

        if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
        {
            return new EncodingDetection("utf-16-be", "UTF-16 BE (BOM)", true);
        }

        if (IsValidUtf8(bytes))
        {
            return new EncodingDetection("utf-8", "UTF-8", false);
        }

        try
        {
            _ = TextEncodingConverter.GetEncoding("gb18030").GetString(bytes);
            return new EncodingDetection("gb18030", "GB18030(启发式)", false);
        }
        catch (DecoderFallbackException)
        {
            return new EncodingDetection("latin1", "ISO-8859-1(回退)", false);
        }
    }

    public static bool TryRead(
        string path,
        int maxBytes,
        out FileTextResult result,
        out string? error)
    {
        result = null!;

        try
        {
            using var stream = File.OpenRead(path);
            var fileSize = stream.Length;
            var readLength = (int)Math.Min(fileSize, Math.Max(0, maxBytes));
            var bytes = new byte[readLength];
            stream.ReadExactly(bytes);

            var detection = Detect(bytes);
            var offset = detection.HasBom
                ? detection.Id == "utf-8" ? 3 : 2
                : 0;

            if (bytes.Length < offset)
            {
                offset = 0;
            }

            var encoding = TextEncodingConverter.GetEncoding(detection.Id);
            var text = encoding.GetString(bytes[offset..]);

            result = new FileTextResult(
                Text: text,
                Detection: detection,
                FileSize: fileSize,
                Truncated: fileSize > maxBytes);

            error = null;
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or DecoderFallbackException)
        {
            error = ex.Message;
            return false;
        }
    }

    public static bool TryWrite(
        string path,
        string text,
        string encodingId,
        bool writeBom,
        out string? error)
    {
        try
        {
            File.WriteAllText(path, text, CreateEncoding(encodingId, writeBom));
            error = null;
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or EncoderFallbackException)
        {
            error = ex.Message;
            return false;
        }
    }

    public static bool TryConvertFile(
        string inputPath,
        string outputPath,
        string sourceId,
        string targetId,
        bool writeBom,
        out string? error)
    {
        try
        {
            var sourceEncoding = TextEncodingConverter.GetEncoding(sourceId);
            var targetEncoding = CreateEncoding(targetId, writeBom);

            using var reader = new StreamReader(
                inputPath,
                sourceEncoding,
                detectEncodingFromByteOrderMarks: true);

            using var writer = new StreamWriter(outputPath, append: false, targetEncoding);

            var buffer = new char[81920];
            int read;

            while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                writer.Write(buffer, 0, read);
            }

            error = null;
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or EncoderFallbackException or DecoderFallbackException)
        {
            error = ex.Message;
            return false;
        }
    }

    private static Encoding CreateEncoding(string encodingId, bool writeBom) => encodingId switch
    {
        "utf-8" => new UTF8Encoding(writeBom, throwOnInvalidBytes: true),
        "utf-16-le" => new UnicodeEncoding(bigEndian: false, byteOrderMark: writeBom, throwOnInvalidBytes: true),
        "utf-16-be" => new UnicodeEncoding(bigEndian: true, byteOrderMark: writeBom, throwOnInvalidBytes: true),
        _ => TextEncodingConverter.GetEncoding(encodingId)
    };

    private static bool IsValidUtf8(ReadOnlySpan<byte> bytes)
    {
        try
        {
            _ = new UTF8Encoding(false, throwOnInvalidBytes: true).GetString(bytes);
            return true;
        }
        catch (DecoderFallbackException)
        {
            return false;
        }
    }
}
