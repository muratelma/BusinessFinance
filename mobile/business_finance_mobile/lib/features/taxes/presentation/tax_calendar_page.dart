import 'package:flutter/material.dart';

import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_state_views.dart';
import '../data/tax_models.dart';
import 'tax_controller.dart';

/// Vergi ve SGK takvimi.
///
/// Kalemler **tekrarlayan plandır**; bu ekran yeni bir zamanlayıcı kurmaz,
/// yalnız hazır kalemi öneren ve formu önceden dolduran bir kapıdır. Tarih ve
/// tutar kurulduğu andan itibaren kullanıcınındır ve uygulama mevzuat takibi
/// yapmaz — bu, ekranın en üstünde yazılıdır (ADR 0016).
class TaxCalendarPage extends StatefulWidget {
  const TaxCalendarPage({
    required this.controller,
    super.key,
    this.ownsController = true,
    this.onInstall,
  });

  final TaxCalendarController controller;
  final bool ownsController;

  /// Kalemi kurma yolu: önerinin doldurduğu tekrarlayan plan formu.
  ///
  /// Ekranın kendi yazma yolu **yok**; olsaydı aynı plan iki ayrı biçimde
  /// oluşabilirdi.
  final void Function(TaxCalendarSuggestion suggestion)? onInstall;

  @override
  State<TaxCalendarPage> createState() => _TaxCalendarPageState();
}

class _TaxCalendarPageState extends State<TaxCalendarPage> {
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
      appBar: AppBar(title: const Text('Vergi takvimi')),
      body: AnimatedBuilder(
        animation: widget.controller,
        builder: (context, _) => _buildBody(context),
      ),
    );
  }

  Widget _buildBody(BuildContext context) {
    final controller = widget.controller;
    if (controller.unauthorized) return const AppUnauthorizedView();
    if (controller.isLoading && controller.suggestions.isEmpty) {
      return const AppLoadingView();
    }
    if (controller.errorMessage != null && controller.suggestions.isEmpty) {
      return AppErrorView(
        message: controller.errorMessage!,
        onRetry: controller.load,
      );
    }
    if (controller.suggestions.isEmpty) {
      return const AppEmptyView(
        title: 'Hazır kalem yok',
        message:
            'Takvim kalemlerini tekrarlayan plan olarak kendiniz '
            'kurabilirsiniz.',
        icon: Icons.event_note_outlined,
      );
    }

    return ListView(
      padding: const EdgeInsets.all(AppSpacing.medium),
      children: [
        const AppInlineNotice(
          icon: Icons.info_outline,
          message:
              'Bu tarihler birer öneridir ve kurduktan sonra size aittir. '
              'Uygulama mevzuat takibi yapmaz, vergi hesaplamaz; kaçırılan '
              'tarihin sorumluluğu size aittir.',
        ),
        const SizedBox(height: AppSpacing.medium),
        AppCard(
          padding: EdgeInsets.zero,
          child: Column(
            children: [
              for (final suggestion in controller.suggestions)
                AppListRow(
                  icon: Icons.event_repeat,
                  title: suggestion.label,
                  subtitle: suggestion.scheduleLabel,
                  onTap: widget.onInstall == null
                      ? null
                      : () => widget.onInstall!(suggestion),
                ),
            ],
          ),
        ),
        const SizedBox(height: AppSpacing.medium),
        const AppInlineNotice(
          icon: Icons.payments_outlined,
          message:
              'Öneriler tutar taşımaz. Kurarken beklediğiniz tutarı yazın; '
              'ödeme günü gerçek tutarı girebilirsiniz.',
        ),
      ],
    );
  }
}
