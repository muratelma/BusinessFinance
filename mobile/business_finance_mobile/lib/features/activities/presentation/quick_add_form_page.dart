import 'package:flutter/material.dart';

import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_menu_group_label.dart';
import '../../../core/widgets/app_state_views.dart';
import '../data/receipt_fee_writer.dart';
import 'quick_add_controller.dart';
import 'quick_add_models.dart';

/// Expense and income share one page because they share every field except the
/// source. Expense can be paid from an account or a card; income can only land
/// in an account.
class QuickAddFormPage extends StatefulWidget {
  const QuickAddFormPage({
    required this.controller,
    required this.isExpense,
    super.key,
    this.today,
    this.prefill,
  });

  final QuickAddController controller;
  final bool isExpense;
  final DateTime? today;

  /// Alanların önü dolu açılmasını sağlayan öneriler.
  ///
  /// Fiş akışı buradan giriyor. Ayrı bir onay formu yazılmadı: iki form zamanla
  /// ayrışır ve aynı finansal kural iki yerde durur.
  final QuickAddPrefill? prefill;

  @override
  State<QuickAddFormPage> createState() => _QuickAddFormPageState();
}

class _QuickAddFormPageState extends State<QuickAddFormPage> {
  final _formKey = GlobalKey<FormState>();
  final _amountController = TextEditingController();
  final _descriptionController = TextEditingController();

  PaymentSource? _source;
  String? _accountId;
  String? _categoryId;
  late bool _keepAttachment = widget.prefill?.keepAttachmentByDefault ?? true;
  late DateTime _date = widget.today ?? DateTime.now();

  @override
  void initState() {
    super.initState();
    _applyPrefill();
    if (widget.isExpense) {
      widget.controller.loadExpenseOptions();
    } else {
      widget.controller.loadIncomeOptions();
    }
  }

  /// Önerileri alanlara yazar.
  ///
  /// Ödeme kaynağı bilerek dışarıda: fiş hangi hesaptan ödendiğini bilmez,
  /// yalnız fişte `NAKİT`/`KREDİ KARTI` yazdığını bilir. Kaynağı seçmek,
  /// kullanıcının onaylaması gereken tek şeyi onun yerine seçmek olurdu.
  void _applyPrefill() {
    final prefill = widget.prefill;
    if (prefill == null) return;
    final amount = prefill.amount;
    if (amount != null) {
      _amountController.text = MoneyText.editable(amount.value);
    }
    final description = prefill.description;
    if (description != null) {
      _descriptionController.text = description.value;
    }
    final date = AppDateField.parse(prefill.date?.value ?? '');
    if (date != null) _date = date;
    _categoryId = prefill.categoryId?.value;
  }

  /// Ad ile önerilen kategoriyi kullanıcının kendi listesinde arar.
  ///
  /// Seçenekler yüklenmeden yapılamaz, bu yüzden burada: kimlik zaten geldiyse
  /// dokunmaz, ad bulunamazsa alan boş kalır — uydurulmuş bir kategori yazmak,
  /// kullanıcının görmediği bir kovaya harcama yazdırmak olurdu.
  void _resolveCategoryNameHint(List<QuickAddChoice> categories) {
    if (_categoryId != null) return;
    final hint = widget.prefill?.categoryNameHint;
    if (hint == null) return;
    for (final category in categories) {
      if (category.name.toLowerCase() == hint.toLowerCase()) {
        _categoryId = category.id;
        return;
      }
    }
  }

