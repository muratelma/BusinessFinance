import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_math.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/data_choice.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_card_head.dart';
import '../../../core/widgets/app_date_leaf.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_segment_rail.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_status_tag.dart';
import '../../../core/widgets/app_submit_button.dart';
import '../../../core/widgets/app_text_action.dart';
import '../data/cash_repository.dart';
import 'cash_controller.dart';
import 'cash_withdrawal_sheet.dart';

/// Bugünün sayım kartı: Kasa ekranının ilk kartı.
///
/// Sayımdan önce **beklenen tutarın nereden geldiğini** gösterir (son sayım,
/// bugünkü nakit giriş ve çıkış); sayımdan sonra elde sayılanı, uygulamaya
/// göre tutarı ve farkı. Bütün tutarlar sunucudan gelir; kart hesap yapmaz.
class CashTodayCard extends StatelessWidget {
  const CashTodayCard({
    required this.controller,
    required this.today,
    required this.onCount,
    required this.onSaveDifference,
    super.key,
  });

  final CashCountController controller;
  final CashCountToday today;
  final VoidCallback onCount;
  final VoidCallback onSaveDifference;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final count = controller.todayCount;
    final help = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);

    return AppCard(
      padding: EdgeInsets.zero,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppCardHead(
            title: DateText.dayMonthWeekday(controller.todayIso),
            status: _status(count),
          ),
          Padding(
            padding: const EdgeInsets.all(AppSpacing.medium),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                MergeSemantics(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        count == null
                            ? 'Uygulamaya göre kasada'
                            : 'Elde sayılan',
                        style: help,
                      ),
                      const SizedBox(height: AppSpacing.xxSmall),
                      AppMoneyText(
                        amount: count?.countedAmount ?? today.expectedBalance,
                        currency: today.currency,
                        size: AppMoneySize.hero,
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: AppSpacing.medium),
                Divider(height: 1, thickness: 1, color: surfaces.border),
                const SizedBox(height: AppSpacing.xSmall),
                ..._breakdown(count),
                const SizedBox(height: AppSpacing.medium),
                _actions(context, count),
                if (count != null) ...[
                  const SizedBox(height: AppSpacing.small),
                  Text(_rule(count), style: help),
                ],
              ],
            ),
          ),
        ],
      ),
    );
  }

  /// Sayımdan sonra kasaya hareket girdi mi (28 Eylül denetimi U10)?
  /// Sunucunun gönderdiği değişimin yalnız yönüne bakılır.
  bool get _changedSinceCount => _sign(today.changeSinceCount) != 0;

  Widget _status(CashCountItem? count) {
    if (count == null) {
      return const AppStatusTag(
        label: 'Sayılmadı',
        icon: Icons.schedule,
        tone: AppStatusTone.planned,
      );
    }
    if (_changedSinceCount) {
      return const AppStatusTag(
        label: 'Sonradan kayıt girildi',
        icon: Icons.update,
        tone: AppStatusTone.neutral,
      );
    }
    if (count.isAdjusted) {
      return const AppStatusTag(
        label: 'Fark kaydedildi',
        icon: Icons.check_circle_outline,
        tone: AppStatusTone.neutral,
      );
    }
    return switch (_sign(count.difference)) {
      0 => const AppStatusTag(
        label: 'Tuttu',
        icon: Icons.check_circle_outline,
        tone: AppStatusTone.income,
      ),
      < 0 => const AppStatusTag(
        label: 'Eksik',
        icon: Icons.error_outline,
        tone: AppStatusTone.expense,
      ),
      _ => const AppStatusTag(
        label: 'Fazla',
        icon: Icons.error_outline,
        tone: AppStatusTone.neutral,
      ),
    };
  }

  List<Widget> _breakdown(CashCountItem? count) {
    final currency = today.currency;
    if (count != null) {
      final difference = count.difference;
      final sign = _sign(difference);
      return [
        _KeyValue(
          label: 'Uygulamaya göre',
          amount: count.expectedBalance ?? today.expectedBalance,
          currency: currency,
          // Sayımdan sonra girilen kayıt ayrı bir satır değil, bu sayının
          // neden değiştiğinin açıklamasıdır (29 Eylül emülatör denemesi).
          detail: _changedSinceCount ? _changeDetail(currency) : null,
        ),
        if (difference != null && sign != 0)
          _KeyValue(
            label: 'Fark',
            amount: _abs(difference),
            currency: currency,
            effect: sign < 0 ? AppMoneyEffect.expense : AppMoneyEffect.income,
          ),
        ?_carriedDifference(currency),
      ];
    }
    final previous = today.previousCount;
    final inflow = today.todayInflow;
    final outflow = today.todayOutflow;
    return [
      if (previous != null)
        _KeyValue(
          label: _previousLabel(previous.countDate),
          amount: previous.countedAmount,
          currency: currency,
        ),
      if (inflow != null)
        _KeyValue(
          label: 'Bugün nakit giriş',
          amount: inflow,
          currency: currency,
          effect: AppMoneyEffect.income,
        ),
      if (outflow != null)
        _KeyValue(
          label: 'Bugün nakit çıkış',
          amount: outflow,
          currency: currency,
          effect: AppMoneyEffect.expense,
        ),
      ?_carriedDifference(currency),
    ];
  }

  /// Önceki sayımın kaydedilmemiş farkı: yalnız bilgi satırıdır, bugünkü
  /// farktan düşülmez (Aşama 06.3 K6).
  Widget? _carriedDifference(String currency) {
    final carried = today.previousUnrecordedDifference;
    final previous = today.previousCount;
    if (carried == null || previous == null) return null;
    return _KeyValue(
      label: 'Kaydedilmemiş fark',
      detail: '${DateText.dayMonth(previous.countDate)} sayımından',
      amount: _abs(carried),
      currency: currency,
      effect: _sign(carried) < 0
          ? AppMoneyEffect.expense
          : AppMoneyEffect.income,
    );
  }

  /// Dünkü sayım ise `Dünkü sayım`; daha eskiyse günüyle `Son sayım`.
  String _previousLabel(String countDate) {
    final today = DateTime.tryParse(controller.todayIso);
    final date = DateTime.tryParse(countDate);
    if (today != null && date != null && today.difference(date).inDays == 1) {
      return 'Dünkü sayım';
    }
    return 'Son sayım · ${DateText.dayMonth(countDate)}';
  }

  Widget _actions(BuildContext context, CashCountItem? count) {
    final busy = controller.isSubmitting;
    if (count == null) {
      return FilledButton.icon(
        onPressed: busy ? null : onCount,
        icon: const Icon(Icons.calculate_outlined),
        label: const Text('Sayımı gir'),
      );
    }
    final recount = OutlinedButton(
      onPressed: busy ? null : onCount,
      child: const Text('Yeniden say'),
    );
    if (!controller.hasOpenDifference) return recount;
    // Aynı fark bir kez daha sayıldıysa kayıt öne çıkmaz: kullanıcı onu
    // geçen sayımda da kaydetmemeyi seçmişti.
    final save = today.differenceSameAsPrevious
        ? OutlinedButton.icon(
            onPressed: busy ? null : onSaveDifference,
            icon: const Icon(Icons.playlist_add_check),
            label: const Text('Farkı kaydet'),
          )
        : FilledButton.icon(
            onPressed: busy ? null : onSaveDifference,
            icon: const Icon(Icons.playlist_add_check),
            label: const Text('Farkı kaydet'),
          );
    // Büyük yazıda iki düğme yan yana sığmaz; alt alta dizilir, birincil üstte.
    if (context.usesLargeText) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          save,
          const SizedBox(height: AppSpacing.small),
          recount,
        ],
      );
    }
    return Row(
      children: [
        Expanded(child: recount),
        const SizedBox(width: AppSpacing.small),
        Expanded(child: save),
      ],
    );
  }

  /// `−₺300,00 sayımdan sonra girildi`: işaret sunucunun gönderdiği
  /// değişimden okunur, istemci çıkarma yapmaz.
  String _changeDetail(String currency) {
    final change = today.changeSinceCount!;
    final sign = _sign(change) < 0 ? '−' : '+';
    return '$sign${MoneyText.format(_abs(change), currency)} sayımdan sonra '
        'girildi';
  }

  String _rule(CashCountItem count) {
    final record = _sign(count.difference) > 0 ? 'gelir' : 'gider';
    // Uygulama kaydın sayımdan sonra **girildiğini** bilir, olayın ne zaman
    // **olduğunu** bilmez: kayıtlarda saat yok. Metin iki ihtimali ayırır
    // (28 Eylül denetimi U10, 29 Eylül emülatör denemesi).
    // Tek kısa cümle: fark ancak kayıt sayımdan önceki bir olaysa gerçektir.
    if (_changedSinceCount) {
      if (count.isAdjusted) return 'Emin olmak için yeniden sayın.';
      if (_sign(count.difference) == 0) return 'Kasa yine uygulamayla aynı.';
      return 'Kayıt sayımdan önce olduysa farkı kaydedin.';
    }
    if (count.isAdjusted) {
      return 'Tek bir $record kaydı oluştu; kasa sayılan tutara oturdu.';
    }
    if (_sign(count.difference) == 0) return 'Kasa uygulamayla aynı.';
    if (today.differenceSameAsPrevious) return 'Fark son sayımdakiyle aynı.';
    return 'Fark kendiliğinden yazılmaz. Kaydederseniz tek bir $record kaydı '
        'oluşur.';
  }
}

