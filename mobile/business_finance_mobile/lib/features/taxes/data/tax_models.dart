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
