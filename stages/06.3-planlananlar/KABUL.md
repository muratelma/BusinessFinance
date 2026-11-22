# Kullanıcı kabul listesi — Özet ve Planlananlar

Her madde emülatörde tek başına denenebilir. Sıra, ekranların bağımlılığına
göredir. Sonuç sütununu sen doldurursun: `Tamam`, `Sorun: …` ya da `Atla`.
Yeni faz bitince altına yeni bölüm eklenir; eskiler silinmez.

Ön koşul: API 5284'te açık (22 Kasım'dan sonraki sürüm), emülatörde son APK.
Planlananlar'a giriş: Özet → Yaklaşanlar kartına ya da başlıktaki tarih
aralığına dokun. (Diğer menüsünde Planlananlar girişi yok; bkz. `README.md`
"Açık sorular".)

## Aşama 1 — Özet (22 Kasım düzeltmeleri, `0040302`)

| # | Nerede | Ne yap | Ne görmelisin | Sonuç |
|---|---|---|---|---|
| 1.1 | Özet · Bütçeler | Hepsi kapsamında bak | Halka büyük (60 dp), yüzde halkanın içinde rahat; başlık ile halka arasında boşluk; `Kalan` gri, tutar kalın; çizgi soluk gri | |
| 1.2 | Özet · Bütçeler | Aşılmış bütçe (İletişim) | Kırmızı zemin, `Aşım`, halka kırmızı | |
| 1.3 | Özet · Bütçeler | Uzun kategori adı (iki satıra inen) | Simge ilk satırla hizalı | |
| 1.4 | Özet · Yaklaşanlar | Kart içeriği | Tarih ve çizgi sola yakın; aynı günün iki kaydında da tarih yazılı; tutar sağda ortalı | |
| 1.5 | Özet · Yaklaşanlar | Alt satırlar | Yalnız `Bugün · …` ve `Yarın · …` gün yazar; diğerlerinde yalnız tür | |
| 1.6 | Özet · Yaklaşanlar | Altta `7 günde ödenecek` | Satır hep var; `· N ödeme` yalnız listede görünmeyen ödeme varsa | |
| 1.7 | Özet · Kategori giderleri | Toplamın altı | `Geçen ayın aynı gününe göre` ve `↑ ₺… fazla` (kırmızı) ya da `↓ … az` (yeşil); kart eskisi kadar uzun | |
| 1.8 | Özet · Kategori giderleri | Önceki aya geç (ay oku) | Geçmiş ayda `Önceki aya göre`; önceki ayda gider yoksa `En büyük pay` | |
| 1.9 | Özet · Varlık durumu | Satırlar | POS tutarı mavi; varlık satırları arasındaki çizgiler aynı kalınlıkta; borç boydan boya çizgiyle ayrı; üstte ve altta biraz boşluk | |
| 1.10 | Özet · Hesap bakiyeleri | Başlık ve liste | Sağda `N hesap ›`; en çok 5 hesap (bakiyesi en büyükler); hesap adları önceki kadar iri değil | |
| 1.11 | Kasa · POS | `Yolda` toplamı | Mavi | |

## Aşama 2 — Planlananlar listesi (`f773928` ve sonrası)

