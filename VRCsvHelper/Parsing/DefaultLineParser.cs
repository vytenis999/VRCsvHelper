using System.Text;
using VRCsvHelper.Models;

namespace VRCsvHelper.Parsing;

public class DefaultLineParser : ILineParser
{
    public IReadOnlyList<string> Parse(string line, CsvConfiguration configuration)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        var i = 0;
        while (i < line.Length)
        {
            var c = line[i];

            inQuotes = inQuotes ? ProcessQuotedCharacter(line, ref i, current) : ProcessUnquotedCharacter(c, configuration, fields, current);

            i++;
        }

        fields.Add(current.ToString());

        return fields;
    }

    private static bool ProcessQuotedCharacter(string line, ref int i, StringBuilder current)
    {
        var c = line[i];

        if (c == '"')
        {
            if (i + 1 >= line.Length || line[i + 1] != '"') return false;

            current.Append('"');
            i++;

            return true;
        }

        current.Append(c);
        return true;
    }

    private static bool ProcessUnquotedCharacter(char c, CsvConfiguration configuration, List<string> fields, StringBuilder current)
    {
        if (c == '"')
            return true;

        if (c == configuration.Delimiter)
        {
            fields.Add(current.ToString());
            current.Clear();
            return false;
        }

        current.Append(c);
        return false;
    }
}