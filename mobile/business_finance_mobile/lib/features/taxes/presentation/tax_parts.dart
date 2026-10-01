import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_math.dart';
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_menu_group_label.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_status_tag.dart';
import '../../activities/data/planned_activity_models.dart';
import '../data/tax_models.dart';
import 'tax_schedule.dart';

// Vergi takibinin ortak parçaları. Tasarım teslimindeki `Parts5.jsx`'in
// karşılıklarıdır; yeni token yok, ölçüler oradan.

/// Tam ekran alt sayfa: geri oklu başlık, kayan gövde ve isteğe bağlı sabit
/// alt eylem şeridi (üstte 1 dp çizgi, zemin tuval rengi).
class TaxPageScaffold extends StatelessWidget {
  const TaxPageScaffold({
    required this.title,
    required this.body,
    super.key,
    this.actions = const [],
    this.footer,
  });

  final String title;
  final Widget body;
  final List<Widget> actions;
  final Widget? footer;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return Scaffold(
      body: SafeArea(
        bottom: footer == null,
        child: Column(
          children: [
            AppPageHeader(
              title: title,
              onBack: Navigator.of(context).canPop()
                  ? () => Navigator.of(context).maybePop()
                  : null,
              actions: actions,
            ),
            Expanded(child: body),
          ],
        ),
      ),
      bottomNavigationBar: footer == null
          ? null
          : Container(
              decoration: BoxDecoration(
                color: surfaces.canvas,
                border: Border(top: BorderSide(color: surfaces.border)),
              ),
              child: SafeArea(
                top: false,
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(
                    AppSpacing.medium,
                    AppSpacing.small + AppSpacing.xSmall,
                    AppSpacing.medium,
                    AppSpacing.medium,
                  ),
                  child: SizedBox(width: double.infinity, child: footer),
                ),
              ),
            ),
    );
  }
}

/// Sayfa gövdesinin kaydırılan listesi: yan boşluk 16, altta 32.
class TaxPageBody extends StatelessWidget {
  const TaxPageBody({required this.children, super.key, this.onRefresh});

  final List<Widget> children;
  final Future<void> Function()? onRefresh;

  @override
  Widget build(BuildContext context) {
    final list = ListView(
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.medium,
        AppSpacing.xSmall,
        AppSpacing.medium,
        AppSpacing.xLarge,
      ),
      children: children,
    );
    return onRefresh == null
        ? list
        : RefreshIndicator(onRefresh: onRefresh!, child: list);
  }
}

/// Bölüm: başlık, isteğe bağlı yan eylem, içerik. İlk bölüm dışında üstte 24.
class TaxSection extends StatelessWidget {
  const TaxSection({
    required this.title,
    required this.child,
    super.key,
    this.trailing,
    this.first = false,
  });

  final String title;
  final Widget child;
  final Widget? trailing;
  final bool first;

  @override
  Widget build(BuildContext context) => Padding(
    padding: first
        ? EdgeInsets.zero
        : const EdgeInsets.only(top: AppSpacing.large),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        AppSectionHeader(title: title, trailing: trailing),
        child,
      ],
    ),
  );
}

/// Bir şeyin neden öyle olduğunu söyleyen kısa gri kural metni.
class TaxRule extends StatelessWidget {
  const TaxRule(
    this.text, {
    super.key,
    this.textAlign = TextAlign.start,
    this.top = AppSpacing.small,
  });

  final String text;
  final TextAlign textAlign;
  final double top;

  @override
  Widget build(BuildContext context) => Padding(
    padding: EdgeInsets.only(top: top),
    child: Text(
      text,
      textAlign: textAlign,
      style: Theme.of(
        context,
      ).textTheme.bodyMedium?.copyWith(color: AppSurfaces.of(context).inkMuted),
    ),
  );
}

/// Form içi alan grubu etiketi (14/600) ve isteğe bağlı ipucu.
class TaxFieldLabel extends StatelessWidget {
  const TaxFieldLabel(this.text, {super.key, this.hint});

