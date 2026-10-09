import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/routing/app_locations.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../pos/presentation/card_collection_fields.dart';
import '../data/obligation_repository.dart';
import 'obligation_controller.dart';
import 'obligation_prefill.dart';

class ObligationsPage extends StatefulWidget {
  const ObligationsPage({required this.controller, super.key});

  final ObligationListController controller;

  @override
  State<ObligationsPage> createState() => _ObligationsPageState();
}

class _ObligationsPageState extends State<ObligationsPage> {
  @override
  void initState() {
    super.initState();
    widget.controller.addListener(_changed);
    widget.controller.load();
  }

  @override
  void dispose() {
    widget.controller.removeListener(_changed);
    widget.controller.dispose();
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    final controller = widget.controller;
    return DefaultTabController(
      length: 3,
      child: Scaffold(
        appBar: AppBar(
          title: const Text('Yükümlülükler'),
          actions: [
            IconButton(
              onPressed: _addObligation,
              icon: const Icon(Icons.add),
              tooltip: 'Yükümlülük ekle',
            ),
          ],
          bottom: const TabBar(
            tabs: [
              Tab(text: 'Yaklaşan'),
              Tab(text: 'Geciken'),
              Tab(text: 'Kapanan'),
            ],
          ),
        ),
        body: _body(controller),
      ),
    );
  }

  /// Liste ekranı kendi kaydını açabilir; kayıt dönünce liste yeniden okunur,
  /// yoksa yeni satır ancak sekme değiştirince görünürdü.
  Future<void> _addObligation() async {
    final saved = await context.push<bool>(
      obligationListCreateLocation,
      extra: const ObligationPrefill(),
    );
    if (saved ?? false) await widget.controller.load();
  }

  Widget _body(ObligationListController controller) {
    if (controller.isLoading && controller.items.isEmpty) {
      return const AppLoadingView(message: 'Yükümlülükler yükleniyor');
    }
    if (controller.unauthorized && controller.items.isEmpty) {
      return const AppUnauthorizedView();
    }
    if (controller.errorMessage != null && controller.items.isEmpty) {
      return AppErrorView(
        message: controller.errorMessage!,
        onRetry: controller.load,
      );
    }

    final upcoming = controller.items
        .where((item) => item.status == 'open' && !item.isOverdue)
        .toList(growable: false);
    final overdue = controller.items
        .where((item) => item.status == 'open' && item.isOverdue)
        .toList(growable: false);
    final closed = controller.items
        .where((item) => item.status != 'open')
        .toList(growable: false);
    return Column(
      children: [
        if (controller.isStale)
          MaterialBanner(
            content: Text(
              '${controller.errorMessage} Son bilinen kayıtlar gösteriliyor.',
            ),
            actions: [
              TextButton(
                onPressed: controller.load,
                child: const Text('Yenile'),
              ),
            ],
          ),
        Expanded(
          child: TabBarView(
            children: [
              _list(upcoming, 'Yaklaşan yükümlülük yok.'),
              _list(overdue, 'Gecikmiş yükümlülük yok.'),
              _list(closed, 'Kapanan yükümlülük yok.'),
            ],
          ),
        ),
      ],
    );
  }

  Widget _list(List<ObligationItem> items, String emptyMessage) {
    if (items.isEmpty) {
      return AppEmptyView(
        title: emptyMessage,
        message: 'Kayıt oluştuğunda burada tarih ve durumuyla görünür.',
        icon: Icons.event_available_outlined,
      );
    }
    return RefreshIndicator(
      onRefresh: widget.controller.load,
      child: ListView.builder(
        padding: const EdgeInsets.all(AppSpacing.medium),
        itemCount: items.length,
        itemBuilder: (context, index) => Padding(
          padding: const EdgeInsets.only(bottom: AppSpacing.small),
          child: _ObligationRow(
            item: items[index],
            // Açık kayıt kapanış panelini açar (iptal de oradadır); kapanmış
            // kayıtta yapılabilecek tek şey iptaldir. İptal edilmiş kayıt
            // yalnız okunur.
            onTap: switch (items[index].status) {
              'open' => () => _openSettlement(items[index]),
              'settled' => () => _confirmCancel(items[index]),
              _ => null,
            },
          ),
        ),
      ),
    );
  }

