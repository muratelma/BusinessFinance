import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/models/transaction_scope.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../data/category_models.dart';

typedef SaveCategory =
    Future<bool> Function({
      BudgetCategory? category,
      required String name,
      required String type,
      required bool isActive,
      TransactionScope? defaultScope,
      bool isTax,
    });

class CategoryFormPage extends StatefulWidget {
  const CategoryFormPage({
    required this.onSave,
    super.key,
    this.category,
    this.readError,
  });

  final BudgetCategory? category;
  final SaveCategory onSave;

  /// [onSave] `false` döndüğünde gösterilecek cümle. Kaydeden katman sunucunun
  /// reddini kendi içinde tutuyor; form onu okuyamazsa `Kaydet` hiçbir şey
  /// yapmamış gibi görünür.
  final String? Function()? readError;

  @override
  State<CategoryFormPage> createState() => _CategoryFormPageState();
}

class _CategoryFormPageState extends State<CategoryFormPage> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _nameController;
  late String _type;
  late bool _isActive;
  TransactionScope? _defaultScope;
  late bool _isTax;
  bool _submitting = false;
  String? _error;

  bool get _editing => widget.category != null;

  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController(text: widget.category?.name ?? '');
    _type = widget.category?.type ?? 'expense';
    _isActive = widget.category?.isActive ?? true;
    _defaultScope = widget.category?.defaultScope;
    _isTax = widget.category?.isTax ?? false;
  }

  @override
  void dispose() {
    _nameController.dispose();
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
        category: widget.category,
        name: _nameController.text,
        type: _type,
        isActive: _isActive,
        // Kapsamı görmeyen kullanıcıda alan hiç çizilmiyor ama değer yine de
        // gidiyor: sunucudaki `PUT` yetkili ve göndermemek "kaldır" demek.
        defaultScope: _defaultScope,
        // İşaret yalnız gider kategorisinde anlamlı; sunucu gelirde reddeder.
        isTax: _type == 'expense' && _isTax,
      );
      if (!mounted) return;
      if (saved) {
        Navigator.of(context).pop(true);
      } else {
        setState(
          () => _error =
              widget.readError?.call() ??
              'Kategori kaydedilemedi. Tekrar deneyin.',
        );
      }
    } on ApiException catch (error) {
      if (mounted) setState(() => _error = error.message);
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(_editing ? 'Kategoriyi düzenle' : 'Kategori ekle'),
      ),
      body: SafeArea(
        child: Form(
          key: _formKey,
          child: ListView(
            padding: const EdgeInsets.all(AppSpacing.medium),
            children: [
              TextFormField(
                controller: _nameController,
                hintLocales: const [Locale('tr', 'TR')],
                decoration: const InputDecoration(
                  labelText: 'Kategori adı',
                  border: OutlineInputBorder(),
                ),
                validator: (value) => value == null || value.trim().isEmpty
                    ? 'Kategori adını girin.'
                    : null,
              ),
              const SizedBox(height: AppSpacing.medium),
              DropdownButtonFormField<String>(
                initialValue: _type,
                decoration: const InputDecoration(
                  labelText: 'Kategori türü',
                  border: OutlineInputBorder(),
                ),
                items: const [
                  DropdownMenuItem(value: 'expense', child: Text('Gider')),
                  DropdownMenuItem(value: 'income', child: Text('Gelir')),
                ],
                onChanged: _editing
                    ? null
                    : (value) => setState(() => _type = value!),
              ),
              if (context.watch<ScopeController?>()?.isVisible ?? false) ...[
                const SizedBox(height: AppSpacing.medium),
                AppScopeDefaultField(
                  label: 'Kapsam',
                  emptyLabel: 'İkisi de',
                  value: _defaultScope,
                  onChanged: (value) => setState(() => _defaultScope = value),
                  helperText: _defaultScope == null
                      ? 'Kayıt girerken sorulur.'
                      : 'Bu kategorideki kayıtlar '
                            '${_defaultScope!.label} yazılır.',
                ),
              ],
              // Kapsam bölümüyle aynı hizada, onun altında durur.
              if (_editing) ...[
                const SizedBox(height: AppSpacing.medium),
                Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.small,
                  ),
                  child: Text(
                    'Kategori türü geçmiş işlemleri korumak için '
                    'değiştirilemez.',
                    style: Theme.of(context).textTheme.bodySmall,
                  ),
                ),
              ],
              if (_type == 'expense') ...[
                const SizedBox(height: AppSpacing.medium),
                _TaxSwitch(
                  value: _isTax,
                  onChanged: (value) => setState(() => _isTax = value),
                ),
              ],
              if (_editing) ...[
                const SizedBox(height: AppSpacing.small),
                SwitchListTile(
                  contentPadding: EdgeInsets.zero,
                  title: const Text('Kategori aktif'),
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
            ],
          ),
        ),
      ),
    );
  }
}

/// Kategorinin "Vergi" işareti: kart içinde başlık, tek cümle ve anahtar.
class _TaxSwitch extends StatelessWidget {
  const _TaxSwitch({required this.value, required this.onChanged});

  final bool value;
  final ValueChanged<bool> onChanged;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return MergeSemantics(
      child: Container(
        padding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.medium,
          vertical: AppSpacing.small + AppSpacing.xSmall,
        ),
        decoration: BoxDecoration(
          color: surfaces.card,
          border: Border.all(color: surfaces.border),
          borderRadius: BorderRadius.circular(AppRadius.card),
        ),
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text('Vergi', style: theme.textTheme.titleSmall),
                  const SizedBox(height: AppSpacing.xxSmall),
                  Text(
                    "Bu kategorideki giderler Vergi takibi › Ödenenler'de "
                    'görünür.',
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: surfaces.inkMuted,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(width: AppSpacing.medium),
            Switch(value: value, onChanged: onChanged),
          ],
        ),
      ),
    );
  }
}
