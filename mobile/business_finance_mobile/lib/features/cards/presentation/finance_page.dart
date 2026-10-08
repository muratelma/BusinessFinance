import 'dart:math';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_metric_tile.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_row_action.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../activities/data/receipt_fee_writer.dart';
import '../data/finance_models.dart';
import '../data/finance_repository.dart';
import 'finance_controller.dart';
import 'transfer_prefill.dart';

/// Bu ekranın hangi yarısının çizileceği.
///
/// Transfer iki hesap arasında para taşır; tanımı hesabı içerir, o yüzden
/// Hesaplar ekranına taşındı. Kredi kartı ise para değil borç tutar ve kendi
/// yaşam döngüsü var (harcama → ekstre → ödeme). İkisini "hesap işlemi değil"
/// diye aynı ekranda tutmak olumsuz bir tanımdı.
enum FinanceSection { cards, transfers }

class FinancePage extends StatefulWidget {
  const FinancePage({
    required this.repository,
    this.financialDataChanges,
    this.section = FinanceSection.cards,
    this.embedded = false,
    this.transferPrefill,
    this.cardPaymentPrefill,
    this.installmentPrefill,
    this.recordFee,
    super.key,
  });
  final FinanceRepositoryContract repository;
  final FinancialDataChanges? financialDataChanges;
  final FinanceSection section;

  /// Bir sekmenin içinde mi çiziliyor; gömülüyken kendi başlık çubuğunu
  /// çizmez.
  final bool embedded;

  /// Dolu gelirse transfer formu, hesaplar yüklenir yüklenmez önerilerle
  /// açılır. Kullanıcı dekontu okuttu; onu listenin başına bırakıp formu
  /// kendisinin açmasını beklemek istediği işi bir adım uzatırdı.
  final TransferPrefill? transferPrefill;

  /// Dolu gelirse kart listesinin başında "hangi kart?" notu çıkar ve seçilen
  /// kartın ödeme formu bu tutarla açılır. Kartı dekont söylemez, kullanıcı
  /// seçer.
  final CardPaymentPrefill? cardPaymentPrefill;

  /// Dolu gelirse kart listesinin başında "hangi kart?" notu çıkar ve seçilen
  /// kartın taksit planı formu bu önerilerle açılır. Kartı fiş söylemez.
  final InstallmentPrefill? installmentPrefill;

  /// Dekonttaki işlem ücretini yazan dar imza.
  ///
  /// Ücret transferin veya kart ödemesinin parçası değildir: transfer parayı
  /// taşır, kart ödemesi borç kapatır; bu ücret ise gerçekten harcanan paradır
  /// ve kendi gider kaydını hak eder. Ekran depoları tanımadığı için yazma
  /// dışarıdan geliyor (`uploadAttachment` ile aynı desen).
  final ReceiptFeeRecorder? recordFee;

  @override
  State<FinancePage> createState() => _FinancePageState();
}

class _FinancePageState extends State<FinancePage> {
  late final FinanceController controller;

  @override
  void initState() {
    super.initState();
    controller = FinanceController(
      widget.repository,
      financialDataChanges: widget.financialDataChanges,
    )..addListener(_changed);
    controller.load().then((_) {
      final snapshot = controller.snapshot;
      if (mounted &&
          snapshot != null &&
          widget.transferPrefill?.isEmpty == false) {
        _addTransfer(snapshot, prefill: widget.transferPrefill);
      }
    });
  }

  @override
  void dispose() {
    controller.removeListener(_changed);
    controller.dispose();
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    final body = _body();
    if (widget.embedded) return body;
    return Scaffold(
      appBar: AppBar(title: const Text('Kredi kartları')),
      body: body,
    );
  }

  Widget _body() {
    if (controller.unauthorized) return const AppUnauthorizedView();
    if (controller.isLoading && controller.snapshot == null) {
      return const AppLoadingView(message: 'Finans hareketleri yükleniyor');
    }
    if (controller.errorMessage != null && controller.snapshot == null) {
      return AppErrorView(
        message: controller.errorMessage!,
        onRetry: controller.load,
      );
    }
    final snapshot = controller.snapshot!;
    return Column(
      children: [
        if (controller.errorMessage != null ||
            controller.successMessage != null)
          MaterialBanner(
            content: Text(
              controller.errorMessage ?? controller.successMessage!,
            ),
            actions: [
              TextButton(
                onPressed: controller.clearMessage,
                child: const Text('Kapat'),
              ),
            ],
          ),
        Expanded(
          child: switch (widget.section) {
            FinanceSection.cards => _cards(snapshot),
            FinanceSection.transfers => _transfers(snapshot),
          },
        ),
      ],
    );
  }

  Widget _transfers(FinanceSnapshot snapshot) => Column(
    children: [
      _PeriodSelector(
        selected: controller.period,
        onSelected: controller.selectPeriod,
      ),
      if (snapshot.transfersHaveMore) const _TruncationNotice(),
      Expanded(child: _transferList(snapshot)),
    ],
  );

  Widget _transferList(FinanceSnapshot snapshot) => _FeatureList(
    onRefresh: controller.load,
    addLabel: 'Transfer ekle',
    onAdd: controller.isSubmitting ? null : () => _addTransfer(snapshot),
    empty: const AppEmptyView(
      title: 'Henüz transfer yok',
      message: 'Hesaplar arası para taşıdığınızda burada görünür.',
      icon: Icons.swap_horiz,
    ),
    isEmpty: snapshot.transfers.isEmpty,
    children: [
      for (final transfer in snapshot.transfers)
        Card(
          child: ListTile(
            leading: const Icon(Icons.swap_horiz),
            title: Text(MoneyText.format(transfer.amount, transfer.currency)),
            subtitle: Text(
              '${_choiceName(snapshot.accounts, transfer.sourceAccountId)} → '
              '${_choiceName(snapshot.accounts, transfer.destinationAccountId)} • '
              '${DateText.dayMonth(transfer.date)}'
              '${transfer.isCancelled ? ' • İptal' : ''}',
            ),
            trailing: const Icon(Icons.chevron_right),
            onTap: () => _showTransfer(snapshot, transfer),
          ),
        ),
    ],
  );

  Widget _cards(FinanceSnapshot snapshot) => _FeatureList(
    onRefresh: controller.load,
    addLabel: 'Kart ekle',
    onAdd: controller.isSubmitting ? null : () => _addCard(),
    empty: const AppEmptyView(
      title: 'Henüz kredi kartı yok',
      message: 'Limit ve ekstre günleriyle ilk kartınızı ekleyin.',
      icon: Icons.credit_card,
    ),
    isEmpty: snapshot.cards.isEmpty,
    children: [
      // Dekonttan gelindiyse liste bir seçim ekranına dönüşür: belge hangi
      // kartın borcunun kapatıldığını söylemez ve yanlış kart başka bir kartın
      // borcunu azaltırdı.
      if (widget.installmentPrefill?.isEmpty == false)
        Padding(
          padding: const EdgeInsets.only(bottom: AppSpacing.small),
          child: AppInlineNotice(
            icon: Icons.credit_card,
            message:
                'Fişteki ${widget.installmentPrefill!.installmentCount} '
                'taksitli alışveriş için kartı seçin.',
          ),
        ),
      if (widget.cardPaymentPrefill?.isEmpty == false)
        AppCard(
          child: Text(
            widget.cardPaymentPrefill!.amount == null
                ? 'Dekonttaki ödeme için kartı seçin.'
                : 'Dekonttaki '
                      '${MoneyText.format(widget.cardPaymentPrefill!.amount!, 'TRY')} '
                      'ödeme için kartı seçin.',
          ),
        ),
      // Her kart kendi kutusu: menüdeki kapılardan farklı olarak bunlar
      // birbirinin alternatifi değil, ayrı ayrı nesneler.
      for (final card in snapshot.cards)
        Padding(
          padding: const EdgeInsets.only(bottom: AppSpacing.small),
          child: AppCard(
            padding: EdgeInsets.zero,
            child: AppListRow(
              icon: Icons.credit_card,
              title: card.name,
              subtitle:
                  '${_debtSentence(card)} • '
                  'Kullanılabilir '
                  '${MoneyText.format(card.availableLimit, card.currency)}',
              trailing: const Icon(Icons.chevron_right),
              onTap: () => Navigator.of(context).push(
                MaterialPageRoute(
                  builder: (_) => CreditCardDetailPage(
                    cardId: card.id,
                    controller: controller,
                    paymentPrefill: widget.cardPaymentPrefill,
                    installmentPrefill: widget.installmentPrefill,
                    recordFee: widget.recordFee,
                  ),
                ),
              ),
            ),
          ),
        ),
    ],
  );

  Future<void> _addTransfer(
    FinanceSnapshot snapshot, {
    TransferPrefill? prefill,
  }) async {
    final input = await AppFormSheet.show<Map<String, Object?>>(
      context: context,
      builder: (_) =>
          _TransferDialog(accounts: snapshot.accounts, prefill: prefill),
    );
    if (input != null) {
      await controller.submit(
        () => controller.repository.createTransfer(input),
        'Transfer kaydedildi; gelir/gider toplamı değişmedi.',
        impact: FinanceMutationImpact.transfer,
      );
      // Ücret transferden **sonra** yazılır, önce değil: transfer
      // kaydedilmediyse ortada harcanmış bir ücret de yoktur. Kaynak
      // sorulmuyor — banka ücreti, paranın çıktığı hesaptan alır.
      final fee = prefill?.feeAmount;
      final source = input['sourceAccountId'];
      if (mounted &&
          fee != null &&
          source is String &&
          controller.errorMessage == null) {
        await _writeFee(
          sourceId: source,
          amount: fee,
          date: input['transferDate'],
          description: prefill?.feeDescription,
        );
      }
    }
  }

