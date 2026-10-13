import 'package:flutter/material.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../day_close/data/day_close_repository.dart';
import '../../day_close/presentation/day_close_controller.dart';
import '../../day_close/presentation/day_close_detail.dart';
import '../../finance_pages/data/finance_page_repository.dart';
import '../../pos/data/pos_repository.dart';
import '../../pos/presentation/pos_deposit_sheets.dart';
import '../data/activity_models.dart';
import 'activity_controller.dart';
import 'activity_detail_sheet.dart';

/// İşlemler, kategori ve hesap sayfaları aynı ayrıntıyı açar.
Future<void> showActivityDetail(
  BuildContext context, {
  required FinancialActivity activity,
  required ActivityController controller,
  FinancePageRepositoryContract? pageRepository,
  PosRepositoryContract? posRepository,
  DayCloseRepositoryContract? dayCloseRepository,
  FinancialDataChanges? changes,
  bool reloadOnCancel = false,
}) async {
  if (activity.kind == ActivityKind.posDeposit && posRepository != null) {
    await showPosDepositDetail(
      context,
      repository: posRepository,
      depositId: activity.activityId,
      changes: changes,
      initial: PosDepositSummary(
        depositedAmount: activity.amount,
        depositDate: activity.activityDate,
        accountName: activity.destinationName ?? '',
        currency: activity.currency,
        isCancelled: activity.isCancelled,
        deductionAmount: activity.hasFee ? activity.feeAmount : null,
        settlementCount: activity.settlementCount ?? 0,
      ),
    );
    return;
  }
  final balances = activity.isCancelled
      ? null
      : controller.balancesAfter(activity);
  final parts = pageRepository?.parts(activity);
  await AppAdaptiveSheet.show<void>(
    context: context,
    builder: (sheetContext) => AnimatedBuilder(
      animation: controller,
      builder: (_, _) => ActivityDetailSheet(
        activity: activity,
        balances: balances,
        parts: parts,
        errorMessage: controller.errorMessage,
        isCancelling: controller.isCancelling,
        onShowDayClose: activity.belongsToDayClose && dayCloseRepository != null
            ? () async {
                Navigator.pop(sheetContext);
                final dayController = DayCloseController(
                  dayCloseRepository,
                  changes: changes,
                );
                try {
                  if (context.mounted) {
                    await showDayCloseDay(
                      context,
                      controller: dayController,
                      date: activity.activityDate,
                    );
                  }
                } finally {
                  dayController.dispose();
                }
              }
            : null,
        onCancel: activity.canCancel
            ? () async {
                if (await controller.cancel(activity, reload: reloadOnCancel) &&
                    sheetContext.mounted) {
                  Navigator.pop(sheetContext);
                }
              }
            : null,
      ),
    ),
  );
  if (!context.mounted) return;
  final message = controller.successMessage ?? controller.errorMessage;
  if (message != null) {
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(message)));
  }
  controller.clearMessages();
}
