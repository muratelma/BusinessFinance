import 'package:flutter/material.dart';

import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../data/debt_book_models.dart';
import '../data/debt_book_repository.dart';
import 'debt_book_controller.dart';
import 'debt_book_view.dart';
import 'debt_entry_sheet.dart';

/// `Borç ve alacak` sayfası (Faz 5): listeyi sunucudan okur ve
/// [DebtBookView] ile çizer. Kişi satırı [personPage]'i açar; yeni kayıt
/// formları ve kayıt sayfası Faz 7'de bağlanır, o zamana kadar kayıt satırı
/// `+` `Ne oldu?` sonrası [entryPage]'i, kayıt satırı [recordPage]'i açar.
class DebtBookPage extends StatefulWidget {
  const DebtBookPage({
    required this.repository,
    super.key,
    this.scopeController,
    this.changes,
    this.personPage,
    this.recordPage,
    this.onOpenClosed,
    this.entryPage,
  });

  final DebtBookRepositoryContract repository;
  final ScopeController? scopeController;
  final FinancialDataChanges? changes;

  /// Kişinin sayfası (kimlik, ad); sayfa bu sayfanın üstüne açılır ve
  /// dönüşte liste yenilenir.
  final Widget Function(String counterpartyId, String name)? personPage;

  /// Taksitli ya da kişisiz kaydın sayfası (kimlik, başlık).
  final Widget Function(String debtId, String title)? recordPage;
  final ValueChanged<DebtSide>? onOpenClosed;

  /// `Ne oldu?` panelinde seçilen kayıt türü.
  /// Yeni kayıt formu (tür, varsa kişi); kaydedilince liste yenilenir.
  final Widget Function(DebtEntryKind kind, DebtPersonChoice? person)?
  entryPage;

  @override
  State<DebtBookPage> createState() => _DebtBookPageState();
}

class _DebtBookPageState extends State<DebtBookPage> {
  late final DebtBookController _controller = DebtBookController(
    widget.repository,
    scopeController: widget.scopeController,
    changes: widget.changes,
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

  Future<void> _add() async {
    final kind = await AppAdaptiveSheet.show<DebtEntryKind>(
      context: context,
      builder: (_) => const DebtEntrySheet(),
    );
    final builder = widget.entryPage;
    if (kind == null || builder == null || !mounted) return;
    await Navigator.of(
      context,
    ).push(MaterialPageRoute<bool>(builder: (_) => builder(kind, null)));
    if (mounted) await _controller.load();
  }

  Future<void> _openPerson(String id, String name) async {
    final builder = widget.personPage;
    if (builder == null) return;
    await Navigator.of(
      context,
    ).push(MaterialPageRoute<void>(builder: (_) => builder(id, name)));
    if (mounted) await _controller.load();
  }

  Future<void> _openRecord(String id, String title) async {
    final builder = widget.recordPage;
    if (builder == null) return;
    await Navigator.of(
      context,
    ).push(MaterialPageRoute<void>(builder: (_) => builder(id, title)));
    if (mounted) await _controller.load();
  }

  void _openItem(DebtBookItem item) {
    if (item.isLedger) {
      _openPerson(item.id, item.title);
    } else {
      _openRecord(item.id, item.title);
    }
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      bottom: false,
      child: AnimatedBuilder(
        animation: _controller,
        builder: (context, _) {
          final book = _controller.book;
          void back() => Navigator.maybePop(context);
          if (_controller.unauthorized) {
            return _framed(back, const AppUnauthorizedView());
          }
          if (book == null) {
            return _framed(
              back,
              _controller.errorMessage != null
                  ? AppErrorView(
                      message: _controller.errorMessage!,
                      onRetry: _controller.load,
                    )
                  : const AppLoadingView(message: 'Borç ve alacak yükleniyor'),
            );
          }
          return DebtBookView(
            // Kapsam değişince sekme ve arama baştan kurulmaz; liste yenilenir.
            book: book,
            scope: _controller.scope,
            scopeVisible: _controller.scopeVisible,
            onScope: _controller.selectScope,
            onBack: back,
            onAdd: _add,
            onOpenItem: _openItem,
            onOpenPerson: (person) =>
                _openPerson(person.counterpartyId, person.name),
            onOpenClosed: widget.onOpenClosed,
            onRefresh: _controller.load,
            notice: _controller.isStale
                ? AppInlineNotice(
                    margin: EdgeInsets.zero,
                    message: 'Liste güncellenemedi.',
                    actionLabel: 'Tekrar dene',
                    onAction: _controller.load,
                  )
                : null,
          );
        },
      ),
    ),
  );

  Widget _framed(VoidCallback back, Widget body) => Column(
    crossAxisAlignment: CrossAxisAlignment.stretch,
    children: [
      AppPageHeader(title: 'Borç ve alacak', onBack: back),
      Expanded(child: body),
    ],
  );
}
