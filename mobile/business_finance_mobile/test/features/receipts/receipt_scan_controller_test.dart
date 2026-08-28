import 'dart:async';
import 'dart:typed_data';

import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/network/api_exception.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_image_source.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_models.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_photo.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_preferences.dart';
import 'package:business_finance_mobile/features/receipts/data/receipt_repository.dart';
import 'package:business_finance_mobile/features/receipts/presentation/receipt_scan_controller.dart';

void main() {
  test('reports picking and analyzing as two separate waits', () async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    final controller = _controller(source, repository);
    final observed = <String>[];
    controller.addListener(() {
      observed.add(
        controller.isPicking
            ? 'picking'
            : controller.isAnalyzing
            ? 'analyzing'
            : 'idle',
      );
    });

    final pending = controller.scan(ReceiptImageOrigin.camera);
    source.complete(_picked());
    await Future<void>.delayed(Duration.zero);
    repository.complete(_draft());
    await pending;

    // Kullanıcı iki farklı şey bekliyor: önce kendi seçimi, sonra sunucu.
    // Tek bir "yükleniyor" göstergesi ikisini aynı şey sanmasına yol açardı.
    expect(observed, ['picking', 'analyzing', 'idle']);
    expect(controller.draft, isNotNull);
    expect(controller.photo, isNotNull);
  });

  test('treats a closed picker as empty, not as an error', () async {
    final source = _FakeSource();
    final controller = _controller(source, _FakeRepository());

    final pending = controller.scan(ReceiptImageOrigin.gallery);
    source.complete(null);
    await pending;

    expect(controller.noImageSelected, isTrue);
    expect(controller.errorMessage, isNull);
    expect(controller.draft, isNull);
  });

  test('surfaces the server message and code on a provider failure', () async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    final controller = _controller(source, repository);

    final pending = controller.scan(ReceiptImageOrigin.camera);
    source.complete(_picked());
    await Future<void>.delayed(Duration.zero);
    repository.fail(
      const ApiException(
        statusCode: 503,
        code: 'receipt.provider_unavailable',
        message: 'Fiş okuma servisine şu an ulaşılamıyor.',
      ),
    );
    await pending;

    expect(controller.errorCode, 'receipt.provider_unavailable');
    expect(controller.errorMessage, contains('ulaşılamıyor'));
    expect(controller.unauthorized, isFalse);
    // Fotoğraf elde kaldığı için kullanıcı fişi yeniden çekmek zorunda değil.
    expect(controller.canRetry, isTrue);
  });

  test('marks an expired session as unauthorized', () async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    final controller = _controller(source, repository);

    final pending = controller.scan(ReceiptImageOrigin.camera);
    source.complete(_picked());
    await Future<void>.delayed(Duration.zero);
    repository.fail(
      const ApiException(
        statusCode: 401,
        code: 'authentication.required',
        message: 'Oturumunuz sona erdi.',
      ),
    );
    await pending;

    expect(controller.unauthorized, isTrue);
  });

  test(
    'keeps the earlier draft after a failed second reading, marked stale',
    () async {
      final source = _FakeSource();
      final repository = _FakeRepository();
      final controller = _controller(source, repository);

      final first = controller.scan(ReceiptImageOrigin.camera);
      source.complete(_picked());
      await Future<void>.delayed(Duration.zero);
      repository.complete(_draft(merchant: 'TEST MARKET 01'));
      await first;

      final second = controller.scan(ReceiptImageOrigin.camera);
      source.complete(_picked());
      await Future<void>.delayed(Duration.zero);
      repository.fail(ApiException.timeout());
      await second;

      // Okunmuş fiş ikinci deneme yüzünden kaybolmuyor; ama ekranın bunun eski
      // taslak olduğunu söylemesi için işaretleniyor.
      expect(controller.draft?.counterpartyName, 'TEST MARKET 01');
      expect(controller.isStale, isTrue);
    },
  );

  test('drops the result of a cancelled reading', () async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    final controller = _controller(source, repository);

    final pending = controller.scan(ReceiptImageOrigin.camera);
    source.complete(_picked());
    await Future<void>.delayed(Duration.zero);
    controller.cancel();
    repository.complete(_draft(merchant: 'GEÇ GELEN'));
    await pending;

    expect(controller.wasCancelled, isTrue);
    expect(controller.draft, isNull);
    expect(controller.isBusy, isFalse);
    expect(controller.errorMessage, isNull);
  });

  test(
    're-reads the photo already in hand without opening the picker',
    () async {
      final source = _FakeSource();
      final repository = _FakeRepository();
      final controller = _controller(source, repository);

      final first = controller.scan(ReceiptImageOrigin.camera);
      source.complete(_picked());
      await Future<void>.delayed(Duration.zero);
      repository.fail(ApiException.timeout());
      await first;

      final retry = controller.retryAnalysis();
      repository.complete(_draft(merchant: 'İKİNCİ OKUMA'));
      await retry;

      expect(source.pickCount, 1);
      expect(repository.analyzeCount, 2);
      expect(controller.draft?.counterpartyName, 'İKİNCİ OKUMA');
      expect(controller.isStale, isFalse);
    },
  );

  test(
    'flags a reading where the server could not read a single field',
    () async {
      final source = _FakeSource();
      final repository = _FakeRepository();
      final controller = _controller(source, repository);

      final pending = controller.scan(ReceiptImageOrigin.camera);
      source.complete(_picked());
      await Future<void>.delayed(Duration.zero);
      repository.complete(_emptyDraft());
      await pending;

      // Teknik olarak başarı; kullanıcı açısından boş sonuç.
      expect(controller.errorMessage, isNull);
      expect(controller.draftIsEmpty, isTrue);
    },
  );

  test('stops an undecodable photo on the device', () async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    final controller = _controller(
      source,
      repository,
      normalizer: _FailingNormalizer(),
    );

    final pending = controller.scan(ReceiptImageOrigin.gallery);
    source.complete(_picked());
    await pending;

    expect(controller.errorCode, 'receipt.image_unreadable');
    expect(repository.analyzeCount, 0);
    expect(controller.canRetry, isFalse);
  });

  test('reports an unexpected response shape as such', () async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    final controller = _controller(source, repository);

    final pending = controller.scan(ReceiptImageOrigin.camera);
    source.complete(_picked());
    await Future<void>.delayed(Duration.zero);
    repository.fail(const FormatException('missing counterpartyState'));
    await pending;

    expect(controller.errorCode, 'response.invalid_format');
  });

  test(
    'reset clears the photo so nothing is carried into the next receipt',
    () async {
      final source = _FakeSource();
      final repository = _FakeRepository();
      final controller = _controller(source, repository);

      final pending = controller.scan(ReceiptImageOrigin.camera);
      source.complete(_picked());
      await Future<void>.delayed(Duration.zero);
      repository.complete(_draft());
      await pending;

      controller.reset();

      expect(controller.draft, isNull);
      expect(controller.photo, isNull);
      expect(controller.canRetry, isFalse);
    },
  );

  test('does not touch the camera before consent is given', () async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    final preferences = _FakePreferences(consent: false);
    final controller = _controller(
      source,
      repository,
      preferences: preferences,
    );

    await controller.scan(ReceiptImageOrigin.camera);

    // Fotoğrafın nereye gideceği söylenmeden çekilmiyor.
    expect(source.pickCount, 0);
    expect(repository.analyzeCount, 0);
  });

  test('reads consent and the keep-photo switch from the device', () async {
    final preferences = _FakePreferences(consent: false, keepPhoto: false);
    final controller = _controller(
      _FakeSource(),
      _FakeRepository(),
      preferences: preferences,
    );

    await controller.loadPreferences();

    expect(controller.consentGranted, isFalse);
    expect(controller.keepPhoto, isFalse);
  });

  test('remembers consent and the switch once they change', () async {
    final preferences = _FakePreferences(consent: false);
    final controller = _controller(
      _FakeSource(),
      _FakeRepository(),
      preferences: preferences,
    );

    await controller.grantConsent();
    await controller.setKeepPhoto(false);

    expect(preferences.consent, isTrue);
    expect(preferences.keepPhoto, isFalse);
    expect(controller.keepPhoto, isFalse);
  });

  test('does not offer a retry for an answer that will not change', () async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    final controller = _controller(source, repository);

    final pending = controller.scan(ReceiptImageOrigin.camera);
    source.complete(_picked());
    await Future<void>.delayed(Duration.zero);
    repository.fail(
      const ApiException(
        statusCode: 400,
        code: 'receipt.bank_document',
        message: 'Bu bir banka dekontu.',
      ),
    );
    await pending;

    // Aynı fotoğrafı yeniden yollamak aynı cevabı getirir; "yeniden dene"
    // olmayan bir çıkış yolu vaat ederdi.
    expect(controller.canRetry, isFalse);
    expect(controller.isBankSlipMismatch, isTrue);
  });

  // Yanlış seçenek bir hata değil bir seçim; düzeltmesi tek dokunuş olmalı ve
  // kullanıcıyı fotoğrafı yeniden çekmeye göndermemeli.
  test('re-reads the same photo as a bank slip', () async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    final controller = _controller(source, repository);

    final pending = controller.scan(ReceiptImageOrigin.camera);
    source.complete(_picked());
    await Future<void>.delayed(Duration.zero);
    repository.fail(
      const ApiException(
        statusCode: 400,
        code: 'receipt.bank_payment_not_transfer',
        message: 'Bu dekont başkasına yapılan bir ödeme.',
      ),
    );
    await pending;
    expect(controller.isBankSlipMismatch, isTrue);

    final retry = controller.retryAsBankSlip();
    await Future<void>.delayed(Duration.zero);
    repository.complete(_draft());
    await retry;

    expect(controller.intent, ReceiptCaptureIntent.bankSlip);
    expect(repository.lastIntent, ReceiptCaptureIntent.bankSlip);
    expect(controller.draft, isNotNull);
    expect(controller.errorMessage, isNull);
  });

  test('still offers a retry when the provider was only unavailable', () async {
    final source = _FakeSource();
    final repository = _FakeRepository();
    final controller = _controller(source, repository);

    final pending = controller.scan(ReceiptImageOrigin.camera);
    source.complete(_picked());
    await Future<void>.delayed(Duration.zero);
    repository.fail(
      const ApiException(
        statusCode: 503,
        code: 'receipt.provider_unavailable',
        message: 'Servise ulaşılamıyor.',
      ),
    );
    await pending;

    expect(controller.canRetry, isTrue);
    expect(controller.isBankSlipMismatch, isFalse);
  });
}

