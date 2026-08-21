import 'package:flutter/material.dart';

import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_content_width.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../activities/presentation/quick_add_models.dart';
import '../data/receipt_image_source.dart';
import '../data/receipt_models.dart';
import 'receipt_prefill.dart';
import 'receipt_scan_controller.dart';

/// Okumanın iki girişi: alışveriş belgesi ve banka belgesi.
///
/// İkisi tek ekranda tek bir seçiciyle toplanmıştı; aynı soru iki kez
/// soruluyordu. Kullanıcı `İşlem ekle` menüsünde zaten "elimde ne var" diyor,
/// ekran açılınca aynı soruyu tekrarlamak o cevabı yok saymak olurdu.
///
/// İki belge sınıfı ekranda da ayrı durmalı, çünkü **sordukları soru farklı**:
/// alışveriş belgesinde yön kullanıcıdan fotoğraf çekilmeden önce alınır (aynı
/// kira makbuzu kiracı için gider, ev sahibi için gelirdir); dekontta yön
/// okuma bitene kadar sorulamaz (5.000 TL ödeme de olabilir, borç verme de,
/// kendi hesabına aktarma da) ve karar sayfasına bırakılır.
enum ReceiptScanVariant {
  receipt,
  bankSlip;

  String get title => switch (this) {
    receipt => 'Fiş veya fatura',
    bankSlip => 'Dekont',
  };

  String get intro => switch (this) {
    receipt =>
      'Market fişi, fatura veya makbuzun fotoğrafını çekin; tutar, tarih ve '
          'işletme adı önerilsin. Okunan bilgiler yalnızca öneridir, hiçbir '
          'kayıt siz onaylamadan oluşmaz.',
    bankSlip =>
      'Havale, EFT, ATM çekimi veya kart borcu ödemesi dekontu. Belgedeki '
          'tutarın ne olduğunu okuma bittikten sonra soracağız: dekont bunu '
          'yazmaz.',
  };

  /// Yön yalnız alışveriş belgesinde ve yalnız burada sorulur.
  ///
  /// Dekontta sorulmaz — sorulsaydı kullanıcı belgede ne yazdığını görmeden
  /// tahmin eder, sonra karar sayfası aynı soruyu tekrar sorup cevabı ezerdi.
  bool get asksDirection => this == receipt;

  ReceiptCaptureIntent get initialIntent => switch (this) {
    receipt => ReceiptCaptureIntent.expense,
    bankSlip => ReceiptCaptureIntent.bankSlip,
  };
}

/// Belgenin fotoğrafını alan ve okutan ara adım.
///
/// Kendi başına hiçbir şey kaydetmez; işi bittiğinde mevcut formu önü dolu
/// açar. Okuma başarısız olursa da aynı forma gider — yalnız boş olarak.
/// Kullanıcı, sağlayıcı çalışmadığı için harcamayı kaydedemez duruma düşmez.
class ReceiptScanPage extends StatefulWidget {
  const ReceiptScanPage({
    required this.controller,
    required this.onDraftReady,
    super.key,
    this.variant = ReceiptScanVariant.receipt,
    this.onBankDocumentReady,
    this.onRefundReady,
    this.onInvoiceReady,
    this.onInstallmentReady,
  });

  final ReceiptScanController controller;

  /// Hangi belge sınıfı için açıldığı: başlığı, anlatımı ve yön sorusunun
  /// sorulup sorulmayacağını belirler.
  final ReceiptScanVariant variant;

  /// Formu açan geri çağırım. Yönlendirme burada değil çağıranda: bu ekran
  /// hangi rotanın gider yazdığını bilmek zorunda değil.
  /// Taslak hazır olduğunda çağrılır. Yön birlikte taşınır: aynı taslak gider
  /// formuna da gelir formuna da gidebilir ve hangisi olduğunu bu ekran bilir,
  /// form bilmez.
  final void Function(QuickAddPrefill? prefill, ReceiptCaptureIntent intent)
  onDraftReady;

  /// Banka belgesi okunduğunda çağrılır.
  ///
  /// Doğrudan bir forma gitmiyor: dekonttaki ana tutarın ne olduğu belgeden
  /// okunamaz (ödeme mi, aktarma mı, kart ödemesi mi, borç verme mi) ve önce
  /// kullanıcıya sorulmalı.
  final void Function(ReceiptDraft draft)? onBankDocumentReady;

