import 'package:flutter/material.dart';

import '../../../core/formatters/money_text.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../pos/data/pos_repository.dart';
import '../data/counterparty_models.dart';
import '../data/counterparty_repository.dart';
import 'counterparties_controller.dart';
import 'counterparty_detail_page.dart';
import 'counterparty_forms.dart';

/// Cari hesap: kiminle ne alacağın, kime ne borcun var.
///
/// Taksitli sözleşmeler kendi ekranında kalıyor (`Borç ve alacaklar`): ikisi
/// aynı kişiye ait olsa bile farklı sorular sorar — biri yürüyen bir hesap,
/// diğeri vadesi belli bir plan. Kişinin ayrıntısında ikisi birlikte görünür.
class CounterpartiesPage extends StatefulWidget {
  const CounterpartiesPage({
    required this.repository,
    this.changes,
    this.posRepository,
    super.key,
  });

  final CounterpartyRepositoryContract repository;

  /// Tahsilatın kartla (POS) alınabilmesi için; yoksa seçenek görünmez.
  final PosRepositoryContract? posRepository;
  final FinancialDataChanges? changes;

  @override
  State<CounterpartiesPage> createState() => _CounterpartiesPageState();
}

class _CounterpartiesPageState extends State<CounterpartiesPage> {
  late final CounterpartiesController controller;

