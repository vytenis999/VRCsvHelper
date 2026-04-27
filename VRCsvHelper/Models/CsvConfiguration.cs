using System.Text;

namespace VRCsvHelper.Models;

public class CsvConfiguration
{
    public char Delimiter { get; set; } = ',';

    public bool HasHeaders { get; set; } = true;

    public Encoding Encoding { get; set; } = Encoding.UTF8;
}