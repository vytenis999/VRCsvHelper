using System.Reflection;
using VRCsvHelper.Exceptions;
using VRCsvHelper.Formatting;
using VRCsvHelper.Models;

namespace VRCsvHelper.Core;

public class CsvWriter<T>(CsvConfiguration? configuration = null, ILineFormatter? formatter = null)
{
    private readonly CsvConfiguration _configuration = configuration ?? new CsvConfiguration();
    private readonly ILineFormatter _formatter = formatter ?? new DefaultLineFormatter();

    public void WriteToFile(IEnumerable<T> records, string filePath)
    {
        try
        {
            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            WriteToStream(records, stream);
        }
        catch (CsvWriteException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CsvWriteException($"Failed to write CSV to '{filePath}'.", ex);
        }
    }

    public void WriteToStream(IEnumerable<T> records, Stream stream)
    {
        using var writer = new StreamWriter(stream, _configuration.Encoding, leaveOpen: true);
        WriteToTextWriter(records, writer);
    }

    public string WriteToString(IEnumerable<T> records)
    {
        using var writer = new StringWriter();
        WriteToTextWriter(records, writer);
        return writer.ToString();
    }

    private void WriteToTextWriter(IEnumerable<T> records, TextWriter writer)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  .Where(p => p.CanRead)
                                  .ToArray();

        if (properties.Length == 0)
            throw new CsvWriteException($"Type '{typeof(T).Name}' has no readable public properties to export.");

        if (_configuration.HasHeaders)
        {
            var headerLine = _formatter.Format(properties.Select(p => p.Name), _configuration);
            writer.WriteLine(headerLine);
        }

        foreach (var record in records)
        {
            if (record == null) continue;
            var fields = properties.Select(p => p.GetValue(record)?.ToString() ?? string.Empty);
            var line = _formatter.Format(fields, _configuration);
            writer.WriteLine(line);
        }
    }
}