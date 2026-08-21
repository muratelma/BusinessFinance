import 'dart:typed_data';

import 'package:image_picker/image_picker.dart';

/// Fotoğrafın nereden geldiği.
///
/// İki değer eşittir: galeriden seçmek kameranın yedeği değil, kendi başına bir
/// yoldur (fiş çekildikten saatler sonra kaydedilebilir).
enum ReceiptImageOrigin { camera, gallery }

/// Cihazdan gelen ham fotoğraf.
class PickedReceiptImage {
  const PickedReceiptImage({
    required this.bytes,
    required this.fileName,
    required this.mediaType,
  });

  final Uint8List bytes;
  final String fileName;
  final String mediaType;
}

abstract interface class ReceiptImageSourceContract {
  /// Kullanıcı seçmeden çıkarsa `null` döner — bu bir hata değil, vazgeçmedir.
  Future<PickedReceiptImage?> pick(ReceiptImageOrigin origin);
}

class ImagePickerReceiptImageSource implements ReceiptImageSourceContract {
  ImagePickerReceiptImageSource([ImagePicker? picker])
    : _picker = picker ?? ImagePicker();

  final ImagePicker _picker;

  @override
  Future<PickedReceiptImage?> pick(ReceiptImageOrigin origin) async {
    // Bilerek `maxWidth`/`imageQuality` verilmiyor: eklenti bu durumda
    // küçültülmüş **yeni** bir dosya döndürür ve orijinal byte'lara erişim
    // kalmaz. Belge olarak saklanacak olan orijinaldir; küçültmeyi
    // `ReceiptImageNormalizer` yapar ve iki hâli de elde tutar.
    final file = await _picker.pickImage(
      source: origin == ReceiptImageOrigin.camera
          ? ImageSource.camera
          : ImageSource.gallery,
    );
    if (file == null) {
      return null;
    }
    return PickedReceiptImage(
      bytes: await file.readAsBytes(),
      fileName: file.name,
      mediaType: file.mimeType ?? _mediaTypeFromName(file.name),
    );
  }

  /// Bazı cihazlar galeri dosyası için MIME bildirmiyor. Tahmin yalnız
  /// **orijinali** etiketlemek için; analize giden kopya her hâlükârda yeniden
  /// JPEG olarak kodlanıyor, yani yanlış tahmin okumayı bozmaz.
  static String _mediaTypeFromName(String fileName) =>
      fileName.toLowerCase().endsWith('.png') ? 'image/png' : 'image/jpeg';
}
