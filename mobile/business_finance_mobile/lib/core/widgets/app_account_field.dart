import 'package:flutter/material.dart';

import '../formatters/money_text.dart';
import '../theme/app_breakpoints.dart';
import '../theme/app_finance_colors.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import '../theme/app_typography.dart';
import 'app_icon_capsule.dart';
import 'app_money_text.dart';
import 'app_untracked_text.dart';

/// Seçilen paranın yeri: nakit, banka, kredi kartı ya da POS.
enum AppAccountKind { cash, bank, card, pos }

/// Hesap seçicinin bir satırı. Tutar sunucudandır; istemci hesaplamaz.
class AppAccountOption {
  const AppAccountOption({
    required this.id,
    required this.name,
    required this.kind,
    this.scope,
    this.amount,
    this.note,
    this.disabledNote,
  });

  /// Bilinen tek şey türü olan kaba seçenek ([AppAccountKind] tahmin edilmez).
  factory AppAccountOption.fromType(String id, String name, String? type) =>
      AppAccountOption(
        id: id,
        name: name,
        kind: type == 'cash' ? AppAccountKind.cash : AppAccountKind.bank,
      );

  final String id;
  final String name;
  final AppAccountKind kind;

  /// `İşletme` / `Şahsi`; boş olması meşrudur.
  final String? scope;

  /// Hesapta bakiye; kartta eksi işaretli kart borcu.
  final String? amount;

  /// Kapsamın yerine yazan bilgi (POS'ta paranın geçeceği hesap).
  final String? note;

  /// Doluysa satır seçilemez ve alt yazısı budur (transferde kaynak hesap).
  final String? disabledNote;

  bool get enabled => disabledNote == null;

  /// Hesap ile kartın aynı alanda durduğu yerde (`account:…`, `card:…`).
  AppAccountOption withId(String key) => AppAccountOption(
    id: key,
    name: name,
    kind: kind,
    scope: scope,
    amount: amount,
    note: note,
    disabledNote: disabledNote,
  );
}

/// Formlardaki hesap ve kart alanı (Claude Design, hesap seçici H1–H7).
///
/// Kapalı alan seçilen hesabı simge, ad, `tür · kapsam` ve bakiyeyle yazar.
/// Dokununca alanın altında, alan genişliğinde, türe göre gruplu bir menü
/// açılır; seçili satır soldaki çubuk ve zeminle ayrılır. Hiç seçenek yoksa
/// menü yerine alanın altında `Henüz hesap yok` kartı açılır ve form aşağı
/// kayar.
class AppAccountField extends StatefulWidget {
  const AppAccountField({
    required this.label,
    required this.options,
    required this.value,
    required this.onChanged,
    super.key,
    this.validator,
    this.helperText,
    this.errorText,
    this.noneLabel,
    this.emptyTitle = 'Henüz hesap yok',
    this.emptyMessage = 'Kayıt için önce bir hesap ekleyin.',
    this.addLabel = 'Hesap ekle',
    this.onAdd,
    this.cardsFirst = false,
    this.manualLabel,
    this.manualNote,
    this.manualValue = 'manual',
  });

  final String label;
  final List<AppAccountOption> options;
  final String? value;

  /// Boşsa alan kapalıdır (kayıt sürerken).
  final ValueChanged<String?>? onChanged;
  final FormFieldValidator<String>? validator;
  final String? helperText;

  /// Sunucudan dönen alan hatası; doğrulama hatasının önüne geçer.
  final String? errorText;

  /// Doluysa listenin başında boş seçim satırı (`Seçilmedi`, `Tümü`).
  final String? noneLabel;
  final String emptyTitle;
  final String emptyMessage;
  final String addLabel;
  final VoidCallback? onAdd;

  /// Kart grubu en üstte (fişte "kredi kartı" yazıyorsa); seçim değişmez.
  final bool cardsFirst;

  /// Doluysa listenin sonunda kalem simgeli satır: seçenek değil, onları
  /// atlayan yol (POS'ta `Elle gir`). [manualNote] yalnız menüdeki alt
  /// yazıdır; kapalı alanda ad tek başına durur. Değeri [manualValue].
  final String? manualLabel;
  final String? manualNote;
  final String manualValue;

