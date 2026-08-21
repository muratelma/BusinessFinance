import 'package:flutter/foundation.dart';
import 'package:image/image.dart' as img;

/// Çekilen fişin iki ayrı hâli.
///
/// İkisi de tutuluyor çünkü iki farklı işe yarıyorlar ve biri diğerinin yerine
/// geçmiyor: analize **küçültülmüş** kopya gider (5 MiB sınırı ve ağ süresi),
/// belge olarak saklanacaksa **orijinal** fotoğraf yazılır (Grup 7). Fişin
/// kanıt değeri sıkıştırılmamış hâlindedir; küçültülmüş kopyayı kanıt diye
/// saklamak, kullanıcının sonradan okuyamayacağı bir belge bırakır.
class ReceiptPhoto {
  const ReceiptPhoto({
    required this.originalBytes,
    required this.originalFileName,
    required this.originalMediaType,
    required this.uploadBytes,
    required this.uploadFileName,
    required this.uploadWidth,
    required this.uploadHeight,
  });

  /// Cihazdan geldiği hâliyle, hiç dokunulmamış byte'lar.
  final Uint8List originalBytes;
  final String originalFileName;
  final String originalMediaType;

  /// Analize gönderilecek, döndürülmüş ve küçültülmüş JPEG.
  final Uint8List uploadBytes;
  final String uploadFileName;
  final int uploadWidth;
  final int uploadHeight;

  int get uploadByteCount => uploadBytes.length;
  int get originalByteCount => originalBytes.length;
}

/// Fotoğraf cihazda okunamadığında atılır.
///
/// Sunucuya gitmeden burada durmak bilinçli: okunamayan byte'ları yüklemek
/// kullanıcının kotasını ve süresini, kesin bilinen bir sonuç için harcar.
class ReceiptImageException implements Exception {
  const ReceiptImageException(this.message);

  final String message;

  @override
  String toString() => 'ReceiptImageException($message)';
}

abstract interface class ReceiptImageNormalizerContract {
  Future<ReceiptPhoto> normalize({
    required Uint8List bytes,
    required String fileName,
    required String mediaType,
  });
}

/// Analize gidecek kopyayı hazırlar: EXIF yönünü piksele işler, uzun kenarı
/// kısar, JPEG olarak kodlar.
///
/// Üç iş de istemcide yapılıyor çünkü üçü de ağa çıkmadan önce kazanç sağlıyor:
/// döndürülmemiş fiş yan yatık gider, 12 MP fotoğraf 5 MiB sınırına takılır ve
/// takılmasa bile yüklenmesi okumadan uzun sürer.
class ReceiptImageNormalizer implements ReceiptImageNormalizerContract {
  const ReceiptImageNormalizer();

  /// Uzun kenar hedefi. Fiş yazısı bu ölçekte okunaklı kalıyor; ölçüm setindeki
  /// en büyük görsel (1800×3000) de bu sınırın altına iniyor.
  static const int maxLongEdge = 2400;

  /// JPEG kalitesi. 85 fotoğraf sıkıştırmasında metin kenarlarını bozmayan
  /// alışılmış eşiktir.
  static const int preferredQuality = 85;

  /// Sunucu 5 MiB kabul ediyor; hedef bunun epey altı, çünkü mobil bağlantıda
  /// yükleme süresi okuma süresine ekleniyor.
  static const int maxUploadBytes = 2 * 1024 * 1024;

  @override
  Future<ReceiptPhoto> normalize({
    required Uint8List bytes,
    required String fileName,
    required String mediaType,
  }) => compute(
    _normalizeInIsolate,
    _NormalizeRequest(bytes: bytes, fileName: fileName, mediaType: mediaType),
  );

  /// Isolate kullanmadan aynı işi yapar.
  ///
  /// Görsel çözme ve JPEG kodlama saniyeler sürebiliyor; uygulama bunu
  /// [compute] ile arka plana atar, testler ise doğrudan buradan çağırır.
  static ReceiptPhoto normalizeSync({
    required Uint8List bytes,
    required String fileName,
    required String mediaType,
  }) {
    if (bytes.isEmpty) {
      throw const ReceiptImageException('Seçilen dosya boş.');
    }

    final decoded = img.decodeImage(bytes);
    if (decoded == null) {
      throw const ReceiptImageException(
        'Fotoğraf okunamadı. Farklı bir görsel deneyin.',
      );
    }

    // EXIF yön etiketi metadata'dır; küçültme sırasında kaybolur ve fiş yan
    // yatık kalır. Yön önce piksele işleniyor.
    final upright = img.bakeOrientation(decoded);

    var quality = preferredQuality;
    var longEdge = maxLongEdge;
    late Uint8List encoded;
    late img.Image scaled;

    // Önce kaliteyi, sonra çözünürlüğü düşürüyoruz: kalite kaybı metnin
    // okunmasını, boyut kaybı harflerin varlığını etkiler.
    for (final step in _reductionSteps) {
      quality = step.quality;
      longEdge = step.longEdge;
      scaled = _fitLongEdge(upright, longEdge);
      encoded = img.encodeJpg(scaled, quality: quality);
      if (encoded.length <= maxUploadBytes) {
        break;
      }
    }

    return ReceiptPhoto(
      originalBytes: bytes,
      originalFileName: fileName,
      originalMediaType: mediaType,
      uploadBytes: encoded,
      uploadFileName: _jpegName(fileName),
      uploadWidth: scaled.width,
      uploadHeight: scaled.height,
    );
  }

  /// Sırayla denenen küçültme adımları. Son adım bile sınırı aşarsa yüklenen
  /// dosya yine de gönderilir: sunucunun 5 MiB kapısı asıl sınırdır ve bu
  /// noktada tahmin yerine sunucunun cevabı beklenir.
  static const List<_ReductionStep> _reductionSteps = [
    _ReductionStep(quality: preferredQuality, longEdge: maxLongEdge),
    _ReductionStep(quality: 75, longEdge: maxLongEdge),
    _ReductionStep(quality: 70, longEdge: 1800),
    _ReductionStep(quality: 65, longEdge: 1400),
  ];

  /// Yalnız küçültür. Küçük fotoğrafı büyütmek bilgi eklemez, dosyayı ve
  /// yükleme süresini büyütür.
  static img.Image _fitLongEdge(img.Image source, int longEdge) {
    final currentLongEdge = source.width >= source.height
        ? source.width
        : source.height;
    if (currentLongEdge <= longEdge) {
      return source;
    }
    return source.width >= source.height
        ? img.copyResize(source, width: longEdge)
        : img.copyResize(source, height: longEdge);
  }

  static String _jpegName(String fileName) {
    final trimmed = fileName.trim();
    final base = trimmed.isEmpty ? 'fis' : trimmed.split(RegExp(r'[\\/]')).last;
    final dotIndex = base.lastIndexOf('.');
    final withoutExtension = dotIndex > 0 ? base.substring(0, dotIndex) : base;
    return '$withoutExtension.jpg';
  }
}

class _ReductionStep {
  const _ReductionStep({required this.quality, required this.longEdge});

  final int quality;
  final int longEdge;
}

class _NormalizeRequest {
  const _NormalizeRequest({
    required this.bytes,
    required this.fileName,
    required this.mediaType,
  });

  final Uint8List bytes;
  final String fileName;
  final String mediaType;
}

ReceiptPhoto _normalizeInIsolate(_NormalizeRequest request) =>
    ReceiptImageNormalizer.normalizeSync(
      bytes: request.bytes,
      fileName: request.fileName,
      mediaType: request.mediaType,
    );
