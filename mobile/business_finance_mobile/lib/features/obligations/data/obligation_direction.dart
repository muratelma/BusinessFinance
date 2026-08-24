/// Tek seferlik yükümlülüğün yönü.
///
/// Yön kategorinin türünü belirler: tahsil edilecek bir alacak gelir, ödenecek
/// bir fatura gider tanır. Sunucu da aynı kuralı uygular ve yönle çelişen
/// kategoriyi reddeder; form bu yüzden listeyi yöne göre okur, kullanıcıyı
/// reddedilecek bir seçime bırakmaz.
enum ObligationDirection {
  payable('payable', 'Ödenecek', 'expense'),
  receivable('receivable', 'Tahsil edilecek', 'income');

  const ObligationDirection(this.apiValue, this.label, this.categoryType);

  /// API sözleşmesinin kararlı makine değeri.
  final String apiValue;

  /// Kullanıcıya gösterilen cümle istemcide üretilir.
  final String label;

  /// Bu yönün kabul ettiği kategori türü.
  final String categoryType;
}