  /// İade fişi okunduğunda çağrılır. Kullanıcı, geri verilen harcamayı görüp
  /// iptali onaylayacak.
  final void Function(ReceiptDraft draft)? onRefundReady;

  /// Son ödeme tarihi olan bir fatura okunduğunda çağrılır; kullanıcıya
  /// "ödediniz mi?" diye sorulacak.
  final void Function(ReceiptDraft draft)? onInvoiceReady;

  /// Taksitli bir fiş okunduğunda çağrılır; kullanıcı kartı seçip plan
  /// oluşturacak.
  final void Function(ReceiptDraft draft)? onInstallmentReady;

  @override
  State<ReceiptScanPage> createState() => _ReceiptScanPageState();
}

class _ReceiptScanPageState extends State<ReceiptScanPage> {
  @override
  void initState() {
    super.initState();
    widget.controller.addListener(_onControllerChanged);
    widget.controller.loadPreferences();
  }

  @override
  void dispose() {
    widget.controller.removeListener(_onControllerChanged);
    super.dispose();
  }

  void _onControllerChanged() {
    final draft = widget.controller.draft;
    // Taslak hazır olur olmaz forma geçiliyor; bu ekranda taslağı ikinci kez
    // göstermek, kullanıcıya aynı bilgiyi iki kez onaylatmak olurdu.
    if (draft != null && !widget.controller.isBusy && mounted) {
      if (draft.documentKind.needsDecision) {
        widget.onBankDocumentReady?.call(draft);
        return;
      }

      // İade yeni bir kayıt üretmez; eski bir harcamayı geri alır. Gider
      // formuna göndermek, geri gelen parayı harcanmış göstermek olurdu.
      if (draft.documentKind == ReceiptDocumentKind.refundReceipt) {
        widget.onRefundReady?.call(draft);
        return;
      }

      // Son ödeme tarihi taşıyan belge, ödendiğini söylemez. Doğrudan gider
      // yazmak, henüz çıkmamış parayı çıkmış göstermek olurdu.
      if (draft.needsPaidQuestion && widget.onInvoiceReady != null) {
        widget.onInvoiceReady!(draft);
        return;
      }

      // Taksitli satış tek seferlik tam tutar gideri değildir: 3.000 TL / 3
      // taksitlik bir fişi tek gider yazmak o ayın bütçesini gerçekte çıkmayan
      // 2.000 TL kadar şişirir.
      if (draft.isInstallmentSale && widget.onInstallmentReady != null) {
        widget.onInstallmentReady!(draft);
        return;
      }

      widget.onDraftReady(
        receiptPrefillFrom(
          draft,
          photo: widget.controller.photo,
          keepPhoto: widget.controller.keepPhoto,
        ),
        widget.controller.intent,
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(widget.variant.title)),
      body: AnimatedBuilder(
        animation: widget.controller,
        builder: (context, _) => _buildBody(context),
      ),
    );
  }

  Widget _buildBody(BuildContext context) {
    final controller = widget.controller;
    if (controller.unauthorized) return const AppUnauthorizedView();
    // Rıza okunmadan hiçbir şey teklif edilmiyor: soruyu sormadan kamerayı
    // açmak, fotoğrafın nereye gideceğini söylemeden çekmek olurdu.
    if (controller.consentGranted == null) return const AppLoadingView();
    if (controller.consentGranted == false) return _buildConsent(context);
    if (controller.isBusy) return _buildBusy(context);
    if (controller.errorMessage != null) return _buildError(context);
    return _buildChoices(context);
  }

  /// İlk kullanımdan önce sorulan rıza.
  ///
  /// Fiş fotoğrafı işletme, tarih, tutar ve kimi zaman kart son dört hanesi
  /// taşır; bu veri cihazdan çıkıp Google'a gidiyor. Kullanıcı bunu **önce**
  /// görür. Kabul etmezse özellik kapalı kalır ve elle giriş bozulmaz.
  Widget _buildConsent(BuildContext context) {
    final theme = Theme.of(context);
    return _Sheet(
      children: [
        Text('Belge okuma nasıl çalışır?', style: theme.textTheme.titleMedium),
        const SizedBox(height: AppSpacing.medium),
        Text(
          'Çektiğiniz fotoğraf, okunmak üzere Google Gemini servisine '
          'gönderilir. Belge üzerinde işletme adı, tarih, tutar ve bazen '
          'kart numaranızın son dört hanesi bulunur.',
          style: theme.textTheme.bodyMedium,
        ),
        const SizedBox(height: AppSpacing.small),
        Text(
          'Okunan bilgiler yalnızca öneridir; hiçbir kayıt siz onaylamadan '
          'oluşmaz. Kabul etmezseniz gideri her zamanki gibi elle '
          'girebilirsiniz.',
          style: theme.textTheme.bodyMedium,
        ),
        const SizedBox(height: AppSpacing.large),
        FilledButton(
          onPressed: widget.controller.grantConsent,
          child: const Text('Kabul ediyorum, belgeyi okut'),
        ),
        const SizedBox(height: AppSpacing.large),
        const _WithoutPhotoDivider(),
        const SizedBox(height: AppSpacing.medium),
        _ManualEntryButton(onPressed: _handManualEntry),
      ],
    );
  }

