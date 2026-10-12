using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BusinessFinance.Api.Tests;

/// <summary>
/// Saati sabitlenmiş bir API: "bugün" hangi gündür sorusuna bağlı kuralları
/// testin koştuğu saatten bağımsız dener.
/// </summary>
/// <remarks>
/// Erişim belirtecinin süresi gerçek saatle doğrulanır; sabit saatle üretilen
/// belirteç ya süresi dolmuş ya da henüz başlamamış görünürdü. Bu testler
/// belirtecin süresini değil kaydın gününü dener, bu yüzden süre doğrulaması
/// yalnız burada kapatılır.
/// </remarks>
internal static class FixedClockApi
{
    public static BusinessFinanceApiFactory Create(DateTimeOffset utcNow) =>
        Create(new TestClock(utcNow));

    public static BusinessFinanceApiFactory Create(TestClock clock) => new(
        configureServices: services =>
        {
            services.RemoveAll<TimeProvider>();
            services.AddSingleton<TimeProvider>(clock);
            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options => options.TokenValidationParameters.ValidateLifetime = false);
        });
}

/// <summary>Testin elle ilerlettiği ya da geri aldığı saat.</summary>
internal sealed class TestClock(DateTimeOffset utcNow) : TimeProvider
{
    public DateTimeOffset UtcNow { get; set; } = utcNow;

    public override DateTimeOffset GetUtcNow() => UtcNow;
}
