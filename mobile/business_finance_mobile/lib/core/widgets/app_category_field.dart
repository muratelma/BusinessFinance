import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';

import '../models/transaction_scope.dart';
import '../theme/app_breakpoints.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import 'app_category_icon.dart';
import 'app_form_sheet.dart';
import 'app_icon_capsule.dart';
import 'app_untracked_text.dart';

/// Kategori seçicinin bir satırı.
class AppCategoryOption {
  const AppCategoryOption({required this.id, required this.name, this.scope});

  final String id;
  final String name;

  /// Kategorinin kapsamı; boşsa iki kapsama da açıktır.
  final TransactionScope? scope;
}

/// Seçilen kategori ve kapsamı. İki kapsama açık kategoride kapsam menüde
/// kategoriyle birlikte seçilir; tek kapsamlı kategoride kategorinin kendi
/// kapsamıdır.
@immutable
class AppCategoryValue {
  const AppCategoryValue(this.categoryId, [this.scope]);

  final String categoryId;
  final TransactionScope? scope;

  @override
  bool operator ==(Object other) =>
      other is AppCategoryValue &&
      other.categoryId == categoryId &&
      other.scope == scope;

  @override
  int get hashCode => Object.hash(categoryId, scope);
}

/// Formlardaki kategori alanı (Claude Design, kategori seçici K4–Z5).
///
/// Kapalı alan seçilen kategoriyi simge, ad ve kapsamla yazar. Dokununca
/// alan formun başına kayar ve menü **formun içinde**, alanın altında açılır;
/// formun alttaki alanları ekranın dışına itilir. Satıra dokunmak seçer ve
/// kapatır; form eski yerine döner. 8'den fazla kategoride arama ve
/// `Son kullanılanlar` çıkar. İki kapsama açık kategori menüde açılır,
/// kapsam altındaki satırlardan seçilir; formda ayrı bir kapsam sorusu yoktur.
///
/// [fixedScope] doluysa ekranın kapsamı sabittir (POS, gün sonu, cari):
/// yalnız o kapsamın ve iki kapsama açık kategoriler listelenir, kapsam
/// sorulmaz. [showScope] kapalıysa (işletmesi olmayan kullanıcı) liste düzdür
/// ve kapsam hiç yazılmaz.
class AppCategoryField extends StatefulWidget {
  const AppCategoryField({
    required this.options,
    required this.value,
    required this.onChanged,
    super.key,
    this.label = 'Kategori',
    this.recent = const [],
    this.showScope = true,
    this.fixedScope,
    this.validator,
    this.helperText,
    this.errorText,
    this.onAdd,
  });

  final String label;
  final List<AppCategoryOption> options;
  final AppCategoryValue? value;

  /// Boşsa alan kapalıdır (kayıt sürerken).
  final ValueChanged<AppCategoryValue?>? onChanged;

  /// En yeni önce; menü en çok dördünü gösterir.
  final List<AppCategoryValue> recent;
  final bool showScope;
  final TransactionScope? fixedScope;

  /// Kategori seçimini denetler. İki kapsama açık kategoride kapsamın seçilmiş
  /// olmasını alan kendisi denetler.
  final FormFieldValidator<AppCategoryValue>? validator;
  final String? helperText;

  /// Sunucudan dönen alan hatası; doğrulama hatasının önüne geçer.
  final String? errorText;

  /// Yeni kategori ekleme; aranan ad (boş olabilir) verilir.
  final ValueChanged<String>? onAdd;

  /// Arama ve son kullanılanlar bu sayıdan fazla kategoride çıkar.
  static const searchThreshold = 8;

  /// Yazılacak kapsam: tek kapsamlı kategoride kategorinin, iki kapsama açık
  /// kategoride menüde seçilen. Kategori bulunamazsa seçilen kapsam.
  static TransactionScope? scopeOf(
    List<AppCategoryOption> options,
    AppCategoryValue? value,
  ) {
    if (value == null) return null;
    for (final option in options) {
      if (option.id == value.categoryId) return option.scope ?? value.scope;
    }
    return value.scope;
  }

  /// Menüde gösterilen en çok son kullanılan.
  static const recentLimit = 4;

  @override
  State<AppCategoryField> createState() => _AppCategoryFieldState();
}

