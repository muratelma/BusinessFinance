import 'package:flutter_test/flutter_test.dart';

import 'package:business_finance_mobile/core/network/api_error_messages.dart';

/// Sunucu hata kodlarının kullanıcı cümlesine çevrilmesi.
///
/// Kapının kendisi son testtedir: **hiçbir kod İngilizce bir cümleye
/// dönmemeli** ve karşılığı olmayan kod nötr cümleye düşmeli.
void main() {
  group('tam kod katmanı', () {
    test('kendi cümlesi olan kod o cümleyi döner', () {
      expect(
        ApiErrorMessages.resolve('authentication.invalid_credentials'),
        'E-posta veya parola geçersiz.',
      );
      expect(
        ApiErrorMessages.resolve('credit_cards.limit_exceeded'),
        contains('kullanılabilir limitini aşıyor'),
      );
    });

    test('iptal kilidi kullanıcıya ne yapacağını söyler', () {
      expect(
        ApiErrorMessages.resolve('transactions.cancel_origin_locked'),
        contains('Planı üzerinden düzenleyin'),
      );
      expect(
        ApiErrorMessages.resolve('credit_card_charges.cancel_origin_locked'),
        contains('Planı üzerinden düzenleyin'),
      );
    });

    test('tam kod, kalıbı ezer', () {
      // `accounts.in_use` kalıptan da bir cümle üretebilirdi; tam kod
      // pasife alma yolunu da söylediği için o kazanır.
      expect(
        ApiErrorMessages.resolve('accounts.in_use'),
        contains('Pasife alabilirsiniz'),
      );
    });
  });

  group('kalıp katmanı', () {
    test('not_found alanın Türkçe adıyla kurulur', () {
      expect(
        ApiErrorMessages.resolve('transactions.not_found'),
        startsWith('İşlem bulunamadı'),
      );
      expect(
        ApiErrorMessages.resolve('counterparties.not_found'),
        startsWith('Cari hesap bulunamadı'),
      );
      // Aşama 06.1 Grup 3: taksit gerçekleştirme bulunamayan plan için 400
      // dönüyordu; 404'e çevrildi ve kodu `installments.not_found` oldu.
      expect(
        ApiErrorMessages.resolve('installments.not_found'),
        startsWith('Taksit planı bulunamadı'),
      );
    });

    test('kabul turunda görülen scope_unresolved artık Türkçe', () {
      // 27 Ağustos 2026 kabul turu: ekranda sunucunun İngilizce cümlesi
      // (`The scope could not be resolved …`) görünmüştü.
      for (final code in const [
        'budgets.scope_unresolved',
        'transactions.scope_unresolved',
        'debt.scope_unresolved',
        'credit_cards.scope_unresolved',
      ]) {
        final message = ApiErrorMessages.resolve(code);
        expect(message, contains('işletme mi şahsi mi'));
        expect(message, isNot(contains('scope')));
      }
    });

    test('bilinmeyen alan da paylaşılan sebepten cümle alır', () {
      // Alan sözlüğünde olmayan bir alan bile sebebi biliniyorsa
      // nötre düşmez.
      expect(
        ApiErrorMessages.resolve('brand_new_feature.scope_unresolved'),
        contains('işletme mi şahsi mi'),
      );
    });

    test('invalid_<alan> alan adını yazar', () {
      expect(
        ApiErrorMessages.resolve('transactions.invalid_amount'),
        'Girilen tutar geçersiz. Kontrol edip tekrar deneyin.',
      );
      expect(
        ApiErrorMessages.resolve('pos_settlements.invalid_commission_rate'),
        contains('komisyon oranı'),
      );
    });

    test('alanı bilinen ama sebebi bilinmeyen invalid_* özneye düşer', () {
      expect(
        ApiErrorMessages.resolve('accounts.invalid_pagination'),
        startsWith('Hesap bilgilerinde geçersiz bir alan var'),
      );
    });
  });

  group('nötr yedek', () {
    test('hiç tanınmayan kod nötr cümleye düşer', () {
      expect(
        ApiErrorMessages.resolve('quantum.flux_capacitor_offline'),
        ApiErrorMessages.neutral,
      );
    });

    test('biçimsiz kod da nötr cümleye düşer', () {
      for (final code in const ['', '.', 'nokta_yok', 'trailing.']) {
        expect(ApiErrorMessages.resolve(code), ApiErrorMessages.neutral);
      }
    });
  });

  // Ad tekliği beş kayıt türünde aynı kuraldır (9 Ekim 2026); POS'un kodu
  // yenidir ve var olan genel cümleyle konuşur.
  test('aynı ad reddi her kayıt türünde Türkçe konuşur', () {
    expect(
      ApiErrorMessages.resolve('pos_definitions.duplicate_name'),
      'Bu adı taşıyan bir POS kaydı zaten var. Farklı bir ad seçin.',
    );
    for (final code in [
      'accounts.duplicate_name',
      'credit_cards.duplicate_name',
      'categories.duplicate_name',
      'counterparties.duplicate_name',
    ]) {
      expect(ApiErrorMessages.resolve(code), contains('zaten var'));
    }
  });

  test('hiçbir cevap İngilizce bir cümleye dönmez', () {
    // Backend'in bugün ürettiği kodlardan bir kesit; hepsi Türkçe konuşmalı.
    const codes = [
      'accounts.not_found',
      'accounts.duplicate_name',
      'budgets.duplicate_period',
      'cash_counts.nothing_to_adjust',
      'counterparties.inactive',
      'counterparties.has_history',
      'debt.repayment_required',
      'goal.has_contributions',
      'imports.mapping_unavailable',
      'obligations.category_unavailable',
      'pos_settlements.commission_ambiguous',
      'recurring.schedule_inactive',
      'recurring.income_card_source_not_supported',
      'restore.destination_not_empty',
      'receipt.unreadable',
      'transfers.account_unavailable',
      'server.unexpected_error',
    ];

    const englishGiveaways = [
      'The ',
      'A ',
      'is required',
      'not found',
      'cannot',
      'invalid',
      'scope',
    ];

    for (final code in codes) {
      final message = ApiErrorMessages.resolve(code);
      expect(message, isNotEmpty, reason: code);
      for (final giveaway in englishGiveaways) {
        expect(message, isNot(contains(giveaway)), reason: '$code → $message');
      }
    }
  });
}