  final String text;
  final String? hint;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Padding(
      padding: const EdgeInsets.only(
        top: AppSpacing.medium,
        bottom: AppSpacing.small,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            text,
            style: theme.textTheme.bodyMedium?.copyWith(
              fontWeight: FontWeight.w600,
              color: surfaces.ink,
            ),
          ),
          if (hint != null) ...[
            const SizedBox(height: AppSpacing.xxSmall),
            Text(
              hint!,
              style: theme.textTheme.bodyMedium?.copyWith(
                color: surfaces.inkMuted,
              ),
            ),
          ],
        ],
      ),
    );
  }
}

/// Büyük tutar alanı ("Sayımı gir" dili): etiket, 36/700 tutar, altında 2 dp
/// marka çizgisi; Türkçe binlik ayırıcı.
class TaxAmountInput extends StatelessWidget {
  const TaxAmountInput({
    required this.controller,
    required this.label,
    super.key,
    this.autofocus = false,
  });

  final TextEditingController controller;
  final String label;
  final bool autofocus;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final style = AppTypography.heroMoney(theme.textTheme.displaySmall!);
    return Column(
      children: [
        Text(
          label,
          style: theme.textTheme.labelMedium?.copyWith(
            letterSpacing: 0,
            color: surfaces.inkFaint,
          ),
        ),
        const SizedBox(height: AppSpacing.xSmall),
        Container(
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
          child: ListenableBuilder(
            listenable: controller,
            // Simge ve tutar tek grup olarak çizginin ortasında durur; alan
            // yalnız yazılan (ya da yer tutucu) kadar yer kaplar, böylece
            // boş alanda `₺0` sola yaslanmaz.
            builder: (context, _) => Row(
              mainAxisSize: MainAxisSize.min,
              mainAxisAlignment: MainAxisAlignment.center,
              crossAxisAlignment: CrossAxisAlignment.baseline,
              textBaseline: TextBaseline.alphabetic,
              children: [
                ExcludeSemantics(
                  child: Text(
                    '₺',
                    style: style.copyWith(
                      color: controller.text.isEmpty
                          ? surfaces.inkFaint
                          : surfaces.ink,
                    ),
                  ),
                ),
                const SizedBox(width: AppSpacing.xSmall + AppSpacing.xxSmall),
                Flexible(
                  child: IntrinsicWidth(
                    child: Semantics(
                      label: label,
                      child: TextField(
                        controller: controller,
                        autofocus: autofocus,
                        keyboardType: const TextInputType.numberWithOptions(
                          decimal: true,
                        ),
                        inputFormatters: const [TurkishAmountInputFormatter()],
                        style: style,
                        decoration: InputDecoration(
                          isCollapsed: true,
                          // Temanın alan dolgusu burada geçerli değil:
                          // tutar yalnız kendi genişliği ve satır yüksekliği
                          // kadar yer kaplar, yoksa grup ortadan kayar.
                          contentPadding: EdgeInsets.zero,
                          constraints: const BoxConstraints(),
                          border: InputBorder.none,
                          enabledBorder: InputBorder.none,
                          focusedBorder: InputBorder.none,
                          filled: false,
                          hintText: '0',
                          hintStyle: style.copyWith(color: surfaces.inkFaint),
                        ),
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }
}

/// Onay kutulu satır: Material onay kutusu glifi, 56 dp.
///
/// [locked] satır seçili görünür ama değiştirilemez (zaten tanımlı tür).
class TaxCheckRow extends StatelessWidget {
  const TaxCheckRow({
    required this.selected,
    required this.title,
    required this.onChanged,
    super.key,
    this.subtitle,
    this.trailing,
    this.locked = false,
  });

  final bool selected;
  final String title;
  final String? subtitle;
  final Widget? trailing;
  final bool locked;
  final ValueChanged<bool> onChanged;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final iconColor = locked
        ? surfaces.inkFaint
        : (selected ? theme.colorScheme.primary : surfaces.inkMuted);
    return Semantics(
      checked: selected,
      enabled: !locked,
      child: InkWell(
        onTap: locked ? null : () => onChanged(!selected),
        child: ConstrainedBox(
          constraints: const BoxConstraints(minHeight: 56),
          child: Padding(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.small + AppSpacing.xSmall,
              AppSpacing.small + AppSpacing.xxSmall,
              AppSpacing.medium,
              AppSpacing.small + AppSpacing.xxSmall,
            ),
            child: Row(
              children: [
                // Tasarımdaki çerçeveli onay kutusu (Material Symbols Outlined).
                Icon(
                  selected
                      ? Icons.check_box_outlined
                      : Icons.check_box_outline_blank,
                  size: 24,
                  color: iconColor,
                ),
                const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(title, style: theme.textTheme.titleSmall),
                      if (subtitle != null) ...[
                        const SizedBox(height: AppSpacing.xxSmall),
                        Text(
                          subtitle!,
                          style: theme.textTheme.bodyMedium?.copyWith(
                            color: surfaces.inkMuted,
                          ),
                        ),
                      ],
                    ],
                  ),
                ),
                if (trailing != null) ...[
                  const SizedBox(width: AppSpacing.small),
                  trailing!,
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }
}

/// Kart içindeki eylem satırı: ikon 22, başlık ve sonucu anlatan alt satır,
/// en az 64 dp. Yıkıcı eylem kırmızı başlıkla; devre dışı eylem kilitle.
class TaxActionRow extends StatelessWidget {
  const TaxActionRow({
    required this.icon,
    required this.title,
    super.key,
    this.subtitle,
    this.onTap,
    this.danger = false,
    this.disabled = false,
    this.chevron = false,
  });

  final IconData icon;
  final String title;
  final String? subtitle;
  final VoidCallback? onTap;
  final bool danger;
  final bool disabled;
  final bool chevron;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    // Yıkıcı eylem devre dışıyken de kırmızı kalır: ne olduğu değişmedi,
    // yalnız şu an yapılamıyor (sağdaki kilit ve alt satır bunu söyler).
    final color = danger
        ? theme.colorScheme.error
        : (disabled ? surfaces.inkFaint : surfaces.ink);
    return MergeSemantics(
      child: Semantics(
        button: true,
        enabled: !disabled,
        child: InkWell(
          onTap: disabled ? null : onTap,
          child: ConstrainedBox(
            constraints: const BoxConstraints(minHeight: 64),
            child: Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.medium,
                vertical: AppSpacing.small + AppSpacing.xxSmall,
              ),
              child: Row(
                children: [
                  Icon(icon, size: 22, color: color),
                  const SizedBox(width: AppSpacing.medium),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        Text(
                          title,
                          style: theme.textTheme.titleSmall?.copyWith(
                            color: color,
                          ),
                        ),
                        if (subtitle != null) ...[
                          const SizedBox(height: AppSpacing.xxSmall),
                          Text(
                            subtitle!,
                            style: theme.textTheme.bodyMedium?.copyWith(
                              color: surfaces.inkMuted,
                            ),
                          ),
                        ],
                      ],
                    ),
                  ),
                  if (chevron && !disabled)
                    Icon(
                      Icons.chevron_right,
                      size: 20,
                      color: surfaces.inkMuted,
                    ),
                  if (disabled)
                    Icon(
                      Icons.lock_outline,
                      size: 18,
                      color: surfaces.inkMuted,
                    ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

/// Eylem satırlarının kartı: üstte 16, kenarlık, yarıçap 20; ayırıcı ikonun
/// bittiği yerden.
class TaxActionCard extends StatelessWidget {
  const TaxActionCard({required this.children, super.key, this.top = 16});

  final List<Widget> children;
  final double top;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return Padding(
      padding: EdgeInsets.only(top: top),
      child: Material(
        color: surfaces.card,
        clipBehavior: Clip.antiAlias,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppRadius.card),
          side: BorderSide(color: surfaces.border),
        ),
        child: AppDividedColumn(inset: 54, children: children),
      ),
    );
  }
}

/// Kenarlı liste kutusu (kapatılacak kalemler, kapattığı kalemler).
class TaxBorderedList extends StatelessWidget {
  const TaxBorderedList({required this.children, super.key, this.inset = 48});

  final List<Widget> children;
  final double inset;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return Material(
      color: Colors.transparent,
      clipBehavior: Clip.antiAlias,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(AppRadius.card),
        side: BorderSide(color: surfaces.border),
      ),
      child: AppDividedColumn(inset: inset, children: children),
    );
  }
}

/// Ayrıntı panelinin başlığı: 40 dp kapsül, ad, alt satır ve kapat.
class TaxSheetHead extends StatelessWidget {
  const TaxSheetHead({
    required this.icon,
    required this.title,
    required this.subtitle,
    super.key,
    this.tone,
  });

  final IconData icon;
  final String title;
  final String subtitle;
  final AppStatusTone? tone;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Row(
      children: [
        AppIconCapsule(icon: icon, tone: tone),
        const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Semantics(
                header: true,
                child: Text(title, style: theme.textTheme.titleSmall),
              ),
              Text(
                subtitle,
                style: theme.textTheme.bodyMedium?.copyWith(
                  color: AppSurfaces.of(context).inkMuted,
                ),
              ),
            ],
          ),
        ),
        IconButton(
          tooltip: 'Kapat',
          onPressed: () => Navigator.of(context).maybePop(),
          icon: const Icon(Icons.close),
        ),
      ],
    );
  }
}

/// Form panelinin başlığı: 24/700 başlık ve altında tek satır.
class TaxSheetTitle extends StatelessWidget {
  const TaxSheetTitle({required this.title, required this.subtitle, super.key});

  final String title;
  final String subtitle;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Semantics(
          header: true,
          child: Text(title, style: theme.textTheme.headlineSmall),
        ),
        const SizedBox(height: AppSpacing.xSmall),
        Text(subtitle, style: theme.textTheme.bodyLarge),
      ],
    );
  }
}

/// Panel kabuğu: kaydırılan gövde ve isteğe bağlı sabit alt eylem.
///
/// Panel ekranı bütünüyle kaplamaz: tasarımdaki gibi üstte sayfanın başlığı
/// perdenin arkasında görünür kalır (412×892 çerçevede panelin tepesi 74 dp
/// aşağıdadır), yani panel tutamaçtan çekilerek ya da dışına dokunularak
/// kapatılabilir. İçerik sığmıyorsa gövde kayar; [footer] (form panellerinin
/// gönderim butonu) kaydırmanın dışında, klavyenin üstünde hep görünür durur.
class TaxSheetBody extends StatefulWidget {
  const TaxSheetBody({required this.children, super.key, this.footer});

  final List<Widget> children;
  final Widget? footer;

  @override
  State<TaxSheetBody> createState() => _TaxSheetBodyState();
}

class _TaxSheetBodyState extends State<TaxSheetBody> {
  /// Panelin tutamaç alanı (`showDragHandle`), yükseklik payından düşülür.
  static const double _handleExtent = 48;

  /// Panelin (tutamaç ve alt güvenli alan dahil) durum çubuğunun altındaki
  /// alana oranı: 818 / 892.
  static const double _heightFactor = 0.917;

  // Gövde kayıyorsa alt eylemin üstünde ince bir çizgi durur; yoksa buton
  // listenin üstüne binmiş gibi görünür.
  bool _scrolls = false;

  bool _onMetrics(ScrollMetricsNotification notification) {
    final scrolls = notification.metrics.maxScrollExtent > 0;
    if (scrolls != _scrolls) setState(() => _scrolls = scrolls);
    return false;
  }

  @override
  Widget build(BuildContext context) {
    final media = MediaQuery.of(context);
    final keyboard = media.viewInsets.bottom;
    // Panelin içindeki `MediaQuery` üst güvenli alanı taşımaz (panel onun
    // altında açılır); durum çubuğunun yüksekliği pencereden okunur.
    final statusBar = MediaQueryData.fromView(View.of(context)).viewPadding.top;
    final available =
        (media.size.height - statusBar) * _heightFactor -
        _handleExtent -
        media.padding.bottom -
        keyboard;
    final footer = widget.footer;
    return SafeArea(
      child: Padding(
        padding: EdgeInsets.only(bottom: keyboard),
        child: ConstrainedBox(
          constraints: BoxConstraints(
            maxHeight: available < 200 ? 200 : available,
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Flexible(
                child: NotificationListener<ScrollMetricsNotification>(
                  onNotification: _onMetrics,
                  child: SingleChildScrollView(
                    padding: EdgeInsets.fromLTRB(
                      AppSpacing.medium,
                      0,
                      AppSpacing.medium,
                      footer == null ? AppSpacing.large : AppSpacing.small,
                    ),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      mainAxisSize: MainAxisSize.min,
                      children: widget.children,
                    ),
                  ),
                ),
              ),
              if (footer != null)
                DecoratedBox(
                  decoration: BoxDecoration(
                    border: Border(
                      top: BorderSide(
                        color: _scrolls
                            ? AppSurfaces.of(context).border
                            : Colors.transparent,
                      ),
                    ),
                  ),
                  child: Padding(
                    padding: const EdgeInsets.fromLTRB(
                      AppSpacing.medium,
                      AppSpacing.small + AppSpacing.xSmall,
                      AppSpacing.medium,
                      AppSpacing.medium,
                    ),
                    child: footer,
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Panelin alt eylem çubuğu: üstte 1 dp çizgi, en fazla iki eşit genişlikte
/// buton (ikincil solda, birincil sağda). Büyük yazıda alt alta.
class TaxSheetFooter extends StatelessWidget {
  const TaxSheetFooter({required this.children, super.key});

  final List<Widget> children;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final stacked = context.usesLargeText;
    return Container(
      margin: const EdgeInsets.only(top: AppSpacing.large),
      padding: const EdgeInsets.only(top: AppSpacing.medium),
      decoration: BoxDecoration(
        border: Border(top: BorderSide(color: surfaces.border)),
      ),
      child: stacked
          ? Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                for (var i = children.length - 1; i >= 0; i--) ...[
                  children[i],
                  if (i > 0) const SizedBox(height: AppSpacing.small),
                ],
              ],
            )
          : Row(
              children: [
                for (var i = 0; i < children.length; i++) ...[
                  if (i > 0) const SizedBox(width: AppSpacing.small),
                  Expanded(child: children[i]),
                ],
              ],
            ),
    );
  }
}

/// Bekleyen satırının "Ödedim" eylemi: kenarlı hap, 36 dp (dokunma alanı 48),
/// ikon 18 + metin 14/600. Seçili hâli yoktur; kaydedilince satır listeden
/// düşer.
class TaxPayButton extends StatelessWidget {
  const TaxPayButton({required this.onPressed, super.key});

  final VoidCallback? onPressed;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return OutlinedButton.icon(
      onPressed: onPressed,
      style: OutlinedButton.styleFrom(
        minimumSize: const Size(0, 36),
        tapTargetSize: MaterialTapTargetSize.padded,
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.small + AppSpacing.xxSmall,
          0,
          AppSpacing.medium - AppSpacing.xxSmall,
          0,
        ),
        shape: const StadiumBorder(),
        side: BorderSide(color: surfaces.borderStrong),
        backgroundColor: surfaces.card,
        foregroundColor: surfaces.ink,
        textStyle: Theme.of(
          context,
        ).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.w600),
      ),
      icon: const Icon(Icons.check_circle_outline, size: 18),
      label: const Text('Ödedim'),
    );
  }
}

/// Yıkıcı kenarlı buton ("Ödemeyi geri al"): metin gider rengi, kenar gider
/// dolgusu. Yıkıcı eylem hiçbir zaman tek başına kırmızı dolgulu durmaz.
ButtonStyle taxDangerOutlineStyle(BuildContext context) {
  final scheme = Theme.of(context).colorScheme;
  return OutlinedButton.styleFrom(
    minimumSize: const Size.fromHeight(48),
    foregroundColor: scheme.error,
    side: BorderSide(color: scheme.error.withValues(alpha: 0.5)),
  );
}

/// Hesap ve kart seçimi ("Nereden ödendi"). Hesaplar ve kartlar ayrı grup
/// başlıklarıyla; kart seçilirse gider kart harcaması olarak yazılır.
class TaxSourceField extends StatelessWidget {
  const TaxSourceField({
    required this.options,
    required this.value,
    required this.onChanged,
    required this.label,
    super.key,
    this.helperText,
    this.errorText,
    this.allowNone = false,
  });

  final TaxOptions options;
  final String? value;
  final ValueChanged<String?> onChanged;
  final String label;
  final String? helperText;
  final String? errorText;

  /// "Seçilmedi" seçeneği (tanımda isteğe bağlı kaynak).
  final bool allowNone;

  static const _none = '';

  @override
  Widget build(BuildContext context) {
    return DropdownButtonFormField<String>(
      key: ValueKey('tax-source-$value'),
      initialValue: value ?? (allowNone ? _none : null),
      isExpanded: true,
      decoration: InputDecoration(
        labelText: label,
        helperText: helperText,
        helperMaxLines: 3,
        errorText: errorText,
      ),
      items: [
        if (allowNone)
          const DropdownMenuItem(value: _none, child: Text('Seçilmedi')),
        if (options.accounts.isNotEmpty)
          const DropdownMenuItem<String>(
            enabled: false,
            child: AppMenuGroupLabel('Hesaplar'),
          ),
        for (final account in options.accounts)
          DropdownMenuItem(value: account.id, child: Text(account.name)),
        if (options.cards.isNotEmpty)
          const DropdownMenuItem<String>(
            enabled: false,
            child: AppMenuGroupLabel('Kredi kartları'),
          ),
        for (final card in options.cards)
          DropdownMenuItem(value: card.id, child: Text(card.name)),
      ],
      onChanged: (selected) =>
          onChanged(selected == null || selected == _none ? null : selected),
    );
  }
}

/// Türün ikonu (Material Symbols Outlined karşılıkları).
IconData taxKindIcon(TaxKind kind) => switch (kind) {
  TaxKind.socialSecurityPremium => Icons.health_and_safety_outlined,
  TaxKind.vatReturn => Icons.receipt_long_outlined,
  TaxKind.withholdingReturn => Icons.badge_outlined,
  TaxKind.advanceTax => Icons.event_repeat_outlined,
  TaxKind.annualIncomeTax => Icons.account_balance_outlined,
  TaxKind.propertyTax => Icons.home_work_outlined,
  TaxKind.motorVehicleTax => Icons.directions_car_outlined,
  TaxKind.advertisingTax => Icons.signpost_outlined,
  TaxKind.custom => Icons.receipt_long_outlined,
};

IconData taxKindIconOf(String? apiValue) {
  final kind = apiValue == null ? null : TaxKind.fromApiOrNull(apiValue);
  return kind == null ? Icons.receipt_long_outlined : taxKindIcon(kind);
}

/// Bekleyen kalemin durum etiketi: `Gecikti`, `Bugün`, `Yaklaşıyor`.
Widget taxTimingTag(PlannedActivity item, {String? suffix}) {
  final label = switch (item.timing) {
    PlannedTiming.overdue => 'Gecikti',
    PlannedTiming.today => 'Bugün',
    PlannedTiming.upcoming => 'Yaklaşıyor',
  };
  return AppStatusTag(
    label: suffix == null ? label : '$label · $suffix',
    icon: item.timing == PlannedTiming.overdue
        ? Icons.error_outline
        : Icons.schedule,
    tone: item.timing == PlannedTiming.overdue
        ? AppStatusTone.expense
        : AppStatusTone.planned,
  );
}

/// `29 Eylül 2026 · bugün`: tarih alanlarının Türkçe gösterimi.
String taxDateText(String iso, DateTime today) {
  final days = TaxSchedule.daysUntil(iso, today);
  final text = DateText.dayMonthYear(iso);
  return switch (days) {
    0 => '$text · bugün',
    -1 => '$text · dün',
    _ => text,
  };
}
