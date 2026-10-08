import 'package:flutter/material.dart';

import '../../../core/formatters/money_input.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/data_choice.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../pos/presentation/card_collection_fields.dart';
import '../data/counterparty_models.dart';

/// Karşı tarafın kendisi: ad, not ve aktiflik.
///
/// Adres, vergi numarası ve telefon **yok**: bu aşamanın kapsamı bir kişiyi
/// tanımak ve hesabını tutmak, kimlik kartı doldurmak değil.
class CounterpartyForm extends StatefulWidget {
  const CounterpartyForm({super.key, this.existing});

  final CounterpartySummary? existing;

  @override
  State<CounterpartyForm> createState() => _CounterpartyFormState();
}

class _CounterpartyFormState extends State<CounterpartyForm> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _name;
  late final TextEditingController _note;
  late bool _isActive;

  @override
  void initState() {
    super.initState();
    _name = TextEditingController(text: widget.existing?.name ?? '');
    _note = TextEditingController(text: widget.existing?.note ?? '');
    _isActive = widget.existing?.isActive ?? true;
  }

  @override
  void dispose() {
    _name.dispose();
    _note.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: _formKey,
    child: AppFormSheet<Map<String, Object?>>(
      title: widget.existing == null
          ? 'Karşı taraf ekle'
          : 'Karşı tarafı düzenle',
      description:
          'Müşteri ve tedarikçi ayrı tutulmaz: aynı kişiden alıp aynı kişiye '
          'satabilirsiniz. Yönü her hareket kendisi taşır.',
      submitLabel: widget.existing == null ? 'Ekle' : 'Kaydet',
      onSubmit: () async {
        if (!(_formKey.currentState?.validate() ?? false)) return null;
        return {
          'name': _name.text.trim(),
          'note': _note.text.trim(),
          'isActive': _isActive,
        };
      },
      children: [
        AppFormField(
          child: TextFormField(
            controller: _name,
            decoration: const InputDecoration(labelText: 'Kişi / kurum'),
            validator: (value) => value == null || value.trim().isEmpty
                ? 'Kişi veya kurum adı zorunludur.'
                : null,
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: _note,
            maxLines: 2,
            decoration: const InputDecoration(
              labelText: 'Not (isteğe bağlı)',
              helperText: 'Kendiniz için: "Çarşı girişindeki manav" gibi.',
            ),
          ),
        ),
        if (widget.existing != null)
          AppFormField(
            child: SwitchListTile(
              contentPadding: EdgeInsets.zero,
              value: _isActive,
              title: const Text('Çalışmaya devam ediyorum'),
              // Pasifleştirme geçmişi silmez ve açık bakiyeyi kapatmayı
              // engellemez; yalnız yeni borçlandırmayı durdurur.
              subtitle: const Text(
                'Kapatırsanız yeni borç yazılamaz; kalan bakiye yine tahsil '
                'edilebilir.',
              ),
              onChanged: (value) => setState(() => _isActive = value),
            ),
          ),
      ],
    ),
  );
}

/// Veresiye satış ya da vadeli alım.
///
/// Kasa alanı **yok** ve olmamalı: borçlandırma ekonomik olayı tanır, parayı
/// taşımaz (ADR 0014). Hesap sorulsaydı kullanıcı parayı almış gibi olurdu.
class CounterpartyChargeForm extends StatefulWidget {
  const CounterpartyChargeForm({
    required this.today,
    required this.categories,
    required this.isReceivable,
    super.key,
  });

  final String today;
  final List<DataChoice> categories;

  /// Alacak mı doğuruyor (satış) yoksa borç mu (alım).
  final bool isReceivable;

  @override
  State<CounterpartyChargeForm> createState() => _CounterpartyChargeFormState();
}

class _CounterpartyChargeFormState extends State<CounterpartyChargeForm> {
  final _formKey = GlobalKey<FormState>();
  final _amount = TextEditingController();
  final _description = TextEditingController();
  late String _date;
  String? _dueDate;
  String? _categoryId;

