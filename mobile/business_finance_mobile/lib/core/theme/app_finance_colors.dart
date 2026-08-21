import 'package:flutter/material.dart';

/// Finansal anlam renkleri.
///
/// Gelir / gider / nötr ayrımı **ürün anlamıdır**, Material rolü değildir.
/// `ColorScheme.primary` gibi rollerin üzerine bindirilirse seed rengi
/// değiştiği anda anlam kayar; ham `Colors.green.shade700` gibi sabitler ise
/// karanlık temaya hiç tepki vermez. Bu yüzden ayrı bir `ThemeExtension`
/// olarak taşınır (bkz. `documentation/adr/0006-*`).
///
/// Sözleşme:
/// - `income` yalnız gelire ayrılmıştır; hesap kaynağı veya "olumlu" durum
///   için kullanılmaz.
/// - `neutral` gelirle karışmaz (transfer, kart borcu ödemesi).
/// - Hiçbir bilgi yalnız renkle taşınmaz; bu token'lar her zaman ikon veya
///   metinle birlikte kullanılır.
///
/// ## Rol başına iki ton, üç değil
///
/// Her rolün **metin tonu** ve **grafik tonu** vardır ve ikisi de adı konmuş
/// birer token'dır:
///
/// - `*` (ör. `income`) — yüzey üzerindeki **metin ve ikon** rengi. WCAG AA
///   normal metin eşiğine (4.5:1) tabidir, bu yüzden canlı olamaz.
/// - `*Fill` (ör. `incomeFill`) — halka dilimi, çubuk, ilerleme dolgusu gibi
///   **metin olmayan geometri**. Eşiği 3:1'dir, bu yüzden belirgin biçimde
///   daha canlı olabilir.
/// - `*Container` + `on*Container` — chip ve rozet çifti.
///
/// Bu ayrım bir düzeltmedir. Önceden grafikler metin tonunu kullanıyordu;
/// aynı ekranda koyu metin yeşili, soluk rozet yeşili ve ikon kapsülünün
/// alfa'lı yeşili yan yana düşüyor, "üç farklı yeşil" hissi veriyordu. Artık
/// tonlar tesadüf değil: her rol için tam olarak iki tane var ve hangisinin
/// nerede kullanılacağı token adından okunuyor.
@immutable
class AppFinanceColors extends ThemeExtension<AppFinanceColors> {
  const AppFinanceColors({
    required this.income,
    required this.incomeFill,
    required this.incomeContainer,
    required this.onIncomeContainer,
    required this.expense,
    required this.expenseFill,
    required this.expenseContainer,
    required this.onExpenseContainer,
    required this.neutral,
    required this.neutralFill,
    required this.neutralContainer,
    required this.onNeutralContainer,
    required this.planned,
    required this.plannedContainer,
    required this.onPlannedContainer,
    required this.cancelled,
    required this.cancelledContainer,
    required this.onCancelledContainer,
    required this.categorySlices,
    required this.categoryOtherSlice,
  });

  /// Aydınlık tema paleti. Değerler `AppTheme.light()` yüzeylerine karşı
  /// kontrast testiyle doğrulanır; metin tonları 4.5:1, dolgu tonları 3:1
  /// eşiğinin hemen üstünde seçilmiştir — yani kapının izin verdiği **en
  /// canlı** değerler.
  static const light = AppFinanceColors(
    income: Color(0xFF137D3F),
    incomeFill: Color(0xFF209E54),
    incomeContainer: Color(0xFFC7E8D4),
    onIncomeContainer: Color(0xFF0C4A26),
    expense: Color(0xFFCF2E1F),
    expenseFill: Color(0xFFE5544A),
    expenseContainer: Color(0xFFFBD9D5),
    onExpenseContainer: Color(0xFF651A12),
    // Nötr rol mordan **maviye** döndü. Mor, gelir yeşili ve gider kırmızısıyla
    // aynı ailede değildi: ekranda üçüncü bir ton ailesi açıyor ve transferi
    // finansal bir anlam yerine dekoratif bir vurgu gibi gösteriyordu. Mavi
    // ikisiyle de uyumlu, ikisiyle de karıştırılmayan tek nötr hue'dur.
    neutral: Color(0xFF0B6AD6),
    neutralFill: Color(0xFF2589FA),
    neutralContainer: Color(0xFFCFE2FA),
    onNeutralContainer: Color(0xFF0A3F80),
    planned: Color(0xFF44515C),
    plannedContainer: Color(0xFFDDE3E8),
    onPlannedContainer: Color(0xFF232C33),
    cancelled: Color(0xFF5F6771),
    cancelledContainer: Color(0xFFDCE0E4),
    onCancelledContainer: Color(0xFF2A2F34),
    categorySlices: _lightCategorySlices,
    categoryOtherSlice: Color(0xFF8A929B),
  );