  @override
  void dispose() {
    _amountController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(widget.isExpense ? 'Gider ekle' : 'Gelir ekle'),
      ),
      body: AnimatedBuilder(
        animation: widget.controller,
        builder: (context, _) => _buildBody(context),
      ),
    );
  }

  Widget _buildBody(BuildContext context) {
    final controller = widget.controller;
    if (controller.unauthorized) return const AppUnauthorizedView();
    if (controller.isLoading) return const AppLoadingView();

    final ready = widget.isExpense
        ? controller.expenseOptions != null
        : controller.incomeOptions != null;
    if (!ready) {
      return AppErrorView(
        message: controller.errorMessage ?? 'Seçenekler yüklenemedi.',
        onRetry: widget.isExpense
            ? controller.loadExpenseOptions
            : controller.loadIncomeOptions,
      );
    }

    return Form(
      key: _formKey,
      child: ListView(
        padding: const EdgeInsets.all(AppSpacing.medium),
        children: [
          ..._buildPrefillNotices(context),
          if (widget.isExpense) _buildSourcePicker() else _buildAccountPicker(),
          const SizedBox(height: AppSpacing.medium),
          _buildCategoryPicker(),
          const SizedBox(height: AppSpacing.medium),
          TextFormField(
            controller: _amountController,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: InputDecoration(
              labelText: 'Tutar',
              suffixText: 'TRY',
              helperText: widget.prefill?.amount?.helperText,
              suffixIcon: _suggestionIcon(widget.prefill?.amount),
            ),
            validator: _validateAmount,
          ),
          const SizedBox(height: AppSpacing.medium),
          AppDateField(
            label: 'Tarih',
            helperText: widget.prefill?.date?.helperText,
            value: _formattedDate,
            onChanged: (value) =>
                setState(() => _date = AppDateField.parse(value)!),
          ),
          const SizedBox(height: AppSpacing.medium),
          TextFormField(
            controller: _descriptionController,
            maxLength: 500,
            // Sayaç gizli: 500 karakterlik bir sayaç kullanıcıyı uzun cümle
            // yazmaya davet ediyordu, oysa bu alan kaydın **adı** oluyor.
            // Sınır sunucu sözleşmesi gereği duruyor, teşviki kalkıyor.
            buildCounter:
                (
                  _, {
                  required currentLength,
                  required isFocused,
                  required maxLength,
                }) => null,
            decoration: InputDecoration(
              labelText: 'Ad (isteğe bağlı)',
              hintText: 'İstanbulkart yükleme',
              helperText:
                  widget.prefill?.description?.helperText ??
                  'Listede kaydın adı olur. Boş bırakılırsa '
                      'kategori adı kullanılır.',
              suffixIcon: _suggestionIcon(widget.prefill?.description),
            ),
          ),
          ..._buildKeepAttachment(context),
          ..._buildAutoFeeNotice(context),
          if (controller.errorMessage != null)
            Padding(
              padding: const EdgeInsets.only(bottom: AppSpacing.medium),
              child: Semantics(
                liveRegion: true,
                child: Text(
                  controller.errorMessage!,
                  style: TextStyle(color: Theme.of(context).colorScheme.error),
                ),
              ),
            ),
          FilledButton(
            // Disabled while a request is in flight, because a second tap would
            // be a second write.
            onPressed: controller.isSubmitting ? null : _submit,
            child: controller.isSubmitting
                ? const SizedBox.square(
                    dimension: 18,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Text('Kaydet'),
          ),
        ],
      ),
    );
  }

  /// Formun tepesinde duran öneri notları.
  ///
  /// Önce alanların nereden geldiğini söyleyen tek cümle, sonra sunucunun
  /// ürettiği uyarılar. Uyarı metni burada yeniden yazılmıyor: doğrulayıcı
  /// değişince iki metin ayrışırdı.
  List<Widget> _buildPrefillNotices(BuildContext context) {
    final prefill = widget.prefill;
    if (prefill == null) return const [];
    return [
      if (!prefill.isEmpty)
        const AppInlineNotice(
          icon: Icons.receipt_long_outlined,
          message:
              'Alanlar fişten okundu ve yalnızca öneridir. Kaydetmeden önce '
              'kontrol edin; hepsini değiştirebilirsiniz.',
        ),
      if (prefill.isEmpty)
        const AppInlineNotice(
          icon: Icons.edit_outlined,
          message:
              'Fişten hiçbir alan okunamadı. Bilgileri elle girebilirsiniz.',
        ),
      for (final warning in prefill.warnings)
        AppInlineNotice(icon: Icons.error_outline, message: warning),
      const SizedBox(height: AppSpacing.small),
    ];
  }

  /// **Fişi sakla** anahtarı.
  ///
  /// Yalnız saklanacak bir fotoğraf varken görünür. Kart harcamasında kapalı
  /// ve kapalı olma sebebi yazılı: sunucudaki belge uç noktası işleme bağlanır
  /// (`/transactions/{id}/attachments`), kart harcamasının böyle bir kimliği
  /// yok. Anahtarı sessizce yok saymak, kullanıcıya sakladığını sandırırdı.
  List<Widget> _buildKeepAttachment(BuildContext context) {
    final attachment = widget.prefill?.attachment;
    if (attachment == null) return const [];
    final isCard = _source?.isCard ?? false;
    return [
      SwitchListTile(
        value: _keepAttachment && !isCard,
        onChanged: isCard
            ? null
            : (value) => setState(() => _keepAttachment = value),
        contentPadding: EdgeInsets.zero,
        title: const Text('Fişi sakla'),
        subtitle: Text(
          isCard
              ? 'Kart harcamasına şimdilik belge eklenemiyor; fotoğraf '
                    'kaydedilmeyecek.'
              : 'Fotoğrafın orijinali kaydın belgesi olarak eklenir. '
                    'Kapalıysa hiçbir yere yazılmaz.',
        ),
      ),
      const SizedBox(height: AppSpacing.medium),
    ];
  }

  /// Ücretin de yazılacağını **kaydetmeden önce** söyler.
  ///
  /// Form açılmadan yazılan bir kayıt, görünmez bir kayıt olmamalı: kullanıcı
  /// karar sayfasında onayladı, burada da ne olacağını okuyor. Kaynak yazmıyor
  /// çünkü ücret her zaman ana kaydın kaynağından çıkıyor.
  List<Widget> _buildAutoFeeNotice(BuildContext context) {
    final fee = widget.prefill?.autoFee;
    if (fee == null) return const [];
    return [
      AppInlineNotice(
        icon: Icons.receipt_long_outlined,
        message:
            'Dekonttaki ${MoneyText.format(fee.amount, 'TRY')} işlem ücreti de '
            'aynı kaynaktan $receiptFeeCategoryName olarak kaydedilecek.',
      ),
      const SizedBox(height: AppSpacing.medium),
    ];
  }

  /// Önerilen alanın yanındaki işaret.
  ///
  /// Bilgi yalnız yardımcı metinde durmuyor: şüpheli alan ayrıca ikonla
  /// ayrılıyor, çünkü uzun formda yardımcı metin gözden kaçıyor.
  Widget? _suggestionIcon(QuickAddSuggestion? suggestion) {
    if (suggestion == null) return null;
    return Icon(
      suggestion.isSuspect ? Icons.help_outline : Icons.receipt_long_outlined,
      semanticLabel: suggestion.helperText,
    );
  }

  /// Accounts and cards are offered together but stay in named groups: they are
  /// separate write models and the user should see which one they are spending
  /// from.
  ///
  /// Fişteki ödeme ipucu grupların **sırasını** değiştirir, seçimi değil.
  /// Kaynak boş başlar ve zorunludur: fiş hangi karttan ödendiğini bilmez,
  /// üstelik kullanıcı fişte yazanın aksine ödemiş olabilir. Diğer grup
  /// gizlenmiyor — gizlemek, meşru bir seçimi kullanıcıdan saklamak olurdu.
  Widget _buildSourcePicker() {
    final options = widget.controller.expenseOptions!;
    // Yalnız fişte "kredi kartı" YAZIYORSA kartlar öne alınır. Türü yazmayan
    // kart ödemesinde sıra değişmez: o fiş banka kartı da olabilir ve kartları
    // öne almak, kullanıcıyı okunmamış bir bilgiye doğru itmek olurdu.
    final cardsFirst =
        widget.prefill?.sourceHint == PaymentSourceHint.creditCard;
    return DropdownButtonFormField<PaymentSource>(
      initialValue: _source,
      isExpanded: true,
      decoration: InputDecoration(
        labelText: 'Ödeme kaynağı',
        helperText: _sourceHintText,
      ),
      validator: (value) => value == null ? 'Ödeme kaynağı seçin.' : null,
      onChanged: (value) => setState(() => _source = value),
      items: cardsFirst
          ? [..._cardItems(options), ..._accountItems(options)]
          : [..._accountItems(options), ..._cardItems(options)],
    );
  }

  String? get _sourceHintText => switch (widget.prefill?.sourceHint) {
    PaymentSourceHint.creditCard =>
      'Fişte kredi kartı yazıyor. Hangi kart olduğunu siz seçin.',
    PaymentSourceHint.debitCard =>
      'Fişte banka kartı yazıyor. Hangi hesaptan çıktığını siz seçin.',
    PaymentSourceHint.cash =>
      'Fişte nakit yazıyor. Hangi hesaptan çıktığını siz seçin.',
    PaymentSourceHint.card =>
      'Fişte kartla ödendiği yazıyor, türü yazmıyor. Banka kartıysa hesabı, '
          'kredi kartıysa kartı seçin.',
    null => null,
  };

  List<DropdownMenuItem<PaymentSource>> _accountItems(
    ExpenseFormOptions options,
  ) => [
    if (options.accounts.isNotEmpty)
      const DropdownMenuItem<PaymentSource>(
        enabled: false,
        child: AppMenuGroupLabel('Hesaplar'),
      ),
    for (final account in options.accounts)
      DropdownMenuItem(value: account, child: Text(account.name)),
  ];

  List<DropdownMenuItem<PaymentSource>> _cardItems(
    ExpenseFormOptions options,
  ) => [
    if (options.cards.isNotEmpty)
      const DropdownMenuItem<PaymentSource>(
        enabled: false,
        child: AppMenuGroupLabel('Kredi kartları'),
      ),
    for (final card in options.cards)
      DropdownMenuItem(
        value: card,
        child: Text(
          card.availableLimit == null
              ? card.name
              : '${card.name} • ${MoneyText.format(card.availableLimit!, 'TRY')} kullanılabilir',
          overflow: TextOverflow.ellipsis,
        ),
      ),
  ];

  Widget _buildAccountPicker() {
    final options = widget.controller.incomeOptions!;
    return DropdownButtonFormField<String>(
      initialValue: _accountId,
      isExpanded: true,
      decoration: const InputDecoration(labelText: 'Hesap'),
      validator: (value) => value == null ? 'Hesap seçin.' : null,
      onChanged: (value) => setState(() => _accountId = value),
      items: [
        for (final account in options.accounts)
          DropdownMenuItem(value: account.id, child: Text(account.name)),
      ],
    );
  }

  Widget _buildCategoryPicker() {
    final categories = widget.isExpense
        ? widget.controller.expenseOptions!.categories
        : widget.controller.incomeOptions!.categories;
    _resolveCategoryNameHint(categories);
    return DropdownButtonFormField<String>(
      initialValue: _categoryId,
      isExpanded: true,
      decoration: InputDecoration(
        labelText: 'Kategori',
        helperText: widget.prefill?.categoryId?.helperText,
      ),
      validator: (value) => value == null ? 'Kategori seçin.' : null,
      onChanged: (value) => setState(() => _categoryId = value),
      items: [
        for (final category in categories)
          DropdownMenuItem(value: category.id, child: Text(category.name)),
      ],
    );
  }

  String? _validateAmount(String? value) {
    final normalized = MoneyText.normalizeInput(value ?? '');
    if (normalized == null) return 'Geçerli bir tutar girin.';
    final parsed = double.tryParse(normalized);
    if (parsed == null || parsed <= 0) return 'Tutar sıfırdan büyük olmalıdır.';
    return null;
  }

  String get _formattedDate {
    final month = _date.month.toString().padLeft(2, '0');
    final day = _date.day.toString().padLeft(2, '0');
    return '${_date.year}-$month-$day';
  }

  Future<void> _submit() async {
    if (!(_formKey.currentState?.validate() ?? false)) return;
    final amount = MoneyText.normalizeInput(_amountController.text)!;
    final description = _descriptionController.text.trim();

    // Anahtar kapalıysa byte'lar buradan öteye geçmiyor: istek kurulmuyor,
    // dosya hiçbir yere yazılmıyor.
    final attachment = _keepAttachment && !(_source?.isCard ?? false)
        ? widget.prefill?.attachment
        : null;

    final saved = widget.isExpense
        ? await widget.controller.submitExpense(
            source: _source!,
            categoryId: _categoryId!,
            amount: amount,
            date: _formattedDate,
            description: description.isEmpty ? null : description,
            attachment: attachment,
          )
        : await widget.controller.submitIncome(
            accountId: _accountId!,
            categoryId: _categoryId!,
            amount: amount,
            date: _formattedDate,
            description: description.isEmpty ? null : description,
          );
    if (!saved || !mounted) return;

    // Ücret ana kayıttan **sonra**: ana kayıt yazılmadıysa harcanmış bir ücret
    // de yoktur. Başarısızlığı ana kaydı geri almaz, yalnız söylenir.
    // Ücret yalnız gider yolunda: banka masrafı bir giderdir, gelir kaydının
    // yanına ücret düşmez.
    final fee = widget.prefill?.autoFee;
    final source = _source;
    if (fee != null && source != null) {
      final result = await widget.controller.submitAutoFee(
        source: source,
        fee: fee,
        date: _formattedDate,
      );
      if (!mounted) return;
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(result.message)));
    }

    final warning = widget.controller.attachmentWarning;
    if (warning != null) {
      // Uyarı sayfa kapanmadan önce gösteriliyor; messenger uygulama
      // seviyesinde olduğu için pop'tan sonra da ekranda kalıyor.
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(warning)));
    }
    Navigator.of(context).pop(true);
  }
}