/// Kartın açıklama satırı: solda gri etiket, sağda tutar; 36 dp.
class _KeyValue extends StatelessWidget {
  const _KeyValue({
    required this.label,
    required this.amount,
    required this.currency,
    this.effect,
    this.detail,
  });

  final String label;
  final String amount;
  final String currency;
  final AppMoneyEffect? effect;

  /// Etiketin altında küçük gri açıklama; sayının neden değiştiğini söyler.
  final String? detail;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return MergeSemantics(
      child: ConstrainedBox(
        constraints: const BoxConstraints(minHeight: 36),
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    label,
                    style: theme.textTheme.bodyMedium?.copyWith(
                      color: AppSurfaces.of(context).inkMuted,
                    ),
                  ),
                  if (detail != null)
                    Text(
                      detail!,
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppSurfaces.of(context).inkMuted,
                      ),
                    ),
                ],
              ),
            ),
            const SizedBox(width: AppSpacing.small),
            AppMoneyText(
              amount: amount,
              currency: currency,
              effect: effect,
              signed: effect != null,
              size: AppMoneySize.body,
            ),
          ],
        ),
      ),
    );
  }
}

/// `Son sayımlar` bölümü: bugünkü dahil geçerli sayımlar, sayılan ve beklenen
/// yan yana; fark sağda. Tamamı `Tümü` ile açılır.
class CashPastCountsSection extends StatelessWidget {
  const CashPastCountsSection({
    required this.controller,
    required this.onShowAll,
    super.key,
  });