  @override
  State<AppAccountField> createState() => _AppAccountFieldState();
}

class _AppAccountFieldState extends State<AppAccountField> {
  final _menu = MenuController();
  final _fieldKey = GlobalKey();
  bool _open = false;
  bool _emptyOpen = false;

  /// Menü alan genişliğindedir; ölçü açılırken alınır (dialog içinde de
  /// çalışsın diye `LayoutBuilder` kullanılmaz).
  double _width = 280;

  AppAccountOption? get _selected {
    for (final option in widget.options) {
      if (option.id == widget.value) return option;
    }
    return null;
  }

  bool get _manual =>
      widget.manualLabel != null && widget.value == widget.manualValue;

  bool get _isEmpty =>
      widget.options.isEmpty &&
      widget.noneLabel == null &&
      widget.manualLabel == null;

  void _toggle() {
    if (widget.onChanged == null) return;
    if (_isEmpty) {
      setState(() => _emptyOpen = !_emptyOpen);
      return;
    }
    if (_menu.isOpen) {
      _menu.close();
      return;
    }
    final width = _fieldKey.currentContext?.size?.width;
    if (width != null && width != _width) setState(() => _width = width);
    _menu.open();
  }

  void _choose(FormFieldState<String> field, String? id) {
    _menu.close();
    field.didChange(id);
    widget.onChanged?.call(id);
  }

  // Teslimle kurulan bileşen: metin harf aralıksız (`AppUntrackedText`);
  // menü anchor'ın temasını devraldığı için satırlar da aralıksızdır.
  @override
  Widget build(BuildContext context) =>
      AppUntrackedText(child: _field(context));