class _AppCategoryFieldState extends State<AppCategoryField> {
  final _fieldKey = GlobalKey();
  final _menuKey = GlobalKey();
  final _query = TextEditingController();
  final _searchFocus = FocusNode();
  bool _open = false;
  bool _emptyOpen = false;
  String? _expanded;
  double? _savedOffset;
  double? _usedViewport;

  /// `+ ekle`ye basıldı; form yeni kategoriyle dönerse ve kategori iki
  /// kapsama açıksa kapsam menüdeki gibi sorulur (kullanıcı kararı D4).
  bool _awaitingAdd = false;

  @override
  void didUpdateWidget(AppCategoryField oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (!_awaitingAdd || widget.value == oldWidget.value) return;
    _awaitingAdd = false;
    final added = _selected;
    if (added == null || !_needsScope(added) || widget.value?.scope != null) {
      return;
    }
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      _openMenu();
      // Uzun listede yeni kategori görünür olsun diye arama onun adıyla dolar.
      if (_big) _query.text = added.name;
      setState(() => _expanded = added.id);
    });
  }

  @override
  void dispose() {
    _query.dispose();
    _searchFocus.dispose();
    super.dispose();
  }

  AppCategoryOption? _optionOf(String? id) {
    for (final option in widget.options) {
      if (option.id == id) return option;
    }
    return null;
  }

  AppCategoryOption? get _selected => _optionOf(widget.value?.categoryId);

  bool get _choosesScope => widget.showScope && widget.fixedScope == null;

  bool _needsScope(AppCategoryOption option) =>
      _choosesScope && option.scope == null;

  /// Kapalı alanın alt yazısı ve kaydın yazılacağı kapsam.
  TransactionScope? _scopeOf(AppCategoryOption option) =>
      option.scope ?? widget.fixedScope ?? widget.value?.scope;

  List<AppCategoryOption> get _visible {
    final fixed = widget.fixedScope;
    if (fixed == null || !widget.showScope) return widget.options;
    return [
      for (final option in widget.options)
        if (option.scope == null || option.scope == fixed) option,
    ];
  }

  bool get _big => _visible.length > AppCategoryField.searchThreshold;

  void _toggle() {
    if (widget.onChanged == null) return;
    if (widget.options.isEmpty) {
      setState(() => _emptyOpen = !_emptyOpen);
      return;
    }
    _open ? _close() : _openMenu();
  }

  void _openMenu() {
    final position = Scrollable.maybeOf(context)?.position;
    _savedOffset = position?.hasPixels == true ? position!.pixels : null;
    _query.clear();
    setState(() {
      _open = true;
      _expanded = null;
    });
    WidgetsBinding.instance.addPostFrameCallback((_) => _scrollFieldToTop());
  }

  /// Alanı kaydırılan gövdenin başına getirir; üstünde bir ölçek boşluğu kalır.
  void _scrollFieldToTop() {
    if (!mounted) return;
    final box = _fieldKey.currentContext?.findRenderObject();
    final position = Scrollable.maybeOf(context)?.position;
    if (box is! RenderBox || position == null) return;
    final viewport = RenderAbstractViewport.maybeOf(box);
    if (viewport == null) return;
    final target =
        (viewport.getOffsetToReveal(box, 0).offset - AppSpacing.medium).clamp(
          position.minScrollExtent,
          position.maxScrollExtent,
        );
    position.animateTo(
      target,
      duration: const Duration(milliseconds: 200),
      curve: Curves.easeOutCubic,
    );
  }

  void _close() {
    _searchFocus.unfocus();
    _sheetFit?.height.value = null;
    setState(() {
      _open = false;
      _expanded = null;
    });
    final saved = _savedOffset;
    _savedOffset = null;
    if (saved == null) return;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      final position = Scrollable.maybeOf(context)?.position;
      if (position == null) return;
      position.animateTo(
        saved.clamp(position.minScrollExtent, position.maxScrollExtent),
        duration: const Duration(milliseconds: 200),
        curve: Curves.easeOutCubic,
      );
    });
  }

  void _choose(FormFieldState<AppCategoryValue> field, AppCategoryValue value) {
    field.didChange(value);
    widget.onChanged?.call(value);
    _close();
  }

  void _add() {
    final name = _query.text.trim();
    if (_open) _close();
    _awaitingAdd = true;
    widget.onAdd?.call(name);
  }

  // Teslimle kurulan bileşen: metin harf aralıksız.
  @override
  Widget build(BuildContext context) =>
      AppUntrackedText(child: _field(context));

  Widget _field(BuildContext context) => FormField<AppCategoryValue>(
    // Doğrulama alanın kendi kopyasını değil, sayfanın değerini okur.
    validator: (_) {
      final error = widget.validator?.call(widget.value);
      if (error != null) return error;
      final selected = _selected;
      if (selected != null &&
          _needsScope(selected) &&
          widget.value?.scope == null) {
        return 'Kapsam seçin.';
      }
      return null;
    },
    builder: (field) => PopScope(
      canPop: !_open,
      onPopInvokedWithResult: (didPop, _) {
        if (!didPop && _open) _close();
      },
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        mainAxisSize: MainAxisSize.min,
        children: [
          // Alanın ve menünün dışına dokunmak seçmeden kapatır (hesap
          // alanındaki gibi). Menünün altındaki boş yer dışarı sayılır.
          TapRegion(
            groupId: this,
            onTapOutside: (_) {
              if (_open) _close();
            },
            child: KeyedSubtree(key: _fieldKey, child: _closed(context, field)),
          ),
          if (_open) _menuArea(context, field),
          if (widget.options.isEmpty && _emptyOpen) ...[
            const SizedBox(height: AppSpacing.small),
            _EmptyCard(onAdd: widget.onAdd == null ? null : _add),
          ],
        ],
      ),
    ),
  );

  Widget _closed(BuildContext context, FormFieldState<AppCategoryValue> field) {
    final selected = _selected;
    final open = _open || _emptyOpen;
    final scope = selected == null || !widget.showScope
        ? null
        : _scopeOf(selected);
    final error = widget.errorText ?? field.errorText;
    final sentence = selected == null
        ? null
        : [
            selected.name,
            if (widget.showScope) scope?.label ?? 'kapsam seçilmedi',
          ].join(', ');
    return Semantics(
      button: true,
      expanded: open,
      label: widget.label,
      value: sentence,
      child: ExcludeSemantics(
        child: InkWell(
          borderRadius: BorderRadius.circular(16),
          onTap: widget.onChanged == null ? null : _toggle,
          child: InputDecorator(
            isFocused: open,
            isEmpty: selected == null,
            decoration: InputDecoration(
              // Değer yokken etiket alanın içinde yer tutucu kalır, açıkken de.
              labelText: selected == null ? null : widget.label,
              hintText: selected == null ? widget.label : null,
              helperText: widget.helperText,
              helperMaxLines: 3,
              errorText: error,
              enabled: widget.onChanged != null,
              contentPadding: selected == null
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
            child: selected == null
                ? null
                // Alan içerik ne olursa olsun 56 dp: 8 + 40 + 8. Yalnız büyük
                // yazıda büyür.
                : SizedBox(
                    height: context.usesLargeText ? null : 40,
                    child: Align(
                      alignment: Alignment.centerLeft,
                      child: _ClosedValue(
                        name: selected.name,
                        subtitle: !widget.showScope
                            ? null
                            : scope?.label ?? 'Kapsam seçin',
                        missing: widget.showScope && scope == null,
                      ),
                    ),
                  ),
          ),
        ),
      ),
    );
  }

  /// Menünün yeri. Tam sayfa formda alanın altından kaydırılan gövdenin
  /// sonuna kadardır: menü içerik kadar uzar, en çok bu yerin 16 dp üstüne
  /// kadar; geri kalanı boş kalır ki formun alttaki alanları ekranın dışında
  /// dursun. Alttan açılan panelde (`AppFormSheet`) boş yer bırakılmaz: panelin
  /// gövdesi alan + menü kadar boylanır (kullanıcı kararı, 28 Ekim 2026).
  Widget _menuArea(
    BuildContext context,
    FormFieldState<AppCategoryValue> field,
  ) {
    // Klavye açılınca yeniden ölçülür.
    MediaQuery.viewInsetsOf(context);
    final sheet = _sheetFit;
    if (sheet != null) {
      // Panelde boş yer bırakılmaz: panel alan + menü kadar daralır.
      final fieldHeight =
          (_fieldKey.currentContext?.findRenderObject() as RenderBox?)
              ?.size
              .height ??
          56;
      final cap =
          (sheet.maxHeight -
                  AppSpacing.medium * 2 -
                  fieldHeight -
                  AppSpacing.small)
              .clamp(120.0, double.infinity);
      WidgetsBinding.instance.addPostFrameCallback((_) => _fitSheet());
      return Padding(
        key: _menuKey,
        padding: const EdgeInsets.only(top: AppSpacing.small),
        child: ConstrainedBox(
          constraints: BoxConstraints(maxHeight: cap),
          child: TapRegion(groupId: this, child: _menuOf(context, field)),
        ),
      );
    }
    final position = Scrollable.maybeOf(context)?.position;
    final viewport = position != null && position.hasViewportDimension
        ? position.viewportDimension
        : null;
    _usedViewport = viewport;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted || !_open) return;
      final now = Scrollable.maybeOf(context)?.position;
      if (now != null &&
          now.hasViewportDimension &&
          now.viewportDimension != _usedViewport) {
        setState(() {});
      }
    });
    final fieldHeight =
        (_fieldKey.currentContext?.findRenderObject() as RenderBox?)
            ?.size
            .height ??
        56;
    final below = viewport == null
        ? null
        : viewport - AppSpacing.medium - fieldHeight;
    final maxHeight = below == null
        ? 360.0
        : (below - AppSpacing.small - AppSpacing.medium).clamp(
            120.0,
            double.infinity,
          );
    final menu = Padding(
      padding: const EdgeInsets.only(top: AppSpacing.small),
      child: ConstrainedBox(
        constraints: BoxConstraints(maxHeight: maxHeight),
        child: TapRegion(groupId: this, child: _menuOf(context, field)),
      ),
    );
    if (below == null) return menu;
    return SizedBox(
      height: below.clamp(maxHeight + AppSpacing.small, double.infinity),
      child: Align(alignment: Alignment.topCenter, child: menu),
    );
  }

  AppFormSheetFit? get _sheetFit => AppFormSheetFit.maybeOf(context);

  /// Panelde: gövde alan + menü kadar daralır, sonra alan başa kayar. Menünün
  /// gerçek boyu ancak çizildikten sonra bilinir.
  void _fitSheet() {
    if (!mounted || !_open) return;
    final fit = _sheetFit;
    final field = _fieldKey.currentContext?.findRenderObject();
    final menu = _menuKey.currentContext?.findRenderObject();
    if (fit == null || field is! RenderBox || menu is! RenderBox) return;
    final wanted =
        AppSpacing.medium +
        field.size.height +
        menu.size.height +
        AppSpacing.medium;
    if (fit.height.value == wanted) return;
    fit.height.value = wanted;
    WidgetsBinding.instance.addPostFrameCallback((_) => _scrollFieldToTop());
  }

  Widget _menuOf(
    BuildContext context,
    FormFieldState<AppCategoryValue> field,
  ) => _Menu(
    search: _big ? _search(context) : null,
    rows: _rows(context, field),
  );

  Widget _search(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final theme = Theme.of(context);
    OutlineInputBorder border(Color color) => OutlineInputBorder(
      borderRadius: BorderRadius.circular(12),
      borderSide: BorderSide(color: color, width: 2),
    );
    return Padding(
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.small,
        AppSpacing.small,
        AppSpacing.small,
        AppSpacing.xSmall,
      ),
      child: SizedBox(
        height: 44,
        child: ValueListenableBuilder(
          valueListenable: _query,
          builder: (context, value, _) => TextField(
            controller: _query,
            focusNode: _searchFocus,
            onChanged: (_) => setState(() => _expanded = null),
            textInputAction: TextInputAction.search,
            style: theme.textTheme.bodyLarge?.copyWith(fontSize: 15),
            decoration: InputDecoration(
              isDense: true,
              filled: true,
              fillColor: surfaces.cardMuted,
              hintText: 'Kategori ara',
              contentPadding: const EdgeInsets.symmetric(
                vertical: AppSpacing.small + AppSpacing.xSmall,
              ),
              prefixIcon: Icon(
                Icons.search,
                size: 20,
                color: surfaces.inkMuted,
              ),
              suffixIcon: value.text.isEmpty
                  ? null
                  : IconButton(
                      tooltip: 'Aramayı temizle',
                      icon: Icon(
                        Icons.close,
                        size: 20,
                        color: surfaces.inkMuted,
                      ),
                      onPressed: () => setState(() {
                        _query.clear();
                        _expanded = null;
                      }),
                    ),
              border: border(Colors.transparent),
              enabledBorder: border(Colors.transparent),
              focusedBorder: border(theme.colorScheme.primary),
            ),
          ),
        ),
      ),
    );
  }

  List<Widget> _rows(
    BuildContext context,
    FormFieldState<AppCategoryValue> field,
  ) {
    final query = _query.text.trim();
    final matches = [
      for (final option in _visible)
        if (query.isEmpty || _matchIndex(option.name, query) >= 0) option,
    ];
    if (matches.isEmpty) {
      return [
        _NoMatch(query: query, onAdd: widget.onAdd == null ? null : _add),
      ];
    }
    final rows = <Widget>[];
    void heading(String label) =>
        rows.add(_GroupHeading(label: label, divided: rows.isNotEmpty));

    if (!widget.showScope) {
      for (final option in matches) {
        rows.add(_row(field, option, query));
      }
      return rows;
    }

    final recent = _recentRows(field);
    if (_big && query.isEmpty && recent.isNotEmpty) {
      heading('Son kullanılanlar');
      rows.addAll(recent);
    }
    for (final (label, scope) in const [
      ('İşletme', TransactionScope.business),
      ('Şahsi', TransactionScope.personal),
      ('İşletme veya şahsi', null),
    ]) {
      final group = [
        for (final option in matches)
          if (option.scope == scope) option,
      ];
      if (group.isEmpty) continue;
      heading(label);
      for (final option in group) {
        rows.add(_row(field, option, query));
        if (_expanded == option.id) {
          for (final sub in TransactionScope.values) {
            rows.add(
              _ScopeRow(
                scope: sub,
                selected: widget.value == AppCategoryValue(option.id, sub),
                onTap: () => _choose(field, AppCategoryValue(option.id, sub)),
              ),
            );
          }
        }
      }
    }
    return rows;
  }

  Widget _row(
    FormFieldState<AppCategoryValue> field,
    AppCategoryOption option,
    String query,
  ) {
    final needsScope = _needsScope(option);
    final expanded = _expanded == option.id;
    final selected = !expanded && widget.value?.categoryId == option.id;
    return _OptionRow(
      name: option.name,
      query: query,
      selected: selected,
      bold: selected || expanded,
      trailingIcon: needsScope
          ? (expanded ? Icons.expand_less : Icons.expand_more)
          : null,
      semanticsHint: needsScope ? 'Kapsamı seçmek için açın' : null,
      onTap: needsScope
          ? () => setState(() => _expanded = expanded ? null : option.id)
          : () => _choose(
              field,
              AppCategoryValue(
                option.id,
                widget.showScope ? option.scope ?? widget.fixedScope : null,
              ),
            ),
    );
  }

  /// Son kullanılan çiftlerden bu ekranda seçilebilenler. Sonradan tek
  /// kapsama daraltılmış kategorinin öbür kapsamdaki satırı düşer.
  List<Widget> _recentRows(FormFieldState<AppCategoryValue> field) {
    final rows = <Widget>[];
    final visible = {for (final option in _visible) option.id: option};
    for (final item in widget.recent) {
      final option = visible[item.categoryId];
      final scope = item.scope;
      if (option == null || scope == null) continue;
      if (option.scope != null && option.scope != scope) continue;
      if (widget.fixedScope != null && widget.fixedScope != scope) continue;
      rows.add(
        _OptionRow(
          name: option.name,
          query: '',
          selected: false,
          bold: false,
          trailingText: scope.label,
          onTap: () => _choose(field, AppCategoryValue(option.id, scope)),
        ),
      );
      if (rows.length == AppCategoryField.recentLimit) break;
    }
    return rows;
  }
}

