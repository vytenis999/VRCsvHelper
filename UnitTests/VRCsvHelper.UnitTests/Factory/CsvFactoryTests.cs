using FluentAssertions;
using VRCsvHelper.Factory;

namespace VRCsvHelper.UnitTests.Factory;

public class CsvFactoryTests
{
    private class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    [Fact]
    public void When_CreateReader_Should_ReturnReader()
    {
        // Arrange / Act
        var reader = CsvFactory.CreateReader<Person>();

        // Assert
        reader.Should().NotBeNull();
    }

    [Fact]
    public void When_CreateWriter_Should_ReturnWriter()
    {
        // Arrange / Act
        var writer = CsvFactory.CreateWriter<Person>();

        // Assert
        writer.Should().NotBeNull();
    }

    [Fact]
    public void When_CreateSemicolonReader_Should_ReturnReaderWithSemicolonDelimiter()
    {
        // Arrange
        var csv = "Name;Age\nAlice;30";
        var reader = CsvFactory.CreateSemicolonReader<Person>();

        // Act
        var result = reader.ReadFromString(csv);

        // Assert
        result.Should().ContainSingle();
        result[0].Name.Should().Be("Alice");
        result[0].Age.Should().Be(30);
    }

    [Fact]
    public void When_CreateSemicolonWriter_Should_ReturnWriterWithSemicolonDelimiter()
    {
        // Arrange
        var records = new List<Person> { new() { Name = "Alice", Age = 30 } };
        var writer = CsvFactory.CreateSemicolonWriter<Person>();

        // Act
        var result = writer.WriteToString(records);

        // Assert
        result.Should().Contain("Alice;30");
    }

    [Fact]
    public void When_CreateReader_With_Configuration_Should_UseConfiguration()
    {
        // Arrange
        var csv = "Alice;30";
        var config = new Models.CsvConfiguration { Delimiter = ';', HasHeaders = false };
        var reader = CsvFactory.CreateReader<Person>(config);

        // Act
        var result = reader.ReadFromString(csv);

        // Assert
        result.Should().ContainSingle();
        result[0].Name.Should().Be("Alice");
    }

    [Fact]
    public void When_CreateWriter_With_Configuration_Should_UseConfiguration()
    {
        // Arrange
        var records = new List<Person> { new() { Name = "Alice", Age = 30 } };
        var config = new Models.CsvConfiguration { Delimiter = ';' };
        var writer = CsvFactory.CreateWriter<Person>(config);

        // Act
        var result = writer.WriteToString(records);

        // Assert
        result.Should().Contain("Alice;30");
    }
}