  final CashCountController controller;
  final VoidCallback onShowAll;

  static const shownCount = 4;

  @override
  Widget build(BuildContext context) {
    final items = controller.recentCounts;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        AppSectionHeader(
          title: 'Son sayımlar',
          padding: EdgeInsets.zero,
          trailing: items.isEmpty
              ? null
              : AppTextAction(
                  label: 'Tümü',
                  trailingIcon: Icons.chevron_right,
                  onPressed: onShowAll,
                ),
        ),
        AppCard(
          padding: EdgeInsets.zero,
          child: items.isEmpty
              ? Padding(
                  padding: const EdgeInsets.all(AppSpacing.medium),
                  child: Text(
                    'Geçmiş sayım yok. Yaptığınız her sayım tarihiyle burada '
                    'kalır.',
                    style: Theme.of(context).textTheme.bodySmall,
                  ),
                )
              : AppDividedColumn(
                  inset: CashCountHistoryRow.inset,
                  children: [
                    for (final item in items.take(shownCount))
                      CashCountHistoryRow(item: item),
                  ],
                ),
        ),
      ],
    );
  }
}

/// Geçmiş sayım satırı. Kapsül kullanılmaz: dar ekranda taşıyordu (tasarım
/// notu); fark kısa metinle söylenir.
class CashCountHistoryRow extends StatelessWidget {
  const CashCountHistoryRow({required this.item, super.key});

  final CashCountItem item;

  /// Ayraç yaprağın sağından başlar: 16 + 44 + 16.
  static const double inset = AppSpacing.medium * 2 + 44;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final date = DateTime.tryParse(item.countDate);
    final expected = item.expectedBalance;
    final help = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);
    final middle = Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        AppMoneyText(
          amount: item.countedAmount,
          currency: item.currency,
          size: AppMoneySize.row,
        ),
        const SizedBox(height: AppSpacing.xxSmall),
        Text(
          expected == null
              ? (item.note ?? 'Sayım')
              : 'Beklenen ${MoneyText.format(expected, item.currency)}',
          maxLines: 1,
          overflow: TextOverflow.ellipsis,
          style: help,
        ),
      ],
    );
    final difference = _difference(context);
    final stacked = context.usesLargeText;

    return MergeSemantics(
      child: ConstrainedBox(
        constraints: const BoxConstraints(minHeight: 64),
        child: Padding(
          padding: const EdgeInsets.symmetric(
            horizontal: AppSpacing.medium,
            vertical: AppSpacing.small + AppSpacing.xSmall,
          ),
          child: Row(
            children: [
              if (date != null)
                Semantics(
                  label: DateText.dayMonth(item.countDate),
                  child: AppDateLeaf.fromDate(date),
                ),
              const SizedBox(width: AppSpacing.medium),
              Expanded(
                child: stacked && difference != null
                    ? Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          middle,
                          const SizedBox(height: AppSpacing.xSmall),
                          difference,
                        ],
                      )
                    : middle,
              ),
              if (!stacked && difference != null) ...[
                const SizedBox(width: AppSpacing.small),
                difference,
              ],
            ],
          ),
        ),
      ),
    );
  }

  Widget? _difference(BuildContext context) {
    final value = item.difference;
    if (value == null) return null;
    final colors = AppFinanceColors.of(context);
    final surfaces = AppSurfaces.of(context);
    final sign = _sign(value);
    if (sign == 0) {
      return Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(Icons.check_circle, size: 16, color: colors.income),
          const SizedBox(width: AppSpacing.xSmall),
          Text(
            'Tuttu',
            style: Theme.of(context).textTheme.bodyMedium?.copyWith(
              fontWeight: FontWeight.w600,
              height: 1.3,
              color: colors.income,
            ),
          ),
        ],
      );
    }
    return Column(
      crossAxisAlignment: CrossAxisAlignment.end,
      mainAxisSize: MainAxisSize.min,
      children: [
        AppMoneyText(
          amount: _abs(value),
          currency: item.currency,
          effect: sign < 0 ? AppMoneyEffect.expense : AppMoneyEffect.income,
          signed: true,
          size: AppMoneySize.body,
          style: const TextStyle(fontWeight: FontWeight.w600),
        ),
        const SizedBox(height: AppSpacing.xxSmall),
        Text(
          '${sign < 0 ? 'Eksik' : 'Fazla'}'
          '${item.isAdjusted ? ' · kaydedildi' : ''}',
          style: Theme.of(context).textTheme.labelSmall?.copyWith(
            fontWeight: FontWeight.w400,
            letterSpacing: 0,
            color: surfaces.inkMuted,
          ),
        ),
      ],
    );
  }
}

