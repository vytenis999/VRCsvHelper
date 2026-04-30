using System.Text;
using VRCsvHelper.Models;

namespace VRCsvHelper.AIGenerated.UnitTests.Models;

public class CsvConfigurationTests
{
    [Fact]
    public void DefaultDelimiter_IsComma()
    {
        var config = new CsvConfiguration();
        Assert.Equal(',', config.Delimiter);
    }

    [Fact]
    public void DefaultHasHeaders_IsTrue()
    {
        var config = new CsvConfiguration();
        Assert.True(config.HasHeaders);
    }

    [Fact]
    public void DefaultEncoding_IsUtf8()
    {
        var config = new CsvConfiguration();
        Assert.Equal(Encoding.UTF8, config.Encoding);
    }

    [Fact]
    public void Properties_CanBeOverridden()
    {
        var config = new CsvConfiguration
        {
            Delimiter = ';',
            HasHeaders = false,
            Encoding = Encoding.ASCII
        };

        Assert.Equal(';', config.Delimiter);
        Assert.False(config.HasHeaders);
        Assert.Equal(Encoding.ASCII, config.Encoding);
    }
}
