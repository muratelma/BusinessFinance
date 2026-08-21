import 'dart:io';

import 'package:flutter_test/flutter_test.dart';

/// Tasarım sistemi kurallarını kaynak kodda uygulayan yapısal kapı.
///
/// Bu kurallar tek tek widget testleriyle korunamaz: kural "hiçbir ekran
/// kendi rengini uydurmasın"dır, yani ihlal her zaman **yeni** ve henüz testi
/// olmayan bir dosyada ortaya çıkar. Backend'de mimari testlerin katman
/// bağımlılıklarını koruduğu gibi, burada da kaynak taranır.
void main() {
  final libDir = Directory('lib');

  List<({String path, int line, String text})> sourceLines() {
    final result = <({String path, int line, String text})>[];
    for (final entity in libDir.listSync(recursive: true)) {
      if (entity is! File || !entity.path.endsWith('.dart')) continue;
      final lines = entity.readAsLinesSync();
      for (var i = 0; i < lines.length; i++) {
        var text = lines[i];
        // Yorumlar kuralın dışındadır: bu kararların *neden* böyle olduğunu
        // anlatan yorumlar çoğu zaman yasaklanan ifadeyi örnek olarak yazar.
        final comment = text.indexOf('//');
        if (comment >= 0) text = text.substring(0, comment);
        if (text.trim().isEmpty) continue;
        result.add((
          path: entity.path.replaceAll(r'\', '/'),
          line: i + 1,
          text: text,
        ));
      }
    }
    return result;
  }

  test('ekranlar ham Material renk sabiti kullanmaz', () {
    // `Colors.green` karanlık temaya tepki vermez ve kontrast kapısından
    // geçmez. Anlamlı renk `AppFinanceColors`'tan, yüzey rengi
    // `ColorScheme`'den gelir.
    final pattern = RegExp(r'\bColors\.(?!transparent\b)[a-z]');
    final offenders = [
      for (final line in sourceLines())
        if (pattern.hasMatch(line.text)) '${line.path}:${line.line}',
    ];

    expect(
      offenders,
      isEmpty,
      reason:
          'Ham Material renk sabiti bulundu. Anlamlı renkler '
          'AppFinanceColors, yüzey renkleri Theme.of(context).colorScheme '
          'üzerinden alınır:\n${offenders.join('\n')}',
    );
  });

  test('boşluklar ölçekten gelir', () {
    // Sayı sayı uydurulan boşluklar ekranlar arasında görünür tutarsızlık
    // üretir. `EdgeInsets` ve dikey ayırıcılar `AppSpacing` adlarını kullanır.
    final edgeInsets = RegExp(
      r'EdgeInsets\.(all|symmetric|only|fromLTRB)\([^)]*\b\d',
    );
    final sizedBox = RegExp(r'SizedBox\(\s*(height|width):\s*\d');

    final offenders = <String>[];
    for (final line in sourceLines()) {
      // Yerleşim yüksekliği (grafik alanı, boş durum kutusu) bir boşluk
      // değildir; ölçeğe zorlanmaz.
      final isLayoutHeight = RegExp(
        r'SizedBox\(\s*height:\s*\d+,\s*child:',
      ).hasMatch(line.text);
      if (isLayoutHeight) continue;

      if (edgeInsets.hasMatch(line.text) || sizedBox.hasMatch(line.text)) {
        offenders.add('${line.path}:${line.line}  ${line.text.trim()}');
      }
    }

    expect(
      offenders,
      isEmpty,
      reason:
          'Ölçek dışı sabit boşluk bulundu; AppSpacing adlarından biri '
          'kullanılmalı:\n${offenders.join('\n')}',
    );
  });

  test('kırılım noktası sayıları tek yerde tanımlıdır', () {
    // Aynı kavramın iki eşiği (shell 720, planlama 700) ekranların
    // birbirinden farklı davranmasına yol açmıştı.
    final pattern = RegExp(r'maxWidth\s*[><]=?\s*\d');
    final offenders = [
      for (final line in sourceLines())
        if (pattern.hasMatch(line.text) &&
            !line.path.endsWith('core/theme/app_breakpoints.dart'))
          '${line.path}:${line.line}',
    ];

    expect(
      offenders,
      isEmpty,
      reason:
          'Ekran genişliği doğrudan karşılaştırılıyor. Pencere sınıfı '
          'AppBreakpoints/context.windowSize üzerinden okunur:\n'
          '${offenders.join('\n')}',
    );
  });
}