/// Bütün sayımlar: başlıktaki geçmiş ikonu ve `Tümü` bağlantısı açar.
Future<void> showAllCashCounts(
  BuildContext context,
  CashCountController controller,
) => AppAdaptiveSheet.show<void>(
  context: context,
  builder: (context) {
    final theme = Theme.of(context);
    final items = [
      for (final item in controller.history)
        if (!item.isCancelled) item,
    ];
    return SafeArea(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.large,
              0,
              AppSpacing.large,
              AppSpacing.small,
            ),
            child: Semantics(
              header: true,
              child: Text('Bütün sayımlar', style: theme.textTheme.titleLarge),
            ),
          ),
          Flexible(
            child: items.isEmpty
                ? Padding(
                    padding: const EdgeInsets.all(AppSpacing.large),
                    child: Text(
                      'Geçmiş sayım yok.',
                      style: theme.textTheme.bodySmall,
                    ),
                  )
                : SingleChildScrollView(
                    padding: const EdgeInsets.only(bottom: AppSpacing.medium),
                    child: AppDividedColumn(
                      inset: CashCountHistoryRow.inset,
                      children: [
                        for (final item in items)
                          CashCountHistoryRow(item: item),
                      ],
                    ),
                  ),
          ),
        ],
      ),
    );
  },
);

/// Sayım panelini açar; kaydedilirse `true`.
Future<bool?> showCashCountSheet(
  BuildContext context,
  CashCountController controller,
  ScopeController? scopeController,
) => AppAdaptiveSheet.show<bool>(
  context: context,
  builder: (_) =>
      CashCountSheet(controller: controller, scopeController: scopeController),
);

/// Farkı kaydetme formunu açar. Eksik farkta kullanıcı `Kendime aldım`
/// derse fark kaydı yazılmaz; aynı tutarla o panel açılır (Aşama 06.3 K7).
Future<bool?> showCashDifferenceForm(
  BuildContext context,
  CashCountController controller, {
  VoidCallback? onOpenPersonalAccount,
}) async {
  final amount = _inputAmount(controller.todayCount?.difference);
  final result = await AppFormSheet.show<CashDifferenceResult>(
    context: context,
    builder: (_) => _DifferenceForm(controller: controller),
  );
  if (result != CashDifferenceResult.withdrawal) {
    return result == null ? null : true;
  }
  if (!context.mounted) return null;
  return showCashWithdrawal(
    context,
    controller,
    initialAmount: amount,
    onOpenPersonalAccount: onOpenPersonalAccount,
  );
}

/// Fark formunun sonucu: kayıt yazıldı ya da kullanıcı parayı kendine
/// aldığını söyledi.
enum CashDifferenceResult { saved, withdrawal }

/// Eksik farkın sebebi. Sunucu sebep ya da kategori seçmez.
enum CashShortageReason {
  expense('Gider'),
  withdrawal('Kendime aldım'),
  unknown('Bilmiyorum');

  const CashShortageReason(this.label);

  final String label;
}

/// Sebebi bilinmeyen eksiğin yazıldığı standart kategorinin adı; kategoriyi
/// sunucu bulur ya da açar.
const cashDifferenceCategoryName = 'Kasa farkı';

/// Sunucunun işaretli farkını tutar alanına yazılacak hâle çevirir
/// (`-100.0000` → `100,00`); para aritmetiği değil, metin kırpma.
String? _inputAmount(String? difference) {
  if (difference == null) return null;
  final unsigned = difference.replaceFirst('-', '').trim();
  final parts = unsigned.split('.');
  if (parts.length != 2 || parts[1].length != 4) return unsigned;
  final decimals = parts[1].endsWith('00')
      ? parts[1].substring(0, 2)
      : parts[1];
  return '${parts[0]},$decimals';
}

enum CashCountMode { total, notes }

/// `Sayımı gir` paneli: toplamı yaz ya da banknotla say.
///
/// Canlı sonuç şeridi yalnız **önizlemedir** ve tam aritmetikle (`MoneyMath`,
/// 10⁴ ölçekli tam sayı) hesaplanır; kaydedilen fark yine sunucudan gelir.
/// Sayım bir gözlemdir: kaydetmek hiçbir bakiyeyi değiştirmez.
class CashCountSheet extends StatefulWidget {
  const CashCountSheet({
    required this.controller,
    super.key,
    this.scopeController,
    this.initialMode = CashCountMode.total,
    this.initialNotes,
    this.initialCoins,
  });

  final CashCountController controller;
  final ScopeController? scopeController;
  final CashCountMode initialMode;

  /// Banknot adetleri ([banknotes] sırasıyla) ve madeni para metni: panel
  /// yarım kalmış bir sayımla açılabilir.
  final List<int>? initialNotes;
  final String? initialCoins;

  static const banknotes = [200, 100, 50, 20, 10, 5];

  @override
  State<CashCountSheet> createState() => _CashCountSheetState();
}

class _CashCountSheetState extends State<CashCountSheet> {
  late CashCountMode mode = widget.initialMode;
  final totalController = TextEditingController();
  late final coinsController = TextEditingController(text: widget.initialCoins);
  late final List<int> noteCounts = [
    for (var i = 0; i < CashCountSheet.banknotes.length; i++)
      widget.initialNotes?.elementAtOrNull(i) ?? 0,
  ];
  TransactionScope? explicitScope;
  bool scopeMissing = false;
  bool amountMissing = false;

