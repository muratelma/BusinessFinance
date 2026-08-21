import '../../../core/network/api_client.dart';
import 'receipt_models.dart';
import 'receipt_photo.dart';

abstract interface class ReceiptRepositoryContract {
  Future<ReceiptDraft> analyze(ReceiptPhoto photo, ReceiptCaptureIntent intent);
}

class ReceiptRepository implements ReceiptRepositoryContract {
  const ReceiptRepository(this._client);

  /// Bu tek çağrı için istemci bütçesi.
  ///
  /// Sunucu sağlayıcıya 60 saniye tanıyor; istemcinin ondan önce vazgeçmesi,
  /// okunmuş bir fişi kullanıcıya "zaman aşımı" diye göstermek olurdu. 75
  /// saniye, sunucu bütçesinin üstüne yükleme ve yanıt payı ekler.
  static const Duration analysisTimeout = Duration(seconds: 75);

  final ApiClient _client;

  @override
  Future<ReceiptDraft> analyze(
    ReceiptPhoto photo,
    ReceiptCaptureIntent intent,
  ) async => ReceiptDraft.fromJson(
    (await _client.postMultipart(
      '/api/v1/receipts/analyze',
      upload: ApiUpload(
        bytes: photo.uploadBytes,
        fileName: photo.uploadFileName,
        mediaType: 'image/jpeg',
      ),
      fields: {'intent': intent.wireValue},
      timeout: analysisTimeout,
    )).requireObject(),
  );
}
