import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/formatters/money_text.dart';

void main() {
  test('formats backend money strings without double conversion', () {
    expect(MoneyText.format('1250.5000', 'TRY'), '₺1.250,50');
    expect(MoneyText.format('1234567.8900', 'TRY'), '₺1.234.567,89');
  });

  group('gösterim daima iki basamak', () {
    test('dört basamak ikiye yuvarlanır, kırpılmaz', () {
      // Kırpma her kuruşu sistematik olarak aşağı çeker; uzun listelerde fark
      // birikir. Yarıda yukarı yuvarlanır.
      expect(MoneyText.format('12.3456', 'TRY'), '₺12,35');
      expect(MoneyText.format('12.3449', 'TRY'), '₺12,34');
      expect(MoneyText.format('12.3450', 'TRY'), '₺12,35');
    });

    test('anlamsız sıfırlar korunur, kuruş sütunu hizada kalır', () {
      // Tutarlarda `12` ile `12,00` aynı değil: ikincisi alt alta gelen
      // satırlarda kuruş sütununu hizalı tutar.
      expect(MoneyText.format('12.0000', 'TRY'), '₺12,00');
      expect(MoneyText.format('12.5000', 'TRY'), '₺12,50');
    });

    test('yuvarlama tam kısma taşabilir', () {
      expect(MoneyText.format('12.9960', 'TRY'), '₺13,00');
      expect(MoneyText.format('999.9950', 'TRY'), '₺1.000,00');
      expect(MoneyText.format('9.9999', 'TRY'), '₺10,00');
    });

    test('negatif tutarda işaret ve yuvarlama birlikte doğru', () {
      expect(MoneyText.format('-12.3456', 'TRY'), '-₺12,35');
      expect(MoneyText.format('-0.9990', 'TRY'), '-₺1,00');
    });

    test('çok büyük tutar double hassasiyetine düşmez', () {
      // Basamak üzerinden çalışıldığı için 19 haneli tutar da tam kalır;
      // `double` bu boyda tam sayıyı temsil edemezdi.
      expect(
        MoneyText.format('123456789012345.6789', 'TRY'),
        '₺123.456.789.012.345,68',
      );
    });

    test('beklenmeyen biçim olduğu gibi gösterilir', () {
      // Sunucu sözleşmeyi bozarsa yanlış bir sayı uydurmak yerine ham değer
      // görünür.
      expect(MoneyText.format('bilinmeyen', 'TRY'), 'bilinmeyen TRY');
    });
  });

  group('oranlar', () {
    test('iki basamağa yuvarlanır ve anlamsız sıfırlar atılır', () {
      // Oranda `25` ile `25,00` aynı şeyi söyler; tutarların aksine hizalanacak
      // bir kuruş sütunu yok.
      expect(MoneyText.percent('151.0780'), '151,08');
      expect(MoneyText.percent('25.0000'), '25');
      expect(MoneyText.percent('10.5000'), '10,5');
      expect(MoneyText.percent('0.0000'), '0');
    });
  });

  test('normalizes Turkish decimal input and rejects excess precision', () {
    expect(MoneyText.normalizeInput(' 12,50 '), '12.50');
    expect(MoneyText.normalizeInput('12.12345'), isNull);
    expect(MoneyText.normalizeInput('-12'), isNull);
  });
}
