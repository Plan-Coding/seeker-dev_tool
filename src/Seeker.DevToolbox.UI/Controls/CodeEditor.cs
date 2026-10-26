using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Wpf.Ui.Appearance;

namespace Seeker.DevToolbox.UI.Controls;

/// <summary>
/// AvalonEdit 的 WPF 绑定封装:提供可双向绑定的 Text、行号和主题感知的 JSON 高亮。
/// </summary>
public class CodeEditor : TextEditor
{
    private const string YamlDarkXshd = """
        <?xml version="1.0"?>
        <SyntaxDefinition name="YamlDark" extensions=".yaml;.yml" xmlns="http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008">
          <Color name="Key" foreground="#9CDCFE" />
          <Color name="String" foreground="#CE9178" />
          <Color name="Number" foreground="#B5CEA8" />
          <Color name="Keyword" foreground="#569CD6" />
          <Color name="Comment" foreground="#6A9955" />
          <RuleSet>
            <Span color="Comment" begin="#" />
            <Rule color="Key">(?m)^[ \t]*(?:-[ \t]*)?[^#\s][^:]*?(?=\s*:)</Rule>
            <Rule color="String">("(?:\\.|[^"])*"|'(?:''|[^'])*')</Rule>
            <Rule color="Number">\b-?\d+(\.\d+)?\b</Rule>
            <Rule color="Keyword">\b(true|false|null|yes|no|on|off)\b</Rule>
          </RuleSet>
        </SyntaxDefinition>
        """;

    private const string YamlLightXshd = """
        <?xml version="1.0"?>
        <SyntaxDefinition name="YamlLight" extensions=".yaml;.yml" xmlns="http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008">
          <Color name="Key" foreground="#0451A5" />
          <Color name="String" foreground="#A31515" />
          <Color name="Number" foreground="#098658" />
          <Color name="Keyword" foreground="#0000FF" />
          <Color name="Comment" foreground="#008000" />
          <RuleSet>
            <Span color="Comment" begin="#" />
            <Rule color="Key">(?m)^[ \t]*(?:-[ \t]*)?[^#\s][^:]*?(?=\s*:)</Rule>
            <Rule color="String">("(?:\\.|[^"])*"|'(?:''|[^'])*')</Rule>
            <Rule color="Number">\b-?\d+(\.\d+)?\b</Rule>
            <Rule color="Keyword">\b(true|false|null|yes|no|on|off)\b</Rule>
          </RuleSet>
        </SyntaxDefinition>
        """;

    private const string XmlDarkXshd = """
        <?xml version="1.0"?>
        <SyntaxDefinition name="XmlDark" extensions=".xml;.config" xmlns="http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008">
          <Color name="Tag" foreground="#569CD6" />
          <Color name="Attribute" foreground="#9CDCFE" />
          <Color name="String" foreground="#CE9178" />
          <Color name="Comment" foreground="#6A9955" />
          <Color name="Entity" foreground="#D7BA7D" />
          <RuleSet>
            <Span color="Comment" begin="&lt;!--" end="--&gt;" multiline="true" />
            <Rule color="Tag">&lt;/?[\w:.-]+</Rule>
            <Rule color="Attribute">[\w:.-]+(?=\s*=)</Rule>
            <Rule color="String">"[^"]*"|'[^']*'</Rule>
            <Rule color="Entity">&amp;[a-zA-Z#0-9]+;</Rule>
          </RuleSet>
        </SyntaxDefinition>
        """;

    private const string XmlLightXshd = """
        <?xml version="1.0"?>
        <SyntaxDefinition name="XmlLight" extensions=".xml;.config" xmlns="http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008">
          <Color name="Tag" foreground="#800000" />
          <Color name="Attribute" foreground="#0451A5" />
          <Color name="String" foreground="#A31515" />
          <Color name="Comment" foreground="#008000" />
          <Color name="Entity" foreground="#B8860B" />
          <RuleSet>
            <Span color="Comment" begin="&lt;!--" end="--&gt;" multiline="true" />
            <Rule color="Tag">&lt;/?[\w:.-]+</Rule>
            <Rule color="Attribute">[\w:.-]+(?=\s*=)</Rule>
            <Rule color="String">"[^"]*"|'[^']*'</Rule>
            <Rule color="Entity">&amp;[a-zA-Z#0-9]+;</Rule>
          </RuleSet>
        </SyntaxDefinition>
        """;

    private const string JsonDarkXshd = """
        <?xml version="1.0"?>
        <SyntaxDefinition name="JsonDark" extensions=".json" xmlns="http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008">
          <Color name="Property" foreground="#9CDCFE" />
          <Color name="String" foreground="#CE9178" />
          <Color name="Number" foreground="#B5CEA8" />
          <Color name="Keyword" foreground="#569CD6" />
          <Color name="Punctuation" foreground="#D4D4D4" />
          <Color name="Comment" foreground="#6A9955" />
          <RuleSet>
            <Span color="Comment" begin="//" />
            <Span color="Comment" begin="/\*" end="\*/" multiline="true" />
            <Rule color="Property">"(?:\\.|[^"\\])*"(?=\s*:)</Rule>
            <Rule color="String">"(?:\\.|[^"\\])*"</Rule>
            <Rule color="Number">\b-?\d+(\.\d+)?([eE][+-]?\d+)?\b</Rule>
            <Rule color="Keyword">\b(true|false|null)\b</Rule>
            <Rule color="Punctuation">[{}\[\],:]</Rule>
          </RuleSet>
        </SyntaxDefinition>
        """;

