using VRCsvHelper.Models;

namespace VRCsvHelper.Parsing;

/// <summary>
/// Defines the contract for a CSV parsing strategy.
/// Implementations decide how a single raw CSV line is split into fields
/// (Strategy pattern: swap implementations without changing the reader).
/// </summary>
public interface ILineParser
{
    /// <summary>
    /// Splits a raw CSV line into individual field values.
    /// </summary>
    /// <param name="line">A single line from the CSV file.</param>
    /// <param name="configuration">The active CSV configuration.</param>
    /// <returns>An ordered list of field values.</returns>
    IReadOnlyList<string> Parse(string line, CsvConfiguration configuration);
}