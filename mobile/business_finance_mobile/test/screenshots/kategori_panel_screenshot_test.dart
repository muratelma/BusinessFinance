import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/core/widgets/app_account_field.dart';
import 'package:business_finance_mobile/core/widgets/app_category_field.dart';
import 'package:business_finance_mobile/core/widgets/app_choice_segments.dart';
import 'package:business_finance_mobile/core/widgets/app_date_field.dart';
import 'package:business_finance_mobile/core/widgets/app_form_sheet.dart';
import 'package:business_finance_mobile/core/widgets/app_untracked_text.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import '../support/category_field_helpers.dart';
import 'screenshot_harness.dart';

const _b = TransactionScope.business;

/// POS tahsilatı `Elle gir` panelinde kategori menüsü: panelin gövdesi alan +
/// menü kadar boylanır; 4, 7 ve 18 kategoriyle (kullanıcı kararı, 28 Ekim 2026).
const _sales = [
  AppCategoryOption(id: 's1', name: 'Satış geliri', scope: _b),
  AppCategoryOption(id: 's2', name: 'Satış geliri (kart)', scope: _b),
  AppCategoryOption(id: 's3', name: 'Hizmet geliri', scope: _b),
  AppCategoryOption(id: 's4', name: 'Diğer işletme geliri', scope: _b),
  AppCategoryOption(id: 's5', name: 'Faiz geliri'),
  AppCategoryOption(id: 's6', name: 'İade ve geri ödeme'),
  AppCategoryOption(id: 's7', name: 'Diğer gelir'),
];

const _p = TransactionScope.personal;

/// `Kendime aldım`daki gibi uzun şahsi gider listesi.
const _personal = [
  AppCategoryOption(id: 'p1', name: 'Abonelikler', scope: _p),
  AppCategoryOption(id: 'p2', name: 'Bağış', scope: _p),
  AppCategoryOption(id: 'p3', name: 'Eğitim', scope: _p),
  AppCategoryOption(id: 'p4', name: 'Eğlence', scope: _p),
  AppCategoryOption(id: 'p5', name: 'Ev eşyası', scope: _p),
  AppCategoryOption(id: 'p6', name: 'Ev faturaları', scope: _p),
  AppCategoryOption(id: 'p7', name: 'Ev kirası ve aidat', scope: _p),
  AppCategoryOption(id: 'p8', name: 'Evcil hayvan', scope: _p),
  AppCategoryOption(id: 'p9', name: 'Giyim', scope: _p),
  AppCategoryOption(id: 'p10', name: 'Hediye', scope: _p),
  AppCategoryOption(id: 'p11', name: 'Kişisel bakım', scope: _p),
  AppCategoryOption(id: 'p12', name: 'Market Alışverişi', scope: _p),
  AppCategoryOption(id: 'p13', name: 'Sağlık', scope: _p),
  AppCategoryOption(id: 'p14', name: 'Ulaşım', scope: _p),
  AppCategoryOption(id: 'p15', name: 'Yeme-içme', scope: _p),
  AppCategoryOption(id: 'p16', name: 'Faiz ve finansman gideri'),
  AppCategoryOption(id: 'p17', name: 'Sigorta'),
  AppCategoryOption(id: 'p18', name: 'Diğer gider'),
];

void main() {
  for (final (name, options, fixed) in [
    ('d-4-kategori', _sales.sublist(0, 4), _b),
    ('d-7-kategori', _sales, _b),
    ('d-18-kategori', _personal, _p),
  ]) {
    testWidgets('panel seçeneği $name', (tester) async {
      await captureScreen(
        tester,
        'kp-$name',
        Scaffold(
          appBar: AppBar(title: const Text('Kasa')),
          body: const SizedBox.expand(),
        ),
        withNavBar: false,
        before: (tester) async {
          AppFormSheet.show<bool>(
            context: tester.element(find.text('Kasa')),
            builder: (_) => _Panel(options: options, fixed: fixed),
          );
          await tester.pump();
          await tester.pump(const Duration(seconds: 1));
          await tester.tap(categoryField('Satış kategorisi'));
          await tester.pump();
          await tester.pump(const Duration(seconds: 1));
        },
      );
    }, skip: !screenshotsEnabled);
  }
}

class _Panel extends StatefulWidget {
  const _Panel({required this.options, required this.fixed});
  final List<AppCategoryOption> options;
  final TransactionScope fixed;

  @override
  State<_Panel> createState() => _PanelState();
}

class _PanelState extends State<_Panel> {
  AppCategoryValue? value;
  String commission = 'Yok';

  @override
  Widget build(BuildContext context) => AppUntrackedText(
    child: AppFormSheet<bool>(
      title: 'POS tahsilatı',
      description: 'Gelir bugün yazılır, para hesaba sonra geçer.',
      horizontalPadding: AppSpacing.medium,
      submitLabel: 'Tahsilatı kaydet',
      onSubmit: () async => true,
      children: [
        AppAccountField(
          label: 'POS',
          options: const [],
          value: 'manual',
          manualLabel: 'Elle gir',
          onChanged: (_) {},
        ),
        const SizedBox(height: AppSpacing.medium),
        AppAccountField(
          label: 'Paranın geçeceği hesap',
          value: 'g',
          options: const [
            AppAccountOption(
              id: 'g',
              name: 'Garanti Vadesiz',
              kind: AppAccountKind.bank,
              scope: 'İşletme',
              amount: '14500.0000',
            ),
          ],
          onChanged: (_) {},
        ),
        const SizedBox(height: AppSpacing.medium),
        AppCategoryField(
          label: 'Satış kategorisi',
          options: widget.options,
          value: value,
          fixedScope: widget.fixed,
          onChanged: (next) => setState(() => value = next),
        ),
        const SizedBox(height: AppSpacing.medium),
        const TextField(
          decoration: InputDecoration(
            labelText: 'Müşterinin ödediği',
            helperText: 'Brüt tutar: kestiğiniz fatura budur.',
          ),
        ),
        const SizedBox(height: AppSpacing.medium),
        AppDateField(
          label: 'Tahsilat günü',
          value: '2026-10-28',
          onChanged: (_) {},
        ),
        const SizedBox(height: AppSpacing.medium),
        AppDateField(
          label: 'Paranın beklendiği gün',
          value: '2026-10-28',
          onChanged: (_) {},
        ),
        const SizedBox(height: AppSpacing.medium),
        AppChoiceSegments<String>(
          label: 'Komisyon',
          values: const ['Yok', 'Tutar', 'Oran'],
          selected: commission,
          labelOf: (v) => v,
          onChanged: (v) => setState(() => commission = v),
        ),
        const SizedBox(height: AppSpacing.medium),
        const TextField(
          decoration: InputDecoration(labelText: 'Açıklama (isteğe bağlı)'),
        ),
      ],
    ),
  );
}
