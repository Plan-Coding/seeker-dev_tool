using System.Text;
using Seeker.DevToolbox.Core.Tools.TextEncodings;

namespace Seeker.DevToolbox.Core.Tests;

public class FileEncodingServiceTests
{
    [Fact]
    public void Detect_Utf8Bom()
    {
        var bytes = new byte[] { 0xEF, 0xBB, 0xBF, 0xE4, 0xBD, 0xA0 };

        var detection = FileEncodingService.Detect(bytes);

        Assert.Equal("utf-8", detection.Id);
        Assert.True(detection.HasBom);
    }

    [Fact]
    public void Detect_Utf16LittleEndian()
    {
        var bytes = new byte[] { 0xFF, 0xFE, 0x60, 0x4F };

        var detection = FileEncodingService.Detect(bytes);

        Assert.Equal("utf-16-le", detection.Id);
        Assert.True(detection.HasBom);
    }

    [Fact]
    public void Detect_GbkBytes_FallsBackToGb18030()
    {
        var gbk = TextEncodingConverter.GetEncoding("gbk");
        var bytes = gbk.GetBytes("你好");

        var detection = FileEncodingService.Detect(bytes);

        Assert.Equal("gb18030", detection.Id);
        Assert.False(detection.HasBom);
    }

    [Fact]
    public void Detect_PlainAscii_IsUtf8()
    {
        Assert.Equal("utf-8", FileEncodingService.Detect(Encoding.ASCII.GetBytes("hello")).Id);
    }

    [Fact]
    public void TryRead_WithBom_StripsBom()
    {
        var path = Path.Combine(Path.GetTempPath(), $"seeker-enc-{Guid.NewGuid():N}.txt");

        try
        {
            File.WriteAllText(path, "你好", new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

            Assert.True(FileEncodingService.TryRead(path, 1024, out var result, out var error));

            Assert.Null(error);
            Assert.Equal("你好", result.Text);
            Assert.True(result.Detection.HasBom);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void TryWrite_Utf8Bom_ProducesBom()
    {
        var path = Path.Combine(Path.GetTempPath(), $"seeker-enc-{Guid.NewGuid():N}.txt");

        try
        {
            Assert.True(FileEncodingService.TryWrite(path, "abc", "utf-8", writeBom: true, out var error));
            Assert.Null(error);

            var bytes = File.ReadAllBytes(path);
            Assert.Equal(0xEF, bytes[0]);
            Assert.Equal(0xBB, bytes[1]);
            Assert.Equal(0xBF, bytes[2]);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void TryConvertFile_GbkToUtf8_PreservesText()
    {
        var inputPath = Path.Combine(Path.GetTempPath(), $"seeker-gbk-{Guid.NewGuid():N}.txt");
        var outputPath = Path.Combine(Path.GetTempPath(), $"seeker-utf8-{Guid.NewGuid():N}.txt");

        try
        {
            var gbk = TextEncodingConverter.GetEncoding("gbk");
            File.WriteAllText(inputPath, "你好,工具箱", gbk);

            Assert.True(FileEncodingService.TryConvertFile(
                inputPath, outputPath, "gbk", "utf-8", writeBom: false, out var error));
            Assert.Null(error);

            var output = File.ReadAllText(outputPath, new UTF8Encoding(false));
            Assert.Equal("你好,工具箱", output);
        }
        finally
        {
            File.Delete(inputPath);
            File.Delete(outputPath);
        }
    }

    [Fact]
    public void TryRead_TruncatesLargeFile()
    {
        var path = Path.Combine(Path.GetTempPath(), $"seeker-enc-{Guid.NewGuid():N}.txt");

        try
        {
            File.WriteAllText(path, new string('a', 100), new UTF8Encoding(false));

            Assert.True(FileEncodingService.TryRead(path, 10, out var result, out _));

            Assert.True(result.Truncated);
            Assert.Equal(10, result.Text.Length);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
