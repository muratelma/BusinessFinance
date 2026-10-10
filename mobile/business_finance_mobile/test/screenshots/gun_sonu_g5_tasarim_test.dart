import 'package:business_finance_mobile/core/theme/app_radius.dart';
import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/core/theme/app_surfaces.dart';
import 'package:business_finance_mobile/core/widgets/app_divided_column.dart';
import 'package:business_finance_mobile/core/widgets/app_form_sheet.dart';
import 'package:business_finance_mobile/core/widgets/app_inline_notice.dart';
import 'package:business_finance_mobile/core/widgets/app_money_text.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'screenshot_harness.dart';
import 'tasarim_ortak.dart';

/// Gün sonu panelinde vadeli satış ve tahsilat soruları (karar G5, 10 Ekim
/// 2026): metin ve yerleşim **önerisi**. Yalnız çizimdir; panel kullanıcı
/// onayından sonra bu çizime göre kurulur. Veri sentetiktir.
void main() {
  Future<void> draw(
    WidgetTester tester,
    String name,
    List<Widget> Function(BuildContext context) children,
  ) => captureScreen(
    tester,
    name,
    Scaffold(
      appBar: AppBar(title: const Text('Kasa')),
      body: const SizedBox.expand(),
    ),
    withNavBar: false,
    before: (tester) async {
      AppFormSheet.show<bool>(
        context: tester.element(find.text('Kasa')),
        builder: (context) => AppFormSheet<bool>(
          title: 'Gün sonu',
          submitLabel: 'Gün sonunu kaydet',
          onSubmit: () async => null,
          children: children(context),
        ),
      );
      await tester.pump();
      await tester.pump(const Duration(seconds: 1));
    },
  );

  testWidgets('g5 01 sorular cevapsız', (tester) async {
    await draw(
      tester,
      'g5-01-sorular-cevapsiz',
      (context) => [
        _cashField('1.300'),
        _question(
          context,
          title: 'Bugünkü tahsilatlar',
          note: 'Zaten kayıtlı. Dahilse yeni satıştan düşülür.',
          selected: '',
          rows: const [_Row('Ahmet Bakkal', 'Kasa', '300.0000')],
        ),
        _question(
          context,
          title: 'Bugünkü veresiye satışlar',
          note: 'Geliri yazıldı. Dahilse yeni satıştan düşülür.',
          selected: '',
          rows: const [_Row('Ayşe Terzi', 'Veresiye satış', '450.0000')],
        ),
      ],
    );
  }, skip: !screenshotsEnabled);

  testWidgets('g5 02 cevaplı ve hesap', (tester) async {
    await draw(
      tester,
      'g5-02-cevapli-ve-hesap',
      (context) => [
        _cashField('1.300'),
        _question(
          context,
          title: 'Bugünkü tahsilatlar',
          note: 'Zaten kayıtlı. Dahilse yeni satıştan düşülür.',
          selected: 'Hepsi',
          rows: const [_Row('Ahmet Bakkal', 'Kasa', '300.0000')],
        ),
        _question(
          context,
          title: 'Bugünkü veresiye satışlar',
          note: 'Geliri yazıldı. Dahilse yeni satıştan düşülür.',
          selected: 'Hiçbiri',
          rows: const [_Row('Ayşe Terzi', 'Veresiye satış', '450.0000')],
        ),
        _summary(
          context,
          calculation: 'Kasa · ₺1.300,00 − kayıtlı ₺300,00',
          amount: '1000.0000',
        ),
      ],
    );
  }, skip: !screenshotsEnabled);

  testWidgets('g5 03 bazıları', (tester) async {
    await draw(
      tester,
      'g5-03-bazilari',
      (context) => [
        _cashField('1.300'),
        _question(
          context,
          title: 'Bugünkü tahsilatlar',
          note: 'Zaten kayıtlı. Dahilse yeni satıştan düşülür.',
          selected: 'Bazıları',
          rows: const [
            _Row('Ahmet Bakkal', 'Kasa', '300.0000', checked: true),
            _Row('Mehmet Usta', 'Kasa', '120.0000', checked: false),
          ],
        ),
        _summary(
          context,
          calculation: 'Kasa · ₺1.300,00 − kayıtlı ₺300,00',
          amount: '1000.0000',
        ),
      ],
    );
  }, skip: !screenshotsEnabled);

  testWidgets('g5 04 ortak tutar', (tester) async {
    await draw(
      tester,
      'g5-04-ortak-tutar',
      (context) => [
        _cashField('1.600'),
        _overlap(context),
        _summary(
          context,
          calculation: 'Kasa · ₺1.600,00 − kayıtlı ₺600,00',
          amount: '1000.0000',
        ),
      ],
    );
  }, skip: !screenshotsEnabled);

  testWidgets('g5 05 toplam farkı', (tester) async {
    await draw(
      tester,
      'g5-05-toplam-farki',
      (context) => [
        _field('Ziraat POS', '500'),
        _field('Toplam (isteğe bağlı)', '1.800'),
        const AppInlineNotice(
          message:
              'Yalnız kart satışı kaydedilecek. Toplamla arasındaki '
              '₺1.300,00 kaydedilmez.',
          margin: EdgeInsets.only(top: AppSpacing.small),
        ),
        _summary(
          context,
          title: 'Ziraat POS',
          calculation: 'komisyon ₺10,00 · 13 Eki beklenir',
          amount: '500.0000',
        ),
      ],
    );
  }, skip: !screenshotsEnabled);
}

class _Row {
  const _Row(this.title, this.subtitle, this.amount, {this.checked});

