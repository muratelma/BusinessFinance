import 'package:flutter/material.dart';

/// Hesap ve kategori ikonları.
///
/// Her hesabın cüzdan, her kategorinin aynı pasta dilimi ikonunu taşıması
/// listeyi okunmaz yapıyordu: göz ikonu tarayarak satır ayırt edemiyor, ikon
/// yalnız yer kaplıyordu. İkon ancak **ayırt ediyorsa** bilgi taşır.
abstract final class AppFinanceIcons {
  /// Hesap türüne göre ikon. Tür sunucudan gelir (`cash` / `bank`), addan
  /// tahmin edilmez.
  static IconData forAccountType(String? type) => switch (type) {
    'cash' => Icons.payments_outlined,
    'bank' => Icons.account_balance_outlined,
    // Sunucu ileride yeni bir tür eklerse satır ikonsuz kalmasın.
    _ => Icons.account_balance_wallet_outlined,
  };

  /// Kategori ikonu.
  ///
  /// Eşleme **sunucunun kanonik adı** üzerinden yapılır (`groceries`,
  /// `bills`, …), kullanıcıya gösterilen Türkçe etiket üzerinden değil:
  /// etiket çevirisi değişince ikonun düşmemesi gerekir.
  ///
  /// Kullanıcının kendi açtığı kategoriler bu listede yoktur; onlar için
  /// addaki anahtar kelimelere bakılır, o da tutmazsa nötr bir etiket ikonu
  /// kullanılır. Amaç her kategoriyi bilmek değil, sık kullanılanları
  /// birbirinden ayırmaktır.
  static IconData forCategory(String canonicalName, {String? displayName}) {
    final canonical = canonicalName.trim().toLowerCase();
    final known = _canonical[canonical];
    if (known != null) return known;

    final haystack = '${displayName ?? ''} $canonicalName'.toLowerCase();
    for (final entry in _keywords.entries) {
      if (haystack.contains(entry.key)) return entry.value;
    }
    return Icons.local_offer_outlined;
  }

  /// Sunucunun varsayılan kategorileri.
  static const Map<String, IconData> _canonical = {
    'salary': Icons.badge_outlined,
    'other income': Icons.savings_outlined,
    'groceries': Icons.shopping_basket_outlined,
    'housing': Icons.home_outlined,
    'transport': Icons.directions_bus_outlined,
    'bills': Icons.receipt_long_outlined,
    'health': Icons.medical_services_outlined,
    'entertainment': Icons.movie_outlined,
  };

  /// Kullanıcının kendi kategorileri için Türkçe/İngilizce anahtar kelimeler.
  /// Sıra önemlidir: ilk eşleşen kazanır.
  static const Map<String, IconData> _keywords = {
    // İşletme seti: esnafın en sık gider kalemleri.
    'ticari mal': Icons.sell_outlined,
    'personel': Icons.badge_outlined,
    'maaş': Icons.badge_outlined,
    'sgk': Icons.account_balance_outlined,
    'vergi': Icons.account_balance_outlined,
    'kırtasiye': Icons.edit_outlined,
    'market': Icons.shopping_basket_outlined,
    'alışveriş': Icons.shopping_bag_outlined,
    'giyim': Icons.checkroom_outlined,
    'kira': Icons.home_outlined,
    'konut': Icons.home_outlined,
    'ev ': Icons.home_outlined,
    'fatura': Icons.receipt_long_outlined,
    'elektrik': Icons.bolt_outlined,
    'su ': Icons.water_drop_outlined,
    'doğalgaz': Icons.local_fire_department_outlined,
    'internet': Icons.wifi_outlined,
    'telefon': Icons.smartphone_outlined,
    'abonelik': Icons.subscriptions_outlined,
    'ulaşım': Icons.directions_bus_outlined,
    'yakıt': Icons.local_gas_station_outlined,
    'benzin': Icons.local_gas_station_outlined,
    'araç': Icons.directions_car_outlined,
    'sağlık': Icons.medical_services_outlined,
    'eczane': Icons.medication_outlined,
    'spor': Icons.fitness_center_outlined,
    'eğitim': Icons.school_outlined,
    'okul': Icons.school_outlined,
    'kitap': Icons.menu_book_outlined,
    'eğlence': Icons.movie_outlined,
    'restoran': Icons.restaurant_outlined,
    'yemek': Icons.restaurant_outlined,
    'kafe': Icons.local_cafe_outlined,
    'tatil': Icons.beach_access_outlined,
    'seyahat': Icons.flight_outlined,
    'hediye': Icons.card_giftcard_outlined,
    'evcil': Icons.pets_outlined,
    'çocuk': Icons.child_care_outlined,
    'sigorta': Icons.shield_outlined,
    'gelir': Icons.savings_outlined,
    'yatırım': Icons.trending_up_outlined,
    'birikim': Icons.savings_outlined,
  };
}
