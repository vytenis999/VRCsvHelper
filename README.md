# VRCsvHelper

A simple, reusable CSV import/export library for .NET.

VRCsvHelper lets you read and write CSV data using strongly-typed objects, with support for custom delimiters, headers, and encodings.

---

## NuGet Setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- Any IDE that supports .NET (Visual Studio 2026, Rider, VS Code, etc.)

### Installation

Install the package into your project using one of the following methods.

**.NET CLI**

```bash
dotnet add package VRCsvHelper
```

**Package Manager Console**

```powershell
Install-Package VRCsvHelper
```

**PackageReference (csproj)**

```xml
<ItemGroup>
  <PackageReference Include="VRCsvHelper" Version="1.0.0" />
</ItemGroup>
```

### Configuration

Add the relevant `using` directives to your file:

```csharp
using VRCsvHelper.Core;
using VRCsvHelper.Factory;
using VRCsvHelper.Models;
```

Optionally configure CSV behavior using `CsvConfiguration`:

| Property     | Type       | Default        | Description                          |
|--------------|------------|----------------|--------------------------------------|
| `Delimiter`  | `char`     | `,`            | Field separator character.           |
| `HasHeaders` | `bool`     | `true`         | Whether the first row is a header.   |
| `Encoding`   | `Encoding` | `Encoding.UTF8`| Encoding used when reading/writing.  |

---

## Usage

### 1. Define a model

Each public property is mapped to a CSV column by name (when headers are enabled) or by order.

```csharp
public class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
}
```

### 2. Create a reader or writer

Use `CsvFactory` for the most common scenarios:

```csharp
var reader = CsvFactory.CreateReader<Person>();
var writer = CsvFactory.CreateWriter<Person>();

// Semicolon-delimited helpers
var semicolonReader = CsvFactory.CreateSemicolonReader<Person>();
var semicolonWriter = CsvFactory.CreateSemicolonWriter<Person>();
```

Or instantiate directly with custom configuration:

```csharp
var config = new CsvConfiguration
{
    Delimiter = '\t',
    HasHeaders = true,
    Encoding = System.Text.Encoding.UTF8
};

var tabWriter = new CsvWriter<Person>(config);
var tabReader = new CsvReader<Person>(config);
```

### 3. Read CSV data

```csharp
IReadOnlyList<Person> fromFile   = reader.ReadFromFile("people.csv");
IReadOnlyList<Person> fromString = reader.ReadFromString(csvText);
IReadOnlyList<Person> fromStream = reader.ReadFromStream(stream);
```

### 4. Write CSV data

```csharp
writer.WriteToFile(people, "people.csv");
string csv = writer.WriteToString(people);
writer.WriteToStream(people, stream);
```

---

## Examples

### Example 1 — Export to a CSV string

```csharp
var people = new List<Person>
{
    new() { Name = "Alice",       Age = 30, Email = "alice@example.com" },
    new() { Name = "Bob",         Age = 25, Email = "bob@example.com" },
    new() { Name = "Smith, John", Age = 40, Email = "john@example.com" },
};

var writer = CsvFactory.CreateWriter<Person>();
Console.WriteLine(writer.WriteToString(people));
```

**Expected output:**

```
Name,Age,Email
Alice,30,alice@example.com
Bob,25,bob@example.com
"Smith, John",40,john@example.com
```

> Values containing the delimiter are automatically quoted.

### Example 2 — Write to a file and read it back

```csharp
var tempFile = Path.Combine(Path.GetTempPath(), "people.csv");

var writer = CsvFactory.CreateWriter<Person>();
writer.WriteToFile(people, tempFile);

var reader   = CsvFactory.CreateReader<Person>();
var imported = reader.ReadFromFile(tempFile);

foreach (var p in imported)
    Console.WriteLine($"{p.Name} | {p.Age} | {p.Email}");
```

**Expected output:**

```
Alice | 30 | alice@example.com
Bob | 25 | bob@example.com
Smith, John | 40 | john@example.com
```

### Example 3 — Semicolon-delimited CSV

```csharp
var writer = CsvFactory.CreateSemicolonWriter<Person>();
Console.WriteLine(writer.WriteToString(people));
```

**Expected output:**

```
Name;Age;Email
Alice;30;alice@example.com
Bob;25;bob@example.com
"Smith, John";40;john@example.com
```

### Example 4 — Tab-delimited with custom configuration

```csharp
var config = new CsvConfiguration
{
    Delimiter  = '\t',
    HasHeaders = true,
    Encoding   = System.Text.Encoding.UTF8
};

var tabWriter = new CsvWriter<Person>(config);
Console.WriteLine(tabWriter.WriteToString(people));
```

**Expected output (tab-separated):**

```
Name    Age    Email
Alice   30     alice@example.com
Bob     25     bob@example.com
Smith, John   40   john@example.com
```

### Example 5 — Reading without headers

If your CSV has no header row, set `HasHeaders = false`. Columns are then mapped by property declaration order.

```csharp
var config = new CsvConfiguration { HasHeaders = false };
var reader = CsvFactory.CreateReader<Person>(config);

var csv = "Alice,30,alice@example.com\nBob,25,bob@example.com";
var people = reader.ReadFromString(csv);
```

---

## Error Handling

- `CsvParseException` — thrown when a row cannot be converted to type `T`. Includes the offending line number.
- `CsvWriteException` — thrown when writing fails (e.g., I/O error or no readable properties).

```csharp
try
{
    var people = reader.ReadFromFile("people.csv");
}
catch (CsvParseException ex)
{
    Console.WriteLine($"Parse error on line {ex.LineNumber}: {ex.Message}");
}
```

---

## License

Released under the [MIT License](https://opensource.org/licenses/MIT).