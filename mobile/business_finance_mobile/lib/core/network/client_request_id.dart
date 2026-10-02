import 'dart:math';

/// Yazma isteğini idempotent yapan kimlik (UUID v4 biçiminde).
///
/// Form açıldığında **bir kez** üretilir ve o formun bütün gönderimlerinde
/// aynı kalır: ağ kopup istek yinelenirse sunucu ikinci bir kayıt yazmaz,
/// ilkini döner.
String newClientRequestId() {
  final random = Random.secure();
  String hex(int length) =>
      List.generate(length, (_) => random.nextInt(16).toRadixString(16)).join();
  return '${hex(8)}-${hex(4)}-4${hex(3)}-'
      '${['8', '9', 'a', 'b'][random.nextInt(4)]}${hex(3)}-${hex(12)}';
}