  Widget _field(BuildContext context) => FormField<String>(
    // Doğrulama alanın kendi kopyasını değil, sayfanın değerini okur.
    validator: widget.validator == null
        ? null
        : (_) => widget.validator!(widget.value),
    builder: (field) => PopScope(
      canPop: !_open,
      onPopInvokedWithResult: (didPop, _) {
        if (!didPop) _menu.close();
      },
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        mainAxisSize: MainAxisSize.min,
        children: [
          MenuAnchor(
            controller: _menu,
            crossAxisUnconstrained: false,
            alignmentOffset: const Offset(0, AppSpacing.small),
            onOpen: () => setState(() => _open = true),
            onClose: () => setState(() => _open = false),
            style: _menuStyle(context, _width),
            menuChildren: _menuRows(context, field, _width),
            builder: (context, _, _) =>
                KeyedSubtree(key: _fieldKey, child: _closed(context, field)),
          ),
          if (_isEmpty && _emptyOpen) ...[
            const SizedBox(height: AppSpacing.small),
            _EmptyCard(
              title: widget.emptyTitle,
              message: widget.emptyMessage,
              addLabel: widget.addLabel,
              onAdd: widget.onAdd,
            ),
          ],
        ],
      ),
    ),
  );

  Widget _closed(BuildContext context, FormFieldState<String> field) {
    final selected = _selected;
    final manual = selected == null && _manual;
    final none = selected == null && !manual && widget.noneLabel != null;
    final blank = selected == null && !none && !manual;
    final open = _open || _emptyOpen;
    final theme = Theme.of(context);
    return Semantics(
      button: true,
      expanded: open,
      label: widget.label,
      value: selected != null
          ? _sentence(selected)
          : manual
          ? widget.manualLabel
          : (none ? widget.noneLabel : null),
      child: ExcludeSemantics(
        child: InkWell(
          borderRadius: BorderRadius.circular(16),
          onTap: widget.onChanged == null ? null : _toggle,
          child: InputDecorator(
            isFocused: open,
            isEmpty: blank,
            decoration: InputDecoration(
              // Değer yokken etiket alanın içinde yer tutucu kalır, açıkken de.
              labelText: blank ? null : widget.label,
              hintText: blank ? widget.label : null,
              helperText: widget.helperText,
              helperMaxLines: 3,
              errorText: widget.errorText ?? field.errorText,
              enabled: widget.onChanged != null,
              contentPadding: selected == null && !manual
                  ? null
                  : const EdgeInsets.fromLTRB(
                      AppSpacing.medium,
                      AppSpacing.small,
                      // Değer ile ok arası 6 (teslim, 28 Ekim).
                      AppSpacing.xSmall + AppSpacing.xxSmall,
                      AppSpacing.small,
                    ),
              suffixIcon: Icon(
                open ? Icons.arrow_drop_up : Icons.arrow_drop_down,
                size: 24,
              ),
            ),
            child: manual || selected != null
                // Alan içerik ne olursa olsun 56 dp: 8 + 40 + 8 (teslim,
                // 28 Ekim). Yalnız büyük yazıda büyür. Kutuya değil içeriğe
                // verilir; alttaki yardım ya da hata yazısı ölçüye girmesin.
                ? SizedBox(
                    height: context.usesLargeText ? null : 40,
                    child: Align(
                      alignment: Alignment.centerLeft,
                      child: manual
                          ? _ManualValue(label: widget.manualLabel!)
                          : _ClosedValue(option: selected!),
                    ),
                  )
                : none
                ? Text(widget.noneLabel!, style: theme.textTheme.bodyLarge)
                : null,
          ),
        ),
      ),
    );
  }

  MenuStyle _menuStyle(BuildContext context, double width) {
    final surfaces = AppSurfaces.of(context);
    return MenuStyle(
      padding: const WidgetStatePropertyAll(
        EdgeInsets.only(top: AppSpacing.xSmall, bottom: AppSpacing.small),
      ),
      backgroundColor: WidgetStatePropertyAll(surfaces.card),
      surfaceTintColor: const WidgetStatePropertyAll(Colors.transparent),
      elevation: const WidgetStatePropertyAll(AppElevation.floating),
      side: const WidgetStatePropertyAll(BorderSide.none),
      minimumSize: WidgetStatePropertyAll(Size(width, 0)),
      maximumSize: WidgetStatePropertyAll(Size(width, 600)),
      shape: WidgetStatePropertyAll(
        RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      ),
    );
  }

  List<Widget> _menuRows(
    BuildContext context,
    FormFieldState<String> field,
    double width,
  ) {
    final rows = <Widget>[];
    if (widget.noneLabel != null) {
      rows.add(
        _MenuRow(
          width: width,
          selected: widget.value == null,
          onPressed: () => _choose(field, null),
          child: Text(
            widget.noneLabel!,
            style: Theme.of(context).textTheme.bodyLarge?.copyWith(
              fontSize: 15,
              height: 1.3,
              fontWeight: widget.value == null
                  ? FontWeight.w600
                  : FontWeight.w400,
            ),
          ),
        ),
      );
    }
    AppAccountKind? group;
    for (final option in _ordered) {
      if (option.kind != group && option.kind != AppAccountKind.pos) {
        rows.add(
          _GroupHeading(
            label: _groupLabel(option.kind),
            divided: rows.isNotEmpty,
          ),
        );
      }
      group = option.kind;
      final selected = option.id == widget.value;
      rows.add(
        _MenuRow(
          width: width,
          selected: selected,
          semanticsLabel: _sentence(option),
          onPressed: option.enabled ? () => _choose(field, option.id) : null,
          child: _OptionContent(option: option, selected: selected),
        ),
      );
    }
    if (widget.manualLabel != null) {
      final selected = _manual;
      rows.add(
        _MenuRow(
          width: width,
          selected: selected,
          semanticsLabel: [widget.manualLabel!, ?widget.manualNote].join(', '),
          onPressed: () => _choose(field, widget.manualValue),
          child: _ManualContent(
            label: widget.manualLabel!,
            note: widget.manualNote,
            selected: selected,
          ),
        ),
      );
    }
    return rows;
  }

  /// Gruplar türe göre: nakit, banka, kart. Grup içi sıra çağıranındır.
  List<AppAccountOption> get _ordered {
    final kinds = <AppAccountKind>[];
    for (final option in widget.options) {
      if (!kinds.contains(option.kind)) kinds.add(option.kind);
    }
    int rank(AppAccountKind kind) =>
        widget.cardsFirst && kind == AppAccountKind.card ? -1 : kind.index;
    kinds.sort((a, b) => rank(a).compareTo(rank(b)));
    return [
      for (final kind in kinds) ...widget.options.where((o) => o.kind == kind),
    ];
  }

  static String _groupLabel(AppAccountKind kind) => switch (kind) {
    AppAccountKind.cash => 'Nakit',
    AppAccountKind.bank => 'Banka',
    AppAccountKind.card => 'Kredi kartı',
    AppAccountKind.pos => 'POS',
  };

  static String _sentence(AppAccountOption option) => [
    option.name,
    accountKindLabel(option.kind),
    ?option.disabledNote,
    if (option.disabledNote == null) ?(option.note ?? option.scope),
    if (option.disabledNote == null && option.amount != null)
      MoneyText.format(option.amount!, 'TRY'),
  ].join(', ');
}