  /// Ücreti yazar ve sonucu — üç sonucun hangisi olursa olsun — söyler.
  Future<void> _writeFee({
    required String sourceId,
    required String amount,
    required Object? date,
    required String? description,
  }) async {
    final record = widget.recordFee;
    if (record == null || date is! String) return;
    final result = await record(
      sourceId: sourceId,
      amount: amount,
      date: date,
      description: description ?? 'İşlem ücreti',
    );
    if (!mounted) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(result.message)));
  }

  Future<void> _showTransfer(
    FinanceSnapshot snapshot,
    TransferItem transfer,
  ) async {
    // Panel, dialog değil: ayrıntı okunacak bir kayıt, cevaplanacak bir soru
    // değil. Önceki hâli etiketli metin satırlarından ibaretti — `Tarih:`,
    // `Açıklama:`, `Durum:` — ve transferin asıl bilgisi olan tutar ile iki
    // ucu, o satırların arasında aynı kademede duruyordu.
    final shouldCancel = await AppAdaptiveSheet.show<bool>(
      context: context,
      builder: (sheetContext) => _TransferDetail(
        transfer: transfer,
        sourceName: _choiceName(snapshot.accounts, transfer.sourceAccountId),
        destinationName: _choiceName(
          snapshot.accounts,
          transfer.destinationAccountId,
        ),
      ),
    );
    if (shouldCancel != true || !mounted) return;

    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.block,
      destructive: true,
      title: 'Transfer iptal edilsin mi?',
      message:
          'Hesap bakiyeleri transfer öncesine döner. Gelir ve gider '
          'toplamları değişmez.',
      confirmLabel: 'Transferi iptal et',
    );
    if (confirmed != true || !mounted) return;

    await controller.submit(
      () => controller.repository.cancelTransfer(transfer.id),
      'Transfer iptal edildi; hesap bakiyeleri geri alındı.',
      impact: FinanceMutationImpact.transfer,
    );
  }

  Future<void> _addCard() async {
    final input = await AppFormSheet.show<Map<String, Object?>>(
      context: context,
      builder: (_) => const _CardDialog(),
    );
    if (input != null) {
      await controller.submit(
        () => controller.repository.createCard(input),
        'Kredi kartı oluşturuldu.',
      );
    }
  }
}

class CreditCardDetailPage extends StatefulWidget {
  const CreditCardDetailPage({
    required this.cardId,
    required this.controller,
    super.key,
    this.paymentPrefill,
    this.installmentPrefill,
    this.recordFee,
  });
  final String cardId;
  final FinanceController controller;

  /// Dekonttan gelindiyse ödeme formu açılışta önerilerle açılır.
  final CardPaymentPrefill? paymentPrefill;

  /// Taksitli fişten gelindiyse plan formu açılışta önerilerle açılır.
  final InstallmentPrefill? installmentPrefill;

  /// Dekonttaki işlem ücretini yazan dar imza; ödeme yazıldıktan sonra
  /// **ödeme hesabından** çalışır.
  final ReceiptFeeRecorder? recordFee;

  @override
  State<CreditCardDetailPage> createState() => _CreditCardDetailPageState();
}

class _CreditCardDetailPageState extends State<CreditCardDetailPage> {
  late Future<CardActivity> activity;

  /// Dekonttan gelen öneri **bir kez** kullanılır.
  ///
  /// Form iki yerden açılıyor (ilk çizim ve liste tazelenmesi), çünkü ilk
  /// çizimde kart listesi henüz yüklenmemiş olabilir. Bayrak olmadan ödeme
  /// kaydedildikten sonra form kendini yeniden açıyordu — ve ücret de her
  /// açılışta yeniden yazılırdı.
  bool _prefillUsed = false;
  bool _planPrefillUsed = false;

  /// Kartın en son kesilmiş ekstresi; kart listesiyle birlikte gelmiyor.
  ///
  /// Bir ekstre beş toplam sorgusu demek; listeye gömülseydi her kart için
  /// tekrarlanırdı. Detay tek kart gösterdiği için burada tek çağrı yeter.
  late Future<CardStatement?> currentStatement;

  @override
  void initState() {
    super.initState();
    widget.controller.addListener(_changed);
    activity = widget.controller.loadActivity(widget.cardId);
    currentStatement = widget.controller.loadCurrentStatement(widget.cardId);

    _openPrefilledPayment();
    _openPrefilledPlan();
  }

