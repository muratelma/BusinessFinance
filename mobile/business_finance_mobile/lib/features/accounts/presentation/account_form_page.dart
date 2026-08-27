import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../data/account_models.dart';

typedef SaveAccount =
    Future<bool> Function({
      Account? account,
      required String name,
      required String type,
      required String openingBalance,
      required bool isActive,
      TransactionScope? defaultScope,
    });

typedef DeleteAccount = Future<String?> Function(Account account);

class AccountFormPage extends StatefulWidget {
  const AccountFormPage({
    required this.onSave,
    super.key,
    this.account,
    this.onDelete,
  }) : assert(account == null || onDelete != null);

  final Account? account;
  final SaveAccount onSave;
  final DeleteAccount? onDelete;

  @override
  State<AccountFormPage> createState() => _AccountFormPageState();
}

class _AccountFormPageState extends State<AccountFormPage> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _nameController;
  late final TextEditingController _balanceController;
  late String _type;
  late bool _isActive;
  TransactionScope? _defaultScope;
  bool _submitting = false;
  String? _error;

  bool get _editing => widget.account != null;

  @override
  void initState() {
    super.initState();
    final account = widget.account;
    _nameController = TextEditingController(text: account?.name ?? '');
    _balanceController = TextEditingController(
      text: account?.openingBalance ?? '0',
    );
    _type = account?.type ?? 'cash';
    _isActive = account?.isActive ?? true;
    _defaultScope = account?.defaultScope;
  }

  @override
  void dispose() {
    _nameController.dispose();
    _balanceController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (_submitting || !_formKey.currentState!.validate()) return;
    setState(() {
      _submitting = true;
      _error = null;
    });
    try {
      final saved = await widget.onSave(
        account: widget.account,
        name: _nameController.text,
        type: _type,
        openingBalance: _balanceController.text.replaceAll(',', '.'),
        isActive: _isActive,
        // Kapsamı görmeyen kullanıcıda alan hiç çizilmiyor ama değer yine de
        // gidiyor: sunucudaki `PUT` yetkili ve göndermemek "kaldır" demek.
        defaultScope: _defaultScope,
      );
      if (saved && mounted) Navigator.of(context).pop(true);
    } on ApiException catch (error) {
      if (mounted) setState(() => _error = error.message);
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  Future<void> _delete() async {
    if (_submitting) return;
    final account = widget.account;
    if (account == null) return;

    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.delete_outline,
      destructive: true,
      title: 'Hesap kalıcı olarak silinsin mi?',
      message:
          'Yalnız finansal geçmişte hiç kullanılmamış hesaplar silinebilir. '
          'Bu işlem geri alınamaz.',
      confirmLabel: 'Kalıcı olarak sil',
    );
    if (!confirmed || !mounted) return;

    setState(() {
      _submitting = true;
      _error = null;
    });
    try {
      final error = await widget.onDelete!(account);
      if (!mounted) return;
      if (error == null) {
        Navigator.of(context).pop(true);
      } else {
        setState(() => _error = error);
      }
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(_editing ? 'Hesabı düzenle' : 'Hesap ekle')),
      body: SafeArea(
        child: Form(
          key: _formKey,
          child: ListView(
            padding: const EdgeInsets.all(AppSpacing.medium),
            children: [
              TextFormField(
                controller: _nameController,
                hintLocales: const [Locale('tr', 'TR')],
                textInputAction: TextInputAction.next,
                decoration: const InputDecoration(
                  labelText: 'Hesap adı',
                  border: OutlineInputBorder(),
                ),
                validator: (value) => value == null || value.trim().isEmpty
                    ? 'Hesap adını girin.'
                    : null,
              ),
              const SizedBox(height: AppSpacing.medium),
              DropdownButtonFormField<String>(
                initialValue: _type,
                decoration: const InputDecoration(
                  labelText: 'Hesap türü',
                  border: OutlineInputBorder(),
                ),
                items: const [
                  DropdownMenuItem(value: 'cash', child: Text('Nakit')),
                  DropdownMenuItem(value: 'bank', child: Text('Banka')),
                ],
                onChanged: _editing ? null : (value) => _type = value!,
              ),
              const SizedBox(height: AppSpacing.medium),
              TextFormField(
                controller: _balanceController,
                enabled: !_editing,
                keyboardType: const TextInputType.numberWithOptions(
                  decimal: true,
                ),
                decoration: const InputDecoration(
                  labelText: 'Açılış bakiyesi',
                  suffixText: 'TRY',
                  border: OutlineInputBorder(),
                ),
                validator: _validateMoney,
              ),
              if (context.watch<ScopeController?>()?.isVisible ?? false) ...[
                const SizedBox(height: AppSpacing.medium),
                AppScopeDefaultField(
                  value: _defaultScope,
                  onChanged: (value) => setState(() => _defaultScope = value),
                  helperText:
                      'Bu hesaptan yazılan kayıtlar, siz başka bir şey '
                      'seçmedikçe bu tarafa yazılır. Boş bırakırsanız kararı '
                      'kategori verir.',
                ),
              ],
              if (_editing) ...[
                const SizedBox(height: AppSpacing.small),
                Text(
                  'Tür ve açılış bakiyesi geçmiş hesaplamaları korumak için değiştirilemez.',
                  style: Theme.of(context).textTheme.bodySmall,
                ),
                SwitchListTile(
                  contentPadding: EdgeInsets.zero,
                  title: const Text('Hesap aktif'),
                  value: _isActive,
                  onChanged: (value) => setState(() => _isActive = value),
                ),
              ],
              if (_error != null) ...[
                const SizedBox(height: AppSpacing.medium),
                Semantics(
                  liveRegion: true,
                  child: Text(
                    _error!,
                    style: TextStyle(
                      color: Theme.of(context).colorScheme.error,
                    ),
                  ),
                ),
              ],
              const SizedBox(height: AppSpacing.large),
              FilledButton.icon(
                onPressed: _submitting ? null : _submit,
                icon: _submitting
                    ? const SizedBox.square(
                        dimension: 18,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.save_outlined),
                label: Text(_submitting ? 'Kaydediliyor' : 'Kaydet'),
              ),
              if (_editing) ...[
                const SizedBox(height: AppSpacing.medium),
                OutlinedButton.icon(
                  onPressed: _submitting ? null : _delete,
                  icon: const Icon(Icons.delete_outline),
                  label: const Text('Hesabı sil'),
                  style: OutlinedButton.styleFrom(
                    foregroundColor: Theme.of(context).colorScheme.error,
                  ),
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }

  String? _validateMoney(String? value) {
    final text = value?.trim() ?? '';
    if (!RegExp(r'^\d+(?:[.,]\d{1,4})?$').hasMatch(text)) {
      return 'En fazla dört ondalık basamaklı bir tutar girin.';
    }
    return null;
  }
}
