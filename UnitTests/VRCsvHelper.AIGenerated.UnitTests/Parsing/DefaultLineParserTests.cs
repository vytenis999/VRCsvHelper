using VRCsvHelper.Models;
using VRCsvHelper.Parsing;

namespace VRCsvHelper.AIGenerated.UnitTests.Parsing;

public class DefaultLineParserTests
{
    private readonly DefaultLineParser _parser = new();
    private readonly CsvConfiguration _config = new();

    [Fact]
    public void Parse_SimpleFields_ReturnsSplitValues()
    {
        var result = _parser.Parse("Alice,30,Engineer", _config);
        Assert.Equal(["Alice", "30", "Engineer"], result);
    }

    [Fact]
    public void Parse_QuotedFieldContainingDelimiter_ReturnsFieldWithDelimiter()
    {
        var result = _parser.Parse("\"New York, NY\",10001", _config);
        Assert.Equal(2, result.Count);
        Assert.Equal("New York, NY", result[0]);
        Assert.Equal("10001", result[1]);
    }

    [Fact]
    public void Parse_EscapedQuoteInsideQuotedField_ReturnsUnescapedQuote()
    {
        var result = _parser.Parse("\"Say \"\"hello\"\"\"", _config);
        Assert.Equal(1, result.Count);
        Assert.Equal("Say \"hello\"", result[0]);
    }

    [Fact]
    public void Parse_EmptyField_ReturnsEmptyString()
    {
        var result = _parser.Parse(",value", _config);
        Assert.Equal(2, result.Count);
        Assert.Equal("", result[0]);
        Assert.Equal("value", result[1]);
    }

    [Fact]
    public void Parse_TrailingDelimiter_ReturnsEmptyLastField()
    {
        var result = _parser.Parse("a,b,", _config);
        Assert.Equal(3, result.Count);
        Assert.Equal("", result[2]);
    }

    [Fact]
    public void Parse_SingleField_ReturnsSingleElement()
    {
        var result = _parser.Parse("hello", _config);
        Assert.Single(result);
        Assert.Equal("hello", result[0]);
    }

    [Fact]
    public void Parse_QuotedFieldAtEnd_ReturnsCorrectValue()
    {
        var result = _parser.Parse("a,\"b,c\"", _config);
        Assert.Equal(2, result.Count);
        Assert.Equal("b,c", result[1]);
    }

    [Fact]
    public void Parse_CustomDelimiter_SplitsOnSemicolon()
    {
        var config = new CsvConfiguration { Delimiter = ';' };
        var result = _parser.Parse("a;b;c", config);
        Assert.Equal(["a", "b", "c"], result);
    }

    [Fact]
    public void Parse_QuotedFieldWithTerminatingQuoteAtEndOfLine_ReturnsField()
    {
        // Closing quote is at the very last character — tests the i+1 >= line.Length branch
        var result = _parser.Parse("\"hello\"", _config);
        Assert.Single(result);
        Assert.Equal("hello", result[0]);
    }

    [Fact]
    public void Parse_MultipleConsecutiveDelimiters_ReturnsEmptyFields()
    {
        var result = _parser.Parse("a,,c", _config);
        Assert.Equal(3, result.Count);
        Assert.Equal("", result[1]);
    }
}
