using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Seeker.DevToolbox.Core.Tools.Xml;

public static class XmlFormatter
{
    public static string Format(string input, int indentSize = 2)
    {
        var document = Load(input);
        return Write(document, indent: true, indentSize);
    }

    public static string Minify(string input)
    {
        var document = Load(input);
        return Write(document, indent: false, indentSize: 0);
    }

    public static bool TryFormat(string input, out string result, out string? error)
    {
        try
        {
            result = Format(input);
            error = null;
            return true;
        }
        catch (XmlException ex)
        {
            result = string.Empty;
            error = $"XML 语法错误:第 {ex.LineNumber} 行,第 {ex.LinePosition} 列。{ex.Message}";
            return false;
        }
    }

    public static bool TryMinify(string input, out string result, out string? error)
    {
        try
        {
            result = Minify(input);
            error = null;
            return true;
        }
        catch (XmlException ex)
        {
            result = string.Empty;
            error = $"XML 语法错误:第 {ex.LineNumber} 行,第 {ex.LinePosition} 列。{ex.Message}";
            return false;
        }
    }

    private static XDocument Load(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var document = XDocument.Parse(input, LoadOptions.PreserveWhitespace);

        foreach (var text in document
                     .DescendantNodes()
                     .OfType<XText>()
                     .Where(node => string.IsNullOrWhiteSpace(node.Value))
                     .ToList())
        {
            text.Remove();
        }

        return document;
    }

    private static string Write(XDocument document, bool indent, int indentSize)
    {
        var settings = new XmlWriterSettings
        {
            Indent = indent,
            IndentChars = new string(' ', Math.Max(0, indentSize)),
            NewLineChars = Environment.NewLine,
            OmitXmlDeclaration = document.Declaration is null,
            Encoding = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
        };

        using var stream = new MemoryStream();
        using (var writer = XmlWriter.Create(stream, settings))
        {
            document.Save(writer);
        }

        return System.Text.Encoding.UTF8.GetString(stream.ToArray());
    }
}
