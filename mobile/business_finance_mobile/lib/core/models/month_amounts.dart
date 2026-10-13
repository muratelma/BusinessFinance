import 'json_readers.dart';

class FinanceMonthValue {
  const FinanceMonthValue({
    required this.month,
    required this.amount,
    required this.barRatio,
    required this.enabled,
  });
  final int month;
  final String amount;
  final double barRatio;
  final bool enabled;
  factory FinanceMonthValue.fromJson(Map<String, dynamic> json) =>
      FinanceMonthValue(
        month: JsonReaders.integer(json, 'month'),
        amount: JsonReaders.money(json, 'amount'),
        barRatio: double.parse(JsonReaders.money(json, 'barRatio')),
        enabled: JsonReaders.boolean(json, 'enabled'),
      );
}

class FinanceYearMonths {
  const FinanceYearMonths({required this.year, required this.months});
  final int year;
  final List<FinanceMonthValue> months;
  factory FinanceYearMonths.fromJson(Map<String, dynamic> json) =>
      FinanceYearMonths(
        year: JsonReaders.integer(json, 'year'),
        months: JsonReaders.list(json, 'months')
            .map(
              (v) => FinanceMonthValue.fromJson(JsonReaders.object(v, 'month')),
            )
            .toList(growable: false),
      );
}
