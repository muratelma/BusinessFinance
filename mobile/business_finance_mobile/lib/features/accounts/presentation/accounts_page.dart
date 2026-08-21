import 'package:flutter/material.dart';

import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../data/account_models.dart';
import 'account_form_page.dart';
import 'accounts_view_model.dart';

class AccountsPage extends StatefulWidget {
  const AccountsPage({
    required this.viewModel,
    this.ownsViewModel = false,
    this.embedded = false,
    super.key,
  });

  final AccountsViewModel viewModel;
  final bool ownsViewModel;

  /// Bir sekmenin içinde mi çiziliyor.
  ///
  /// Gömülüyken kendi `Scaffold`'unu, başlık çubuğunu ve kayan butonunu
  /// çizmez; onları barındıran sayfa sağlar. İç içe iki `Scaffold` aynı
  /// başlığı iki kez gösterir ve iki kayan buton üst üste biner.
  final bool embedded;

  @override
  State<AccountsPage> createState() => AccountsPageState();
}

/// Barındıran sayfanın hesap ekleme formunu açabilmesi için public.
class AccountsPageState extends State<AccountsPage> {
  @override
  void initState() {
    super.initState();
    widget.viewModel.addListener(_refresh);
    if (widget.viewModel.status == AccountsViewStatus.initial) {
      widget.viewModel.load();
    }
  }

  @override
  void didUpdateWidget(covariant AccountsPage oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.viewModel != widget.viewModel) {
      oldWidget.viewModel.removeListener(_refresh);
      widget.viewModel.addListener(_refresh);
    }
  }

  @override
  void dispose() {
    widget.viewModel.removeListener(_refresh);
    if (widget.ownsViewModel) widget.viewModel.dispose();
    super.dispose();
  }

  void _refresh() {
    if (mounted) setState(() {});
  }

  Future<void> _openForm([Account? account]) async {
    final saved = await Navigator.of(context).push<bool>(
      MaterialPageRoute(
        builder: (_) => AccountFormPage(
          account: account,
          onSave: widget.viewModel.save,
          onDelete: account == null ? null : widget.viewModel.delete,
        ),
      ),
    );
    if (saved == true && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(widget.viewModel.message ?? 'Hesap kaydedildi.'),
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final body = _body();
    if (widget.embedded) return body;
    return Scaffold(
      appBar: AppBar(title: const Text('Hesaplar')),
      floatingActionButton: FloatingActionButton(
        heroTag: 'accounts-add-account',
        tooltip: 'Hesap ekle',
        onPressed: widget.viewModel.isSubmitting ? null : _openForm,
        child: const Icon(Icons.add),
      ),
      body: body,
    );
  }

  /// Sayfayı barındıran ekranın çağırabilmesi için ayrı: gömülü modda hesap
  /// ekleme eylemi de host tarafından sunulur.
  void openAccountForm() => _openForm();

  Widget _body() {
    return switch (widget.viewModel.status) {
      AccountsViewStatus.initial || AccountsViewStatus.loading =>
        const AppLoadingView(message: 'Hesaplar yükleniyor'),
      AccountsViewStatus.empty => const AppEmptyView(
        title: 'Henüz hesap yok',
        message: 'İlk nakit veya banka hesabınızı ekleyin.',
        icon: Icons.account_balance_wallet_outlined,
      ),
      AccountsViewStatus.error => AppErrorView(
        message: widget.viewModel.message ?? 'Hesaplar yüklenemedi.',
        onRetry: widget.viewModel.load,
      ),
      AccountsViewStatus.unauthorized => const AppUnauthorizedView(),
      AccountsViewStatus.ready => RefreshIndicator(
        onRefresh: widget.viewModel.load,
        child: ListView.separated(
          padding: const EdgeInsets.fromLTRB(
            AppSpacing.medium,
            AppSpacing.small,
            AppSpacing.medium,
            96,
          ),
          itemCount: widget.viewModel.accounts.length,
          separatorBuilder: (_, _) => const SizedBox(height: AppSpacing.small),
          itemBuilder: (context, index) {
            final account = widget.viewModel.accounts[index];
            return AppCard(
              padding: EdgeInsets.zero,
              child: AppListRow(
                icon: account.type == 'bank'
                    ? Icons.account_balance_outlined
                    : Icons.payments_outlined,
                title: account.name,
                subtitle: account.type == 'bank' ? 'Banka' : 'Nakit',
                // Pasif hesap yalnız solukluğuyla anlatılmaz: solukluk tek başına
                // ekran okuyucuya hiçbir şey söylemez ve düşük kontrastta
                // fark edilmez.
                dimmed: !account.isActive,
                badge: account.isActive
                    ? null
                    : const AppStatusChip(
                        label: 'Pasif',
                        icon: Icons.visibility_off_outlined,
                        tone: AppStatusTone.cancelled,
                      ),
                trailing: AppMoneyText(
                  amount: account.balance,
                  currency: account.currency,
                  semanticsSuffix: 'bakiye',
                ),
                onTap: () => _openForm(account),
              ),
            );
          },
        ),
      ),
    };
  }
}
