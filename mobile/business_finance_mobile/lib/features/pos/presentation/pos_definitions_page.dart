import 'package:flutter/material.dart';

import '../../../core/models/data_choice.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_row.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_status_tag.dart';
import '../../../core/widgets/app_submit_button.dart';
import '../data/pos_repository.dart';
import 'pos_controller.dart';

/// POS'larım (ADR 0019 T4): kullanıcının bir kez girdiği POS ayarları.
///
/// Tanım para taşımaz; tahsilat formunu doldurur. Yemek kartı ayrı bir özellik
/// değildir: kendi oranı ve geçiş süresi olan bir POS tanımıdır.
class PosDefinitionsPage extends StatefulWidget {
  const PosDefinitionsPage({required this.controller, super.key});

  final PosController controller;

  @override
  State<PosDefinitionsPage> createState() => _PosDefinitionsPageState();
}

class _PosDefinitionsPageState extends State<PosDefinitionsPage> {
  PosController get controller => widget.controller;

  @override
  void initState() {
    super.initState();
    controller.addListener(_changed);
    controller.loadDefinitions();
  }

  @override
  void dispose() {
    controller.removeListener(_changed);
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  Future<void> _openForm([PosDefinitionItem? definition]) =>
      openPosDefinitionForm(context, controller, definition: definition);

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(
      title: const Text("POS'larım"),
      actions: [
        IconButton(
          tooltip: 'POS ekle',
          onPressed: _openForm,
          icon: const Icon(Icons.add),
        ),
      ],
    ),
    body: SafeArea(child: _body(context)),
  );

