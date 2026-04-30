using VRCsvHelper.Core;
using VRCsvHelper.Exceptions;
using VRCsvHelper.Models;

namespace VRCsvHelper.AIGenerated.UnitTests.Core;

public class CsvReaderTests
{
    // ── model types ───────────────────────────────────────────────────────────

    private sealed class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string? City { get; set; }
    }

    private sealed class PersonWithNullable
    {
        public string Name { get; set; } = string.Empty;
        public int? Score { get; set; }
    }

    // ── ReadFromString with headers ───────────────────────────────────────────

    [Fact]
    public void ReadFromString_WithHeaders_ParsesCorrectly()
    {
        const string csv = "Name,Age,City\nAlice,30,London";
        var results = new CsvReader<Person>().ReadFromString(csv);

        Assert.Single(results);
        Assert.Equal("Alice", results[0].Name);
        Assert.Equal(30, results[0].Age);
        Assert.Equal("London", results[0].City);
    }

    [Fact]
    public void ReadFromString_HeadersAreCaseInsensitive()
    {
        const string csv = "name,AGE,CITY\nBob,25,Paris";
        var results = new CsvReader<Person>().ReadFromString(csv);

        Assert.Single(results);
        Assert.Equal("Bob", results[0].Name);
        Assert.Equal(25, results[0].Age);
    }

    [Fact]
    public void ReadFromString_ExtraHeaderColumn_IsIgnored()
    {
        const string csv = "Name,Age,City,Unknown\nAlice,30,London,extra";
        var results = new CsvReader<Person>().ReadFromString(csv);

        Assert.Single(results);
        Assert.Equal("Alice", results[0].Name);
    }

    [Fact]
    public void ReadFromString_MissingColumn_LeavesPropertyDefault()
    {
        const string csv = "Name,Age\nAlice,30";
        var results = new CsvReader<Person>().ReadFromString(csv);

        Assert.Single(results);
        Assert.Null(results[0].City);
    }

    [Fact]
    public void ReadFromString_EmptyLines_AreSkipped()
    {
        const string csv = "Name,Age,City\n\nAlice,30,London\n\n";
        var results = new CsvReader<Person>().ReadFromString(csv);

        Assert.Single(results);
    }

    [Fact]
    public void ReadFromString_MultipleRecords_ParsesAll()
    {
        const string csv = "Name,Age,City\nAlice,30,London\nBob,25,Paris";
        var results = new CsvReader<Person>().ReadFromString(csv);

        Assert.Equal(2, results.Count);
        Assert.Equal("Bob", results[1].Name);
    }

    // ── ReadFromString without headers ────────────────────────────────────────

    [Fact]
    public void ReadFromString_NoHeaders_MapsByColumnOrder()
    {
        var config = new CsvConfiguration { HasHeaders = false };
        const string csv = "Alice,30,London";
        var results = new CsvReader<Person>(config).ReadFromString(csv);

        Assert.Single(results);
        Assert.Equal("Alice", results[0].Name);
        Assert.Equal(30, results[0].Age);
        Assert.Equal("London", results[0].City);
    }

    [Fact]
    public void ReadFromString_NoHeaders_FewerColumnsThanProperties_MapsAvailableColumns()
    {
        var config = new CsvConfiguration { HasHeaders = false };
        const string csv = "Alice,30";
        var results = new CsvReader<Person>(config).ReadFromString(csv);

        Assert.Single(results);
        Assert.Equal("Alice", results[0].Name);
        Assert.Equal(30, results[0].Age);
        Assert.Null(results[0].City);
    }

    // ── nullable properties ───────────────────────────────────────────────────

    [Fact]
    public void ReadFromString_EmptyValueForNullableProperty_SetsNull()
    {
        const string csv = "Name,Score\nAlice,";
        var results = new CsvReader<PersonWithNullable>().ReadFromString(csv);

        Assert.Single(results);
        Assert.Null(results[0].Score);
    }

    [Fact]
    public void ReadFromString_ValueForNullableProperty_SetsValue()
    {
        const string csv = "Name,Score\nAlice,99";
        var results = new CsvReader<PersonWithNullable>().ReadFromString(csv);

        Assert.Single(results);
        Assert.Equal(99, results[0].Score);
    }

    // ── type conversion error ─────────────────────────────────────────────────

    [Fact]
    public void ReadFromString_InvalidTypeConversion_ThrowsCsvParseException()
    {
        const string csv = "Name,Age,City\nAlice,notAnInt,London";
        var ex = Assert.Throws<CsvParseException>(() =>
            new CsvReader<Person>().ReadFromString(csv));

        Assert.Equal(2, ex.LineNumber);
    }

    // ── ReadFromStream ────────────────────────────────────────────────────────

    [Fact]
    public void ReadFromStream_ParsesCorrectly()
    {
        const string csv = "Name,Age,City\nCarol,22,Rome";
        using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));
        var results = new CsvReader<Person>().ReadFromStream(ms);

        Assert.Single(results);
        Assert.Equal("Carol", results[0].Name);
    }

    // ── ReadFromFile ──────────────────────────────────────────────────────────

    [Fact]
    public void ReadFromFile_ValidFile_ParsesCorrectly()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "Name,Age,City\nDave,28,Oslo");
            var results = new CsvReader<Person>().ReadFromFile(path);

            Assert.Single(results);
            Assert.Equal("Dave", results[0].Name);
            Assert.Equal(28, results[0].Age);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void ReadFromFile_MissingFile_ThrowsFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() =>
            new CsvReader<Person>().ReadFromFile(@"C:\does\not\exist.csv"));
    }

    // ── quoted fields round-trip ──────────────────────────────────────────────

    [Fact]
    public void ReadFromString_QuotedFieldContainingComma_ParsesCorrectly()
    {
        const string csv = "Name,Age,City\n\"Smith, Jr.\",40,NY";
        var results = new CsvReader<Person>().ReadFromString(csv);

        Assert.Single(results);
        Assert.Equal("Smith, Jr.", results[0].Name);
    }

    // ── header column with no matching property ───────────────────────────────

    [Fact]
    public void ReadFromString_UnknownHeader_IsSkipped()
    {
        const string csv = "Unknown,Name,Age,City\nX,Alice,30,London";
        var results = new CsvReader<Person>().ReadFromString(csv);

        Assert.Single(results);
        Assert.Equal("Alice", results[0].Name);
    }
}
