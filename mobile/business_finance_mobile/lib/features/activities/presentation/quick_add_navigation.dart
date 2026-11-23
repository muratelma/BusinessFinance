import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../core/routing/app_locations.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../debt_book/presentation/debt_entry_prefill.dart';
import '../../debt_book/presentation/debt_entry_sheet.dart';
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
    // `Borç ve alacak` sayfasındaki `+` ile aynı yol: önce `Ne oldu?`, sonra
    // form. Kaydın kişinin hesabına mı ayrı kayda mı gideceğini form seçer.
    case QuickAddOption.debt:
      final kind = await AppAdaptiveSheet.show<DebtEntryKind>(
        context: context,
        builder: (_) => const DebtEntrySheet(),
      );
      if (kind == null || !context.mounted) return;
      context.push(debtEntryLocation, extra: DebtEntryRequest(kind));
    case QuickAddOption.income:
      context.push('/transactions/new/income');
    // POS tahsilatının kendi formu `Kasa` ekranındadır ve orada kalır: tahsilat
    // listesiyle aynı yerde durması, kullanıcının yoldaki parayı girdiği anda
    // görmesini sağlıyor. Menü ikinci bir kopya form açmıyor, oraya götürüyor.
    case QuickAddOption.posCollection:
      context.push('$cashLocation?tab=pos');
    // Gün sonunun paneli de Kasa'da açılır: kaydedince kullanıcı günün
    // kapandığını ve yoldaki parayı aynı ekranda görür.
    case QuickAddOption.dayClose:
      context.push('$cashLocation?tab=day-close');
    // Dekontun kendi sayfası var: yön sormaz, üç banka belgesini de okur ve
    // "bu tutar ne?" sorusunu okuma bittikten sonra karar sayfasında sorar.
    case QuickAddOption.bankSlip:
      context.push(bankSlipScanLocation);
    // Transfers belong to accounts and card payments to cards; recurring
    // plans to planning. The launcher routes to them rather than growing a
    // second copy of forms that already exist.
    case QuickAddOption.transfer:
      context.push(transferLocation);
    case QuickAddOption.cardPayment:
      context.push('/more/cards');
    case QuickAddOption.recurringPlan:
      context.push('/more/planning');
  }
}
