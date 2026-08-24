import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../data/obligation_repository.dart';
import 'obligation_controller.dart';

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
            onTap: items[index].status == 'open'
                ? () => _openSettlement(items[index])
                : null,
          ),
        ),
      ),
    );
  }

  Future<void> _openSettlement(ObligationItem item) async {
    await AppFormSheet.show<bool>(
      context: context,
      builder: (_) =>
          _SettlementForm(item: item, controller: widget.controller),
    );
  }
}

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
        title:
            item.counterpartyName ??
            item.description ??
            item.categoryName ??
            'Yükümlülük',
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
  late final Future<List<ObligationAccount>> accounts;
  String? accountId;

  @override
  void initState() {
    super.initState();
    accounts = widget.controller.loadAccounts();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: formKey,
    child: AppFormSheet<bool>(
      title: widget.item.direction == 'payable'
          ? 'Ödemeyi kaydet'
          : 'Tahsilatı kaydet',
      description:
          'Bu işlem yalnız hesap bakiyesini değiştirir; gelir veya gider yeniden yazılmaz.',
      submitLabel: widget.item.direction == 'payable'
          ? 'Öde ve kapat'
          : 'Tahsil et ve kapat',
      onSubmit: _submit,
      children: [
        AppMoneyText(
          amount: widget.item.amount,
          currency: widget.item.currency,
        ),
        const SizedBox(height: AppSpacing.medium),
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
      ],
    ),
  );

  Future<bool?> _submit() async {
    if (!formKey.currentState!.validate()) return null;
    final success = await widget.controller.settle(widget.item, accountId!);
    return success ? true : null;
  }
}
