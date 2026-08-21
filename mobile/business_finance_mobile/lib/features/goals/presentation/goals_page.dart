import 'package:flutter/material.dart';

import '../../../core/formatters/money_input.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/data_choice.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_state_views.dart';
import '../data/goal_models.dart';
import '../data/goal_repository.dart';
import 'goals_controller.dart';

/// Tasarruf hedefleri.
///
/// Veri Araçları'nın bir sekmesiydi; CSV ve yedekle aynı çekmecede duruyordu.
/// Kendi ekranı olunca yalnız kendi verisini çekiyor.
class GoalsPage extends StatefulWidget {
  const GoalsPage({required this.repository, super.key});

  final GoalRepositoryContract repository;

  @override
  State<GoalsPage> createState() => _GoalsPageState();
}

class _GoalsPageState extends State<GoalsPage> {
  late final GoalsController controller;

  @override
  void initState() {
    super.initState();
    controller = GoalsController(widget.repository)..addListener(_changed);
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
    appBar: AppBar(title: const Text('Tasarruf hedefleri')),
    body: _body(),
  );

  Widget _body() {
    if (controller.unauthorized) return const AppUnauthorizedView();
    if (controller.isLoading && controller.snapshot == null) {
      return const AppLoadingView(message: 'Hedefler yükleniyor');
    }
    if (controller.snapshot == null) {
      return AppErrorView(
        message: controller.errorMessage ?? 'Hedefler alınamadı.',
        onRetry: controller.load,
      );
    }

    final goals = controller.snapshot!.goals;
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
        Expanded(
          child: RefreshIndicator(
            onRefresh: controller.load,
            child: ListView(
              padding: const EdgeInsets.all(AppSpacing.medium),
              children: [
                FilledButton.icon(
                  onPressed: controller.isSubmitting ? null : _showForm,
                  icon: const Icon(Icons.flag_outlined),
                  label: const Text('Tasarruf hedefi ekle'),
                ),
                const SizedBox(height: AppSpacing.medium),
                if (goals.isEmpty)
                  const SizedBox(
                    height: 220,
                    child: AppEmptyView(
                      icon: Icons.savings_outlined,
                      title: 'Tasarruf hedefi yok',
                      message: 'Henüz tasarruf hedefi yok.',
                    ),
                  ),
                for (final goal in goals) _goalCard(goal),
              ],
            ),
          ),
        ),
      ],
    );
  }

  Widget _goalCard(GoalItem goal) => Padding(
    padding: const EdgeInsets.only(bottom: AppSpacing.medium),
    child: AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(goal.name, style: Theme.of(context).textTheme.titleMedium),
          Text(
            '${MoneyText.format(goal.allocated, 'TRY')} / '
            '${MoneyText.format(goal.target, 'TRY')} • '
            '${_statusLabel(goal.status)}',
          ),
          const SizedBox(height: AppSpacing.small),
          LinearProgressIndicator(
            value: (double.tryParse(goal.progress) ?? 0) / 100,
          ),
          Text(
            'Kalan ${MoneyText.format(goal.remaining, 'TRY')} • '
            '%${MoneyText.percent(goal.progress)}',
          ),
          const SizedBox(height: AppSpacing.small),
          _modeNote(goal),
          const Divider(height: AppSpacing.large),
          _goalActions(goal),
        ],
      ),
    ),
  );

  /// Hedefin ilerlemesinin nereden geldiğini söyleyen satır.
  ///
  /// Bu satır olmadan iki mod ekranda birbirinin aynısı görünüyordu ve manuel
  /// hedefteki katkı gerçek bir para hareketi sanılıyordu. Oysa manuel modda
  /// hiçbir hesaptan para çıkmaz: tutulan şey bir nottur. Bakiye modunda ise
  /// tersine, kullanıcı hiçbir şey girmez — sayı hesabın kendi bakiyesidir.
  Widget _modeNote(GoalItem goal) {
    final theme = Theme.of(context);
    final muted = AppSurfaces.of(context).inkMuted;
    final linked = _accountName(goal.accountId);
    final (label, icon, explanation) = goal.isManual
        ? (
            'Elle takip',
            Icons.edit_note_outlined,
            'Katkılar yalnız kayıttır; hesaplarınızdan para çıkmaz.',
          )
        : (
            'Hesap bakiyesi',
            Icons.account_balance_outlined,
            linked == null
                ? 'Bağlı hesabın bakiyesini izler.'
                : '$linked hesabının bakiyesini izler.',
          );

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        AppStatusChip(label: label, icon: icon, tone: AppStatusTone.neutral),
        const SizedBox(height: AppSpacing.xSmall),
        Text(
          explanation,
          style: theme.textTheme.bodySmall?.copyWith(color: muted),
        ),
      ],
    );
  }

  /// Bağlı hesabın adı; hesap listede yoksa `null`.
  ///
  /// Pasifleştirilmiş bir hesap seçim listesinden düşebilir; o durumda adı
  /// uydurmak yerine genel cümle yazılıyor.
  String? _accountName(String? accountId) {
    if (accountId == null) return null;
    for (final account
        in controller.snapshot?.accounts ?? const <DataChoice>[]) {
      if (account.id == accountId) return account.name;
    }
    return null;
  }

  /// Hedefin eylemleri.
  ///
  /// Önceden iki `TextButton` alt alta, kartın tüm genişliğine yayılmış hâlde
  /// duruyordu: ikisi de aynı ağırlıkta görünüyor, hangisinin yıkıcı olduğu
  /// yalnız metinden anlaşılıyordu. Artık kendi ayrılmış satırlarında ve iki
  /// ayrı biçimde: katkı dolgulu bir eylem, silme kırmızı kenarlıklı.
  ///
  /// Katkı butonu **gelir yeşiline** boyanmıyor. Manuel hedefte katkı gerçek
  /// bir para hareketi değil, yalnız bir kayıt; yeşil onu para girişi gibi
  /// gösterirdi ve "yeşil yalnız gelirdir" kuralını bozardı.
  Widget _goalActions(GoalItem goal) {
    final scheme = Theme.of(context).colorScheme;
    return Wrap(
      spacing: AppSpacing.small,
      runSpacing: AppSpacing.small,
      children: [
        if (goal.isManual)
          FilledButton.tonalIcon(
            onPressed: controller.isSubmitting ? null : () => _contribute(goal),
            icon: const Icon(Icons.add),
            label: const Text('Katkı ekle'),
          ),
        OutlinedButton.icon(
          onPressed: controller.isSubmitting
              ? null
              : () => _confirmDelete(goal),
          icon: const Icon(Icons.delete_outline),
          label: const Text('Hedefi sil'),
          style: OutlinedButton.styleFrom(
            foregroundColor: scheme.error,
            side: BorderSide(color: scheme.error),
          ),
        ),
      ],
    );
  }

  Future<void> _showForm() async {
    final payload = await AppFormSheet.show<Map<String, dynamic>>(
      context: context,
      builder: (_) => _GoalForm(accounts: controller.snapshot!.accounts),
    );
    if (payload == null) return;
    await controller.create(payload);
  }

  Future<void> _confirmDelete(GoalItem goal) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text('Tasarruf hedefi silinsin mi?'),
        content: Text(
          '${goal.name} kalıcı olarak silinecek. Katkı geçmişi bulunan '
          'hedefler silinemez.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(dialogContext, false),
            child: const Text('Vazgeç'),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(dialogContext, true),
            child: const Text('Hedefi sil'),
          ),
        ],
      ),
    );
    if (confirmed == true) await controller.delete(goal.id);
  }

  Future<void> _contribute(GoalItem goal) async {
    final amount = await AppFormSheet.show<String>(
      context: context,
      builder: (_) => _ContributionForm(goalName: goal.name),
    );
    if (amount == null) return;
    await controller.contribute(goal.id, amount);
  }

  static String _statusLabel(String status) => switch (status) {
    'completed' => 'Tamamlandı',
    'overdue' => 'Süresi geçti',
    'active' => 'Aktif',
    _ => 'Bilinmeyen durum',
  };
}