  /// Karanlık tema paleti. Aydınlık paletin aynısı değildir: koyu yüzeyde aynı
  /// koyu yeşil/kırmızı okunmaz, bu yüzden roller açık tonlara, chip
  /// arkaplanları ise doygun koyu tonlara döner.
  static const dark = AppFinanceColors(
    income: Color(0xFF4ED88A),
    incomeFill: Color(0xFF35C270),
    incomeContainer: Color(0xFF12331F),
    onIncomeContainer: Color(0xFF9FE7BC),
    expense: Color(0xFFFF8A80),
    expenseFill: Color(0xFFFF8C82),
    expenseContainer: Color(0xFF431814),
    onExpenseContainer: Color(0xFFFFC7C2),
    neutral: Color(0xFF8AB4F8),
    neutralFill: Color(0xFF4F97F5),
    neutralContainer: Color(0xFF0E2A47),
    onNeutralContainer: Color(0xFFB6D4FB),
    planned: Color(0xFFAFBAC4),
    plannedContainer: Color(0xFF283138),
    onPlannedContainer: Color(0xFFC3CDD5),
    cancelled: Color(0xFF9BA3AD),
    cancelledContainer: Color(0xFF2C3138),
    onCancelledContainer: Color(0xFFB7BEC6),
    categorySlices: _darkCategorySlices,
    categoryOtherSlice: Color(0xFFB3BAC3),
  );

  /// Kategori dağılımının dilim renkleri: **ayrı hue'lar**.
  ///
  /// Önce tek hue'un (kırmızının) altı tonuydu; gerekçe "ekrandaki anlamlı
  /// renk sayısı ikiden yediye çıkmasın"dı. Liste biçiminde bu yeterliydi ama
  /// halka grafiğinde çalışmıyor: yan yana duran altı kırmızı tonu göz
  /// efsaneyle eşleştiremiyor, tam da renk körlüğü olmayan kullanıcıda bile.
  ///
  /// Sonraki adımda turuncu ve altın çıkıp yerlerine **yeşil ve mavi** geldi:
  /// o iki ton, açıldıkça kahverengiye kayıyordu ve halka isli görünüyordu.
  /// Bu, "yeşil yalnız gelirdir, mavi yalnız nötrdür" kuralının kategori
  /// paletinde **bilerek** gevşetilmesi demek. Gevşeme şu üç şeyle sınırlı:
  ///
  /// 1. Hue ailesi aynı, **ton ailesi değil**: kategori yeşili misket yeşili
  ///    (`0xFF679612`), gelir yeşili koyu orman yeşili (`0xFF137D3F`);
  ///    kategori mavisi teal'e kaçıyor (`0xFF0B8798`), nötr mavi gerçek mavi
  ///    (`0xFF0B6AD6`). Aradaki RGB uzaklığı hâlâ test edilen eşiğin üstünde.
  /// 2. Rol renkleri **tutar metinlerinde** kullanılıyor, kategori renkleri
  ///    yalnız halkanın dilimlerinde ve efsane ikonunda. İkisi aynı ekranda
  ///    yan yana düşse bile aynı işi yapmıyor.
  /// 3. Renk zaten tek başına anlam taşımıyor: efsanenin her satırı
  ///    kategorinin **ikonunu**, adını ve tutarını da yazıyor; kartın tamamı
  ///    zaten giderdir.
  ///
  /// Beş renk, dört kategori ve "Diğer" için: "Diğer" kendi grisini kullandığı
  /// için palet dört dilime yetiyor, beşinci yedek.
  ///
  /// Tonlar bilerek **kontrast kapısının izin verdiği kadar açık**: ilk seçim
  /// metin gibi ölçülüp koyu tutulmuştu. Dilim ve ikon metin değil, eşikleri
  /// 3:1 — bu bütçe ton açmaya harcandı, aydınlık palet 3.1–4.5:1 aralığında
  /// duruyor. Daha açığı kartın beyazına gömülür; aydınlık temada yeşilin
  /// "daha açık" olamamasının sebebi de budur, beyaz zemin sınırı.
  static const _lightCategorySlices = [
    Color(0xFFE0574A),
    Color(0xFF679612),
    Color(0xFF0B8798),
    Color(0xFFD63384),
    Color(0xFF8B5CF6),
  ];

  static const _darkCategorySlices = [
    Color(0xFFFF8C82),
    Color(0xFFB6E05A),
    Color(0xFF4FD8E8),
    Color(0xFFFF9FCB),
    Color(0xFFC48CEC),
  ];

  /// Yüzey üzerindeki metin/ikon rengi (AA normal metin).
  final Color income;

  /// Metin olmayan geometri: halka dilimi, çubuk, ilerleme dolgusu.
  final Color incomeFill;
  final Color incomeContainer;
  final Color onIncomeContainer;

  final Color expense;
  final Color expenseFill;
  final Color expenseContainer;
  final Color onExpenseContainer;

  final Color neutral;
  final Color neutralFill;
  final Color neutralContainer;
  final Color onNeutralContainer;

  final Color planned;
  final Color plannedContainer;
  final Color onPlannedContainer;

  final Color cancelled;
  final Color cancelledContainer;
  final Color onCancelledContainer;

  final List<Color> categorySlices;

