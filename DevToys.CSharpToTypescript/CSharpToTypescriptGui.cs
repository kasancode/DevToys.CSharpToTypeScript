using DevToys.Api;
using DevToys.CSharpToTypescript.Converters;
using System.ComponentModel.Composition;
using System.Diagnostics;
using static DevToys.Api.GUI;

namespace DevToys.CSharpToTypescript;

[Export(typeof(IGuiTool))]
[Name("CSharpToTypescriptExtension")]
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",
    IconGlyph = '\uF0DF',
    GroupName = PredefinedCommonToolGroupNames.Converters,
    ResourceManagerAssemblyIdentifier = nameof(CSharpToTypescriptAssemblyIdentifier),
    ResourceManagerBaseName = "DevToys.CSharpToTypescript.CSharpToTypescriptExtension",
    ShortDisplayTitleResourceName = nameof(CSharpToTypescriptExtension.ShortDisplayTitle),
    LongDisplayTitleResourceName = nameof(CSharpToTypescriptExtension.LongDisplayTitle),
    DescriptionResourceName = nameof(CSharpToTypescriptExtension.Description),
    AccessibleNameResourceName = nameof(CSharpToTypescriptExtension.AccessibleName))]
internal sealed class CSharpToTypescriptGui : IGuiTool
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly IUIMultiLineTextInput _inputTextArea = MultiLineTextInput("csharp-to-typescript-input-text-area");
    private readonly IUIMultiLineTextInput _outputTextArea = MultiLineTextInput("csharp-to-typescript-output-text-area");
    private static readonly SettingDefinition<DateType> _dateAsString = new(name: "Convert Date like type to ", defaultValue: DateType.Union);
    private static readonly SettingDefinition<bool> _toCamelCase = new(name: "csharp-to-typescript-camel-case", defaultValue: false);
    private static readonly SettingDefinition<bool> _publicOnly = new(name: "csharp-to-typescript-public-only", defaultValue: true);

    [ImportingConstructor]
    public CSharpToTypescriptGui(ISettingsProvider settingsProvider)
    {
        this._settingsProvider = settingsProvider;
    }

    private enum GridColumn
    {
        Content
    }

    private enum GridRow
    {
        Header,
        Content,
        Footer
    }

    public UIToolView View => new
        (
            isScrollable: true,
            Grid()
                .ColumnLargeSpacing()
                .RowLargeSpacing()
                .Rows(
                    (GridRow.Header, Auto),
                    (GridRow.Content, new UIGridLength(1, UIGridUnitType.Fraction))
                )
                .Columns(
                    (GridColumn.Content, new UIGridLength(1, UIGridUnitType.Fraction))
                )
            .Cells(
                Cell(
                    GridRow.Header,
                    GridColumn.Content,
                    Stack()
                        .Vertical()
                        .LargeSpacing()
                        .WithChildren(
                            Label().Text(CSharpToTypescriptExtension.ConfigurationTitle),
                            Setting()
                                .Icon("FluentSystemIcons", '\uE243')
                                .Title("Date type")
                                .Description("Convert Date type to")

                                .Handle(
                                    this._settingsProvider,
                                    _dateAsString,
                                    this.OnDateTypeChanged,
                                    Item("Union", DateType.Union),
                                    Item("string", DateType.String),
                                    Item("Date", DateType.Date)
                                ),
                            Setting()
                                .Icon("FluentSystemIcons", '\uF797')
                                .Title("To camel case")
                                .Description("Convert member name to camel case.")
                                .Handle(
                                    this._settingsProvider,
                                    _toCamelCase,
                                    this.OnToCamelCaseChanged
                                ),
                            Setting()
                                .Icon("FluentSystemIcons", '\uF581')
                                .Title("Public only")
                                .Description("Convert only public classes, interfaces and members.")
                                .Handle(
                                    this._settingsProvider,
                                    _publicOnly,
                                    this.OnPublicOnlyChanged
                                )
                        )
                ),
                Cell(
                    GridRow.Content,
                    GridColumn.Content,
                    SplitGrid()
                        .Vertical()
                        .WithLeftPaneChild(
                            this._inputTextArea
                                .Title(CSharpToTypescriptExtension.InputTitle)
                                .Language("csharp")
                                .OnTextChanged(this.OnInputTextChanged))
                        .WithRightPaneChild(
                            this._outputTextArea
                                .Title(CSharpToTypescriptExtension.OutputTitle)
                                .Language("typescript")
                                .ReadOnly()
                                .Extendable()
                        )
                )
            )
        );

    public void OnDataReceived(string dataTypeName, object? parsedData) => throw new NotImplementedException();


    private void OnDateTypeChanged(DateType _)
    {
        this.Convert();
    }

    private void OnToCamelCaseChanged(bool _)
    {
        this.Convert();
    }

    private void OnPublicOnlyChanged(bool _)
    {
        this.Convert();
    }


    private void OnInputTextChanged(string _)
    {
        this.Convert();
    }

    private void Convert()
    {
        var cs = this._inputTextArea.Text;
        var dateAsString = _settingsProvider.GetSetting(_dateAsString);
        var toCamelCase = _settingsProvider.GetSetting(_toCamelCase);
        var publicOnly = _settingsProvider.GetSetting(_publicOnly);

        if (string.IsNullOrEmpty(cs))
        {
            this._outputTextArea.Text(string.Empty);
            return;
        }

        try
        {
            var converter = new CsTsCodeConveter(cs, dateAsString, toCamelCase, publicOnly);
            this._outputTextArea.Text(converter.Convert());
        }
        catch
        {
            this._outputTextArea.Text("Please provide a valid C# code.");
        }
    }
}