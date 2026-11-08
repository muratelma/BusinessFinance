Bu klasörü (nomiva_handoff) projenin köküne kopyaladıktan sonra Claude Code'a aşağıdaki metni yapıştır.

---

Uygulamanın adı artık **Nomiva** (slogan: "İşin ve cebin tek yerde."). Giriş akışının yeni tasarımı `nomiva_handoff/` klasöründe:

- `nomiva_handoff/TASARIM_NOTLARI.md` — bütün renkler, ölçüler, ekranlar ve animasyonlar. Tek doğru kaynak bu.
- `nomiva_handoff/prototip/nomiva_giris_prototip.html` — çalışan prototip; görünüş ve animasyon için referans. Gerekirse CSS/JS'ini oku.
- `nomiva_handoff/logo/` — logo SVG'leri, `nomiva_handoff/app_icon/` — uygulama ikonları.

Yapmanı istediklerim, sırayla ve her adımdan sonra çalıştırıp kontrol ederek:

1. Logo SVG'lerini `assets/brand/` altına taşı, `flutter_svg` ile göster. Uygulama adını her yerde (Android `android:label`, iOS `CFBundleDisplayName`, uygulama içi metinler) Nomiva yap.
2. `flutter_launcher_icons` ile ikonları ayarla: iOS ve genel ikon `nomiva_ikon_koyu_1024.png`; Android uyarlanabilir ikon ön planı `android_on_plan_1024.png`, arka plan `#0F111A`. (iOS 18 koyu/açık ikon desteği varsa açık tema için `nomiva_ikon_acik_1024.png`.)
3. Notlardaki renk tablosundan açık ve koyu `ThemeData` oluştur (telefonun temasına uysun: `ThemeMode.system`). Yazı tipi Figtree (`google_fonts`).
4. Açılış, Giriş, Hesap oluştur, Parola sıfırla ve Bir adım kaldı ekranlarını notlara göre yeniden düzenle: siyah kavisli üst bölüm, logo konumu, alan stili (etiket + solda simge), boşluklar, parola tekrarının kaldırılması, sabit 30 pt alt bağlantı boşluğu.
5. Açılış animasyonunu ve ekranlar arası geçişi notlardaki sürelerle uygula.

Mevcut iş mantığına (kimlik doğrulama, yönlendirme, servisler) dokunma; yalnız arayüzü değiştir. Bir ölçü notlarda yoksa prototipe bak, ikisinde de yoksa bana sor.
