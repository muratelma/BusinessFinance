import 'package:flutter/foundation.dart';

import '../../../core/network/api_exception.dart';
import '../data/receipt_image_source.dart';
import '../data/receipt_models.dart';
import '../data/receipt_photo.dart';
import '../data/receipt_preferences.dart';
import '../data/receipt_repository.dart';

/// Fiş çekme akışının durumu: seç → hazırla → okut → taslak.
///
/// Akış hiçbir yere para yazmaz. Ürettiği tek şey, kullanıcının Grup 6'da
/// onaylayacağı taslak ve Grup 7'de saklanabilecek orijinal fotoğraftır.
class ReceiptScanController extends ChangeNotifier {
  ReceiptScanController(
    this._source,
    this._repository,
    this._normalizer,
    this._preferences, {
    this.intent = ReceiptCaptureIntent.expense,
  });

  final ReceiptImageSourceContract _source;
  final ReceiptRepositoryContract _repository;
  final ReceiptImageNormalizerContract _normalizer;
  final ReceiptPreferencesContract _preferences;

  /// Hangi koşumun sonucunun geçerli olduğunu belirler.
  ///
  /// İptal ve yeni bir tarama bu sayacı artırır; geç dönen cevap kendi
  /// numarasını bulamayınca sessizce düşer. Ağ isteği gerçekten kesilmiyor —
  /// sunucu okumayı bitirir — ama sonucu ekrana basılmaz. İptali "istek
  /// durduruldu" diye anlatmıyoruz, çünkü durmuyor.
  int _generation = 0;

  /// Kullanıcı fotoğrafın Google'a gideceğini kabul etti mi.
  ///
  /// Depodan okunana kadar `null`: "henüz bilmiyoruz" ile "hayır" aynı şey
  /// değil, ilkinde soru sorulur, ikincisinde özellik kapalı kalır.
  bool? consentGranted;

  /// Kaydedilen giderin yanına orijinal fotoğraf eklensin mi.
  bool keepPhoto = true;

  /// Kullanıcının bildirdiği yön; okuma bununla yapılır.
  ///
  /// Varsayılan gider: en sık yol bir dokunuş uzamasın. Dekont sayfası bunu
  /// `bankSlip` olarak kurar ve hiç sormaz — kullanıcı `İşlem ekle` menüsünde
  /// zaten elindeki belgenin ne olduğunu söyledi. Yeniden okutmada da aynı yön
  /// kullanılır: kullanıcı yönü değiştirdiyse yeni okuma o yönle yapılır, eski
  /// taslak değil.
  ReceiptCaptureIntent intent;

  void setIntent(ReceiptCaptureIntent value) {
    if (intent == value) return;
    intent = value;
    notifyListeners();
  }

  bool isPicking = false;
  bool isAnalyzing = false;

  /// Kullanıcı seçim ekranından fotoğraf seçmeden çıktı.
  bool noImageSelected = false;

  bool wasCancelled = false;
  bool unauthorized = false;
  String? errorMessage;
  String? errorCode;

  ReceiptPhoto? photo;
  ReceiptDraft? draft;

  bool get isBusy => isPicking || isAnalyzing;

  /// Sunucu cevap verdi ama tek alan bile okunamadı.
  ///
  /// Teknik olarak başarılı bir yanıt; kullanıcı açısından boş sonuç. Ekranın
  /// bunu "okundu" diye göstermemesi gerekiyor.
  bool get draftIsEmpty => draft?.isEmpty ?? false;

  /// Ekranda duran taslak bir önceki fişe ait, son deneme başarısız oldu.
  ///
  /// Taslak hata anında silinmiyor: kullanıcı okunmuş bir fişi ikinci deneme
  /// yüzünden kaybetmemeli. Ama ekranın bunun **eski** taslak olduğunu
  /// söylemesi şart, yoksa kullanıcı yeni fişin okunduğunu sanır.
  bool get isStale => draft != null && errorMessage != null;

  /// Aynı fotoğrafı yeniden okutmanın anlamı var mı.
  ///
  /// Yalnız **geçici** hatalarda: sağlayıcıya ulaşılamadı, kota doldu, zaman
  /// aşımı. Sunucu "bu bir banka dekontu" ya da "bu fiş değil" dediyse aynı
  /// fotoğrafı yeniden yollamak aynı cevabı getirir; düğmeyi göstermek
  /// kullanıcıya olmayan bir çıkış yolu vaat etmek olurdu.
  bool get canRetry =>
      photo != null && !isBusy && _transientErrorCodes.contains(errorCode);

  static const Set<String> _transientErrorCodes = {
    'receipt.provider_unavailable',
    'receipt.provider_rate_limited',
    'rate_limit.exceeded',
    'network.timeout',
    'network.unavailable',
    'server.unexpected_error',
  };

