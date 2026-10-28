import 'package:flutter/material.dart';

/// Kategorinin simgesi, adından.
///
/// Sunucu simge göndermez; varsayılan setlerin adları (`DefaultCategorySets`)
/// burada bir simgeye bağlanır. Kullanıcının açtığı ya da adını değiştirdiği
/// kategori bulunamaz ve etiket simgesiyle çizilir (teslim: `local_offer`).
/// Ad, ad tekliğindeki gibi yalnız harf ve rakamlarıyla, dört i harfi tek
/// harf sayılarak eşlenir.
IconData categoryIcon(String name) =>
    _icons[categoryNameKey(name)] ?? Icons.local_offer_outlined;

/// Ad tekliğinin anahtarı (`NameKeys`): harf büyüklüğü, boşluk ve noktalama
/// fark değildir; `İ`, `I`, `ı`, `i` aynı harftir.
String categoryNameKey(String name) {
  final buffer = StringBuffer();
  for (final rune in name.runes) {
    final char = String.fromCharCode(rune);
    final lower = switch (char) {
      'İ' || 'I' || 'ı' => 'i',
      _ => char.toLowerCase(),
    };
    if (RegExp(r'[\p{L}\p{N}]', unicode: true).hasMatch(lower)) {
      buffer.write(lower);
    }
  }
  return buffer.toString();
}

final Map<String, IconData> _icons = {
  for (final entry in const <(String, IconData)>[
    // İşletme gideri
    ('Ticari mal alımı', Icons.inventory_2_outlined),
    ('Ambalaj ve sarf malzemesi', Icons.inventory_outlined),
    ('İşyeri faturaları', Icons.receipt_long_outlined),
    ('İşyeri kirası', Icons.store_outlined),
    ('Personel giderleri', Icons.badge_outlined),
    ('Araç ve yakıt', Icons.local_gas_station_outlined),
    ('Bakım-onarım', Icons.build_outlined),
    ('Banka ve POS komisyonu', Icons.percent_outlined),
    ('Banka masrafı', Icons.account_balance_outlined),
    ('İletişim', Icons.call_outlined),
    ('Reklam', Icons.campaign_outlined),
    ('Nakliye ve kargo', Icons.local_shipping_outlined),
    ('Muhasebeci ve danışmanlık', Icons.calculate_outlined),
    ('Vergi ve harç', Icons.gavel_outlined),
    ('SGK ve vergi ödemesi', Icons.gavel_outlined),
    ('Sigorta', Icons.shield_outlined),
    ('Faiz ve finansman gideri', Icons.request_quote_outlined),
    ('Diğer işletme gideri', Icons.local_offer_outlined),
    // Şahsi gider
    ('Yeme-içme', Icons.restaurant_outlined),
    ('Market Alışverişi', Icons.shopping_basket_outlined),
    ('Market', Icons.shopping_basket_outlined),
    ('Ev faturaları', Icons.receipt_long_outlined),
    ('Faturalar', Icons.receipt_long_outlined),
    ('Ev kirası ve aidat', Icons.home_outlined),
    ('Konut', Icons.home_outlined),
    ('Ulaşım', Icons.directions_bus_outlined),
    ('Yakıt', Icons.local_gas_station_outlined),
    ('Sağlık', Icons.local_hospital_outlined),
    ('Eğitim', Icons.school_outlined),
    ('Eğlence', Icons.theater_comedy_outlined),
    ('Giyim', Icons.checkroom_outlined),
    ('Abonelikler', Icons.subscriptions_outlined),
    ('Ev eşyası', Icons.chair_outlined),
    ('Kişisel bakım', Icons.spa_outlined),
    ('Evcil hayvan', Icons.pets_outlined),
    ('Hediye', Icons.card_giftcard_outlined),
    ('Bağış', Icons.volunteer_activism_outlined),
    // Gelir
    ('Satış geliri', Icons.sell_outlined),
    ('Satış geliri (kart)', Icons.sell_outlined),
    ('Hizmet geliri', Icons.handyman_outlined),
    ('Kira geliri', Icons.home_outlined),
    ('Faiz geliri', Icons.savings_outlined),
    ('Maaş', Icons.payments_outlined),
    ('Ek iş', Icons.work_outline),
    ('Temettü', Icons.trending_up),
    ('Yatırım getirisi', Icons.trending_up),
    ('Borsa', Icons.show_chart),
    ('Kripto', Icons.currency_bitcoin),
    ('İade ve geri ödeme', Icons.replay),
    ('Diğer işletme geliri', Icons.local_offer_outlined),
  ])
    categoryNameKey(entry.$1): entry.$2,
};
