import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_state_views.dart';
import '../data/tax_models.dart';
import 'tax_controller.dart';
import 'tax_parts.dart';
import 'tax_sheets.dart';
import 'tax_tracking_page.dart';

/// Ödenenler › Tümü: vergi işaretli kategorilerdeki giderlerin sayfalı
/// listesi, en yeniden eskiye. Satırlar ana ekrandakiyle aynıdır.
class TaxPaymentsPage extends StatefulWidget {
  const TaxPaymentsPage({required this.controller, super.key});

  final TaxController controller;

  @override
  State<TaxPaymentsPage> createState() => _TaxPaymentsPageState();
}

class _TaxPaymentsPageState extends State<TaxPaymentsPage> {
  static const pageSize = 20;

  TaxController get controller => widget.controller;

  final items = <TaxPayment>[];
  bool hasMore = false;
  bool loading = false;
  ApiException? error;
  int _seenFeed = 0;

  @override
  void initState() {
    super.initState();
    _seenFeed = controller.changes?.activityFeedRevision ?? 0;
    controller.changes?.addListener(_handleChanges);
    _reload();
  }

  @override
  void dispose() {
    controller.changes?.removeListener(_handleChanges);
    super.dispose();
  }

  void _handleChanges() {
    final revision = controller.changes!.activityFeedRevision;
    if (revision == _seenFeed) return;
    _seenFeed = revision;
    _reload();
  }

  Future<void> _reload() => _load(reset: true);

  Future<void> _load({required bool reset}) async {
    if (loading) return;
    setState(() {
      loading = true;
      error = null;
    });
    try {
      final page = await controller.repository.listPayments(
        skip: reset ? 0 : items.length,
        take: pageSize,
      );
      if (!mounted) return;
      setState(() {
        if (reset) items.clear();
        items.addAll(page.items);
        hasMore = page.hasMore;
      });
    } on ApiException catch (exception) {
      if (mounted) setState(() => error = exception);
    } on FormatException {
      if (mounted) {
        setState(() => error = ApiException.local('response.invalid_format'));
      }
    } finally {
      if (mounted) setState(() => loading = false);
    }
  }

  @override
  Widget build(BuildContext context) =>
      TaxPageScaffold(title: 'Ödenenler', body: _body(context));

  Widget _body(BuildContext context) {
    if (error?.isUnauthorized ?? false) return const AppUnauthorizedView();
    if (items.isEmpty) {
      if (loading) return const AppLoadingView(message: 'Ödemeler yükleniyor');
      if (error != null) {
        return AppErrorView(message: error!.message, onRetry: _reload);
      }
      return const AppEmptyView(
        icon: Icons.receipt_long_outlined,
        title: 'Ödenen vergi yok',
        message: "Vergi işaretli kategorilerdeki giderler burada görünür.",
      );
    }
    // Ödemeler en yeniden eskiye gelir; ay değiştikçe yeni bir başlık açılır.
    // Başlık yalnız gruplar, toplam yazmaz: toplamı istemci hesaplamaz.
    final months = <String, List<TaxPayment>>{};
    for (final payment in items) {
      months.putIfAbsent(payment.paidOn.substring(0, 7), () => []).add(payment);
    }
    return TaxPageBody(
      onRefresh: _reload,
      children: [
        for (final (index, month) in months.entries.indexed)
          TaxSection(
            first: index == 0,
            title: DateText.monthYear(
              int.parse(month.key.substring(0, 4)),
              int.parse(month.key.substring(5, 7)),
            ),
            child: AppCard(
              padding: EdgeInsets.zero,
              child: AppDividedColumn(
                inset: 76,
                children: [
                  for (final payment in month.value)
                    TaxPaidRow(
                      payment: payment,
                      onTap: () =>
                          showTaxPaidSheet(context, controller, payment),
                    ),
                ],
              ),
            ),
          ),
        if (error != null)
          Padding(
            padding: const EdgeInsets.only(top: AppSpacing.small),
            child: Text(
              error!.message,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: Theme.of(context).colorScheme.error,
              ),
            ),
          ),
        if (hasMore)
          Padding(
            padding: const EdgeInsets.only(top: AppSpacing.small),
            child: TextButton(
              onPressed: loading ? null : () => _load(reset: false),
              child: const Text('Daha fazla'),
            ),
          ),
      ],
    );
  }
}