| # | Nerede | Ne yap | Ne görmelisin | Sonuç |
|---|---|---|---|---|
| 2.1 | Özet'ten giriş | Yaklaşanlar'a dokun | Başlıkta bugünden başlayan 7 gün (`22–28 Kasım`); `Ödenecek` Özet'teki `7 günde ödenecek` ile aynı | |
| 2.2 | Özet'ten giriş | Üstteki kırmızı gecikme şeridine dokun | Aynı ekran, `N geciken kayıt` açık gelir | |
| 2.3 | Dönem kartı | `Hepsi ⌄` → İşletme | Liste ve toplamlar değişir; Özet'e dönünce Özet de İşletme'de | |
| 2.4 | Dönem kartı | Toplamlar | Yalnız kaydı olan satırlar: `Ödenecek`, `Beklenen giriş`, `Karta yansıyacak`; tutarı belli olmayan vergi varsa `Tutarı eksik: N ödeme` | |
| 2.5 | Gecikenler | Satıra dokun | Açılır/kapanır; her kayıt kendi gününü yazar; tutar dönem toplamına girmez | |
| 2.6 | Gün başlıkları | Bak | `25 Kasım Çarşamba`; bugünün günü `Bugün`, dönem içindeki geçmiş gün `Gecikmiş` (kırmızı) | |
| 2.7 | Satır | Bak | Ad ve tutar; altında tür (`Tekrarlayan ödeme`, `Vergi`, `Kart ekstresi`…); en altta kaynak ve etki solda, mavi kısa eylem sağda | |
| 2.8 | Satır | Tutarı belli olmayan vergi | Tutar yerine `Tutar bekliyor` tek satırda | |
| 2.9 | Filtre | `Filtre` → Tür → Vergi → Sonuçları göster | Yalnız vergiler; dönem kartının altında `Vergi ✕` etiketi; toplamlar da yalnız vergiden | |
| 2.10 | Filtre | Etiketteki ✕ | Tür kalkar, dönem aynı kalır | |
| 2.11 | Filtre | Önümüzdeki 30 gün / 90 gün | Başlıktaki tarih ve liste değişir | |
| 2.12 | Filtre | Tarih aralığı seç… → takvimde iki gün → Seç → Sonuçları göster | Başlık seçilen aralığı yazar; aralıktan önce açık kalanlar gecikenler satırında | |
| 2.13 | Filtre | İleri bir ay seç (ör. 1–31 Aralık) | Bugün ile 1 Aralık arasındaki kayıtlar listede **yok** (gecikmiş sayılmaz); gecikenler satırında yalnız bugünden önceki açıklar | |
| 2.14 | Filtre | Sıfırla | 30 gün ve tüm türler; panel açık kalır | |
| 2.15 | Boş durum | Kaydı olmayan bir tür seç | `Bu filtreye uyan planlanan işlem yok` ve `Filtreyi temizle` | |
| 2.16 | Boş durum | Kaydı olmayan bir aralık | `Bu dönemde bekleyen kayıt yok` ve `Dönem seç` | |
| 2.17 | Hata | API kapalıyken aşağı çekip yenile | Liste yerinde kalır, `Liste güncellenemedi.` ve `Tekrar dene` | |

## Aşama 3 — Satırdan ödeme (A3)

| # | Nerede | Ne yap | Ne görmelisin | Sonuç |
|---|---|---|---|---|
| 3.1 | Vergi satırı | `Ödedim ›` ya da satırın herhangi bir yeri | Aynı vadenin `Ödedim` paneli Planlananlar'ın üstünde; kaydedince satır listeden düşer | |
| 3.2 | Gecikmiş vergi | Aç | Tutar boş gelir (gecikme zammını uygulama bilemez) | |
| 3.3 | Ekstre satırı | `Ekstreyi öde ›` | Doğru kartın `Ödeme tutarı` seçimi (kalan dönem borcu / asgari / başka tutar) | |
| 3.4 | Ekstre ödemesi | Kısmi tutar öde | Planlananlar'a dönülür; ekstre satırı kalanla kalır | |
| 3.5 | Ekstre ödemesi | Vazgeç (geri) | Doğrudan Planlananlar'a dönülür | |
| 3.6 | Günü gelmemiş kart taksidi | `Planı görüntüle ›` | Kartın taksit planı açılır; kapatınca Planlananlar | |
| 3.7 | Günü gelmiş kart taksidi | `Karta yansıt ›` | `Karta yansıtılsın mı?` onayı; onayla kart harcaması yazılır | |
| 3.8 | Tekrarlayan ödeme / gelir | `Ödeme kaydet ›` / `Geliri kaydet ›` | Gün soran panel (bugünle açılır); önceki dönem bekliyorsa onay sorar | |
| 3.9 | Kişinin vadesi, eski yükümlülük | Satıra dokun | Hiçbir şey olmaz (bilgi satırı; borç-alacak ekranlarıyla bağlanacak) | |
| 3.10 | Ödeme sonrası | Özet'e dön | Yaklaşanlar ve toplamlar güncel | |