  /// Taksitli fişin önerdiği planı **bir kez** açar; kart ödeme yolunda
  /// öğrenilen ders.
  void _openPrefilledPlan() {
    final prefill = widget.installmentPrefill;
    if (_planPrefillUsed || prefill == null || prefill.isEmpty) return;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final card = widget.controller.snapshot?.cards
          .where((item) => item.id == widget.cardId)
          .firstOrNull;
      if (card == null || !mounted || _planPrefillUsed) return;
      _planPrefillUsed = true;
      _addPlan(card, prefill: prefill);
    });
  }

  /// Dekont okunarak gelindiyse ödeme formunu kendiliğinden açar: kullanıcı
  /// kartı bir önceki ekranda seçti, ikinci kez "Ödeme"ye basması istediği işi
  /// uzatırdı.
  void _openPrefilledPayment() {
    final prefill = widget.paymentPrefill;
    if (_prefillUsed || prefill == null || prefill.isEmpty) return;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final card = widget.controller.snapshot?.cards
          .where((item) => item.id == widget.cardId)
          .firstOrNull;
      if (card == null || !mounted || _prefillUsed) return;
      _prefillUsed = true;
      _addPayment(
        card,
        presetAmount: prefill.amount,
        presetDate: prefill.date,
        presetDescription: prefill.description,
        // Ücret yalnız bu yolda yazılır. Kullanıcının elle açtığı bir ödeme
        // dekontla ilgisizdir ve ona ücret iliştirmek olmayan bir gider
        // yazmak olurdu.
        feeAmount: prefill.feeAmount,
        feeDescription: prefill.feeDescription,
      );
    });
  }

  @override
  void dispose() {
    widget.controller.removeListener(_changed);
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  CreditCardItem? get card => widget.controller.snapshot?.cards
      .where((item) => item.id == widget.cardId)
      .firstOrNull;

  @override
  Widget build(BuildContext context) {
    final current = card;
    return Scaffold(
      appBar: AppBar(
        title: Text(current?.name ?? 'Kart detayı'),
        actions: [
          if (current != null)
            IconButton(
              onPressed: () => _editCard(current),
              icon: const Icon(Icons.edit_outlined),
              tooltip: 'Kartı düzenle',
            ),
        ],
      ),
      body: current == null
          ? const AppErrorView(message: 'Kart artık bulunamıyor.')
          : Column(
              children: [
                // Yazma sonucunu **bu** ekran söyler. Denetim sırasında bulunan
                // kusur buydu: harcama, ödeme ve taksit planı buradan
                // yazılıyor ama sonucu yalnız kart listesi gösteriyordu.
                // Reddedilen bir ödeme sessizce kayboluyor, kullanıcı da
                // kaydedildi sanıyordu; hata ancak listeye dönünce, bağlamından
                // kopmuş hâlde görünüyordu.
                if (widget.controller.errorMessage != null ||
                    widget.controller.successMessage != null)
                  MaterialBanner(
                    content: Text(
                      widget.controller.errorMessage ??
                          widget.controller.successMessage!,
                    ),
                    actions: [
                      TextButton(
                        onPressed: widget.controller.clearMessage,
                        child: const Text('Kapat'),
                      ),
                    ],
                  ),
                Expanded(
                  child: ListView(
                    padding: const EdgeInsets.all(AppSpacing.medium),
                    children: [
                      _summary(current),
                      const SizedBox(height: AppSpacing.large),

                      // Planlar kart hareketlerinin dışında: hiç harcaması olmayan
                      // bir kartın planları da görünmeli, yoksa boş durum ekranı
                      // onları gizlerdi.
                      _plansOfCard(current),
                      FutureBuilder<CardActivity>(
                        future: activity,
                        builder: (context, snapshot) {
                          if (snapshot.connectionState !=
                              ConnectionState.done) {
                            return const AppLoadingView(
                              message: 'Kart hareketleri yükleniyor',
                            );
                          }
                          if (snapshot.hasError) {
                            return AppErrorView(
                              message: 'Kart hareketleri yüklenemedi.',
                              onRetry: _reloadActivity,
                            );
                          }
                          final value = snapshot.data!;
                          if (value.charges.isEmpty && value.payments.isEmpty) {
                            // Seçici boş durumda da duruyor: liste dönem yüzünden
                            // boş olabilir ve kullanıcının onu genişletebilmesi
                            // gerekir. Gizlenseydi ekran "hiç hareket yok" derdi.
                            return Column(
                              children: [
                                _PeriodSelector(
                                  selected: widget.controller.period,
                                  onSelected: (period) async {
                                    await widget.controller.selectPeriod(
                                      period,
                                    );
                                    _reloadActivity();
                                  },
                                ),
                                const SizedBox(
                                  height: 320,
                                  child: AppEmptyView(
                                    title: 'Bu dönemde kart hareketi yok',
                                    message:
                                        'Harcama ve ödemeler burada ayrı gösterilir. '
                                        'Daha eskisi için dönemi genişletin.',
                                    icon: Icons.credit_card,
                                  ),
                                ),
                              ],
                            );
                          }
                          return Column(
                            crossAxisAlignment: CrossAxisAlignment.stretch,
                            children: [
                              _PeriodSelector(
                                selected: widget.controller.period,
                                onSelected: (value) async {
                                  await widget.controller.selectPeriod(value);
                                  _reloadActivity();
                                },
                              ),
                              if (value.hasMore) const _TruncationNotice(),
                              // Harcama ve ödeme ayrı gruplarda: biri kart borcunu
                              // büyütür, diğeri küçültür. Tek listede olsalar
                              // tutarların yönü kaybolurdu.
                              if (value.charges.isNotEmpty) ...[
                                const AppSectionHeader(title: 'Harcamalar'),
                                _movementGroup(
                                  [
                                    for (final item in value.charges)
                                      (
                                        amount: item.amount,
                                        currency: item.currency,
                                        date: item.date,
                                        description: item.description,
                                        isCancelled: item.isCancelled,
                                      ),
                                  ],
                                  Icons.shopping_cart_outlined,
                                  AppMoneyEffect.expense,
                                ),
                                const SizedBox(height: AppSpacing.medium),
                              ],
                              if (value.payments.isNotEmpty) ...[
                                const AppSectionHeader(title: 'Ödemeler'),
                                _movementGroup(
                                  [
                                    for (final item in value.payments)
                                      (
                                        amount: item.amount,
                                        currency: item.currency,
                                        date: item.date,
                                        description: item.description,
                                        isCancelled: item.isCancelled,
                                      ),
                                  ],
                                  Icons.payments_outlined,

                                  // Kart ödemesi gider değildir: aynı harcamayı iki
                                  // kez saymamak için nötr gösterilir.
                                  AppMoneyEffect.neutral,
                                ),
                              ],
                            ],
                          );
                        },
                      ),
                    ],
                  ),
                ),
              ],
            ),
    );
  }

  /// Kartın durumu, üç kademeli.
  ///
  /// Sıra bankaların sırası ve bilinçli: en üstte **ödenmesi gereken** tutar
  /// (dönem borcu), altında kartın **anlık** durumu (güncel borç,
  /// kullanılabilir), en altta kartın **ayarı** (kesim ve son ödeme günü).
  /// Üçü eşit ağırlıkta gösterilseydi "şimdi ne yapmalıyım" sorusunun cevabı
  /// diğer sayıların arasında kaybolurdu.
  ///
  /// Dönem borcu ile güncel borç farklı şeyler: ilki kesilmiş ekstrenin
  /// kalanı, ikincisi kesimden sonraki harcamalar dâhil bütün borç. Ayrı
  /// kademelerde durmaları bu farkı yerleşimle anlatıyor.
  Widget _summary(CreditCardItem card) => AppCard(
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        _statementHeadline(card),
        const Divider(height: AppSpacing.large),
        Row(
          children: [
            Expanded(
              // Alacaklı kartta hem etiket hem yön değişir: "Güncel borç
              // −₺500" kullanıcıya borcu varmış gibi okunur ve kırmızı durur.
              child: MoneyText.isNegative(card.currentDebt)
                  ? AppMetricTile(
                      label: 'Kart alacağınız',
                      amount: MoneyText.unsigned(card.currentDebt),
                      currency: card.currency,
                      effect: AppMoneyEffect.income,
                    )
                  : AppMetricTile(
                      label: 'Güncel borç',
                      amount: card.currentDebt,
                      currency: card.currency,
                      effect: AppMoneyEffect.expense,
                    ),
            ),
            const SizedBox(width: AppSpacing.medium),
            Expanded(
              child: AppMetricTile(
                label: 'Kullanılabilir',
                amount: card.availableLimit,
                currency: card.currency,
              ),
            ),
          ],
        ),
        const Divider(height: AppSpacing.large),
        _dateRow(
          Icons.event_outlined,
          'Hesap kesim günü',
          'Her ayın ${card.statementClosingDay}.',
        ),
        const SizedBox(height: AppSpacing.small),
        _dateRow(
          Icons.schedule_outlined,
          'Son ödeme günü',
          'Her ayın ${card.paymentDueDay}.',
        ),
        const Divider(height: AppSpacing.large),
        _actions(card),
      ],
    ),
  );

  /// Özet kartının manşeti: şu an ödenmesi gereken ekstre.
  Widget _statementHeadline(CreditCardItem card) =>
      FutureBuilder<CardStatement?>(
        future: currentStatement,
        builder: (context, snapshot) {
          if (snapshot.connectionState != ConnectionState.done) {
            // Sabit yükseklik verilmiyor: en büyük yazı ölçeğinde yükleme
            // görünümü 96 px'e sığmıyor ve satırı taşırıyordu.
            return const AppLoadingView(message: 'Ekstre hesaplanıyor');
          }
          if (snapshot.hasError) {
            return _headlineNote(
              'Ekstre yüklenemedi.',
              action: TextButton(
                onPressed: _reloadActivity,
                child: const Text('Yeniden dene'),
              ),
            );
          }

          final statement = snapshot.data;
          if (statement == null) {
            // Kusur değil: kartın ilk kesim tarihi henüz gelmemiş. Hata gibi
            // göstermek kullanıcıyı olmayan bir sorunu aramaya iterdi.
            return _headlineNote(
              'Henüz ekstre kesilmedi. İlk ekstre, ayın '
              '${card.statementClosingDay}. günü oluşur.',
            );
          }
          return _statementBanner(card, statement);
        },
      );

  Widget _headlineNote(String message, {Widget? action}) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      Text(
        message,
        style: Theme.of(context).textTheme.bodyMedium?.copyWith(
          color: AppSurfaces.of(context).inkMuted,
        ),
      ),
      ?action,
    ],
  );

  Widget _statementBanner(CreditCardItem card, CardStatement statement) {
    final theme = Theme.of(context);
    final muted = AppSurfaces.of(context).inkMuted;
    final isSettled = statement.isPaid;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: Text(
                'Dönem borcu',
                style: theme.textTheme.labelMedium?.copyWith(color: muted),
              ),
            ),
            _statementStatusChip(statement),
          ],
        ),
        const SizedBox(height: AppSpacing.xSmall),

        // Manşetteki tutar kalan borç, ekstre borcu değil: kısmen ödenmiş bir
        // ekstrede kullanıcının sorusu "daha ne kadar" — kesimdeki tutar
        // ayrıntı satırlarında ve ekstre penceresinde duruyor.
        AppMoneyText(
          amount: statement.remainingBalance,
          currency: statement.currency,
          effect: isSettled ? null : AppMoneyEffect.expense,
          style: AppTypography.heroMoney(theme.textTheme.displaySmall!),
        ),
        const SizedBox(height: AppSpacing.xSmall),
        Text(
          '${DateText.monthYear(statement.year, statement.month)} ekstresi',
          style: theme.textTheme.bodySmall?.copyWith(color: muted),
        ),
        const SizedBox(height: AppSpacing.medium),
        if (!isSettled) ...[
          _dateRow(
            Icons.pending_actions_outlined,
            'Asgari ödeme',
            MoneyText.format(
              statement.remainingMinimumPayment,
              statement.currency,
            ),
          ),
          const SizedBox(height: AppSpacing.small),
        ],
        _dateRow(
          Icons.event_available_outlined,
          'Son ödeme tarihi',
          DateText.dayMonth(statement.dueDate),
        ),
        if (!isSettled) ...[
          const SizedBox(height: AppSpacing.medium),
          _statementPayActions(card, statement),
        ],
      ],
    );
  }

  Widget _statementStatusChip(CardStatement statement) => AppStatusChip(
    label: statement.isPaid
        ? 'Ödendi'
        : statement.isOverdue
        ? 'Gecikmiş'
        : 'Açık',
    icon: statement.isPaid
        ? Icons.check_circle_outline
        : statement.isOverdue
        ? Icons.warning_amber_outlined
        : Icons.schedule_outlined,
    tone: statement.isPaid
        ? AppStatusTone.income
        : statement.isOverdue
        ? AppStatusTone.expense
        : AppStatusTone.planned,
  );

  /// Ekstre ödeme kısayolları.
  ///
  /// İkisi de aynı `Ödeme` akışını açıyor, yalnız tutarı önceden dolduruyor:
  /// ödeme bir döneme bağlanmıyor. Ekstre kalıcı bir kayıt değil, kesim ve
  /// ödeme tarihlerinden hesaplanan bir görünüm; ona yabancı anahtar bağlamak
  /// olmayan bir satıra referans vermek olurdu.
  Widget _statementPayActions(CreditCardItem card, CardStatement statement) {
    // Sunucu tutarları hep dört ondalıklı kanonik biçimde gönderiyor, bu
    // yüzden karşılaştırma metin üzerinden güvenli — ondalık sayıya çevirip
    // kıyaslamak, gösterim için kayan nokta matematiği yapmak olurdu.
    final minimum = statement.remainingMinimumPayment;
    final hasSeparateMinimum =
        minimum != statement.remainingBalance && minimum != '0.0000';
    return Wrap(
      spacing: AppSpacing.small,
      runSpacing: AppSpacing.small,
      children: [
        FilledButton.icon(
          onPressed: () => _payStatement(card, statement.remainingBalance),
          icon: const Icon(Icons.receipt_long_outlined),
          label: const Text('Ekstreyi öde'),
        ),
        if (hasSeparateMinimum)
          OutlinedButton.icon(
            onPressed: () => _payStatement(card, minimum),
            icon: const Icon(Icons.pending_actions_outlined),
            label: const Text('Asgariyi öde'),
          ),
      ],
    );
  }

  /// Etiket–değer satırı: solda ne olduğu, sağda değeri.
  Widget _dateRow(IconData icon, String label, String value) => Row(
    children: [
      Icon(icon, size: 18, color: AppSurfaces.of(context).inkMuted),
      const SizedBox(width: AppSpacing.small),
      Expanded(
        child: Text(
          label,
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
            color: AppSurfaces.of(context).inkMuted,
          ),
        ),
      ),
      // Sağdaki değer de esnek: kısıtsız bırakıldığında en büyük yazı
      // ölçeğinde satırı 31 px taşırıyordu.
      Flexible(
        child: Text(
          value,
          textAlign: TextAlign.end,
          style: Theme.of(
            context,
          ).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.w600),
        ),
      ),
    ],
  );

  /// Kart eylemleri, ne yaptıklarına göre iki gruba ayrılmış.
  ///
  /// Dördü tek bir `Wrap` içindeyken sıraları rastgele görünüyordu ve
  /// `runSpacing` verilmediği için alt satıra düşen butonun kenarı üsttekine
  /// yapışıyordu. Üst grup para harcar, alt grup kartı yönetir.
  ///
  /// **Taksitli harcama da dolgulu:** o da bir harcamadır, yalnız tek seferde
  /// değil taksitle. Tonal bırakmak onu ikincil bir işmiş gibi gösteriyordu.
  /// Tasarım sistemindeki "ekran başına tek birincil" kuralı burada tek bir
  /// eylemin iki biçimine uygulanıyor; ikisi birlikte bir grup.
  ///
  /// Grup özet kartının **içinde** duruyor. Dışarıda, kartın altında serbest
  /// dururken neye ait olduğunu yalnız yakınlığı söylüyordu; kartın kenarlığı
  /// bitip butonlar boşlukta başlıyordu. Kendi başlığı ve ayıracı var ki
  /// üstündeki sayı kademeleriyle karışmasın.
  Widget _actions(CreditCardItem card) => Column(
    crossAxisAlignment: CrossAxisAlignment.stretch,
    children: [
      Text(
        'İŞLEMLER',
        style: Theme.of(context).textTheme.labelSmall?.copyWith(
          color: AppSurfaces.of(context).inkMuted,
        ),
      ),
      const SizedBox(height: AppSpacing.small),
      Wrap(
        spacing: AppSpacing.small,
        runSpacing: AppSpacing.small,
        children: [
          FilledButton.icon(
            onPressed: card.isActive ? () => _addCharge(card) : null,
            icon: const Icon(Icons.shopping_cart_outlined),
            label: const Text('Harcama'),
          ),
          FilledButton.icon(
            onPressed: card.isActive ? () => _addPlan(card) : null,
            icon: const Icon(Icons.calendar_view_month),
            label: const Text('Taksitli harcama'),
          ),
        ],
      ),
      const SizedBox(height: AppSpacing.small),
      Wrap(
        spacing: AppSpacing.small,
        runSpacing: AppSpacing.small,
        children: [
          FilledButton.tonalIcon(
            onPressed: () => _addPayment(card),
            icon: const Icon(Icons.payments_outlined),
            label: const Text('Ödeme'),
          ),
          OutlinedButton.icon(
            onPressed: () => _showStatement(card),
            icon: const Icon(Icons.receipt_long_outlined),
            label: const Text('Ekstre'),
          ),
        ],
      ),
    ],
  );

  /// Kart hareketlerinin ortak satır grubu.
  ///
  /// Harcama ve ödeme farklı tipler ama ekranda aynı şekli alıyorlar; iki ayrı
  /// `ListTile` bloğu yazmak, birinde yapılan düzeltmenin diğerinde
  /// unutulmasıyla biterdi.
  Widget _movementGroup(
    List<
      ({
        String amount,
        String currency,
        String date,
        String? description,
        bool isCancelled,
      })
    >
    items,
    IconData icon,
    AppMoneyEffect effect,
  ) => AppCard(
    padding: EdgeInsets.zero,
    child: Column(
      children: [
        for (final (index, item) in items.indexed) ...[
          if (index > 0) const Divider(height: 1),
          AppListRow(
            icon: icon,
            title: MoneyText.format(item.amount, item.currency),
            subtitle: item.description == null
                ? item.date
                : '${DateText.dayMonth(item.date)} • ${item.description}',
            dimmed: item.isCancelled,
            badge: item.isCancelled
                ? const AppStatusChip(
                    label: 'İptal',
                    icon: Icons.block,
                    tone: AppStatusTone.cancelled,
                  )
                : null,
            trailing: AppMoneyText(
              amount: item.amount,
              currency: item.currency,
              effect: effect,
              isCancelled: item.isCancelled,
            ),
          ),
        ],
      ],
    ),
  );

  /// Bu kartın taksit planları.
  ///
  /// Plan yalnız niyettir; gerçekleşmemiş taksit hiçbir rapora girmez. O yüzden
  /// harcamaların *üstünde* ayrı bir bölüm: aynı listeye karıştırılsa gerçek
  /// harcamayla plan aynı şeymiş gibi görünürdü.
  Widget _plansOfCard(CreditCardItem card) {
    final plans = (widget.controller.snapshot?.plans ?? const [])
        .where((plan) => plan.creditCardId == card.id)
        .toList(growable: false);
    if (plans.isEmpty) return const SizedBox.shrink();

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const AppSectionHeader(title: 'Taksit planları'),
        for (final plan in plans)
          ExpansionTile(
            tilePadding: EdgeInsets.zero,
            shape: const Border(),
            collapsedShape: const Border(),
            leading: const Icon(Icons.calendar_view_month),
            title: Text(plan.description ?? 'Taksit planı'),
            subtitle: Text(
              '${plan.items.length} taksit · '
              '${MoneyText.format(plan.totalAmount, plan.currency)}',
            ),
            children: [for (final item in plan.items) _planItemRow(plan, item)],
          ),
        const SizedBox(height: AppSpacing.medium),
      ],
    );
  }

  /// Bir taksit planının tek satırı.
  ///
  /// Durum ve eylem **aynı yerde ve aynı boyda** duruyor. Eskiden gerçekleşmiş
  /// taksitte kocaman bir `Chip`, gerçekleşmemişte dolgulu bir buton vardı: iki
  /// satır aynı listede farklı yükseklikte ve farklı ağırlıkta görünüyor,
  /// listeyi zıplatıyordu. Oysa ikisi aynı bilginin iki hâli — biri oldu, biri
  /// olmadı — ve satırın asıl bilgisi tutar.
  Widget _planItemRow(
    InstallmentPlanModel plan,
    InstallmentItemModel item,
  ) => AppListRow(
    icon: item.isRealized ? Icons.check_circle_outline : Icons.schedule,
    title: '${item.sequence}. taksit',
    subtitle: DateText.dayMonthYear(item.scheduledDate),
    badge: Wrap(
      alignment: WrapAlignment.spaceBetween,
      spacing: AppSpacing.small,
      runSpacing: AppSpacing.small,
      crossAxisAlignment: WrapCrossAlignment.center,
      children: [
        AppStatusChip(
          label: item.isRealized ? 'Gerçekleşti' : 'Planlandı',
          icon: item.isRealized ? Icons.check : Icons.schedule,
          tone: item.isRealized ? AppStatusTone.neutral : AppStatusTone.planned,
        ),
        // Plan yalnız niyettir; gerçek kayıt yalnız burada üretilir.
        //
        // Çerçeveli: çerçevesiz hâlde rozetin yanındaki ikinci bir etiket gibi
        // okunuyor ve tıklanabilir olduğu fark edilmiyordu. Dolgulu buton ise
        // satır içinde fazla ağır — o ağırlık ekranın birincil eylemine ait.
        if (!item.isRealized)
          AppRowAction(
            label: 'Gerçekleştir',
            onPressed: widget.controller.isSubmitting
                ? null
                : () => _realize(plan.id, item.sequence),
          ),
      ],
    ),
    trailing: AppMoneyText(
      amount: item.amount,
      currency: item.currency,
      // Gerçekleşmemiş taksit henüz gider değil: kırmızı yazmak olmamış
      // bir harcamayı olmuş göstermek olurdu.
      effect: item.isRealized ? AppMoneyEffect.expense : AppMoneyEffect.neutral,
    ),
  );

  Future<void> _realize(String planId, int sequence) async {
    if (await widget.controller.submit(
      () => widget.controller.repository.realizeInstallment(planId, sequence),
      'Taksit gider hareketine dönüştürüldü.',
      impact: FinanceMutationImpact.installmentRealized,
    )) {
      _reloadActivity();
    }
  }

  /// Kart hareketini **ve** ekstreyi birlikte tazeler.
  ///
  /// İkisi aynı veriden türüyor: harcama ya da ödeme eklendiğinde yalnız
  /// listeyi yenilemek, üstteki dönem borcunu eski değerinde bırakırdı.
  void _reloadActivity() => setState(() {
    activity = widget.controller.loadActivity(widget.cardId);
    currentStatement = widget.controller.loadCurrentStatement(widget.cardId);

    _openPrefilledPayment();
  });

  /// Kart ayarlarını düzenler.
  ///
  /// Asgari ödeme oranı karta ait bir ayar; onu değiştirebilmek için kartın
  /// düzenlenebilmesi gerekiyordu ve uygulamada hiç kart düzenleme yolu yoktu.
  Future<void> _editCard(CreditCardItem card) async {
    final input = await AppFormSheet.show<Map<String, Object?>>(
      context: context,
      builder: (_) => _CardDialog(card: card),
    );
    if (input == null) return;
    if (await widget.controller.submit(
      () => widget.controller.repository.updateCard(card.id, input),
      'Kart güncellendi.',
    )) {
      _reloadActivity();
    }
  }

  Future<void> _addPlan(
    CreditCardItem card, {
    InstallmentPrefill? prefill,
  }) async {
    final input = await AppFormSheet.show<Map<String, Object?>>(
      context: context,
      builder: (_) => _PlanDialog(
        cards: const [],
        categories: widget.controller.snapshot!.expenseCategories,
        fixedCardId: card.id,
        fixedCardScope: card.defaultScope,
        prefill: prefill,
      ),
    );
    if (input != null) {
      await widget.controller.submit(
        () => widget.controller.repository.createPlan(input),
        'Taksit planı oluşturuldu; henüz gider yazılmadı.',
      );
    }
  }

  Future<void> _addCharge(CreditCardItem card) async {
    final input = await AppFormSheet.show<Map<String, Object?>>(
      context: context,
      builder: (_) => _ActivityDialog(
        title: 'Kart harcaması',
        choiceLabel: 'Gider kategorisi',
        choices: widget.controller.snapshot!.expenseCategories,
        choiceKey: 'categoryId',
        dateKey: 'chargeDate',
        asksScope: true,
        sourceScope: card.defaultScope,
        sourceName: card.name,
      ),
    );
    if (input != null &&
        await widget.controller.submit(
          () => widget.controller.repository.createCharge(card.id, input),
          'Kart harcaması kaydedildi.',
          impact: FinanceMutationImpact.cardCharge,
        )) {
      _reloadActivity();
    }
  }

  /// Ekstre kalanı ya da asgarisi kadar ödeme.
  ///
  /// Ayrı bir uç nokta yok, ayrı bir kayıt türü de yok: aynı kart ödemesi,
  /// yalnız tutarı önceden doldurulmuş. Tutar düzenlenebilir kalıyor çünkü
  /// kısmi ödeme meşru bir davranış.
  Future<void> _payStatement(CreditCardItem card, String amount) =>
      _addPayment(card, presetAmount: amount);

  Future<void> _addPayment(
    CreditCardItem card, {
    String? presetAmount,
    String? presetDate,
    String? presetDescription,
    String? feeAmount,
    String? feeDescription,
  }) async {
    final input = await AppFormSheet.show<Map<String, Object?>>(
      context: context,
      builder: (_) => _ActivityDialog(
        title: 'Kart ödemesi',
        choiceLabel: 'Ödeme hesabı',
        choices: widget.controller.snapshot!.accounts,
        choiceKey: 'accountId',
        dateKey: 'paymentDate',
        presetAmount: presetAmount,
        presetDate: presetDate,
        presetDescription: presetDescription,
      ),
    );
    if (input != null &&
        await widget.controller.submit(
          () => widget.controller.repository.createPayment(card.id, input),
          'Kart borcu ödemesi kaydedildi; yeni gider oluşmadı.',
          impact: FinanceMutationImpact.cardPayment,
        )) {
      _reloadActivity();
      // Ücret ödemeden **sonra** ve ödemenin çıktığı hesaptan. Kart borcuna
      // eklenseydi bankanın aldığı para karta borç görünürdü; kart ödemesinin
      // gider üretmemesi kuralı da yalnız ödemenin kendisi için geçerli —
      // ücret gerçekten harcanmış paradır.
      final account = input['accountId'];
      if (feeAmount != null && account is String) {
        await _writeFee(
          sourceId: account,
          amount: feeAmount,
          date: input['paymentDate'],
          description: feeDescription,
        );
      }
    }
  }

  /// Ücreti yazar ve sonucu — üç sonucun hangisi olursa olsun — söyler.
  Future<void> _writeFee({
    required String sourceId,
    required String amount,
    required Object? date,
    required String? description,
  }) async {
    final record = widget.recordFee;
    if (record == null || date is! String) return;
    final result = await record(
      sourceId: sourceId,
      amount: amount,
      date: date,
      description: description ?? 'İşlem ücreti',
    );
    if (!mounted) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(result.message)));
  }

  /// Ekstre penceresini açar.
  ///
  /// Dönem seçimi artık önden doldurulan bir form değil, pencerenin içindeki
  /// ileri/geri: kimse "hangi ekstre" sorusuna yıl ve ay yazarak cevap
  /// vermiyor, "şu anki" isteniyor ve gerekirse geriye bakılıyor.
  Future<void> _showStatement(CreditCardItem card) async {
    final amount = await showDialog<String>(
      context: context,
      builder: (_) =>
          _StatementSheet(card: card, controller: widget.controller),
    );
    if (amount != null && mounted) await _payStatement(card, amount);
  }
}