  Future<void> _openSettlement(ObligationItem item) async {
    final result = await AppFormSheet.show<bool>(
      context: context,
      builder: (_) =>
          _SettlementForm(item: item, controller: widget.controller),
    );
    // Panel `false` ile kapandıysa kullanıcı `Kaydı iptal et`e dokundu.
    if (result == false && mounted) await _confirmCancel(item);
  }

  /// Silme yerine iptal. Kapanmış kayıtta ödeme ya da tahsilat da birlikte
  /// iptal olur; diyalog bunu söyler. Sunucu reddederse (yatış, gün sonu)
  /// sebebi alt bantta yazar ve liste değişmez.
  Future<void> _confirmCancel(ObligationItem item) async {
    final payable = item.direction == 'payable';
    final settled = item.status == 'settled';
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.block,
      destructive: true,
      title: 'Kayıt iptal edilsin mi?',
      highlight:
          '${_obligationTitle(item)} · '
          '${MoneyText.format(item.amount, item.currency)}',
      message: settled
          ? (payable
                ? 'Ödemesi de iptal edilir; para hesaba geri döner.'
                : 'Tahsilatı da iptal edilir; para hesaptan düşer.')
          : (payable
                ? 'Giderlerden ve bekleyenlerden düşer.'
                : 'Gelirlerden ve bekleyenlerden düşer.'),
      confirmLabel: 'Kaydı iptal et',
    );
    if (!confirmed || !mounted) return;
    final messenger = ScaffoldMessenger.of(context);
    final cancelled = await widget.controller.cancel(item);
    if (!mounted) return;
    messenger.showSnackBar(
      SnackBar(
        content: Text(
          cancelled
              ? 'Kayıt iptal edildi.'
              : widget.controller.errorMessage ?? 'Kayıt iptal edilemedi.',
        ),
      ),
    );
  }
}

String _obligationTitle(ObligationItem item) =>
    item.counterpartyName ??
    item.description ??
    item.categoryName ??
    'Yükümlülük';

class _ObligationRow extends StatelessWidget {
  const _ObligationRow({required this.item, this.onTap});

  final ObligationItem item;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final payable = item.direction == 'payable';
    final label = item.isOverdue
        ? 'Gecikmiş'
        : item.status == 'settled'
        ? 'Kapandı'
        : item.status == 'cancelled'
        ? 'İptal'
        : 'Yaklaşan';
    final tone = item.isOverdue
        ? AppStatusTone.expense
        : item.status == 'settled'
        ? AppStatusTone.income
        : item.status == 'cancelled'
        ? AppStatusTone.cancelled
        : AppStatusTone.planned;
    return AppCard(
      padding: EdgeInsets.zero,
      child: AppListRow(
        icon: payable ? Icons.north_east : Icons.south_west,
        title: _obligationTitle(item),
        subtitle:
            '${payable ? 'Ödenecek' : 'Tahsil edilecek'} · Vade ${DateText.dayMonth(item.dueDate)}',
        badge: AppStatusChip(
          label: label,
          icon: item.isOverdue ? Icons.warning_amber : Icons.schedule,
          tone: tone,
        ),
        trailing: AppMoneyText(amount: item.amount, currency: item.currency),
        onTap: onTap,
        dimmed: item.status != 'open',
      ),
    );
  }
}

class _SettlementForm extends StatefulWidget {
  const _SettlementForm({required this.item, required this.controller});

  final ObligationItem item;
  final ObligationListController controller;

  @override
  State<_SettlementForm> createState() => _SettlementFormState();
}

