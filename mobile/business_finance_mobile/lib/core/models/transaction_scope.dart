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

/// Türetme zincirinin istemcideki **önizlemesi**: kullanıcının açık seçimi →
/// kaynağın (hesap/kart) etiketi → kategorinin varsayılanı.
///
/// Kararın sahibi sunucudur (`TransactionScopeResolution`); burası onun
/// kopyası değil, formun kullanıcıya **ne yazılacağını gösterebilmesi** için
/// var. Çip boş dursaydı kullanıcı kaydın hangi tarafa yazıldığını ancak
/// listeye düştükten sonra görürdü; yanlış etiketlenmiş bir kayıt ise işletme
/// netini sessizce bozar. Form gösterdiği değeri açıkça gönderir, böylece
/// ekranda okunan ile yazılan aynı olur.
///
/// Üçü de boşsa `null` döner: istemci de kapsam **uydurmaz**.
TransactionScope? previewResolvedScope({
  TransactionScope? explicit,
  TransactionScope? source,
  TransactionScope? category,
}) => explicit ?? source ?? category;