class _FeatureList extends StatelessWidget {
  const _FeatureList({
    required this.onRefresh,
    required this.addLabel,
    required this.onAdd,
    required this.empty,
    required this.isEmpty,
    required this.children,
  });
  final String addLabel;
  final Future<void> Function() onRefresh;
  final VoidCallback? onAdd;
  final Widget empty;

  /// Listenin **verisi** boş mu.
  ///
  /// `children.isEmpty` ile ölçülmüyor: liste veriden başka satır da taşır
  /// (fişten gelindiğinde eklenen yönlendirme kutusu gibi). O kutu tek başına
  /// kaldığında liste dolu sayılır ve ekran boş durumu hiç çizmezdi — fişten
  /// gelen kullanıcı, hiç kartı olmadığını söyleyen cümleyi göremiyordu.
  final bool isEmpty;

  final List<Widget> children;

  @override
  Widget build(BuildContext context) => RefreshIndicator(
    onRefresh: onRefresh,
    child: ListView(
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.medium,
        AppSpacing.medium,
        AppSpacing.medium,
        AppSpacing.fabClearance,
      ),
      children: [
        Align(
          alignment: Alignment.centerRight,
          child: FilledButton.icon(
            onPressed: onAdd,
            icon: const Icon(Icons.add),
            label: Text(addLabel),
          ),
        ),
        const SizedBox(height: AppSpacing.small),
        ...children,
        if (isEmpty) SizedBox(height: 420, child: empty),
      ],
    ),
  );
}

