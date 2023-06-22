using System.CommandLine;
using System.Drawing;
using System.Reflection;
using Figgle;
using Pastel;

var textOption = new Option<string>(new string[] { "-t", "--text" }, () => "Hello, world!", "Specify the text to be rendered.");
var fontOption = new Option<List<string>>(new string[] { "-f", "--font" }, "Specify which font(s) will be used to render the text.")
{
    AllowMultipleArgumentsPerToken = true,
    Arity = ArgumentArity.ZeroOrMore
};

var rootCommand = new RootCommand("Render text using figgle fonts")
{
    textOption,
    fontOption
};

rootCommand.SetHandler((textOptionValue, fontOptionValue) =>
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
}, textOption, fontOption);

await rootCommand.InvokeAsync(args);
