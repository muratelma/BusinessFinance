import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';

/// Uygulamanın dil yapılandırması.
///
/// Uygulamanın kendi metinleri kaynak kodda Türkçe yazılıdır, fakat
/// Material'in **kendi** ürettiği metinler (tarih seçicinin `OK`/`Cancel`
/// butonları, ay ve gün adları, metin alanının kes/kopyala/yapıştır menüsü,
/// `RefreshIndicator`'ın ekran okuyucu duyurusu) delege verilmediği sürece
/// İngilizce kalır. Baştan sona Türkçe bir ekranda tarih seçiciyi açınca
/// İngilizce bir takvim çıkıyordu.
///
/// Değerler tek yerde tutulur ki test kurulumları uygulamanın gerçeğinden
/// sapmasın: aynı sapma daha önce temada yaşandı ve testler uygulamanın
/// kullanmadığı bir görünümü doğrular hâle gelmişti.
abstract final class AppLocale {
  static const Locale turkish = Locale('tr', 'TR');

  static const List<Locale> supported = [turkish];

  static const List<LocalizationsDelegate<Object>> delegates = [
    GlobalMaterialLocalizations.delegate,
    GlobalWidgetsLocalizations.delegate,
    GlobalCupertinoLocalizations.delegate,
  ];
}
