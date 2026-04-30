using FluentAssertions;
using VRCsvHelper.Formatting;
using VRCsvHelper.Models;

namespace VRCsvHelper.UnitTests.Formatting;

public class DefaultLineFormatterTests
{
    private readonly DefaultLineFormatter _sut = new();
    private readonly CsvConfiguration _config = new();

    [Fact]
    public void When_Format_Should_ReturnCsvLine()
    {
        // Arrange
        var fields = new[] { "Alice", "30", "Engineer" };

        // Act
        var result = _sut.Format(fields, _config);

        // Assert
        result.Should().Be("Alice,30,Engineer");
    }

    [Fact]
    public void When_Format_With_DelimiterInValue_Should_QuoteField()
    {
        // Arrange
        var fields = new[] { "Alice, Bob", "30" };

        // Act
        var result = _sut.Format(fields, _config);

        // Assert
        result.Should().Be("\"Alice, Bob\",30");
    }

    [Fact]
    public void When_Format_With_QuoteInValue_Should_EscapeAndQuoteField()
    {
        // Arrange
        var fields = new[] { "She said \"hello\"", "30" };

        // Act
        var result = _sut.Format(fields, _config);

        // Assert
        result.Should().Be("\"She said \"\"hello\"\"\",30");
    }

    [Fact]
    public void When_Format_With_NewlineInValue_Should_QuoteField()
    {
        // Arrange
        var fields = new[] { "line1\nline2", "30" };

        // Act
        var result = _sut.Format(fields, _config);

        // Assert
        result.Should().Be("\"line1\nline2\",30");
    }

    [Fact]
    public void When_Format_With_CarriageReturnInValue_Should_QuoteField()
    {
        // Arrange
        var fields = new[] { "line1\rline2", "30" };

        // Act
        var result = _sut.Format(fields, _config);

        // Assert
        result.Should().Be("\"line1\rline2\",30");
    }

    [Fact]
    public void When_Format_With_CustomDelimiter_Should_UseThatDelimiter()
    {
        // Arrange
        var config = new CsvConfiguration { Delimiter = ';' };
        var fields = new[] { "Alice", "30" };

        // Act
        var result = _sut.Format(fields, config);

        // Assert
        result.Should().Be("Alice;30");
    }
}