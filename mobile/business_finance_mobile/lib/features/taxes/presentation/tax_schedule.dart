import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../data/tax_models.dart';

/// Vergi ritminin istemcideki metinleri ve tarih önizlemesi.
///
/// Tarihlerin **sahibi sunucudur**; buradaki hesap yalnız formun "sıradaki"
/// önizlemesi ve başlangıç seçeneklerini kurmak içindir. Sunucu başlangıcın
/// ritme uymasını ister; seçenekler ritimden türetildiği için gönderilen tarih
/// her zaman ritme uyar.
abstract final class TaxSchedule {
  /// "Ay sonu" gün değeri.
  static const monthEnd = 31;

  static const shortMonths = [
    'Oca',
    'Şub',
    'Mar',
    'Nis',
    'May',
    'Haz',
    'Tem',
    'Ağu',
    'Eyl',
    'Eki',
    'Kas',
    'Ara',
  ];

  /// `31` → `ay sonu`; `17` → `17'si`.
  static String dayLabel(int day) =>
      day >= monthEnd ? 'ay sonu' : "$day'${_suffix(day)}";

  /// Günün sıra eki: `1'i`, `2'si`, `3'ü`, `6'sı`, `9'u`, `10'u`, `20'si`.
  static String _suffix(int day) {
    if (day % 10 == 0) return day == 20 ? 'si' : 'u';
    return switch (day % 10) {
      1 || 5 || 8 => 'i',
      2 || 7 => 'si',
      3 || 4 => 'ü',
      6 => 'sı',
      _ => 'u',
    };
  }

  /// Vergilerim satırı ve ayrıntıdaki ritim: `Her ay · ay sonu`,
  /// `Şub · May · Ağu · Kas · 17'si`, `Ocak, Temmuz · ay sonu`,
  /// `Yılda bir · Ocak sonu`.
  static String rhythmLabel({
    required TaxRhythm rhythm,
    required int day,
    required List<int> months,
    required String startDate,
  }) {
    final startMonth = DateTime.parse(startDate).month;
    return switch (rhythm) {
      TaxRhythm.monthly => 'Her ay · ${dayLabel(day)}',
      TaxRhythm.quarterly => 'Üç ayda bir · ${dayLabel(day)}',
      TaxRhythm.yearly =>
        day >= monthEnd
            ? 'Yılda bir · ${DateText.months[startMonth - 1]} sonu'
            : 'Yılda bir · $day ${DateText.months[startMonth - 1]}',
      TaxRhythm.selectedMonths => '${_monthList(months)} · ${dayLabel(day)}',
    };
  }

  static String planRhythm(TaxPlan plan) => rhythmLabel(
    rhythm: plan.rhythm,
    day: plan.effectiveDay,
    months: plan.months,
    startDate: plan.startDate,
  );

  /// İki aya kadar tam ad (`Ocak, Temmuz`), fazlası kısa (`Şub · May · …`).
  static String _monthList(List<int> months) {
    final sorted = [...months]..sort();
    return sorted.length <= 2
        ? sorted.map((month) => DateText.months[month - 1]).join(', ')
        : sorted.map((month) => shortMonths[month - 1]).join(' · ');
  }

  /// `30 Eyl`.
  static String shortDate(String isoDate) {
    final date = DateTime.parse(isoDate);
    return '${date.day} ${shortMonths[date.month - 1]}';
  }

  /// Ritmin `from` gününden (dahil) başlayan ilk [count] tarihi.
  ///
  /// Üç ayda bir ve yılda bir ritminde başlangıç ayı ritmi belirler; seçenek
  /// her ay olabilir. Seçilen aylarda ve yılda birde yalnız seçili aylar.
  static List<DateTime> candidates({
    required TaxRhythm rhythm,
    required int day,
    required Set<int> months,
    required DateTime from,
    int count = 12,
  }) {
    final start = DateUtils.dateOnly(from);
    final result = <DateTime>[];
    for (var offset = 0; offset < 60 && result.length < count; offset++) {
      final month = DateTime(start.year, start.month + offset);
      final allowed = switch (rhythm) {
        TaxRhythm.monthly || TaxRhythm.quarterly => true,
        TaxRhythm.selectedMonths ||
        TaxRhythm.yearly => months.contains(month.month),
      };
      if (!allowed) continue;
      final date = onDay(month.year, month.month, day);
      if (!date.isBefore(start)) result.add(date);
    }
    return result;
  }

  /// Başlangıçtan itibaren ritmin ilk [count] kalemi (başlangıç dahil).
  static List<DateTime> following({
    required TaxRhythm rhythm,
    required int day,
    required Set<int> months,
    required DateTime start,
    int count = 2,
  }) {
    final result = <DateTime>[DateUtils.dateOnly(start)];
    var cursor = DateTime(start.year, start.month);
    while (result.length < count) {
      cursor = switch (rhythm) {
        TaxRhythm.monthly => DateTime(cursor.year, cursor.month + 1),
        TaxRhythm.quarterly => DateTime(cursor.year, cursor.month + 3),
        TaxRhythm.yearly => DateTime(cursor.year + 1, cursor.month),
        TaxRhythm.selectedMonths => _nextSelected(cursor, months),
      };
      result.add(onDay(cursor.year, cursor.month, day));
    }
    return result;
  }

  static DateTime _nextSelected(DateTime cursor, Set<int> months) {
    var next = DateTime(cursor.year, cursor.month + 1);
    for (var guard = 0; guard < 12 && !months.contains(next.month); guard++) {
      next = DateTime(next.year, next.month + 1);
    }
    return next;
  }

  /// Ayın günü; kısa ayda ayın son gününe iner (`31` ay sonudur).
  static DateTime onDay(int year, int month, int day) {
    final last = DateUtils.getDaysInMonth(year, month);
    return DateTime(year, month, day > last ? last : day);
  }

  static String iso(DateTime value) =>
      '${value.year.toString().padLeft(4, '0')}-'
      '${value.month.toString().padLeft(2, '0')}-'
      '${value.day.toString().padLeft(2, '0')}';

  /// Vadenin bugüne göre metni: `1 gün gecikti`, `bugün`, `yarın`, `29 gün`.
  static String relative(String dueDate, DateTime today) {
    final days = daysUntil(dueDate, today);
    if (days < 0) return '${-days} gün gecikti';
    return switch (days) {
      0 => 'bugün',
      1 => 'yarın',
      _ => '$days gün',
    };
  }

  /// Gün farkı UTC günleriyle sayılır: yaz saati geçişinde 23 saatlik bir
  /// gün farkı sıfıra yuvarlanmasın.
  static int daysUntil(String dueDate, DateTime today) {
    final due = DateTime.parse(dueDate);
    return DateTime.utc(
      due.year,
      due.month,
      due.day,
    ).difference(DateTime.utc(today.year, today.month, today.day)).inDays;
  }
}