  /// Cari kayıt her zaman işletme yazılır (ADR 0020 İ9): form taraf sormaz ve
  /// yalnız işletmeye özel ya da iki tarafa açık kategorileri listeler.
  List<DataChoice> get _options => widget.categories
      .where(
        (item) =>
            item.type == (widget.isReceivable ? 'income' : 'expense') &&
            categoryAllowsSide(item.defaultScope, TransactionScope.business),
      )
      .toList(growable: false);

  @override
  void initState() {
    super.initState();
    _date = widget.today;
    _categoryId = _options.isEmpty ? null : _options.first.id;
  }

  @override
  void dispose() {
    _amount.dispose();
    _description.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: _formKey,
    child: AppFormSheet<Map<String, Object?>>(
      title: widget.isReceivable ? 'Veresiye satış' : 'Vadeli alım',
      description: widget.isReceivable
          ? 'Satış geliri bugün yazılır; para henüz gelmedi ve kasa '
                'değişmez.'
          : 'Alım gideri bugün yazılır; ödeme henüz yapılmadı ve kasa '
                'değişmez.',
      submitLabel: 'Kaydet',
      onSubmit: () async {
        if (!(_formKey.currentState?.validate() ?? false)) return null;
        return {
          'amount': MoneyInput.wire(_amount.text),
          'categoryId': _categoryId,
          'chargeDate': _date,
          'dueDate': _dueDate,
          'description': _description.text.trim(),
        };
      },
      children: [
        AppFormField(
          child: TextFormField(
            controller: _amount,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: const InputDecoration(
              labelText: 'Tutar',
              suffixText: 'TRY',
            ),
            validator: MoneyInput.positiveError,
          ),
        ),
        AppFormField(
          child: DropdownButtonFormField<String>(
            initialValue: _categoryId,
            isExpanded: true,
            decoration: InputDecoration(
              labelText: 'Kategori',
              helperText: widget.isReceivable
                  ? 'Ne sattınız? Alacak doğuran kayıt gelir kategorisi ister.'
                  : 'Ne aldınız? Borç doğuran kayıt gider kategorisi ister.',
            ),
            items: [
              for (final option in _options)
                DropdownMenuItem(value: option.id, child: Text(option.name)),
            ],
            onChanged: (value) => setState(() => _categoryId = value),
            validator: (value) => value == null ? 'Bir kategori seçin.' : null,
          ),
        ),
        AppFormField(
          child: AppDateField(
            label: 'Tarih',
            value: _date,
            onChanged: (value) => setState(() {
              _date = value;
              final due = AppDateField.parse(_dueDate);
              final charge = AppDateField.parse(value);
              if (due != null && charge != null && due.isBefore(charge)) {
                _dueDate = null;
              }
            }),
          ),
        ),
        AppFormField(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              AppDateField(
                label: 'Vade (isteğe bağlı)',
                value: _dueDate,
                firstDate: AppDateField.parse(_date),
                helperText: 'Boş bırakırsanız hareket vadesiz izlenir.',
                onChanged: (value) => setState(() => _dueDate = value),
              ),
              if (_dueDate != null)
                Align(
                  alignment: Alignment.centerRight,
                  child: TextButton.icon(
                    onPressed: () => setState(() => _dueDate = null),
                    icon: const Icon(Icons.clear),
                    label: const Text('Vadeyi kaldır'),
                  ),
                ),
            ],
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: _description,
            decoration: const InputDecoration(
              labelText: 'Açıklama (isteğe bağlı)',
              helperText: 'Yazarsanız kayıt listede bu adla görünür.',
            ),
          ),
        ),
      ],
    ),
  );
}

/// Tahsilat ya da ödeme.
///
/// Kategori ve kapsam alanı **yok**: bu kayıt gelir/gider üretmez, yalnız
/// kasayı değiştirir. Sorulsaydı cevabı hiçbir yerde kullanılmayan bir soru
/// olurdu.
///
/// Tahsilatta [cards] verilirse `Nasıl ödendi?` rayı çıkar (ADR 0019 T5):
/// kartla alınan tahsilatta hesap yerine POS sorulur, para yola çıkar ve
/// hesaba yatışla geçer.
class CounterpartyPaymentForm extends StatefulWidget {
  const CounterpartyPaymentForm({
    required this.today,
    required this.accounts,
    required this.isReceivable,
    super.key,
    this.suggestedAmount,
    this.cards,
  });