class _TransferDialog extends StatefulWidget {
  const _TransferDialog({required this.accounts, this.prefill});
  final List<FinanceChoice> accounts;

  /// Dekonttan okunan öneriler. Kaynak ve hedef burada **yok**: belge hangi
  /// hesabın kullanıldığını söylemez ve yanlış uç, transferi bambaşka bir para
  /// hareketine çevirir.
  final TransferPrefill? prefill;
  @override
  State<_TransferDialog> createState() => _TransferDialogState();
}

class _TransferDialogState extends State<_TransferDialog> {
  final key = GlobalKey<FormState>();
  late final TextEditingController amount;
  late final TextEditingController description;
  String? source;
  String? destination;
  late String date;

  @override
  void initState() {
    super.initState();
    final prefill = widget.prefill;
    amount = TextEditingController(text: prefill?.amount ?? '');
    description = TextEditingController(text: prefill?.description ?? '');
    date = prefill?.date ?? _dateText(DateTime.now());
  }

  @override
  void dispose() {
    amount.dispose();
    description.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: key,
    child: AppFormSheet<Map<String, Object?>>(
      title: 'Yeni transfer',
      description:
          'Transfer gelir veya gider değildir; parayı iki hesap arasında '
          'taşır ve raporları etkilemez.',
      submitLabel: 'Kaydet',
      onSubmit: () async {
        if (!(key.currentState?.validate() ?? false)) return null;
        if (source == destination) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Kaynak ve hedef farklı olmalı.')),
          );
          return null;
        }
        return {
          'sourceAccountId': source,
          'destinationAccountId': destination,
          'amount': _money(amount.text),
          'currency': 'TRY',
          'transferDate': date,
          'description': _nullable(description.text),
        };
      },
      children: [
        AppFormField(
          child: _choice(
            widget.accounts,
            'Kaynak hesap',
            source,
            (v) => setState(() => source = v),
          ),
        ),
        AppFormField(
          child: _choice(
            widget.accounts,
            'Hedef hesap',
            destination,
            (v) => setState(() => destination = v),
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: amount,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: const InputDecoration(
              labelText: 'Tutar',
              suffixText: 'TRY',
            ),
            validator: _moneyError,
          ),
        ),
        AppFormField(
          child: AppDateField(
            label: 'Tarih',
            value: date,
            onChanged: (value) => setState(() => date = value),
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: description,
            hintLocales: const [Locale('tr', 'TR')],
            decoration: const InputDecoration(labelText: 'Açıklama'),
          ),
        ),
      ],
    ),
  );
}

/// Kart oluşturma ve düzenleme formu.
///
/// Tek form: alanlar aynı, yalnız başlangıç değerleri ve gönderilen fiil
/// farklı. İki ayrı form olsaydı birinde düzeltilen doğrulama kuralı ötekinde
/// unutulurdu.
class _CardDialog extends StatefulWidget {
  const _CardDialog({this.card});

  /// Doluysa düzenleme, boşsa yeni kart.
  final CreditCardItem? card;
  @override
  State<_CardDialog> createState() => _CardDialogState();
}

class _CardDialogState extends State<_CardDialog> {
  final key = GlobalKey<FormState>();
  late final name = TextEditingController(text: widget.card?.name ?? '');
  late final limit = TextEditingController(
    text: widget.card == null ? '' : MoneyText.editable(widget.card!.limit),
  );
  late final closing = TextEditingController(
    text: '${widget.card?.statementClosingDay ?? 10}',
  );
  late final due = TextEditingController(
    text: '${widget.card?.paymentDueDay ?? 20}',
  );

  // Yüzde olarak yazılıyor: kullanıcı "20" girer, "0,2" değil.
  late final minimumRate = TextEditingController(
    text: MoneyText.percent(widget.card?.minimumPaymentRate ?? '20.0000'),
  );

  late TransactionScope? defaultScope = widget.card?.defaultScope;
  @override
  void dispose() {
    name.dispose();
    limit.dispose();
    closing.dispose();
    due.dispose();
    minimumRate.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: key,
    child: AppFormSheet<Map<String, Object?>>(
      title: widget.card == null ? 'Yeni kredi kartı' : 'Kartı düzenle',
      submitLabel: 'Kaydet',
      onSubmit: () async {
        if (!(key.currentState?.validate() ?? false)) return null;
        return {
          'name': name.text.trim(),
          'limit': _money(limit.text),
          'currency': 'TRY',
          'statementClosingDay': int.parse(closing.text),
          'paymentDueDay': int.parse(due.text),
          'minimumPaymentRate': _money(minimumRate.text),
          if (widget.card != null) 'isActive': widget.card!.isActive,
          // Kapsamı görmeyen kullanıcıda alan hiç çizilmiyor ama değer yine de
          // gidiyor: sunucudaki `PUT` yetkili ve göndermemek "kaldır" demek.
          'defaultScope': defaultScope?.apiValue,
        };
      },
      children: [
        AppFormField(
          child: TextFormField(
            controller: name,
            hintLocales: const [Locale('tr', 'TR')],
            decoration: const InputDecoration(labelText: 'Kart adı'),
            validator: (v) =>
                v == null || v.trim().isEmpty ? 'Kart adı girin.' : null,
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: limit,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: const InputDecoration(
              labelText: 'Limit',
              suffixText: 'TRY',
            ),
            validator: _moneyError,
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: closing,
            decoration: const InputDecoration(labelText: 'Kesim günü (1–28)'),
            keyboardType: TextInputType.number,
            validator: _dayError,
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: due,
            decoration: const InputDecoration(
              labelText: 'Son ödeme günü (1–28)',
            ),
            keyboardType: TextInputType.number,
            validator: _dayError,
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: minimumRate,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: const InputDecoration(
              labelText: 'Asgari ödeme oranı',
              suffixText: '%',

              // Oranı bankası ve limiti belirliyor; uygulama bunu bilemez,
              // bu yüzden soruyor ve makul bir varsayılanla başlıyor.
              helperText:
                  'Bankanızın uyguladığı oran; genellikle %20 ya da %40.',
            ),
            validator: _rateError,
          ),
        ),
        if (context.watch<ScopeController?>()?.isVisible ?? false)
          AppFormField(
            child: AppScopeDefaultField(
              value: defaultScope,
              onChanged: (value) => setState(() => defaultScope = value),
              helperText:
                  'Harcamanın tarafını kategori belirler. Kategori iki tarafa '
                  'da açıksa bu seçili gelir.',
            ),
          ),
      ],
    ),
  );
}

