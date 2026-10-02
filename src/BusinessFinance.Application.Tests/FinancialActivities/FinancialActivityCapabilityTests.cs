using BusinessFinance.Application.FinancialActivities;

namespace BusinessFinance.Application.Tests.FinancialActivities;

public sealed class FinancialActivityCapabilityTests
{
    [Theory]
    [InlineData(FinancialActivityKind.AccountTransaction, FinancialActivityOrigin.Manual)]
    [InlineData(FinancialActivityKind.AccountTransaction, FinancialActivityOrigin.CsvImport)]
    [InlineData(FinancialActivityKind.CardCharge, FinancialActivityOrigin.Manual)]
    [InlineData(FinancialActivityKind.Transfer, FinancialActivityOrigin.Manual)]
    [InlineData(FinancialActivityKind.CardPayment, FinancialActivityOrigin.Manual)]
    public void CanCancel_AllowsManualAndImportedRealizedActivities(
        FinancialActivityKind kind,
        FinancialActivityOrigin origin)
    {
        Assert.True(FinancialActivityCapabilities.CanCancel(
            kind, origin, FinancialActivityStatus.Realized));
    }

    /// <summary>
    /// A recurring occurrence and an installment item each hold exactly one result id
    /// and cannot be un-realized, so their results are locked.
    /// </summary>
    [Theory]
    [InlineData(FinancialActivityKind.AccountTransaction, FinancialActivityOrigin.Recurring)]
    [InlineData(FinancialActivityKind.CardCharge, FinancialActivityOrigin.Recurring)]
    [InlineData(FinancialActivityKind.CardCharge, FinancialActivityOrigin.Installment)]
    public void CanCancel_RejectsResultsOwnedByASourceThatCannotBeUnrealized(
        FinancialActivityKind kind,
        FinancialActivityOrigin origin)
    {
        Assert.False(FinancialActivityCapabilities.CanCancel(
            kind, origin, FinancialActivityStatus.Realized));
    }

    [Theory]
    [InlineData(FinancialActivityKind.DebtPayment)]
    [InlineData(FinancialActivityKind.DebtCollection)]
    public void CanCancel_RejectsDebtMovements(FinancialActivityKind kind)
    {
        Assert.False(FinancialActivityCapabilities.CanCancel(
            kind, FinancialActivityOrigin.Manual, FinancialActivityStatus.Realized));
    }

    /// <summary>
    /// POS yatışı kendi ucundan geri alınır; kesinti gideri de yatışla birlikte
    /// doğar ve yalnız onunla birlikte geri alınır. Kesinti tek başına iptal
    /// edilseydi yatış, hesaba gerçekte geçmemiş bir tutarı geçmiş gösterirdi.
    /// </summary>
    [Theory]
    [InlineData(FinancialActivityKind.PosDeposit, FinancialActivityOrigin.Manual)]
    [InlineData(FinancialActivityKind.AccountTransaction, FinancialActivityOrigin.PosDeposit)]
    public void CanCancel_RejectsAPosDepositAndItsDeduction(
        FinancialActivityKind kind,
        FinancialActivityOrigin origin)
    {
        Assert.False(FinancialActivityCapabilities.CanCancel(
            kind, origin, FinancialActivityStatus.Realized));
    }

    [Fact]
    public void CanCancel_RejectsAnAlreadyCancelledActivity()
    {
        Assert.False(FinancialActivityCapabilities.CanCancel(
            FinancialActivityKind.AccountTransaction,
            FinancialActivityOrigin.Manual,
            FinancialActivityStatus.Cancelled));
    }

    [Fact]
    public void SupportsAttachments_IsLimitedToBudgetTransactions()
    {
        Assert.True(FinancialActivityCapabilities.SupportsAttachments(
            FinancialActivityKind.AccountTransaction));
        Assert.False(FinancialActivityCapabilities.SupportsAttachments(
            FinancialActivityKind.CardCharge));
        Assert.False(FinancialActivityCapabilities.SupportsAttachments(
            FinancialActivityKind.Transfer));
        Assert.False(FinancialActivityCapabilities.SupportsAttachments(
            FinancialActivityKind.DebtPayment));
    }
}
