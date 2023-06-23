using System.Collections.Immutable;
using System.CommandLine;
using System.Drawing;
using System.Reflection;
using System.Text;
using abremir.Figgle.CommandLine;
using Figgle;
using Pastel;

var textOption = new Option<string>(new string[] { "-t", "--text" }, () => "Hello, world!", "Specify the text to be rendered.");
var fontOption = new Option<List<string>>(new string[] { "-f", "--font" }, "Specify which font(s) will be used to render the text.")
{
    AllowMultipleArgumentsPerToken = true,
    Arity = ArgumentArity.ZeroOrMore
};
var listOption = new Option<bool>(new string[] { "-l", "--list" }, "Display list of all Figgle fonts");

var rootCommand = new RootCommand("Render text using figgle fonts")
{
    textOption,
    fontOption,
    listOption
};

rootCommand.SetHandler((textOptionValue, fontOptionValue, listOptionValue) =>
{
    var figgleFontProperties = typeof(FiggleFonts)
        .GetProperties(BindingFlags.Static | BindingFlags.Public)
        .Where(propertyInfo => propertyInfo.PropertyType == typeof(FiggleFont));

    if (fontOptionValue.Count is not 0)
    {
        figgleFontProperties = figgleFontProperties
            .Where(propertyInfo => fontOptionValue.Contains(propertyInfo.Name, StringComparer.OrdinalIgnoreCase));
    }

    figgleFontProperties = figgleFontProperties
        .OrderBy(propertyInfo => propertyInfo.Name);

    if (!figgleFontProperties.Any())
    {
        Console.WriteLine("No fonts found to render!".Pastel(Color.Orange));
        return;
    }

    if (listOptionValue)
    {
        var fontNames = figgleFontProperties
            .OrderBy(propertyInfo => propertyInfo.Name)
            .Select(propertyInfo => propertyInfo.Name)
            .ToImmutableList();
        var longestLenght = fontNames.Max(fontName => fontName.Length);
        var columns = Console.WindowWidth / longestLenght;

        var table = new TableBuilder(columns, longestLenght);
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(table.AddTopLine());

        foreach (var chunk in fontNames.Chunk(columns))
        {
            stringBuilder.AppendLine(table.AddRow(chunk));
        }

        stringBuilder.AppendLine(table.AddEndLine());

        Console.WriteLine(stringBuilder.ToString().Pastel(Color.LightGray));
        return;
    }

    foreach (var figgleFontProperty in figgleFontProperties)
    {
        try
        {
            Console.WriteLine(figgleFontProperty.Name.Pastel(Color.LimeGreen));
            Console.WriteLine(((FiggleFont)figgleFontProperty.GetValue(null)!).Render(textOptionValue).Pastel(Color.LightGray));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message.Pastel(Color.Red));
            Console.WriteLine(ex.StackTrace.Pastel(Color.IndianRed));
            Console.WriteLine("Press ENTER to continue...".Pastel(Color.White));
            Console.ReadLine();
        }
    }
}, textOption, fontOption, listOption);

await rootCommand.InvokeAsync(args);
