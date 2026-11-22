import '../../../core/formatters/money_math.dart';
import 'dart:async';
import 'dart:math';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_category_field.dart';
import '../../../core/widgets/app_first_use_view.dart';
import '../../../core/widgets/app_page_bar.dart';
import '../../categories/presentation/category_create_page.dart';
import '../../categories/presentation/recent_categories.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../activities/data/receipt_fee_writer.dart';
import '../data/finance_models.dart';
import '../data/finance_repository.dart';
import 'credit_card_tile.dart';
import 'card_design_widgets.dart';
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/widgets/app_adaptive_sheet.dart';
import 'finance_controller.dart';
import 'card_installments.dart';
import 'card_movements.dart';
import 'card_plan_widgets.dart';
import '../data/card_page_models.dart';
import '../../activities/data/activity_repository.dart';
import '../../activities/presentation/activity_controller.dart';
import '../../activities/presentation/show_activity_detail.dart';
import '../../day_close/data/day_close_repository.dart';
import '../../finance_pages/data/finance_page_models.dart';
import '../../finance_pages/data/finance_page_repository.dart';
import '../../pos/data/pos_repository.dart';
import 'transfer_prefill.dart';
import '../../../core/widgets/app_account_field.dart';
import '../../../features/accounts/presentation/account_directory.dart';

/// Kredi kartları listesi. Transfer ayrı sayfada ve ortak işlem ayrıntısındadır.
class FinancePage extends StatefulWidget {
  const FinancePage({
    required this.repository,
    this.financialDataChanges,
    this.embedded = false,
    this.cardPaymentPrefill,
    this.installmentPrefill,
    this.recordFee,
    this.activityRepository,
    this.pageRepository,
    this.posRepository,
    this.dayCloseRepository,
    super.key,
  });
  final FinanceRepositoryContract repository;
  final FinancialDataChanges? financialDataChanges;

  /// Kart hareketine dokununca ortak işlem ayrıntısı bunlarla açılır;
  /// boşsa satırlar dokunulmaz.
  final ActivityRepositoryContract? activityRepository;
  final FinancePageRepositoryContract? pageRepository;
  final PosRepositoryContract? posRepository;
  final DayCloseRepositoryContract? dayCloseRepository;

  /// Bir sekmenin içinde mi çiziliyor; gömülüyken kendi başlık çubuğunu
  /// çizmez.
  final bool embedded;

  /// Dolu gelirse kart listesinin başında "hangi kart?" notu çıkar ve seçilen
  /// kartın ödeme formu bu tutarla açılır. Kartı dekont söylemez, kullanıcı
  /// seçer.
  final CardPaymentPrefill? cardPaymentPrefill;

  /// Dolu gelirse kart listesinin başında "hangi kart?" notu çıkar ve seçilen
  /// kartın taksit planı formu bu önerilerle açılır. Kartı fiş söylemez.
  final InstallmentPrefill? installmentPrefill;

  /// Dekonttaki işlem ücretini yazan dar imza.
  ///
  /// Kart ödemesi borç kapatır; dekontun işlem ücreti ayrı giderdir
  /// ve kendi gider kaydını hak eder. Ekran depoları tanımadığı için yazma
  /// dışarıdan geliyor (`uploadAttachment` ile aynı desen).
  final ReceiptFeeRecorder? recordFee;

  @override
  State<FinancePage> createState() => _FinancePageState();
}

class _FinancePageState extends State<FinancePage> {
  late final FinanceController controller;
  ActivityController? _details;

  /// Kartın verisi değiştikçe artar; Hareketler bölümü yeniden okunur.
  int _revision = 0;

  @override
  void initState() {
    super.initState();
    controller = FinanceController(
      widget.repository,
      financialDataChanges: widget.financialDataChanges,
    )..addListener(_changed);
    controller.load();
    if (widget.activityRepository != null) {
      _details = ActivityController(
        widget.activityRepository!,
        financialDataChanges: widget.financialDataChanges,
        observeChanges: false,
      );
    }
  }

  @override
  void dispose() {
    controller.removeListener(_changed);
    controller.dispose();
    _details?.dispose();
    _pages.dispose();
    super.dispose();
  }

  /// Kart hareketinin ortak ayrıntısı; iptal edilirse kart ve hareketler
  /// tazelenir.
  Future<bool> _openRow(FinancePageActivity row) async {
    final details = _details;
    if (details == null) return false;
    final changed = await showActivityDetail(
      Navigator.of(context).context,
      activity: row.activity,
      controller: details,
      pageRepository: widget.pageRepository,
      posRepository: widget.posRepository,
      dayCloseRepository: widget.dayCloseRepository,
      changes: widget.financialDataChanges,
    );
    // Hiçbir şey değişmediyse sayfa yerinde kalır; baştan okumak kaydırmayı
    // başa atıyordu.
    if (!changed || !mounted) return changed;
    setState(() => _revision++);
    await controller.load();
    return changed;
  }

  void _changed() {
    if (!mounted) return;
    _showSuccess(context, controller);
    setState(() {});
  }

  /// Kartlar yan yana kayar; komşu kartın kenarı görünür.
  final PageController _pages = PageController(viewportFraction: 0.9);
  int _index = 0;

  /// Fişten ya da dekonttan gelindiğinde kullanıcının dokunarak seçtiği kart.
  /// Öneriyle açılan form yalnız bu kartta açılır; belge kartı söylemez.
  String? _chosenCardId;

  @override
  Widget build(BuildContext context) {
    final body = _body();
    if (widget.embedded) return body;
    return Scaffold(
      appBar: AppPageBar(
        title: const Text('Kredi kartları'),
        // Deneme: ikon 4 dp içeride (Hareketler sayfasıyla birlikte).
        actions: [
          // Kart yokken ekleme boş durumun düğmesindedir; aynı eylem iki
          // kez görünmez.
          if (controller.snapshot?.cards.isNotEmpty ?? false)
            IconButton(
              tooltip: 'Kart ekle',
              onPressed: controller.isSubmitting ? null : _addCard,
              icon: const Icon(Icons.add),
            ),
        ],
      ),
      body: body,
    );
  }

