using VRCsvHelper.Core;
using VRCsvHelper.Example;
using VRCsvHelper.Factory;
using VRCsvHelper.Models;

// ---------------------------------------------------------------------------
// 1. Export a list of Person objects to a CSV string
// ---------------------------------------------------------------------------
var people = new List<Person>
{
    new() { Name = "Alice", Age = 30, Email = "alice@example.com" },
    new() { Name = "Bob",   Age = 25, Email = "bob@example.com" },
    new() { Name = "Smith, John", Age = 40, Email = "john@example.com" }, // comma in name → will be quoted
};

var writer = CsvFactory.CreateWriter<Person>();
var csvString = writer.WriteToString(people);

Console.WriteLine("=== Exported CSV ===");
Console.WriteLine(csvString);

// ---------------------------------------------------------------------------
// 2. Write that CSV to a temp file, then read it back
// ---------------------------------------------------------------------------
var tempFile = Path.Combine(Path.GetTempPath(), "people.csv");
writer.WriteToFile(people, tempFile);
Console.WriteLine($"Written to: {tempFile}");

var reader = CsvFactory.CreateReader<Person>();
var imported = reader.ReadFromFile(tempFile);

Console.WriteLine("\n=== Imported records ===");
foreach (var p in imported)
    Console.WriteLine($"  {p.Name} | {p.Age} | {p.Email}");

// ---------------------------------------------------------------------------
// 3. Semicolon-delimited file (e.g. European locale)
// ---------------------------------------------------------------------------
var semicolonWriter = CsvFactory.CreateSemicolonWriter<Person>();
var semicolonCsv = semicolonWriter.WriteToString(people);

Console.WriteLine("\n=== Semicolon-delimited CSV ===");
Console.WriteLine(semicolonCsv);

// ---------------------------------------------------------------------------
// 4. Custom configuration example
// ---------------------------------------------------------------------------
var config = new CsvConfiguration
{
    Delimiter = '\t',
    HasHeaders = true,
    Encoding = System.Text.Encoding.UTF8
};

var tabWriter = new CsvWriter<Person>(config);
Console.WriteLine("=== Tab-delimited CSV ===");
Console.WriteLine(tabWriter.WriteToString(people));