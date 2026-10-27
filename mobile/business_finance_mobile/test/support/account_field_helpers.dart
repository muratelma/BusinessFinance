import 'package:business_finance_mobile/core/widgets/app_account_field.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

/// Hesap alanı: [label] verilmezse sayfadaki ilk alan.
Finder accountField([String? label]) => label == null
    ? find.byType(AppAccountField).first
    : find
          .ancestor(
            of: find.text(label),
            matching: find.byType(AppAccountField),
          )
          .first;

/// Hesap alanının menüsündeki satırlar; menü sayfanın içinde çizildiği için
/// `find.text(...).last` formun altındaki başka bir yazıyı bulabilir.
Finder accountMenuRow(String name) => find
    .descendant(
      of: find.byType(MenuItemButton),
      matching: find.textContaining(name),
    )
    .first;

/// Alanı açar ve adı [name] içeren satırı seçer.
Future<void> chooseAccount(
  WidgetTester tester,
  String name, {
  String? label,
}) async {
  await tester.tap(accountField(label));
  await tester.pumpAndSettle();
  await tester.tap(accountMenuRow(name));
  await tester.pumpAndSettle();
}