ReceiptScanController _controller(
  _FakeSource source,
  _FakeRepository repository, {
  ReceiptImageNormalizerContract? normalizer,
  _FakePreferences? preferences,
}) {
  final controller = ReceiptScanController(
    source,
    repository,
    normalizer ?? _PassThroughNormalizer(),
    preferences ?? _FakePreferences(),
  );
  // Testlerin çoğu rızanın verilmiş olduğu hâli sınıyor; rıza kapısı kendi
  // testinde sürülüyor.
  controller.consentGranted = preferences?.consent ?? true;
  return controller;
}

PickedReceiptImage _picked() => PickedReceiptImage(
  bytes: Uint8List.fromList(const [1, 2, 3]),
  fileName: 'fis.jpg',
  mediaType: 'image/jpeg',
);

ReceiptDraft _draft({String merchant = 'TEST MARKET'}) => ReceiptDraft(
  documentKind: ReceiptDocumentKind.purchaseReceipt,
  counterpartyName: merchant,
  counterpartyState: ReceiptFieldState.read,
  purchasedAt: '2026-08-18',
  purchasedAtState: ReceiptFieldState.read,
  dueDate: null,
  dueDateState: ReceiptFieldState.missing,
  totalAmount: '847.5000',
  totalAmountState: ReceiptFieldState.read,
  vat: null,
  vatState: ReceiptFieldState.missing,
  feeAmount: null,
  feeAmountState: ReceiptFieldState.missing,
  installmentCount: null,
  currencyCode: 'TRY',
  paymentHint: ReceiptPaymentHint.card,
  categoryId: 'category-1',
  categoryName: 'Market Alışverişi',
  categoryState: ReceiptFieldState.read,
  warnings: const [],
);