/// Alandaki ve ekran okuyucudaki tür adı.
String accountKindLabel(AppAccountKind kind) => switch (kind) {
  AppAccountKind.cash => 'Nakit',
  AppAccountKind.bank => 'Banka hesabı',
  AppAccountKind.card => 'Kredi kartı',
  AppAccountKind.pos => 'POS',
};

IconData _icon(AppAccountKind kind) => switch (kind) {
  AppAccountKind.cash => Icons.payments_outlined,
  AppAccountKind.bank => Icons.account_balance_outlined,
  AppAccountKind.card => Icons.credit_card,
  AppAccountKind.pos => Icons.point_of_sale_outlined,
};

/// Hesaplar sayfasıyla aynı renkler: nakit ve banka kategori paletinden,
/// kart ve POS nötr.
Color accountKindColor(BuildContext context, AppAccountKind kind) {
  final colors = AppFinanceColors.of(context);
  return switch (kind) {
    AppAccountKind.cash => colors.categorySlice(2),
    AppAccountKind.bank => colors.categorySlice(4),
    AppAccountKind.card || AppAccountKind.pos => colors.neutral,
  };
}

/// Alandaki seçili değer: 32 dp kapsül, ad 15/20 600 ve `tür · kapsam`
/// 13/16, bakiye 14/500 soluk. Ad kısalır, bakiye kısalmaz; büyük yazıda bakiye alta iner.
class _ClosedValue extends StatelessWidget {
  const _ClosedValue({required this.option});
  final AppAccountOption option;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final small = theme.textTheme.bodySmall?.copyWith(
      fontSize: 13,
      height: 16 / 13,
      color: surfaces.inkMuted,
    );
    final amount = option.amount == null
        ? null
        : Text(
            MoneyText.format(option.amount!, 'TRY'),
            style: theme.textTheme.bodyMedium?.copyWith(
              fontSize: 14,
              height: 20 / 14,
              fontWeight: FontWeight.w500,
              color: surfaces.inkMuted,
              fontFeatures: AppTypography.tabularFigures,
            ),
          );
    final detail = [
      accountKindLabel(option.kind),
      ?(option.note ?? option.scope),
    ].join(' · ');
    return Row(
      children: [
        AppIconCapsule(
          icon: _icon(option.kind),
          size: 32,
          iconColor: accountKindColor(context, option.kind),
        ),
        const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                option.name,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: theme.textTheme.titleSmall?.copyWith(
                  fontSize: 15,
                  height: 20 / 15,
                  fontWeight: FontWeight.w600,
                ),
              ),
              Text(
                detail,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: small,
              ),
              if (context.usesLargeText && amount != null) amount,
            ],
          ),
        ),
        if (!context.usesLargeText && amount != null) ...[
          const SizedBox(width: AppSpacing.small),
          amount,
        ],
      ],
    );
  }
}

