import '../localization/default_category_labels.dart';
import 'json_readers.dart';

/// Bir açılır listede seçilebilen kayıt: hesap, kategori ya da kart.
///
/// Ekranların çoğu bir hesap veya kategori seçtiriyor ve hepsinin ihtiyacı
/// aynı üç alan. Her özellik kendi kopyasını taşısaydı, "kategori adını
/// yerelleştir" gibi bir kural birinde düzeltilip diğerlerinde unutulurdu.
class DataChoice {
  const DataChoice(this.id, this.name, {this.type});

  factory DataChoice.fromJson(Map<String, dynamic> json) => DataChoice(
    JsonReaders.string(json, 'id'),
    JsonReaders.string(json, 'name'),
    type: JsonReaders.nullableString(json, 'type'),
  );

  /// Kategori seçimi: yalnız eski varsayılan İngilizce adlar Türkçeye çevrilir.
  factory DataChoice.categoryFromJson(Map<String, dynamic> json) => DataChoice(
    JsonReaders.string(json, 'id'),
    DefaultCategoryLabels.localized(JsonReaders.string(json, 'name')),
    type: JsonReaders.nullableString(json, 'type'),
  );

  final String id;
  final String name;

  /// Kategoride `income` / `expense`, hesapta hesap türü; yoksa `null`.
  final String? type;
}
