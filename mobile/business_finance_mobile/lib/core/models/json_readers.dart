abstract final class JsonReaders {
  static String string(Map<String, dynamic> json, String key) {
    final value = json[key];
    if (value is! String || value.trim().isEmpty) {
      throw FormatException('Missing or invalid $key.');
    }
    return value;
  }

  static String? nullableString(Map<String, dynamic> json, String key) {
    final value = json[key];
    if (value == null) {
      return null;
    }
    if (value is! String) {
      throw FormatException('Invalid $key.');
    }
    return value;
  }

  static String money(Map<String, dynamic> json, String key) {
    final value = string(json, key);
    if (!RegExp(r'^-?\d+\.\d{4}$').hasMatch(value)) {
      throw FormatException('Invalid money value at $key.');
    }
    return value;
  }

  static String date(Map<String, dynamic> json, String key) {
    final value = string(json, key);
    if (!RegExp(r'^\d{4}-\d{2}-\d{2}$').hasMatch(value) ||
        DateTime.tryParse(value) == null) {
      throw FormatException('Invalid date value at $key.');
    }
    return value;
  }

  static int integer(Map<String, dynamic> json, String key) {
    final value = json[key];
    if (value is! int) {
      throw FormatException('Missing or invalid $key.');
    }
    return value;
  }

  /// Alan yoksa ya da `null` ise `null`; **yanlış tipte ise hata**.
  ///
  /// Sessizce `null`'a düşmek, sözleşme kaymasını gizlerdi: sunucu string
  /// göndermeye başlasa istemci "değer yok" der ve kimse fark etmezdi.
  static int? nullableInt(Map<String, dynamic> json, String key) {
    final value = json[key];
    if (value == null) return null;
    if (value is! int) {
      throw FormatException('Invalid $key.');
    }
    return value;
  }

  static bool boolean(Map<String, dynamic> json, String key) {
    final value = json[key];
    if (value is! bool) {
      throw FormatException('Missing or invalid $key.');
    }
    return value;
  }

  static List<dynamic> list(Map<String, dynamic> json, String key) {
    final value = json[key];
    if (value is! List<dynamic>) {
      throw FormatException('Missing or invalid $key.');
    }
    return value;
  }

  static Map<String, dynamic> object(Object? value, String key) {
    if (value is! Map<String, dynamic>) {
      throw FormatException('Missing or invalid $key.');
    }
    return value;
  }
}
