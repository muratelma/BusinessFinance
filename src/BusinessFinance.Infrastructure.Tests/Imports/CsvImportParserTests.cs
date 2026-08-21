using System.Text;
using BusinessFinance.Application.Imports;
using BusinessFinance.Infrastructure.Imports;

namespace BusinessFinance.Infrastructure.Tests.Imports;

public sealed class CsvImportParserTests
{
    [Fact]
    public async Task Parse_FingerprintUsesExactRawBytes()
    {
        var options = new CsvParsingOptions("utf-8", "comma", "yyyy-MM-dd", '.',
            new CsvColumnMapping("Date", "Amount", null, null));
        var first = await ParseAsync(Encoding.UTF8.GetBytes("Date,Amount\n2026-08-11,10"), options);
        var retry = await ParseAsync(Encoding.UTF8.GetBytes("Date,Amount\n2026-08-11,10"), options);
        var changedNewline = await ParseAsync(Encoding.UTF8.GetBytes("Date,Amount\r\n2026-08-11,10"), options);

        Assert.Equal(64, first.FileFingerprint.Length);
        Assert.Equal(first.FileFingerprint, retry.FileFingerprint);
        Assert.NotEqual(first.FileFingerprint, changedNewline.FileFingerprint);
    }

    [Fact]
    public async Task Parse_Utf8BomSemicolonAndQuotedDelimiter_ReturnsRowsAndErrors()
    {
        var csv = "\uFEFFTarih;Tutar;Açıklama\r\n" +
                  "2026-08-10;-125,5000;\"Market; haftalık\"\r\n" +
                  "not-a-date;0;Bozuk";
        var result = await ParseAsync(
            new UTF8Encoding(true).GetBytes(csv),
            new CsvParsingOptions("auto", "auto", "yyyy-MM-dd", ',',
                new CsvColumnMapping("Tarih", "Tutar", "Açıklama", null)));

        Assert.Equal("utf-8", result.EncodingName);
        Assert.Equal(';', result.Delimiter);
        Assert.Equal(2, result.Rows.Count);
        Assert.Equal(-125.5000m, result.Rows[0].SignedAmount);
        Assert.Equal("Market; haftalık", result.Rows[0].Description);
        Assert.Empty(result.Rows[0].Errors);
        Assert.Equal(2, result.Rows[1].Errors.Count);
    }

    [Fact]
    public async Task Parse_Windows1254TabAndTurkishText_DecodesExplicitly()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var encoding = Encoding.GetEncoding(1254);
        var bytes = encoding.GetBytes("Tarih\tTutar\tAçıklama\n11.08.2026\t25.2500\tÇay ödemesi");

        var result = await ParseAsync(bytes,
            new CsvParsingOptions("windows-1254", "tab", "dd.MM.yyyy", '.',
                new CsvColumnMapping("Tarih", "Tutar", "Açıklama", null)));

        Assert.Equal("windows-1254", result.EncodingName);
        Assert.Equal('\t', result.Delimiter);
        Assert.Equal("Çay ödemesi", Assert.Single(result.Rows).Description);
    }

    [Fact]
    public async Task Parse_MissingMappedColumn_RejectsWholeBatch()
    {
        var exception = await Assert.ThrowsAsync<CsvImportParsingException>(() => ParseAsync(
            Encoding.UTF8.GetBytes("Date,Value\n2026-08-11,10"),
            new CsvParsingOptions("utf-8", "comma", "yyyy-MM-dd", '.',
                new CsvColumnMapping("Date", "Amount", null, null))));

        Assert.Contains("Amount", exception.Message);
    }

    [Fact]
    public async Task Parse_InvalidUtf8_RejectsWithoutReplacementCharacters()
    {
        await Assert.ThrowsAsync<CsvImportParsingException>(() => ParseAsync(
            [0xFF, 0xFE, 0xFD],
            new CsvParsingOptions("utf-8", "comma", "yyyy-MM-dd", '.',
                new CsvColumnMapping("Date", "Amount", null, null))));
    }

    [Theory]
    [InlineData("2026-08-11,12,1\"2\"")]
    [InlineData("2026-08-11,12,\"foo\"bar")]
    public async Task Parse_QuoteOutsideStrictCsvPositions_MarksRowInvalid(string dataRow)
    {
        var result = await ParseAsync(
            Encoding.UTF8.GetBytes($"Date,Amount,Description\n{dataRow}"),
            new CsvParsingOptions("utf-8", "comma", "yyyy-MM-dd", '.',
                new CsvColumnMapping("Date", "Amount", "Description", null)));

        Assert.Contains("quote", Assert.Single(result.Rows).Errors.Single(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("999999999999999.9999", true)]
    [InlineData("-999999999999999.9999", true)]
    [InlineData("1000000000000000.0000", false)]
    public async Task Parse_EnforcesSqlDecimal19Scale4Magnitude(string amount, bool valid)
    {
        var result = await ParseAsync(
            Encoding.UTF8.GetBytes($"Date,Amount\n2026-08-11,{amount}"),
            new CsvParsingOptions("utf-8", "comma", "yyyy-MM-dd", '.',
                new CsvColumnMapping("Date", "Amount", null, null)));

        Assert.Equal(valid, Assert.Single(result.Rows).Errors.Count == 0);
    }

    private static Task<ParsedCsvFile> ParseAsync(byte[] bytes, CsvParsingOptions options) =>
        new CsvImportParser().ParseAsync(new MemoryStream(bytes), options, CancellationToken.None);
}
