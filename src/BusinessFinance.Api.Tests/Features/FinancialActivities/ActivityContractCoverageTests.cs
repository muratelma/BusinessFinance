using BusinessFinance.Api.Features.FinancialActivities;
using BusinessFinance.Application.FinancialActivities;

namespace BusinessFinance.Api.Tests.Features.FinancialActivities;

/// <summary>
/// Sözleşme sözlüklerinin enum'la birlikte büyüdüğünü doğrular.
/// </summary>
/// <remarks>
/// Bu kapı bir üretim hatasından sonra yazıldı. <c>DebtOpening</c> enum'a
/// eklendi, sözlüğe yazılmadı ve derleme sorunsuz geçti; hata İşlemler
/// sayfasını açan ilk istekte <c>KeyNotFoundException</c> olarak çıktı.
///
/// Enum'la anahtarlanmış bir sözlük hiçbir zaman derleyici tarafından
/// korunmaz: eksik giriş yalnız o değer gerçekten üretildiğinde patlar, yani
/// yeni türü kullanan ilk kayıt oluşana kadar sessiz kalır. Kapı, her enum
/// üyesinin bir kablo değeri olduğunu ve iki üyenin aynı değeri
/// paylaşmadığını iddia eder.
/// </remarks>
public sealed class ActivityContractCoverageTests
{
    public static TheoryData<string, IEnumerable<object>, IEnumerable<object>> Maps()
    {
        var data = new TheoryData<string, IEnumerable<object>, IEnumerable<object>>();
        data.Add(
            nameof(FinancialActivityKind),
            Enum.GetValues<FinancialActivityKind>().Cast<object>(),
            FinancialActivityEndpoints.ActivityKindValues.Keys.Cast<object>());
        data.Add(
            nameof(FinancialActivityEffect),
            Enum.GetValues<FinancialActivityEffect>().Cast<object>(),
            FinancialActivityEndpoints.EffectValues.Keys.Cast<object>());
        data.Add(
            nameof(FinancialActivitySourceGroup),
            Enum.GetValues<FinancialActivitySourceGroup>().Cast<object>(),
            FinancialActivityEndpoints.SourceGroupValues.Keys.Cast<object>());
        data.Add(
            nameof(FinancialActivityOrigin),
            Enum.GetValues<FinancialActivityOrigin>().Cast<object>(),
            FinancialActivityEndpoints.OriginValues.Keys.Cast<object>());
        data.Add(
            nameof(FinancialActivityStatus),
            Enum.GetValues<FinancialActivityStatus>().Cast<object>(),
            FinancialActivityEndpoints.StatusValues.Keys.Cast<object>());
        data.Add(
            nameof(PlannedActivityKind),
            Enum.GetValues<PlannedActivityKind>().Cast<object>(),
            FinancialActivityEndpoints.PlannedKindValues.Keys.Cast<object>());
        data.Add(
            nameof(PlannedActivityTiming),
            Enum.GetValues<PlannedActivityTiming>().Cast<object>(),
            FinancialActivityEndpoints.TimingValues.Keys.Cast<object>());
        data.Add(
            nameof(PlannedActivityReadiness),
            Enum.GetValues<PlannedActivityReadiness>().Cast<object>(),
            FinancialActivityEndpoints.ReadinessValues.Keys.Cast<object>());
        data.Add(
            nameof(PlannedActivityAttention),
            Enum.GetValues<PlannedActivityAttention>().Cast<object>(),
            FinancialActivityEndpoints.AttentionValues.Keys.Cast<object>());
        data.Add(
            nameof(PlannedActivityAction),
            Enum.GetValues<PlannedActivityAction>().Cast<object>(),
            FinancialActivityEndpoints.ActionValues.Keys.Cast<object>());
        return data;
    }

    [Theory]
    [MemberData(nameof(Maps))]
    public void ContractValues_CoverEveryEnumMember(
        string enumName,
        IEnumerable<object> members,
        IEnumerable<object> mapped)
    {
        var missing = members.Except(mapped).ToArray();

        Assert.True(
            missing.Length == 0,
            $"{enumName} üyeleri sözleşme sözlüğünde yok: {string.Join(", ", missing)}. " +
            "Eksik giriş derlemeyi bozmaz; feed'i okuyan ilk istekte " +
            "KeyNotFoundException olarak çıkar.");
    }

    [Fact]
    public void ContractValues_AreDistinctWithinEachMap()
    {
        // İki üyenin aynı kablo değerini paylaşması, filtreyi de bozar:
        // istemci bir türü isterken diğerini alır.
        AssertDistinct(nameof(FinancialActivityKind), FinancialActivityEndpoints.ActivityKindValues.Values);
        AssertDistinct(nameof(FinancialActivityEffect), FinancialActivityEndpoints.EffectValues.Values);
        AssertDistinct(nameof(FinancialActivitySourceGroup), FinancialActivityEndpoints.SourceGroupValues.Values);
        AssertDistinct(nameof(FinancialActivityOrigin), FinancialActivityEndpoints.OriginValues.Values);
        AssertDistinct(nameof(FinancialActivityStatus), FinancialActivityEndpoints.StatusValues.Values);
        AssertDistinct(nameof(PlannedActivityKind), FinancialActivityEndpoints.PlannedKindValues.Values);
        AssertDistinct(nameof(PlannedActivityAction), FinancialActivityEndpoints.ActionValues.Values);

        static void AssertDistinct(string name, IEnumerable<string> values)
        {
            var all = values.ToArray();
            Assert.True(
                all.Length == all.Distinct(StringComparer.Ordinal).Count(),
                $"{name} sözlüğünde tekrarlanan kablo değeri var.");
        }
    }
}
