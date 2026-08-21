namespace BusinessFinance.Domain;

public enum MonthEndBehavior
{
    ClampToLastDay = 1,
    SkipInvalidPeriod = 2
}
