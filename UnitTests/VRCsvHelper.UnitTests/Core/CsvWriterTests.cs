using FluentAssertions;
using Moq;
using System.Text;
using VRCsvHelper.Core;
using VRCsvHelper.Exceptions;
using VRCsvHelper.Formatting;
using VRCsvHelper.Models;

namespace VRCsvHelper.UnitTests.Core;

public class CsvWriterTests
{
    private class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public string Role { get; set; } = "";
    }

    private class NoReadableProps
    {
        private string Name { get; set; } = "";
    }

    [Fact]
    public void When_WriteToString_Should_ReturnCsvContent()
    {
        // Arrange
        var records = new List<Person> { new() { Name = "Alice", Age = 30, Role = "Engineer" } };
        var sut = new CsvWriter<Person>();

        // Act
        var result = sut.WriteToString(records);

        // Assert
        result.Should().Contain("Name,Age,Role");
        result.Should().Contain("Alice,30,Engineer");
    }

    [Fact]
    public void When_WriteToString_With_NoHeaders_Should_OmitHeaderLine()
    {
        // Arrange
        var records = new List<Person> { new() { Name = "Alice", Age = 30, Role = "Engineer" } };
        var config = new CsvConfiguration { HasHeaders = false };
        var sut = new CsvWriter<Person>(config);

        // Act
        var result = sut.WriteToString(records);

        // Assert
        result.Should().NotContain("Name,Age,Role");
        result.Should().Contain("Alice,30,Engineer");
    }

    [Fact]
    public void When_WriteToString_With_NullRecord_Should_SkipIt()
    {
        // Arrange
        var records = new List<Person?> { null };
        var sut = new CsvWriter<Person?>();

        // Act
        var result = sut.WriteToString(records!);

        // Assert
        result.Trim().Should().Be("Name,Age,Role");
    }

    [Fact]
    public void When_WriteToString_With_NoReadableProperties_Should_ThrowCsvWriteException()
    {
        // Arrange
        var records = new List<NoReadableProps> { new() };
        var sut = new CsvWriter<NoReadableProps>();

        // Act
        var act = () => sut.WriteToString(records);

        // Assert
        act.Should().Throw<CsvWriteException>();
    }

    [Fact]
    public void When_WriteToString_With_CustomFormatter_Should_UseIt()
    {
        // Arrange
        var mockFormatter = new Mock<ILineFormatter>();
        mockFormatter.Setup(f => f.Format(It.IsAny<IEnumerable<string>>(), It.IsAny<CsvConfiguration>()))
                     .Returns("mocked");

        var records = new List<Person> { new() { Name = "Alice", Age = 30, Role = "Engineer" } };
        var sut = new CsvWriter<Person>(null, mockFormatter.Object);

        // Act
        var result = sut.WriteToString(records);

        // Assert
        mockFormatter.Verify(f => f.Format(It.IsAny<IEnumerable<string>>(), It.IsAny<CsvConfiguration>()), Times.Exactly(2));
        result.Should().Contain("mocked");
    }

    [Fact]
    public void When_WriteToStream_Should_WriteContent()
    {
        // Arrange
        var records = new List<Person> { new() { Name = "Alice", Age = 30, Role = "Engineer" } };
        var sut = new CsvWriter<Person>();
        using var stream = new MemoryStream();

        // Act
        sut.WriteToStream(records, stream);
        var content = Encoding.UTF8.GetString(stream.ToArray());

        // Assert
        content.Should().Contain("Alice,30,Engineer");
    }

    [Fact]
    public void When_WriteToFile_Should_CreateFileWithContent()
    {
        // Arrange
        var records = new List<Person> { new() { Name = "Alice", Age = 30, Role = "Engineer" } };
        var filePath = Path.GetTempFileName();
        var sut = new CsvWriter<Person>();

        try
        {
            // Act
            sut.WriteToFile(records, filePath);
            var content = File.ReadAllText(filePath);

            // Assert
            content.Should().Contain("Alice,30,Engineer");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void When_WriteToFile_With_InvalidPath_Should_ThrowCsvWriteException()
    {
        // Arrange
        var records = new List<Person> { new() { Name = "Alice", Age = 30, Role = "Engineer" } };
        var sut = new CsvWriter<Person>();

        // Act
        var act = () => sut.WriteToFile(records, "Z:\\invalid\\path\\file.csv");

        // Assert
        act.Should().Throw<CsvWriteException>();
    }

    [Fact]
    public void When_WriteToFile_With_NoReadableProperties_Should_RethrowCsvWriteException()
    {
        // Arrange
        var records = new List<NoReadableProps> { new() };
        var filePath = Path.GetTempFileName();
        var sut = new CsvWriter<NoReadableProps>();

        try
        {
            // Act
            var act = () => sut.WriteToFile(records, filePath);

            // Assert
            act.Should().Throw<CsvWriteException>()
               .Which.Message.Should().Contain("no readable public properties");
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}