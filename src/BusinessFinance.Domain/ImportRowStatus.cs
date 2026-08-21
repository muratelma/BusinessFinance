namespace BusinessFinance.Domain;

public enum ImportRowStatus : byte
{
    Valid = 1,
    Invalid = 2,
    Ready = 3,
    Imported = 4,
    PendingDuplicateReview = 5,
    SkippedDuplicate = 6
}