  Widget _body(BuildContext context) {
    final definitions = controller.definitions;
    if (definitions == null) {
      if (controller.unauthorized) return const AppUnauthorizedView();
      if (controller.definitionError != null) {
        return AppErrorView(
          message: controller.definitionError!,
          onRetry: controller.loadDefinitions,
        );
      }
      return const AppLoadingView(message: "POS'lar yükleniyor");
    }
    if (definitions.isEmpty) {
      return AppEmptyView(
        icon: Icons.point_of_sale_outlined,
        title: 'Henüz POS eklenmedi',
        message:
            "POS'unuzu bir kez ekleyin; tahsilat girerken yalnız tutarı "
            'yazarsınız. Yemek kartı da bir POS olarak eklenir.',
        action: FilledButton.icon(
          onPressed: _openForm,
          icon: const Icon(Icons.add),
          label: const Text('POS ekle'),
        ),
      );
    }
    final surfaces = AppSurfaces.of(context);
    return RefreshIndicator(
      onRefresh: controller.loadDefinitions,
      child: ListView(
        padding: const EdgeInsets.all(AppSpacing.medium),
        children: [
          // Eski listeyle açık kalan ekran bunu söyler.
          if (controller.definitionError != null)
            Padding(
              padding: const EdgeInsets.only(bottom: AppSpacing.small),
              child: AppInlineNotice(
                message:
                    '${controller.definitionError} Son bilinen liste '
                    'gösteriliyor.',
                actionLabel: 'Yenile',
                onAction: controller.loadDefinitions,
              ),
            ),
          AppCard(
            padding: EdgeInsets.zero,
            child: AppDividedColumn(
              inset: AppIconCapsule.rowInset,
              children: [
                for (final definition in definitions)
                  AppRow(
                    leading: const AppIconCapsule(
                      icon: Icons.point_of_sale_outlined,
                    ),
                    title: definition.name,
                    subtitle: definition.summary,
                    // Yıldız ana POS'u seçer: tahsilat formunda o seçili
                    // gelir. Pasif POS seçilemez.
                    trailing: definition.isActive
                        ? IconButton(
                            tooltip: definition.isDefault
                                ? 'Ana POS'
                                : 'Ana POS yap',
                            isSelected: definition.isDefault,
                            onPressed:
                                definition.isDefault || controller.isSubmitting
                                ? null
                                : () => controller.setDefaultDefinition(
                                    definition,
                                  ),
                            icon: Icon(
                              Icons.star_border,
                              color: surfaces.inkMuted,
                            ),
                            selectedIcon: Icon(
                              Icons.star,
                              color: Theme.of(context).colorScheme.primary,
                            ),
                          )
                        : const AppStatusTag(
                            label: 'Pasif',
                            icon: Icons.pause_circle_outline,
                            tone: AppStatusTone.cancelled,
                          ),
                    onTap: () => _openForm(definition),
                  ),
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.only(top: AppSpacing.small),
            child: Text(
              'Yıldızlı POS tahsilat formunda seçili gelir. POS bilgisini '
              'değiştirmek geçmiş tahsilatları değiştirmez.',
              style: Theme.of(
                context,
              ).textTheme.bodySmall?.copyWith(color: surfaces.inkMuted),
            ),
          ),
        ],
      ),
    );
  }
}

/// POS tanımı formunu ayrı sayfa olarak açar; kaydedilince ya da silinince
/// `true` döner.
Future<bool?> openPosDefinitionForm(
  BuildContext context,
  PosController controller, {
  PosDefinitionItem? definition,
}) => Navigator.of(context, rootNavigator: true).push<bool>(
  MaterialPageRoute(
    builder: (_) =>
        PosDefinitionFormPage(controller: controller, definition: definition),
  ),
);

/// POS tanımı formu: ad, hesap, satış kategorisi, oran, komisyon kategorisi,
/// geçiş süresi ve iş günü seçeneği.
class PosDefinitionFormPage extends StatefulWidget {
  const PosDefinitionFormPage({
    required this.controller,
    super.key,
    this.definition,
  });

  final PosController controller;
  final PosDefinitionItem? definition;

  /// İşletme setinde hazır gelen kategoriler; varsa formda seçili açılır.
  /// Yalnız bir ön seçimdir: kullanıcı değiştirebilir, sunucu ada bakmaz.
  static const defaultSalesCategoryName = 'Satış geliri';
  static const defaultCommissionCategoryName = 'Banka ve POS komisyonu';

  @override
  State<PosDefinitionFormPage> createState() => _PosDefinitionFormPageState();
}

class _PosDefinitionFormPageState extends State<PosDefinitionFormPage> {
  final formKey = GlobalKey<FormState>();
  late final nameController = TextEditingController(
    text: widget.definition?.name ?? '',
  );
  late final rateController = TextEditingController(
    text: widget.definition == null
        ? ''
        : (PosRate.percentText(widget.definition!.commissionRate) ?? ''),
  );
  late final daysController = TextEditingController(
    text: '${widget.definition?.transferDays ?? 1}',
  );

  late final Future<PosOptions> options;
  late String? accountId = widget.definition?.accountId;
  late String? salesCategoryId = widget.definition?.salesCategoryId;
  late String? commissionCategoryId = widget.definition?.commissionCategoryId;
  late bool businessDaysOnly = widget.definition?.businessDaysOnly ?? true;
  bool defaultsApplied = false;

  PosController get controller => widget.controller;
  PosDefinitionItem? get definition => widget.definition;
  bool get editing => definition != null;

  @override
  void initState() {
    super.initState();
    options = controller.loadOptions();
    rateController.addListener(_rateChanged);
  }

  @override
  void dispose() {
    nameController.dispose();
    rateController.dispose();
    daysController.dispose();
    super.dispose();
  }

  void _rateChanged() => setState(() {});

  /// Oran sıfırdan büyükse komisyonun yazılacağı kategori de gerekir.
  bool get hasRate {
    final wire = PosRate.fractionWire(rateController.text);
    return wire != null && wire != '0.0000';
  }

  void _applyDefaults(PosOptions loaded) {
    if (defaultsApplied) return;
    defaultsApplied = true;
    if (editing) return;
    if (loaded.accounts.length == 1) accountId = loaded.accounts.single.id;
    salesCategoryId = _named(
      loaded.incomeCategories,
      PosDefinitionFormPage.defaultSalesCategoryName,
    );
    commissionCategoryId = _named(
      loaded.expenseCategories,
      PosDefinitionFormPage.defaultCommissionCategoryName,
    );
  }

  /// Adı [name] olan seçenek; yoksa ve tek seçenek varsa o, yoksa boş.
  String? _named(List<DataChoice> choices, String name) {
    for (final choice in choices) {
      if (choice.name == name) return choice.id;
    }
    return choices.length == 1 ? choices.single.id : null;
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(title: Text(editing ? "POS'u düzenle" : 'POS ekle')),
    body: SafeArea(
      child: FutureBuilder<PosOptions>(
        future: options,
        builder: (context, snapshot) {
          if (snapshot.hasError) {
            return AppErrorView(
              message: 'Hesap ve kategoriler yüklenemedi.',
              onRetry: () => Navigator.of(context).maybePop(),
            );
          }
          if (!snapshot.hasData) {
            return const AppLoadingView(message: 'Form hazırlanıyor');
          }
          _applyDefaults(snapshot.data!);
          return ListenableBuilder(
            listenable: controller,
            builder: (context, _) => _form(context, snapshot.data!),
          );
        },
      ),
    ),
  );

  Widget _form(BuildContext context, PosOptions loaded) {
    final theme = Theme.of(context);
    return Form(
      key: formKey,
      child: ListView(
        padding: const EdgeInsets.all(AppSpacing.medium),
        children: [
          TextFormField(
            controller: nameController,
            hintLocales: const [Locale('tr', 'TR')],
            textCapitalization: TextCapitalization.sentences,
            maxLength: 80,
            decoration: const InputDecoration(
              labelText: 'POS adı',
              hintText: 'ör. Ziraat POS, Yemek kartı',
              counterText: '',
            ),
            validator: (value) => value == null || value.trim().isEmpty
                ? 'POS için bir ad yazın.'
                : null,
          ),
          const SizedBox(height: AppSpacing.medium),
          if (loaded.accounts.isEmpty)
            const AppInlineNotice(
              message:
                  'POS parası bir banka hesabına geçer. Önce Hesaplar '
                  'ekranından bir banka hesabı açın.',
            )
          else
            DropdownButtonFormField<String>(
              initialValue: _valid(accountId, loaded.accounts),
              isExpanded: true,
              decoration: const InputDecoration(
                labelText: 'Paranın geçeceği hesap',
              ),
              items: [
                for (final account in loaded.accounts)
                  DropdownMenuItem(
                    value: account.id,
                    child: Text(account.name),
                  ),
              ],
              onChanged: (value) => setState(() => accountId = value),
              validator: (value) => value == null ? 'Hesap seçin.' : null,
            ),
          const SizedBox(height: AppSpacing.medium),
          DropdownButtonFormField<String>(
            initialValue: _valid(salesCategoryId, loaded.incomeCategories),
            isExpanded: true,
            decoration: const InputDecoration(labelText: 'Satış kategorisi'),
            items: [
              for (final category in loaded.incomeCategories)
                DropdownMenuItem(
                  value: category.id,
                  child: Text(category.name),
                ),
            ],
            onChanged: (value) => setState(() => salesCategoryId = value),
            validator: (value) =>
                value == null ? 'Satışın yazılacağı kategoriyi seçin.' : null,
          ),
          const SizedBox(height: AppSpacing.medium),
          TextFormField(
            controller: rateController,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: const InputDecoration(
              labelText: 'Komisyon oranı (%)',
              hintText: 'ör. 1,79',
              helperText:
                  'Komisyon kesilmiyorsa boş bırakın. Oran her tahsilatta '
                  'tutara çevrilir.',
              helperMaxLines: 3,
            ),
            validator: (value) => PosRate.fractionWire(value ?? '') == null
                ? 'Oranı yüzde olarak yazın (ör. 1,79).'
                : null,
          ),
          if (hasRate || commissionCategoryId != null) ...[
            const SizedBox(height: AppSpacing.medium),
            DropdownButtonFormField<String>(
              key: ValueKey('commission-$commissionCategoryId'),
              initialValue: _valid(
                commissionCategoryId,
                loaded.expenseCategories,
              ),
              isExpanded: true,
              decoration: const InputDecoration(
                labelText: 'Komisyon gider kategorisi',
              ),
              items: [
                for (final category in loaded.expenseCategories)
                  DropdownMenuItem(
                    value: category.id,
                    child: Text(category.name),
                  ),
              ],
              onChanged: (value) =>
                  setState(() => commissionCategoryId = value),
              validator: (value) => hasRate && value == null
                  ? 'Komisyonun yazılacağı kategoriyi seçin.'
                  : null,
            ),
          ],
          const SizedBox(height: AppSpacing.medium),
          TextFormField(
            controller: daysController,
            keyboardType: TextInputType.number,
            decoration: const InputDecoration(
              labelText: 'Para kaç günde hesaba geçer',
              helperText: 'Aynı gün geçiyorsa 0 yazın.',
            ),
            validator: (value) {
              final days = int.tryParse((value ?? '').trim());
              return days == null || days < 0 || days > 60
                  ? '0 ile 60 arasında bir gün sayısı yazın.'
                  : null;
            },
          ),
          SwitchListTile(
            contentPadding: EdgeInsets.zero,
            title: const Text('İş günü say'),
            subtitle: const Text(
              'Hafta sonunu atlar; beklenen gün cumartesi ya da pazara düşmez.',
            ),
            value: businessDaysOnly,
            onChanged: (value) => setState(() => businessDaysOnly = value),
          ),
          if (controller.definitionError != null)
            Padding(
              padding: const EdgeInsets.only(top: AppSpacing.small),
              child: Semantics(
                liveRegion: true,
                child: Text(
                  controller.definitionError!,
                  style: theme.textTheme.bodyMedium?.copyWith(
                    color: theme.colorScheme.error,
                  ),
                ),
              ),
            ),
          const SizedBox(height: AppSpacing.large),
          AppSubmitButton(
            label: 'Kaydet',
            icon: Icons.check,
            isBusy: controller.isSubmitting,
            onSubmit: loaded.accounts.isEmpty ? null : _submit,
          ),
          if (editing) ...[
            const SizedBox(height: AppSpacing.small),
            OutlinedButton.icon(
              style: OutlinedButton.styleFrom(
                minimumSize: const Size.fromHeight(48),
              ),
              onPressed: controller.isSubmitting ? null : _toggleActive,
              icon: Icon(
                definition!.isActive
                    ? Icons.pause_circle_outline
                    : Icons.play_circle_outline,
              ),
              label: Text(definition!.isActive ? 'Pasife al' : 'Etkinleştir'),
            ),
            TextButton.icon(
              style: TextButton.styleFrom(
                minimumSize: const Size.fromHeight(48),
                foregroundColor: theme.colorScheme.error,
              ),
              onPressed: controller.isSubmitting ? null : _delete,
              icon: const Icon(Icons.delete_outline),
              label: const Text('Sil'),
            ),
          ],
        ],
      ),
    );
  }

  /// Seçili değer artık seçeneklerde yoksa (pasife alınmış hesap ya da
  /// kategori) alan boş açılır ve kullanıcıdan yeniden seçmesi istenir.
  String? _valid(String? id, List<DataChoice> choices) =>
      choices.any((choice) => choice.id == id) ? id : null;

  Future<void> _submit() async {
    if (!formKey.currentState!.validate()) return;
    final saved = await controller.saveDefinition(
      PosDefinitionInput(
        name: nameController.text.trim(),
        accountId: accountId!,
        salesCategoryId: salesCategoryId!,
        commissionRate: PosRate.fractionWire(rateController.text)!,
        commissionCategoryId: commissionCategoryId,
        transferDays: int.parse(daysController.text.trim()),
        businessDaysOnly: businessDaysOnly,
      ),
      definitionId: definition?.id,
    );
    if (saved && mounted) Navigator.of(context).pop(true);
  }

  Future<void> _toggleActive() async {
    final saved = await controller.setDefinitionActive(
      definition!,
      !definition!.isActive,
    );
    if (saved && mounted) Navigator.of(context).pop(true);
  }

  Future<void> _delete() async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      destructive: true,
      subject: AppConfirmSubject(
        title: definition!.name,
        detail: definition!.summary,
      ),
      message:
          'POS silinir. Bu POS ile tahsilat yazıldıysa silinemez; pasife '
          'alabilirsiniz.',
      confirmLabel: 'Sil',
    );
    if (!confirmed || !mounted) return;
    final deleted = await controller.deleteDefinition(definition!);
    if (deleted && mounted) Navigator.of(context).pop(true);
  }
}
