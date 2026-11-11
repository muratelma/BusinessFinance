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
import '../../../core/widgets/app_row_action.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../activities/data/receipt_fee_writer.dart';
import '../data/finance_models.dart';
import '../data/finance_repository.dart';
import 'credit_card_tile.dart';
import 'finance_controller.dart';
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
    super.key,
  });
  final FinanceRepositoryContract repository;
  final FinancialDataChanges? financialDataChanges;

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

  @override
  void initState() {
    super.initState();
    controller = FinanceController(
      widget.repository,
      financialDataChanges: widget.financialDataChanges,
    )..addListener(_changed);
    controller.load();
  }

  @override
  void dispose() {
    controller.removeListener(_changed);
    controller.dispose();
    _pages.dispose();
    super.dispose();
  }

  void _changed() {
    if (mounted) setState(() {});
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
        Expanded(child: _cards(snapshot)),
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
              height: CreditCardTile.heightOf(context),
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
            const SizedBox(height: AppGaps.block),
            padded(
              CreditCardDetailPage(
                // Kart değişince bölümler o kartın verisiyle baştan kurulur;
                // öneriyle seçilince form o kartta açılır.
                key: ValueKey('${selected.id}-${_chosenCardId == selected.id}'),
                cardId: selected.id,
                controller: controller,
                asPanel: true,
                paymentPrefill: _chosenCardId == selected.id
                    ? widget.cardPaymentPrefill
                    : null,
                installmentPrefill: _chosenCardId == selected.id
                    ? widget.installmentPrefill
                    : null,
                recordFee: widget.recordFee,
              ),
            ),
          ],
        ],
      ),
    );
  }

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
  });
  final String cardId;
  final FinanceController controller;

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
    if (widget.asPanel) {
      return current == null
          ? const AppErrorView(message: 'Kart artık bulunamıyor.')
          : _sections(current);
    }
    return Scaffold(
      appBar: AppPageBar(title: Text(current?.name ?? 'Kart detayı')),
      body: current == null
          ? const AppErrorView(message: 'Kart artık bulunamıyor.')
          : Column(
              children: [
                // Yazma sonucunu **bu** ekran söyler: reddedilen bir ödeme
                // sessizce kaybolmamalı.
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
                    padding: AppPageInsets.list,
                    children: [_sections(current)],
                  ),
                ),
              ],
            ),
    );
  }

  /// Seçili kartın bölümleri, yukarıdan aşağı: işlemler, limit, ekstre ve
  /// borç, taksitler, hareketler.
  Widget _sections(CreditCardItem current) => Column(
    crossAxisAlignment: CrossAxisAlignment.stretch,
    mainAxisSize: MainAxisSize.min,
    children: [
      if (!widget.asPanel) ...[
        CreditCardTile(card: current),
        const SizedBox(height: AppGaps.block),
      ],
      _actions(current),
      const SizedBox(height: AppGaps.block),
      _limitCard(current),
      const SizedBox(height: AppGaps.card),
      _summary(current),
      const SizedBox(height: AppGaps.section),
      // Taksitler hareketlerin dışında: hiç harcaması olmayan kartın
      // taksitleri de görünmeli.
      _plansOfCard(current),
      AppSectionHeader(
        title: 'Hareketler',
        trailing: TextButton(
          onPressed: () => _openMovements(current),
          child: const Text('Tümü'),
        ),
      ),
      FutureBuilder<CardActivity>(
        future: activity,
        builder: (context, snapshot) {
          if (snapshot.connectionState != ConnectionState.done) {
            return const AppLoadingView(message: 'Kart hareketleri yükleniyor');
          }
          if (snapshot.hasError) {
            return AppErrorView(
              message: 'Kart hareketleri yüklenemedi.',
              onRetry: _reloadActivity,
            );
          }
          final rows = _Movement.of(snapshot.data!);
          if (rows.isEmpty) {
            // Bölüm boş: kartın içinde tek satır. Daha eskisi `Tümü`dedir.
            return AppCard(
              child: Text(
                '${widget.controller.period.sentence} kart hareketi yok.',
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  color: AppSurfaces.of(context).inkMuted,
                ),
              ),
            );
          }
          return _MovementList(
            rows.take(_previewCount).toList(growable: false),
          );
        },
      ),
    ],
  );

  /// Sayfada gösterilen hareket sayısı; fazlası `Tümü` sayfasındadır.
  static const int _previewCount = 10;

  Future<void> _openMovements(CreditCardItem card) async {
    await Navigator.of(context).push(
      MaterialPageRoute<void>(
        builder: (_) =>
            CardMovementsPage(card: card, controller: widget.controller),
      ),
    );
    // Sayfada dönem değişmiş olabilir; önizleme aynı dönemi gösterir.
    if (mounted) _reloadActivity();
  }

  /// Kullanılabilir limit: tek satır tutar, ince doluluk çubuğu ve altında
  /// kartın limiti.
  ///
  /// İki tutar da sunucudan gelir. Çubuğun doluluğu yalnız çizim içindir:
  /// ekranda sayı olarak yazılmaz.
  Widget _limitCard(CreditCardItem card) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final muted = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);
    final available = double.tryParse(card.availableLimit) ?? 0;
    final limit = double.tryParse(card.limit) ?? 0;
    final fill = limit <= 0 ? 0.0 : (available / limit).clamp(0.0, 1.0);
    return AppCard(
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.medium,
        vertical: AppSpacing.compact,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            children: [
              Expanded(child: Text('Kullanılabilir limit', style: muted)),
              const SizedBox(width: AppSpacing.small),
              Text(
                MoneyText.format(card.availableLimit, card.currency),
                style: theme.textTheme.titleSmall?.copyWith(
                  fontSize: AppTextSizes.row,
                  fontWeight: FontWeight.w700,
                ),
              ),
            ],
          ),
          const SizedBox(height: AppSpacing.small),
          ExcludeSemantics(
            child: ClipRRect(
              borderRadius: BorderRadius.circular(AppRadius.chip),
              child: SizedBox(
                height: 6,
                child: Stack(
                  fit: StackFit.expand,
                  children: [
                    ColoredBox(color: surfaces.cardMuted),
                    FractionallySizedBox(
                      alignment: Alignment.centerLeft,
                      widthFactor: fill,
                      child: ColoredBox(
                        color: AppFinanceColors.of(context).neutral,
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
          const SizedBox(height: AppSpacing.step6),
          Text(
            'Kart limiti ${MoneyText.format(card.limit, card.currency)}',
            textAlign: TextAlign.end,
            style: muted,
          ),
        ],
      ),
    );
  }

  /// Dönem borcu: kesilmiş ekstreden kalan tutar ve son ödeme günü.
  ///
  /// Kartın yüzündeki güncel borçtan farklıdır: o, kesimden sonraki
  /// harcamalar dâhil bütün borçtur.
  Widget _summary(CreditCardItem card) =>
      AppCard(child: _statementHeadline(card));

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
    final muted = theme.textTheme.bodySmall?.copyWith(
      color: AppSurfaces.of(context).inkMuted,
    );
    final minimum = statement.remainingMinimumPayment;
    // Sunucu tutarları dört ondalıklı kanonik biçimde gönderir; kıyas metin
    // üzerinden güvenlidir.
    final hasSeparateMinimum =
        minimum != statement.remainingBalance && minimum != '0.0000';
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('Dönem borcu', style: muted),
                  const SizedBox(height: AppSpacing.xxSmall),
                  // Kalan borç, ekstre borcu değil: kısmen ödenmiş ekstrede
                  // sorulan "daha ne kadar"dır.
                  AppMoneyText(
                    amount: statement.remainingBalance,
                    currency: statement.currency,
                    effect: statement.isOverdue ? AppMoneyEffect.expense : null,
                    style: theme.textTheme.titleLarge?.copyWith(
                      fontSize: AppTextSizes.metric,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(width: AppSpacing.small),
            _StatementDue(statement: statement),
          ],
        ),
        const SizedBox(height: AppSpacing.small),
        Text(
          hasSeparateMinimum
              ? '${DateText.monthYear(statement.year, statement.month)} '
                    'ekstresi · Asgari '
                    '${MoneyText.format(minimum, statement.currency)}'
              : '${DateText.monthYear(statement.year, statement.month)} '
                    'ekstresi',
          style: muted,
        ),
        if (!statement.isPaid) ...[
          const SizedBox(height: AppSpacing.compact),
          _StatementPayRow(
            compact: true,
            onPayAll: () => _payStatement(card, statement.remainingBalance),
            onPayMinimum: hasSeparateMinimum
                ? () => _payStatement(card, minimum)
                : null,
          ),
        ],
        // Renk tek başına anlam taşımaz; gecikme yazıyla da söylenir.
      ],
    );
  }

  /// Kartın işlemleri: kartın hemen altında, eşit aralıklı beş kutu.
  ///
  /// Sıra soldan sağa kullanım sıklığıdır: harcama, taksitli harcama, ödeme,
  /// ekstre, kartın ayarı. Pasif kartta harcama yazılamaz.
  Widget _actions(CreditCardItem card) => Row(
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
        onTap: () => _addPayment(card),
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
  );

  /// Bu kartın taksitli harcamaları.
  ///
  /// Her harcama tek satırdır: kaç taksitin gerçekleştiği, sıradaki gün,
  /// toplam tutar ve bir ilerleme çubuğu. Taksitlerin tek tek listesi satıra
  /// dokununca açılan panelde durur; sayfa uzun bir çizelgeyle dolmaz.
  ///
  /// Plan yalnız niyettir; gerçekleşmemiş taksit hiçbir rapora girmez. O
  /// yüzden hareketlerden ayrı bir bölümdür.
  Widget _plansOfCard(CreditCardItem card) {
    final plans = (widget.controller.snapshot?.plans ?? const [])
        .where((plan) => plan.creditCardId == card.id)
        .toList(growable: false);
    if (plans.isEmpty) return const SizedBox.shrink();
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        const AppSectionHeader(title: 'Taksitler'),
        AppCard(
          padding: EdgeInsets.zero,
          child: Column(
            children: [
              for (final (index, plan) in plans.indexed) ...[
                if (index > 0) const Divider(height: 1),
                InkWell(
                  onTap: () => _showPlan(plan.id),
                  child: Padding(
                    padding: const EdgeInsets.all(AppSpacing.medium),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      children: [
                        Row(
                          children: [
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    plan.description ?? 'Taksitli harcama',
                                    maxLines: 1,
                                    overflow: TextOverflow.ellipsis,
                                    style: theme.textTheme.titleSmall,
                                  ),
                                  const SizedBox(height: AppSpacing.xxSmall),
                                  Text(
                                    _planProgress(plan),
                                    style: theme.textTheme.bodySmall?.copyWith(
                                      color: surfaces.inkMuted,
                                    ),
                                  ),
                                ],
                              ),
                            ),
                            const SizedBox(width: AppSpacing.small),
                            AppMoneyText(
                              amount: plan.totalAmount,
                              currency: plan.currency,
                            ),
                            Icon(
                              Icons.chevron_right,
                              size: 20,
                              color: surfaces.inkMuted,
                            ),
                          ],
                        ),
                        const SizedBox(height: AppSpacing.compact),
                        ExcludeSemantics(
                          child: ClipRRect(
                            borderRadius: BorderRadius.circular(AppRadius.chip),
                            child: SizedBox(
                              height: 6,
                              child: Stack(
                                fit: StackFit.expand,
                                children: [
                                  ColoredBox(color: surfaces.cardMuted),
                                  FractionallySizedBox(
                                    alignment: Alignment.centerLeft,
                                    widthFactor: plan.items.isEmpty
                                        ? 0
                                        : plan.items
                                                  .where((i) => i.isRealized)
                                                  .length /
                                              plan.items.length,
                                    child: ColoredBox(
                                      color: AppFinanceColors.of(
                                        context,
                                      ).neutral,
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ],
          ),
        ),
        const SizedBox(height: AppGaps.section),
      ],
    );
  }

  /// `2/6 taksit · sıradaki 15 Aralık`; bitmiş planda `6/6 taksit`.
  static String _planProgress(InstallmentPlanModel plan) {
    final done = plan.items.where((item) => item.isRealized).length;
    final next = plan.items.where((item) => !item.isRealized).firstOrNull;
    final count = '$done/${plan.items.length} taksit';
    return next == null
        ? count
        : '$count · sıradaki ${DateText.dayMonth(next.scheduledDate)}';
  }

  /// Bir taksitli harcamanın bütün taksitleri; gerçekleştirme buradadır.
  Future<void> _showPlan(String planId) => showModalBottomSheet<void>(
    context: context,
    isScrollControlled: true,
    showDragHandle: true,
    builder: (context) => ListenableBuilder(
      listenable: widget.controller,
      builder: (context, _) {
        final plan = (widget.controller.snapshot?.plans ?? const [])
            .where((item) => item.id == planId)
            .firstOrNull;
        if (plan == null) return const SizedBox.shrink();
        return SafeArea(
          child: ConstrainedBox(
            constraints: BoxConstraints(
              maxHeight: MediaQuery.sizeOf(context).height * 0.75,
            ),
            child: ListView(
              shrinkWrap: true,
              padding: AppSheetInsets.body,
              children: [
                Text(
                  plan.description ?? 'Taksitli harcama',
                  style: Theme.of(context).textTheme.titleLarge,
                ),
                const SizedBox(height: AppSpacing.xxSmall),
                Text(
                  '${_planProgress(plan)} · '
                  '${MoneyText.format(plan.totalAmount, plan.currency)}',
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: AppSurfaces.of(context).inkMuted,
                  ),
                ),
                const SizedBox(height: AppSpacing.small),
                for (final item in plan.items) _planItemRow(plan, item),
              ],
            ),
          ),
        );
      },
    ),
  );

  /// Bir taksitin satırı: solda oldu / olmadı işareti, ortada sırası, günü
  /// ve durumu, sağda tutarı.
  ///
  /// Plan yalnız niyettir; gerçek kayıt yalnız `Gerçekleştir` ile üretilir.
  /// Günü gelmemiş taksitte eylem yoktur: taksit kendi gününe yazılır ve
  /// erken gerçekleştirmek ileri tarihli bir harcama doğururdu.
  Widget _planItemRow(InstallmentPlanModel plan, InstallmentItemModel item) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final due = !item.isRealized && _isDue(item);
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.compact),
      child: Row(
        children: [
          Icon(
            item.isRealized ? Icons.check_circle : Icons.radio_button_unchecked,
            size: 22,
            color: item.isRealized
                ? AppFinanceColors.of(context).neutral
                : surfaces.borderStrong,
          ),
          const SizedBox(width: AppSpacing.medium),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  '${item.sequence}. taksit',
                  style: theme.textTheme.titleSmall,
                ),
                const SizedBox(height: AppSpacing.xxSmall),
                Text(
                  DateText.dayMonthYear(item.scheduledDate),
                  style: theme.textTheme.bodySmall?.copyWith(
                    color: surfaces.inkMuted,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(width: AppSpacing.small),
          Column(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              AppMoneyText(
                amount: item.amount,
                currency: item.currency,
                // Gerçekleşmemiş taksit henüz gider değil: kırmızı yazmak
                // olmamış bir harcamayı olmuş göstermek olurdu.
                effect: item.isRealized ? AppMoneyEffect.expense : null,
                size: AppMoneySize.row,
              ),
              const SizedBox(height: AppSpacing.xxSmall),
              if (due)
                AppRowAction(
                  label: 'Gerçekleştir',
                  onPressed: widget.controller.isSubmitting
                      ? null
                      : () => _realize(plan.id, item.sequence),
                )
              else
                Text(
                  item.isRealized ? 'Gerçekleşti' : 'Planlandı',
                  style: theme.textTheme.bodySmall?.copyWith(
                    color: surfaces.inkMuted,
                  ),
                ),
            ],
          ),
        ],
      ),
    );
  }

  static bool _isDue(InstallmentItemModel item) {
    final scheduled = AppDateField.parse(item.scheduledDate);
    return scheduled == null ||
        !scheduled.isAfter(DateUtils.dateOnly(DateTime.now()));
  }

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
    if (input != null &&
        await widget.controller.submit(
          () => widget.controller.repository.createPayment(card.id, input),
          'Kart ödemesi kaydedildi.',
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
    final amount = await showModalBottomSheet<String>(
      context: context,
      isScrollControlled: true,
      showDragHandle: true,
      builder: (_) =>
          _StatementSheet(card: card, controller: widget.controller),
    );
    if (amount != null && mounted) await _payStatement(card, amount);
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
      title: widget.card == null ? 'Yeni kredi kartı' : 'Kartı düzenle',
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
                  'Harcamanın kapsamını kategori belirler. Kategoride kapsam '
                  'seçilmemişse bu kartın varsayılan kapsamı kullanılır.',
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
  });
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
  late List<FinanceChoice> choices = widget.choices;
  late final TextEditingController amount = TextEditingController(
    text: widget.presetAmount == null
        ? ''
        : MoneyText.editable(widget.presetAmount!),
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
      submitLabel: 'Kaydet',
      onSubmit: () async {
        // İki kapsama açık kategoride kapsamı alan kendisi ister.
        if (!(key.currentState?.validate() ?? false)) return null;
        final scope = AppCategoryField.scopeOf(
          _categoryOptions(choices),
          choice == null ? null : AppCategoryValue(choice!, choiceScope),
        );
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
    this.prefill,
  });
  final List<CreditCardItem> cards;
  final List<FinanceChoice> categories;
  final String? fixedCardId;

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

  /// İki kapsama açık kategorinin menüde seçilen kapsamı.
  TransactionScope? categoryScope;
  late List<FinanceChoice> categories = widget.categories;
  String date = _dateText(DateTime.now());

  @override
  void initState() {
    super.initState();
    cardId = widget.fixedCardId;
    _applyPrefill();
  }

  bool get _showScope => context.read<ScopeController?>()?.isVisible ?? false;

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
  // Teslimle kurulan panel: 16 dp yan boşluk, metin harf aralıksız.
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
          'installmentCount': int.parse(count.text),
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
            label: 'Gider kategorisi',
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
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return SafeArea(
      child: ConstrainedBox(
        constraints: BoxConstraints(
          maxHeight: MediaQuery.sizeOf(context).height * 0.88,
        ),
        child: SingleChildScrollView(
          padding: AppSheetInsets.body,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            mainAxisSize: MainAxisSize.min,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      'Ekstre',
                      style: theme.textTheme.titleLarge?.copyWith(
                        fontSize: AppTextSizes.metric,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ),
                  IconButton(
                    onPressed: () => _shift(-1),
                    icon: const Icon(Icons.chevron_left),
                    tooltip: 'Önceki dönem',
                  ),
                  Text(
                    DateText.monthYear(_period.year, _period.month),
                    style: theme.textTheme.titleSmall,
                  ),
                  IconButton(
                    // İleri, en son kapanmış dönemde durur; ötesi henüz
                    // kesilmedi.
                    onPressed: _canGoForward ? () => _shift(1) : null,
                    icon: const Icon(Icons.chevron_right),
                    tooltip: 'Sonraki dönem',
                  ),
                ],
              ),
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
    );
  }

  Widget _content(CardStatement value) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final muted = theme.textTheme.bodySmall?.copyWith(color: surfaces.inkMuted);
    final currency = value.currency;
    final minimum = value.remainingMinimumPayment;
    final hasSeparateMinimum =
        minimum != value.remainingBalance && minimum != '0.0000';
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
            color: surfaces.cardMuted,
            borderRadius: BorderRadius.circular(AppRadius.field),
          ),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text('Kalan borç', style: muted),
                    const SizedBox(height: AppSpacing.xxSmall),
                    AppMoneyText(
                      amount: value.remainingBalance,
                      currency: currency,
                      effect: value.isOverdue ? AppMoneyEffect.expense : null,
                      style: theme.textTheme.titleLarge?.copyWith(
                        fontSize: AppTextSizes.metric,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(width: AppSpacing.small),
              _StatementDue(statement: value, withYear: true),
            ],
          ),
        ),
        const SizedBox(height: AppSpacing.large),

        // Hesap açıkça yazılır ki kullanıcı ekstreyi doğrulayabilsin: her
        // satır bir öncekiyle toplanıp ara toplamı verir.
        Text(
          'Hesap özeti',
          style: muted?.copyWith(fontWeight: FontWeight.w600),
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
        _amountRow(
          'Asgari ödeme (%${MoneyText.percent(value.minimumPaymentRate)})',
          minimum,
          currency,
        ),
        if (!value.isPaid) ...[
          const SizedBox(height: AppSpacing.large),
          _StatementPayRow(
            onPayAll: () => Navigator.pop(context, value.remainingBalance),
            onPayMinimum: hasSeparateMinimum
                ? () => Navigator.pop(context, minimum)
                : null,
          ),
        ],
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

String _money(String value) => value.trim().replaceAll(',', '.');

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

/// Kartın altındaki işlem kutusu: ikon kutusu ve altında kısa etiket.
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

/// Ekstre ödeme düğmeleri: ikisi de aynı `Kart ödemesi` formunu açar, yalnız
/// tutarı önceden doldurur. Ödeme bir döneme bağlanmaz.
class _StatementPayRow extends StatelessWidget {
  const _StatementPayRow({
    required this.onPayAll,
    required this.onPayMinimum,
    this.compact = false,
  });

  final VoidCallback onPayAll;
  final VoidCallback? onPayMinimum;

  /// Sayfanın içindeki kartta düğmeler bir kademe kısadır.
  final bool compact;

  @override
  Widget build(BuildContext context) {
    final size = Size(0, compact ? AppSizes.menuButton : AppSizes.button);
    final shape = RoundedRectangleBorder(
      borderRadius: BorderRadius.circular(AppRadius.field),
    );
    return Row(
      children: [
        if (onPayMinimum != null) ...[
          Expanded(
            child: OutlinedButton(
              onPressed: onPayMinimum,
              style: OutlinedButton.styleFrom(minimumSize: size, shape: shape),
              child: const Text('Asgariyi öde'),
            ),
          ),
          const SizedBox(width: AppSpacing.small),
        ],
        Expanded(
          child: FilledButton(
            onPressed: onPayAll,
            style: FilledButton.styleFrom(minimumSize: size, shape: shape),
            child: const Text('Ekstreyi öde'),
          ),
        ),
      ],
    );
  }
}

/// Kartın bir hareketi: harcama ya da ödeme.
class _Movement {
  const _Movement({
    required this.isPayment,
    required this.amount,
    required this.currency,
    required this.date,
    required this.description,
    required this.isCancelled,
  });

  final bool isPayment;
  final String amount;
  final String currency;
  final String date;
  final String? description;
  final bool isCancelled;

  /// Harcamalar ve ödemeler tek listede, en yeni üstte (kullanıcı kararı,
  /// 11 Kasım 2026).
  static List<_Movement> of(CardActivity value) => [
    for (final item in value.charges)
      _Movement(
        isPayment: false,
        amount: item.amount,
        currency: item.currency,
        date: item.date,
        description: item.description,
        isCancelled: item.isCancelled,
      ),
    for (final item in value.payments)
      _Movement(
        isPayment: true,
        amount: item.amount,
        currency: item.currency,
        date: item.date,
        description: item.description,
        isCancelled: item.isCancelled,
      ),
  ]..sort((a, b) => b.date.compareTo(a.date));
}

/// Kart hareketlerinin listesi.
///
/// Yön kaybolmaz: harcama gider renginde, ödeme nötr renkte yazılır. Kart
/// ödemesi gider değildir; aynı harcamayı iki kez saymamak için nötrdür.
class _MovementList extends StatelessWidget {
  const _MovementList(this.rows);

  final List<_Movement> rows;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final muted = AppSurfaces.of(context).inkMuted;
    return AppCard(
      padding: EdgeInsets.zero,
      child: Column(
        children: [
          for (final (index, item) in rows.indexed) ...[
            if (index > 0)
              const Divider(height: 1, indent: AppIconCapsule.rowInset),
            Opacity(
              opacity: item.isCancelled ? 0.55 : 1,
              child: Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppSpacing.medium,
                  vertical: AppSpacing.compact,
                ),
                child: Row(
                  children: [
                    AppIconCapsule(
                      icon: item.isPayment
                          ? Icons.payments_outlined
                          : Icons.shopping_cart_outlined,
                      tone: item.isCancelled
                          ? AppStatusTone.cancelled
                          : item.isPayment
                          ? AppStatusTone.neutral
                          : AppStatusTone.expense,
                    ),
                    const SizedBox(width: AppSpacing.medium),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            item.description ??
                                (item.isPayment
                                    ? 'Kart ödemesi'
                                    : 'Kart harcaması'),
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                            style: theme.textTheme.titleSmall,
                          ),
                          const SizedBox(height: AppSpacing.xxSmall),
                          Text(
                            [
                              DateText.dayMonth(item.date),
                              // Açıklamalı ödemede türü alt satır söyler.
                              if (item.isPayment && item.description != null)
                                'Ödeme',
                              if (item.isCancelled) 'İptal edildi',
                            ].join(' • '),
                            style: theme.textTheme.bodySmall?.copyWith(
                              color: muted,
                            ),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(width: AppSpacing.small),
                    AppMoneyText(
                      amount: item.amount,
                      currency: item.currency,
                      effect: item.isPayment
                          ? AppMoneyEffect.neutral
                          : AppMoneyEffect.expense,
                      isCancelled: item.isCancelled,
                      size: AppMoneySize.row,
                    ),
                  ],
                ),
              ),
            ),
          ],
        ],
      ),
    );
  }
}

