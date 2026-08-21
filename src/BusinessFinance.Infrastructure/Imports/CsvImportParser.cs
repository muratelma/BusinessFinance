using System.Globalization;
using System.Text;
using System.Security.Cryptography;
using BusinessFinance.Application.Imports;
using BusinessFinance.Domain;

namespace BusinessFinance.Infrastructure.Imports;

public sealed class CsvImportParser : ICsvImportParser
{
    public const int MaximumFileSizeBytes = 2 * 1024 * 1024;
    public const int MaximumRows = 5000;
    private static readonly char[] SupportedDelimiters = [',', ';', '\t'];

    public async Task<ParsedCsvFile> ParseAsync(
        Stream content,
        CsvParsingOptions options,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(options);

        var bytes = await ReadBoundedAsync(content, cancellationToken);
        var (text, encodingName) = Decode(bytes, options.EncodingName);
        if (string.IsNullOrWhiteSpace(text)) throw new CsvImportParsingException("CSV file is empty.");

        var delimiter = ResolveDelimiter(text, options.Delimiter);
        var records = ReadRecords(text, delimiter);
        if (records.Count == 0) throw new CsvImportParsingException("CSV header is missing.");
        if (records[0].Malformed) throw new CsvImportParsingException("CSV header contains invalid quote placement or an unclosed quote.");

        var headers = records[0].Fields.Select(header => header.Trim().TrimStart('\uFEFF')).ToArray();
        if (headers.Length < 2 || headers.Any(string.IsNullOrWhiteSpace))
            throw new CsvImportParsingException("CSV header must contain at least two named columns.");
        if (headers.Distinct(StringComparer.OrdinalIgnoreCase).Count() != headers.Length)
            throw new CsvImportParsingException("CSV header column names must be unique.");

        var indexes = headers.Select((header, index) => (header, index))
            .ToDictionary(item => item.header, item => item.index, StringComparer.OrdinalIgnoreCase);
        var dateIndex = RequiredColumn(indexes, options.Columns.DateColumn, "date");
        var amountIndex = RequiredColumn(indexes, options.Columns.AmountColumn, "amount");
        var descriptionIndex = OptionalColumn(indexes, options.Columns.DescriptionColumn, "description");
        var referenceIndex = OptionalColumn(indexes, options.Columns.ReferenceColumn, "reference");

        var rows = new List<ParsedCsvRow>();
        foreach (var record in records.Skip(1))
        {
            if (record.Fields.All(string.IsNullOrWhiteSpace)) continue;
            if (rows.Count == MaximumRows)
                throw new CsvImportParsingException($"CSV cannot contain more than {MaximumRows} data rows.");

            var errors = new List<string>();
            if (record.Malformed) errors.Add("Row contains invalid quote placement or an unclosed quote.");
            if (record.Fields.Count != headers.Length)
                errors.Add($"Expected {headers.Length} columns but found {record.Fields.Count}.");

            DateOnly? date = null;
            var dateText = ValueAt(record.Fields, dateIndex).Trim();
            if (DateOnly.TryParseExact(
                    dateText, options.DateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var parsedDate))
                date = parsedDate;
            else
                errors.Add($"Date must use the {options.DateFormat} format.");

            decimal? amount = null;
            var amountText = ValueAt(record.Fields, amountIndex).Trim();
            var normalizedAmount = options.DecimalSeparator == ','
                ? amountText.Replace(',', '.')
                : amountText;
            if (decimal.TryParse(
                    normalizedAmount,
                    NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture,
                out var parsedAmount) &&
                parsedAmount != 0m && decimal.Round(parsedAmount, 4) == parsedAmount &&
                decimal.Abs(parsedAmount) <= ImportRow.MaximumAbsoluteAmount)
                amount = parsedAmount;
            else
                errors.Add("Amount must fit decimal(19,4), be non-zero, and have at most four decimal places.");

            var description = descriptionIndex is int descriptionColumn
                ? NullIfWhiteSpace(ValueAt(record.Fields, descriptionColumn))
                : null;
            if (description?.Length > BudgetTransaction.MaximumDescriptionLength)
            {
                errors.Add($"Description cannot exceed {BudgetTransaction.MaximumDescriptionLength} characters.");
                description = description[..BudgetTransaction.MaximumDescriptionLength];
            }

            var reference = referenceIndex is int referenceColumn
                ? NullIfWhiteSpace(ValueAt(record.Fields, referenceColumn))
                : null;
            if (reference?.Length > ImportRow.MaximumExternalReferenceLength)
            {
                errors.Add($"Reference cannot exceed {ImportRow.MaximumExternalReferenceLength} characters.");
                reference = reference[..ImportRow.MaximumExternalReferenceLength];
            }

            var raw = record.Raw;
            if (raw.Length > ImportRow.MaximumRawDataLength)
            {
                errors.Add($"Raw row cannot exceed {ImportRow.MaximumRawDataLength} characters.");
                raw = raw[..ImportRow.MaximumRawDataLength];
            }

            rows.Add(new ParsedCsvRow(
                record.RowNumber, raw, date, amount, description, reference, errors));
        }

        if (rows.Count == 0) throw new CsvImportParsingException("CSV contains no data rows.");
        var fingerprint = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        return new ParsedCsvFile(fingerprint, encodingName, delimiter, headers, rows);
    }

