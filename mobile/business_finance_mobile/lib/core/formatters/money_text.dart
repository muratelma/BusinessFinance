/// Sunucudan gelen dört ondalıklı para dizesini ekrana yazar.
///
/// API sözleşmesi parayı dört ondalıklı **string** taşır: JSON `number`
/// hassasiyet kaybettirirdi ve backend `decimal(19,4)` kullanıyor. Ekranda ise
/// dört basamak okumayı zorlaştırıyor ve tutarları göz karşılaştırmasından
/// çıkarıyordu; gösterim daima **iki** basamağa yuvarlanır.
///
/// Yuvarlama gösterimdedir, hesap değil. İstemci parayı ikinci kez hesaplamaz;
/// toplamlar backend'den gelir. Bunun görünür sonucu şudur: ekrandaki
/// satırların gözle toplamı, ekrandaki toplamdan bir kuruş sapabilir. Doğru
/// olan toplamdır — satırlar yuvarlanmıştır, toplam yuvarlanmamış değerlerden
/// hesaplanmıştır.
class MoneyText {
  const MoneyText._();

  /// Ekranda gösterilen ondalık basamak sayısı.
  static const int displayDecimals = 2;

  /// Tutar eksi mi.
  ///
  /// Kart borcu **negatif olabilir** (Aşama 06 Grup 5): fazla ödenmiş ya da
  /// ödenmiş harcaması iptal edilmiş kartta duran para kullanıcınındır. Ekranda
  /// bu "eksi borç" diye değil, "alacağınız var" diye okunur; kararı çağıran
  /// verir, bu yalnız soruyu cevaplar.
  static bool isNegative(String amount) => amount.startsWith('-');

  /// Tutarın işaretsiz hâli.
  ///
  /// Cümlenin kendisi yönü söylediğinde eksi işareti gereksizdir ve
  /// "−₺500 alacağınız var" gibi iki kez olumsuzlanmış bir metin doğururdu.
  static String unsigned(String amount) =>
      amount.startsWith('-') ? amount.substring(1) : amount;

  static String format(String amount, String currency) {
    final match = RegExp(r'^(-?)(\d+)\.(\d{4})$').firstMatch(amount);
    if (match == null) {
      return '$amount $currency';
    }

    final rounded = _roundToDisplay(match.group(2)!, match.group(3)!);
    final symbol = currency == 'TRY' ? '₺' : '$currency ';
    return '${match.group(1)!}$symbol'
        '${_groupThousands(rounded.integer)},${rounded.fraction}';
  }

  /// Oran ve yüzdeleri ekrana yazar (`151.0780` -> `151,08`, `25.0000` -> `25`).
  ///
  /// Yüzde para değildir ama sorun aynı: API dört ondalıklı string gönderiyor
  /// ve `%151,0780` okunmuyor. Paradan farkı, anlamsız sıfırların büsbütün
  /// atılması — bir oranda `25` ile `25,00` aynı şeyi söyler, tutarlarda ise
  /// kuruş sütunu hizayı taşır.
  static String percent(String value) {
    final match = RegExp(r'^(-?)(\d+)\.(\d{4})$').firstMatch(value);
    if (match == null) return value;

    final rounded = _roundToDisplay(match.group(2)!, match.group(3)!);
    var fraction = rounded.fraction;
    while (fraction.isNotEmpty && fraction.endsWith('0')) {
      fraction = fraction.substring(0, fraction.length - 1);
    }

    return fraction.isEmpty
        ? '${match.group(1)!}${rounded.integer}'
        : '${match.group(1)!}${rounded.integer},$fraction';
  }

  /// Sunucu tutarını, kullanıcının düzenleyebileceği alan değerine çevirir
  /// (`3500.0000` -> `3500`, `3500.5000` -> `3500,5`).
  ///
  /// [format]'tan iki farkı var ve ikisi de kasıtlı: para simgesi ve binlik
  /// ayracı yok — alan onları geri okuyamaz — ve **yuvarlama yok**. Önerilen
  /// tutar iki basamağa yuvarlansaydı, dört basamaklı bir ekstre kalanına
  /// kuruşun altında eksik ödeme yapılır, ekstre kapanmış görünmezdi. Yalnız
  /// anlamsız son sıfırlar atılıyor.
  static String editable(String amount) {
    final match = RegExp(r'^(-?)(\d+)\.(\d{4})$').firstMatch(amount);
    if (match == null) return amount;

    var fraction = match.group(3)!;
    while (fraction.isNotEmpty && fraction.endsWith('0')) {
      fraction = fraction.substring(0, fraction.length - 1);
    }

    return fraction.isEmpty
        ? '${match.group(1)!}${match.group(2)!}'
        : '${match.group(1)!}${match.group(2)!},$fraction';
  }