/// Hareketlerin türe göre süzgeci.
enum _MovementKind {
  all('Hepsi'),
  charges('Harcamalar'),
  payments('Ödemeler');

  const _MovementKind(this.label);
  final String label;
}

extension on HistoryPeriod {
  /// Filtre panelinde ve etikette görünen ad.
  String get title => switch (this) {
    HistoryPeriod.threeMonths => 'Son 3 ay',
    HistoryPeriod.year => 'Son 1 yıl',
    HistoryPeriod.all => 'Tüm zamanlar',
  };

  /// `Son 3 ayda kart hareketi yok.`
  String get sentence => switch (this) {
    HistoryPeriod.threeMonths => 'Son 3 ayda',
    HistoryPeriod.year => 'Son 1 yılda',
    HistoryPeriod.all => 'Bu kartta',
  };
}

/// Bir kartın bütün hareketleri; dönem ve tür başlıktaki filtreyle seçilir.
///
/// Filtre başlıkta tek ikondur; seçim panelde yapılır ve `Uygula` ile
/// geçerli olur. Varsayılandan farklı seçim listenin üstünde etiket olarak
/// durur ve etiketinden kaldırılır.
class CardMovementsPage extends StatefulWidget {
  const CardMovementsPage({
    required this.card,
    required this.controller,
    super.key,
  });