  @override
  void initState() {
    super.initState();
    totalController.addListener(_changed);
    coinsController.addListener(_changed);
  }

  @override
  void dispose() {
    totalController.dispose();
    coinsController.dispose();
    super.dispose();
  }

  void _changed() => setState(() => amountMissing = false);

  TransactionScope? get accountScope =>
      widget.controller.selectedAccount?.defaultScope;

  /// Zincir: kullanıcının seçimi → kasanın etiketi. Sayımın kategorisi yok.
  TransactionScope? get resolvedScope => explicitScope ?? accountScope;

  /// Kapsam yalnız zincir çözülemediğinde sorulur; yoksa sunucu kapsam
  /// uydurmaz ve sayımı reddederdi.
  bool get showScope => accountScope == null;

  /// Sayılan tutar (10⁴ ölçekli); toplam modunda alan boşsa `null`.
  BigInt? get counted {
    if (mode == CashCountMode.total) {
      final wire = MoneyMath.fromInput(totalController.text);
      return wire == null ? null : MoneyMath.parse(wire);
    }
    var sum = BigInt.zero;
    for (var i = 0; i < noteCounts.length; i++) {
      sum +=
          MoneyMath.lira(CashCountSheet.banknotes[i]) *
          BigInt.from(noteCounts[i]);
    }
    final coins = MoneyMath.fromInput(coinsController.text);
    if (coins != null) sum += MoneyMath.parse(coins) ?? BigInt.zero;
    return sum;
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final controller = widget.controller;
    final today = controller.today;
    final currency = today?.currency ?? 'TRY';
    final value = counted;
    final help = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);
    final keyboard = MediaQuery.viewInsetsOf(context).bottom;
    final accountName =
        controller.selectedAccount?.name ?? today?.accountName ?? 'Kasa';

