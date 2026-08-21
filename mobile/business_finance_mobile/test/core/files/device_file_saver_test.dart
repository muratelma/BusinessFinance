import 'package:flutter/services.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:business_finance_mobile/core/files/device_file_saver.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  const channel = MethodChannel(
    'com.nef.business_finance_mobile/document_file',
  );

  tearDown(() {
    TestDefaultBinaryMessengerBinding.instance.defaultBinaryMessenger
        .setMockMethodCallHandler(channel, null);
  });

  test('passes file bytes and a normalized MIME type to Android', () async {
    MethodCall? received;
    TestDefaultBinaryMessengerBinding.instance.defaultBinaryMessenger
        .setMockMethodCallHandler(channel, (call) async {
          received = call;
          return true;
        });

    final saved = await const AndroidDocumentFileSaver().save(
      bytes: Uint8List.fromList([1, 2, 3]),
      fileName: 'transactions.csv',
      mimeType: 'text/csv; charset=utf-8',
    );

    expect(saved, isTrue);
    expect(received?.method, 'save');
    expect(received?.arguments['fileName'], 'transactions.csv');
    expect(received?.arguments['mimeType'], 'text/csv');
    expect(received?.arguments['bytes'], Uint8List.fromList([1, 2, 3]));
  });
}
