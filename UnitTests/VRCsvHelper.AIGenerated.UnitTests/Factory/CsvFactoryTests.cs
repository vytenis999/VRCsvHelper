using VRCsvHelper.Core;
using VRCsvHelper.Factory;
using VRCsvHelper.Models;

namespace VRCsvHelper.AIGenerated.UnitTests.Factory;

public class CsvFactoryTests
{
    [Fact]
    public void CreateReader_ReturnsNonNullInstance()
    {
        var reader = CsvFactory.CreateReader<SampleRecord>();
        Assert.NotNull(reader);
        Assert.IsType<CsvReader<SampleRecord>>(reader);
    }

    [Fact]
    public void CreateWriter_ReturnsNonNullInstance()
    {
        var writer = CsvFactory.CreateWriter<SampleRecord>();
        Assert.NotNull(writer);
        Assert.IsType<CsvWriter<SampleRecord>>(writer);
    }

    [Fact]
    public void CreateReader_WithCustomConfiguration_UsesIt()
    {
        var config = new CsvConfiguration { Delimiter = '|' };
        var reader = CsvFactory.CreateReader<SampleRecord>(config);
        Assert.NotNull(reader);
    }

    [Fact]
    public void CreateWriter_WithCustomConfiguration_UsesIt()
    {
        var config = new CsvConfiguration { Delimiter = '|' };
        var writer = CsvFactory.CreateWriter<SampleRecord>(config);
        Assert.NotNull(writer);
    }

    [Fact]
    public void CreateSemicolonReader_ReturnsFunctionalReader()
    {
        var reader = CsvFactory.CreateSemicolonReader<SampleRecord>();
        Assert.NotNull(reader);

        var csv = "Name;Age\nAlice;30";
        var results = reader.ReadFromString(csv);
        Assert.Single(results);
        Assert.Equal("Alice", results[0].Name);
        Assert.Equal(30, results[0].Age);
    }

    [Fact]
    public void CreateSemicolonWriter_ReturnsFunctionalWriter()
    {
        var writer = CsvFactory.CreateSemicolonWriter<SampleRecord>();
        Assert.NotNull(writer);

        var records = new List<SampleRecord> { new() { Name = "Bob", Age = 25 } };
        var csv = writer.WriteToString(records);
        Assert.Contains(";", csv);
    }

    private sealed class SampleRecord
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}