class _ActivityDialog extends StatefulWidget {
  const _ActivityDialog({
    required this.title,
    required this.choiceLabel,
    required this.choices,
    required this.choiceKey,
    required this.dateKey,
    this.presetAmount,
    this.presetDate,
    this.presetDescription,
    this.asksScope = false,
    this.sourceScope,
    this.sourceName,
  });
  final String title;
  final String choiceLabel;
  final List<FinanceChoice> choices;
  final String choiceKey;
  final String dateKey;

  /// Kayıt gider yazıyorsa (kart harcaması) taraf alanı çizilir; kart ödemesi
  /// para taşır, taraf taşımaz.
  final bool asksScope;

  /// Kartın etiketi: iki tarafa açık kategoride seçimin ön değeri.
  final TransactionScope? sourceScope;
  final String? sourceName;

  /// Tarih alanına önceden yazılacak `yyyy-MM-dd` değeri.
  final String? presetDate;

  /// Tutar alanına önceden yazılacak, kayıpsız biçimdeki değer.
  ///
  /// Alan kilitlenmiyor: ekstrenin tamamı yerine bir kısmını ödemek geçerli
  /// bir davranış, önerilen tutarı dayatmak kullanıcıyı yanlış rakama
  /// zorlardı.
  final String? presetAmount;

  /// Açıklama alanına önceden yazılacak ad (dekontta okunan karşı taraf).
  final String? presetDescription;

  @override
  State<_ActivityDialog> createState() => _ActivityDialogState();
}

class _ActivityDialogState extends State<_ActivityDialog> {
  final key = GlobalKey<FormState>();
  late final TextEditingController amount = TextEditingController(
    text: widget.presetAmount == null
        ? ''
        : MoneyText.editable(widget.presetAmount!),
  );
  late final TextEditingController description = TextEditingController(
    text: widget.presetDescription ?? '',
  );
  String? choice;
  TransactionScope? explicitScope;
  bool scopeMissing = false;
  late String date = widget.presetDate ?? _dateText(DateTime.now());
  @override
  void dispose() {
    amount.dispose();
    description.dispose();
    super.dispose();
  }

  bool get _showScope =>
      widget.asksScope &&
      (context.read<ScopeController?>()?.isVisible ?? false);

  TransactionScope? get _categoryScope => widget.choices
      .where((item) => item.id == choice)
      .firstOrNull
      ?.defaultScope;

  @override
  Widget build(BuildContext context) => Form(
    key: key,
    child: AppFormSheet<Map<String, Object?>>(
      title: widget.title,
      submitLabel: 'Kaydet',
      onSubmit: () async {
        if (!(key.currentState?.validate() ?? false)) return null;
        final scope = previewResolvedScope(
          explicit: explicitScope,
          source: widget.sourceScope,
          category: _categoryScope,
        );
        if (_showScope && scope == null) {
          setState(() => scopeMissing = true);
          return null;
        }
        return {
          widget.choiceKey: choice,
          if (_showScope) 'scope': scope?.apiValue,
          'amount': _money(amount.text),
          'currency': 'TRY',
          widget.dateKey: date,
          'description': _nullable(description.text),
        };
      },
      children: [
        AppFormField(
          child: _choice(
            widget.choices,
            widget.choiceLabel,
            choice,
            (v) => setState(() {
              choice = v;
              scopeMissing = false;
            }),
          ),
        ),
        if (_showScope && choice != null)
          AppFormField(
            child: AppScopeSection(
              explicit: explicitScope,
              source: widget.sourceScope,
              category: _categoryScope,
              sourceName: widget.sourceName,
              errorText: scopeMissing ? 'Bu kayıt için kapsam seçin.' : null,
              onChanged: (value) => setState(() {
                explicitScope = value;
                scopeMissing = false;
              }),
            ),
          ),
        AppFormField(
          child: TextFormField(
            controller: amount,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: const InputDecoration(
              labelText: 'Tutar',
              suffixText: 'TRY',
            ),
            validator: _moneyError,
          ),
        ),
        AppFormField(
          child: AppDateField(
            label: 'Tarih',
            value: date,
            onChanged: (value) => setState(() => date = value),
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: description,
            hintLocales: const [Locale('tr', 'TR')],
            decoration: const InputDecoration(labelText: 'Açıklama'),
          ),
        ),
      ],
    ),
  );
}

/// Taksitli kart harcaması.
///
/// Kartın içinden açıldığı için kart sorulmaz: `fixedCardId` doluyken seçici
/// hiç çizilmez. Zaten bir kartın sayfasındayken "hangi kart" diye sormak,
/// kullanıcının verdiği bilgiyi geri istemektir — ve yanlış kartı seçmesine
/// izin verirdi.
class _PlanDialog extends StatefulWidget {
  const _PlanDialog({
    required this.cards,
    required this.categories,
    this.fixedCardId,
    this.fixedCardScope,
    this.prefill,
  });
  final List<CreditCardItem> cards;
  final List<FinanceChoice> categories;
  final String? fixedCardId;

  /// [fixedCardId] kartının etiketi: iki tarafa açık kategoride ön değer.
  final TransactionScope? fixedCardScope;

  /// Taksitli fişten gelen öneriler; hepsi değiştirilebilir.
  final InstallmentPrefill? prefill;
  @override
  State<_PlanDialog> createState() => _PlanDialogState();
}

class _PlanDialogState extends State<_PlanDialog> {
  final key = GlobalKey<FormState>();
  final total = TextEditingController();
  final count = TextEditingController(text: '2');
  final description = TextEditingController();
  String? cardId;
  String? categoryId;
  TransactionScope? explicitScope;
  bool scopeMissing = false;
  String date = _dateText(DateTime.now());

  @override
  void initState() {
    super.initState();
    cardId = widget.fixedCardId;
    _applyPrefill();
  }

  bool get _showScope => context.read<ScopeController?>()?.isVisible ?? false;

  TransactionScope? get _categoryScope => widget.categories
      .where((item) => item.id == categoryId)
      .firstOrNull
      ?.defaultScope;

  TransactionScope? get _cardScope => widget.fixedCardId != null
      ? widget.fixedCardScope
      : widget.cards
            .where((card) => card.id == cardId)
            .firstOrNull
            ?.defaultScope;

  /// Fişin söylediğini yazar. **Taksit tutarı hesaplanmıyor**: forma toplam
  /// giriliyor, bölmeyi sunucu yapıyor — istemci finansal toplamı ikinci kez
  /// hesaplamaz.
  void _applyPrefill() {
    final prefill = widget.prefill;
    if (prefill == null || prefill.isEmpty) return;
    total.text = MoneyText.editable(prefill.totalAmount!);
    count.text = prefill.installmentCount!.toString();
    if (prefill.firstInstallmentDate case final value?) date = value;
    if (prefill.description case final value?) description.text = value;
    if (prefill.categoryId case final value?) categoryId = value;
  }

  @override
  void dispose() {
    total.dispose();
    count.dispose();
    description.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: key,
    child: AppFormSheet<Map<String, Object?>>(
      title: 'Yeni taksit planı',
      description:
          'Plan yalnız niyettir: yalnız gerçekleşen taksit kart harcaması '
          've gider üretir.',
      submitLabel: 'Kaydet',
      onSubmit: () async {
        if (!(key.currentState?.validate() ?? false)) return null;
        // Plan tarafını kurulurken alır; gerçekleşen her taksit onu taşır.
        final scope = previewResolvedScope(
          explicit: explicitScope,
          source: _cardScope,
          category: _categoryScope,
        );
        if (_showScope && scope == null) {
          setState(() => scopeMissing = true);
          return null;
        }
        return {
          'creditCardId': cardId,
          'categoryId': categoryId,
          if (_showScope) 'scope': scope?.apiValue,
          'clientRequestId': _newGuid(),
          'totalAmount': _money(total.text),
          'currency': 'TRY',
          'installmentCount': int.parse(count.text),
          'firstInstallmentDate': date,
          'description': _nullable(description.text),
        };
      },
      children: [
        if (widget.fixedCardId == null)
          AppFormField(
            child: _choice(
              widget.cards
                  .map((c) => FinanceChoice(id: c.id, name: c.name))
                  .toList(),
              'Kart',
              cardId,
              (v) => setState(() => cardId = v),
            ),
          ),
        AppFormField(
          child: _choice(
            widget.categories,
            'Gider kategorisi',
            categoryId,
            (v) => setState(() {
              categoryId = v;
              scopeMissing = false;
            }),
          ),
        ),
        if (_showScope && categoryId != null)
          AppFormField(
            child: AppScopeSection(
              explicit: explicitScope,
              source: _cardScope,
              category: _categoryScope,
              errorText: scopeMissing ? 'Bu plan için kapsam seçin.' : null,
              onChanged: (value) => setState(() {
                explicitScope = value;
                scopeMissing = false;
              }),
            ),
          ),
        AppFormField(
          child: TextFormField(
            controller: total,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: const InputDecoration(
              labelText: 'Toplam',
              suffixText: 'TRY',
            ),
            validator: _moneyError,
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: count,
            decoration: const InputDecoration(
              labelText: 'Taksit sayısı (2–60)',
            ),
            keyboardType: TextInputType.number,
            validator: (v) {
              final n = int.tryParse(v ?? '');
              return n == null || n < 2 || n > 60
                  ? '2–60 arasında sayı girin.'
                  : null;
            },
          ),
        ),
        AppFormField(
          child: AppDateField(
            label: 'İlk taksit tarihi',
            value: date,
            onChanged: (value) => setState(() => date = value),
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: description,
            hintLocales: const [Locale('tr', 'TR')],
            decoration: const InputDecoration(labelText: 'Açıklama'),
          ),
        ),
      ],
    ),
  );
}

