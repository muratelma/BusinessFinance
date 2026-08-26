/// Vergi takviminden gelen hazır kalemin, tekrarlayan plan formunu önceden
/// dolduran alanları.
///
/// Tutar **yok**: öneri tutar taşımaz ve bir sayı önermek, hesaplanmış bir
/// vergi tutarı iddia etmek olurdu (ADR 0016). Kategori kimlikle değil **adla**
/// taşınır; kimlik istemcide üretilemez ve ad bulunamazsa alan boş kalır.
class RecurringPrefill {
  const RecurringPrefill({
    required this.description,
    required this.frequency,
    required this.startDate,
    required this.categoryName,
    this.kind = 'bill-payment',
  });

  /// Sunucunun gönderdiği makine değerlerinden bir öneri kurar.
  ///
  /// Başlangıç günü, önerilen ayın gününün **bugünden sonraki ilk** düşüşüdür;
  /// geçmişe kurmak, ilk gerçekleşmeyi daha kurulurken gecikmiş yapardı.
  factory RecurringPrefill.fromSuggestion({
    required String label,
    required String frequency,
    required int dayOfMonth,
    required String categoryName,
    DateTime? today,
  }) {
    final now = today ?? DateTime.now();
    final day = dayOfMonth.clamp(1, 28);
    final thisMonth = DateTime(now.year, now.month, day);
    final start = thisMonth.isBefore(DateTime(now.year, now.month, now.day))
        ? DateTime(now.year, now.month + 1, day)
        : thisMonth;
    return RecurringPrefill(
      description: label,
      frequency: frequency,
      startDate: start,
      categoryName: categoryName,
    );
  }

  final String description;
  final String frequency;
  final DateTime startDate;
  final String categoryName;
  final String kind;
}
