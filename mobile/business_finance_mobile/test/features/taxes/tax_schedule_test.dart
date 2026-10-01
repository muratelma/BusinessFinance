import 'package:business_finance_mobile/features/taxes/data/tax_models.dart';
import 'package:business_finance_mobile/features/taxes/presentation/tax_schedule.dart';
import 'package:flutter_test/flutter_test.dart';

/// Vergi ritminin istemci metinleri ve başlangıç seçenekleri. Tarihlerin
/// sahibi sunucudur; burada sınanan şey formun ve listenin **önizlemesidir**.
void main() {
  group('gün etiketi', () {
    test('31 ay sonudur', () {
      expect(TaxSchedule.dayLabel(31), 'ay sonu');
    });

    test('Türkçe sıra eki sayının okunuşuna uyar', () {
      expect(
        [
          1,
          2,
          3,
          4,
          5,
          6,
          7,
          8,
          9,
          10,
          17,
          20,
          26,
          28,
          30,
        ].map(TaxSchedule.dayLabel),
        [
          "1'i",
          "2'si",
          "3'ü",
          "4'ü",
          "5'i",
          "6'sı",
          "7'si",
          "8'i",
          "9'u",
          "10'u",
          "17'si",
          "20'si",
          "26'sı",
          "28'i",
          "30'u",
        ],
      );
    });
  });

  group('ritim etiketi', () {
    test('her ay', () {
      expect(
        TaxSchedule.rhythmLabel(
          rhythm: TaxRhythm.monthly,
          day: 31,
          months: const [],
          startDate: '2026-09-30',
        ),
        'Her ay · ay sonu',
      );
    });

    test('iki ay tam adla, fazlası kısa adla yazılır', () {
      expect(
        TaxSchedule.rhythmLabel(
          rhythm: TaxRhythm.selectedMonths,
          day: 31,
          months: const [7, 1],
          startDate: '2027-01-31',
        ),
        'Ocak, Temmuz · ay sonu',
      );
      expect(
        TaxSchedule.rhythmLabel(
          rhythm: TaxRhythm.selectedMonths,
          day: 17,
          months: const [2, 5, 8, 11],
          startDate: '2026-11-17',
        ),
        "Şub · May · Ağu · Kas · 17'si",
      );
    });

    test('yılda bir başlangıç ayını yazar', () {
      expect(
        TaxSchedule.rhythmLabel(
          rhythm: TaxRhythm.yearly,
          day: 31,
          months: const [],
          startDate: '2027-01-31',
        ),
        'Yılda bir · Ocak sonu',
      );
    });
  });

  group('başlangıç seçenekleri', () {
    test('ay sonu kısa ayda son güne iner', () {
      final dates = TaxSchedule.candidates(
        rhythm: TaxRhythm.monthly,
        day: 31,
        months: const {},
        from: DateTime(2026, 9, 29),
        count: 3,
      );

      expect(dates, [
        DateTime(2026, 9, 30),
        DateTime(2026, 10, 31),
        DateTime(2026, 11, 30),
      ]);
    });

    test('seçilen aylarda yalnız o ayları önerir ve bugünü geçmez', () {
      final dates = TaxSchedule.candidates(
        rhythm: TaxRhythm.selectedMonths,
        day: 17,
        months: const {2, 5, 8, 11},
        from: DateTime(2026, 9, 29),
        count: 2,
      );

      expect(dates, [DateTime(2026, 11, 17), DateTime(2027, 2, 17)]);
    });

    test('sıradaki iki vade ritmi izler', () {
      final following = TaxSchedule.following(
        rhythm: TaxRhythm.selectedMonths,
        day: 31,
        months: const {1, 7},
        start: DateTime(2027, 1, 31),
      );

      expect(following, [DateTime(2027, 1, 31), DateTime(2027, 7, 31)]);
    });
  });

  test('göreli vade', () {
    final today = DateTime(2026, 9, 29, 23, 30);

    expect(TaxSchedule.relative('2026-09-28', today), '1 gün gecikti');
    expect(TaxSchedule.relative('2026-09-29', today), 'bugün');
    expect(TaxSchedule.relative('2026-09-30', today), 'yarın');
    expect(TaxSchedule.relative('2026-10-28', today), '29 gün');
  });
}