  /// İki bekleme ayrı anlatılıyor: kendi seçimini bekleyen kullanıcı ile
  /// sunucuyu bekleyen kullanıcı aynı şeyi beklemiyor.
  Widget _buildBusy(BuildContext context) {
    final controller = widget.controller;
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Expanded(
          child: AppLoadingView(
            message: controller.isPicking
                ? 'Fotoğraf seçiliyor'
                : 'Belge okunuyor, birkaç saniye sürebilir',
          ),
        ),
        Padding(
          padding: const EdgeInsets.all(AppSpacing.medium),
          child: AppContentWidth(
            child: OutlinedButton(
              onPressed: controller.cancel,
              child: const Text('Vazgeç'),
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildError(BuildContext context) {
    final controller = widget.controller;
    final showBankSlipRetry = controller.isBankSlipMismatch;
    return Column(
      children: [
        Expanded(
          child: AppErrorView(
            message: controller.errorMessage!,
            // Dekontta "yeniden dene" yok: aynı fotoğraf aynı cevabı getirir.
            onRetry: controller.canRetry ? controller.retryAnalysis : null,
          ),
        ),
        Padding(
          padding: const EdgeInsets.all(AppSpacing.medium),
          child: AppContentWidth(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                // Yanlış kapıdan girmek bir hata değil, bir seçim; düzeltmesi
                // de tek dokunuş olmalı. Fotoğraf duruyor, yalnız belge sınıfı
                // değişiyor — kullanıcıyı yeniden fotoğraf çekmeye göndermek
                // yaptığı işi boşa çıkarırdı.
                if (showBankSlipRetry) ...[
                  FilledButton.icon(
                    onPressed: controller.retryAsBankSlip,
                    icon: const Icon(Icons.account_balance_outlined),
                    label: const Text('Dekont olarak okut'),
                  ),
                  const SizedBox(height: AppSpacing.large),
                ],
                const _WithoutPhotoDivider(),
                const SizedBox(height: AppSpacing.medium),
                // Okuma çalışmadığında akış tıkanmıyor: aynı form boş olarak
                // açılıyor. Belgenin okunamaması, harcamanın kaydedilememesi
                // demek değil.
                _ManualEntryButton(onPressed: _handManualEntry),
              ],
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildChoices(BuildContext context) {
    final controller = widget.controller;
    final variant = widget.variant;
    return _Sheet(
      children: [
        if (controller.noImageSelected)
          const _ScanNote('Fotoğraf seçilmedi. Kamera veya galeriden seçin.'),
        if (controller.wasCancelled)
          const _ScanNote('Okuma iptal edildi. Yeniden deneyebilirsiniz.'),
        if (controller.draftIsEmpty)
          const _ScanNote(
            'Belgeden hiçbir alan okunamadı. Daha net bir fotoğraf '
            'deneyebilir veya bilgileri elle girebilirsiniz.',
          ),
        Text(variant.intro, style: Theme.of(context).textTheme.bodyMedium),
        // Yön, fotoğraf çekilmeden önce sorulur. Belgeden okunamaz: aynı kira
        // makbuzu kiracı için gider, ev sahibi için gelirdir ve kâğıt aynıdır.
        // Varsayılan gider, çünkü en sık yol o; bir dokunuş uzamıyor.
        if (variant.asksDirection) ...[
          const SizedBox(height: AppSpacing.large),
          const AppSectionHeader(title: 'Bu belge harcama mı, gelir mi?'),
          _DirectionPicker(
            value: controller.intent,
            onChanged: controller.isBusy ? null : controller.setIntent,
          ),
        ],
        const SizedBox(height: AppSpacing.large),
        // Kamera ve galeri aynı eylemin iki biçimi, bu yüzden aynı ağırlıkta:
        // belge çekildikten saatler sonra da kaydedilebilir, galeri kameranın
        // yedeği değil.
        FilledButton.icon(
          onPressed: () => controller.scan(ReceiptImageOrigin.camera),
          icon: const Icon(Icons.photo_camera_outlined),
          label: const Text('Fotoğraf çek'),
        ),
        const SizedBox(height: AppSpacing.small),
        FilledButton.icon(
          onPressed: () => controller.scan(ReceiptImageOrigin.gallery),
          icon: const Icon(Icons.photo_library_outlined),
          label: const Text('Galeriden seç'),
        ),
        const SizedBox(height: AppSpacing.large),
        // Elle giriş okumanın bir biçimi değil, okumayı atlayan başka bir yol:
        // butonlara yapışmıyor, kendi çerçevesiyle duruyor ve üç durumda da
        // (seçim, hata, rıza) aynı görünüyor.
        const _WithoutPhotoDivider(),
        const SizedBox(height: AppSpacing.medium),
        _ManualEntryButton(onPressed: _handManualEntry),
      ],
    );
  }

  void _handManualEntry() =>
      widget.onDraftReady(null, widget.controller.intent);
}

/// Ekranın ortak kabuğu: kaydırılır, geniş ekranda okunabilir genişlikte kalır
/// ve butonları satır genişliğine yayar.
class _Sheet extends StatelessWidget {
  const _Sheet({required this.children});

  final List<Widget> children;

  @override
  Widget build(BuildContext context) => ListView(
    padding: const EdgeInsets.all(AppSpacing.medium),
    children: [
      AppContentWidth(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: children,
        ),
      ),
    ],
  );
}

/// Yön seçici.
///
/// İki seçenek: harcama ve gelir. İkisi de birer `BudgetTransaction`, yalnız
/// yönleri ters — ve hangisi olduğu belgede yazmaz.
///
/// Dekont burada yok: kendi sayfası var. Transfer de yok; kabul ettiği tek
/// belge türü (kendi hesaplar arası aktarma) dekont sayfasında da okunuyor ve
/// yön oradan sonra açılan karar sayfasında zaten soruluyor. Burada bir
/// seçenek olarak dururken gerçek dekontların çoğunu — üçüncü tarafa yapılan
/// ödemeleri — reddediyordu.
class _DirectionPicker extends StatelessWidget {
  const _DirectionPicker({required this.value, required this.onChanged});

  final ReceiptCaptureIntent value;
  final ValueChanged<ReceiptCaptureIntent>? onChanged;

  @override
  Widget build(BuildContext context) => Semantics(
    label: 'Belge yönü',
    child: SegmentedButton<ReceiptCaptureIntent>(
      segments: const [
        ButtonSegment(
          value: ReceiptCaptureIntent.expense,
          icon: Icon(Icons.south_west),
          label: Text('Harcama'),
        ),
        ButtonSegment(
          value: ReceiptCaptureIntent.income,
          icon: Icon(Icons.north_east),
          label: Text('Gelir'),
        ),
      ],
      selected: {value},
      onSelectionChanged: onChanged == null
          ? null
          : (selection) => onChanged!(selection.first),
    ),
  );
}

/// Fotoğraflı yolları elle girişten ayıran başlıklı çizgi.
class _WithoutPhotoDivider extends StatelessWidget {
  const _WithoutPhotoDivider();

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Row(
      children: [
        const Expanded(child: Divider()),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: AppSpacing.small),
          child: Text(
            'Fotoğraf olmadan',
            style: theme.textTheme.labelMedium?.copyWith(
              color: theme.colorScheme.onSurfaceVariant,
            ),
          ),
        ),
        const Expanded(child: Divider()),
      ],
    );
  }
}

class _ManualEntryButton extends StatelessWidget {
  const _ManualEntryButton({required this.onPressed});

  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) => OutlinedButton.icon(
    onPressed: onPressed,
    icon: const Icon(Icons.edit_outlined),
    label: const Text('Bilgileri elle gir'),
  );
}

class _ScanNote extends StatelessWidget {
  const _ScanNote(this.message);

  final String message;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.only(bottom: AppSpacing.medium),
    child: Semantics(
      liveRegion: true,
      child: Text(message, style: Theme.of(context).textTheme.bodyMedium),
    ),
  );
}