  Widget _body() {
    if (controller.unauthorized) return const AppUnauthorizedView();
    if (controller.isLoading && controller.snapshot == null) {
      return const AppLoadingView(message: 'Kartlar yükleniyor');
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
        if (controller.errorMessage != null)
          MaterialBanner(
            content: Text(controller.errorMessage!),
            actions: [
              TextButton(
                onPressed: controller.clearMessage,
                child: const Text('Kapat'),
              ),
            ],
          ),
        Expanded(
          child: Align(
            alignment: Alignment.topCenter,
            child: ConstrainedBox(
              constraints: const BoxConstraints(
                maxWidth: AppBreakpoints.contentMaxWidth,
              ),
              child: _cards(snapshot),
            ),
          ),
        ),
      ],
    );
  }

  Widget _cards(FinanceSnapshot snapshot) {
    final cards = snapshot.cards;
    final index = cards.isEmpty ? 0 : _index.clamp(0, cards.length - 1);
    final selected = cards.isEmpty ? null : cards[index];
    final hasPrefill =
        widget.installmentPrefill?.isEmpty == false ||
        widget.cardPaymentPrefill?.isEmpty == false;
    Widget padded(Widget child) => Padding(
      padding: const EdgeInsets.symmetric(horizontal: AppPageInsets.edge),
      child: child,
    );
    return RefreshIndicator(
      onRefresh: controller.load,
      child: ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: AppPageInsets.fullBleedList,
        children: [
          // Bir sekmenin içindeyken başlık çubuğu yoktur; ekleme gövdede durur.
          if (widget.embedded && cards.isNotEmpty)
            padded(
              Align(
                alignment: Alignment.centerRight,
                child: TextButton.icon(
                  onPressed: controller.isSubmitting ? null : _addCard,
                  icon: const Icon(Icons.add),
                  label: const Text('Kart ekle'),
                ),
              ),
            ),
          // Dekonttan gelindiyse sayfa bir seçim ekranına dönüşür: belge hangi
          // kartın borcunun kapatıldığını söylemez ve yanlış kart başka bir
          // kartın borcunu azaltırdı.
          if (widget.installmentPrefill?.isEmpty == false)
            padded(
              Padding(
                padding: const EdgeInsets.only(bottom: AppGaps.notice),
                child: AppInlineNotice(
                  icon: Icons.credit_card,
                  message:
                      'Fişteki ${widget.installmentPrefill!.installmentCount} '
                      'taksitli alışveriş için karta dokunun.',
                ),
              ),
            ),
          if (widget.cardPaymentPrefill?.isEmpty == false)
            padded(
              Padding(
                padding: const EdgeInsets.only(bottom: AppGaps.notice),
                child: AppInlineNotice(
                  icon: Icons.credit_card,
                  message: widget.cardPaymentPrefill!.amount == null
                      ? 'Dekonttaki ödeme için karta dokunun.'
                      : 'Dekonttaki '
                            '${MoneyText.format(widget.cardPaymentPrefill!.amount!, 'TRY')} '
                            'ödeme için karta dokunun.',
                ),
              ),
            ),
          if (selected == null)
            padded(
              AppFirstUseView(
                preview: AppEmptyPreview.card,
                icon: Icons.credit_card,
                title: 'Henüz kredi kartı yok',
                message: 'Limit ve ekstre günleriyle ilk kartınızı ekleyin.',
                actionLabel: 'Kart ekle',
                onAction: controller.isSubmitting ? null : _addCard,
              ),
            )
          else ...[
            SizedBox(
              height: cards
                  .map(
                    (card) => CreditCardTile.heightOf(
                      context,
                      card: card,
                      width: _cardWidth(context),
                    ),
                  )
                  .reduce(max),
              child: PageView.builder(
                controller: _pages,
                itemCount: cards.length,
                onPageChanged: (value) => setState(() => _index = value),
                itemBuilder: (context, i) => Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.xSmall,
                  ),
                  child: CreditCardTile(
                    card: cards[i],
                    fill: true,
                    statementBalance:
                        controller.statements[cards[i].id]?.remainingBalance,
                    onTap: () {
                      if (i != index) {
                        _pages.animateToPage(
                          i,
                          duration: const Duration(milliseconds: 250),
                          curve: Curves.easeOut,
                        );
                      }
                      if (hasPrefill) {
                        setState(() => _chosenCardId = cards[i].id);
                      }
                    },
                  ),
                ),
              ),
            ),
            if (cards.length > 1) ...[
              const SizedBox(height: AppSpacing.compact),
              _PageDots(count: cards.length, index: index),
            ],
            const SizedBox(height: AppGaps.section),
            CreditCardDetailPage(
              // Kart değişince bölümler o kartın verisiyle baştan kurulur;
              // öneriyle seçilince form o kartta açılır.
              key: ValueKey('${selected.id}-${_chosenCardId == selected.id}'),
              cardId: selected.id,
              controller: controller,
              asPanel: true,
              revision: _revision,
              onOpenRow: _details == null ? null : _openRow,
              paymentPrefill: _chosenCardId == selected.id
                  ? widget.cardPaymentPrefill
                  : null,
              installmentPrefill: _chosenCardId == selected.id
                  ? widget.installmentPrefill
                  : null,
              recordFee: widget.recordFee,
            ),
          ],
        ],
      ),
    );
  }

  /// Şeritteki kartın genişliği: sayfanın %90'ı, iki yanında 4 dp.
  static double _cardWidth(BuildContext context) =>
      min(MediaQuery.sizeOf(context).width, AppBreakpoints.contentMaxWidth) *
          .9 -
      AppSpacing.small;

  Future<void> _addCard() async {
    await AppFormSheet.show<Map<String, Object?>>(
      context: context,
      builder: (_) => _CardDialog(
        onSave: (input) => controller.refusalOf(
          controller.submit(
            () => controller.repository.createCard(input),
            'Kredi kartı oluşturuldu.',
          ),
        ),
      ),
    );
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
    this.asPanel = false,
    this.revision = 0,
    this.onOpenRow,
    this.openStatementPayment = false,
    this.openPlanId,
  });
  final String cardId;
  final FinanceController controller;

  /// Planlananlar'ın ekstre satırından gelindi: ödeme seçimi açılışta bir
  /// kez açılır, kapanınca sayfa Planlananlar'a döner.
  final bool openStatementPayment;

  /// Planlananlar'ın taksit satırından gelindi: bu plan açılışta bir kez
  /// açılır, kapanınca sayfa Planlananlar'a döner.
  final String? openPlanId;

  /// Kartın verisi dışarıda değiştiğinde artar (işlem ayrıntısında iptal).
  final int revision;

  /// Kart hareketine dokununca ortak ayrıntı; boşsa satırlar dokunulmaz.
  final OpenCardMovement? onOpenRow;

  /// Kredi kartlarım sayfasının içinde, seçili kartın altında mı çiziliyor.
  /// Öyleyse kendi sayfa iskeletini kurmaz; yalnız bölümleri döner.
  final bool asPanel;

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
  /// Kaydedilen her hareketle artar; Hareketler bölümü yeniden okunur.
  int _revision = 0;

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
    currentStatement = widget.controller.loadCurrentStatement(widget.cardId);

    _openPrefilledPayment();
    _openPrefilledPlan();
    _openFromPlanned();
  }

  /// Planlananlar'dan gelen istek **bir kez** kullanılır; kart listesi henüz
  /// yüklenmemişse yüklenince açılır (`_changed`).
  bool _plannedUsed = false;

  void _openFromPlanned() {
    if (_plannedUsed ||
        (!widget.openStatementPayment && widget.openPlanId == null)) {
      return;
    }
    WidgetsBinding.instance.addPostFrameCallback((_) async {
      final current = card;
      if (current == null || !mounted || _plannedUsed) return;
      _plannedUsed = true;
      if (widget.openStatementPayment) {
        await _choosePayment(current);
      } else {
        await _showPlan(widget.openPlanId!);
      }
      // Ödeme ya da plan bu sayfanın değil, Planlananlar'daki satırın işiydi;
      // panel kapanınca kullanıcı geldiği yere döner.
      if (mounted) Navigator.of(context).maybePop();
    });
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
    if (!mounted) return;
    // Panel olarak çizildiğinde bildirimi sayfa söyler; iki kez çıkmasın.
    if (!widget.asPanel) _showSuccess(context, widget.controller);
    setState(() {});
    _openFromPlanned();
  }

  CreditCardItem? get card => widget.controller.snapshot?.cards
      .where((item) => item.id == widget.cardId)
      .firstOrNull;

  @override
  Widget build(BuildContext context) {
    final current = card;
    if (widget.asPanel) {
      return current == null
          ? const AppErrorView(message: 'Kart artık bulunamıyor.')
          : _sections(current);
    }
    return Scaffold(
      appBar: AppPageBar(title: Text(current?.name ?? 'Kart detayı')),
      body: current == null && widget.controller.snapshot == null
          ? const AppLoadingView(message: 'Kart yükleniyor')
          : current == null
          ? const AppErrorView(message: 'Kart artık bulunamıyor.')
          : Column(
              children: [
                // Yazma sonucunu **bu** ekran söyler: reddedilen bir ödeme
                // sessizce kaybolmamalı.
                if (widget.controller.errorMessage != null)
                  MaterialBanner(
                    content: Text(widget.controller.errorMessage!),
                    actions: [
                      TextButton(
                        onPressed: widget.controller.clearMessage,
                        child: const Text('Kapat'),
                      ),
                    ],
                  ),
                Expanded(
                  child: ListView(
                    padding: AppPageInsets.list,
                    children: [_sections(current)],
                  ),
                ),
              ],
            ),
    );
  }

  /// Seçili kartın bölümleri, yukarıdan aşağı: işlemler, dönem borcu,
  /// taksitler, hareketler. Limit kartın yüzündedir (teslim §1).
  /// Hareketler tam genişlik bloktur; diğer bölümler 16 dp içeridedir.
  Widget _sections(CreditCardItem current) {
    Widget padded(Widget child) => Padding(
      padding: const EdgeInsets.symmetric(horizontal: AppPageInsets.edge),
      child: child,
    );
    final plans = _cardPlans(current);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      mainAxisSize: MainAxisSize.min,
      children: [
        if (!widget.asPanel) ...[
          padded(CreditCardTile(card: current)),
          const SizedBox(height: AppGaps.block),
        ],
        padded(_actions(current)),
        const SizedBox(height: AppGaps.section),
        padded(_summary(current)),
        if (plans.isNotEmpty) ...[
          const SizedBox(height: AppGaps.section),
          CardInstallmentStrip(plans: plans, onOpen: () => _openPlans(current)),
        ],
        const SizedBox(height: AppGaps.section),
        CardMovementsSection(
          repository: widget.controller.repository,
          cardId: current.id,
          revision: _revision + widget.revision,
          onOpenAll: () => _openMovements(current),
          onOpenRow: widget.onOpenRow == null ? null : _openRow,
        ),
      ],
    );
  }

  Future<bool> _openRow(FinancePageActivity row) async {
    final changed = await widget.onOpenRow!(row);
    if (changed && mounted) _reloadActivity();
    return changed;
  }

  Future<void> _openMovements(CreditCardItem card) async {
    // Hareketler sayfasında bir kayıt iptal edildiyse dönüşte kart tazelenir;
    // edilmediyse sayfa olduğu yerde kalır.
    var changed = false;
    final open = widget.onOpenRow;
    await Navigator.of(context).push(
      MaterialPageRoute<void>(
        builder: (_) => CardMovementsPage(
          repository: widget.controller.repository,
          card: card,
          onOpenRow: open == null
              ? null
              : (row) async {
                  final result = await open(row);
                  changed = changed || result;
                  return result;
                },
          showScope: context.read<ScopeController?>()?.isVisible ?? false,
        ),
      ),
    );
    if (changed && mounted) _reloadActivity();
  }

  Widget _summary(CreditCardItem card) =>
      AppCard(child: _statementHeadline(card));

  /// Özet kartının manşeti: şu an ödenmesi gereken ekstre.
  Widget _statementHeadline(CreditCardItem card) =>
      FutureBuilder<CardStatement?>(
        future: currentStatement,
        builder: (context, snapshot) {
          if (snapshot.connectionState != ConnectionState.done) {
            // Tazelenirken eski ekstre yerinde kalır; yükleme görünümü daha
            // kısa olduğu için sayfa boyu çöküp kaydırma yukarı kayıyordu.
            if (snapshot.hasData) {
              return _statementBanner(card, snapshot.data!);
            }
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

  /// Ekstresi olmayan ya da okunamayan kartta dönem borcu kartının aynı
  /// başlığı ve tek cümle.
  Widget _headlineNote(String message, {Widget? action}) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      Text(
        'Dönem borcu',
        style: Theme.of(context).textTheme.titleSmall?.copyWith(
          fontSize: AppTextSizes.compact,
          height: 1.3,
        ),
      ),
      const SizedBox(height: AppSpacing.xSmall),
      Text(
        message,
        style: Theme.of(context).textTheme.bodyMedium?.copyWith(
          color: AppSurfaces.of(context).inkMuted,
        ),
      ),
      ?action,
    ],
  );

  /// Dönem borcu kartı (teslim §2, DA): etiket solda, değer **sağa yaslı**
  /// aynı satırda; değerin üstündeki küçük satır durumu söyler. Ayırıcının
  /// üstü ve altı 12 dp. `Ödeme yap` düğmesinin boşluğu değişmedi.
  ///
  /// Durumlar (kullanıcı isteği, 13 Kasım 2026), hepsi aynı iskelette:
  /// - **Ödeme bekliyor:** günün üstünde `12 gün kaldı`; son üç günde
  ///   `3 gün kaldı` / `Yarın son gün` / `Bugün son gün` kırmızı.
  /// - **Gecikti:** `Ödeme tarihi geçti`; asgari ödenmediyse gün ve asgari
  ///   kırmızı, etiket `Kalan asgari ödeme`.
  /// - **Asgari ödendi:** asgari satırında yeşil `✓ Ödendi`.
  /// - **Kısmen ödendi:** tutarın altında `₺… ödendi · Ekstre ₺…`.
  /// - **Tamamı ödendi:** manşet ekstrenin tamamıdır (soluk), altında yeşil
  ///   `✓ Tamamı ödendi`; asgari satırı ve düğme yok.
  /// - **Bu dönemde borç yok:** ekstre sıfırsa tek cümle.
  ///
  /// Gün sayısı yalnız etikettir; ödeme durumu ve tutarlar sunucudandır.
  Widget _statementBanner(CreditCardItem card, CardStatement statement) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final surfaces = AppSurfaces.of(context);
    final minimumMet = !_positiveMoney(statement.remainingMinimumPayment);
    final overdue = statement.isOverdue;
    final late = overdue && !minimumMet;
    final empty = !_positiveMoney(statement.statementBalance);
    final settled = statement.isPaid || empty;
    final paidPart = _positiveMoney(statement.paymentsAfterClosing);
    final daysLeft = _daysUntil(statement.dueDate);
    final soon = !overdue && daysLeft <= 3;
    final valueStyle = theme.textTheme.titleSmall?.copyWith(
      fontSize: AppTextSizes.compact,
      fontWeight: FontWeight.w700,
      height: 20 / 15,
    );
    final noteStyle = theme.textTheme.bodySmall?.copyWith(
      fontSize: AppTextSizes.caption,
      height: 16 / 13,
      color: surfaces.inkMuted,
    );

    // Değer sağa yaslıdır; üstündeki durum notu da sağa yaslı ayrı satırdır.
    // Büyük yazıda değer etiketin yanına sığmaz; sağa yaslı alt satıra iner.
    Widget line(IconData icon, String label, Widget value, {Widget? note}) {
      // Değerler kartın kenarında değil, `Ödeme yap` düğmesinin yuvarlak
      // ucunun göze bittiği yerde biter (kullanıcı, 13 Kasım 2026: kenarla
      // aynı çizgide fazla sağda duruyordu).
      final trailing = Padding(
        padding: const EdgeInsets.only(right: AppSpacing.compact),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            if (note != null) ...[
              note,
              const SizedBox(height: AppSpacing.xxSmall),
            ],
            value,
          ],
        ),
      );
      // Değer kendi genişliğindedir ve sağa yaslıdır; dar ekranda satırın
      // en çok %60'ını alır ve kırılır, etiket kalanı kullanır.
      final head = LayoutBuilder(
        builder: (context, constraints) => Row(
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Icon(icon, size: 20, color: surfaces.inkMuted),
            const SizedBox(width: AppSpacing.step10),
            Expanded(
              child: Text(
                label,
                style: theme.textTheme.bodyMedium?.copyWith(height: 20 / 14),
              ),
            ),
            if (!context.usesLargeText) ...[
              const SizedBox(width: AppSpacing.small),
              ConstrainedBox(
                constraints: BoxConstraints(
                  maxWidth: constraints.maxWidth * 0.6,
                ),
                child: trailing,
              ),
            ],
          ],
        ),
      );
      if (!context.usesLargeText) return head;
      return Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          head,
          const SizedBox(height: AppSpacing.xSmall),
          Align(alignment: Alignment.centerRight, child: trailing),
        ],
      );
    }

    Widget done(String label) => Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(Icons.check_circle, size: 18, color: colors.income),
        const SizedBox(width: AppSpacing.xSmall),
        Flexible(
          child: Text(label, style: valueStyle?.copyWith(color: colors.income)),
        ),
      ],
    );

    final dueNote = overdue
        ? 'Ödeme tarihi geçti'
        : daysLeft <= 0
        ? 'Bugün son gün'
        : daysLeft == 1
        ? 'Yarın son gün'
        : '$daysLeft gün kaldı';
    final dueColor = late || soon ? colors.expense : null;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          '${DateText.monthYear(statement.year, statement.month)} dönem borcu',
          style: theme.textTheme.titleSmall?.copyWith(
            fontSize: AppTextSizes.compact,
            height: 1.3,
          ),
        ),
        const SizedBox(height: AppSpacing.xSmall),
        AppMoneyText(
          // Ödenmiş ekstrede kalan sıfırdır; manşet ödenen ekstreyi söyler.
          amount: statement.isPaid && !empty
              ? statement.statementBalance
              : statement.remainingBalance,
          currency: statement.currency,
          style: theme.textTheme.titleLarge?.copyWith(
            fontSize: AppTextSizes.amount,
            color: settled ? surfaces.inkMuted : null,
          ),
        ),
        if (empty) ...[
          const SizedBox(height: AppSpacing.xSmall),
          Text('Bu dönemde borç oluşmadı.', style: noteStyle),
        ] else if (statement.isPaid) ...[
          const SizedBox(height: AppSpacing.xSmall),
          done('Tamamı ödendi'),
        ] else if (paidPart) ...[
          const SizedBox(height: AppSpacing.xSmall),
          Text(
            '${MoneyText.format(statement.paymentsAfterClosing, statement.currency)} ödendi'
            ' · Ekstre ${MoneyText.format(statement.statementBalance, statement.currency)}',
            style: noteStyle,
          ),
        ],
        const SizedBox(height: AppSpacing.compact),
        line(
          Icons.calendar_today_outlined,
          'Son ödeme tarihi',
          Text(
            DateText.dayMonth(statement.dueDate),
            style: valueStyle?.copyWith(
              color: settled ? surfaces.inkMuted : dueColor,
            ),
          ),
          note: settled
              ? null
              : Text(
                  dueNote,
                  textAlign: TextAlign.end,
                  style: noteStyle?.copyWith(
                    color: dueColor ?? surfaces.inkMuted,
                  ),
                ),
        ),
        if (!settled) ...[
          const SizedBox(height: AppSpacing.compact),
          Divider(height: 1, color: surfaces.border),
          const SizedBox(height: AppSpacing.compact),
          line(
            Icons.receipt_long_outlined,
            late ? 'Kalan asgari ödeme' : 'Asgari ödeme',
            minimumMet
                ? done('Ödendi')
                : AppMoneyText(
                    amount: statement.remainingMinimumPayment,
                    currency: statement.currency,
                    size: AppMoneySize.row,
                    style: valueStyle?.copyWith(
                      color: late ? colors.expense : null,
                    ),
                  ),
          ),
          const SizedBox(height: AppGaps.block),
          FilledButton(
            onPressed: () => _choosePayment(card, statement: statement),
            child: const Text('Ödeme yap'),
          ),
        ],
      ],
    );
  }

  /// Son ödeme gününe kalan gün; geçmişse eksi. Yalnız etiket içindir.
  static int _daysUntil(String isoDate) {
    final due = DateTime.tryParse(isoDate);
    if (due == null) return 0;
    final today = DateUtils.dateOnly(DateTime.now());
    return DateTime.utc(
      due.year,
      due.month,
      due.day,
    ).difference(DateTime.utc(today.year, today.month, today.day)).inDays;
  }

  Widget _actions(CreditCardItem card) => LayoutBuilder(
    builder: (context, constraints) {
      final scale = MediaQuery.textScalerOf(context).scale(1);
      final width = max(constraints.maxWidth, AppSizes.button * 5 * scale);
      return SingleChildScrollView(
        scrollDirection: Axis.horizontal,
        child: SizedBox(
          width: width,
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              _QuickAction(
                icon: Icons.shopping_cart_outlined,
                label: 'Harcama',
                onTap: card.isActive ? () => _addCharge(card) : null,
              ),
              _QuickAction(
                icon: Icons.calendar_view_month,
                label: 'Taksitli harcama',
                onTap: card.isActive ? () => _addPlan(card) : null,
              ),
              _QuickAction(
                icon: Icons.payments_outlined,
                label: 'Ödeme',
                onTap: () => _choosePayment(card),
              ),
              _QuickAction(
                icon: Icons.receipt_long_outlined,
                label: 'Ekstre',
                onTap: () => _showStatement(card),
              ),
              _QuickAction(
                icon: Icons.tune,
                label: 'Düzenle',
                tooltip: 'Kartı düzenle',
                onTap: () => _editCard(card),
              ),
            ],
          ),
        ),
      );
    },
  );

  /// Bu kartın taksitli harcamaları.
  ///
  /// Her harcama tek satırdır: kaç taksitin gerçekleştiği, sıradaki gün,
  /// toplam tutar ve bir ilerleme çubuğu. Taksitlerin tek tek listesi satıra
  /// dokununca açılan panelde durur; sayfa uzun bir çizelgeyle dolmaz.
  ///
  /// Plan yalnız niyettir; gerçekleşmemiş taksit hiçbir rapora girmez. O
  /// yüzden hareketlerden ayrı bir bölümdür.
  List<InstallmentPlanModel> _cardPlans(CreditCardItem card) =>
      (widget.controller.snapshot?.plans ?? const <InstallmentPlanModel>[])
          .where((plan) => plan.creditCardId == card.id)
          .toList();

  Future<void> _openPlans(CreditCardItem card) =>
      Navigator.of(context).push<void>(
        MaterialPageRoute(
          builder: (_) => CardInstallmentsPage(
            listenable: widget.controller,
            plans: () => _cardPlans(card),
            onOpenPlan: (plan) => _showPlan(plan.id),
          ),
        ),
      );

  Future<void> _showPlan(String planId) => showInstallmentPlanDetail(
    context,
    listenable: widget.controller,
    plan: () =>
        (widget.controller.snapshot?.plans ?? const <InstallmentPlanModel>[])
            .where((item) => item.id == planId)
            .firstOrNull,
    isSubmitting: () => widget.controller.isSubmitting,
    onRealize: (plan, sequence) => _realize(plan.id, sequence),
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
    _revision++;
    currentStatement = widget.controller.loadCurrentStatement(widget.cardId);

    _openPrefilledPayment();
  });

  /// Kart ayarlarını düzenler.
  ///
  /// Asgari ödeme oranı karta ait bir ayar; onu değiştirebilmek için kartın
  /// düzenlenebilmesi gerekiyordu ve uygulamada hiç kart düzenleme yolu yoktu.
  Future<void> _editCard(CreditCardItem card) async {
    final saved = await AppFormSheet.show<Map<String, Object?>>(
      context: context,
      builder: (_) => _CardDialog(
        card: card,
        onSave: (input) => widget.controller.refusalOf(
          widget.controller.submit(
            () => widget.controller.repository.updateCard(card.id, input),
            'Kart güncellendi.',
          ),
        ),
      ),
    );
    if (saved != null && mounted) _reloadActivity();
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
        cardName: card.name,
        repository: widget.controller.repository,
        prefill: prefill,
      ),
    );
    if (input != null &&
        await widget.controller.submit(
          () => widget.controller.repository.createPlan(input),
          'Taksit planı oluşturuldu; henüz gider yazılmadı.',
        )) {
      _reloadActivity();
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
  Future<void> _choosePayment(
    CreditCardItem card, {
    CardStatement? statement,
  }) async {
    CardStatement? value;
    try {
      value = statement ?? await currentStatement;
    } catch (_) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Ödeme bilgileri yüklenemedi. Yeniden deneyin.'),
          ),
        );
      }
      return;
    }
    if (!mounted) return;
    String? selected;
    while (mounted) {
      if (!mounted) return;
      final choice = await AppFormSheet.show<String>(
        context: context,
        builder: (_) => _PaymentChoiceSheet(
          card: card,
          statement: value,
          initialChoice: selected,
        ),
      );
      if (choice == null || !mounted) return;
      selected = choice;
      final back = await _addPayment(
        card,
        allowChoiceReturn: true,
        presetAmount: switch (choice) {
          'balance' => value?.remainingBalance,
          'minimum' => value?.remainingMinimumPayment,
          'debt' => card.currentDebt,
          _ => null,
        },
      );
      if (!back) return;
    }
  }

  Future<bool> _addPayment(
    CreditCardItem card, {
    bool allowChoiceReturn = false,
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
        allowChoiceReturn: allowChoiceReturn,
        onSave: (input) => widget.controller.refusalOf(
          widget.controller.submit(
            () => widget.controller.repository.createPayment(card.id, input),
            'Kart ödemesi kaydedildi.',
            impact: FinanceMutationImpact.cardPayment,
          ),
        ),
        choiceLabel: 'Ödeme hesabı',
        choices: widget.controller.snapshot!.accounts,
        reloadChoices: () async {
          await widget.controller.load();
          return widget.controller.snapshot?.accounts ?? const [];
        },
        choiceKey: 'accountId',
        dateKey: 'paymentDate',
        presetAmount: presetAmount,
        presetDate: presetDate,
        presetDescription: presetDescription,
      ),
    );
    if (input?['returnToChoice'] == true) return true;
    if (input != null && mounted) {
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
    return false;
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
    final statement = await AppAdaptiveSheet.show<CardStatement>(
      context: context,
      builder: (_) =>
          _StatementSheet(card: card, controller: widget.controller),
    );
    if (statement != null && mounted) {
      await _choosePayment(card, statement: statement);
    }
  }
}

