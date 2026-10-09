namespace BusinessFinance.Domain;

/// <summary>
/// Kullanıcının takvim günü ile sunucunun UTC günü arasındaki pay.
/// </summary>
/// <remarks>
/// <para>
/// Sunucu saati UTC tutar; kullanıcı kaydın gününü kendi takvimiyle yazar.
/// Türkiye'de gece yarısından 03:00'e kadar kullanıcının "bugün"ü, sunucunun
/// UTC gününden <b>bir gün ileridedir</b>. "Gün gelecekte olamaz" denetimi
/// UTC günüyle yapıldığında o saatlerde bugünün tarihiyle kasa sayımı, gün
/// sonu, POS tahsilatı, yatış ve yükümlülük reddediliyordu (9 Ekim 2026'da
/// gerçek API'de doğrulandı).
/// </para>
/// <para>
/// Denetim bu yüzden UTC gününe bir gün pay tanır. Sunucu kullanıcının saat
/// dilimini bilmez ve bilmesi de gerekmez: hiçbir saat dilimi UTC'den bir
/// takvim gününden fazla ileride değildir. Pay yalnız "gelecekte mi?"
/// sorusunu gevşetir; iki gün sonrası hâlâ reddedilir. Vergi ödemesi aynı
/// payı zaten kullanıyordu.
/// </para>
/// </remarks>
public static class LocalDay
{
    /// <summary>Bir kaydın taşıyabileceği en ileri gün.</summary>
    public static DateOnly LatestAllowed(DateTimeOffset utcNow) =>
        DateOnly.FromDateTime(utcNow.UtcDateTime).AddDays(1);
}
