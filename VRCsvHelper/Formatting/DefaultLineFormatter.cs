using VRCsvHelper.Models;

namespace VRCsvHelper.Formatting;

public class DefaultLineFormatter : ILineFormatter
{
    public string Format(IEnumerable<string> fields, CsvConfiguration configuration)
    {
        var parts = fields.Select(field => QuoteIfNeeded(field, configuration.Delimiter));
        return string.Join(configuration.Delimiter, parts);
    }

    private static string QuoteIfNeeded(string value, char delimiter)
    {
        var needsQuoting = value.Contains(delimiter)
                           || value.Contains('"')
                           || value.Contains('\n')
                           || value.Contains('\r');

        return !needsQuoting ? value :
            $"\"{value.Replace("\"", "\"\"")}\"";
    }
}