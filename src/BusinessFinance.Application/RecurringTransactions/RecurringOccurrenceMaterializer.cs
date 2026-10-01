using BusinessFinance.Domain;

namespace BusinessFinance.Application.RecurringTransactions;

public enum OccurrenceLookupStatus
{
    Found = 1,
    PlanNotFound = 2,
    PlanInactive = 3,

    /// <summary>İstenen gün planın ritmine düşmüyor.</summary>
    NotOnSchedule = 4
}

public sealed record OccurrenceLookup(
    OccurrenceLookupStatus Status,
    RecurringTransactionOccurrence? Occurrence);

/// <summary>
/// Bir planın tek bir gününe düşen kalemi, izlenerek, bulur; henüz
/// üretilmemişse yalnız o planı o güne kadar ilerleterek üretir.
/// </summary>
/// <remarks>
/// <para>
/// Planlanan görünüm üretilmemiş günleri de gösterir ve kullanıcı onlar
/// üzerinde karar verir: "Ödedim", "tutar belli oldu", toplu ödemede
/// "bunu kapatıyor". Kararın bir satıra yazılabilmesi için satırın var olması
/// gerekir; üretmek sistemin idempotentlik için tuttuğu bir defter işidir.
/// </para>
/// <para>
/// <b>Yalnız bu plan ilerler.</b> Genel üretim bütün planları aynı güne kadar
/// yürütür; ileri bir tarihteki tek kalem için (erken ödeme, tutarı önceden
/// belli olan vergi) bütün planların aylarca kalemini üretmek gereksiz olurdu.
/// Tarih ritme düşmüyorsa plana <b>dokunulmaz</b>: izlenen planı ilerletip
/// kaydetmeden dönmek, aynı istekteki sonraki bir <c>SaveChanges</c>'ın o
/// ilerlemeyi sessizce yazmasına yol açardı.
/// </para>
/// </remarks>
public sealed class RecurringOccurrenceMaterializer(IRecurringTransactionRepository repository)
{
    public async Task<OccurrenceLookup> EnsureAsync(
        Guid userId,
        Guid recurringTransactionId,
        DateOnly scheduledDate,
        CancellationToken cancellationToken)
    {
        // İki deneme: aynı kalemi aynı anda üreten başka bir istek kazanırsa
        // benzersiz anahtar ikinciyi reddeder; ikinci turda kalem bulunur.
        for (var attempt = 0; attempt < 2; attempt++)
        {
            var existing = await repository.FindOccurrenceOwnedByDateAsync(
                recurringTransactionId, scheduledDate, userId, cancellationToken, track: true);
            if (existing is not null)
            {
                return new OccurrenceLookup(OccurrenceLookupStatus.Found, existing);
            }

            var plan = await repository.FindOwnedByIdAsync(
                recurringTransactionId, userId, track: true, cancellationToken);
            if (plan is null) return new OccurrenceLookup(OccurrenceLookupStatus.PlanNotFound, null);
            if (!plan.IsActive) return new OccurrenceLookup(OccurrenceLookupStatus.PlanInactive, null);
            if (!FallsOnSchedule(plan, scheduledDate))
            {
                return new OccurrenceLookup(OccurrenceLookupStatus.NotOnSchedule, null);
            }

            var generated = new List<RecurringTransactionOccurrence>();
            while (plan.IsActive &&
                   plan.NextOccurrenceDate is DateOnly next &&
                   next <= scheduledDate)
            {
                generated.Add(RecurringTransactionOccurrence.Create(Guid.NewGuid(), plan, next));
                plan.AdvanceAfter(next);
            }

            if (await repository.TrySaveGeneratedAsync(generated, cancellationToken))
            {
                return new OccurrenceLookup(
                    OccurrenceLookupStatus.Found,
                    generated.Single(occurrence => occurrence.ScheduledDate == scheduledDate));
            }
        }

        throw new InvalidOperationException("Occurrence generation kept conflicting with another request.");
    }

    /// <summary>
    /// Planı değiştirmeden, sıradaki günden istenen güne yürür.
    /// </summary>
    private static bool FallsOnSchedule(RecurringTransaction plan, DateOnly scheduledDate)
    {
        var date = plan.NextOccurrenceDate;
        var count = plan.GeneratedOccurrenceCount;
        for (var step = 0; step < GenerateRecurringOccurrencesUseCase.MaximumBatchSize; step++)
        {
            if (date is not DateOnly current || current > scheduledDate) return false;
            if (current == scheduledDate) return true;

            count++;
            if (plan.OccurrenceLimit is int limit && count >= limit) return false;
            date = plan.GetFollowingDate(current);
        }

        return false;
    }
}
