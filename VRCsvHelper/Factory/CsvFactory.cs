using VRCsvHelper.Core;
using VRCsvHelper.Formatting;
using VRCsvHelper.Models;
using VRCsvHelper.Parsing;

namespace VRCsvHelper.Factory;

public static class CsvFactory
{
    public static CsvReader<T> CreateReader<T>(CsvConfiguration? configuration = null) where T : new()
        => new(configuration, new DefaultLineParser());

    public static CsvWriter<T> CreateWriter<T>(CsvConfiguration? configuration = null)
        => new(configuration, new DefaultLineFormatter());

    public static CsvReader<T> CreateSemicolonReader<T>() where T : new()
        => CreateReader<T>(new CsvConfiguration { Delimiter = ';' });

    public static CsvWriter<T> CreateSemicolonWriter<T>()
        => CreateWriter<T>(new CsvConfiguration { Delimiter = ';' });
}