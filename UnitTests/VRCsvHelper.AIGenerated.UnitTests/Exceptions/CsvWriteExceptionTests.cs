using VRCsvHelper.Exceptions;

namespace VRCsvHelper.AIGenerated.UnitTests.Exceptions;

public class CsvWriteExceptionTests
{
    [Fact]
    public void Constructor_SetsMessage()
    {
        var ex = new CsvWriteException("write failed");

        Assert.Equal("write failed", ex.Message);
    }

    [Fact]
    public void Constructor_WithInnerException_SetsMessageAndInnerException()
    {
        var inner = new IOException("disk full");
        var ex = new CsvWriteException("write failed", inner);

        Assert.Equal("write failed", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }
}
