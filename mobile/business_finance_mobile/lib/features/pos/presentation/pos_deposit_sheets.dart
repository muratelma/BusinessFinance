import 'dart:async';

import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_input.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/network/client_request_id.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_detail_block.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_status_tag.dart';
import '../data/pos_repository.dart';
import 'pos_controller.dart';
import 'pos_deposit_controller.dart';

/// Tahsilatın başlığı: kullanıcının yazdığı açıklama, yoksa günü.
/// Kartla tahsilde satırın adı parayı ödeyen kişidir (ADR 0019 T5): satış
/// değildir ve "gün sonu" diye okunmamalıdır.
String posSettlementTitle(PosSettlementItem item) => item.isCollection
    ? (item.description ?? item.counterpartyName ?? 'Kartla tahsil')
    : (item.description ??
          '${DateText.dayMonth(item.settlementDate)} gün sonu');

/// "Hesaba geçenleri işaretle" panelini açar (ADR 0019 T5). Kaydedilince
/// `true`, vazgeçilince `null`.
///
/// [initialSettlementId] verilirse yalnız o tahsilat seçili gelir (tahsilat
/// ayrıntısındaki `Hesaba geçti`); verilmezse günü gelmiş tahsilatlar.
Future<bool?> showPosDepositForm(
  BuildContext context,
  PosController controller, {
  String? initialSettlementId,
}) => AppFormSheet.show<bool>(
  context: context,
  builder: (_) => _DepositForm(
    controller: controller,
    initialSettlementId: initialSettlementId,
  ),
);

/// Yatış ayrıntısını açar: Kasa'daki tahsilat panelinden ve İşlemler'deki
/// yatış satırından.
Future<void> showPosDepositDetail(
  BuildContext context, {
  required PosRepositoryContract repository,
  required String depositId,
  FinancialDataChanges? changes,
  PosDepositSummary? initial,
}) => AppAdaptiveSheet.show<void>(
  context: context,
  builder: (_) => PosDepositDetailSheet(
    repository: repository,
    depositId: depositId,
    changes: changes,
    initial: initial,
  ),
);

String _today() => AppDateField.format(DateTime.now());

class _DepositForm extends StatefulWidget {
  const _DepositForm({required this.controller, this.initialSettlementId});

  final PosController controller;
  final String? initialSettlementId;

  @override
  State<_DepositForm> createState() => _DepositFormState();
}

/// Yatış formu: yoldaki tahsilatların toplu seçimi, bankanın gerçekten
/// yatırdığı tutar ve gün.
///
/// Beklenen toplam ve kesinti **sunucunun önizlemesinden** gelir; istemci
/// parayı ikinci kez hesaplamaz. Bir yatış tek hesaba düşer: ilk seçimden
/// sonra başka hesabın tahsilatları seçilemez.
class _DepositFormState extends State<_DepositForm> {
  /// Aynı panelden ikinci gönderim ikinci yatış yazmasın: kimlik panel
  /// açıldığında bir kez üretilir.
  final requestId = newClientRequestId();
  final formKey = GlobalKey<FormState>();
  final amountController = TextEditingController();

  late final List<PosSettlementItem> candidates;
  late final Set<String> selected;
  late String depositDate = _today();

  /// Gider kategorileri yalnız kesinti çıkınca okunur.
  Future<PosOptions>? options;

  PosDepositPreview? preview;
  String? deductionCategoryId;

  /// Kullanıcı tutarı elle değiştirdiyse seçim değişince üstüne yazılmaz.
  bool amountEdited = false;
  bool selectionMissing = false;
  Timer? _previewTimer;
  int _previewRequest = 0;

  PosController get controller => widget.controller;

  /// Seçimin hesabı; seçim boşsa `null` ve her tahsilat seçilebilir.
  String? get lockedAccountId {
    for (final item in candidates) {
      if (selected.contains(item.id)) return item.accountId;
    }
    return null;
  }

  bool get hasSeveralAccounts =>
      candidates.map((item) => item.accountId).toSet().length > 1;

  @override
  void initState() {
    super.initState();
    candidates = [
      ...controller.inTransitItems,
    ]..sort((a, b) => a.expectedTransferDate.compareTo(b.expectedTransferDate));
    selected = _initialSelection();
    controller.errorMessage = null;
    controller.addListener(_changed);
    if (selected.isNotEmpty) _loadPreview();
  }

