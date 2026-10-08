/// Bir kaydın işletmeye mi sahibinin cebine mi ait olduğu.
///
/// Para tek havuzda yaşar; bu yalnız bir raporlama boyutudur (ADR 0013).
/// Üçüncü bir "bilinmiyor" değeri **yok**: sunucu kapsamı çözemediğinde kayıt
/// yazmaz, isteği reddeder. Bu yüzden istemcide de boş değer yalnız iki şeyi
/// anlatır — "bu kaynak kapsam belirlemiyor" (varsayılan kapsam) ya da
/// "iki tarafı birden oku" (filtre).
enum TransactionScope {
  business('business', 'İşletme'),
  personal('personal', 'Şahsi');

  const TransactionScope(this.apiValue, this.label);

  /// Sunucunun kararlı makine değeri. Kullanıcı cümlesi değildir.
  final String apiValue;

  /// Ekranda görünen ad. API bunu göndermez; istemci üretir.
  final String label;

  /// Tanınmayan bir metin `FormatException` atar: sözleşme kayması sessizce
  /// "kapsamsız" diye okunursa, kapsamı olan bir kayıt kapsamsız görünür.
  static TransactionScope fromApi(String value) => switch (value) {
    'business' => business,
    'personal' => personal,
    _ => throw FormatException('Unknown transaction scope: $value'),
  };

  /// Boş bırakılabilen kapsam alanı (`defaultScope`, filtre, feed satırı).
  static TransactionScope? fromApiOrNull(Object? value) {
    if (value == null) return null;
    if (value is! String) {
      throw FormatException('Invalid transaction scope: $value');
    }
    return fromApi(value);
  }
}

/// Kapsam filtresinin üç konumu için ekran adı: boş değer `Hepsi`.
///
/// Filtre ile varsayılan kapsam aynı `TransactionScope?` tipini kullanıyor ama
/// boşluğun anlamı farklı; etiketi de bu yüzden ayrı bir yerde duruyor.
String scopeFilterLabel(TransactionScope? scope) => scope?.label ?? 'Hepsi';

/// Taraf kuralının istemcideki **önizlemesi** (ADR 0020): kategori tek
/// taraflıysa taraf odur; iki tarafa açıksa kullanıcının açık seçimi, o da
/// yoksa kaynağın (hesap/kart) etiketi. Tarafı sabit olan ekranlar (POS, gün
/// sonu, cari) taraf sormaz ve bu fonksiyonu kullanmaz.
///
/// Kararın sahibi sunucudur (`TransactionScopeResolution`); burası formun
/// kullanıcıya **ne yazılacağını gösterebilmesi** için var. Form gösterdiği
/// değeri gönderir, böylece ekranda okunan ile yazılan aynı olur; kategorinin
/// izin vermediği bir tarafı göndermez, çünkü sunucu onu reddeder.
///
/// Hiçbiri yoksa `null` döner: istemci de taraf **uydurmaz**.
TransactionScope? previewResolvedScope({
  TransactionScope? explicit,
  TransactionScope? source,
  TransactionScope? category,
}) => category ?? explicit ?? source;

/// İki tarafa açık kategoride çipin altındaki kısa açıklama: seçimin
/// **nereden** geldiğini söyler. Tek taraflı kategoride çip çizilmez, bilgi
/// satırı çizilir (`AppScopeSection`).
String scopePreviewHelperText({
  TransactionScope? explicit,
  TransactionScope? source,
  String? sourceName,
}) {
  if (explicit != null) return 'Bu kayıt için siz seçtiniz.';
  if (source != null) {
    return sourceName == null
        ? 'Ödeme kaynağının etiketinden geldi — değiştirebilirsiniz.'
        : '$sourceName etiketinden geldi — değiştirebilirsiniz.';
  }
  return 'Bu kayıt için seçin.';
}

/// Kategori [side] tarafındaki bir kayıtta kullanılabilir mi: o tarafa özelse
/// ya da iki tarafa açıksa evet (ADR 0020 T2).
///
/// Tarafı sabit olan ekranlar (POS, gün sonu, cari, `Kendime aldım`) yalnız
/// kullanabilecekleri kategorileri listeler; sunucu öbürünü zaten reddeder.
bool categoryAllowsSide(
  TransactionScope? categorySide,
  TransactionScope side,
) => categorySide == null || categorySide == side;