class _SettlementFormState extends State<_SettlementForm> {
  final formKey = GlobalKey<FormState>();
  final cardFields = GlobalKey<CardCollectionFieldsState>();
  late final Future<List<ObligationAccount>> accounts;
  late final TextEditingController amount;
  Future<CardCollectionSource?>? cards;
  String? accountId;
  CollectionMethod method = CollectionMethod.account;

  bool get receivable => widget.item.direction != 'payable';
  bool get byCard => method == CollectionMethod.card;

  @override
  void initState() {
    super.initState();
    accounts = widget.controller.loadAccounts();
    // Alacağın tutarı sabittir; kart önizlemesi onu okur.
    amount = TextEditingController(text: widget.item.amount);
    // Kartla tahsil yalnız alacakta (ADR 0019 T5).
    if (receivable) cards = widget.controller.loadCardCollection();
  }

  @override
  void dispose() {
    amount.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: formKey,
    child: AppFormSheet<bool>(
      title: receivable ? 'Tahsilatı kaydet' : 'Ödemeyi kaydet',
      description: byCard
          ? 'Alacak bugün kapanır; gelir yeniden yazılmaz. Para yola çıkar, '
                'hesaba yatışla geçer.'
          : 'Bu işlem yalnız hesap bakiyesini değiştirir; gelir veya gider '
                'yeniden yazılmaz.',
      submitLabel: receivable ? 'Tahsil et ve kapat' : 'Öde ve kapat',
      onSubmit: _submit,
      children: [
        AppMoneyText(
          amount: widget.item.amount,
          currency: widget.item.currency,
        ),
        const SizedBox(height: AppSpacing.medium),
        if (cards != null)
          FutureBuilder<CardCollectionSource?>(
            future: cards,
            builder: (context, snapshot) {
              final source = snapshot.data;
              if (source == null) return const SizedBox.shrink();
              return Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  CollectionMethodRail(
                    selected: method,
                    onChanged: (value) => setState(() => method = value),
                  ),
                  const SizedBox(height: AppSpacing.medium),
                  if (byCard)
                    CardCollectionFields(
                      key: cardFields,
                      source: source,
                      amount: amount,
                      collectedOn: _today(),
                    ),
                ],
              );
            },
          ),
        if (!byCard)
          FutureBuilder<List<ObligationAccount>>(
            future: accounts,
            builder: (context, snapshot) {
              if (snapshot.hasError) {
                return const Text(
                  'Hesaplar yüklenemedi. Paneli kapatıp yeniden deneyin.',
                );
              }
              if (!snapshot.hasData) return const LinearProgressIndicator();
              return DropdownButtonFormField<String>(
                initialValue: accountId,
                isExpanded: true,
                decoration: const InputDecoration(labelText: 'Hesap'),
                items: [
                  for (final account in snapshot.data!)
                    DropdownMenuItem(
                      value: account.id,
                      child: Text(account.name),
                    ),
                ],
                onChanged: (value) => accountId = value,
                validator: (value) => value == null ? 'Hesap seçin.' : null,
              );
            },
          ),
        const SizedBox(height: AppSpacing.small),
        // Yanlış yazılan kayıt buradan iptal edilir; onay sayfada sorulur.
        TextButton.icon(
          style: TextButton.styleFrom(
            minimumSize: const Size.fromHeight(48),
            foregroundColor: Theme.of(context).colorScheme.error,
          ),
          onPressed: () => Navigator.of(context).pop(false),
          icon: const Icon(Icons.block),
          label: const Text('Kaydı iptal et'),
        ),
      ],
    ),
  );

  Future<bool?> _submit() async {
    if (!formKey.currentState!.validate()) return null;
    final success = byCard
        ? await widget.controller.settle(
            widget.item,
            card: cardFields.currentState?.request,
          )
        : await widget.controller.settle(widget.item, accountId: accountId);
    return success ? true : null;
  }

  static String _today() {
    final value = DateTime.now();
    return '${value.year.toString().padLeft(4, '0')}-'
        '${value.month.toString().padLeft(2, '0')}-'
        '${value.day.toString().padLeft(2, '0')}';
  }
}
