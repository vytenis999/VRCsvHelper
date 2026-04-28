using System.Reflection;
using VRCsvHelper.Exceptions;
using VRCsvHelper.Models;
using VRCsvHelper.Parsing;

namespace VRCsvHelper.Core;

public class CsvReader<T>(CsvConfiguration? configuration = null, ILineParser? parser = null)
    where T : new()
{
    private readonly CsvConfiguration _configuration = configuration ?? new CsvConfiguration();
    private readonly ILineParser _parser = parser ?? new DefaultLineParser();

    public IReadOnlyList<T> ReadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"CSV file not found: {filePath}", filePath);

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return ReadFromStream(stream);
    }

    public IReadOnlyList<T> ReadFromStream(Stream stream)
    {
        using var reader = new StreamReader(stream, _configuration.Encoding, leaveOpen: true);
        return ReadFromTextReader(reader);
    }

    public IReadOnlyList<T> ReadFromString(string csvContent)
    {
        using var reader = new StringReader(csvContent);
        return ReadFromTextReader(reader);
    }


    private IReadOnlyList<T> ReadFromTextReader(TextReader reader)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  .Where(p => p.CanWrite)
                                  .ToArray();

        PropertyInfo?[]? columnMap = null;

        var results = new List<T>();
        var lineNumber = 0;

        while (reader.ReadLine() is { } line)
        {
            lineNumber++;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var fields = _parser.Parse(line, _configuration);

            if (columnMap == null)
            {
                if (_configuration.HasHeaders)
                {
                    columnMap = BuildMapFromHeaders(fields, properties);
                    continue;
                }

                columnMap = BuildMapFromOrder(properties, fields.Count);
            }

            results.Add(MapRow(fields, columnMap, lineNumber));
        }

        return results;
    }

    private static PropertyInfo?[] BuildMapFromHeaders(
        IReadOnlyList<string> headers,
        PropertyInfo[] properties)
    {
        var map = new PropertyInfo?[headers.Count];
        for (var i = 0; i < headers.Count; i++)
        {
            var header = headers[i].Trim();
            map[i] = Array.Find(properties,
                p => p.Name.Equals(header, StringComparison.OrdinalIgnoreCase));

            if (map[i] == null) continue;
        }
        return map;
    }

    private static PropertyInfo?[] BuildMapFromOrder(PropertyInfo[] properties, int columnCount)
    {
        var map = new PropertyInfo?[columnCount];
        for (var i = 0; i < Math.Min(columnCount, properties.Length); i++)
            map[i] = properties[i];
        return map;
    }

    private static T MapRow(IReadOnlyList<string> fields, PropertyInfo?[] columnMap, int lineNumber)
    {
        var obj = new T();
        for (var i = 0; i < Math.Min(fields.Count, columnMap.Length); i++)
        {
            var prop = columnMap[i];
            if (prop == null) continue;

            try
            {
                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var rawValue = fields[i].Trim();

                if (string.IsNullOrEmpty(rawValue) && Nullable.GetUnderlyingType(prop.PropertyType) != null)
                {
                    prop.SetValue(obj, null);
                }
                else
                {
                    var converted = Convert.ChangeType(rawValue, targetType);
                    prop.SetValue(obj, converted);
                }
            }
            catch (Exception ex)
            {
                throw new CsvParseException(
                    $"Cannot convert value '{fields[i]}' to type '{prop.PropertyType.Name}' for property '{prop.Name}'.",
                    lineNumber, ex);
            }
        }
        return obj;
    }
}