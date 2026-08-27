import 'package:business_finance_mobile/core/formatters/money_text.dart';
import 'package:flutter_test/flutter_test.dart';

/// Aşama 06 Grup 5: kart borcu negatif olabilir ve ekranda "eksi borç" diye
/// değil, "alacağınız var" diye okunur.
void main() {
  group('işaret okuması', () {
    test('negatif tutar tanınır, pozitif tanınmaz', () {
      expect(MoneyText.isNegative('-500.0000'), isTrue);
      expect(MoneyText.isNegative('500.0000'), isFalse);
      expect(MoneyText.isNegative('0.0000'), isFalse);
    });

    test('işaret atılınca tutarın kendisi bozulmaz', () {
      expect(MoneyText.unsigned('-500.1250'), '500.1250');
      expect(MoneyText.unsigned('500.1250'), '500.1250');
    });

    test('işaretsiz tutar ekranda eksi göstermez', () {
      // Cümle yönü kendisi söylediğinde eksi işareti ikinci bir olumsuzlama
      // olurdu: "−₺500 alacağınız var".
      expect(MoneyText.format('-500.0000', 'TRY'), '-₺500,00');
      expect(
        MoneyText.format(MoneyText.unsigned('-500.0000'), 'TRY'),
        '₺500,00',
      );
    });
  });
}