  final String title;
  final String subtitle;
  final String amount;

  /// Boş: satır yalnız bilgi verir (`Hepsi` / `Hiçbiri` / cevapsız).
  final bool? checked;
}

Widget _cashField(String value) => _field('Nakit satış', value);

Widget _field(String label, String value) => Padding(
  padding: const EdgeInsets.only(bottom: AppSpacing.small),
  child: TextFormField(
    initialValue: value,
    decoration: InputDecoration(labelText: label, suffixText: 'TRY'),
  ),
);

/// Bölüm başına tek soru: başlık, soru, üç cevap ve o bölümün kayıtları.
Widget _question(
  BuildContext context, {
  required String title,
  required String note,
  required String selected,
  required List<_Row> rows,
}) {
  final theme = Theme.of(context);
  final muted = theme.textTheme.bodySmall?.copyWith(
    color: AppSurfaces.of(context).inkMuted,
  );
  return Padding(
    padding: const EdgeInsets.only(top: AppSpacing.medium),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(title, style: theme.textTheme.labelMedium),
        Text('Yazdığınız nakit tutarına dahil mi?', style: muted),
        const SizedBox(height: AppSpacing.small),
        designRail(context, const ['Hepsi', 'Hiçbiri', 'Bazıları'], selected),
        for (final row in rows) _recordRow(context, row),
        Text(note, style: muted),
      ],
    ),
  );
}

Widget _recordRow(BuildContext context, _Row row) {
  final theme = Theme.of(context);
  return ConstrainedBox(
    constraints: const BoxConstraints(minHeight: 48),
    child: Row(
      children: [
        if (row.checked != null) ...[
          Checkbox(value: row.checked, onChanged: (_) {}),
          const SizedBox(width: AppSpacing.xSmall),
        ],
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text(
                row.title,
                style: theme.textTheme.bodyMedium?.copyWith(
                  fontWeight: FontWeight.w500,
                ),
              ),
              Text(
                row.subtitle,
                style: theme.textTheme.bodySmall?.copyWith(
                  color: AppSurfaces.of(context).inkMuted,
                ),
              ),
            ],
          ),
        ),
        AppMoneyText(
          amount: row.amount,
          currency: 'TRY',
          size: AppMoneySize.body,
        ),
      ],
    ),
  );
}

/// Aynı kişinin satışı da tahsilatı da dahil edilince sorulan ortak tutar.
Widget _overlap(BuildContext context) {
  final theme = Theme.of(context);
  final surfaces = AppSurfaces.of(context);
  final muted = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);

  Widget option(String label, String result, {required bool selected}) =>
      ConstrainedBox(
        constraints: const BoxConstraints(minHeight: 48),
        child: Row(
          children: [
            Icon(
              selected
                  ? Icons.radio_button_checked
                  : Icons.radio_button_unchecked,
              size: 22,
              color: selected ? surfaces.ink : surfaces.inkMuted,
            ),
            const SizedBox(width: AppSpacing.small),
            Expanded(child: Text(label, style: theme.textTheme.bodyMedium)),
            Text(result, style: muted),
          ],
        ),
      );

  return Padding(
    padding: const EdgeInsets.only(top: AppSpacing.medium),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text('Ahmet Bakkal', style: theme.textTheme.labelMedium),
        Text(
          'Satışı (₺500,00) ve tahsilatı (₺300,00) nakit tutarında nasıl '
          'sayıldı?',
          style: muted,
        ),
        option('Ayrı ayrı', '₺800,00 düşülür', selected: false),
        option('Tahsilat satışın içinde', '₺500,00 düşülür', selected: false),
        option('Bir kısmı ortak', '₺600,00 düşülür', selected: true),
        Padding(
          padding: const EdgeInsets.only(left: 30, top: AppSpacing.xSmall),
          child: TextFormField(
            initialValue: '200',
            decoration: const InputDecoration(
              labelText: 'Ortak tutar',
              suffixText: 'TRY',
              helperText: 'İki kayıtta da görünen para.',
            ),
          ),
        ),
      ],
    ),
  );
}

/// "Yazılacak" özeti: bugünkü özetle aynı blok, hesabı açık yazar.
Widget _summary(
  BuildContext context, {
  required String calculation,
  required String amount,
  String title = 'Nakit satış',
}) {
  final theme = Theme.of(context);
  final surfaces = AppSurfaces.of(context);
  return Padding(
    padding: const EdgeInsets.only(top: AppSpacing.large),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text('Yazılacak', style: theme.textTheme.labelMedium),
        const SizedBox(height: AppSpacing.xSmall),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
          decoration: BoxDecoration(
            color: surfaces.cardMuted,
            borderRadius: BorderRadius.circular(AppRadius.field),
          ),
          child: AppDividedColumn(
            children: [
              ConstrainedBox(
                constraints: const BoxConstraints(minHeight: 48),
                child: Padding(
                  padding: const EdgeInsets.symmetric(
                    vertical: AppSpacing.small,
                  ),
                  child: Row(
                    children: [
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              title,
                              style: theme.textTheme.bodyMedium?.copyWith(
                                fontWeight: FontWeight.w500,
                              ),
                            ),
                            Text(
                              calculation,
                              style: theme.textTheme.bodySmall?.copyWith(
                                color: surfaces.inkMuted,
                              ),
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(width: AppSpacing.medium),
                      AppMoneyText(
                        amount: amount,
                        currency: 'TRY',
                        effect: AppMoneyEffect.income,
                        size: AppMoneySize.body,
                        style: const TextStyle(fontWeight: FontWeight.w600),
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ),
        ),
      ],
    ),
  );
}
