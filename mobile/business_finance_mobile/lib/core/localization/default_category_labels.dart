abstract final class DefaultCategoryLabels {
  static String localized(String name) => switch (name.trim().toLowerCase()) {
    'salary' => 'Maaş',
    'other income' => 'Diğer Gelir',
    'groceries' => 'Market Alışverişi',
    'housing' => 'Konut',
    'transport' => 'Ulaşım',
    'bills' => 'Faturalar',
    'health' => 'Sağlık',
    'entertainment' => 'Eğlence',
    _ => name,
  };
}