bool _positiveMoney(String value) =>
    !value.startsWith('-') && RegExp('[1-9]').hasMatch(value);

class _PaymentChoiceSheet extends StatefulWidget {
  const _PaymentChoiceSheet({
    required this.card,
    required this.statement,
    this.initialChoice,
  });
  final CreditCardItem card;
  final CardStatement? statement;
  final String? initialChoice;
  @override
  State<_PaymentChoiceSheet> createState() => _PaymentChoiceSheetState();
}

class _PaymentChoiceSheetState extends State<_PaymentChoiceSheet> {
  late String selected =
      widget.initialChoice ??
      (_positiveMoney(widget.statement?.remainingBalance ?? '0')
          ? 'balance'
          : _positiveMoney(widget.card.currentDebt)
          ? 'debt'
          : 'other');

  @override
  Widget build(BuildContext context) {
    final statement = widget.statement;
    return AppFormSheet<String>(
      title: 'Ödeme tutarı',
      description: widget.card.name,
      submitLabel: 'Devam et',
      showCancel: false,
      onSubmit: () async => selected,
      children: [
        _option('balance', 'Kalan dönem borcu', statement?.remainingBalance),
        _option('minimum', 'Asgari ödeme', statement?.remainingMinimumPayment),
        // Ekstre ödenmiş olsa da kesimden sonraki harcamalar borçtur; banka
        // uygulamalarındaki "toplam borç" seçeneği. Dönem borcuyla aynıysa
        // ikinci kez yazılmaz.
        if (_positiveMoney(widget.card.currentDebt) &&
            widget.card.currentDebt != statement?.remainingBalance)
          _option('debt', 'Güncel borcun tamamı', widget.card.currentDebt),
        _option('other', 'Başka tutar', null),
      ],
    );
  }