/// Ekstre penceresi.
///
/// Eski hâli önce bir form soruyordu (yıl, ay, durum tarihi) ve sonra altı
/// satır düz metin gösteriyordu. İki kusuru vardı: kimse ekstresini yıl ve ay
/// yazarak aramıyor, ve satırlar **toplanmıyordu** — kesime kadar yapılan
/// ödemeler API'den geldiği hâlde ekranda yoktu, dolayısıyla
/// `önceki devir + dönem harcaması` ile `ekstre borcu` tutmuyor, kullanıcı
/// rakamları doğrulayamıyordu. Burada hesap açıkça yazılıyor.
///
/// Pencere kapanırken bir tutar döndürürse çağıran taraf ödeme panelini o
/// tutarla açar; ödeme buradan yürütülmüyor çünkü ekstre kalıcı bir kayıt
/// değil, ödemenin bağlanacağı bir satır yok.
class _StatementSheet extends StatefulWidget {
  const _StatementSheet({required this.card, required this.controller});
  final CreditCardItem card;
  final FinanceController controller;
  @override
  State<_StatementSheet> createState() => _StatementSheetState();
}

class _StatementSheetState extends State<_StatementSheet> {
  late final DateTime _today = DateTime.now();
  late DateTime _period = _latestClosedPeriod();
  late Future<CardStatement> _statement = _load();

  /// Kesim tarihi geçmiş en son dönem.
  ///
  /// Henüz kapanmamış dönem gösterilemez: sunucu kesim tarihinden önceki bir
  /// ekstre isteğini reddediyor, çünkü ortada daha ekstre yok.
  DateTime _latestClosedPeriod() {
    final closing = DateTime(
      _today.year,
      _today.month,
      widget.card.statementClosingDay,
    );
    return _today.isBefore(closing)
        ? DateTime(_today.year, _today.month - 1)
        : DateTime(_today.year, _today.month);
  }

  Future<CardStatement> _load() => widget.controller.loadStatement(
    widget.card.id,
    _period.year,
    _period.month,
    _dateText(_today),
  );

  bool get _canGoForward => _period.isBefore(_latestClosedPeriod());

  void _shift(int months) => setState(() {
    _period = DateTime(_period.year, _period.month + months);
    _statement = _load();
  });

  @override
  Widget build(BuildContext context) => AlertDialog(
    title: const Text('Ekstre'),
    content: SizedBox(
      width: 420,
      child: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          mainAxisSize: MainAxisSize.min,
          children: [
            _periodNavigator(),
            const Divider(height: AppSpacing.large),
            FutureBuilder<CardStatement>(
              future: _statement,
              builder: (context, snapshot) {
                if (snapshot.connectionState != ConnectionState.done) {
                  return const AppLoadingView(message: 'Ekstre hesaplanıyor');
                }
                if (snapshot.hasError) {
                  return const AppErrorView(
                    message: 'Bu dönemin ekstresi hesaplanamadı.',
                  );
                }
                return _content(snapshot.data!);
              },
            ),
          ],
        ),
      ),
    ),
    actions: [
      TextButton(
        onPressed: () => Navigator.pop(context),
        child: const Text('Kapat'),
      ),
    ],
  );

  Widget _periodNavigator() => Row(
    children: [
      IconButton(
        onPressed: () => _shift(-1),
        icon: const Icon(Icons.chevron_left),
        tooltip: 'Önceki dönem',
      ),
      Expanded(
        child: Text(
          DateText.monthYear(_period.year, _period.month),
          textAlign: TextAlign.center,
          style: Theme.of(context).textTheme.titleMedium,
        ),
      ),
      IconButton(
        // İleri, en son kapanmış dönemde durur; ötesi henüz kesilmedi.
        onPressed: _canGoForward ? () => _shift(1) : null,
        icon: const Icon(Icons.chevron_right),
        tooltip: 'Sonraki dönem',
      ),
    ],
  );

  Widget _content(CardStatement value) {
    final theme = Theme.of(context);
    final muted = AppSurfaces.of(context).inkMuted;
    final currency = value.currency;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      mainAxisSize: MainAxisSize.min,
      children: [
        Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: Text(
                'Kalan borç',
                style: theme.textTheme.labelMedium?.copyWith(color: muted),
              ),
            ),
            _statusChip(value),
          ],
        ),
        const SizedBox(height: AppSpacing.xSmall),
        AppMoneyText(
          amount: value.remainingBalance,
          currency: currency,
          effect: value.isPaid ? null : AppMoneyEffect.expense,
          style: AppTypography.money(theme.textTheme.headlineMedium!),
        ),
        const SizedBox(height: AppSpacing.xSmall),
        Text(
          '${DateText.dayMonth(value.periodStart)} – '
          '${DateText.dayMonth(value.closingDate)} dönemi',
          style: theme.textTheme.bodySmall?.copyWith(color: muted),
        ),
        const SizedBox(height: AppSpacing.large),

        // Hesap açıkça yazılıyor ki kullanıcı ekstreyi doğrulayabilsin:
        // her satır bir öncekiyle toplanıp ara toplamı veriyor.
        Text(
          'HESAP ÖZETİ',
          style: theme.textTheme.labelSmall?.copyWith(color: muted),
        ),
        const SizedBox(height: AppSpacing.small),
        _amountRow('Önceki devir', value.previousBalance, currency),
        _amountRow('Dönem harcaması', value.periodCharges, currency, sign: '+'),
        _amountRow(
          'Kesime kadar ödeme',
          value.paymentsThroughClosing,
          currency,
          sign: '−',
        ),
        const Divider(),
        _amountRow(
          'Ekstre borcu',
          value.statementBalance,
          currency,
          emphasised: true,
        ),
        _amountRow(
          'Kesimden sonra ödeme',
          value.paymentsAfterClosing,
          currency,
          sign: '−',
        ),
        const Divider(),
        _amountRow('Kalan', value.remainingBalance, currency, emphasised: true),
        const SizedBox(height: AppSpacing.small),
        _amountRow(
          'Asgari ödeme (%${MoneyText.percent(value.minimumPaymentRate)})',
          value.remainingMinimumPayment,
          currency,
        ),
        _amountRow(
          'Son ödeme tarihi',
          null,
          currency,
          text: DateText.dayMonthYear(value.dueDate),
        ),
        if (!value.isPaid) ...[
          const SizedBox(height: AppSpacing.large),
          _payActions(value),
        ],
      ],
    );
  }

  Widget _payActions(CardStatement value) {
    final minimum = value.remainingMinimumPayment;
    final hasSeparateMinimum =
        minimum != value.remainingBalance && minimum != '0.0000';
    return Wrap(
      spacing: AppSpacing.small,
      runSpacing: AppSpacing.small,
      children: [
        FilledButton.icon(
          onPressed: () => Navigator.pop(context, value.remainingBalance),
          icon: const Icon(Icons.payments_outlined),
          label: const Text('Ekstreyi öde'),
        ),
        if (hasSeparateMinimum)
          OutlinedButton.icon(
            onPressed: () => Navigator.pop(context, minimum),
            icon: const Icon(Icons.pending_actions_outlined),
            label: const Text('Asgariyi öde'),
          ),
      ],
    );
  }

  Widget _statusChip(CardStatement value) => AppStatusChip(
    label: value.isPaid
        ? 'Ödendi'
        : value.isOverdue
        ? 'Gecikmiş'
        : 'Açık',
    icon: value.isPaid
        ? Icons.check_circle_outline
        : value.isOverdue
        ? Icons.warning_amber_outlined
        : Icons.schedule_outlined,
    tone: value.isPaid
        ? AppStatusTone.income
        : value.isOverdue
        ? AppStatusTone.expense
        : AppStatusTone.planned,
  );

  /// Hesap özetinin bir satırı: solda ne olduğu, sağda tutarı.
  ///
  /// [sign] yalnız gösterim: tutarın işaretini değil, o satırın toplama nasıl
  /// girdiğini anlatıyor. Sunucu bütün bileşenleri pozitif gönderiyor.
  Widget _amountRow(
    String label,
    String? amount,
    String currency, {
    String? sign,
    String? text,
    bool emphasised = false,
  }) {
    final theme = Theme.of(context);
    final style = emphasised
        ? theme.textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.w600)
        : theme.textTheme.bodyMedium;
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.xSmall),
      child: Row(
        children: [
          Expanded(child: Text(label, style: style)),
          const SizedBox(width: AppSpacing.small),
          Flexible(
            child: Text(
              text ?? _signed(sign, amount!, currency),
              textAlign: TextAlign.end,
              style: style,
            ),
          ),
        ],
      ),
    );
  }

  String _signed(String? sign, String amount, String currency) {
    final formatted = MoneyText.format(amount, currency);
    return sign == null ? formatted : '$sign $formatted';
  }
}

