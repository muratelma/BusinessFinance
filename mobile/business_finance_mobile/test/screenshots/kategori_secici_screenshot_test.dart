import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/core/theme/app_spacing.dart';
import 'package:business_finance_mobile/core/widgets/app_account_field.dart';
import 'package:business_finance_mobile/core/widgets/app_category_field.dart';
import 'package:business_finance_mobile/core/widgets/app_date_field.dart';
import 'package:business_finance_mobile/core/widgets/app_untracked_text.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import '../support/category_field_helpers.dart';
import 'screenshot_harness.dart';

const _b = TransactionScope.business;
const _p = TransactionScope.personal;

/// Gider ekle teslimindeki kategoriler (`kod/KatSec.jsx`, `KCG`).
const _expense = [
  AppCategoryOption(id: 'amb', name: 'Ambalaj ve sarf malzemesi', scope: _b),
  AppCategoryOption(id: 'arac', name: 'Araç ve yakıt', scope: _b),
  AppCategoryOption(id: 'bakim', name: 'Bakım-onarım', scope: _b),
  AppCategoryOption(id: 'kom', name: 'Banka ve POS komisyonu', scope: _b),
  AppCategoryOption(id: 'ilet', name: 'İletişim', scope: _b),
  AppCategoryOption(id: 'fat', name: 'İşyeri faturaları', scope: _b),
  AppCategoryOption(id: 'kira', name: 'İşyeri kirası', scope: _b),
  AppCategoryOption(id: 'pers', name: 'Personel giderleri', scope: _b),
  AppCategoryOption(id: 'sig', name: 'Sigorta', scope: _b),
  AppCategoryOption(id: 'mal', name: 'Ticari mal alımı', scope: _b),
  AppCategoryOption(id: 'vergi', name: 'Vergi ve harç', scope: _b),
  AppCategoryOption(id: 'abon', name: 'Abonelikler', scope: _p),
  AppCategoryOption(id: 'egit', name: 'Eğitim', scope: _p),
  AppCategoryOption(id: 'eglen', name: 'Eğlence', scope: _p),
  AppCategoryOption(id: 'evfat', name: 'Ev faturaları', scope: _p),
  AppCategoryOption(id: 'evkira', name: 'Ev kirası ve aidat', scope: _p),
  AppCategoryOption(id: 'giyim', name: 'Giyim', scope: _p),
  AppCategoryOption(id: 'market', name: 'Market', scope: _p),
  AppCategoryOption(id: 'saglik', name: 'Sağlık', scope: _p),
  AppCategoryOption(id: 'faiz', name: 'Faiz ve finansman gideri'),
  AppCategoryOption(id: 'diger', name: 'Diğer gider'),
];

const _recent = [
  AppCategoryValue('mal', _b),
  AppCategoryValue('fat', _b),
  AppCategoryValue('arac', _b),
  AppCategoryValue('market', _p),
];

/// Gelir ekle teslimindeki kategoriler (`KCI`).
const _income = [
  AppCategoryOption(id: 'satis', name: 'Satış geliri', scope: _b),
  AppCategoryOption(id: 'hizmet', name: 'Hizmet geliri', scope: _b),
  AppCategoryOption(id: 'kirag', name: 'Kira geliri', scope: _p),
  AppCategoryOption(id: 'faizg', name: 'Faiz geliri'),
  AppCategoryOption(id: 'digerg', name: 'Diğer gelir'),
];

