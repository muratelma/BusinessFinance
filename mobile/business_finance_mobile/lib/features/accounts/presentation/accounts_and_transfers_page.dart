import 'package:flutter/material.dart';

import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../activities/data/receipt_fee_writer.dart';
import '../../cards/data/finance_repository.dart';
import '../../cards/presentation/finance_page.dart';
import '../../cards/presentation/transfer_prefill.dart';
import 'accounts_page.dart';
import 'accounts_view_model.dart';

/// Hesaplar ve transferler.
///
/// Transfer iki hesap arasında para taşır — tanımı hesabı içerir, hesap
/// olmadan anlamsızdır. Önceden kredi kartlarıyla aynı ekrandaydı; oradaki
/// tek ortak nokta "hesap işlemi değil" olmalarıydı, yani olumsuz bir tanım.
///
/// İki sekme iki ayrı controller kullanıyor ve bu sorun değil: hesap listesi
/// ile transfer listesi birbirinden bağımsız okunuyor, biri yenilenirken
/// diğerini beklemesi için bir sebep yok.
class AccountsAndTransfersPage extends StatefulWidget {
  const AccountsAndTransfersPage({
    required this.viewModel,
    this.financeRepository,
    this.financialDataChanges,
    this.ownsViewModel = false,
    this.initialTab = 0,
    this.transferPrefill,
    this.recordFee,
    super.key,
  });

  final AccountsViewModel viewModel;
  final FinanceRepositoryContract? financeRepository;
  final FinancialDataChanges? financialDataChanges;

  /// View model'in ömrünü bu sayfa mı yönetiyor.
  ///
  /// Sahiplik **burada** durur, sekmenin içindeki [AccountsPage]'te değil:
  /// `TabBarView` görünmeyen sekmeyi atar, o da sahibi olduğu view model'i
  /// dispose ederdi ve sekmeye geri dönüldüğünde ölü bir model'e listener
  /// eklenirdi. Cihazda tam olarak bu oldu.
  final bool ownsViewModel;

  /// Hangi sekmeyle açılacağı. Hızlı ekleme menüsünden "Transfer" seçildiğinde
  /// doğrudan Transferler sekmesi açılır; kullanıcıyı Hesaplar'a bırakıp
  /// sekmeyi kendisinin bulmasını beklemek istediği işi bir adım uzatırdı.
  final int initialTab;

  /// Dekont okunarak gelindiyse transfer formunu önerilerle açar.
  final TransferPrefill? transferPrefill;

  /// Dekontta işlem ücreti varsa transfer kaydedildikten sonra onu **kaynak
  /// hesaptan** yazar.
  final ReceiptFeeRecorder? recordFee;

  @override
  State<AccountsAndTransfersPage> createState() =>
      _AccountsAndTransfersPageState();
}

class _AccountsAndTransfersPageState extends State<AccountsAndTransfersPage>
    with SingleTickerProviderStateMixin {
  final _accountsKey = GlobalKey<AccountsPageState>();
  late final TabController _tabs;

  @override
  void initState() {
    super.initState();
    // Kendi controller'ı: kayan buton sekmeye göre değişiyor ve
    // `DefaultTabController` dinlenmeden okunduğunda buton eski sekmeye
    // takılı kalıyordu.
    _tabs = TabController(
      length: 2,
      initialIndex: widget.initialTab,
      vsync: this,
    )..addListener(_tabChanged);
  }

  void _tabChanged() {
    if (mounted) setState(() {});
  }

  @override
  void dispose() {
    _tabs
      ..removeListener(_tabChanged)
      ..dispose();
    if (widget.ownsViewModel) widget.viewModel.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(
      title: const Text('Hesaplar ve transferler'),
      bottom: TabBar(
        controller: _tabs,
        tabs: const [
          Tab(text: 'Hesaplar'),
          Tab(text: 'Transferler'),
        ],
      ),
    ),
    // Kayan buton yalnız Hesaplar sekmesinde: Transferler kendi ekleme
    // eylemini listenin başında taşıyor ve iki ekleme butonu aynı anda
    // görünseydi hangisinin neyi eklediği belirsiz kalırdı.
    floatingActionButton: _tabs.index == 0
        ? FloatingActionButton(
            heroTag: 'accounts-add-account',
            tooltip: 'Hesap ekle',
            onPressed: widget.viewModel.isSubmitting
                ? null
                : () => _accountsKey.currentState?.openAccountForm(),
            child: const Icon(Icons.add),
          )
        : null,
    body: TabBarView(
      controller: _tabs,
      children: [
        // `ownsViewModel` bilerek geçilmiyor: model'i host dispose eder.
        AccountsPage(
          key: _accountsKey,
          viewModel: widget.viewModel,
          embedded: true,
        ),
        widget.financeRepository == null
            ? const AppErrorView(message: 'Transfer servisi yapılandırılmadı.')
            : FinancePage(
                repository: widget.financeRepository!,
                financialDataChanges: widget.financialDataChanges,
                section: FinanceSection.transfers,
                embedded: true,
                transferPrefill: widget.transferPrefill,
                recordFee: widget.recordFee,
              ),
      ],
    ),
  );
}