Widget _choice(
  List<FinanceChoice> choices,
  String label,
  String? value,
  ValueChanged<String?> changed,
) => DropdownButtonFormField<String>(
  isExpanded: true,
  initialValue: value,
  decoration: InputDecoration(labelText: label),
  items: choices
      .map((item) => DropdownMenuItem(value: item.id, child: Text(item.name)))
      .toList(),
  onChanged: changed,
  validator: (selected) => selected == null ? '$label seçin.' : null,
);

String _choiceName(List<FinanceChoice> choices, String id) =>
    choices
        .where((item) => item.id == id)
        .map((item) => item.name)
        .firstOrNull ??
    'Hesap';
String? _moneyError(String? value) {
  final normalized = _money(value ?? '');
  final parsed = double.tryParse(normalized);
  return parsed == null ||
          parsed <= 0 ||
          !RegExp(r'^\d+(\.\d{1,4})?$').hasMatch(normalized)
      ? 'Pozitif tutar girin (en fazla 4 ondalık).'
      : null;
}

String? _dayError(String? value) {
  final day = int.tryParse(value ?? '');
  return day == null || day < 1 || day > 28 ? '1–28 arasında gün girin.' : null;
}

/// Asgari ödeme oranı doğrulaması: yüzde, 0–100 arası.
String? _rateError(String? value) {
  final parsed = double.tryParse(_money(value ?? ''));
  return parsed == null || parsed < 0 || parsed > 100
      ? '0 ile 100 arasında bir oran girin.'
      : null;
}

String _money(String value) => value.trim().replaceAll(',', '.');

/// Kart satırının borç yarısı.
///
/// Alacaklı kartta "Borç −₺500,00" yazmak kullanıcıya borcu varmış gibi
/// okunuyordu; cümle yönü kendisi söylüyor, eksi işaretine gerek yok
/// (Aşama 06 Grup 5).
String _debtSentence(CreditCardItem card) =>
    MoneyText.isNegative(card.currentDebt)
    ? 'Kartınızda '
          '${MoneyText.format(MoneyText.unsigned(card.currentDebt), card.currency)} '
          'alacağınız var'
    : 'Borç ${MoneyText.format(card.currentDebt, card.currency)}';
String? _nullable(String value) => value.trim().isEmpty ? null : value.trim();
String _dateText(DateTime value) =>
    '${value.year.toString().padLeft(4, '0')}-${value.month.toString().padLeft(2, '0')}-${value.day.toString().padLeft(2, '0')}';

String _newGuid() {
  final random = Random.secure();
  String hex(int length) =>
      List.generate(length, (_) => random.nextInt(16).toRadixString(16)).join();
  return '${hex(8)}-${hex(4)}-4${hex(3)}-${['8', '9', 'a', 'b'][random.nextInt(4)]}${hex(3)}-${hex(12)}';
}

/// Geçmiş listelerinin dönem seçici.
///
/// Varsayılan son üç ay. Seçenek sunulmasının nedeni performans değil
/// dürüstlük: sınır konduğunu kullanıcı görmeli ve gerektiğinde
/// genişletebilmeli. "Tümü" tarih sınırını kaldırır ama sunucudaki satır
/// tavanını kaldırmaz; kırpma olursa liste bunu ayrıca söyler.
class _PeriodSelector extends StatelessWidget {
  const _PeriodSelector({required this.selected, required this.onSelected});

  final HistoryPeriod selected;
  final ValueChanged<HistoryPeriod> onSelected;

  /// Satırın tamamını kaplar; parçalar eşit bölüşür.
  ///
  /// İlk yazımda bir `Align` içinde sola yaslıydı ve sağında ölü bir boşluk
  /// bırakıyordu. `Align`'ı kaldırmak tek başına yetmedi: `SegmentedButton`
  /// varsayılan olarak içeriği kadar genişleyip ortalanıyor, yani iki yanda
  /// bu kez simetrik boşluk kalıyordu. Genişliğin açıkça verilmesi gerekiyor.
  ///
  /// Tam genişlik bir tercih: bu çubuk altındaki listenin tamamını yönetiyor,
  /// kart kenarlarına oturması bunu yerleşimle söylüyor.
  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.fromLTRB(
      AppSpacing.medium,
      AppSpacing.small,
      AppSpacing.medium,
      AppSpacing.small,
    ),
    child: SizedBox(
      width: double.infinity,
      child: SegmentedButton<HistoryPeriod>(
        segments: [
          for (final period in HistoryPeriod.values)
            ButtonSegment(value: period, label: Text(period.label)),
        ],
        selected: {selected},
        onSelectionChanged: (values) => onSelected(values.first),
      ),
    ),
  );
}

/// Listenin kırpıldığını söyleyen satır.
///
/// Sessizce kırpmak, ekranı yanlış bir tamlık iddiasında bırakırdı: kullanıcı
/// gördüğünü "hepsi" sanar ve eksik bir geçmişe göre karar verir.
class _TruncationNotice extends StatelessWidget {
  const _TruncationNotice();

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return Padding(
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.medium,
        vertical: AppSpacing.small,
      ),
      child: Row(
        children: [
          Icon(Icons.info_outline, size: 18, color: surfaces.inkMuted),
          const SizedBox(width: AppSpacing.small),
          Expanded(
            child: Text(
              'Bu dönemde gösterilenden daha fazla kayıt var. '
              'Daha eskisini görmek için dönemi daraltın.',
              style: Theme.of(
                context,
              ).textTheme.bodySmall?.copyWith(color: surfaces.inkMuted),
            ),
          ),
        ],
      ),
    );
  }
}

/// Bir transferin ayrıntısı.
///
/// Transferin anlatacağı tek şey var: **bu para nereden nereye taşındı ve
/// gelir/gider toplamı değişmedi.** Panel bunu üç kademede söylüyor — tutar
/// manşet, iki uç ok ile, gerisi ikincil satırlar — çünkü etiketli metin
/// satırları (`Tarih:`, `Durum:`) hepsini aynı kademeye indiriyordu.
class _TransferDetail extends StatelessWidget {
  const _TransferDetail({
    required this.transfer,
    required this.sourceName,
    required this.destinationName,
  });

  final TransferItem transfer;
  final String sourceName;
  final String destinationName;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);

    return SafeArea(
      child: Padding(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          0,
          AppSpacing.medium,
          AppSpacing.medium,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text('Transfer ayrıntısı', style: theme.textTheme.titleLarge),
            const SizedBox(height: AppSpacing.medium),
            // Tutar manşet. Transferde rol yok: para el değiştirmedi, yer
            // değiştirdi — gelir de gider de değil, o yüzden `effect`
            // verilmiyor ve tutar varsayılan mürekkep renginde kalıyor.
            AppMoneyText(
              amount: transfer.amount,
              currency: transfer.currency,
              style: theme.textTheme.headlineSmall,
              isCancelled: transfer.isCancelled,
            ),
            const SizedBox(height: AppSpacing.medium),
            // İki uç ve aralarındaki ok: transferin bütün hikâyesi bu satır.
            Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSpacing.medium,
                vertical: AppSpacing.small + AppSpacing.xSmall,
              ),
              decoration: BoxDecoration(
                color: surfaces.cardMuted,
                borderRadius: BorderRadius.circular(AppRadius.field),
              ),
              child: Row(
                children: [
                  Expanded(
                    child: _Endpoint(label: 'Çıkan hesap', name: sourceName),
                  ),
                  Padding(
                    padding: const EdgeInsets.symmetric(
                      horizontal: AppSpacing.small,
                    ),
                    child: Icon(
                      Icons.arrow_forward,
                      size: 18,
                      color: surfaces.inkMuted,
                    ),
                  ),
                  Expanded(
                    child: _Endpoint(
                      label: 'Giren hesap',
                      name: destinationName,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: AppSpacing.medium),
            Wrap(
              spacing: AppSpacing.small,
              runSpacing: AppSpacing.small,
              crossAxisAlignment: WrapCrossAlignment.center,
              children: [
                AppStatusChip(
                  label: transfer.isCancelled ? 'İptal edildi' : 'Aktif',
                  icon: transfer.isCancelled ? Icons.block : Icons.swap_horiz,
                  tone: transfer.isCancelled
                      ? AppStatusTone.cancelled
                      : AppStatusTone.neutral,
                ),
                Text(
                  DateText.dayMonthYear(transfer.date),
                  style: theme.textTheme.bodyMedium,
                ),
              ],
            ),
            if (transfer.description case final description?) ...[
              const SizedBox(height: AppSpacing.small),
              Text(description, style: theme.textTheme.bodyMedium),
            ],
            const SizedBox(height: AppSpacing.medium),
            // Kuralın kendisi yazılı: transferin raporlara etkisi yok ve bu,
            // kullanıcının en çok yanıldığı yer.
            Text(
              'Hesaplar arası taşınan para gelir veya gider sayılmaz.',
              style: theme.textTheme.bodySmall?.copyWith(
                color: surfaces.inkMuted,
              ),
            ),
            const SizedBox(height: AppSpacing.medium),
            if (!transfer.isCancelled)
              OutlinedButton.icon(
                onPressed: () => Navigator.of(context).pop(true),
                icon: const Icon(Icons.block),
                label: const Text('Transferi iptal et'),
              ),
            TextButton(
              onPressed: () => Navigator.of(context).pop(false),
              child: const Text('Kapat'),
            ),
          ],
        ),
      ),
    );
  }
}

/// Transferin bir ucu: ne olduğu üstte küçük, adı altta.
class _Endpoint extends StatelessWidget {
  const _Endpoint({required this.label, required this.name});

  final String label;
  final String name;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisSize: MainAxisSize.min,
      children: [
        Text(
          label,
          style: theme.textTheme.labelSmall?.copyWith(
            color: AppSurfaces.of(context).inkMuted,
          ),
        ),
        const SizedBox(height: AppSpacing.xSmall),
        Text(
          name,
          style: theme.textTheme.titleSmall,
          maxLines: 2,
          overflow: TextOverflow.ellipsis,
        ),
      ],
    );
  }
}