  /// Alana yazılan tutarı sunucunun biçimine çevirir (`1.233,50` → `1233.50`).
  ///
  /// Alan yazarken binlik ayırıcı koyar (`TurkishAmountInputFormatter`):
  /// virgül ondalık ayıracıdır, noktalar binlik. Virgülsüz `23.100` binlik
  /// gruplu tam sayıdır (23,1 değil); gruplanmamış `12.5` eski yazımdır ve
  /// ondalık okunur. Biçim tutmazsa `null`.
  static String? normalizeInput(String input) {
    var text = input.trim().replaceAll(' ', '');
    if (text.contains(',')) {
      if (!RegExp(r'^(\d+|\d{1,3}(\.\d{3})+),\d{1,4}$').hasMatch(text)) {
        return null;
      }
      text = text.replaceAll('.', '').replaceAll(',', '.');
    } else if (RegExp(r'^\d{1,3}(\.\d{3})+$').hasMatch(text)) {
      text = text.replaceAll('.', '');
    }
    if (!RegExp(r'^\d+(\.\d{1,4})?$').hasMatch(text)) {
      return null;
    }
    return text;
  }

  /// [editable] ile aynı değer, alanın yazarken kullandığı binlik ayırıcıyla
  /// (`3500.5000` → `3.500,5`): önceden dolu gelen tutar da yazılan tutar
  /// gibi görünür.
  static String field(String amount) {
    final text = editable(amount);
    final match = RegExp(r'^(-?)(\d+)(,\d+)?$').firstMatch(text);
    if (match == null) return text;
    final digits = match.group(2)!;
    final grouped = StringBuffer();
    for (var i = 0; i < digits.length; i++) {
      if (i > 0 && (digits.length - i) % 3 == 0) grouped.write('.');
      grouped.write(digits[i]);
    }
    return '${match.group(1)!}$grouped${match.group(3) ?? ''}';
  }

  /// Dört basamaklı kesri ikiye yuvarlar, gerekirse tam kısma elde taşır.
  ///
  /// Kesirli aritmetik `double` üzerinden yapılmaz: para dizesi 19 haneli
  /// olabilir ve `double` o boyda tam sayıyı temsil edemez. Basamaklar
  /// üzerinde çalışmak hem tam hem de girdiden bağımsız olarak kesin.
  static ({String integer, String fraction}) _roundToDisplay(
    String integer,
    String fraction,
  ) {
    final kept = int.parse(fraction.substring(0, displayDecimals));
    final dropped = int.parse(fraction.substring(displayDecimals));

    // Yarıda yukarı: 0,005 aşağı yuvarlanırsa her kuruş sistematik olarak
    // eksilir ve uzun listelerde fark birikir.
    if (dropped < 50) {
      return (integer: integer, fraction: kept.toString().padLeft(2, '0'));
    }

    final carried = kept + 1;
    return carried < 100
        ? (integer: integer, fraction: carried.toString().padLeft(2, '0'))
        : (integer: _increment(integer), fraction: '00');
  }

  /// Ondalık bir tam sayı dizesini bir artırır (`999` -> `1000`).
  static String _increment(String value) {
    final digits = value.split('').map(int.parse).toList();
    for (var index = digits.length - 1; index >= 0; index--) {
      if (digits[index] < 9) {
        digits[index]++;
        return digits.join();
      }
      digits[index] = 0;
    }
    return '1${digits.join()}';
  }

  static String _groupThousands(String value) {
    final buffer = StringBuffer();
    for (var index = 0; index < value.length; index++) {
      if (index > 0 && (value.length - index) % 3 == 0) {
        buffer.write('.');
      }
      buffer.write(value[index]);
    }
    return buffer.toString();
  }
}