  /// "Diğer" diliminin rengi.
  ///
  /// Paletten değil, ayrı: o dilim bir kategori değil kalanın toplamıdır ve
  /// kendine ait bir kimliği yoktur. Nötr gri, adı olan kategorilerle
  /// yarışmasın diye.
  final Color categoryOtherSlice;

  /// Sırayla dilim rengi verir; kategori sayısı paletten fazlaysa başa döner.
  Color categorySlice(int index) =>
      categorySlices[index % categorySlices.length];

  static AppFinanceColors of(BuildContext context) {
    final colors = Theme.of(context).extension<AppFinanceColors>();
    if (colors == null) {
      throw FlutterError(
        'AppFinanceColors tema uzantısı bulunamadı. Tema AppTheme.light() '
        'veya AppTheme.dark() ile kurulmalıdır.',
      );
    }
    return colors;
  }

  @override
  AppFinanceColors copyWith({
    Color? income,
    Color? incomeFill,
    Color? incomeContainer,
    Color? onIncomeContainer,
    Color? expense,
    Color? expenseFill,
    Color? expenseContainer,
    Color? onExpenseContainer,
    Color? neutral,
    Color? neutralFill,
    Color? neutralContainer,
    Color? onNeutralContainer,
    Color? planned,
    Color? plannedContainer,
    Color? onPlannedContainer,
    Color? cancelled,
    Color? cancelledContainer,
    Color? onCancelledContainer,
    List<Color>? categorySlices,
    Color? categoryOtherSlice,
  }) {
    return AppFinanceColors(
      income: income ?? this.income,
      incomeFill: incomeFill ?? this.incomeFill,
      incomeContainer: incomeContainer ?? this.incomeContainer,
      onIncomeContainer: onIncomeContainer ?? this.onIncomeContainer,
      expense: expense ?? this.expense,
      expenseFill: expenseFill ?? this.expenseFill,
      expenseContainer: expenseContainer ?? this.expenseContainer,
      onExpenseContainer: onExpenseContainer ?? this.onExpenseContainer,
      neutral: neutral ?? this.neutral,
      neutralFill: neutralFill ?? this.neutralFill,
      neutralContainer: neutralContainer ?? this.neutralContainer,
      onNeutralContainer: onNeutralContainer ?? this.onNeutralContainer,
      planned: planned ?? this.planned,
      plannedContainer: plannedContainer ?? this.plannedContainer,
      onPlannedContainer: onPlannedContainer ?? this.onPlannedContainer,
      cancelled: cancelled ?? this.cancelled,
      cancelledContainer: cancelledContainer ?? this.cancelledContainer,
      onCancelledContainer: onCancelledContainer ?? this.onCancelledContainer,
      categorySlices: categorySlices ?? this.categorySlices,
      categoryOtherSlice: categoryOtherSlice ?? this.categoryOtherSlice,
    );
  }

  @override
  AppFinanceColors lerp(covariant AppFinanceColors? other, double t) {
    if (other == null) {
      return this;
    }
    return AppFinanceColors(
      income: Color.lerp(income, other.income, t)!,
      incomeFill: Color.lerp(incomeFill, other.incomeFill, t)!,
      incomeContainer: Color.lerp(incomeContainer, other.incomeContainer, t)!,
      onIncomeContainer: Color.lerp(
        onIncomeContainer,
        other.onIncomeContainer,
        t,
      )!,
      expense: Color.lerp(expense, other.expense, t)!,
      expenseFill: Color.lerp(expenseFill, other.expenseFill, t)!,
      expenseContainer: Color.lerp(
        expenseContainer,
        other.expenseContainer,
        t,
      )!,
      onExpenseContainer: Color.lerp(
        onExpenseContainer,
        other.onExpenseContainer,
        t,
      )!,
      neutral: Color.lerp(neutral, other.neutral, t)!,
      neutralFill: Color.lerp(neutralFill, other.neutralFill, t)!,
      neutralContainer: Color.lerp(
        neutralContainer,
        other.neutralContainer,
        t,
      )!,
      onNeutralContainer: Color.lerp(
        onNeutralContainer,
        other.onNeutralContainer,
        t,
      )!,
      planned: Color.lerp(planned, other.planned, t)!,
      plannedContainer: Color.lerp(
        plannedContainer,
        other.plannedContainer,
        t,
      )!,
      onPlannedContainer: Color.lerp(
        onPlannedContainer,
        other.onPlannedContainer,
        t,
      )!,
      cancelled: Color.lerp(cancelled, other.cancelled, t)!,
      cancelledContainer: Color.lerp(
        cancelledContainer,
        other.cancelledContainer,
        t,
      )!,
      onCancelledContainer: Color.lerp(
        onCancelledContainer,
        other.onCancelledContainer,
        t,
      )!,
      categorySlices: [
        for (var i = 0; i < categorySlices.length; i++)
          Color.lerp(categorySlices[i], other.categorySlice(i), t)!,
      ],
      categoryOtherSlice: Color.lerp(
        categoryOtherSlice,
        other.categoryOtherSlice,
        t,
      )!,
    );
  }
}
