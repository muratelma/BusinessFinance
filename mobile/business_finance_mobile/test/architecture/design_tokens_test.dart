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

  test('panelin başlığı gövde payının içine konmaz', () {
    // Panelin başlığı tutamacın hemen altından başlar
    // (`AppSheetInsets.header` / `top`). Başlık `AppSheetInsets.body` ile
    // boşluklanan gövdenin içine konunca 8 dp aşağıda durur ve paneller
    // arasında başlık yüksekliği ayrışır (Ekstre paneli, 15 Kasım 2026).
    final offenders = <String>[];
    for (final entity in libDir.listSync(recursive: true)) {
      if (entity is! File || !entity.path.endsWith('.dart')) continue;
      final path = entity.path.replaceAll(r'\', '/');
      if (path.contains('/core/theme/')) continue;
      final lines = entity.readAsLinesSync();
      for (var i = 0; i < lines.length; i++) {
        if (!lines[i].contains('padding: AppSheetInsets.body,')) continue;
        final end = i + 25 < lines.length ? i + 25 : lines.length;
        for (var j = i + 1; j < end; j++) {
          if (lines[j].contains('textTheme.titleLarge')) {
            offenders.add('$path:${i + 1}');
            break;
          }
        }
      }
    }
    expect(
      offenders,
      isEmpty,
      reason:
          'Panel başlığı gövde payının (AppSheetInsets.body) içinde. Başlık '
          'AppSheetInsets.header ya da top ile boşluklanır:\n'
          '${offenders.join('\n')}',
    );
  });

  test('sayfa gövdesinin boşluğu AppPageInsets adlarından gelir', () {
    // Her sayfa kaydırılan gövdeye kendi boşluğunu yazınca başlığın altı
    // sayfadan sayfaya 0, 4, 8, 12 ve 16 dp olmuştu (10 Kasım 2026). Gövde
    // içeriğin türüne uyan adı seçer; değer tek yerde değişir.
    final scroll = RegExp(
      r'(ListView(\.separated|\.builder)?|SingleChildScrollView|'
      r'CustomScrollView)\(',
    );
    final padding = RegExp(r'padding:\s*(const\s+)?(\w+)');
    // Sayfa gövdesi olmayan tek kaydırıcı: vergi panelinin gövdesi sayfa
    // dosyasında durur ve alt boşluğu duruma göre değişir.
    const exceptions = {'tax_parts.dart'};
    final offenders = <String>[];
    for (final entity in libDir.listSync(recursive: true)) {
      if (entity is! File) continue;
      final path = entity.path.replaceAll(r'\', '/');
      final name = path.split('/').last;
      final isPage =
          path.contains('/presentation/') &&
          (name.contains('page') || name == 'tax_parts.dart');
      if (!isPage || path.contains('/auth/') || path.contains('/onboarding/')) {
        continue;
      }
      final source = entity.readAsStringSync();
      for (final match in scroll.allMatches(source)) {
        final end = match.end + 900 > source.length
            ? source.length
            : match.end + 900;
        final segment = source.substring(match.end, end);
        if (padding.firstMatch(segment)?.group(2) != 'EdgeInsets') continue;
        final next = scroll.firstMatch(segment);
        final found = padding.firstMatch(segment)!;
        // Boşluk bir sonraki kaydırıcıya aitse bu kaydırıcının değildir.
        if (next != null && next.start < found.start) continue;
        if (exceptions.contains(name)) continue;
        final line = '\n'.allMatches(source.substring(0, match.start)).length;
        offenders.add('$path:${line + 1}');
      }
    }

    expect(
      offenders,
      isEmpty,
      reason:
          'Kaydırılan sayfa gövdesi kendi EdgeInsets değerini yazıyor. '
          'AppPageInsets.list / form / underTabs adlarından biri '
          'kullanılır:\n${offenders.join('\n')}',
    );
  });

  test('yazı boyu AppTextSizes adlarından gelir', () {
    // Ekranlar boyu elle yazınca 11'den 32'ye on iki ayrı sayı birikmişti
    // (10 Kasım 2026). Boy tema kademesinden ya da AppTextSizes adından
    // gelir; giriş akışı kendi oranlı ölçeğini taşır (NomivaScale).
    final literal = RegExp(r'fontSize:[^,\n]*\b\d+(\.\d+)?\b');
    // Avatarın harfi dairenin çapına oranlıdır.
    const exceptions = {'app_avatar.dart'};
    final offenders = <String>[];
    for (final entity in libDir.listSync(recursive: true)) {
      if (entity is! File || !entity.path.endsWith('.dart')) continue;
      final path = entity.path.replaceAll(r'\', '/');
      if (path.endsWith('core/theme/app_typography.dart') ||
          path.contains('/auth/') ||
          exceptions.contains(path.split('/').last)) {
        continue;
      }
      final lines = entity.readAsLinesSync();
      for (var i = 0; i < lines.length; i++) {
        if (lines[i].trimLeft().startsWith('//')) continue;
        if (literal.hasMatch(lines[i])) offenders.add('$path:${i + 1}');
      }
    }

    expect(
      offenders,
      isEmpty,
      reason:
          'Yazı boyu sayıyla yazılmış. Tema kademesi ya da AppTextSizes adı '
          'kullanılır:\n${offenders.join('\n')}',
    );
  });

  test('köşe yarıçapı AppRadius adlarından gelir', () {
    // 16, 20 ve 999 on sekiz yerde sayıyla yazılmıştı (10 Kasım 2026).
    // Tek haneli yarıçap grafik işaretidir (çubuk ucu, ilerleme şeridi) ve
    // ada bağlanmaz.
    final literal = RegExp(r'Radius\.circular\(\s*\d{2,}');
    final offenders = <String>[];
    for (final line in sourceLines()) {
      if (literal.hasMatch(line.text)) {
        offenders.add('${line.path}:${line.line}  ${line.text.trim()}');
      }
    }

    expect(
      offenders,
      isEmpty,
      reason:
          'Köşe yarıçapı sayıyla yazılmış; AppRadius adlarından biri '
          'kullanılır:\n${offenders.join('\n')}',
    );
  });
}
