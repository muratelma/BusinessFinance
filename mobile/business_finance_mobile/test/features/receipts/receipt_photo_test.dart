import 'dart:typed_data';

import 'package:flutter_test/flutter_test.dart';
import 'package:image/image.dart' as img;
import 'package:business_finance_mobile/features/receipts/data/receipt_photo.dart';

void main() {
  group('normalizeSync', () {
    test('brings an oversized photo under the long edge and the byte cap', () {
      // Ölçüm setindeki en büyük fişten de büyük: 12 MP'lik telefon fotoğrafı.
      final bytes = _photoLikeJpeg(width: 3000, height: 4000);
      expect(bytes.length, greaterThan(ReceiptImageNormalizer.maxUploadBytes));

      final photo = ReceiptImageNormalizer.normalizeSync(
        bytes: bytes,
        fileName: 'IMG_0042.jpeg',
        mediaType: 'image/jpeg',
      );

      expect(photo.uploadHeight, ReceiptImageNormalizer.maxLongEdge);
      expect(photo.uploadWidth, lessThan(photo.uploadHeight));
      expect(
        photo.uploadByteCount,
        lessThanOrEqualTo(ReceiptImageNormalizer.maxUploadBytes),
      );
    });

    test('keeps the original bytes untouched next to the upload copy', () {
      final bytes = _photoLikeJpeg(width: 3000, height: 4000);

      final photo = ReceiptImageNormalizer.normalizeSync(
        bytes: bytes,
        fileName: 'IMG_0042.jpeg',
        mediaType: 'image/jpeg',
      );

      // Grup 7'de belge olarak saklanacak olan bu: sıkıştırılmış kopya değil.
      expect(photo.originalBytes, same(bytes));
      expect(photo.originalMediaType, 'image/jpeg');
      expect(photo.uploadByteCount, lessThan(photo.originalByteCount));
    });

    test('falls all the way down the ladder for an incompressible photo', () {
      // Saf gürültü JPEG'de neredeyse hiç sıkışmıyor; gerçek bir fişte
      // görülmez ama merdivenin son basamağının çalıştığını kanıtlıyor.
      final photo = ReceiptImageNormalizer.normalizeSync(
        bytes: _noisyJpeg(width: 3000, height: 4000),
        fileName: 'gurultu.jpg',
        mediaType: 'image/jpeg',
      );

      expect(photo.uploadHeight, 1400);
    });

    test('does not upscale a photo that is already small', () {
      final bytes = Uint8List.fromList(
        img.encodeJpg(img.Image(width: 800, height: 1200), quality: 90),
      );

      final photo = ReceiptImageNormalizer.normalizeSync(
        bytes: bytes,
        fileName: 'kucuk.jpg',
        mediaType: 'image/jpeg',
      );

      expect(photo.uploadWidth, 800);
      expect(photo.uploadHeight, 1200);
    });

    test('bakes the EXIF orientation into the pixels', () {
      // Orientation 6: telefon dik tutulmuş, sensör yan yazmış. Etiket
      // küçültmede kaybolduğu için fişin yönü piksele işlenmeli.
      final source = img.Image(width: 1200, height: 600);
      source.exif.imageIfd.orientation = 6;
      final bytes = Uint8List.fromList(img.encodeJpg(source, quality: 90));

      final photo = ReceiptImageNormalizer.normalizeSync(
        bytes: bytes,
        fileName: 'donuk.jpg',
        mediaType: 'image/jpeg',
      );

      expect(photo.uploadWidth, 600);
      expect(photo.uploadHeight, 1200);
    });

    test('renames the upload copy to .jpg whatever the source was', () {
      final bytes = Uint8List.fromList(
        img.encodePng(img.Image(width: 400, height: 400)),
      );

      final photo = ReceiptImageNormalizer.normalizeSync(
        bytes: bytes,
        fileName: 'gorsel.png',
        mediaType: 'image/png',
      );

      expect(photo.uploadFileName, 'gorsel.jpg');
      // Orijinalin türü, saklanacak belgeyi etiketlediği için korunuyor.
      expect(photo.originalMediaType, 'image/png');
    });

    test('refuses empty and undecodable bytes before spending the quota', () {
      expect(
        () => ReceiptImageNormalizer.normalizeSync(
          bytes: Uint8List(0),
          fileName: 'bos.jpg',
          mediaType: 'image/jpeg',
        ),
        throwsA(isA<ReceiptImageException>()),
      );

      expect(
        () => ReceiptImageNormalizer.normalizeSync(
          bytes: Uint8List.fromList(const [1, 2, 3, 4, 5, 6, 7, 8]),
          fileName: 'bozuk.jpg',
          mediaType: 'image/jpeg',
        ),
        throwsA(isA<ReceiptImageException>()),
      );
    });
  });
}

/// Gerçek bir telefon fotoğrafına yakın entropi: yumuşak geçiş + hafif doku.
///
/// Düz renkli görsel JPEG'de birkaç kilobayta iniyor ve boyut kapısını hiç
/// sınamıyor; saf gürültü ise hiç sıkışmıyor. Fiş fotoğrafı ikisinin arasında.
Uint8List _photoLikeJpeg({required int width, required int height}) {
  final image = img.Image(width: width, height: height);
  var seed = 11;
  for (var y = 0; y < height; y++) {
    for (var x = 0; x < width; x++) {
      seed = (seed * 1103515245 + 12345) & 0x7fffffff;
      final texture = (seed >> 16) % 24;
      final base = 40 + (x * 180 ~/ width);
      final shade = (base + texture).clamp(0, 255);
      image.setPixelRgb(x, y, shade, shade, (shade + 12).clamp(0, 255));
    }
  }
  return Uint8List.fromList(img.encodeJpg(image, quality: 95));
}

/// Hiç sıkışmayan patolojik girdi: her piksel bağımsız rastgele.
Uint8List _noisyJpeg({required int width, required int height}) {
  final image = img.Image(width: width, height: height);
  var seed = 7;
  for (var y = 0; y < height; y++) {
    for (var x = 0; x < width; x++) {
      seed = (seed * 1103515245 + 12345) & 0x7fffffff;
      image.setPixelRgb(
        x,
        y,
        seed & 0xff,
        (seed >> 8) & 0xff,
        (seed >> 16) & 0xff,
      );
    }
  }
  return Uint8List.fromList(img.encodeJpg(image, quality: 95));
}
