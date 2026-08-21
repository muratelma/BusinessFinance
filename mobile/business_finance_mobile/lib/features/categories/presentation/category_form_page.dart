import 'package:flutter/material.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/theme/app_spacing.dart';
import '../data/category_models.dart';

typedef SaveCategory =
    Future<bool> Function({
      BudgetCategory? category,
      required String name,
      required String type,
      required bool isActive,
    });

class CategoryFormPage extends StatefulWidget {
  const CategoryFormPage({required this.onSave, super.key, this.category});

  final BudgetCategory? category;
  final SaveCategory onSave;

  @override
  State<CategoryFormPage> createState() => _CategoryFormPageState();
}

class _CategoryFormPageState extends State<CategoryFormPage> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _nameController;
  late String _type;
  late bool _isActive;
  bool _submitting = false;
  String? _error;

  bool get _editing => widget.category != null;

  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController(text: widget.category?.name ?? '');
    _type = widget.category?.type ?? 'expense';
    _isActive = widget.category?.isActive ?? true;
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
      );
      if (saved && mounted) Navigator.of(context).pop(true);
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
                onChanged: _editing ? null : (value) => _type = value!,
              ),
              if (_editing) ...[
                const SizedBox(height: AppSpacing.small),
                Text(
                  'Kategori türü geçmiş işlemleri korumak için değiştirilemez.',
                  style: Theme.of(context).textTheme.bodySmall,
                ),
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
