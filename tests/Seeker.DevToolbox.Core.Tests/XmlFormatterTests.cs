using Seeker.DevToolbox.Core.Tools.Xml;

namespace Seeker.DevToolbox.Core.Tests;

public class XmlFormatterTests
{
    [Fact]
    public void Format_IndentsNestedElements()
    {
        var result = XmlFormatter.Format("<root><a>1</a><b><c>2</c></b></root>");

        Assert.Contains("\n", result);
        Assert.Contains("  <a>1</a>", result);
        Assert.Contains("    <c>2</c>", result);
    }

    [Fact]
    public void Format_KeepsDeclarationAndChineseText()
    {
        var result = XmlFormatter.Format("""<?xml version="1.0" encoding="utf-8"?><root><name>工具箱</name></root>""");

        Assert.StartsWith("<?xml", result);
        Assert.Contains("encoding=\"utf-8\"", result);
        Assert.Contains("工具箱", result);
    }

    [Fact]
    public void Minify_RemovesInterElementWhitespace()
    {
        var result = XmlFormatter.Minify("<root>\n  <a>1</a>\n  <b>2</b>\n</root>");

        Assert.Equal("<root><a>1</a><b>2</b></root>", result);
    }

    [Fact]
    public void TryFormat_InvalidXml_ReturnsLineAndColumn()
    {
        Assert.False(XmlFormatter.TryFormat("<root>", out var result, out var error));

        Assert.Empty(result);
        Assert.Contains("XML 语法错误", error);
        Assert.Contains("行", error);
    }
}
