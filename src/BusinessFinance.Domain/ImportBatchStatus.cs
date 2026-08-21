namespace BusinessFinance.Domain;

public enum ImportBatchStatus : byte
{
    Staged = 1,
    PartiallyImported = 2,
    Imported = 3
}