  Widget _option(String key, String label, String? amount) {
    final enabled =
        key == 'other' || (amount != null && _positiveMoney(amount));
    return Semantics(
      checked: selected == key,
      enabled: enabled,
      child: InkWell(
        onTap: enabled ? () => setState(() => selected = key) : null,
        borderRadius: BorderRadius.circular(AppRadius.inner),
        child: Container(
          constraints: const BoxConstraints(minHeight: AppSizes.button),
          padding: const EdgeInsets.all(AppSpacing.compact),
          decoration: BoxDecoration(
            color: selected == key ? AppSurfaces.of(context).cardMuted : null,
            borderRadius: BorderRadius.circular(AppRadius.inner),
          ),
          child: CardInfoPair(
            leading: CardStatusLine(
              icon: selected == key
                  ? Icons.radio_button_checked
                  : Icons.radio_button_unchecked,
              label: label,
              color: enabled ? null : Theme.of(context).disabledColor,
            ),
            trailing: key == 'other'
                ? const Icon(Icons.keyboard_outlined)
                : !enabled
                ? Text(amount == null ? 'Henüz oluşmadı' : 'Tamamlandı')
                : AppMoneyText(
                    amount: amount!,
                    currency: widget.card.currency,
                    size: AppMoneySize.body,
                  ),
          ),
        ),
      ),
    );
  }
}