    return SafeArea(
      child: SingleChildScrollView(
        padding: EdgeInsets.fromLTRB(
          AppSpacing.medium,
          0,
          AppSpacing.medium,
          AppSpacing.large + keyboard,
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          mainAxisSize: MainAxisSize.min,
          children: [
            Semantics(
              header: true,
              child: Text('Sayımı gir', style: theme.textTheme.headlineSmall),
            ),
            const SizedBox(height: AppSpacing.xSmall),
            Text(
              '$accountName · ${DateText.dayMonth(controller.todayIso)}',
              style: theme.textTheme.bodyLarge,
            ),
            const SizedBox(height: AppSpacing.medium),
            SegmentedButton<CashCountMode>(
              segments: const [
                ButtonSegment(
                  value: CashCountMode.total,
                  label: Text('Toplamı yaz'),
                ),
                ButtonSegment(
                  value: CashCountMode.notes,
                  label: Text('Banknotla say'),
                ),
              ],
              selected: {mode},
              onSelectionChanged: (selection) => setState(() {
                mode = selection.first;
                amountMissing = false;
              }),
            ),
            const SizedBox(height: AppSpacing.large),
            Text(
              'Elde sayılan',
              textAlign: TextAlign.center,
              style: theme.textTheme.labelMedium?.copyWith(letterSpacing: 0),
            ),
            const SizedBox(height: AppSpacing.xSmall),
            Center(
              child: mode == CashCountMode.total
                  ? _TotalField(controller: totalController)
                  : AppMoneyText(
                      amount: MoneyMath.wire(value ?? BigInt.zero),
                      currency: currency,
                      size: AppMoneySize.hero,
                    ),
            ),
            if (amountMissing) ...[
              const SizedBox(height: AppSpacing.small),
              Text(
                'Sayılan tutarı yazın; kasa boşsa 0 yazın.',
                textAlign: TextAlign.center,
                style: theme.textTheme.bodySmall?.copyWith(
                  color: theme.colorScheme.error,
                ),
              ),
            ],
            if (value != null && today != null) ...[
              const SizedBox(height: AppSpacing.medium),
              _ResultStrip(
                counted: value,
                expected: today.expectedBalance,
                currency: currency,
              ),
            ],
            if (mode == CashCountMode.notes) ...[
              const SizedBox(height: AppSpacing.medium),
              _BanknoteCard(
                counts: noteCounts,
                currency: currency,
                coinsController: coinsController,
                onChanged: (index, count) =>
                    setState(() => noteCounts[index] = count),
              ),
            ],
            if (showScope) ...[
              const SizedBox(height: AppSpacing.medium),
              AppScopeField(
                value: resolvedScope,
                onChanged: (value) => setState(() {
                  explicitScope = value;
                  scopeMissing = false;
                }),
                helperText: 'Kasa kapsam taşımıyor; bu sayım için seçin.',
                errorText: scopeMissing ? 'Bu sayım için kapsam seçin.' : null,
              ),
            ],
            const SizedBox(height: AppSpacing.medium),
            Text(
              '${mode == CashCountMode.notes ? 'Her banknotun adedini girin; toplam kendiliğinden hesaplanır. ' : ''}'
              'Sayım bir gözlemdir; kaydetmek bakiyeyi değiştirmez.',
              style: help,
            ),
            ListenableBuilder(
              listenable: controller,
              builder: (context, _) {
                final error = controller.errorMessage;
                if (error == null || controller.isSubmitting) {
                  return const SizedBox.shrink();
                }
                return Padding(
                  padding: const EdgeInsets.only(top: AppSpacing.small),
                  child: Text(
                    error,
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: theme.colorScheme.error,
                    ),
                  ),
                );
              },
            ),
            const SizedBox(height: AppSpacing.medium),
            AppSubmitButton(
              label: 'Sayımı kaydet',
              icon: Icons.check,
              onSubmit: _submit,
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _submit() async {
    final value = counted;
    if (value == null) {
      setState(() => amountMissing = true);
      return;
    }
    // Zincir çözülemediyse istek sunucuya gitmeden burada duruyor: sunucu da
    // reddederdi (`cash_counts.scope_unresolved`) ama kullanıcı hatayı
    // düzeltebileceği yerde görmeli.
    if (resolvedScope == null) {
      setState(() => scopeMissing = true);
      return;
    }
    final saved = await widget.controller.recordCount(
      countedAmount: MoneyMath.wire(value),
      countDate: widget.controller.todayIso,
      scope: resolvedScope,
    );
    if (saved && mounted) Navigator.of(context).pop(true);
  }
}

/// Toplam modunun giriş alanı: 36/700, altında 2 dp marka çizgisi, Türkçe
/// binlik ayırıcı (`23.100`).
class _TotalField extends StatelessWidget {
  const _TotalField({required this.controller});

  final TextEditingController controller;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final style = AppTypography.heroMoney(theme.textTheme.displaySmall!);
    return Container(
      constraints: const BoxConstraints(minWidth: 220),
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.medium,
        AppSpacing.xSmall,
        AppSpacing.medium,
        AppSpacing.small,
      ),
      decoration: BoxDecoration(
        border: Border(
          bottom: BorderSide(color: theme.colorScheme.primary, width: 2),
        ),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        mainAxisAlignment: MainAxisAlignment.center,
        crossAxisAlignment: CrossAxisAlignment.baseline,
        textBaseline: TextBaseline.alphabetic,
        children: [
          ExcludeSemantics(child: Text('₺', style: style)),
          const SizedBox(width: AppSpacing.xSmall),
          Flexible(
            child: IntrinsicWidth(
              child: ConstrainedBox(
                constraints: const BoxConstraints(minWidth: 48),
                // Görünür etiket alanın üstündeki `Elde sayılan`; ekran
                // okuyucu alanı bu adla duyar.
                child: Semantics(
                  label: 'Elde sayılan tutar',
                  child: TextField(
                    controller: controller,
                    autofocus: true,
                    keyboardType: const TextInputType.numberWithOptions(
                      decimal: true,
                    ),
                    inputFormatters: const [TurkishAmountInputFormatter()],
                    style: style,
                    decoration: const InputDecoration(
                      hintText: '0',
                      isCollapsed: true,
                      border: InputBorder.none,
                      enabledBorder: InputBorder.none,
                      focusedBorder: InputBorder.none,
                      filled: false,
                      contentPadding: EdgeInsets.zero,
                    ),
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

/// Canlı sonuç şeridi: rol kapsül zemini — tuttu yeşil, eksik kırmızı, fazla
/// mavi.
class _ResultStrip extends StatelessWidget {
  const _ResultStrip({
    required this.counted,
    required this.expected,
    required this.currency,
  });

  final BigInt counted;
  final String expected;
  final String currency;

  @override
  Widget build(BuildContext context) {
    final colors = AppFinanceColors.of(context);
    final theme = Theme.of(context);
    final expectedValue = MoneyMath.parse(expected) ?? BigInt.zero;
    final difference = counted - expectedValue;
    final amount = MoneyText.format(MoneyMath.wire(difference.abs()), currency);
    final (background, foreground, icon, title) = switch (difference.sign) {
      0 => (
        colors.incomeContainer,
        colors.onIncomeContainer,
        Icons.check_circle_outline,
        'Tuttu',
      ),
      < 0 => (
        colors.expenseContainer,
        colors.onExpenseContainer,
        Icons.difference_outlined,
        '$amount eksik',
      ),
      _ => (
        colors.neutralContainer,
        colors.onNeutralContainer,
        Icons.difference_outlined,
        '$amount fazla',
      ),
    };
    return Semantics(
      liveRegion: true,
      child: MergeSemantics(
        child: Container(
          padding: const EdgeInsets.symmetric(
            horizontal: AppSpacing.medium,
            vertical: AppSpacing.small + AppSpacing.xSmall,
          ),
          decoration: BoxDecoration(
            color: background,
            borderRadius: BorderRadius.circular(AppRadius.field),
          ),
          child: Row(
            children: [
              Icon(icon, size: 22, color: foreground),
              const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      title,
                      style: theme.textTheme.titleSmall?.copyWith(
                        height: 1.3,
                        color: foreground,
                      ),
                    ),
                    const SizedBox(height: AppSpacing.xxSmall),
                    Text(
                      'Uygulamaya göre ${MoneyText.format(expected, currency)}',
                      style: theme.textTheme.labelSmall?.copyWith(
                        fontWeight: FontWeight.w400,
                        letterSpacing: 0,
                        color: foreground,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Banknot satırları ve madeni para kutusu.
class _BanknoteCard extends StatelessWidget {
  const _BanknoteCard({
    required this.counts,
    required this.currency,
    required this.coinsController,
    required this.onChanged,
  });

  final List<int> counts;
  final String currency;
  final TextEditingController coinsController;
  final void Function(int index, int count) onChanged;

  static const _rowPadding = EdgeInsets.fromLTRB(
    AppSpacing.medium,
    AppSpacing.xSmall,
    AppSpacing.small + AppSpacing.xSmall,
    AppSpacing.xSmall,
  );

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final strong = theme.textTheme.titleSmall?.copyWith(
      height: 1,
      fontFeatures: AppTypography.tabularFigures,
    );
    return AppCard(
      padding: EdgeInsets.zero,
      child: AppDividedColumn(
        inset: AppSpacing.medium,
        children: [
          for (var i = 0; i < counts.length; i++)
            Padding(
              padding: _rowPadding,
              child: ConstrainedBox(
                constraints: const BoxConstraints(minHeight: 56),
                child: Row(
                  children: [
                    SizedBox(
                      width: 64,
                      child: Text(
                        '₺${CashCountSheet.banknotes[i]}',
                        style: strong,
                      ),
                    ),
                    _NoteStepper(
                      value: counts[i],
                      label: '${CashCountSheet.banknotes[i]} lira',
                      onChanged: (count) => onChanged(i, count),
                    ),
                    const SizedBox(width: AppSpacing.small),
                    // Adet sıfırken tutar soluk: `AppMoneyText` rengi rolden
                    // seçtiği için burada düz metin.
                    Expanded(
                      child: Text(
                        MoneyText.format(
                          MoneyMath.wire(
                            MoneyMath.lira(CashCountSheet.banknotes[i]) *
                                BigInt.from(counts[i]),
                          ),
                          currency,
                        ),
                        textAlign: TextAlign.right,
                        style: AppTypography.money(theme.textTheme.bodyMedium!)
                            .copyWith(
                              color: counts[i] == 0
                                  ? surfaces.inkFaint
                                  : surfaces.ink,
                            ),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          Padding(
            padding: _rowPadding,
            child: ConstrainedBox(
              constraints: const BoxConstraints(minHeight: 56),
              child: Row(
                children: [
                  Expanded(child: Text('Madeni para', style: strong)),
                  Container(
                    width: 120,
                    height: 48,
                    padding: const EdgeInsets.symmetric(
                      horizontal: AppSpacing.small + AppSpacing.xSmall,
                    ),
                    decoration: BoxDecoration(
                      color: surfaces.canvas,
                      border: Border.all(color: surfaces.border),
                      borderRadius: BorderRadius.circular(AppRadius.field),
                    ),
                    child: Row(
                      children: [
                        ExcludeSemantics(
                          child: Text('₺', style: theme.textTheme.bodySmall),
                        ),
                        Expanded(
                          child: Semantics(
                            label: 'Madeni para toplamı',
                            child: TextField(
                              controller: coinsController,
                              textAlign: TextAlign.right,
                              keyboardType:
                                  const TextInputType.numberWithOptions(
                                    decimal: true,
                                  ),
                              inputFormatters: const [
                                TurkishAmountInputFormatter(),
                              ],
                              style: theme.textTheme.bodyLarge?.copyWith(
                                fontSize: 15,
                                fontWeight: FontWeight.w500,
                              ),
                              decoration: const InputDecoration(
                                isCollapsed: true,
                                border: InputBorder.none,
                                enabledBorder: InputBorder.none,
                                focusedBorder: InputBorder.none,
                                filled: false,
                                hintText: '0',
                              ),
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

/// Banknot adedi: −/+ düğmeleri (44 dp daire, 48 dp dokunma alanı) ve adet.
class _NoteStepper extends StatelessWidget {
  const _NoteStepper({
    required this.value,
    required this.label,
    required this.onChanged,
  });

  final int value;
  final String label;
  final ValueChanged<int> onChanged;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final style = IconButton.styleFrom(
      fixedSize: const Size.square(44),
      minimumSize: const Size.square(44),
      tapTargetSize: MaterialTapTargetSize.padded,
      backgroundColor: surfaces.card,
      foregroundColor: surfaces.ink,
      side: BorderSide(color: surfaces.border),
    );
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        IconButton(
          style: style,
          tooltip: '$label azalt',
          onPressed: value == 0 ? null : () => onChanged(value - 1),
          icon: const Icon(Icons.remove, size: 20),
        ),
        SizedBox(
          width: 40,
          child: Semantics(
            label: '$label adedi',
            value: '$value',
            excludeSemantics: true,
            child: Text(
              '$value',
              textAlign: TextAlign.center,
              style: Theme.of(context).textTheme.titleSmall?.copyWith(
                height: 1,
                fontFeatures: AppTypography.tabularFigures,
                color: value == 0 ? surfaces.inkFaint : surfaces.ink,
              ),
            ),
          ),
        ),
        IconButton(
          style: style,
          tooltip: '$label artır',
          onPressed: () => onChanged(value + 1),
          icon: const Icon(Icons.add, size: 20),
        ),
      ],
    );
  }
}

/// Tutarın mutlak değeri (dört ondalıklı dize); işaret ayrıca söylenir.
String _abs(String value) {
  final parsed = MoneyMath.parse(value);
  return parsed == null ? value : MoneyMath.wire(parsed.abs());
}

int _sign(String? value) {
  final normalized = value?.trim();
  if (normalized == null || normalized.isEmpty) return 0;
  final digits = normalized.replaceAll(RegExp('[^0-9]'), '');
  if (digits.isEmpty || !digits.contains(RegExp('[1-9]'))) return 0;
  return normalized.startsWith('-') ? -1 : 1;
}

class _DifferenceForm extends StatefulWidget {
  const _DifferenceForm({required this.controller});

  final CashCountController controller;

  @override
  State<_DifferenceForm> createState() => _DifferenceFormState();
}

class _DifferenceFormState extends State<_DifferenceForm> {
  final formKey = GlobalKey<FormState>();
  late final Future<List<DataChoice>> categories;
  String? categoryId;
  CashShortageReason reason = CashShortageReason.expense;

  /// Sunucunun reddettiği son kaydın cümlesi.
  String? submitError;

  @override
  void initState() {
    super.initState();
    categories = widget.controller.loadDifferenceCategories();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final isIncome = widget.controller.differenceCategoryType == 'income';
    final withdrawal = !isIncome && reason == CashShortageReason.withdrawal;
    final unknown = !isIncome && reason == CashShortageReason.unknown;
    return Form(
      key: formKey,
      child: AppFormSheet<CashDifferenceResult>(
        title: isIncome ? 'Fazlayı kaydet' : 'Eksiği kaydet',
        description: isIncome
            ? 'Kasada beklenenden fazla nakit çıktı; bu tek bir gelir kaydı '
                  'olarak yazılır.'
            : 'Kasada beklenenden az nakit çıktı. Neden eksik?',
        submitLabel: withdrawal ? 'Devam' : 'Kaydet',
        onSubmit: _submit,
        children: [
          // Fazla çıkan farkta sebep sorulmaz.
          if (!isIncome) ...[
            AppSegmentRail<CashShortageReason>(
              values: CashShortageReason.values,
              selected: reason,
              semanticLabel: 'Neden eksik?',
              segmentLabel: (value) => value.label,
              onChanged: (value) => setState(() => reason = value),
              segmentBuilder: (context, value, isSelected) => Text(
                value.label,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: theme.textTheme.bodyMedium?.copyWith(
                  fontWeight: FontWeight.w600,
                  color: isSelected ? surfaces.ink : surfaces.inkMuted,
                ),
              ),
            ),
            const SizedBox(height: AppSpacing.medium),
          ],
          if (withdrawal)
            Text(
              'Gider yazılmaz. Sonraki adımda şahsi hesaba aktarır ya da '
              'şahsi gider olarak yazarsınız.',
              style: theme.textTheme.bodySmall?.copyWith(
                color: surfaces.inkMuted,
              ),
            )
          // Sebebi bilmeyen kullanıcıya kategori sorulmaz.
          else if (unknown)
            Text(
              '"$cashDifferenceCategoryName" kategorisine gider olarak '
              'yazılır.',
              style: theme.textTheme.bodySmall?.copyWith(
                color: surfaces.inkMuted,
              ),
            )
          else
            FutureBuilder<List<DataChoice>>(
              future: categories,
              builder: (context, snapshot) {
                if (snapshot.hasError) {
                  return const Text(
                    'Kategoriler yüklenemedi. Paneli kapatıp yeniden deneyin.',
                  );
                }
                if (!snapshot.hasData) return const LinearProgressIndicator();
                return DropdownButtonFormField<String>(
                  initialValue: categoryId,
                  isExpanded: true,
                  decoration: const InputDecoration(labelText: 'Kategori'),
                  items: [
                    for (final category in snapshot.data!)
                      DropdownMenuItem(
                        value: category.id,
                        child: Text(category.name),
                      ),
                  ],
                  onChanged: (value) => categoryId = value,
                  validator: (value) =>
                      value == null ? 'Kategori seçin.' : null,
                );
              },
            ),
          if (submitError != null)
            Padding(
              padding: const EdgeInsets.only(top: AppSpacing.small),
              child: Text(
                submitError!,
                style: theme.textTheme.bodySmall?.copyWith(
                  color: theme.colorScheme.error,
                ),
              ),
            ),
        ],
      ),
    );
  }

  Future<CashDifferenceResult?> _submit() async {
    final isIncome = widget.controller.differenceCategoryType == 'income';
    if (!isIncome && reason == CashShortageReason.withdrawal) {
      return CashDifferenceResult.withdrawal;
    }
    final unknown = !isIncome && reason == CashShortageReason.unknown;
    if (!unknown && !formKey.currentState!.validate()) return null;
    setState(() => submitError = null);
    final saved = await widget.controller.confirmDifference(
      unknown ? null : categoryId,
      unknownReason: unknown,
    );
    if (saved) return CashDifferenceResult.saved;
    if (mounted) {
      setState(() => submitError = widget.controller.errorMessage);
    }
    return null;
  }
}