class _GroupHeading extends StatelessWidget {
  const _GroupHeading({required this.label, required this.divided});
  final String label;
  final bool divided;

  @override
  Widget build(BuildContext context) => ExcludeSemantics(
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      mainAxisSize: MainAxisSize.min,
      children: [
        if (divided) ...[
          const SizedBox(height: AppSpacing.xSmall),
          const Divider(height: 1),
        ],
        Padding(
          padding: const EdgeInsets.fromLTRB(
            AppSpacing.medium,
            AppSpacing.small + AppSpacing.xSmall,
            AppSpacing.medium,
            AppSpacing.xSmall,
          ),
          child: Text(
            label,
            style: Theme.of(context).textTheme.labelMedium?.copyWith(
              fontSize: 12,
              height: 1,
              fontWeight: FontWeight.w600,
              letterSpacing: 0.48,
              color: AppSurfaces.of(context).inkFaint,
            ),
          ),
        ),
      ],
    ),
  );
}

/// Menü satırı: en az 56 dp; seçili satırda solda 4 dp marka çubuğu ve
/// soluk zemin, işaret yok.
class _MenuRow extends StatelessWidget {
  const _MenuRow({
    required this.width,
    required this.selected,
    required this.onPressed,
    required this.child,
    this.semanticsLabel,
  });
  final double width;
  final bool selected;
  final VoidCallback? onPressed;
  final Widget child;
  final String? semanticsLabel;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      selected: selected,
      label: semanticsLabel,
      child: MenuItemButton(
        onPressed: onPressed,
        style: ButtonStyle(
          backgroundColor: WidgetStatePropertyAll(
            selected ? surfaces.cardMuted : null,
          ),
          minimumSize: const WidgetStatePropertyAll(Size.fromHeight(56)),
          padding: const WidgetStatePropertyAll(EdgeInsets.zero),
        ),
        child: SizedBox(
          width: width,
          child: Stack(
            children: [
              Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppSpacing.medium,
                  vertical: AppSpacing.xSmall + AppSpacing.xxSmall,
                ),
                child: semanticsLabel == null
                    ? child
                    : ExcludeSemantics(child: child),
              ),
              if (selected)
                Positioned(
                  left: 0,
                  top: AppSpacing.small,
                  bottom: AppSpacing.small,
                  width: 4,
                  child: DecoratedBox(
                    decoration: BoxDecoration(
                      color: Theme.of(context).colorScheme.primary,
                      borderRadius: const BorderRadius.horizontal(
                        right: Radius.circular(4),
                      ),
                    ),
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Menüdeki hesap: 20 dp renkli simge, ad 15 ve kapsam, tutar 15/600.
/// Kart borcu eksi ve gider rengindedir; kapalı satır soluk, tutarsızdır.
class _OptionContent extends StatelessWidget {
  const _OptionContent({required this.option, required this.selected});
  final AppAccountOption option;
  final bool selected;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final off = !option.enabled;
    final subtitle =
        option.disabledNote ??
        (option.kind == AppAccountKind.card
            ? ['Kart borcu', ?option.scope].join(' · ')
            : option.note ?? option.scope);
    final amount = off || option.amount == null
        ? null
        : AppMoneyText(
            amount: option.amount!,
            currency: 'TRY',
            size: AppMoneySize.body,
            effect: option.amount!.startsWith('-')
                ? AppMoneyEffect.expense
                : null,
            signedByAmount: option.amount!.startsWith('-'),
            style: TextStyle(
              fontSize: 15,
              height: 1.25,
              fontWeight: selected ? FontWeight.w700 : FontWeight.w600,
            ),
          );
    return Row(
      children: [
        Icon(
          _icon(option.kind),
          size: 20,
          color: off
              ? surfaces.inkFaint
              : accountKindColor(context, option.kind),
        ),
        const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                option.name,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: theme.textTheme.bodyLarge?.copyWith(
                  fontSize: 15,
                  height: 1.3,
                  fontWeight: selected ? FontWeight.w600 : FontWeight.w400,
                  color: off ? surfaces.inkFaint : surfaces.ink,
                ),
              ),
              if (subtitle != null)
                Text(
                  subtitle,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: theme.textTheme.bodySmall?.copyWith(
                    fontSize: 13,
                    height: 1.35,
                    color: off ? surfaces.inkFaint : surfaces.inkMuted,
                  ),
                ),
              if (context.usesLargeText && amount != null) amount,
            ],
          ),
        ),
        if (!context.usesLargeText && amount != null) ...[
          const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
          amount,
        ],
      ],
    );
  }
}