  final CreditCardItem card;
  final FinanceController controller;

  @override
  State<CardMovementsPage> createState() => _CardMovementsPageState();
}

class _CardMovementsPageState extends State<CardMovementsPage> {
  late Future<CardActivity> _activity = _load();
  _MovementKind _kind = _MovementKind.all;

  HistoryPeriod get _period => widget.controller.period;

  Future<CardActivity> _load() =>
      widget.controller.loadActivity(widget.card.id);

  bool get _filtered =>
      _kind != _MovementKind.all || _period != HistoryPeriod.threeMonths;

  Future<void> _apply(HistoryPeriod period, _MovementKind kind) async {
    if (period != _period) await widget.controller.selectPeriod(period);
    if (!mounted) return;
    setState(() {
      _kind = kind;
      _activity = _load();
    });
  }

  Future<void> _openFilter() async {
    final result =
        await AppFormSheet.show<({HistoryPeriod period, _MovementKind kind})>(
          context: context,
          builder: (_) => _MovementFilterSheet(period: _period, kind: _kind),
        );
    if (result != null) await _apply(result.period, result.kind);
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppPageBar(
      title: const Text('Kart hareketleri'),
      actions: [
        IconButton(
          tooltip: 'Filtrele',
          onPressed: _openFilter,
          icon: Badge(
            isLabelVisible: _filtered,
            smallSize: 8,
            child: const Icon(Icons.filter_list),
          ),
        ),
      ],
    ),
    body: FutureBuilder<CardActivity>(
      future: _activity,
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) {
          return const AppLoadingView(message: 'Kart hareketleri yükleniyor');
        }
        if (snapshot.hasError) {
          return AppErrorView(
            message: 'Kart hareketleri yüklenemedi.',
            onRetry: () => setState(() => _activity = _load()),
          );
        }
        final value = snapshot.data!;
        final rows = _Movement.of(value)
            .where(
              (row) => switch (_kind) {
                _MovementKind.all => true,
                _MovementKind.charges => !row.isPayment,
                _MovementKind.payments => row.isPayment,
              },
            )
            .toList(growable: false);
        final muted = Theme.of(context).textTheme.bodySmall?.copyWith(
          color: AppSurfaces.of(context).inkMuted,
        );
        return ListView(
          padding: AppPageInsets.list,
          children: [
            Text('${widget.card.name} · ${_period.title}', style: muted),
            if (_filtered) ...[
              const SizedBox(height: AppSpacing.small),
              Wrap(
                spacing: AppSpacing.small,
                runSpacing: AppSpacing.xSmall,
                children: [
                  if (_period != HistoryPeriod.threeMonths)
                    InputChip(
                      label: Text(_period.title),
                      onDeleted: () => _apply(HistoryPeriod.threeMonths, _kind),
                      deleteButtonTooltipMessage: 'Dönemi kaldır',
                    ),
                  if (_kind != _MovementKind.all)
                    InputChip(
                      label: Text(_kind.label),
                      onDeleted: () => _apply(_period, _MovementKind.all),
                      deleteButtonTooltipMessage: 'Türü kaldır',
                    ),
                ],
              ),
            ],
            const SizedBox(height: AppSpacing.compact),
            if (value.hasMore) const _TruncationNotice(),
            if (rows.isEmpty)
              AppEmptyView(
                compact: true,
                title: _filtered
                    ? 'Bu filtreye uyan hareket yok'
                    : '${_period.sentence} kart hareketi yok',
                message: _filtered
                    ? 'Filtreyi değiştirebilir ya da kaldırabilirsiniz.'
                    : 'Daha eskisi için filtreden dönemi genişletin.',
                icon: Icons.credit_card,
              )
            else
              _MovementList(rows),
          ],
        );
      },
    ),
  );
}