  @override
  void initState() {
    super.initState();
    controller = CounterpartiesController(
      widget.repository,
      financialDataChanges: widget.changes,
      posRepository: widget.posRepository,
    )..addListener(_changed);
    controller.load();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  @override
  void dispose() {
    controller.removeListener(_changed);
    controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(title: const Text('Cari hesap')),
    body: _body(),
  );

  Widget _body() {
    if (controller.unauthorized) return const AppUnauthorizedView();
    if (controller.isLoading && controller.snapshot == null) {
      return const AppLoadingView(message: 'Cari hesaplar yükleniyor');
    }
    if (controller.snapshot == null) {
      return AppErrorView(
        message: controller.errorMessage ?? 'Cari hesaplar alınamadı.',
        onRetry: controller.load,
      );
    }

    final people = controller.counterparties;
    return Column(
      children: [
        if (controller.isLoading || controller.isSubmitting)
          const LinearProgressIndicator(),
        if (controller.errorMessage != null ||
            controller.successMessage != null)
          MaterialBanner(
            content: Text(
              controller.errorMessage ?? controller.successMessage!,
            ),
            leading: Icon(
              controller.errorMessage == null
                  ? Icons.check_circle_outline
                  : Icons.error_outline,
            ),
            actions: [
              TextButton(
                onPressed: controller.clearMessage,
                child: const Text('Kapat'),
              ),
            ],
          ),
        // Elde kalan son okuma: sunucuya ulaşılamadı ama ekran boş kalmadı.
        // Kullanıcı gördüğü sayının tazeliğini bilmeli.
        if (controller.isStale)
          const Padding(
            padding: EdgeInsets.symmetric(horizontal: AppSpacing.medium),
            child: AppStatusChip(
              label: 'Son bilinen bakiye',
              icon: Icons.history,
              tone: AppStatusTone.cancelled,
            ),
          ),
        Expanded(
          child: RefreshIndicator(
            onRefresh: controller.load,
            child: ListView(
              padding: const EdgeInsets.all(AppSpacing.medium),
              children: [
                FilledButton.icon(
                  onPressed: controller.isSubmitting ? null : _addCounterparty,
                  icon: const Icon(Icons.person_add_alt),
                  label: const Text('Karşı taraf ekle'),
                ),
                const SizedBox(height: AppSpacing.medium),
                _filters(),
                const SizedBox(height: AppSpacing.small),
                if (people.isEmpty) _empty(),
                for (final person in people) _row(person),
              ],
            ),
          ),
        ),
      ],
    );
  }

  Widget _empty() => SizedBox(
    height: 220,
    child: AppEmptyView(
      icon: Icons.people_outline,
      title: switch (controller.filter) {
        CounterpartyBalanceFilter.open => 'Açık hesap yok',
        CounterpartyBalanceFilter.settled => 'Kapanmış cari yok',
        CounterpartyBalanceFilter.all => 'Karşı taraf yok',
      },
      message: switch (controller.filter) {
        CounterpartyBalanceFilter.open =>
          'Kimseye borcunuz, kimsenin size borcu yok.',
        CounterpartyBalanceFilter.settled =>
          'Hesabı kapanmış bir karşı taraf yok.',
        CounterpartyBalanceFilter.all =>
          'Veresiye sattığınız ya da vadeli aldığınız kişileri buraya ekleyin.',
      },
    ),
  );

  /// Kapanmış cari listeden düşmez, yalnız ayrı okunur: bir müşteriyle hesabın
  /// kapanmış olması onunla iş yapılmadığı anlamına gelmez.
  Widget _filters() => Wrap(
    spacing: AppSpacing.small,
    children: [
      for (final filter in CounterpartyBalanceFilter.values)
        ChoiceChip(
          label: Text(filter.label),
          selected: controller.filter == filter,
          onSelected: controller.isLoading
              ? null
              : (_) => controller.changeFilter(filter),
        ),
    ],
  );

  Widget _row(CounterpartySummary person) => AppListRow(
    key: ValueKey(person.id),
    onTap: () => _openDetail(person),
    icon: person.isActive ? Icons.person_outline : Icons.person_off_outlined,
    title: person.name,
    subtitle: _subtitle(person),
    badge: _badges(person),
    // Net tek sayıya iner ama işaret kaybolmaz: eksi, bizim ona borçlu
    // olduğumuz anlamına gelir ve gider tonunda okunur.
    trailing: AppMoneyText(
      amount: person.net,
      currency: 'TRY',
      effect: person.isSettled
          ? AppMoneyEffect.neutral
          : person.isReceivableSide
          ? AppMoneyEffect.income
          : AppMoneyEffect.expense,
    ),
  );

  Widget? _badges(CounterpartySummary person) {
    if (person.isActive &&
        !person.hasOverdueReceivable &&
        !person.hasOverduePayable) {
      return null;
    }

    return Wrap(
      spacing: AppSpacing.small,
      runSpacing: AppSpacing.xSmall,
      children: [
        if (!person.isActive)
          const AppStatusChip(
            label: 'Pasif',
            icon: Icons.pause_circle_outline,
            tone: AppStatusTone.cancelled,
          ),
        if (person.hasOverdueReceivable)
          AppStatusChip(
            label:
                'Vadesi geçmiş alacak '
                '${MoneyText.format(person.overdueReceivable, 'TRY')}',
            icon: Icons.schedule,
            tone: AppStatusTone.planned,
          ),
        if (person.hasOverduePayable)
          AppStatusChip(
            label:
                'Vadesi geçmiş borç '
                '${MoneyText.format(person.overduePayable, 'TRY')}',
            icon: Icons.schedule,
            tone: AppStatusTone.planned,
          ),
      ],
    );
  }

  String _subtitle(CounterpartySummary person) {
    if (person.isSettled) return 'Hesap kapandı';
    return person.isReceivableSide
        ? 'Sizden alacağı yok, size borçlu'
        : 'Siz borçlusunuz';
  }

  Future<void> _openDetail(CounterpartySummary person) async {
    await Navigator.of(context).push(
      MaterialPageRoute<void>(
        builder: (_) => CounterpartyDetailPage(
          controller: controller,
          counterpartyId: person.id,
        ),
      ),
    );
    // Ayrıntıda yazılan bir hareket listedeki bakiyeyi de değiştirir.
    if (mounted) await controller.load();
  }

  Future<void> _addCounterparty() async {
    await AppFormSheet.show<Map<String, Object?>>(
      context: context,
      builder: (_) => CounterpartyForm(
        onSave: (payload) => controller.refusalOf(
          controller.create(
            payload['name']! as String,
            payload['note'] as String?,
          ),
        ),
      ),
    );
  }
}
