import 'package:business_finance_mobile/features/data_tools/presentation/picker_type_identifiers.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test('every picker on the data tools page has an Apple type identifier', () {
    expect(uniformTypeIdentifiersFor(['json']), ['public.json']);
    expect(uniformTypeIdentifiersFor(['csv']), [
      'public.comma-separated-values-text',
    ]);
    expect(uniformTypeIdentifiersFor(['pdf', 'jpg', 'jpeg', 'png']), [
      'com.adobe.pdf',
      'public.jpeg',
      'public.png',
    ]);
  });

  test('an extension without an identifier fails instead of opening blind', () {
    expect(() => uniformTypeIdentifiersFor(['xlsx']), throwsArgumentError);
  });
}
