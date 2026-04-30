using FluentAssertions;
using VRCsvHelper.Models;
using VRCsvHelper.Parsing;

namespace VRCsvHelper.UnitTests.Parsing;

public class DefaultLineParserTests
{
    private readonly DefaultLineParser _sut = new();
    private readonly CsvConfiguration _config = new();

    [Fact]
    public void When_Parse_Should_ReturnFields()
    {
        // Arrange
        var line = "Alice,30,Engineer";

        // Act
        var result = _sut.Parse(line, _config);

        // Assert
        result.Should().Equal("Alice", "30", "Engineer");
    }

    [Fact]
    public void When_Parse_With_QuotedField_Should_ReturnUnquotedValue()
    {
        // Arrange
        var line = "\"Alice, Bob\",30,Engineer";

        // Act
        var result = _sut.Parse(line, _config);

        // Assert
        result.Should().Equal("Alice, Bob", "30", "Engineer");
    }

    [Fact]
    public void When_Parse_With_EscapedQuoteInQuotedField_Should_ReturnQuoteInValue()
    {
        // Arrange
        var line = "\"She said \"\"hello\"\"\",30";

        // Act
        var result = _sut.Parse(line, _config);

        // Assert
        result.Should().Equal("She said \"hello\"", "30");
    }

    [Fact]
    public void When_Parse_With_EmptyField_Should_ReturnEmptyString()
    {
        // Arrange
        var line = "Alice,,Engineer";

        // Act
        var result = _sut.Parse(line, _config);

        // Assert
        result.Should().Equal("Alice", "", "Engineer");
    }

    [Fact]
    public void When_Parse_With_CustomDelimiter_Should_SplitCorrectly()
    {
        // Arrange
        var config = new CsvConfiguration { Delimiter = ';' };
        var line = "Alice;30;Engineer";

        // Act
        var result = _sut.Parse(line, config);

        // Assert
        result.Should().Equal("Alice", "30", "Engineer");
    }

    [Fact]
    public void When_Parse_With_SingleField_Should_ReturnOneElement()
    {
        // Arrange
        var line = "OnlyField";

        // Act
        var result = _sut.Parse(line, _config);

        // Assert
        result.Should().ContainSingle().Which.Should().Be("OnlyField");
    }

    [Fact]
    public void When_Parse_With_QuotedFieldAtEnd_Should_ReturnCorrectValue()
    {
        // Arrange
        var line = "Alice,\"Engineer, Senior\"";

        // Act
        var result = _sut.Parse(line, _config);

        // Assert
        result.Should().Equal("Alice", "Engineer, Senior");
    }
}