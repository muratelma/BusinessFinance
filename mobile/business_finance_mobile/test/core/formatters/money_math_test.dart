import 'package:business_finance_mobile/core/formatters/money_math.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test('tutar 10⁴ ölçekli tam sayıdır; double kaybı yoktur', () {
    final a = MoneyMath.parse('0.1000')!;
    final b = MoneyMath.parse('0.2000')!;
    expect(MoneyMath.wire(a + b), '0.3000');
    expect(MoneyMath.wire(MoneyMath.parse('-120.0000')!.abs()), '120.0000');
    expect(
      MoneyMath.wire(MoneyMath.parse('123456789012345.6789')!),
      '123456789012345.6789',
    );
    expect(
      MoneyMath.wire(MoneyMath.lira(200) * BigInt.from(100)),
      '20000.0000',
    );
    expect(MoneyMath.parse('12,5'), isNull);
  });

  test('Türkçe girdi okunur: binlik nokta, ondalık virgül', () {
    expect(MoneyMath.fromInput('23.100'), '23100.0000');
    expect(MoneyMath.fromInput('23.100,5'), '23100.5000');
    expect(MoneyMath.fromInput('23100.5'), '23100.5000');
    expect(MoneyMath.fromInput('0'), '0.0000');
    expect(MoneyMath.fromInput(''), isNull);
    expect(MoneyMath.fromInput('-5'), isNull);
  });

  test('biçimlendirici yazarken binlik ayırır', () {
    const formatter = TurkishAmountInputFormatter();
    TextEditingValue type(String text) => formatter.formatEditUpdate(
      TextEditingValue.empty,
      TextEditingValue(text: text),
    );
    expect(type('23100').text, '23.100');
    expect(type('1234567,25').text, '1.234.567,25');
    expect(type('007').text, '7');
    expect(type('12a').text, '');
  });
}