  /// Tek tahsilattan açıldıysa o; değilse günü gelmiş olanlar. Günü gelmiş
  /// tahsilatlar birden çok hesaba geçiyorsa en eskisinin hesabı seçilir.
  Set<String> _initialSelection() {
    final initial = widget.initialSettlementId;
    if (initial != null) {
      return {
        for (final item in candidates)
          if (item.id == initial) item.id,
      };
    }
    final today = _today();
    final due = [
      for (final item in candidates)
        if (item.expectedTransferDate.compareTo(today) <= 0) item,
    ];
    if (due.isEmpty) return {};
    return {
      for (final item in due)
        if (item.accountId == due.first.accountId) item.id,
    };
  }

  @override
  void dispose() {
    _previewTimer?.cancel();
    controller.removeListener(_changed);
    amountController.dispose();
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  void _toggle(PosSettlementItem item, bool value) {
    setState(() {
      value ? selected.add(item.id) : selected.remove(item.id);
      selectionMissing = false;
      // Seçim değişti: beklenen tutar da değişti, yazılmış tutar eskidi.
      amountEdited = false;
      if (selected.isEmpty) {
        preview = null;
        amountController.clear();
      }
    });
    if (selected.isNotEmpty) _loadPreview();
  }

  /// Tutar değişince önizleme kısa bir duraklamadan sonra istenir; her tuşta
  /// istek atılmaz.
  void _amountChanged(String _) {
    amountEdited = true;
    _previewTimer?.cancel();
    if (selected.isEmpty) return;
    _previewTimer = Timer(const Duration(milliseconds: 350), _loadPreview);
  }

  Future<void> _loadPreview() async {
    final request = ++_previewRequest;
    final typed = amountEdited ? MoneyInput.parse(amountController.text) : null;
    final result = await controller.previewDeposit(
      settlementIds: selected.toList(growable: false),
      depositedAmount: typed != null && typed > 0
          ? MoneyInput.wire(amountController.text)
          : null,
    );
    // Sonradan değişen seçimin cevabı eskisinin üstüne yazılmasın.
    if (!mounted || request != _previewRequest) return;
    setState(() {
      preview = result;
      if (result == null) return;
      if (!amountEdited) {
        amountController.text = MoneyText.editable(result.expectedAmount);
      }
      deductionCategoryId ??= result.deductionCategoryId;
      if (depositDate.compareTo(result.earliestDepositDate) < 0) {
        depositDate = result.earliestDepositDate;
      }
    });
  }

  @override
  Widget build(BuildContext context) => Form(
    key: formKey,
    child: AppFormSheet<bool>(
      title: 'Hesaba geçenleri işaretle',
      description:
          'Bankanın yatırdığı tutarı yazın. Satış tahsil edildiği gün gelir '
          'olarak yazıldı; bu adım gelir yazmaz.',
      submitLabel: 'Kaydet',
      onSubmit: candidates.isEmpty ? null : _submit,
      children: candidates.isEmpty
          ? const [Text('Yolda tahsilat yok.')]
          : _fields(context),
    ),
  );

  List<Widget> _fields(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final shown = preview;
    final locked = lockedAccountId;
    final error = controller.errorMessage ?? controller.depositPreviewError;
    return [
      if (error != null)
        AppInlineNotice(
          message: error,
          icon: Icons.error_outline,
          margin: const EdgeInsets.only(bottom: AppSpacing.small),
        ),
      Text('Yoldaki tahsilatlar', style: theme.textTheme.labelMedium),
      for (final item in candidates)
        _SelectableSettlementRow(
          item: item,
          selected: selected.contains(item.id),
          enabled:
              !controller.isSubmitting &&
              (locked == null || item.accountId == locked),
          onChanged: (value) => _toggle(item, value),
        ),
      if (hasSeveralAccounts)
        Text(
          'Bir yatış tek hesaba düşer; başka hesabın tahsilatlarını ayrıca '
          'işaretleyin.',
          style: theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted),
        ),
      if (selectionMissing)
        Padding(
          padding: const EdgeInsets.only(top: AppSpacing.xSmall),
          child: Text(
            'En az bir tahsilat seçin.',
            style: theme.textTheme.bodySmall?.copyWith(
              color: theme.colorScheme.error,
            ),
          ),
        ),
      const SizedBox(height: AppSpacing.medium),
      AppDetailBlock(
        rows: [
          AppDetailRow(
            label: 'Beklenen',
            trailing: shown == null
                ? null
                : AppMoneyText(
                    amount: shown.expectedAmount,
                    currency: shown.currency,
                    size: AppMoneySize.body,
                    style: const TextStyle(fontWeight: FontWeight.w600),
                  ),
            value: shown == null ? '—' : null,
          ),
          if (shown != null)
            AppDetailRow(label: 'Hesap', value: shown.accountName),
        ],
      ),
      const SizedBox(height: AppSpacing.medium),
      TextFormField(
        controller: amountController,
        enabled: selected.isNotEmpty,
        keyboardType: const TextInputType.numberWithOptions(decimal: true),
        decoration: InputDecoration(
          labelText: 'Yatan tutar',
          helperText: 'Bankanın hesabınıza gerçekten yatırdığı tutar.',
          helperMaxLines: 2,
          errorMaxLines: 3,
          // Fazla yatan gelir değildir, fazla yazılmış komisyondur.
          errorText: shown?.exceedsExpected ?? false
              ? 'Beklenenden fazla olamaz. Komisyon fazla yazıldıysa '
                    'tahsilatı iptal edip yeniden girin.'
              : null,
        ),
        onChanged: _amountChanged,
        validator: MoneyInput.positiveError,
      ),
      const SizedBox(height: AppSpacing.medium),
      AppDateField(
        label: 'Yattığı gün',
        value: depositDate,
        firstDate: AppDateField.parse(shown?.earliestDepositDate),
        lastDate: DateTime.now(),
        onChanged: (value) => setState(() => depositDate = value),
      ),
      if (shown != null && shown.hasDeduction) ..._deductionFields(shown),
    ];
  }

