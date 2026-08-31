import 'package:flutter/material.dart';

import '../../../core/formatters/money_text.dart';
import '../../../core/models/tax_fields.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_submit_button.dart';
import '../data/obligation_direction.dart';
import 'obligation_controller.dart';
import 'obligation_prefill.dart';

class ObligationFormPage extends StatefulWidget {
  const ObligationFormPage({
    required this.controller,
    required this.prefill,
    super.key,
    this.scopeController,
    this.today,
  });

  final ObligationController controller;
  final ObligationPrefill prefill;
  final ScopeController? scopeController;
  final DateTime? today;

  @override
  State<ObligationFormPage> createState() => _ObligationFormPageState();
}

class _ObligationFormPageState extends State<ObligationFormPage> {
  final _formKey = GlobalKey<FormState>();
  final _amount = TextEditingController();
  final _description = TextEditingController();
  final _vatRate = TextEditingController();
  final _vatAmount = TextEditingController();
  late ObligationDirection _direction =
      widget.prefill.direction ?? ObligationDirection.payable;
  late DateTime _issueDate = widget.today ?? DateTime.now();
  late DateTime _dueDate = _issueDate;
  String? _categoryId;
  String? _counterpartyId;
  TransactionScope? _explicitScope;
  bool _scopeMissing = false;

  /// Vergi bölümü açık mı. Boş formda kapalı başlar; KDV'si olmayan bir
  /// yükümlülük için form sadeliğini bozmaz.
  bool _taxExpanded = false;

  @override
  void initState() {
    super.initState();
    _applyPrefill();
    widget.controller.addListener(_changed);
    widget.controller.load(direction: _direction);
  }

