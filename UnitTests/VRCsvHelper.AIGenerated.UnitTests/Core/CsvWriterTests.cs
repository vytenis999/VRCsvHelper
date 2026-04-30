using VRCsvHelper.Core;
using VRCsvHelper.Exceptions;
using VRCsvHelper.Models;

namespace VRCsvHelper.AIGenerated.UnitTests.Core;

public class CsvWriterTests
{
    // ── helpers ──────────────────────────────────────────────────────────────

    private sealed class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string? City { get; set; }
    }

    private sealed class NoProps { }          // no readable public properties

    private static CsvWriter<T> Writer<T>(CsvConfiguration? cfg = null) => new(cfg);

    // ── WriteToString ─────────────────────────────────────────────────────────

    [Fact]
    public void WriteToString_WithHeaders_FirstLineIsHeader()
    {
        var records = new List<Person> { new() { Name = "Alice", Age = 30, City = "London" } };
        var csv = Writer<Person>().WriteToString(records);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("Name,Age,City", lines[0]);
    }

    [Fact]
    public void WriteToString_WithHeaders_DataLineIsCorrect()
    {
        var records = new List<Person> { new() { Name = "Alice", Age = 30, City = "London" } };
        var csv = Writer<Person>().WriteToString(records);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("Alice,30,London", lines[1]);
    }

    [Fact]
    public void WriteToString_NoHeaders_OnlyDataLine()
    {
        var config = new CsvConfiguration { HasHeaders = false };
        var records = new List<Person> { new() { Name = "Bob", Age = 25, City = "Paris" } };
        var csv = Writer<Person>(config).WriteToString(records);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Single(lines);
        Assert.Equal("Bob,25,Paris", lines[0]);
    }

    [Fact]
    public void WriteToString_NullRecord_IsSkipped()
    {
        var records = new List<Person?> { null, new() { Name = "Alice", Age = 30, City = "NYC" } };
        var csv = new CsvWriter<Person?>().WriteToString(records);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        // header + 1 data row (null skipped)
        Assert.Equal(2, lines.Length);
    }

    [Fact]
    public void WriteToString_NullPropertyValue_WritesEmptyString()
    {
        var records = new List<Person> { new() { Name = "Alice", Age = 30, City = null } };
        var csv = Writer<Person>().WriteToString(records);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("Alice,30,", lines[1]);
    }

    [Fact]
    public void WriteToString_EmptyCollection_OnlyHeader()
    {
        var csv = Writer<Person>().WriteToString([]);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Single(lines);
        Assert.Equal("Name,Age,City", lines[0]);
    }

    [Fact]
    public void WriteToString_TypeWithNoProperties_ThrowsCsvWriteException()
    {
        Assert.Throws<CsvWriteException>(() => Writer<NoProps>().WriteToString([]));
    }

    [Fact]
    public void WriteToString_FieldNeedingQuotes_IsQuoted()
    {
        var records = new List<Person> { new() { Name = "Smith, Jr.", Age = 40, City = "NY" } };
        var csv = Writer<Person>().WriteToString(records);

        Assert.Contains("\"Smith, Jr.\"", csv);
    }

    // ── WriteToStream ─────────────────────────────────────────────────────────

    [Fact]
    public void WriteToStream_WritesCorrectContent()
    {
        var records = new List<Person> { new() { Name = "Carol", Age = 22, City = "Rome" } };
        using var ms = new MemoryStream();
        Writer<Person>().WriteToStream(records, ms);
        ms.Position = 0;
        var content = new StreamReader(ms).ReadToEnd();

        Assert.Contains("Name,Age,City", content);
        Assert.Contains("Carol,22,Rome", content);
    }

    // ── WriteToFile ───────────────────────────────────────────────────────────

    [Fact]
    public void WriteToFile_CreatesFileWithCorrectContent()
    {
        var path = Path.GetTempFileName();
        try
        {
            var records = new List<Person> { new() { Name = "Dave", Age = 28, City = "Oslo" } };
            Writer<Person>().WriteToFile(records, path);

            var lines = File.ReadAllLines(path);
            Assert.Equal("Name,Age,City", lines[0]);
            Assert.Equal("Dave,28,Oslo", lines[1]);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void WriteToFile_InvalidPath_ThrowsCsvWriteException()
    {
        Assert.Throws<CsvWriteException>(() =>
            Writer<Person>().WriteToFile([], @"Z:\invalid\path\file.csv"));
    }
}