  /// Banka eksik yatırdı: fark kesinti gideridir ve bir kategoriye yazılır.
  /// Kategori POS'un komisyon kategorisinden dolu gelir.
  List<Widget> _deductionFields(PosDepositPreview shown) => [
    const SizedBox(height: AppSpacing.medium),
    AppDetailBlock(
      rows: [
        AppDetailRow(
          label: 'Kesinti',
          trailing: AppMoneyText(
            amount: shown.deductionAmount,
            currency: shown.currency,
            effect: AppMoneyEffect.expense,
            signed: true,
            size: AppMoneySize.body,
          ),
        ),
      ],
    ),
    const SizedBox(height: AppSpacing.small),
    FutureBuilder<PosOptions>(
      future: options ??= controller.loadOptions(),
      builder: (context, snapshot) {
        if (snapshot.hasError) {
          return const Text(
            'Kategoriler yüklenemedi. Paneli kapatıp yeniden deneyin.',
          );
        }
        if (!snapshot.hasData) return const LinearProgressIndicator();
        final categories = snapshot.data!.expenseCategories;
        // Dolu gelen kategori listede yoksa (pasife alınmış) seçim boş kalır
        // ve kullanıcı seçer.
        final value = categories.any((item) => item.id == deductionCategoryId)
            ? deductionCategoryId
            : null;
        return DropdownButtonFormField<String>(
          key: ValueKey('deduction-category-$value'),
          initialValue: value,
          isExpanded: true,
          decoration: const InputDecoration(
            labelText: 'Kesinti kategorisi',
            helperText: 'Eksik yatan tutar bu kategoriye gider yazılır.',
            helperMaxLines: 2,
          ),
          items: [
            for (final category in categories)
              DropdownMenuItem(value: category.id, child: Text(category.name)),
          ],
          onChanged: (selection) =>
              setState(() => deductionCategoryId = selection),
          validator: (selection) =>
              selection == null ? 'Kesinti için kategori seçin.' : null,
        );
      },
    ),
  ];