    private const string JsonLightXshd = """
        <?xml version="1.0"?>
        <SyntaxDefinition name="JsonLight" extensions=".json" xmlns="http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008">
          <Color name="Property" foreground="#0451A5" />
          <Color name="String" foreground="#A31515" />
          <Color name="Number" foreground="#098658" />
          <Color name="Keyword" foreground="#0000FF" />
          <Color name="Punctuation" foreground="#333333" />
          <Color name="Comment" foreground="#008000" />
          <RuleSet>
            <Span color="Comment" begin="//" />
            <Span color="Comment" begin="/\*" end="\*/" multiline="true" />
            <Rule color="Property">"(?:\\.|[^"\\])*"(?=\s*:)</Rule>
            <Rule color="String">"(?:\\.|[^"\\])*"</Rule>
            <Rule color="Number">\b-?\d+(\.\d+)?([eE][+-]?\d+)?\b</Rule>
            <Rule color="Keyword">\b(true|false|null)\b</Rule>
            <Rule color="Punctuation">[{}\[\],:]</Rule>
          </RuleSet>
        </SyntaxDefinition>
        """;

    private static IHighlightingDefinition? _jsonDark;
    private static IHighlightingDefinition? _jsonLight;
    private static IHighlightingDefinition? _yamlDark;
    private static IHighlightingDefinition? _yamlLight;
    private static IHighlightingDefinition? _xmlDark;
    private static IHighlightingDefinition? _xmlLight;

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(CodeEditor),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnTextChanged));

    public static readonly DependencyProperty HighlightingNameProperty = DependencyProperty.Register(
        nameof(HighlightingName),
        typeof(string),
        typeof(CodeEditor),
        new PropertyMetadata("Json", OnHighlightingNameChanged));

    public CodeEditor()
    {
        ShowLineNumbers = true;
        FontFamily = new FontFamily("Cascadia Mono, Consolas");
        FontSize = 13;
        WordWrap = false;
        Padding = new Thickness(8, 6, 8, 6);
        HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
        VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;

        Options.ConvertTabsToSpaces = true;
        Options.IndentationSize = 2;
        Options.EnableHyperlinks = false;
        Options.EnableEmailHyperlinks = false;

        SetResourceReference(BackgroundProperty, "ControlFillColorDefaultBrush");
        SetResourceReference(ForegroundProperty, "TextFillColorPrimaryBrush");

        TextChanged += OnEditorTextChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public new string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string HighlightingName
    {
        get => (string)GetValue(HighlightingNameProperty);
        set => SetValue(HighlightingNameProperty, value);
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (CodeEditor)d;
        var text = e.NewValue as string ?? string.Empty;

        if (string.Equals(editor.Document.Text, text, StringComparison.Ordinal))
        {
            return;
        }

        var caret = editor.CaretOffset;
        editor.Document.Text = text;
        editor.CaretOffset = Math.Min(caret, editor.Document.TextLength);
    }

    private static void OnHighlightingNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((CodeEditor)d).ApplyHighlighting();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ApplicationThemeManager.Changed -= OnThemeChanged;
        ApplicationThemeManager.Changed += OnThemeChanged;
        ApplyHighlighting();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ApplicationThemeManager.Changed -= OnThemeChanged;
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
    }

    private void OnThemeChanged(ApplicationTheme theme, Color systemAccent) => ApplyHighlighting();

    private void OnEditorTextChanged(object? sender, EventArgs e)
    {
        if (!string.Equals(Text, Document.Text, StringComparison.Ordinal))
        {
            SetCurrentValue(TextProperty, Document.Text);
        }
    }

    private void ApplyHighlighting()
    {
        if (!IsLoaded)
        {
            return;
        }

        var isDark = ApplicationThemeManager.GetAppTheme() != ApplicationTheme.Light;

        SyntaxHighlighting = (HighlightingName ?? string.Empty).ToLowerInvariant() switch
        {
            "json" => isDark
                ? _jsonDark ??= LoadDefinition(JsonDarkXshd)
                : _jsonLight ??= LoadDefinition(JsonLightXshd),

            "yaml" => isDark
                ? _yamlDark ??= LoadDefinition(YamlDarkXshd)
                : _yamlLight ??= LoadDefinition(YamlLightXshd),

            "xml" => isDark
                ? _xmlDark ??= LoadDefinition(XmlDarkXshd)
                : _xmlLight ??= LoadDefinition(XmlLightXshd),

            "" => null,

            _ => HighlightingManager.Instance.GetDefinition(HighlightingName)
        };
    }

    private static IHighlightingDefinition LoadDefinition(string xshd)
    {
        using var stringReader = new StringReader(xshd);
        using var xmlReader = XmlReader.Create(stringReader);
        return HighlightingLoader.Load(xmlReader, HighlightingManager.Instance);
    }
}