/// Türkçe büyük/küçük harfi tanıyan arama: `İ`/`I`, `i`/`ı` çiftleri doğru
/// küçülür ve uzunluk değişmez, eşleşen harfler yerinde kalınlaşır.
String _lowerTr(String text) =>
    text.replaceAll('İ', 'i').replaceAll('I', 'ı').toLowerCase();

int _matchIndex(String name, String query) =>
    _lowerTr(name).indexOf(_lowerTr(query));

class _Menu extends StatelessWidget {
  const _Menu({required this.search, required this.rows});
  final Widget? search;
  final List<Widget> rows;

  @override
  Widget build(BuildContext context) => Material(
    color: AppSurfaces.of(context).card,
    elevation: AppElevation.floating,
    borderRadius: BorderRadius.circular(16),
    clipBehavior: Clip.antiAlias,
    child: Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        ?search,
        Flexible(
          child: ScrollConfiguration(
            // Liste menünün içinde kayar; kaydırma çubuğu görünmez.
            behavior: ScrollConfiguration.of(
              context,
            ).copyWith(scrollbars: false),
            child: ListView(
              shrinkWrap: true,
              padding: EdgeInsets.only(
                top: search == null ? AppSpacing.xSmall : 0,
                bottom: AppSpacing.small,
              ),
              children: rows,
            ),
          ),
        ),
      ],
    ),
  );
}

