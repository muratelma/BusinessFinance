namespace BusinessFinance.Domain;

/// <summary>
/// Kullanıcının takvim günü: sunucunun saati UTC, günü Türkiye günüdür.
/// </summary>
/// <remarks>
/// <para>
/// Sunucu anı UTC tutar; kullanıcı kaydın gününü kendi takvimiyle yazar.
/// Türkiye'de gece yarısından 03:00'e kadar kullanıcının "bugün"ü UTC gününden
/// bir gün ileridedir. "Gün gelecekte olamaz" denetimi UTC günüyle yapıldığında
/// o saatlerde bugünün tarihiyle kasa sayımı, gün sonu, POS tahsilatı, yatış ve
/// yükümlülük reddediliyordu (9 Ekim 2026'da gerçek API'de doğrulandı).
/// </para>
/// <para>
/// İlk çözüm UTC gününe bir gün pay tanımaktı; o pay gece sorununu kapattı ama
/// günün her saatinde yarının tarihini de kabul etti. Gün artık Türkiye
/// saatiyle hesaplanır (kullanıcı kararı, 12 Ekim 2026): pay yoktur, yarının
/// tarihi reddedilir. Türkiye 2016'dan beri yıl boyu UTC+3'tür, bu yüzden
/// sabit fark yeter ve saat dilimi verisine bağımlılık doğmaz.
/// </para>
/// <para>
/// Yalnız <b>gün</b> buradan okunur. Kayıt anları, oturum süreleri ve giriş
/// sırası UTC kalır. Uygulama başka bir saat dilimine açılırsa değişecek tek
/// yer burasıdır.
/// </para>
/// </remarks>
public static class LocalDay
{
    private static readonly TimeSpan Offset = TimeSpan.FromHours(3);

    /// <summary>Kullanıcının takvimindeki bugün.</summary>
    public static DateOnly Today(DateTimeOffset now) =>
        DateOnly.FromDateTime(now.ToOffset(Offset).DateTime);

    /// <summary>Bir kaydın taşıyabileceği en ileri gün: bugün.</summary>
    public static DateOnly LatestAllowed(DateTimeOffset now) => Today(now);
}