ReceiptDraft _emptyDraft() => const ReceiptDraft(
  documentKind: ReceiptDocumentKind.purchaseReceipt,
  counterpartyName: null,
  counterpartyState: ReceiptFieldState.missing,
  purchasedAt: null,
  purchasedAtState: ReceiptFieldState.missing,
  dueDate: null,
  dueDateState: ReceiptFieldState.missing,
  totalAmount: null,
  totalAmountState: ReceiptFieldState.missing,
  vat: null,
  vatState: ReceiptFieldState.missing,
  feeAmount: null,
  feeAmountState: ReceiptFieldState.missing,
  installmentCount: null,
  currencyCode: null,
  paymentHint: ReceiptPaymentHint.unknown,
  categoryId: null,
  categoryName: null,
  categoryState: ReceiptFieldState.missing,
  warnings: [],
);

class _FakeSource implements ReceiptImageSourceContract {
  Completer<PickedReceiptImage?>? _pending;
  int pickCount = 0;

  @override
  Future<PickedReceiptImage?> pick(ReceiptImageOrigin origin) {
    pickCount++;
    final completer = Completer<PickedReceiptImage?>();
    _pending = completer;
    return completer.future;
  }

  void complete(PickedReceiptImage? image) => _pending!.complete(image);
}

class _FakeRepository implements ReceiptRepositoryContract {
  Completer<ReceiptDraft>? _pending;
  int analyzeCount = 0;
  ReceiptCaptureIntent? lastIntent;