    private static async Task<byte[]> ReadBoundedAsync(Stream content, CancellationToken cancellationToken)
    {
        using var buffer = new MemoryStream();
        var block = new byte[81920];
        while (true)
        {
            var read = await content.ReadAsync(block, cancellationToken);
            if (read == 0) break;
            if (buffer.Length + read > MaximumFileSizeBytes)
                throw new CsvImportParsingException($"CSV file cannot exceed {MaximumFileSizeBytes} bytes.");
            await buffer.WriteAsync(block.AsMemory(0, read), cancellationToken);
        }
        return buffer.ToArray();
    }

    private static (string Text, string EncodingName) Decode(byte[] bytes, string requested)
    {
        try
        {
            var normalized = requested.Trim().ToLowerInvariant();
            if (normalized is "auto" or "utf-8" or "utf8")
            {
                var utf8 = new UTF8Encoding(false, true);
                return (utf8.GetString(bytes), "utf-8");
            }
            if (normalized is "windows-1254" or "cp1254")
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var encoding = Encoding.GetEncoding(
                    1254, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
                return (encoding.GetString(bytes), "windows-1254");
            }
        }
        catch (DecoderFallbackException)
        {
            throw new CsvImportParsingException("CSV bytes are not valid for the selected encoding.");
        }

        throw new CsvImportParsingException("Encoding must be auto, utf-8, or windows-1254.");
    }

    private static char ResolveDelimiter(string text, string requested)
    {
        var value = requested.Trim().ToLowerInvariant();
        if (value is not "auto")
        {
            var explicitDelimiter = value switch
            {
                "comma" or "," => ',',
                "semicolon" or ";" => ';',
                "tab" or "\\t" => '\t',
                _ => '\0'
            };
            if (explicitDelimiter == '\0')
                throw new CsvImportParsingException("Delimiter must be auto, comma, semicolon, or tab.");
            return explicitDelimiter;
        }

        var firstRecord = text.Split(['\r', '\n'], 2)[0];
        var selected = SupportedDelimiters
            .Select(delimiter => (Delimiter: delimiter, Count: CountOutsideQuotes(firstRecord, delimiter)))
            .OrderByDescending(candidate => candidate.Count)
            .ThenBy(candidate => Array.IndexOf(SupportedDelimiters, candidate.Delimiter))
            .First();
        if (selected.Count == 0)
            throw new CsvImportParsingException("CSV delimiter could not be detected.");
        return selected.Delimiter;
    }

    private static int CountOutsideQuotes(string value, char delimiter)
    {
        var quoted = false;
        var count = 0;
        for (var i = 0; i < value.Length; i++)
        {
            if (value[i] == '"')
            {
                if (quoted && i + 1 < value.Length && value[i + 1] == '"') i++;
                else quoted = !quoted;
            }
            else if (!quoted && value[i] == delimiter) count++;
        }
        return count;
    }

    private static List<CsvRecord> ReadRecords(string text, char delimiter)
    {
        var records = new List<CsvRecord>();
        var fields = new List<string>();
        var field = new StringBuilder();
        var raw = new StringBuilder();
        var quoted = false;
        var afterClosingQuote = false;
        var malformed = false;
        var rowNumber = 1;
        var recordStartRow = 1;

        for (var index = 0; index < text.Length; index++)
        {
            var current = text[index];
            if (current == '"')
            {
                raw.Append(current);
                if (quoted && index + 1 < text.Length && text[index + 1] == '"')
                {
                    field.Append('"');
                    raw.Append('"');
                    index++;
                }
                else if (quoted)
                {
                    quoted = false;
                    afterClosingQuote = true;
                }
                else if (field.Length == 0 && !afterClosingQuote)
                {
                    quoted = true;
                }
                else
                {
                    malformed = true;
                    field.Append(current);
                }
            }
            else if (current == delimiter && !quoted)
            {
                fields.Add(field.ToString());
                field.Clear();
                raw.Append(current);
                afterClosingQuote = false;
            }
            else if ((current == '\r' || current == '\n') && !quoted)
            {
                fields.Add(field.ToString());
                field.Clear();
                records.Add(new CsvRecord(recordStartRow, raw.ToString(), fields.ToArray(), malformed));
                fields.Clear();
                raw.Clear();
                afterClosingQuote = false;
                malformed = false;
                if (current == '\r' && index + 1 < text.Length && text[index + 1] == '\n') index++;
                rowNumber++;
                recordStartRow = rowNumber;
            }
            else
            {
                if (afterClosingQuote) malformed = true;
                field.Append(current);
                raw.Append(current);
                if (current == '\n') rowNumber++;
            }
        }

        if (field.Length > 0 || fields.Count > 0 || raw.Length > 0)
        {
            fields.Add(field.ToString());
            records.Add(new CsvRecord(recordStartRow, raw.ToString(), fields.ToArray(), quoted || malformed));
        }
        return records;
    }

    private static int RequiredColumn(
        IReadOnlyDictionary<string, int> indexes, string column, string role)
    {
        if (!indexes.TryGetValue(column.Trim(), out var index))
            throw new CsvImportParsingException($"Mapped {role} column '{column}' was not found in the header.");
        return index;
    }

    private static int? OptionalColumn(
        IReadOnlyDictionary<string, int> indexes, string? column, string role)
    {
        if (string.IsNullOrWhiteSpace(column)) return null;
        return RequiredColumn(indexes, column, role);
    }

    private static string ValueAt(IReadOnlyList<string> values, int index) =>
        index < values.Count ? values[index] : string.Empty;

    private static string? NullIfWhiteSpace(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record CsvRecord(
        int RowNumber,
        string Raw,
        IReadOnlyList<string> Fields,
        bool Malformed);
}