/// Tasarruf hedefi.
class _GoalForm extends StatefulWidget {
  const _GoalForm({required this.accounts});

  final List<DataChoice> accounts;

  @override
  State<_GoalForm> createState() => _GoalFormState();
}

class _GoalFormState extends State<_GoalForm> {
  final _formKey = GlobalKey<FormState>();
  final _name = TextEditingController();
  final _target = TextEditingController();
  var _mode = 'manual-contributions';
  String? _accountId;
  late var _targetDate = AppDateField.format(
    DateTime.now().add(const Duration(days: 365)),
  );

  @override
  void dispose() {
    _name.dispose();
    _target.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: _formKey,
    child: AppFormSheet<Map<String, dynamic>>(
      title: 'Tasarruf hedefi',
      submitLabel: 'Oluştur',
      onSubmit: _mode == 'account-balance' && _accountId == null
          ? null
          : () async {
              if (!(_formKey.currentState?.validate() ?? false)) return null;
              return {
                'name': _name.text.trim(),
                'targetAmount': MoneyInput.wire(_target.text),
                'currency': 'TRY',
                'targetDate': _targetDate,
                'trackingMode': _mode,
                'accountId': _accountId,
                'description': null,
              };
            },
      children: [
        AppFormField(
          child: TextFormField(
            controller: _name,
            hintLocales: const [Locale('tr', 'TR')],
            decoration: const InputDecoration(labelText: 'Hedef adı'),
            validator: (value) => value == null || value.trim().isEmpty
                ? 'Hedef adı zorunludur.'
                : null,
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: _target,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: const InputDecoration(
              labelText: 'Hedef tutar',
              suffixText: 'TRY',
            ),
            validator: MoneyInput.positiveError,
          ),
        ),
        AppFormField(
          child: AppDateField(
            label: 'Hedef tarihi',
            value: _targetDate,
            onChanged: (value) => setState(() => _targetDate = value),
          ),
        ),
        AppFormField(
          child: DropdownButtonFormField<String>(
            initialValue: _mode,
            isExpanded: true,
            decoration: InputDecoration(
              labelText: 'İlerleme kaynağı',

              // İki mod adları dışında hiçbir şey söylemiyordu ve aralarındaki
              // asıl fark buydu: biri para taşıdığınızı varsayar, diğeri
              // taşımadığınızı.
              helperText: _mode == 'manual-contributions'
                  ? 'Tutarı siz girersiniz; para hareket etmez.'
                  : 'Seçtiğiniz hesabın bakiyesi ilerlemeyi belirler.',
              helperMaxLines: 2,
            ),
            items: const [
              DropdownMenuItem(
                value: 'manual-contributions',
                child: Text('Manuel katkılar'),
              ),
              DropdownMenuItem(
                value: 'account-balance',
                child: Text('Hesap bakiyesi'),
              ),
            ],
            onChanged: (value) => setState(() {
              _mode = value!;
              _accountId = null;
            }),
          ),
        ),
        if (_mode == 'account-balance')
          AppFormField(
            child: DropdownButtonFormField<String>(
              initialValue: _accountId,
              isExpanded: true,
              decoration: const InputDecoration(labelText: 'Bağlı hesap'),
              items: [
                for (final item in widget.accounts)
                  DropdownMenuItem(value: item.id, child: Text(item.name)),
              ],
              onChanged: (value) => setState(() => _accountId = value),
            ),
          ),
      ],
    ),
  );
}

