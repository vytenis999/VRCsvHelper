using VRCsvHelper.Models;

namespace VRCsvHelper.Formatting;

/// <summary>
/// Defines the contract for a CSV formatting strategy.
/// Implementations decide how a collection of field values is serialised to a CSV line
/// (Strategy pattern: swap implementations without changing the writer).
/// </summary>
public interface ILineFormatter
{
    /// <summary>
    /// Formats a set of field values into a single CSV line string.
    /// </summary>
    /// <param name="fields">The field values to write.</param>
    /// <param name="configuration">The active CSV configuration.</param>
    /// <returns>A formatted CSV line.</returns>
    string Format(IEnumerable<string> fields, CsvConfiguration configuration);
}