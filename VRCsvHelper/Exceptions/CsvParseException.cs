namespace VRCsvHelper.Exceptions;

public class CsvParseException : Exception
{
    public int LineNumber { get; }

    public CsvParseException(string message, int lineNumber)
        : base($"Line {lineNumber}: {message}")
    {
        LineNumber = lineNumber;
    }

    public CsvParseException(string message, int lineNumber, Exception innerException)
        : base($"Line {lineNumber}: {message}", innerException)
    {
        LineNumber = lineNumber;
    }
}