/// Hedefe manuel katkı.
class _ContributionForm extends StatefulWidget {
  const _ContributionForm({required this.goalName});

  final String goalName;

  @override
  State<_ContributionForm> createState() => _ContributionFormState();
}

class _ContributionFormState extends State<_ContributionForm> {
  final _formKey = GlobalKey<FormState>();
  final _amount = TextEditingController();

  @override
  void dispose() {
    _amount.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: _formKey,
    child: AppFormSheet<String>(
      title: '${widget.goalName} katkısı',
      submitLabel: 'Ekle',
      onSubmit: () async {
        if (!(_formKey.currentState?.validate() ?? false)) return null;
        return MoneyInput.wire(_amount.text);
      },
      children: [
        AppFormField(
          child: TextFormField(
            controller: _amount,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: const InputDecoration(
              labelText: 'Tutar',
              suffixText: 'TRY',

              // Bu uyarı alanın hemen altında: kullanıcı tutarı yazarken
              // parasının taşınacağını sanıyordu. Kartta da yazıyor ama
              // eylemin yapıldığı yerde tekrar edilmesi gerekiyor.
              helperText: 'Bu kayıt para taşımaz; hesap bakiyeniz değişmez.',
              helperMaxLines: 2,
            ),
            validator: MoneyInput.positiveError,
          ),
        ),
      ],
    ),
  );
}