/// Hiç hesap yokken alanın altında açılan kart; hiçbir şeyin üstüne binmez.
class _EmptyCard extends StatelessWidget {
  const _EmptyCard({
    required this.title,
    required this.message,
    required this.addLabel,
    required this.onAdd,
  });
  final String title;
  final String message;
  final String addLabel;
  final VoidCallback? onAdd;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      liveRegion: true,
      child: DecoratedBox(
        decoration: BoxDecoration(
          color: surfaces.card,
          border: Border.all(color: surfaces.border),
          borderRadius: BorderRadius.circular(16),
        ),
        child: Padding(
          padding: const EdgeInsets.fromLTRB(
            AppSpacing.medium,
            AppSpacing.small + AppSpacing.xSmall,
            AppSpacing.medium,
            AppSpacing.small,
          ),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Icon(
                Icons.account_balance_wallet_outlined,
                size: 20,
                color: surfaces.inkMuted,
              ),
              const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(
                      title,
                      style: theme.textTheme.titleSmall?.copyWith(
                        fontSize: 15,
                        height: 1.35,
                      ),
                    ),
                    const SizedBox(height: AppSpacing.xxSmall),
                    Text(
                      message,
                      style: theme.textTheme.bodyMedium?.copyWith(
                        fontSize: 14,
                        height: 1.35,
                        color: surfaces.inkMuted,
                      ),
                    ),
                    if (onAdd != null)
                      Padding(
                        padding: const EdgeInsets.only(top: AppSpacing.xSmall),
                        child: TextButton.icon(
                          onPressed: onAdd,
                          style: TextButton.styleFrom(
                            minimumSize: const Size(0, 40),
                            // Simge başlığın hizasında başlar.
                            padding: const EdgeInsets.only(
                              right: AppSpacing.small,
                            ),
                            foregroundColor: surfaces.ink,
                            alignment: Alignment.centerLeft,
                          ),
                          icon: const Icon(Icons.add, size: 18),
                          label: Text(addLabel),
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

/// Kapalı alanda `Elle gir`: hesap satırıyla aynı kapsül ve kalınlık, alt
/// yazı yok.
class _ManualValue extends StatelessWidget {
  const _ManualValue({required this.label});
  final String label;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return Row(
      children: [
        AppIconCapsule(
          icon: Icons.edit_outlined,
          size: 32,
          iconColor: surfaces.inkMuted,
        ),
        const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
        Expanded(
          child: Text(
            label,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: Theme.of(context).textTheme.titleSmall?.copyWith(
              fontSize: 15,
              height: 20 / 15,
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
      ],
    );
  }
}

/// Menünün son satırı: 20 dp kalem simgesi, ad 15 ve alt yazı.
class _ManualContent extends StatelessWidget {
  const _ManualContent({
    required this.label,
    required this.note,
    required this.selected,
  });
  final String label;
  final String? note;
  final bool selected;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Row(
      children: [
        Icon(Icons.edit_outlined, size: 20, color: surfaces.inkMuted),
        const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                label,
                style: theme.textTheme.bodyLarge?.copyWith(
                  fontSize: 15,
                  height: 1.3,
                  fontWeight: selected ? FontWeight.w600 : FontWeight.w400,
                  color: surfaces.ink,
                ),
              ),
              if (note != null)
                Text(
                  note!,
                  style: theme.textTheme.bodySmall?.copyWith(
                    fontSize: 13,
                    height: 1.35,
                    color: surfaces.inkMuted,
                  ),
                ),
            ],
          ),
        ),
      ],
    );
  }
}