/// Kategori seçici, Claude Design teslimiyle
/// (`design/kategori-pos-handoff/screenshots/`) aynı örnek veriyle.
void main() {
  Future<void> tapField(WidgetTester t) => t.tap(categoryField());

  for (final (name, page, before) in [
    ('06-k4-gider-kapali', _Form(expense: true), null),
    (
      '07-k4o-kategori-menu-acik',
      _Form(expense: true, initial: const AppCategoryValue('fat', _b)),
      tapField,
    ),
    (
      '08-k4s-arama',
      _Form(expense: true),
      (WidgetTester t) async {
        await tapField(t);
        await t.pumpAndSettle();
        await t.enterText(find.byType(TextField).first, 'fat');
      },
    ),
    (
      '09-k4n-eslesme-yok',
      _Form(expense: true),
      (WidgetTester t) async {
        await tapField(t);
        await t.pumpAndSettle();
        await t.enterText(find.byType(TextField).first, 'kırtasiye');
      },
    ),
    ('10-g1-gelir-az-kategori', _Form(expense: false), tapField),
    (
      '11-z0-kapsamli-secildi',
      _Form(expense: false, initial: const AppCategoryValue('hizmet', _b)),
      null,
    ),
    (
      '12-z5-gelir-kapsam-menude',
      _Form(expense: false),
      (WidgetTester t) async {
        await tapField(t);
        await t.pumpAndSettle();
        await t.tap(categoryMenuRow('Faiz geliri'));
      },
    ),
    (
      '15-z5gb-gider-secildi',
      _Form(expense: true, initial: const AppCategoryValue('faiz', _p)),
      null,
    ),
    (
      '16-k2-isletmem-yok-acik',
      _Form(expense: true, showScope: false, personalOnly: true),
      tapField,
    ),
  ]) {
    testWidgets('kategori seçici $name', (tester) async {
      await captureScreen(
        tester,
        'ks-$name',
        page,
        selectedTab: 1,
        before: before,
      );
    }, skip: !screenshotsEnabled);
  }
}

class _Form extends StatefulWidget {
  const _Form({
    required this.expense,
    this.initial,
    this.showScope = true,
    this.personalOnly = false,
  });
  final bool expense;
  final AppCategoryValue? initial;
  final bool showScope;
  final bool personalOnly;

  @override
  State<_Form> createState() => _FormState();
}

class _FormState extends State<_Form> {
  late AppCategoryValue? value = widget.initial;

  @override
  Widget build(BuildContext context) {
    final options = widget.personalOnly
        ? const [
            AppCategoryOption(id: 'market', name: 'Market'),
            AppCategoryOption(id: 'ulasim', name: 'Ulaşım'),
            AppCategoryOption(id: 'fatura', name: 'Faturalar'),
            AppCategoryOption(id: 'saglik', name: 'Sağlık'),
            AppCategoryOption(id: 'diger', name: 'Diğer gider'),
          ]
        : widget.expense
        ? _expense
        : _income;
    return Scaffold(
      appBar: AppBar(title: Text(widget.expense ? 'Gider ekle' : 'Gelir ekle')),
      body: AppUntrackedText(
        child: ListView(
          padding: const EdgeInsets.all(AppSpacing.medium),
          children: [
            AppAccountField(
              label: 'Hesap',
              value: 'ziraat',
              options: const [
                AppAccountOption(
                  id: 'ziraat',
                  name: 'Ziraat vadesiz',
                  kind: AppAccountKind.bank,
                  scope: 'İşletme',
                  amount: '30658.3500',
                ),
              ],
              onChanged: (_) {},
            ),
            const SizedBox(height: AppSpacing.medium),
            AppCategoryField(
              options: options,
              value: value,
              recent: widget.expense && widget.showScope ? _recent : const [],
              showScope: widget.showScope,
              onAdd: (_) {},
              onChanged: (next) => setState(() => value = next),
            ),
            const SizedBox(height: AppSpacing.medium),
            const TextField(
              decoration: InputDecoration(labelText: 'Tutar', prefixText: '₺ '),
            ),
            const SizedBox(height: AppSpacing.medium),
            AppDateField(
              label: 'Tarih',
              value: '2026-10-28',
              onChanged: (_) {},
            ),
            const SizedBox(height: AppSpacing.medium),
            const TextField(
              decoration: InputDecoration(
                labelText: 'Ad (isteğe bağlı)',
                helperText: 'Boş bırakılırsa kategori adı kullanılır.',
              ),
            ),
            const SizedBox(height: AppSpacing.large),
            FilledButton(onPressed: () {}, child: const Text('Kaydet')),
          ],
        ),
      ),
    );
  }
}
