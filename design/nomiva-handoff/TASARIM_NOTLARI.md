# Nomiva — Giriş akışı tasarım notları

Kaynak prototip: `prototip/nomiva_giris_prototip.html` (tarayıcıda açılır, düğmeler çalışır; sağ panelden Tema: Açık/Koyu).
Prototipteki telefon ekranı 320 × 700 pt'dir. Aşağıdaki ölçüler bu ekrana göredir; gerçek cihazda yatayda oranla ölçekle, dikey konumları güvenli alanın (status bar) altından say.

Seçilen renk: **İndigo** (maviden mora). Uygulama adı: **Nomiva**. Slogan: **İşin ve cebin tek yerde.**

## Dosyalar

| Dosya | Nerede kullanılır |
|---|---|
| `logo/nomiva_isaret_koyu_zemin.svg` | Uygulama içindeki logo. Açılış ve giriş/kayıt ekranlarının üst bölümü **iki temada da siyah** olduğu için hep bu kullanılır. |
| `logo/nomiva_isaret_acik_zemin.svg` | Açık zemin üzerine logo gerekirse (ör. ayarlar/hakkında). İnce konturludur. |
| `app_icon/nomiva_ikon_koyu_1024.png` | Uygulama ikonu, koyu tema (köşesiz kare; köşeyi işletim sistemi yuvarlar). |
| `app_icon/nomiva_ikon_acik_1024.png` | Uygulama ikonu, açık tema. |
| `app_icon/android_on_plan_1024.png` | Android uyarlanabilir ikon ön planı (şeffaf, güvenli alanda). Arka plan rengi: `#0F111A`. |
| `app_icon/*.svg` | İkonların vektör hâli (köşeleri yuvarlatılmış önizleme). |

SVG'ler gerçek vektördür (gömülü resim yok); doğrusal/radyal degrade, clipPath ve evenodd kullanır — `flutter_svg` ile çizilir.

## Renkler

| Belirteç | Açık tema | Koyu tema |
|---|---|---|
| Üst bölüm / açılış zemini | `#050608` | `#050608` |
| Yüzey (form kağıdı) | `#FFFFFF` | `#161922` |
| Metin | `#16191D` | `#F2F3F7` |
| İkincil metin, etiket | `#5E6670` | `#A3A9BA` |
| Alan zemini | `#F2F4F6` | `#1F2330` |
| Alan simgesi | `#8B939D` | `#7D8596` |
| Çizgi ("ya da" ayırıcı, sosyal düğme kenarı) | `#E3E6EA` | `#2C3140` |
| Sosyal düğme zemini | `#FFFFFF` | `#1F2330` |
| Ana düğme zemini / yazısı | `#16191D` / `#FFFFFF` | `#F2F3F7` / `#0E1015` |
| Not | Vurgu rengi küçük metinde (bağlantı vb.) kullanılmaz; yalnız hale, dalga, anahtar ve geçiş efektinde. | |
| Hata / hata zemini | `#CF2E1F` / `#FBE7E5` | `#FF8A80` / `#3A1E22` |
| Vurgu (indigo) | `#6A6CF6` | `#6A6CF6` |
| Vurgu 2 (dalga degradesi) | `#8B5CF6` | `#8B5CF6` |
| Üst bölümdeki logo adı / slogan | `#F2F3F7` / `#A3A9BA` | aynı |

Durum çubuğu üst bölümde açık renk ikonlu; Özet ekranında (açık tema) koyu ikonlu.

## Yazı

Figtree (Google Fonts). Ad "Nomiva": 30 pt, 700, harf aralığı −0.02em. Başlık ("Giriş yap"): 23 pt, 700. Etiket: 12 pt, 500. Alan metni: 14 pt. Düğme: 15 pt, 600. Bağlantılar: 13 pt, 600, altı çizili (alt çizgi ofseti 3).

## Ekranlar

### Açılış (splash)
- Zemin `#050608`. Ortada logo (84 × 90), altında "Nomiva", altında slogan; dikeyde ~250 pt'de.
- Logonun arkasında vurgu renginde yumuşak hale (300 pt daire, radyal: vurgu %38 → %12 → saydam).
- Altta iki katmanlı dalga (yükseklik 190 pt), Vurgu ve Vurgu 2 degradesi.
- Animasyon: logo 750 ms'de belirir (ölçek 0.85 → 1, 6 pt yukarı); 700 ms sonra ad harf harf yükselir (her harf 380 ms, 70 ms arayla); slogan 400 ms gecikmeyle belirir; hale ve dalga 900 ms'de gelir.
- ~1.5 sn sonra beyaz yüzey kavisli kenarıyla alttan yükselir (820 ms, ease-out); logo bloğu aynı anda küçülerek yukarı oturur; slogan ve hale söner; form parçaları 50 ms arayla belirir.

### Üst bölüm (giriş, kayıt, parola sıfırlama, "bir adım kaldı")
- Logo bloğu ölçek 0.62, üstten 62 pt (kayıtta 0.58 ve 58 pt).
- Yüzeyin üst kenarı: girişte 158 pt, kayıtta 148 pt. Kenar kavisli: yanlarda yüzeyin tepesinde, ortada 52 pt aşağı iner (kübik eğri; 320 pt genişlikte kontrol noktaları x=90 ve x=230). Kavis yüksekliği 56 pt.
- Ekranlar arası geçiş: paneller yatay kayar (360 ms), yüzey ve logo 420 ms'de yeni konumuna gider.

### Form
- Yan boşluk 24 pt; öğeler arası 11 pt (kayıtta 8 pt).
- Alan: yükseklik 46, köşe 12, iç boşluk 14, solda 18 pt dolu simge (kişi / zarf / kilit), aralık 10. Parolada sağda göz simgesi. Alanın üstünde etiket.
- "Parolamı unuttum" sağa yaslı, altında 10 pt boşluk, sonra ana düğme.
- Ana düğme: yükseklik 50, tam yuvarlak (25). Kayıtta parola alanıyla arası +10 pt.
- "ya da" ayırıcı, altında Google ve Apple için 52 pt yuvarlak düğmeler (aralık 16). Apple yalnız iOS'ta.
- Alt bağlantı ("Hesabın yok mu? Hesap oluştur" / "Hesabın var mı? Giriş yap"): sosyal düğmelerden **sabit 30 pt** aşağıda, iki ekranda aynı.

### Hesap oluştur
Kullanıcı adı, e-posta, parola. **Parola tekrarı yok.** Kurallar yalnız hata olunca alanın altında kırmızı çıkar.

### Parola sıfırla
Üstte altı çizili "‹ Girişe dön" (14 pt, 600; ok simgesi çizgisiz). Başlık, e-posta alanı, "Kod gönder" düğmesi (alan–düğme boşluğu kayıttakiyle aynı). Açıklama metni yok.

### Bir adım kaldı (Google/Apple ile ilk giriş ve kayıt sonrası)
Kullanıcı adı (yalnız Google/Apple'dan gelene) + "İşletmem var" anahtarı (açıkken vurgu rengi) + "Devam et".

### Bekleme ve hata
Basılan düğme daralıp dönen göstergeye dönüşür; hata gelirse geri açılır, uyarı formun içinde çıkar. Başarılı girişte vurgu rengi basılan düğmeden dairesel yayılır ve Özet açılır.