  void _applyPrefill() {
    final prefill = widget.prefill;
    if (prefill.amount case final suggestion?) {
      _amount.text = MoneyText.editable(suggestion.value);
    }
    if (prefill.description case final suggestion?) {
      _description.text = suggestion.value;
    }
    _issueDate = AppDateField.parse(prefill.issueDate?.value) ?? _issueDate;
    _dueDate = AppDateField.parse(prefill.dueDate?.value) ?? _issueDate;
    _categoryId = prefill.categoryId?.value;
    _counterpartyId = prefill.counterpartyId;
    // Belgeden KDV okunduysa bölüm **açık** açılır: kapalı bir bölümün
    // arkasında duran öneri, kullanıcının kontrol edemediği bir öneridir.
    if (prefill.vat case final vat? when !vat.isEmpty) {
      _vatRate.text = vat.ratePercentInput ?? '';
      _vatAmount.text = vat.amount == null
          ? ''
          : MoneyText.editable(vat.amount!);
      _taxExpanded = true;
    }
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  @override
  void dispose() {
    widget.controller.removeListener(_changed);
    widget.controller.dispose();
    _amount.dispose();
    _description.dispose();
    _vatRate.dispose();
    _vatAmount.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(title: Text(_title)),
    body: AnimatedBuilder(
      animation: widget.scopeController ?? const _NeverListenable(),
      builder: (context, _) => _body(context),
    ),
  );

  Widget _body(BuildContext context) {
    if (widget.controller.unauthorized) return const AppUnauthorizedView();
    if (widget.controller.isLoading) {
      return const AppLoadingView(message: 'Form seçenekleri yükleniyor');
    }
    final options = widget.controller.options;
    if (options == null) {
      return AppErrorView(
        message: widget.controller.errorMessage ?? 'Seçenekler yüklenemedi.',
        onRetry: widget.controller.load,
      );
    }

    if (_counterpartyId != null &&
        !options.counterparties.any((item) => item.id == _counterpartyId)) {
      _counterpartyId = null;
    }

    return Form(
      key: _formKey,
      child: ListView(
        padding: const EdgeInsets.all(AppSpacing.medium),
        children: [
          AppInlineNotice(
            icon: Icons.receipt_long_outlined,
            message: _noticeMessage,
          ),
          if (_canChooseDirection) ...[
            const SizedBox(height: AppSpacing.medium),
            Semantics(
              label: 'Yükümlülüğün yönü',
              child: SegmentedButton<ObligationDirection>(
                segments: [
                  for (final option in ObligationDirection.values)
                    ButtonSegment(
                      value: option,
                      icon: Icon(
                        option == ObligationDirection.payable
                            ? Icons.north_east
                            : Icons.south_west,
                      ),
                      label: Text(option.label),
                    ),
                ],
                selected: {_direction},
                onSelectionChanged: _changeDirection,
              ),
            ),
          ],
          for (final warning in widget.prefill.warnings) ...[
            const SizedBox(height: AppSpacing.small),
            AppInlineNotice(icon: Icons.error_outline, message: warning),
          ],
          const SizedBox(height: AppSpacing.medium),
          DropdownButtonFormField<String>(
            initialValue: _categoryId,
            isExpanded: true,
            decoration: InputDecoration(
              labelText: 'Kategori',
              helperText: widget.prefill.categoryId?.helperText,
            ),
            validator: (value) => value == null ? 'Kategori seçin.' : null,
            onChanged: (value) => setState(() {
              _categoryId = value;
              _scopeMissing = false;
            }),
            items: [
              for (final item in options.categories)
                DropdownMenuItem(value: item.id, child: Text(item.name)),
            ],
          ),
          const SizedBox(height: AppSpacing.medium),
          DropdownButtonFormField<String?>(
            initialValue: _counterpartyId,
            isExpanded: true,
            decoration: const InputDecoration(
              labelText: 'Karşı taraf (isteğe bağlı)',
              helperText: 'Eşleşen kayıt varsa seçilidir; değiştirebilirsiniz.',
            ),
            onChanged: (value) => setState(() => _counterpartyId = value),
            items: [
              const DropdownMenuItem(
                value: null,
                child: Text('Karşı taraf yok'),
              ),
              for (final item in options.counterparties)
                DropdownMenuItem(value: item.id, child: Text(item.name)),
            ],
          ),
          const SizedBox(height: AppSpacing.medium),
          if (_showScope) ...[
            AppScopeField(
              value: _resolvedScope,
              helperText: _scopeHelperText,
              errorText: _scopeMissing ? 'Bu kayıt için kapsam seçin.' : null,
              onChanged: (value) => setState(() {
                _explicitScope = value;
                _scopeMissing = false;
              }),
            ),
            const SizedBox(height: AppSpacing.medium),
          ],
          TextFormField(
            controller: _amount,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: InputDecoration(
              labelText: 'Tutar',
              suffixText: 'TRY',
              helperText: widget.prefill.amount?.helperText,
            ),
            validator: _validateAmount,
          ),
          const SizedBox(height: AppSpacing.medium),
          AppDateField(
            label: 'Belge tarihi',
            value: AppDateField.format(_issueDate),
            helperText: widget.prefill.issueDate?.helperText,
            lastDate: widget.today ?? DateTime.now(),
            onChanged: (value) => setState(() {
              _issueDate = AppDateField.parse(value)!;
              if (_dueDate.isBefore(_issueDate)) _dueDate = _issueDate;
            }),
          ),
          const SizedBox(height: AppSpacing.medium),
          AppDateField(
            label: 'Son ödeme tarihi',
            value: AppDateField.format(_dueDate),
            helperText: widget.prefill.dueDate?.helperText,
            firstDate: _issueDate,
            onChanged: (value) => setState(() {
              _dueDate = AppDateField.parse(value)!;
            }),
          ),
          const SizedBox(height: AppSpacing.medium),
          TextFormField(
            controller: _description,
            maxLength: 500,
            buildCounter:
                (
                  _, {
                  required currentLength,
                  required isFocused,
                  required maxLength,
                }) => null,
            decoration: InputDecoration(
              labelText: 'Ad (isteğe bağlı)',
              helperText: widget.prefill.description?.helperText,
            ),
          ),
          ..._buildTaxSection(),
          if (widget.controller.errorMessage case final error?)
            Padding(
              padding: const EdgeInsets.only(bottom: AppSpacing.medium),
              child: Semantics(
                liveRegion: true,
                child: Text(
                  error,
                  style: TextStyle(color: Theme.of(context).colorScheme.error),
                ),
              ),
            ),
          AppSubmitButton(
            label: 'Yükümlülüğü kaydet',
            isBusy: widget.controller.isSubmitting,
            onSubmit: _submit,
          ),
        ],
      ),
    );
  }

  /// Yönü bilerek açılan form onu sormaz; elle açılan sorar.
  bool get _canChooseDirection => widget.prefill.direction == null;

  String get _title =>
      _canChooseDirection ? 'Yükümlülük ekle' : 'Ödenmemiş faturayı kaydet';

  String get _noticeMessage {
    final recognized = _direction == ObligationDirection.payable
        ? 'Bu kayıt gideri belge tarihinde tanır; hesabınızdan henüz para '
              'çıkarmaz.'
        : 'Bu kayıt geliri belge tarihinde tanır; hesabınıza henüz para '
              'girmez.';
    return _canChooseDirection
        ? '$recognized Para, kaydı kapattığınız gün hareket eder.'
        : 'Alanlar faturadan okunan önerilerdir. $recognized';
  }

  /// Yön değişince kategori listesi yeniden okunur ve seçim düşer: gelir
  /// kategorisi ödenecek bir faturaya, gider kategorisi bir alacağa
  /// yazılamaz — sunucu da aynı sebeple reddeder.
  void _changeDirection(Set<ObligationDirection> selection) {
    final chosen = selection.single;
    if (chosen == _direction) return;
    setState(() {
      _direction = chosen;
      _categoryId = null;
      _scopeMissing = false;
    });
    widget.controller.load(direction: chosen);
  }

  bool get _showScope => widget.scopeController?.isVisible ?? false;

  TransactionScope? get _categoryScope {
    final options = widget.controller.options;
    if (options == null || _categoryId == null) return null;
    for (final category in options.categories) {
      if (category.id == _categoryId) return category.defaultScope;
    }
    return null;
  }

  TransactionScope? get _resolvedScope =>
      previewResolvedScope(explicit: _explicitScope, category: _categoryScope);

  String get _scopeHelperText => _explicitScope == null
      ? 'Kategorinin varsayılanından önerildi; değiştirebilirsiniz.'
      : 'Sizin seçiminiz; kategori varsayılanını ezer.';

  /// Vergi bölümü: KDV oranı ve tutarı.
  ///
  /// Yükümlülük gideri (ya da geliri) **tanıyan** bir kayıttır ve ADR 0016'ya
  /// göre KDV taşır. Bölüm yalnız kapsam boyutunu gören kullanıcıda çizilir:
  /// "işletmem yok" diyen kişinin formunda KDV bir soru değildir.
  ///
  /// İndirilebilirlik burada **sorulmaz**. Yükümlülüğün kategorisi zaten bir
  /// varsayılan taşıyor ve sunucu onu uyguluyor; ikinci bir anahtar koymak,
  /// aynı cevabı iki yerden vermek olurdu. Gider formundaki anahtar oradaki
  /// kaydın kategorisi seçildikten sonra anlam kazanıyor — burada kayıt
  /// kapanana kadar para hareket etmiyor.
  List<Widget> _buildTaxSection() {
    if (!_showScope) return const [];
    return [
      Card(
        margin: EdgeInsets.zero,
        clipBehavior: Clip.antiAlias,
        child: ExpansionTile(
          initiallyExpanded: _taxExpanded,
          onExpansionChanged: (value) => setState(() => _taxExpanded = value),
          tilePadding: const EdgeInsets.symmetric(
            horizontal: AppSpacing.medium,
          ),
          childrenPadding: const EdgeInsets.fromLTRB(
            AppSpacing.medium,
            0,
            AppSpacing.medium,
            AppSpacing.medium,
          ),
          title: const Text('Vergi bilgisi (isteğe bağlı)'),
          subtitle: Text(_taxSummary),
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: TextFormField(
                    controller: _vatRate,
                    keyboardType: const TextInputType.numberWithOptions(
                      decimal: true,
                    ),
                    decoration: InputDecoration(
                      labelText: 'KDV oranı',
                      suffixText: '%',
                      helperText: widget.prefill.vat == null
                          ? null
                          : 'Fişten okundu — kontrol edin',
                    ),
                    validator: _validateVatRate,
                    onChanged: (_) => setState(() {}),
                  ),
                ),
                const SizedBox(width: AppSpacing.medium),
                Expanded(
                  child: TextFormField(
                    controller: _vatAmount,
                    keyboardType: const TextInputType.numberWithOptions(
                      decimal: true,
                    ),
                    decoration: InputDecoration(
                      labelText: 'KDV tutarı',
                      suffixText: 'TRY',
                      helperText: widget.prefill.vat == null
                          ? null
                          : 'Fişten okundu — kontrol edin',
                    ),
                    validator: _validateVatAmount,
                    onChanged: (_) => setState(() {}),
                  ),
                ),
              ],
            ),
            const SizedBox(height: AppSpacing.small),
            const AppInlineNotice(
              icon: Icons.info_outline,
              message:
                  'Belgede ne yazıyorsa onu girin. Uygulama KDV hesaplamaz; '
                  'boş bırakmak da geçerli bir cevaptır.',
            ),
          ],
        ),
      ),
      const SizedBox(height: AppSpacing.medium),
    ];
  }

  /// Kapalı bölümün altındaki tek satır: içinde ne olduğunu söyler.
  String get _taxSummary {
    final vat = _vatFields;
    if (vat == null) return 'KDV girilmedi';
    final rate = vat.ratePercentLabel;
    final amount = vat.amount;
    if (rate != null && amount != null) {
      return 'KDV $rate • ${MoneyText.format(amount, 'TRY')}';
    }
    if (rate != null) return 'KDV $rate';
    return 'KDV ${MoneyText.format(amount!, 'TRY')}';
  }

  /// Forma yazılan KDV; ikisi de boşsa kayıt KDV taşımaz.
  VatFields? get _vatFields {
    final rate = VatFields.rateFromPercentInput(_vatRate.text);
    final amount = VatFields.amountFromInput(_vatAmount.text);
    if (rate == null && amount == null) return null;
    return VatFields(rate: rate, amount: amount);
  }

  String? _validateVatRate(String? value) {
    if ((value ?? '').trim().isEmpty) return null;
    return VatFields.rateFromPercentInput(value) == null
        ? '0 ile 100 arasında bir oran girin.'
        : null;
  }

  String? _validateVatAmount(String? value) {
    if ((value ?? '').trim().isEmpty) return null;
    final normalized = MoneyText.normalizeInput(value ?? '');
    if (normalized == null) return 'Geçerli bir tutar girin.';
    final amount = double.tryParse(normalized);
    if (amount == null) return 'Geçerli bir tutar girin.';
    final total = double.tryParse(MoneyText.normalizeInput(_amount.text) ?? '');
    // Sınır, hesaplama değil: brütün içindeki KDV brütten büyük olamaz.
    if (total != null && amount > total) {
      return 'KDV, kaydın tutarından büyük olamaz.';
    }
    return null;
  }

  String? _validateAmount(String? value) {
    final normalized = MoneyText.normalizeInput(value ?? '');
    if (normalized == null) return 'Geçerli bir tutar girin.';
    final parsed = double.tryParse(normalized);
    if (parsed == null || parsed <= 0) return 'Tutar sıfırdan büyük olmalıdır.';
    return null;
  }

  Future<void> _submit() async {
    if (!(_formKey.currentState?.validate() ?? false)) return;
    if (_showScope && _resolvedScope == null) {
      setState(() => _scopeMissing = true);
      return;
    }
    final saved = await widget.controller.create(
      direction: _direction,
      amount: MoneyText.normalizeInput(_amount.text)!,
      categoryId: _categoryId!,
      issueDate: AppDateField.format(_issueDate),
      dueDate: AppDateField.format(_dueDate),
      scope: _showScope ? _resolvedScope : null,
      counterpartyId: _counterpartyId,
      description: _description.text.trim().isEmpty
          ? null
          : _description.text.trim(),
      vat: _vatFields,
    );
    if (!mounted || !saved) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(const SnackBar(content: Text('Yükümlülük kaydedildi.')));
    Navigator.of(context).pop(true);
  }
}

class _NeverListenable implements Listenable {
  const _NeverListenable();

  @override
  void addListener(VoidCallback listener) {}

  @override
  void removeListener(VoidCallback listener) {}
}
