using VRCsvHelper.Exceptions;

namespace VRCsvHelper.AIGenerated.UnitTests.Exceptions;

public class CsvParseExceptionTests
{
    [Fact]
    public void Constructor_SetsMessageWithLineNumber()
    {
        var ex = new CsvParseException("bad value", 5);

        Assert.Contains("5", ex.Message);
        Assert.Contains("bad value", ex.Message);
        Assert.Equal(5, ex.LineNumber);
    }

    [Fact]
    public void Constructor_WithInnerException_SetsMessageAndInnerException()
    {
        var inner = new InvalidCastException("cast failed");
        var ex = new CsvParseException("bad value", 3, inner);

        Assert.Contains("3", ex.Message);
        Assert.Contains("bad value", ex.Message);
        Assert.Equal(3, ex.LineNumber);
        Assert.Same(inner, ex.InnerException);
    }
}