/// Hareket filtresi: seçim taslaktır, `Uygula` ile geçerli olur; `Temizle`
/// varsayılana döner, `Vazgeç` hiçbir şeyi değiştirmez.
class _MovementFilterSheet extends StatefulWidget {
  const _MovementFilterSheet({required this.period, required this.kind});

  final HistoryPeriod period;
  final _MovementKind kind;

  @override
  State<_MovementFilterSheet> createState() => _MovementFilterSheetState();
}

class _MovementFilterSheetState extends State<_MovementFilterSheet> {
  late HistoryPeriod _period = widget.period;
  late _MovementKind _kind = widget.kind;

  @override
  Widget build(BuildContext context) =>
      AppFormSheet<({HistoryPeriod period, _MovementKind kind})>(
        title: 'Filtrele',
        submitLabel: 'Uygula',
        onSubmit: () async => (period: _period, kind: _kind),
        secondaryLabel: 'Temizle',
        onSecondary: () async =>
            (period: HistoryPeriod.threeMonths, kind: _MovementKind.all),
        children: [
          const AppSectionHeader(title: 'Dönem'),
          Wrap(
            spacing: AppSpacing.small,
            runSpacing: AppSpacing.xSmall,
            children: [
              for (final period in HistoryPeriod.values)
                ChoiceChip(
                  label: Text(period.title),
                  selected: period == _period,
                  onSelected: (_) => setState(() => _period = period),
                ),
            ],
          ),
          const SizedBox(height: AppGaps.section),
          const AppSectionHeader(title: 'Tür'),
          Wrap(
            spacing: AppSpacing.small,
            runSpacing: AppSpacing.xSmall,
            children: [
              for (final kind in _MovementKind.values)
                ChoiceChip(
                  label: Text(kind.label),
                  selected: kind == _kind,
                  onSelected: (_) => setState(() => _kind = kind),
                ),
            ],
          ),
        ],
      );
}
