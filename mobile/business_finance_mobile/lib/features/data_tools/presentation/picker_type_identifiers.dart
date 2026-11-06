/// Dosya seçicinin uzantılarının Apple tür kimlikleri.
///
/// iOS'taki seçici türü uzantıyla değil yalnız bu kimliklerle süzer; kimliği
/// olmayan bir tür grubu verildiğinde seçici açılmadan hata fırlatır. Android
/// ve web uzantıyı okur, kimlikleri yok sayar.
const Map<String, String> _identifierByExtension = {
  'csv': 'public.comma-separated-values-text',
  'jpeg': 'public.jpeg',
  'jpg': 'public.jpeg',
  'json': 'public.json',
  'pdf': 'com.adobe.pdf',
  'png': 'public.png',
};

List<String> uniformTypeIdentifiersFor(List<String> extensions) {
  final identifiers = <String>[];
  for (final extension in extensions) {
    final identifier = _identifierByExtension[extension.toLowerCase()];
    if (identifier == null) {
      throw ArgumentError.value(
        extension,
        'extensions',
        'Bu uzantının Apple tür kimliği tanımlı değil.',
      );
    }
    if (!identifiers.contains(identifier)) identifiers.add(identifier);
  }
  return identifiers;
}
