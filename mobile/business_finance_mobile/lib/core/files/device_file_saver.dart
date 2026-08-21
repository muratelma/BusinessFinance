import 'package:flutter/services.dart';

abstract interface class DeviceFileSaver {
  Future<bool> save({
    required Uint8List bytes,
    required String fileName,
    required String mimeType,
  });
}

class AndroidDocumentFileSaver implements DeviceFileSaver {
  const AndroidDocumentFileSaver();

  static const _channel = MethodChannel(
    'com.nef.business_finance_mobile/document_file',
  );

  @override
  Future<bool> save({
    required Uint8List bytes,
    required String fileName,
    required String mimeType,
  }) async {
    final saved = await _channel.invokeMethod<bool>('save', {
      'bytes': bytes,
      'fileName': fileName,
      'mimeType': mimeType.split(';').first.trim(),
    });
    return saved ?? false;
  }
}