class _CardDialog extends StatefulWidget {
  const _CardDialog({required this.onSave, this.card});

  /// Doluysa düzenleme, boşsa yeni kart.
  final CreditCardItem? card;

  /// Kaydı yazar; reddedildiyse gösterilecek cümleyi, yazıldıysa `null` döner.
  /// Form reddedilen kayıtta kapanmaz: cümle arkadaki sayfada çıkarsa
  /// kullanıcı kartın yazıldığını sanabilir.
  final Future<String?> Function(Map<String, Object?> input) onSave;
  @override
  State<_CardDialog> createState() => _CardDialogState();
}

class _CardDialogState extends State<_CardDialog> {
  final key = GlobalKey<FormState>();
  late final name = TextEditingController(text: widget.card?.name ?? '');
  late final limit = TextEditingController(
    text: widget.card == null ? '' : MoneyText.field(widget.card!.limit),
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
  String? error;
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
      title: widget.card == null ? 'Kart ekle' : 'Kartı düzenle',
      submitLabel: 'Kaydet',
      errorMessage: error,
      onSubmit: () async {
        if (!(key.currentState?.validate() ?? false)) return null;
        setState(() => error = null);
        final input = <String, Object?>{
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
        final refusal = await widget.onSave(input);
        if (refusal == null) return input;
        if (mounted) setState(() => error = refusal);
        return null;
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
            inputFormatters: const [TurkishAmountInputFormatter()],
            decoration: const InputDecoration(
              labelText: 'Limit',
              suffixText: 'TRY',
            ),
            validator: _moneyError,
          ),
        ),
        AppFormField(
          child: _Pair(
            first: TextFormField(
              controller: closing,
              decoration: const InputDecoration(labelText: 'Kesim günü (1–28)'),
              keyboardType: TextInputType.number,
              validator: _dayError,
            ),
            second: TextFormField(
              controller: due,
              decoration: const InputDecoration(
                labelText: 'Son ödeme günü (1–28)',
              ),
              keyboardType: TextInputType.number,
              validator: _dayError,
            ),
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
    this.reloadChoices,
    this.onSave,
    this.allowChoiceReturn = false,
  });
  final Future<String?> Function(Map<String, Object?>)? onSave;
  final bool allowChoiceReturn;
  final String title;
  final String choiceLabel;
  final List<FinanceChoice> choices;

  /// `Hesap ekle`den dönülünce hesap listesini yeniden okur.
  final Future<List<FinanceChoice>> Function()? reloadChoices;
  final String choiceKey;
  final String dateKey;

  /// Kayıt gider yazıyorsa (kart harcaması) taraf alanı çizilir; kart ödemesi
  /// para taşır, taraf taşımaz.
  final bool asksScope;

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
  String? error;
  late List<FinanceChoice> choices = widget.choices;
  late final TextEditingController amount = TextEditingController(
    text: widget.presetAmount == null
        ? ''
        : MoneyText.field(widget.presetAmount!),
  );
  late final TextEditingController description = TextEditingController(
    text: widget.presetDescription ?? '',
  );
  String? choice;

  /// Kart harcamasında iki kapsama açık kategorinin menüde seçilen kapsamı.
  TransactionScope? choiceScope;
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

  /// Yeni hesap listede ve seçili gelir.
  Future<void> _added(String id) async {
    final next = await widget.reloadChoices!();
    if (!mounted) return;
    setState(() {
      choices = next;
      if (next.any((item) => item.id == id)) choice = id;
    });
  }

  @override
  // Teslimle kurulan panel: 16 dp yan boşluk, metin harf aralıksız.
  Widget build(BuildContext context) => Form(
    key: key,
    child: AppFormSheet<Map<String, Object?>>(
      title: widget.title,
      leading: widget.allowChoiceReturn
          ? IconButton(
              tooltip: 'Tutar seçimi',
              onPressed: () => Navigator.pop(context, <String, Object?>{
                'returnToChoice': true,
              }),
              icon: const Icon(Icons.arrow_back),
            )
          : null,
      errorMessage: error,
      submitLabel: 'Kaydet',
      onSubmit: () async {
        // İki kapsama açık kategoride kapsamı alan kendisi ister.
        if (!(key.currentState?.validate() ?? false)) return null;
        final scope = AppCategoryField.scopeOf(
          _categoryOptions(choices),
          choice == null ? null : AppCategoryValue(choice!, choiceScope),
        );
        final input = <String, Object?>{
          widget.choiceKey: choice,
          if (_showScope) 'scope': scope?.apiValue,
          'amount': _money(amount.text),
          'currency': 'TRY',
          widget.dateKey: date,
          'description': _nullable(description.text),
        };
        final refusal = await widget.onSave?.call(input);
        if (!mounted) return null;
        if (refusal != null) {
          setState(() => error = refusal);
          return null;
        }
        return input;
      },
      children: [
        AppFormField(
          child: widget.choiceKey == 'accountId'
              ? _accountChoice(
                  context,
                  choices,
                  widget.choiceLabel,
                  choice,
                  (v) => setState(() => choice = v),
                  onAdded: widget.reloadChoices == null ? null : _added,
                )
              : _categoryChoice(
                  context,
                  choices: choices,
                  label: widget.choiceLabel,
                  value: choice == null
                      ? null
                      : AppCategoryValue(choice!, choiceScope),
                  showScope: _showScope,
                  onChanged: (v) => setState(() {
                    choice = v?.categoryId;
                    choiceScope = v?.scope;
                  }),
                  onAdded: (added) => setState(() {
                    choices = [...choices, added];
                    choice = added.id;
                    choiceScope = _showScope ? added.defaultScope : null;
                  }),
                ),
        ),
        AppFormField(
          child: TextFormField(
            controller: amount,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            inputFormatters: const [TurkishAmountInputFormatter()],
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
            lastDate: DateTime.now(),
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

/// Taksitli kart harcaması (Claude Design, kart taksit ve limit teslimi §8,
/// A): Kategori · Toplam tutar · Taksit sayısı ↔ İlk taksit yan yana · mavi
/// önizleme · Açıklama. Üstteki eski açıklama ve çip satırı yoktur.
///
/// Kartın içinden açıldığı için kart sorulmaz: `fixedCardId` doluyken seçici
/// hiç çizilmez, kartın adı başlığın sağında yazar. **Taksit tutarı
/// istemcide hesaplanmaz**: menüdeki aylık tutarlar ve önizleme, planı
/// kuran bölmeyle sunucudan gelir.
class _PlanDialog extends StatefulWidget {
  const _PlanDialog({
    required this.cards,
    required this.categories,
    required this.repository,
    this.fixedCardId,
    this.cardName,
    this.prefill,
  });
  final List<CreditCardItem> cards;
  final List<FinanceChoice> categories;
  final FinanceRepositoryContract repository;
  final String? fixedCardId;
  final String? cardName;

  /// Taksitli fişten gelen öneriler; hepsi değiştirilebilir.
  final InstallmentPrefill? prefill;
  @override
  State<_PlanDialog> createState() => _PlanDialogState();
}

class _PlanDialogState extends State<_PlanDialog> {
  final key = GlobalKey<FormState>();
  final _countRow = GlobalKey();
  final total = TextEditingController();
  final otherCount = TextEditingController(text: '15');
  final description = TextEditingController();
  String? cardId;
  String? categoryId;
  int count = 9;
  bool otherSelected = false;

  /// Sunucunun önizlemesi; tutar ya da gün değişince yeniden okunur.
  List<InstallmentPreviewOption> previews = const [];
  int _previewToken = 0;

  /// İki kapsama açık kategorinin menüde seçilen kapsamı.
  TransactionScope? categoryScope;
  late List<FinanceChoice> categories = widget.categories;
  String date = _dateText(DateTime.now());

  @override
  void initState() {
    super.initState();
    cardId = widget.fixedCardId;
    _applyPrefill();
    total.addListener(_schedulePreview);
    otherCount.addListener(_schedulePreview);
    _schedulePreview();
  }

  bool get _showScope => context.read<ScopeController?>()?.isVisible ?? false;

  int? get _count => otherSelected ? int.tryParse(otherCount.text) : count;

  /// Fişin söylediğini yazar. **Taksit tutarı hesaplanmıyor**: forma toplam
  /// giriliyor, bölmeyi sunucu yapıyor — istemci finansal toplamı ikinci kez
  /// hesaplamaz.
  void _applyPrefill() {
    final prefill = widget.prefill;
    if (prefill == null || prefill.isEmpty) return;
    total.text = MoneyText.field(prefill.totalAmount!);
    final value = prefill.installmentCount!;
    if (installmentCountChoices.contains(value)) {
      count = value;
    } else {
      otherSelected = true;
      otherCount.text = '$value';
    }
    if (prefill.firstInstallmentDate case final value?) date = value;
    if (prefill.description case final value?) description.text = value;
    if (prefill.categoryId case final value?) categoryId = value;
  }

  Timer? _previewTimer;

  void _schedulePreview() {
    final token = ++_previewToken;
    _previewTimer?.cancel();
    _previewTimer = Timer(const Duration(milliseconds: 300), () async {
      if (!mounted || token != _previewToken) return;
      final amount = _money(total.text);
      final n = _count;
      if (_moneyError(amount) != null) {
        if (previews.isNotEmpty) setState(() => previews = const []);
        return;
      }
      final counts = {
        ...installmentCountChoices,
        if (n != null && n >= 2 && n <= 60) n,
      }.toList();
      try {
        final result = await widget.repository.previewInstallments(
          totalAmount: amount,
          firstInstallmentDate: date,
          counts: counts,
        );
        if (mounted && token == _previewToken) {
          setState(() => previews = result);
        }
      } on Object {
        // Önizleme yalnız bilgidir; okunamazsa kutu çizilmez, kayıt sürer.
        if (mounted && token == _previewToken) {
          setState(() => previews = const []);
        }
      }
    });
  }

  @override
  void dispose() {
    _previewTimer?.cancel();
    total.dispose();
    otherCount.dispose();
    description.dispose();
    super.dispose();
  }

  @override
  // Teslimle kurulan panel: 16 dp yan boşluk, metin harf aralıksız.
  Widget build(BuildContext context) {
    final current = previews.where((p) => p.count == _count).firstOrNull;
    final countField = InstallmentCountField(
      count: count,
      other: otherSelected,
      otherController: otherCount,
      previews: previews,
      menuAnchorKey: _countRow,
      onCount: (value) => setState(() {
        count = value;
        _schedulePreview();
      }),
      onOther: () => setState(() {
        otherSelected = true;
        _schedulePreview();
      }),
    );
    final dateField = AppDateField(
      label: 'İlk taksit',
      value: date,
      onChanged: (value) => setState(() {
        date = value;
        _schedulePreview();
      }),
    );
    return Form(
      key: key,
      child: AppFormSheet<Map<String, Object?>>(
        title: 'Taksitli harcama',
        trailing: widget.cardName == null
            ? null
            : CardNameChip(name: widget.cardName!),
        submitLabel: 'Kaydet',
        onSubmit: () async {
          if (!(key.currentState?.validate() ?? false)) return null;
          // Plan kapsamını kurulurken alır; gerçekleşen her taksit onu taşır.
          final scope = AppCategoryField.scopeOf(
            _categoryOptions(categories),
            categoryId == null
                ? null
                : AppCategoryValue(categoryId!, categoryScope),
          );
          return {
            'creditCardId': cardId,
            'categoryId': categoryId,
            if (_showScope) 'scope': scope?.apiValue,
            'clientRequestId': _newGuid(),
            'totalAmount': _money(total.text),
            'currency': 'TRY',
            'installmentCount': _count,
            'firstInstallmentDate': date,
            'description': _nullable(description.text),
          };
        },
        children: [
          if (widget.fixedCardId == null)
            AppFormField(
              child: AppAccountField(
                label: 'Kart',
                value: cardId,
                options: [
                  for (final c in widget.cards)
                    AccountChoices.of(
                      context,
                    ).card(c.id, c.name, scope: c.defaultScope),
                ],
                onChanged: (v) => setState(() => cardId = v),
                validator: (v) => v == null ? 'Kart seçin.' : null,
                emptyTitle: 'Henüz kart yok',
                emptyMessage: 'Taksitli harcama için önce bir kart ekleyin.',
              ),
            ),
          AppFormField(
            child: _categoryChoice(
              context,
              choices: categories,
              label: 'Kategori',
              value: categoryId == null
                  ? null
                  : AppCategoryValue(categoryId!, categoryScope),
              showScope: _showScope,
              onChanged: (v) => setState(() {
                categoryId = v?.categoryId;
                categoryScope = v?.scope;
              }),
              onAdded: (added) => setState(() {
                categories = [...categories, added];
                categoryId = added.id;
                categoryScope = _showScope ? added.defaultScope : null;
              }),
            ),
          ),
          AppFormField(
            child: TextFormField(
              controller: total,
              keyboardType: const TextInputType.numberWithOptions(
                decimal: true,
              ),
              inputFormatters: const [TurkishAmountInputFormatter()],
              decoration: const InputDecoration(
                labelText: 'Toplam tutar',
                suffixText: 'TRY',
              ),
              validator: _moneyError,
            ),
          ),
          AppFormField(
            child: KeyedSubtree(
              key: _countRow,
              // Büyük yazıda iki alan yan yana sığmaz; alt alta iner.
              child: _Pair(first: countField, second: dateField, flex: 10),
            ),
          ),
          if (current != null)
            AppFormField(child: InstallmentPreviewBox(preview: current)),
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
/// Seçili ekstreyle ödeme tutarı seçimine döner; burada kayıt oluşturulmaz.
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

  /// Kesilmiş en son dönem.
  ///
  /// Ekstre kesim gününün **sonunda** kesilir: kesim günü hâlâ açık dönemdir
  /// ve o günün ekstresi ertesi gün çıkar. Henüz kesilmemiş dönem
  /// gösterilemez; sunucu o isteği reddeder, çünkü ortada daha ekstre yok.
  DateTime _latestClosedPeriod() =>
      _today.day <= widget.card.statementClosingDay
      ? DateTime(_today.year, _today.month - 1)
      : DateTime(_today.year, _today.month);

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
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return FutureBuilder<CardStatement>(
      future: _statement,
      builder: (context, snapshot) {
        final ready =
            snapshot.connectionState == ConnectionState.done &&
            !snapshot.hasError;
        final value = ready ? snapshot.data : null;
        return SafeArea(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Flexible(
                child: SingleChildScrollView(
                  // Başlık diğer panellerdeki gibi tutamacın hemen altından
                  // başlar (`AppSheetInsets.top`); gövdenin 8 dp'lik üst payı
                  // ve 48 dp'lik ok düğmeleri başlığı aşağı itiyordu.
                  padding: AppSheetInsets.body.copyWith(
                    top: AppSheetInsets.top(context),
                  ),
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      CardInfoPair(
                        leading: Text(
                          'Ekstre',
                          style: theme.textTheme.titleLarge,
                        ),
                        trailing: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            AppSheetHeaderSlot(
                              child: IconButton(
                                onPressed: () => _shift(-1),
                                icon: const Icon(Icons.chevron_left),
                                color: AppFinanceColors.of(context).neutral,
                                tooltip: 'Önceki dönem',
                              ),
                            ),
                            Flexible(
                              child: Text(
                                DateText.monthYear(_period.year, _period.month),
                                style: theme.textTheme.titleSmall,
                              ),
                            ),
                            AppSheetHeaderSlot(
                              child: IconButton(
                                onPressed: _canGoForward
                                    ? () => _shift(1)
                                    : null,
                                icon: const Icon(Icons.chevron_right),
                                color: AppFinanceColors.of(context).neutral,
                                tooltip: 'Sonraki dönem',
                              ),
                            ),
                          ],
                        ),
                      ),
                      if (snapshot.connectionState != ConnectionState.done)
                        const AppLoadingView(message: 'Ekstre hesaplanıyor')
                      else if (snapshot.hasError)
                        AppErrorView(
                          message: 'Bu dönemin ekstresi hesaplanamadı.',
                          onRetry: () => setState(() => _statement = _load()),
                        )
                      else if (value != null)
                        _content(value),
                    ],
                  ),
                ),
              ),
              if (value != null && !value.isPaid) ...[
                const Divider(height: 1),
                Padding(
                  padding: AppSheetInsets.actions,
                  child: SizedBox(
                    width: double.infinity,
                    child: FilledButton(
                      onPressed: () => Navigator.pop(context, value),
                      child: const Text('Ödeme yap'),
                    ),
                  ),
                ),
              ],
            ],
          ),
        );
      },
    );
  }

  Widget _content(CardStatement value) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final muted = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);
    final currency = value.currency;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      mainAxisSize: MainAxisSize.min,
      children: [
        Text(
          '${DateText.dayMonth(value.periodStart)} – '
          '${DateText.dayMonth(value.closingDate)} dönemi',
          style: muted,
        ),
        const SizedBox(height: AppSpacing.medium),
        Container(
          padding: const EdgeInsets.all(AppSpacing.medium),
          decoration: BoxDecoration(
            color: AppFinanceColors.of(context).neutralContainer,
            borderRadius: BorderRadius.circular(AppRadius.field),
          ),
          child: CardInfoPair(
            leading: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Kalan dönem borcu', style: muted),
                const SizedBox(height: AppSpacing.xxSmall),
                AppMoneyText(
                  amount: value.remainingBalance,
                  currency: currency,
                  effect: AppMoneyEffect.neutral,
                  onContainer: true,
                  style: theme.textTheme.titleLarge?.copyWith(
                    fontSize: AppTextSizes.metric,
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ],
            ),
            trailing: _StatementDue(statement: value, withYear: true),
          ),
        ),
        const SizedBox(height: AppSpacing.large),

        // Hesap açıkça yazılır ki kullanıcı ekstreyi doğrulayabilsin: her
        // satır bir öncekiyle toplanıp ara toplamı verir.
        Text(
          'Dönem hesabı',
          style: muted?.copyWith(fontWeight: FontWeight.w600),
        ),
        const SizedBox(height: AppSpacing.small),
        _amountRow('Önceki dönemden devir', value.previousBalance, currency),
        _amountRow(
          'Dönem harcamaları',
          value.periodCharges,
          currency,
          sign: '+',
        ),
        _amountRow(
          'Kesime kadar ödenen',
          value.paymentsThroughClosing,
          currency,
          sign: '−',
        ),
        const Divider(),
        _amountRow(
          'Dönem borcu',
          value.statementBalance,
          currency,
          emphasised: true,
        ),
        _amountRow(
          'Kesim sonrası ödenen',
          value.paymentsAfterClosing,
          currency,
          sign: '−',
        ),
        const Divider(),
        _amountRow(
          'Kalan dönem borcu',
          value.remainingBalance,
          currency,
          emphasised: true,
        ),
        const SizedBox(height: AppGaps.block),
        Container(
          padding: const EdgeInsets.all(AppSpacing.compact),
          decoration: BoxDecoration(
            color: AppFinanceColors.of(context).neutralContainer,
            borderRadius: BorderRadius.circular(AppRadius.inner),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // Tek satır: açıklama yok, etiket ve tutar kalın yazılır.
              _amountRow(
                'Ekstredeki asgari tutar',
                value.minimumPayment,
                currency,
                emphasised: true,
              ),
            ],
          ),
        ),
      ],
    );
  }

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
          Expanded(flex: 3, child: Text(label, style: style)),
          const SizedBox(width: AppSpacing.small),
          // Tutar sütunu sağ kenara yaslanır; rakamlar alt alta hizalanır.
          Expanded(
            flex: 2,
            child: Text(
              text ?? _signed(sign, amount!, currency),
              textAlign: TextAlign.end,
              style: label == 'Kesim sonrası ödenen'
                  ? style?.copyWith(color: AppFinanceColors.of(context).neutral)
                  : style,
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

/// Hesap seçimi ortak hesap alanıyla; tür ve bakiye rehberden gelir.
Widget _accountChoice(
  BuildContext context,
  List<FinanceChoice> choices,
  String label,
  String? value,
  ValueChanged<String?> changed, {
  Future<void> Function(String id)? onAdded,
}) {
  final options = AccountChoices.of(context);
  return AppAccountField(
    label: label,
    value: value,
    options: [
      for (final item in choices)
        options.account(item.id, item.name, scope: item.defaultScope),
    ],
    onChanged: changed,
    onAdd: () async {
      final id = (await addAccountFromForm(context))?.id;
      if (id != null) await onAdded?.call(id);
    },
    validator: (selected) => selected == null ? '$label seçin.' : null,
  );
}

List<AppCategoryOption> _categoryOptions(List<FinanceChoice> choices) => [
  for (final item in choices)
    AppCategoryOption(id: item.id, name: item.name, scope: item.defaultScope),
];

/// Gider kategorisi alanı; iki kapsama açık kategoride kapsam menüde seçilir,
/// kartın kapsamı ön değer olmaz (kullanıcı kararı D5).
Widget _categoryChoice(
  BuildContext context, {
  required List<FinanceChoice> choices,
  required String label,
  required AppCategoryValue? value,
  required bool showScope,
  required ValueChanged<AppCategoryValue?> onChanged,
  required ValueChanged<FinanceChoice> onAdded,
}) => AppCategoryField(
  label: label,
  options: _categoryOptions(choices),
  value: value,
  recent: RecentCategories.of(context, 'expense'),
  showScope: showScope,
  onChanged: onChanged,
  validator: (selected) => selected == null ? '$label seçin.' : null,
  onAdd: (name) async {
    final created = await addCategoryFromForm(
      context,
      type: 'expense',
      name: name,
    );
    if (created == null || !context.mounted) return;
    onAdded(
      FinanceChoice(
        id: created.id,
        name: created.name,
        defaultScope: created.defaultScope,
      ),
    );
  },
);

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

String _money(String value) =>
    MoneyText.normalizeInput(value) ?? value.trim().replaceAll(',', '.');

/// Kart satırının borç yarısı.
///
/// Alacaklı kartta "Borç −₺500,00" yazmak kullanıcıya borcu varmış gibi
/// okunuyordu; cümle yönü kendisi söylüyor, eksi işaretine gerek yok
/// (Aşama 06 Grup 5).
String? _nullable(String value) => value.trim().isEmpty ? null : value.trim();
String _dateText(DateTime value) =>
    '${value.year.toString().padLeft(4, '0')}-${value.month.toString().padLeft(2, '0')}-${value.day.toString().padLeft(2, '0')}';

String _newGuid() {
  final random = Random.secure();
  String hex(int length) =>
      List.generate(length, (_) => random.nextInt(16).toRadixString(16)).join();
  return '${hex(8)}-${hex(4)}-4${hex(3)}-${['8', '9', 'a', 'b'][random.nextInt(4)]}${hex(3)}-${hex(12)}';
}

/// Kartın altındaki işlem kutusu: ikon kutusu ve altında kısa etiket.
/// Kaydın sonucunu kendiliğinden kapanan bir bildirimle söyler.
///
/// Başarı şeritte kalsaydı kullanıcı kapatana kadar sayfanın üstünü
/// kaplardı; ret cümlesi ise şeritte durur, sessizce kaybolmamalı.
void _showSuccess(BuildContext context, FinanceController controller) {
  final message = controller.takeSuccessMessage();
  if (message == null) return;
  ScaffoldMessenger.maybeOf(context)
    ?..hideCurrentSnackBar()
    ..showSnackBar(SnackBar(content: Text(message)));
}

class _QuickAction extends StatelessWidget {
  const _QuickAction({
    required this.icon,
    required this.label,
    required this.onTap,
    this.tooltip,
  });

  final IconData icon;
  final String label;
  final VoidCallback? onTap;
  final String? tooltip;

  static const double _box = 52;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    final theme = Theme.of(context);
    final content = Opacity(
      opacity: onTap == null ? 0.4 : 1,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(AppRadius.inner),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: AppSpacing.xSmall),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Container(
                width: _box,
                height: _box,
                decoration: BoxDecoration(
                  color: surfaces.card,
                  borderRadius: BorderRadius.circular(AppRadius.field),
                  border: Border.all(color: surfaces.border),
                ),
                child: Icon(icon, color: surfaces.ink),
              ),
              const SizedBox(height: AppSpacing.step6),
              Text(
                label,
                textAlign: TextAlign.center,
                maxLines: 2,
                style: theme.textTheme.bodySmall?.copyWith(
                  fontSize: AppTextSizes.micro,
                  fontWeight: FontWeight.w600,
                  height: 1.25,
                  color: surfaces.ink,
                ),
              ),
            ],
          ),
        ),
      ),
    );
    return Expanded(
      child: Semantics(
        button: true,
        enabled: onTap != null,
        child: tooltip == null
            ? content
            : Tooltip(message: tooltip!, child: content),
      ),
    );
  }
}

