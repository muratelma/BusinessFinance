namespace BusinessFinance.Domain;

/// <summary>Adı değişse de otomatik kayıtların bulabildiği kategori görevi.</summary>
public enum CategoryRole : byte
{
    InterestExpense = 1,
    InterestIncome = 2,
    CashDifference = 3,
    BankFee = 4
}
