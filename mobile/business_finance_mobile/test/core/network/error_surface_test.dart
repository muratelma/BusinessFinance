import 'dart:io';

import 'package:flutter_test/flutter_test.dart';

/// Ekranda görünen hata cümlesinin kaynağı denetimi (Aşama 06.1 Grup 4).
///
/// Sunucunun cevabı ve yakalanan istisnanın kendisi kullanıcıya gösterilmez:
/// cümle ya `ApiErrorMessages` sözlüğünden ya da ekranın kendi yazdığı sabit
/// Türkçe metinden gelir. Yakalanan bir hatayı doğrudan metne çevirmek
/// (`$error`, `error.toString()`) sunucunun iç yapısını ekrana taşır.
///
/// Denetim kaynağı tarar, çünkü ölçülen şey bir davranış değil bir **alışkanlık**:
/// tek bir ekranın tek bir `catch` bloğu kuralı bozabilir ve o ekran için ayrı
/// bir widget testi yazılmadığı sürece hiçbir yerde görünmez.
void main() {
  test('yakalanan hata doğrudan metne çevrilmiyor', () {
    // Bir hatayı kullanıcıya taşımanın bilinen yolları.
    //
    // `error.message` bilerek serbest: `ApiException.message` sunucudan gelmez,
    // `ApiErrorMessages` sözlüğünün kod karşılığında ürettiği Türkçe cümledir.
    // Yasak olan, hatanın kendisini metne çevirmek.
    final forbidden = RegExp(
      r'(\$\{?(error|exception|err)\}?(?![\w.]))'
      r'|((error|exception|err)\.toString\(\))'
      r'|(\$\{(error|exception|err)\.(?!message))',
    );

    final findings = <String>[];

    for (final file in _dartFiles(Directory('lib'))) {
      final lines = file.readAsLinesSync();

      for (var index = 0; index < lines.length; index++) {
        final line = lines[index];
        if (line.trimLeft().startsWith('//')) continue;
        if (!forbidden.hasMatch(line)) continue;

        findings.add('${file.path}:${index + 1}');
      }
    }

    expect(findings, isEmpty, reason: findings.join('\n'));
  });

  test('tarama gerçekten kaynak okuyor', () {
    // Hiçbir dosya okumayan bir tarama da yeşil görünürdü.
    final files = _dartFiles(Directory('lib')).toList();

    expect(files.length, greaterThan(100));
    expect(
      files.map((file) => file.uri.pathSegments.last),
      contains('api_error_messages.dart'),
    );
  });
}

Iterable<File> _dartFiles(Directory root) => root
    .listSync(recursive: true)
    .whereType<File>()
    .where((file) => file.path.endsWith('.dart'));
