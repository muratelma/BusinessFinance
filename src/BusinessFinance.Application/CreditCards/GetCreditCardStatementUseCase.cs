using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.CreditCards;

public sealed class GetCreditCardStatementUseCase(
    ICurrentUser currentUser,
    ICreditCardRepository cardRepository,
    ICreditCardStatementRepository statementRepository)
{
    public async Task<ApplicationResult<CreditCardStatementDto>> ExecuteAsync(
        GetCreditCardStatementQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CreditCardStatementDto>.Failure(
                CreditCardErrors.AuthenticationRequired);
        }

        var card = await cardRepository.FindOwnedByIdAsync(
            query.CreditCardId,
            userId,
            false,
            cancellationToken);
        if (card is null)
        {
            return ApplicationResult<CreditCardStatementDto>.Failure(
                CreditCardErrors.NotFound(query.CreditCardId));
        }

        try
        {
            var period = CreditCardStatementPeriod.ForClosingMonth(
                card,
                query.Year,
                query.Month);
            if (query.AsOfDate < period.ClosingDate)
            {
                return ApplicationResult<CreditCardStatementDto>.Failure(
                    CreditCardErrors.Validation("Statement cannot be calculated before its closing date."));
            }

            var snapshot = await statementRepository.GetActivityAsync(
                card.Id,
                userId,
                period,
                query.AsOfDate,
                cancellationToken);
            var statement = CreditCardStatement.Create(
                card,
                query.Year,
                query.Month,
                query.AsOfDate,
                snapshot.PreviousBalance,
                snapshot.PeriodCharges,
                snapshot.PaymentsThroughClosing,
                snapshot.PaymentsAfterClosing);
            return ApplicationResult<CreditCardStatementDto>.Success(ToDto(statement));
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return ApplicationResult<CreditCardStatementDto>.Failure(
                CreditCardErrors.Validation(exception.Message));
        }
    }

    internal static CreditCardStatementDto ToDto(CreditCardStatement statement) => new(
        statement.CreditCardId,
        statement.Year,
        statement.Month,
        statement.PeriodStart,
        statement.ClosingDate,
        statement.DueDate,
        statement.PreviousBalance,
        statement.PeriodCharges,
        statement.PaymentsThroughClosing,
        statement.StatementBalance,
        statement.PaymentsAfterClosing,
        statement.RemainingBalance,
        statement.MinimumPayment,
        statement.RemainingMinimumPayment,
        statement.MinimumPaymentRate,
        statement.Currency,
        statement.PaymentStatus);
}

/// <summary>
/// Kartın "şu an ödenmesi gereken" ekstresi: kesim tarihi geçmiş en son dönem.
/// </summary>
/// <remarks>
/// Neden kart listesi DTO'suna gömülmedi: bir ekstre beş toplam sorgusu
/// demek, listede N kart × 5 sorgu ederdi. Kart detayı tek kart gösterdiği
/// için bu uç nokta orada tek kez çağrılıyor.
/// </remarks>
public sealed class GetCurrentCreditCardStatementUseCase(
    ICurrentUser currentUser,
    ICreditCardRepository cardRepository,
    ICreditCardStatementRepository statementRepository)
{
    public async Task<ApplicationResult<CurrentCreditCardStatementDto>> ExecuteAsync(
        Guid creditCardId,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CurrentCreditCardStatementDto>.Failure(
                CreditCardErrors.AuthenticationRequired);
        }

        var card = await cardRepository.FindOwnedByIdAsync(
            creditCardId,
            userId,
            false,
            cancellationToken);
        if (card is null)
        {
            return ApplicationResult<CurrentCreditCardStatementDto>.Failure(
                CreditCardErrors.NotFound(creditCardId));
        }

        // Kesim günü 1–28 arasında olduğu için bu tarih her ayda geçerli.
        var closingThisMonth = new DateOnly(
            asOfDate.Year,
            asOfDate.Month,
            card.StatementClosingDay);
        var closingMonth = asOfDate >= closingThisMonth
            ? asOfDate
            : asOfDate.AddMonths(-1);
        if (closingMonth.Year < 2000)
        {
            return ApplicationResult<CurrentCreditCardStatementDto>.Success(
                new CurrentCreditCardStatementDto(card.Id, null));
        }

        var period = CreditCardStatementPeriod.ForClosingMonth(
            card,
            closingMonth.Year,
            closingMonth.Month);
        var snapshot = await statementRepository.GetActivityAsync(
            card.Id,
            userId,
            period,
            asOfDate,
            cancellationToken);
        var statement = CreditCardStatement.Create(
            card,
            closingMonth.Year,
            closingMonth.Month,
            asOfDate,
            snapshot.PreviousBalance,
            snapshot.PeriodCharges,
            snapshot.PaymentsThroughClosing,
            snapshot.PaymentsAfterClosing);
        return ApplicationResult<CurrentCreditCardStatementDto>.Success(
            new CurrentCreditCardStatementDto(
                card.Id,
                GetCreditCardStatementUseCase.ToDto(statement)));
    }
}
