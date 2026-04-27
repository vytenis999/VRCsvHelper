namespace VRCsvHelper.Exceptions;

public class CsvWriteException : Exception
{
    public CsvWriteException(string message) : base(message) { }

    public CsvWriteException(string message, Exception innerException)
        : base(message, innerException) { }
}