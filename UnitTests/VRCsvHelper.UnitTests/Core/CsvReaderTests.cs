using FluentAssertions;
using Moq;
using System.Text;
using VRCsvHelper.Core;
using VRCsvHelper.Exceptions;
using VRCsvHelper.Models;
using VRCsvHelper.Parsing;

namespace VRCsvHelper.UnitTests.Core;

public class CsvReaderTests
{
    private class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public string Role { get; set; } = "";
    }

    private class NullablePerson
    {
        public string Name { get; set; } = "";
        public int? Age { get; set; }
    }

    [Fact]
    public void When_ReadFromString_Should_ReturnMappedObjects()
    {
        // Arrange
        var csv = "Name,Age,Role\nAlice,30,Engineer\nBob,25,Designer";
        var sut = new CsvReader<Person>();

        // Act
        var result = sut.ReadFromString(csv);

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Alice");
        result[0].Age.Should().Be(30);
        result[0].Role.Should().Be("Engineer");
        result[1].Name.Should().Be("Bob");
    }

    [Fact]
    public void When_ReadFromString_With_NoHeaders_Should_MapByOrder()
    {
        // Arrange
        var csv = "Alice,30,Engineer";
        var config = new CsvConfiguration { HasHeaders = false };
        var sut = new CsvReader<Person>(config);

        // Act
        var result = sut.ReadFromString(csv);

        // Assert
        result.Should().ContainSingle();
        result[0].Name.Should().Be("Alice");
        result[0].Age.Should().Be(30);
        result[0].Role.Should().Be("Engineer");
    }

    [Fact]
    public void When_ReadFromString_With_EmptyLines_Should_SkipThem()
    {
        // Arrange
        var csv = "Name,Age,Role\n\nAlice,30,Engineer\n";
        var sut = new CsvReader<Person>();

        // Act
        var result = sut.ReadFromString(csv);

        // Assert
        result.Should().ContainSingle();
    }

    [Fact]
    public void When_ReadFromString_With_UnknownHeader_Should_IgnoreColumn()
    {
        // Arrange
        var csv = "Name,Age,Unknown\nAlice,30,X";
        var sut = new CsvReader<Person>();

        // Act
        var result = sut.ReadFromString(csv);

        // Assert
        result.Should().ContainSingle();
        result[0].Name.Should().Be("Alice");
        result[0].Age.Should().Be(30);
    }

    [Fact]
    public void When_ReadFromString_With_NullableProperty_Should_SetNullForEmpty()
    {
        // Arrange
        var csv = "Name,Age\nAlice,";
        var sut = new CsvReader<NullablePerson>();

        // Act
        var result = sut.ReadFromString(csv);

        // Assert
        result.Should().ContainSingle();
        result[0].Age.Should().BeNull();
    }

    [Fact]
    public void When_ReadFromString_With_InvalidConversion_Should_ThrowCsvParseException()
    {
        // Arrange
        var csv = "Name,Age\nAlice,NotANumber";
        var sut = new CsvReader<Person>();

        // Act
        var act = () => sut.ReadFromString(csv);

        // Assert
        act.Should().Throw<CsvParseException>().Which.LineNumber.Should().Be(2);
    }

    [Fact]
    public void When_ReadFromString_With_CustomParser_Should_UseIt()
    {
        // Arrange
        var mockParser = new Mock<ILineParser>();
        mockParser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<CsvConfiguration>()))
                  .Returns((string line, CsvConfiguration _) => line.Split(','));

        var csv = "Name,Age,Role\nAlice,30,Engineer";
        var sut = new CsvReader<Person>(null, mockParser.Object);

        // Act
        var result = sut.ReadFromString(csv);

        // Assert
        mockParser.Verify(p => p.Parse(It.IsAny<string>(), It.IsAny<CsvConfiguration>()), Times.Exactly(2));
        result.Should().ContainSingle();
    }

    [Fact]
    public void When_ReadFromStream_Should_ReturnMappedObjects()
    {
        // Arrange
        var csv = "Name,Age,Role\nAlice,30,Engineer";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var sut = new CsvReader<Person>();

        // Act
        var result = sut.ReadFromStream(stream);

        // Assert
        result.Should().ContainSingle();
        result[0].Name.Should().Be("Alice");
    }

    [Fact]
    public void When_ReadFromFile_With_MissingFile_Should_ThrowFileNotFoundException()
    {
        // Arrange
        var sut = new CsvReader<Person>();

        // Act
        var act = () => sut.ReadFromFile("nonexistent.csv");

        // Assert
        act.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void When_ReadFromFile_Should_ReturnMappedObjects()
    {
        // Arrange
        var csv = "Name,Age,Role\nAlice,30,Engineer";
        var filePath = Path.GetTempFileName();
        File.WriteAllText(filePath, csv);
        var sut = new CsvReader<Person>();

        try
        {
            // Act
            var result = sut.ReadFromFile(filePath);

            // Assert
            result.Should().ContainSingle();
            result[0].Name.Should().Be("Alice");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void When_ReadFromString_With_ParserThrowingCsvParseException_Should_PropagateWithLineNumber()
    {
        // Arrange
        var mockParser = new Mock<ILineParser>();
        mockParser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<CsvConfiguration>()))
                  .Throws(new CsvParseException("bad line", 42));

        var sut = new CsvReader<Person>(null, mockParser.Object);

        // Act
        var act = () => sut.ReadFromString("Name,Age,Role");

        // Assert
        act.Should().Throw<CsvParseException>()
           .Which.LineNumber.Should().Be(42);
    }

    [Fact]
    public void When_ReadFromString_With_HeadersAndFewerColumns_Should_MapAvailableColumns()
    {
        // Arrange
        var csv = "Name,Age,Role\nAlice,30";
        var sut = new CsvReader<Person>();

        // Act
        var result = sut.ReadFromString(csv);

        // Assert
        result.Should().ContainSingle();
        result[0].Name.Should().Be("Alice");
        result[0].Age.Should().Be(30);
        result[0].Role.Should().Be("");
    }
}