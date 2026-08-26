import '../../../core/models/json_readers.dart';

/// Kurulmaya hazır takvim kalemi.
///
/// Sunucu **tutar göndermez** ve kullanıcıya gösterilecek cümleyi de göndermez:
/// kararlı makine değerleri gelir, adı ve açıklamayı istemci kurar (ADR 0016).
class TaxCalendarSuggestion {
  const TaxCalendarSuggestion({
    required this.key,
    required this.frequency,
    required this.suggestedDayOfMonth,
    required this.suggestedCategoryName,
    required this.kind,
    required this.scope,
  });

  factory TaxCalendarSuggestion.fromJson(Map<String, dynamic> json) =>
      TaxCalendarSuggestion(
        key: JsonReaders.string(json, 'key'),
        frequency: JsonReaders.string(json, 'frequency'),
        suggestedDayOfMonth: JsonReaders.integer(json, 'suggestedDayOfMonth'),
        suggestedCategoryName: JsonReaders.string(
          json,
          'suggestedCategoryName',
        ),
        kind: JsonReaders.string(json, 'kind'),
        scope: JsonReaders.string(json, 'scope'),
      );

  final String key;
  final String frequency;
  final int suggestedDayOfMonth;
  final String suggestedCategoryName;
  final String kind;
  final String scope;

  /// Ekranda görünen ad. API göndermez; kullanıcı cümlesi istemcinin işidir.
  String get label => switch (key) {
    'vat-return' => 'KDV beyanı',
    'withholding-return' => 'Muhtasar beyanı',
    'social-security-premium' => 'SGK / Bağkur primi',
    'advance-tax' => 'Geçici vergi',
    _ => key,
  };

  String get frequencyLabel => switch (frequency) {
    'monthly' => 'Her ay',
    'quarterly' => 'Üç ayda bir',
    'yearly' => 'Yılda bir',
    'weekly' => 'Her hafta',
    'daily' => 'Her gün',
    _ => frequency,
  };

  /// Önerilen günü kullanıcının anlayacağı bir cümleye çevirir.
  String get scheduleLabel =>
      '$frequencyLabel • ayın $suggestedDayOfMonth. günü';
}

/// Ay sonu muhasebeci paketinin önizlemesi.
///
/// Bütün tutarlar sunucudan gelir; istemci hiçbirini yeniden hesaplamaz.
class AccountantPackage {
  const AccountantPackage({
    required this.year,
    required this.month,
    required this.currency,
    required this.totalIncome,
    required this.totalExpense,
    required this.net,
    required this.vatOnIncome,
    required this.vatOnExpense,
    required this.linesWithoutVat,
    required this.nonDeductibleExpense,
    required this.nonDeductibleCount,
    required this.deductibilityUnansweredCount,
    required this.lineCount,
    required this.attachmentCount,
    required this.omittedAttachmentCount,
  });

  factory AccountantPackage.fromJson(Map<String, dynamic> json) {
    final attachments = (json['attachments'] as List<dynamic>? ?? const [])
        .cast<Map<String, dynamic>>();
    return AccountantPackage(
      year: JsonReaders.integer(json, 'year'),
      month: JsonReaders.integer(json, 'month'),
      currency: JsonReaders.string(json, 'currency'),
      totalIncome: JsonReaders.string(json, 'totalIncome'),
      totalExpense: JsonReaders.string(json, 'totalExpense'),
      net: JsonReaders.string(json, 'net'),
      vatOnIncome: JsonReaders.string(json, 'vatOnIncome'),
      vatOnExpense: JsonReaders.string(json, 'vatOnExpense'),
      linesWithoutVat: JsonReaders.integer(json, 'linesWithoutVat'),
      nonDeductibleExpense: JsonReaders.string(json, 'nonDeductibleExpense'),
      nonDeductibleCount: JsonReaders.integer(json, 'nonDeductibleCount'),
      deductibilityUnansweredCount: JsonReaders.integer(
        json,
        'deductibilityUnansweredCount',
      ),
      lineCount: (json['lines'] as List<dynamic>? ?? const []).length,
      attachmentCount: attachments.length,
      omittedAttachmentCount: attachments
          .where((item) => item['isIncluded'] == false)
          .length,
    );
  }

  final int year;
  final int month;
  final String currency;
  final String totalIncome;
  final String totalExpense;
  final String net;
  final String vatOnIncome;
  final String vatOnExpense;
  final int linesWithoutVat;
  final String nonDeductibleExpense;
  final int nonDeductibleCount;
  final int deductibilityUnansweredCount;
  final int lineCount;
  final int attachmentCount;

  /// Boyut tavanını aştığı için dosyası pakete konmayan ek sayısı.
  final int omittedAttachmentCount;

  bool get isEmpty => lineCount == 0;

  String get periodLabel {
    const months = [
      'Ocak',
      'Şubat',
      'Mart',
      'Nisan',
      'Mayıs',
      'Haziran',
      'Temmuz',
      'Ağustos',
      'Eylül',
      'Ekim',
      'Kasım',
      'Aralık',
    ];
    return '${months[month - 1]} $year';
  }
}