  Future<bool?> _submit() async {
    if (selected.isEmpty) {
      setState(() => selectionMissing = true);
      return null;
    }
    if (!formKey.currentState!.validate()) return null;
    // Bekleyen önizleme varsa güncel tutarla kapatılır: kesinti alanı ve
    // "fazla" denetimi gönderilen tutarı görmüş olmalı.
    _previewTimer?.cancel();
    await _loadPreview();
    final shown = preview;
    if (!mounted || shown == null || shown.exceedsExpected) return null;
    if (shown.hasDeduction && !formKey.currentState!.validate()) return null;

    final saved = await controller.createDeposit(
      clientRequestId: requestId,
      settlementIds: selected.toList(growable: false),
      depositedAmount: MoneyInput.wire(amountController.text),
      depositDate: depositDate,
      deductionCategoryId: shown.hasDeduction ? deductionCategoryId : null,
    );
    return saved ? true : null;
  }
}

/// Yatış formundaki seçilebilir tahsilat: onay kutusu, başlık, hesap ve
/// beklenen gün, sağda hesaba geçecek net tutar.
///
/// `CheckboxListTile` değil: o kendi yazı stilini taşır ve uygulamanın
/// tipografisinden ayrılır. Satırın tamamı dokunma hedefidir.
class _SelectableSettlementRow extends StatelessWidget {
  const _SelectableSettlementRow({
    required this.item,
    required this.selected,
    required this.enabled,
    required this.onChanged,
  });