  @override
  Future<ReceiptDraft> analyze(
    ReceiptPhoto photo,
    ReceiptCaptureIntent intent,
  ) {
    analyzeCount++;
    lastIntent = intent;
    final completer = Completer<ReceiptDraft>();
    _pending = completer;
    return completer.future;
  }

  void complete(ReceiptDraft draft) => _pending!.complete(draft);

  void fail(Object error) => _pending!.completeError(error);
}

class _PassThroughNormalizer implements ReceiptImageNormalizerContract {
  @override
  Future<ReceiptPhoto> normalize({
    required Uint8List bytes,
    required String fileName,
    required String mediaType,
  }) async => ReceiptPhoto(
    originalBytes: bytes,
    originalFileName: fileName,
    originalMediaType: mediaType,
    uploadBytes: bytes,
    uploadFileName: fileName,
    uploadWidth: 100,
    uploadHeight: 200,
  );
}

class _FailingNormalizer implements ReceiptImageNormalizerContract {
  @override
  Future<ReceiptPhoto> normalize({
    required Uint8List bytes,
    required String fileName,
    required String mediaType,
  }) async => throw const ReceiptImageException('Fotoğraf okunamadı.');
}

class _FakePreferences implements ReceiptPreferencesContract {
  _FakePreferences({this.consent = true, this.keepPhoto = true});

  bool consent;
  bool keepPhoto;

  @override
  Future<bool> readConsent() async => consent;

  @override
  Future<void> writeConsent(bool granted) async => consent = granted;

  @override
  Future<bool> readKeepPhoto() async => keepPhoto;

  @override
  Future<void> writeKeepPhoto(bool keep) async => keepPhoto = keep;
}