  /// Kartla tahsilin okumaları; yoksa ray gösterilmez.
  final CardCollectionSource? cards;

  final String today;
  final List<DataChoice> accounts;

  /// Tahsilat mı (para giriyor) yoksa ödeme mi (para çıkıyor).
  final bool isReceivable;

  /// Açık bakiye: alan bununla dolu açılır ama kilitli değildir — kısmi
  /// tahsilat kuraldır, istisna değil.
  final String? suggestedAmount;

  @override
  State<CounterpartyPaymentForm> createState() =>
      _CounterpartyPaymentFormState();
}

class _CounterpartyPaymentFormState extends State<CounterpartyPaymentForm> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _amount;
  final _description = TextEditingController();
  final _cardFields = GlobalKey<CardCollectionFieldsState>();
  late String _date;
  String? _accountId;
  CollectionMethod _method = CollectionMethod.account;

  bool get _byCard => _method == CollectionMethod.card;

  @override
  void initState() {
    super.initState();
    _amount = TextEditingController(
      text: widget.suggestedAmount == null
          ? ''
          : MoneyText.editable(widget.suggestedAmount!),
    );
    _date = widget.today;
    _accountId = widget.accounts.isEmpty ? null : widget.accounts.first.id;
  }

  @override
  void dispose() {
    _amount.dispose();
    _description.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: _formKey,
    child: AppFormSheet<Map<String, Object?>>(
      title: widget.isReceivable ? 'Tahsilat' : 'Ödeme',
      description: _byCard
          ? 'Alacak bugün kapanır; satış geliri zaten yazılmıştı, ikinci kez '
                'gelir yazılmaz. Para yola çıkar, hesaba yatışla geçer.'
          : widget.isReceivable
          ? 'Kasaya para girer; satış geliri zaten yazılmıştı, ikinci kez '
                'gelir yazılmaz.'
          : 'Kasadan para çıkar; alım gideri zaten yazılmıştı, ikinci kez '
                'gider yazılmaz.',
      submitLabel: 'Kaydet',
      onSubmit: () async {
        if (!(_formKey.currentState?.validate() ?? false)) return null;
        return {
          'amount': MoneyInput.wire(_amount.text),
          'accountId': _byCard ? null : _accountId,
          'card': _byCard ? _cardFields.currentState?.request : null,
          'paymentDate': _date,
          'description': _description.text.trim(),
        };
      },
      children: [
        if (widget.isReceivable && widget.cards != null)
          AppFormField(
            child: CollectionMethodRail(
              selected: _method,
              onChanged: (value) => setState(() => _method = value),
            ),
          ),
        AppFormField(
          child: TextFormField(
            controller: _amount,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: InputDecoration(
              labelText: 'Tutar',
              suffixText: 'TRY',
              helperText: widget.suggestedAmount == null
                  ? null
                  : 'Açık bakiyeyle dolduruldu; kısmi tutar da yazabilirsiniz.',
            ),
            validator: MoneyInput.positiveError,
          ),
        ),
        AppFormField(
          child: AppDateField(
            label: 'Tarih',
            value: _date,
            lastDate: _byCard ? DateTime.now() : null,
            onChanged: (value) => setState(() => _date = value),
          ),
        ),
        if (_byCard)
          AppFormField(
            child: CardCollectionFields(
              key: _cardFields,
              source: widget.cards!,
              amount: _amount,
              collectedOn: _date,
            ),
          )
        else
          AppFormField(
            child: DropdownButtonFormField<String>(
              initialValue: _accountId,
              isExpanded: true,
              decoration: InputDecoration(
                labelText: widget.isReceivable
                    ? 'Paranın gireceği hesap'
                    : 'Ödeme hesabı',
              ),
              items: [
                for (final account in widget.accounts)
                  DropdownMenuItem(
                    value: account.id,
                    child: Text(account.name),
                  ),
              ],
              onChanged: (value) => setState(() => _accountId = value),
              validator: (value) => value == null ? 'Bir hesap seçin.' : null,
            ),
          ),
        AppFormField(
          child: TextFormField(
            controller: _description,
            decoration: const InputDecoration(
              labelText: 'Açıklama (isteğe bağlı)',
            ),
          ),
        ),
      ],
    ),
  );
}