class _GroupHeading extends StatelessWidget {
  const _GroupHeading({required this.label, required this.divided});
  final String label;
  final bool divided;

  @override
  Widget build(BuildContext context) => Semantics(
    header: true,
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

/// Menü satırı: en az 48 dp, 20 dp simge, ad 15. Seçili satırda solda 4 dp
/// marka çubuğu ve soluk zemin; aramayla eşleşen harfler kalın.
class _OptionRow extends StatelessWidget {
  const _OptionRow({
    required this.name,
    required this.query,
    required this.selected,
    required this.bold,
    required this.onTap,
    this.trailingIcon,
    this.trailingText,
    this.semanticsHint,
  });
  final String name;
  final String query;
  final bool selected;
  final bool bold;
  final VoidCallback onTap;
  final IconData? trailingIcon;
  final String? trailingText;
  final String? semanticsHint;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final style = theme.textTheme.bodyLarge?.copyWith(
      fontSize: 15,
      height: 1.3,
      fontWeight: bold ? FontWeight.w600 : FontWeight.w400,
      color: surfaces.ink,
    );
    final index = query.isEmpty ? -1 : _matchIndex(name, query);
    final text = index < 0
        ? TextSpan(text: name)
        : TextSpan(
            children: [
              TextSpan(text: name.substring(0, index)),
              TextSpan(
                text: name.substring(index, index + query.length),
                style: const TextStyle(fontWeight: FontWeight.w700),
              ),
              TextSpan(text: name.substring(index + query.length)),
            ],
          );
    return Semantics(
      button: true,
      selected: selected,
      label: [name, ?trailingText].join(', '),
      hint: semanticsHint,
      excludeSemantics: true,
      child: _Selectable(
        selected: selected,
        onTap: onTap,
        padding: const EdgeInsets.symmetric(horizontal: AppSpacing.medium),
        child: Row(
          children: [
            Icon(categoryIcon(name), size: 20, color: surfaces.inkMuted),
            const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
            Expanded(
              child: Text.rich(
                text,
                style: style,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
            ),
            if (trailingText != null) ...[
              const SizedBox(width: AppSpacing.small),
              Text(
                trailingText!,
                style: theme.textTheme.bodySmall?.copyWith(
                  fontSize: 13,
                  height: 1.35,
                  color: surfaces.inkMuted,
                ),
              ),
            ],
            if (trailingIcon != null) ...[
              const SizedBox(width: AppSpacing.small),
              Icon(trailingIcon, size: 20, color: surfaces.inkMuted),
            ],
          ],
        ),
      ),
    );
  }
}

/// Açılan kategorinin altındaki kapsam satırı: 48 dp, solda 48 dp boşluk.
class _ScopeRow extends StatelessWidget {
  const _ScopeRow({
    required this.scope,
    required this.selected,
    required this.onTap,
  });
  final TransactionScope scope;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      button: true,
      selected: selected,
      label: scope.label,
      excludeSemantics: true,
      child: _Selectable(
        selected: selected,
        onTap: onTap,
        padding: const EdgeInsets.only(
          left: AppSpacing.xxLarge,
          right: AppSpacing.medium,
        ),
        child: Row(
          children: [
            Icon(
              scope == TransactionScope.business
                  ? Icons.storefront_outlined
                  : Icons.person_outline,
              size: 20,
              color: surfaces.inkMuted,
            ),
            const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
            Expanded(
              child: Text(
                scope.label,
                style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                  fontSize: 15,
                  height: 1.3,
                  fontWeight: selected ? FontWeight.w600 : FontWeight.w400,
                  color: surfaces.ink,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _Selectable extends StatelessWidget {
  const _Selectable({
    required this.selected,
    required this.onTap,
    required this.padding,
    required this.child,
  });
  final bool selected;
  final VoidCallback onTap;
  final EdgeInsets padding;
  final Widget child;

  @override
  Widget build(BuildContext context) => Material(
    color: selected ? AppSurfaces.of(context).cardMuted : Colors.transparent,
    child: InkWell(
      onTap: onTap,
      child: Stack(
        children: [
          ConstrainedBox(
            constraints: const BoxConstraints(minHeight: 48),
            child: Padding(
              padding: padding,
              child: Align(alignment: Alignment.centerLeft, child: child),
            ),
          ),
          if (selected)
            Positioned(
              left: 0,
              top: AppSpacing.small,
              bottom: AppSpacing.small,
              width: AppSpacing.xSmall,
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
  );
}

/// Kapalı alandaki değer: 32 dp kapsül, ad 15/20 600 ve kapsam 13/16.
class _ClosedValue extends StatelessWidget {
  const _ClosedValue({
    required this.name,
    required this.subtitle,
    required this.missing,
  });
  final String name;
  final String? subtitle;

  /// Kapsam henüz seçilmedi (fişten gelen öneri gibi).
  final bool missing;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return Row(
      children: [
        AppIconCapsule(
          icon: categoryIcon(name),
          size: 32,
          iconColor: surfaces.ink,
        ),
        const SizedBox(width: AppSpacing.small + AppSpacing.xSmall),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                name,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: theme.textTheme.titleSmall?.copyWith(
                  fontSize: 15,
                  height: 20 / 15,
                  fontWeight: FontWeight.w600,
                ),
              ),
              if (subtitle != null)
                Text(
                  subtitle!,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: theme.textTheme.bodySmall?.copyWith(
                    fontSize: 13,
                    height: 16 / 13,
                    color: missing
                        ? theme.colorScheme.error
                        : surfaces.inkMuted,
                  ),
                ),
            ],
          ),
        ),
      ],
    );
  }
}

/// Aramayla eşleşen kategori yok (hesap seçicinin H7 dili).
class _NoMatch extends StatelessWidget {
  const _NoMatch({required this.query, required this.onAdd});
  final String query;
  final VoidCallback? onAdd;

  @override
  Widget build(BuildContext context) => _Notice(
    icon: Icons.search_off,
    title: 'Bu adla kategori yok',
    message: onAdd == null
        ? 'Başka bir adla arayın.'
        : 'Aradığınız adla yeni kategori ekleyebilirsiniz.',
    addLabel: '"$query" ekle',
    onAdd: onAdd,
  );
}

/// Hiç kategori yokken alanın altında açılan kart.
class _EmptyCard extends StatelessWidget {
  const _EmptyCard({required this.onAdd});
  final VoidCallback? onAdd;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return DecoratedBox(
      decoration: BoxDecoration(
        color: surfaces.card,
        border: Border.all(color: surfaces.border),
        borderRadius: BorderRadius.circular(16),
      ),
      child: _Notice(
        icon: Icons.category_outlined,
        title: 'Henüz kategori yok',
        message: 'Kayıt için önce bir kategori ekleyin.',
        addLabel: 'Kategori ekle',
        onAdd: onAdd,
      ),
    );
  }
}

class _Notice extends StatelessWidget {
  const _Notice({
    required this.icon,
    required this.title,
    required this.message,
    required this.addLabel,
    required this.onAdd,
  });
  final IconData icon;
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
            Icon(icon, size: 20, color: surfaces.inkMuted),
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
                          padding: const EdgeInsets.only(
                            right: AppSpacing.small,
                          ),
                          foregroundColor: surfaces.ink,
                          alignment: Alignment.centerLeft,
                          textStyle: const TextStyle(
                            fontSize: 14,
                            fontWeight: FontWeight.w600,
                          ),
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
    );
  }
}
