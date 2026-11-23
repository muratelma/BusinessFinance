import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/presentation/loadable_view_model.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_no_results_view.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../data/debt_book_models.dart';
import '../data/debt_book_repository.dart';
import 'debt_book_parts.dart';

/// `Kapananlar`: seçili taraftaki kapanmış taksitli ve kişisiz kayıtlar
/// (`05-EKRAN-TASARIMI.md` §3.1, liste sonu). Kayıt sayfası buradan da
/// açılır; ödeme geri alınırsa kayıt yeniden açık listeye döner.
class DebtClosedPage extends StatefulWidget {
  const DebtClosedPage({
    required this.repository,
    required this.side,
    super.key,
    this.recordPage,
  });

  final DebtBookRepositoryContract repository;
  final DebtSide side;
  final Widget Function(String debtId, String title)? recordPage;

  @override
  State<DebtClosedPage> createState() => _DebtClosedPageState();
}

class _DebtClosedPageState extends State<DebtClosedPage> {
  late final _DebtClosedController _controller = _DebtClosedController(
    widget.repository,
  );

  @override
  void initState() {
    super.initState();
    _controller.load();
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  Future<void> _open(DebtClosedItem item) async {
    final builder = widget.recordPage;
    if (builder == null) return;
    await Navigator.of(context).push(
      MaterialPageRoute<void>(builder: (_) => builder(item.id, item.title)),
    );
    if (mounted) await _controller.load();
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      bottom: false,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppPageHeader(
            title: 'Kapananlar',
            onBack: () => Navigator.maybePop(context),
          ),
          Expanded(
            child: AnimatedBuilder(
              animation: _controller,
              builder: (context, _) => _body(),
            ),
          ),
        ],
      ),
    ),
  );

  Widget _body() {
    final items = _controller.items;
    if (_controller.unauthorized) return const AppUnauthorizedView();
    if (items == null) {
      return _controller.errorMessage != null
          ? AppErrorView(
              message: _controller.errorMessage!,
              onRetry: _controller.load,
            )
          : const AppLoadingView(message: 'Kapananlar yükleniyor');
    }
    final rows = [
      for (final item in items)
        if (item.side == widget.side) item,
    ];
    if (rows.isEmpty) {
      return AppNoResultsView(
        reason: AppNoResultsReason.filter,
        title: widget.side == DebtSide.receivable
            ? 'Kapanan alacak yok'
            : 'Kapanan borç yok',
      );
    }
    return RefreshIndicator(
      onRefresh: _controller.load,
      child: ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: AppPageInsets.aboveBar(context, AppPageInsets.list),
        children: [
          DebtSectionCard(
            indent: AppIconCapsule.rowInset,
            children: [
              for (final item in rows)
                _ClosedRow(item: item, onTap: () => _open(item)),
            ],
          ),
        ],
      ),
    );
  }
}

class _ClosedRow extends StatelessWidget {
  const _ClosedRow({required this.item, required this.onTap});

  final DebtClosedItem item;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final parts = [
      'Kapandı ${DateText.dayMonthYear(item.closedOn)}',
      if (item.counterpartyName != null && item.counterpartyName != item.title)
        item.counterpartyName!,
    ];
    return DebtRowFrame(
      leading: AppIconCapsule(
        icon: item.isInstallment
            ? Icons.event_repeat
            : Icons.receipt_long_outlined,
        size: AppSizes.rowButton,
      ),
      title: item.title,
      trailing: DebtRowAmount(item.amount),
      subtitle: DebtRowSubtitle(parts: parts),
      onTap: onTap,
      semanticLabel: [
        item.title,
        MoneyText.format(item.amount, 'TRY'),
        ...parts,
      ].join('. '),
    );
  }
}

class _DebtClosedController extends LoadableViewModel {
  _DebtClosedController(this._repository);

  final DebtBookRepositoryContract _repository;
  List<DebtClosedItem>? items;

  bool get unauthorized => error?.isUnauthorized ?? false;
  String? get errorMessage => error?.message;

  Future<void> load() => loadSafely(() async {
    items = await _repository.loadClosed();
  });
}
