using VRCsvHelper.Formatting;
using VRCsvHelper.Models;

namespace VRCsvHelper.AIGenerated.UnitTests.Formatting;

public class DefaultLineFormatterTests
{
    private readonly DefaultLineFormatter _formatter = new();
    private readonly CsvConfiguration _config = new();

    [Fact]
    public void Format_SimplFields_JoinsWithDelimiter()
    {
        var result = _formatter.Format(["Alice", "30", "Engineer"], _config);
        Assert.Equal("Alice,30,Engineer", result);
    }

    [Fact]
    public void Format_FieldContainingDelimiter_IsQuoted()
    {
        var result = _formatter.Format(["New York, NY", "10001"], _config);
        Assert.Equal("\"New York, NY\",10001", result);
    }

    [Fact]
    public void Format_FieldContainingDoubleQuote_IsQuotedAndEscaped()
    {
        var result = _formatter.Format(["Say \"hello\""], _config);
        Assert.Equal("\"Say \"\"hello\"\"\"", result);
    }

    [Fact]
    public void Format_FieldContainingNewline_IsQuoted()
    {
        var result = _formatter.Format(["line1\nline2"], _config);
        Assert.Equal("\"line1\nline2\"", result);
    }

    [Fact]
    public void Format_FieldContainingCarriageReturn_IsQuoted()
    {
        var result = _formatter.Format(["line1\rline2"], _config);
        Assert.Equal("\"line1\rline2\"", result);
    }

    [Fact]
    public void Format_EmptyField_IsNotQuoted()
    {
        var result = _formatter.Format(["", "value"], _config);
        Assert.Equal(",value", result);
    }

    [Fact]
    public void Format_CustomDelimiter_UsesDelimiter()
    {
        var config = new CsvConfiguration { Delimiter = ';' };
        var result = _formatter.Format(["a", "b", "c"], config);
        Assert.Equal("a;b;c", result);
    }

    [Fact]
    public void Format_FieldContainingCustomDelimiter_IsQuoted()
    {
        var config = new CsvConfiguration { Delimiter = ';' };
        var result = _formatter.Format(["a;b", "c"], config);
        Assert.Equal("\"a;b\";c", result);
    }
}