  /// Fotoğraf bir banka dekontu ama yanlış seçenekle okutuldu.
  ///
  /// Okuma hatası değil, seçim hatası — ve düzeltmesi tek dokunuş: aynı
  /// fotoğraf `Dekont` seçeneğiyle yeniden okunur. Kullanıcıyı fotoğrafı
  /// yeniden çekmeye göndermek, yaptığı işi boşa çıkarmak olurdu.
  bool get isBankSlipMismatch =>
      photo != null && _bankSlipErrorCodes.contains(errorCode);

  static const Set<String> _bankSlipErrorCodes = {
    'receipt.bank_document',
    'receipt.bank_payment_not_transfer',
  };

  /// Aynı fotoğrafı `Dekont` seçeneğiyle yeniden okur.
  Future<void> retryAsBankSlip() async {
    if (photo == null || isBusy) return;
    intent = ReceiptCaptureIntent.bankSlip;
    await retryAnalysis();
  }

  /// Cihazda hatırlananı okur. Ekran açılır açılmaz çağrılır.
  Future<void> loadPreferences() async {
    consentGranted = await _preferences.readConsent();
    keepPhoto = await _preferences.readKeepPhoto();
    notifyListeners();
  }

  /// Rızayı kaydeder. Fotoğraf ancak bundan **sonra** sağlayıcıya gidebilir.
  Future<void> grantConsent() async {
    await _preferences.writeConsent(true);
    consentGranted = true;
    notifyListeners();
  }

  Future<void> setKeepPhoto(bool keep) async {
    keepPhoto = keep;
    await _preferences.writeKeepPhoto(keep);
    notifyListeners();
  }

  Future<void> scan(ReceiptImageOrigin origin) async {
    // Rıza kapısı burada, çağıranda değil: ekranın bir yerini atlayan ikinci
    // bir yol açılırsa fotoğraf yine de onaysız gitmemeli.
    if (consentGranted != true) return;
    if (isBusy) return;
    final generation = ++_generation;
    isPicking = true;
    noImageSelected = false;
    wasCancelled = false;
    errorMessage = null;
    errorCode = null;
    notifyListeners();

    try {
      final picked = await _source.pick(origin);
      if (generation != _generation) return;
      if (picked == null) {
        noImageSelected = true;
        return;
      }

      isPicking = false;
      isAnalyzing = true;
      notifyListeners();

      final prepared = await _normalizer.normalize(
        bytes: picked.bytes,
        fileName: picked.fileName,
        mediaType: picked.mediaType,
      );
      if (generation != _generation) return;

      // Fotoğraf okumadan **önce** saklanıyor: okuma başarısız olsa da elde
      // hazır kopya kalsın ki kullanıcı fişi yeniden çekmek zorunda kalmasın.
      photo = prepared;

      final result = await _repository.analyze(prepared, intent);
      if (generation != _generation) return;

      draft = result;
      unauthorized = false;
    } catch (error) {
      if (generation != _generation) return;
      _recordFailure(error);
    } finally {
      if (generation == _generation) {
        isPicking = false;
        isAnalyzing = false;
        notifyListeners();
      }
    }
  }

  /// Elde duran fotoğrafı yeniden okutur; kullanıcı fişi tekrar çekmez.
  Future<void> retryAnalysis() async {
    final prepared = photo;
    if (prepared == null || isBusy) return;
    final generation = ++_generation;
    isAnalyzing = true;
    wasCancelled = false;
    errorMessage = null;
    errorCode = null;
    notifyListeners();

    try {
      final result = await _repository.analyze(prepared, intent);
      if (generation != _generation) return;
      draft = result;
      unauthorized = false;
    } catch (error) {
      if (generation != _generation) return;
      _recordFailure(error);
    } finally {
      if (generation == _generation) {
        isAnalyzing = false;
        notifyListeners();
      }
    }
  }

  /// Bekleyen koşumun sonucunu geçersiz kılar.
  void cancel() {
    if (!isBusy) return;
    _generation++;
    isPicking = false;
    isAnalyzing = false;
    wasCancelled = true;
    errorMessage = null;
    errorCode = null;
    notifyListeners();
  }

  /// Akışı baştan başlatır; fotoğraf da taslak da bellekten düşer.
  void reset() {
    _generation++;
    isPicking = false;
    isAnalyzing = false;
    noImageSelected = false;
    wasCancelled = false;
    unauthorized = false;
    errorMessage = null;
    errorCode = null;
    photo = null;
    draft = null;
    notifyListeners();
  }

  void _recordFailure(Object error) {
    switch (error) {
      case ReceiptImageException():
        errorCode = 'receipt.image_unreadable';
        errorMessage = error.message;
      case ApiException():
        unauthorized = error.isUnauthorized;
        errorCode = error.code;
        errorMessage = error.message;
      case FormatException():
        errorCode = 'response.invalid_format';
        errorMessage = 'Sunucudan beklenmeyen bir fiş yanıtı alındı.';
      default:
        errorCode = 'receipt.capture_failed';
        errorMessage = 'Fotoğraf alınamadı. Tekrar deneyin.';
    }
  }
}