/// Kaydırma şeridinin altındaki noktalar: kaç kart var, hangisi seçili.
class _PageDots extends StatelessWidget {
  const _PageDots({required this.count, required this.index});

  final int count;
  final int index;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return Semantics(
      label: '${index + 1}. kart, toplam $count kart',
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          for (var i = 0; i < count; i++)
            AnimatedContainer(
              duration: const Duration(milliseconds: 200),
              width: i == index ? 18 : 6,
              height: 6,
              margin: const EdgeInsets.symmetric(
                horizontal: AppSpacing.xxSmall,
              ),
              decoration: BoxDecoration(
                color: i == index ? surfaces.ink : surfaces.borderStrong,
                borderRadius: BorderRadius.circular(AppRadius.chip),
              ),
            ),
        ],
      ),
    );
  }
}

/// Ekstrenin sağ üst köşesi: son ödeme günü ya da ödendiği.
///
/// Renk tek başına anlam taşımaz: gecikme `Son ödeme geçti` diye yazılır.
class _StatementDue extends StatelessWidget {
  const _StatementDue({required this.statement, this.withYear = false});

  final CardStatement statement;
  final bool withYear;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final muted = AppSurfaces.of(context).inkMuted;
    final label = statement.isPaid
        ? 'Durum'
        : statement.isOverdue
        ? 'Son ödeme geçti'
        : 'Son ödeme';
    return Column(
      crossAxisAlignment: CrossAxisAlignment.end,
      children: [
        Text(
          label,
          style: theme.textTheme.bodySmall?.copyWith(
            color: statement.isOverdue && !statement.isPaid
                ? colors.expense
                : muted,
          ),
        ),
        const SizedBox(height: AppSpacing.xxSmall),
        Text(
          statement.isPaid
              ? 'Ödendi'
              : withYear
              ? DateText.dayMonthYear(statement.dueDate)
              : DateText.dayMonth(statement.dueDate),
          style: theme.textTheme.titleSmall?.copyWith(
            fontSize: AppTextSizes.row,
            color: statement.isPaid ? colors.income : null,
          ),
        ),
      ],
    );
  }
}

/// Formda yan yana iki alan (kesim ve son ödeme günü; taksit sayısı ve ilk
/// taksit), aralarında 12 dp. Büyük yazıda yan yana sığmazlar; alt alta
/// iner. [flex] verilirse ikinci alan birincinin 1,3 katı genişliktedir.
class _Pair extends StatelessWidget {
  const _Pair({required this.first, required this.second, this.flex});
  final Widget first;
  final Widget second;
  final int? flex;

  @override
  Widget build(BuildContext context) {
    if (context.usesLargeText) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          first,
          const SizedBox(height: AppGaps.field),
          second,
        ],
      );
    }
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Expanded(flex: flex ?? 1, child: first),
        const SizedBox(width: AppSpacing.compact),
        Expanded(flex: flex == null ? 1 : (flex! * 13) ~/ 10, child: second),
      ],
    );
  }
}
