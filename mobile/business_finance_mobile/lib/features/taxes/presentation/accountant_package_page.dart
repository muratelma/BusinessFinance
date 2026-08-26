import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:share_plus/share_plus.dart';

import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_metric_tile.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_state_views.dart';
import 'tax_controller.dart';

/// Ay sonu muhasebeci paketi.
///
/// Ekran **yalnız işletme** tarafını gösterir; şahsi kayıt pakete girmez ve bu
/// ekranın da bir kapsam anahtarı yoktur. Bütün tutarlar sunucudan gelir —
/// istemci hiçbirini toplamaz (ADR 0016).
class AccountantPackagePage extends StatefulWidget {
  const AccountantPackagePage({
    required this.controller,
    super.key,
    this.ownsController = true,
    this.share,
  });

  final AccountantPackageController controller;
  final bool ownsController;

  /// Dosyayı paylaşma yolu; testte yerine geçer.
  final Future<void> Function(Uint8List bytes, String fileName)? share;

  @override
  State<AccountantPackagePage> createState() => _AccountantPackagePageState();
}

class _AccountantPackagePageState extends State<AccountantPackagePage> {
  @override
  void initState() {
    super.initState();
    widget.controller.load();
  }

  @override
  void dispose() {
    if (widget.ownsController) widget.controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Muhasebeci paketi')),
      body: AnimatedBuilder(
        animation: widget.controller,
        builder: (context, _) => _buildBody(context),
      ),
    );
  }

  Widget _buildBody(BuildContext context) {
    final controller = widget.controller;
    if (controller.unauthorized) return const AppUnauthorizedView();
    final package = controller.package;
    if (controller.isLoading && package == null) return const AppLoadingView();
    if (package == null) {
      return AppErrorView(
        message: controller.errorMessage ?? 'Paket yüklenemedi.',
        onRetry: controller.load,
      );
    }

    return ListView(
      padding: const EdgeInsets.all(AppSpacing.medium),
      children: [
        _buildPeriodPicker(context, package.periodLabel),
        const SizedBox(height: AppSpacing.medium),
        if (package.isEmpty)
          const AppInlineNotice(
            icon: Icons.inbox_outlined,
            message:
                'Bu ayda işletme kapsamlı kayıt yok. Paket yine üretilebilir '
                'ama içinde satır olmaz.',
          )
        else ...[
          Row(
            children: [
              Expanded(
                child: AppMetricTile(
                  label: 'İşletme geliri',
                  amount: package.totalIncome,
                  currency: package.currency,
                  effect: AppMoneyEffect.income,
                ),
              ),
              const SizedBox(width: AppSpacing.medium),
              Expanded(
                child: AppMetricTile(
                  label: 'İşletme gideri',
                  amount: package.totalExpense,
                  currency: package.currency,
                  effect: AppMoneyEffect.expense,
                ),
              ),
            ],
          ),
          const SizedBox(height: AppSpacing.medium),
          AppMetricTile(
            label: 'Ayın işletme neti',
            amount: package.net,
            currency: package.currency,
            caption: 'Aynı ayın işletme raporuyla birebir aynı sayı.',
          ),
          const SizedBox(height: AppSpacing.medium),
          AppCard(
            padding: EdgeInsets.zero,
            child: Column(
              children: [
                AppListRow(
                  icon: Icons.receipt_long_outlined,
                  title: 'Gelirin KDV\'si',
                  subtitle: 'Belgelerde yazdığı kadar',
                  trailing: Text(
                    MoneyText.format(package.vatOnIncome, package.currency),
                  ),
                ),
                AppListRow(
                  icon: Icons.receipt_outlined,
                  title: 'Giderin KDV\'si',
                  subtitle: 'Belgelerde yazdığı kadar',
                  trailing: Text(
                    MoneyText.format(package.vatOnExpense, package.currency),
                  ),
                ),
                AppListRow(
                  icon: Icons.block_outlined,
                  title: 'İndirilemeyen gider',
                  subtitle: '${package.nonDeductibleCount} kalem',
                  trailing: Text(
                    MoneyText.format(
                      package.nonDeductibleExpense,
                      package.currency,
                    ),
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: AppSpacing.medium),
          AppCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Pakette ne var',
                  style: Theme.of(context).textTheme.titleSmall,
                ),
                const SizedBox(height: AppSpacing.small),
                Text('${package.lineCount} kayıt'),
                Text('${package.attachmentCount} belge'),
                if (package.linesWithoutVat > 0)
                  Text('${package.linesWithoutVat} kayıtta KDV yazılmamış'),
                if (package.deductibilityUnansweredCount > 0)
                  Text(
                    '${package.deductibilityUnansweredCount} giderde '
                    'indirilebilirlik cevaplanmamış',
                  ),
              ],
            ),
          ),
          if (package.omittedAttachmentCount > 0) ...[
            const SizedBox(height: AppSpacing.medium),
            AppInlineNotice(
              icon: Icons.warning_amber_outlined,
              message:
                  '${package.omittedAttachmentCount} belge boyut sınırını '
                  'aştığı için dosyaya konmadı; listede adı geçiyor.',
            ),
          ],
        ],
        const SizedBox(height: AppSpacing.medium),
        const AppInlineNotice(
          icon: Icons.lock_outline,
          message:
              'Pakette yalnız işletme kayıtları var; şahsi hiçbir kayıt '
              'girmez. Dosya bu cihazdan paylaşılır.',
        ),
        const SizedBox(height: AppSpacing.medium),
        FilledButton.icon(
          onPressed: controller.isDownloading ? null : _sharePackage,
          icon: const Icon(Icons.ios_share),
          label: Text(
            controller.isDownloading ? 'Hazırlanıyor…' : 'Paketi paylaş',
          ),
        ),
        if (controller.errorMessage != null)
          Padding(
            padding: const EdgeInsets.only(top: AppSpacing.medium),
            child: Semantics(
              liveRegion: true,
              child: Text(
                controller.errorMessage!,
                style: TextStyle(color: Theme.of(context).colorScheme.error),
              ),
            ),
          ),
      ],
    );
  }

  Widget _buildPeriodPicker(BuildContext context, String label) => Row(
    children: [
      IconButton(
        onPressed: () => widget.controller.shiftMonth(-1),
        icon: const Icon(Icons.chevron_left),
        tooltip: 'Önceki ay',
      ),
      Expanded(
        child: Text(
          label,
          textAlign: TextAlign.center,
          style: Theme.of(context).textTheme.titleMedium,
        ),
      ),
      IconButton(
        onPressed: () => widget.controller.shiftMonth(1),
        icon: const Icon(Icons.chevron_right),
        tooltip: 'Sonraki ay',
      ),
    ],
  );

  Future<void> _sharePackage() async {
    final response = await widget.controller.download();
    if (response == null || !mounted) return;
    final bytes = Uint8List.fromList(response.bytes);
    final fileName =
        'muhasebeci-paketi-${widget.controller.year.toString().padLeft(4, '0')}'
        '-${widget.controller.month.toString().padLeft(2, '0')}.zip';
    final share = widget.share;
    try {
      if (share != null) {
        await share(bytes, fileName);
        return;
      }
      await SharePlus.instance.share(
        ShareParams(
          files: [XFile.fromData(bytes, mimeType: 'application/zip')],
          fileNameOverrides: [fileName],
        ),
      );
    } on Exception {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Dosya paylaşımı başlatılamadı.')),
      );
    }
  }
}