  final PosSettlementItem item;
  final bool selected;
  final bool enabled;
  final ValueChanged<bool> onChanged;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final muted = surfaces.inkMuted;
    return MergeSemantics(
      child: InkWell(
        onTap: enabled ? () => onChanged(!selected) : null,
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 48),
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: AppSpacing.xSmall),
            child: Row(
              children: [
                Checkbox(
                  key: ValueKey('deposit-settlement-${item.id}'),
                  value: selected,
                  onChanged: enabled
                      ? (value) => onChanged(value ?? false)
                      : null,
                ),
                const SizedBox(width: AppSpacing.xSmall),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        posSettlementTitle(item),
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: theme.textTheme.bodyMedium?.copyWith(
                          fontWeight: FontWeight.w500,
                          color: enabled ? surfaces.ink : muted,
                        ),
                      ),
                      Text(
                        '${item.accountName} · '
                        '${item.isLate ? 'gecikti' : 'beklenen'} '
                        '${DateText.dayMonth(item.expectedTransferDate)}',
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: muted,
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(width: AppSpacing.small),
                AppMoneyText(
                  amount: item.netAmount,
                  currency: item.currency,
                  size: AppMoneySize.body,
                  style: enabled ? null : TextStyle(color: muted),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

/// Yatışın kapattığı bir tahsilat: başlık ve hesaba giren net tutar.
class _ClosedSettlementRow extends StatelessWidget {
  const _ClosedSettlementRow({required this.item});

  final PosSettlementItem item;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return MergeSemantics(
      child: ConstrainedBox(
        constraints: const BoxConstraints(minHeight: 44),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: AppSpacing.small),
          child: Row(
            children: [
              Expanded(
                child: Text(
                  posSettlementTitle(item),
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                  style: theme.textTheme.bodyMedium?.copyWith(
                    height: 1.3,
                    color: AppSurfaces.of(context).inkMuted,
                  ),
                ),
              ),
              const SizedBox(width: AppSpacing.medium),
              AppMoneyText(
                amount: item.netAmount,
                currency: item.currency,
                size: AppMoneySize.body,
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Yatışın, ayrıntı açılmadan önce zaten bilinen kısmı (İşlemler satırı
/// bunları taşır). Panel bununla **hemen** açılır; kapattığı tahsilatlar,
/// beklenen tutar ve kalan bakiye yerlerinde yüklenir.
class PosDepositSummary {
  const PosDepositSummary({
    required this.depositedAmount,
    required this.depositDate,
    required this.accountName,
    required this.currency,
    required this.isCancelled,
    this.deductionAmount,
    this.settlementCount = 0,
  });

  final String depositedAmount;
  final String depositDate;
  final String accountName;
  final String currency;
  final bool isCancelled;

  /// Kesinti; yoksa `null`.
  final String? deductionAmount;
  final int settlementCount;
}

/// POS yatışının ayrıntısı: yatan tutar, beklenen, kesinti, gün, hesap,
/// kalan bakiye ve kapattığı tahsilatlar; geri alınmamışsa `Yatışı geri al`.
///
/// Diğer işlem ayrıntıları gibi açılır: ekranı kaplayan bir "yükleniyor"
/// perdesi yoktur. [initial] verildiyse panel son boyutunda hemen çizilir ve
/// eksik satırlar yerlerinde dolar; verilmediyse (Kasa'daki tahsilattan
/// gelindiğinde) küçük bir bekleme kutusu gösterilir.
class PosDepositDetailSheet extends StatefulWidget {
  const PosDepositDetailSheet({
    required this.repository,
    required this.depositId,
    super.key,
    this.changes,
    this.initial,
  });

  final PosRepositoryContract repository;
  final String depositId;
  final FinancialDataChanges? changes;
  final PosDepositSummary? initial;

  @override
  State<PosDepositDetailSheet> createState() => _PosDepositDetailSheetState();
}

class _PosDepositDetailSheetState extends State<PosDepositDetailSheet> {
  /// Henüz gelmemiş değerin yeri; satır yerinde durur, panel zıplamaz.
  static const _pending = '…';

  late final PosDepositController controller = PosDepositController(
    widget.repository,
    widget.depositId,
    changes: widget.changes,
  );

  @override
  void initState() {
    super.initState();
    controller.addListener(_changed);
    controller.load();
  }

  @override
  void dispose() {
    controller.removeListener(_changed);
    controller.dispose();
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    final deposit = controller.deposit;
    final summary = deposit == null
        ? widget.initial
        : PosDepositSummary(
            depositedAmount: deposit.depositedAmount,
            depositDate: deposit.depositDate,
            accountName: deposit.accountName,
            currency: deposit.currency,
            isCancelled: deposit.isCancelled,
            deductionAmount: deposit.hasDeduction
                ? deposit.deductionAmount
                : null,
            settlementCount: deposit.settlements.length,
          );
    if (summary == null) {
      // Kısa ve sabit: `AppLoadingView` ortalandığı için paneli tam
      // yüksekliğe çıkarıyor, içerik gelince panel bir anda küçülüyordu.
      return SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(AppSpacing.medium),
          child: SizedBox(
            height: 300,
            child: controller.unauthorized
                ? const AppUnauthorizedView()
                : controller.errorMessage != null
                ? AppErrorView(
                    message: controller.errorMessage!,
                    onRetry: controller.load,
                  )
                : const AppLoadingView(message: 'Yatış yükleniyor'),
          ),
        ),
      );
    }
    return _content(context, summary, deposit);
  }

  Widget _content(
    BuildContext context,
    PosDepositSummary summary,
    PosDeposit? deposit,
  ) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final note = theme.textTheme.bodySmall;
    final hasDeduction = summary.deductionAmount != null;
    final loading = deposit == null && controller.errorMessage == null;
    return SafeArea(
      child: SingleChildScrollView(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          0,
          AppSpacing.medium,
          AppSpacing.large,
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          mainAxisSize: MainAxisSize.min,
          children: [
            Row(
              children: [
                // Diğer işlem ayrıntılarıyla aynı dil: yatış nötr bir
                // harekettir, kapsülü ve tutarı mavidir.
                AppIconCapsule(
                  icon: Icons.move_to_inbox_outlined,
                  tone: summary.isCancelled
                      ? AppStatusTone.cancelled
                      : AppStatusTone.neutral,
                ),
                const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Semantics(
                        header: true,
                        child: Text(
                          'POS yatışı',
                          style: theme.textTheme.titleSmall,
                        ),
                      ),
                      Text(summary.accountName, style: note),
                    ],
                  ),
                ),
                IconButton(
                  tooltip: 'Kapat',
                  onPressed: () => Navigator.of(context).maybePop(),
                  icon: const Icon(Icons.close),
                ),
              ],
            ),
            const SizedBox(height: AppSpacing.medium),
            Wrap(
              alignment: WrapAlignment.spaceBetween,
              crossAxisAlignment: WrapCrossAlignment.center,
              spacing: AppSpacing.small,
              runSpacing: AppSpacing.small,
              children: [
                AppMoneyText(
                  amount: summary.depositedAmount,
                  currency: summary.currency,
                  effect: AppMoneyEffect.neutral,
                  isCancelled: summary.isCancelled,
                  size: AppMoneySize.metric,
                  style: const TextStyle(
                    fontSize: 28,
                    fontWeight: FontWeight.w700,
                  ),
                ),
                summary.isCancelled
                    ? const AppStatusTag(
                        label: 'Geri alındı',
                        icon: Icons.undo,
                        tone: AppStatusTone.cancelled,
                      )
                    : AppStatusTag(
                        label:
                            'Hesaba geçti · '
                            '${DateText.dayMonth(summary.depositDate)}',
                        icon: Icons.check_circle_outline,
                        tone: AppStatusTone.income,
                      ),
              ],
            ),
            Text('Hesaba yatan tutar', style: note),
            if (controller.errorMessage != null)
              AppInlineNotice(
                message: controller.errorMessage!,
                icon: Icons.error_outline,
                actionLabel: deposit == null ? 'Tekrar dene' : null,
                onAction: deposit == null ? controller.load : null,
                margin: const EdgeInsets.only(top: AppSpacing.small),
              ),
            const SizedBox(height: AppSpacing.medium),
            AppDetailBlock(
              rows: [
                // Yatan tutarın nereden geldiği: satış − komisyon = beklenen.
                // Günler sonra yatan küsuratlı bir tutarda ne kadar komisyon
                // kesildiği başka türlü okunmuyor. Komisyon satış günü gider
                // yazıldı; burada yalnız gösterilir. Sıfır olsa da satır
                // durur: panel yüklenince boyut değiştirmesin.
                // Kartla tahsil (ADR 0019 T5) satış değildir: ayrı satırda
                // okunur. İki tutar da sunucudandır; istemci brütten çıkarmaz.
                if (!summary.isCancelled &&
                    (loading || deposit?.grossAmount != null)) ...[
                  if (deposit?.hasSale ?? true)
                    AppDetailRow(
                      icon: Icons.point_of_sale_outlined,
                      label: 'Satış',
                      trailing: deposit?.grossAmount == null
                          ? null
                          : AppMoneyText(
                              amount:
                                  deposit!.saleAmount ?? deposit.grossAmount!,
                              currency: deposit.currency,
                              size: AppMoneySize.body,
                            ),
                      value: deposit?.grossAmount == null ? _pending : null,
                    ),
                  if (deposit?.hasCollection ?? false)
                    AppDetailRow(
                      icon: Icons.handshake_outlined,
                      label: 'Tahsilat',
                      trailing: AppMoneyText(
                        amount: deposit!.collectionAmount!,
                        currency: deposit.currency,
                        size: AppMoneySize.body,
                      ),
                    ),
                  AppDetailRow(
                    icon: Icons.percent,
                    label: 'Komisyon',
                    trailing: deposit?.commissionAmount == null
                        ? null
                        : AppMoneyText(
                            amount: deposit!.commissionAmount!,
                            currency: deposit.currency,
                            effect: AppMoneyEffect.expense,
                            signed: true,
                            size: AppMoneySize.body,
                          ),
                    value: deposit?.commissionAmount == null ? _pending : null,
                  ),
                ],
                AppDetailRow(
                  icon: Icons.schedule,
                  label: 'Beklenen',
                  trailing: deposit == null
                      ? null
                      : AppMoneyText(
                          amount: deposit.expectedAmount,
                          currency: deposit.currency,
                          size: AppMoneySize.body,
                        ),
                  value: deposit == null ? _pending : null,
                ),
                if (hasDeduction)
                  AppDetailRow(
                    icon: Icons.remove_circle_outline,
                    label: 'Kesinti',
                    trailing: AppMoneyText(
                      amount: summary.deductionAmount!,
                      currency: summary.currency,
                      effect: AppMoneyEffect.expense,
                      signed: true,
                      size: AppMoneySize.body,
                    ),
                  ),
                if (hasDeduction && deposit?.deductionCategoryName != null)
                  AppDetailRow(
                    icon: Icons.local_offer_outlined,
                    label: 'Kategori',
                    value: deposit!.deductionCategoryName,
                  ),
                AppDetailRow(
                  icon: Icons.event_outlined,
                  // Diğer ayrıntılarla aynı ve kısa: ikonlu satırda uzun
                  // etiket en büyük yazıda taşıyor.
                  label: 'Tarih',
                  value: DateText.dayMonthYear(summary.depositDate),
                ),
                AppDetailRow(
                  icon: Icons.account_balance_outlined,
                  label: 'Hesap',
                  value: summary.accountName,
                ),
                // Yatıştan hemen sonra hesabın bakiyesi; geri alınmış
                // yatışın "sonrası" yoktur.
                if (!summary.isCancelled &&
                    (loading || deposit?.balanceAfter != null))
                  AppDetailRow(
                    icon: Icons.account_balance_wallet_outlined,
                    label: 'Bakiye',
                    trailing: deposit?.balanceAfter == null
                        ? null
                        : AppMoneyText(
                            amount: deposit!.balanceAfter!,
                            currency: deposit.currency,
                            // Yatış parayı hesaba sokar: bakiye yeşil.
                            effect: AppMoneyEffect.income,
                            size: AppMoneySize.body,
                            style: const TextStyle(fontWeight: FontWeight.w600),
                          ),
                    value: deposit?.balanceAfter == null ? _pending : null,
                  ),
              ],
            ),
            if (summary.settlementCount > 0) ...[
              const SizedBox(height: AppSpacing.medium),
              Text('Kapattığı tahsilatlar', style: theme.textTheme.labelMedium),
              const SizedBox(height: AppSpacing.xSmall),
              // Başlık kullanıcının yazdığı metindir ve uzun olabilir:
              // etiket–değer satırı yerine saran bir satır.
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppSpacing.medium,
                ),
                decoration: BoxDecoration(
                  color: surfaces.cardMuted,
                  borderRadius: BorderRadius.circular(AppRadius.field),
                ),
                child: AppDividedColumn(
                  children: [
                    if (deposit == null)
                      for (var i = 0; i < summary.settlementCount; i++)
                        const _PendingSettlementRow()
                    else
                      for (final item in deposit.settlements)
                        _ClosedSettlementRow(item: item),
                  ],
                ),
              ),
            ],
            const SizedBox(height: AppSpacing.medium),
            Text(
              summary.isCancelled
                  ? 'Kayıt duruyor; tahsilatlar yeniden yolda.'
                  : hasDeduction
                  ? 'Kesinti ayrı bir gider olarak yazıldı; yatış gelir '
                        'yazmaz.'
                  : 'Yatış gelir yazmaz; satış ve komisyon tahsil edildiği '
                        'gün yazıldı.',
              style: note?.copyWith(color: surfaces.inkMuted),
            ),
            if (!summary.isCancelled) ...[
              const SizedBox(height: AppSpacing.medium),
              OutlinedButton.icon(
                style: OutlinedButton.styleFrom(
                  minimumSize: const Size.fromHeight(48),
                ),
                // Yüklenmeden geri alınamaz: onay metni yatışın kesintisini
                // söyler ve o bilgi yüklenen kayıttadır.
                onPressed: deposit == null || controller.isSubmitting
                    ? null
                    : () => _confirmRevert(context, deposit),
                icon: const Icon(Icons.undo),
                label: const Text('Yatışı geri al'),
              ),
            ],
          ],
        ),
      ),
    );
  }

  Future<void> _confirmRevert(BuildContext context, PosDeposit deposit) async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.undo,
      title: 'Yatış geri alınsın mı?',
      message: deposit.hasDeduction
          ? 'Tutar hesaptan geri çekilir, tahsilatlar yeniden yolda görünür '
                've kesinti gideri iptal edilir.'
          : 'Tutar hesaptan geri çekilir ve tahsilatlar yeniden yolda '
                'görünür. Satış ve komisyon olduğu gibi kalır.',
      highlight: MoneyText.format(deposit.depositedAmount, deposit.currency),
      confirmLabel: 'Geri al',
    );
    if (!confirmed || !context.mounted) return;
    final reverted = await controller.revert();
    if (reverted && context.mounted) Navigator.of(context).maybePop();
  }
}

/// Henüz yüklenmemiş bir tahsilat satırının yeri: aynı yükseklikte, boş.
class _PendingSettlementRow extends StatelessWidget {
  const _PendingSettlementRow();

  @override
  Widget build(BuildContext context) => ConstrainedBox(
    constraints: const BoxConstraints(minHeight: 44),
    child: Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.small),
      child: Align(
        alignment: AlignmentDirectional.centerStart,
        child: Text(
          '…',
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
            height: 1.3,
            color: AppSurfaces.of(context).inkMuted,
          ),
        ),
      ),
    ),
  );
}
