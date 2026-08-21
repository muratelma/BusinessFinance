import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../core/routing/app_locations.dart';
import 'quick_add_launcher.dart';
import 'quick_add_models.dart';

/// Shared entry used by the shell FAB and the İşlemler FAB, so both open the same
/// launcher instead of each deciding what "add" means.
Future<void> openQuickAdd(BuildContext context) async {
  final option = await QuickAddLauncher.show(context);
  if (option == null || !context.mounted) return;

  switch (option) {
    case QuickAddOption.expense:
      context.push('/transactions/new/expense');
    // Fiş de gider yazar; yalnız formun önünü fotoğraftan dolduran bir adım
    // önce gelir. Ayrı bir kayıt yolu değil, aynı yolun girişi.
    case QuickAddOption.receipt:
      context.push('/transactions/new/receipt');
    case QuickAddOption.income:
      context.push('/transactions/new/income');
    // Dekontun kendi sayfası var: yön sormaz, üç banka belgesini de okur ve
    // "bu tutar ne?" sorusunu okuma bittikten sonra karar sayfasında sorar.
    case QuickAddOption.bankSlip:
      context.push(bankSlipScanLocation);
    // Transfers belong to accounts and card payments to cards; recurring
    // plans to planning. The launcher routes to them rather than growing a
    // second copy of forms that already exist.
    case QuickAddOption.transfer:
      context.push('/more/accounts?tab=transfers');
    case QuickAddOption.cardPayment:
      context.push('/more/cards');
    case QuickAddOption.recurringPlan:
      context.push('/more/planning');
  }
}
