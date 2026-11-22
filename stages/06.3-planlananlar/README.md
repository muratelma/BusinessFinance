# Planlananlar, tekrarlayanlar ve abonelikler — giriş kapısı

Aşama 06.3'ün çalışma klasörü. Bu konuda oturum buradan başlar. Tasarım
kararlarının kaynağı Codex'in çalışma belgesidir:
`design/planlananlar/README.md` (17–21 Kasım 2026; panolar
`tasarim-onizleme/planlananlar-*`). O belge tasarım geçmişidir; uygulama
durumu, sıra ve günlük burada tutulur.

İşleyiş kullanıcı tarafından Claude Code'a bırakıldı (22 Kasım 2026).

## Sınırlar

- Borç-alacak satırlarının anlamı değişmez (`stages/06.3-borc-alacak/README.md`
  "Codex çalışması" maddesi): `debt-installment`, `receivable-installment`,
  `payable-obligation`, `receivable-obligation`, `counterparty-*-due` türleri,
  eylemleri ve hedefleri; `includeCounterpartyDues` bayrağı. Bu satırların
  ekrandaki eylemi borç-alacak Faz 5–7'de bağlanır.
- Özet'teki Yaklaşanlar kartı (21–22 Kasım teslimi) bu işin dışında kalır;
  yalnız Planlananlar'a geçişleri korunur.

## Fazlar

| Faz | İş | Durum |
|---|---|---|
| A1 | Sunucu: özel tarih aralığı, dönem özeti (ödenecek / beklenen giriş / karta yansıyacak, tutarı belirsiz sayısı), seçimden önce gecikenler, tür seçimi | Bitti (22 Kasım) |
| A2 | Flutter: Planlananlar listesi (04 pano), filtre paneli ve takvim (18), boş durumlar (06), yenileme uyarısı (18-01), bilinmeyen türün atlanması | Bitti (22 Kasım); emülatör kabulü bekliyor |
| A3 | Flutter: satırdan ilgili ödeme paneline (dönem, ekstre, vergi, taksit planı) | Bitti (22 Kasım); `…görüntüle` bağlantıları B2 ile |
| B1 | Tekrarlayanlar listesi (09/11 pano, arama, ilk kullanım) | Bitti (22 Kasım); `Abonelikler ›` C ile |
| B2 | Plan ayrıntısı tam sayfa (07 pano): açık vadeler, son iki ödeme, Duraklat/Sürdür, Sil | Bitti (22 Kasım); Düzenle B3 ile, ödeme takvimi sonra |
| B3 | Sunucu: D1 dönemden itibaren tutar, D2 planı sona erdir | Başlamadı |
| C | Abonelikler ve ücretsiz deneme (katalog, form, hatırlatma, teyit) | Borç-alacaktan sonra |

Sıra: A → B → borç-alacak Faz 4–7 → C. Gerekçe: borç-alacak ekranları A'nın
Planlananlar yapısına bağlı; C yeni bir veri modeli ister ve ötekileri
beklemez.

## Kabul edilmiş davranış (kısa)

- Seçim başlangıcı ve sonu dahil, vadesi seçimdeki açık kayıtlar ana listededir.
  Gecikme bugüne göre belirlenir. Hem seçim başlangıcından hem bugünden önce
  vadesi geçmiş açık kayıtlar `15 Aralık'tan önce gecikenler` gibi tarihli,
  açılır satırdadır ve dönem toplamına girmez (19 Kasım kararı).
- Dönem toplamları ayrıdır: ödenecek, beklenen giriş, karta yansıyacak; yalnız
  ilgili kayıt varsa görünür. Tutarı bilinmeyen ödeme sayıyla belirtilir.
  Toplamlar birbirinden düşülmez.
- Satırın tamamı ve kısa eylem aynı hedefi açar; tek dokunuş mali kaydı
  onaylamaz.

## Kabul

Senin deneyeceğin yerler aşama aşama `KABUL.md`'de.

## Kararlar ve panolardan ayrılan yerler

Her satır: panoda ya da önceki kararda ne yazıyordu, ne yapıldı, neden.
`Soru` sütunu dolu olanlar senin onayını bekler.

| # | Yer | Panoda / önce | Yapılan | Neden | Soru |
|---|---|---|---|---|---|
| S1 | Planlananlar satırı | Tutar siyah (04) | Aynen: tutar renksiz, paranın yönü alttaki etki satırında | Pano öyle; renk Özet'te kalır | — |
| S2 | Gecikenler satırı | `Geciken ödemeler · 8 Aralık öncesi` | `N geciken kayıt · 8 Aralık öncesi`; tutar yalnız ödemelerin toplamı | Seçimden önce gecikenler tahsilat da içerebiliyor; "ödemeler" yanlış olurdu | Başlık uygun mu? |
| S3 | Tutarı belli olmayan vergi | `Tutar bekliyor` (04) | Satırda `Tutar bekliyor`; dönem kartında `Tutarı eksik: N ödeme` | Uygulamanın genel metni (`Tutar ödemede girilecek`) satırı iki satıra bölüyordu | — |
| S4 | Gün başlığı | Yalnız tarih (04) | Tarih + soluk gün adı (`25 Kasım Çarşamba`) + `Bugün` / `Gecikmiş` | İşlemler'in gün başlığıyla tutarlı; vadeyi haftaya yerleştirmek kolay | — |
| S5 | Kısa eylem | Mavi (04) | Mavi (uygulamanın nötr mavisi) ve `›` | İlk denemede siyahtı, dokunulabilir olduğu anlaşılmıyordu | — |
| S6 | Kaynak yazımı | `İşletme hesabından ödenecek` | `Garanti Vadesiz · Hesaptan ödenecek` | Hesap adına Türkçe ek eklemek (`Kasasından`, `kartına`) birçok adda yanlış çıkıyor | — |
| S7 | Eylemsiz satır | — | Kişinin vadesi ve eski yükümlülükte alt satır ve eylem yok | Borç-alacak ekranları bağlanana kadar ödeme yolu yok; çalışmayan eylem gösterilmez (19 Kasım kararı) | — |
| S8 | Kapsam seçimi | `Hepsi ⌄` dönem kartında (04) | Aynen; açılır menü uygulamanın genel kapsamını değiştirir | 20 Kasım kararı | — |
| S9 | Tür menüsü | 7 tür (18-02) | Tüm türler, Tekrarlayan, Kart ekstresi, Kart taksidi, Borç ödemesi, Alacak tahsilatı, Vergi; tür sunucuda süzülür | Liste ile toplam aynı seçimden gelsin; istemci para toplamaz | — |
| S10 | Ekstre ödemesi | Ödeme paneli Planlananlar'ın üstünde (05) | Kartın sayfası açılır, ödeme seçimi üstünde gelir; kapanınca Planlananlar'a dönülür | Kart ödeme formu kart sayfasının içinde; ikinci bir form yazmak yerine aynısı kullanıldı (19 Kasım kararı: ödeme ilgili özelliğin formuyla) | Kart sayfasının arkada kısa görünmesi sorun mu? |
| S11 | Vergi ödemesi | `Ödedim` paneli | Planlananlar'ın üstünde açılır | 19 Kasım kararı | — |
| S12 | Planlananlar'a giriş | — | Diğer → Planlama'ya `Planlananlar` ve `Tekrarlayanlar` eklendi | Önceden yalnız Özet'ten girilebiliyordu | — |
| S13 | Özet · Hesap bakiyeleri | Bütün hesaplar | En çok 5 hesap (bakiyesi en büyük), başlıkta sayı | Senin önerin (22 Kasım) | — |
| S14 | Özet · eksi bakiye | — | Renklenmiyor | 2 Ekim kararı: nakit hesabın eksiye düşmesi uyarı değildir | — |
| S15 | Özet · Kategori giderleri | `En büyük pay` | Aynı yerde önceki aya göre fark; önceki ayda gider yoksa `En büyük pay` | Senin kararın (22 Kasım): kart uzamasın | — |
| S17 | Tekrarlayanlar · `Abonelikler ›` | Aramanın sağında (09) | Yok | Abonelik modeli Faz C'de; çalışmayan bağlantı gösterilmez | — |
| S18 | Tekrarlayanlar · simge rengi | Gider çoğunlukla gri (09) | Gider gri, gelir yeşil, duran plan soluk | İlk denemede bütün giderler kırmızıydı, gecikme öne çıkmıyordu | — |
| S19 | Tekrarlayanlar · yer | Kendi sayfası (09) | `/more/recurring` kendi sayfası; `Planlama ve raporlar`daki sekme aynı listeyi çizer | Raporlar'ın ayrılması (19 Kasım kararı) ayrı iş; iki görünüm ayrışmasın | Planlama ve raporlar'dan Tekrarlayanlar ve Yaklaşanlar sekmeleri kaldırılıp sayfa `Raporlar` olsun mu? |
| S20 | Tekrarlayanlar · satıra dokunma | Plan ayrıntı sayfası (07) | Şimdilik duraklat / sürdür / sil paneli | Ayrıntı sayfası B2 | — |
| S21 | Plan ayrıntısı · Düzenle | Sağ üstte `Düzenle` ve `Hangi vadeden itibaren?` (07) | Yok | Bugünkü sunucu bekleyen kalemleri tutar eşitliğiyle değiştiriyor (S3–S4); dönemden itibaren değişiklik (D1) gelmeden düzenleme açılırsa eski bekleyenler sessizce değişebilir | — |
| S22 | Plan ayrıntısı · geçmiş | Ödeme gününe göre, farklıysa vade ayrıca (19 Kasım) | Vade gününe göre | Kalem listesi ödeme gününü taşımıyor; sunucuya eklenmesi B3'te | — |
| S23 | Plan ayrıntısı · ödeme takvimi | `Ödeme takvimi ›` (17) | Yok | Ayrı iş; şimdilik gecikenler ve ilk gelecek vade | — |
| S24 | Plan ayrıntısı · sil | — | Yalnız hiç ödemesi olmayan planda `Planı sil` | Sunucu ödemesi olan planı zaten reddediyor; çalışmayan eylem gösterilmez | — |
| S25 | Özet kartı | Siyah kart (07) | Aynen; tutar düz metin | Para bileşeni koyu zeminde görünmüyordu | — |
| S16 | Özet · Yaklaşanlar alt satırı | `7 günde ödenecek · N ödeme` | Satır hep var, `· N ödeme` yalnız görünmeyen ödeme varsa | Senin kararın (22 Kasım) | — |

## Günlük

### 22 Kasım 2026 — klasör açıldı, A1 başladı

- Özet düzeltmeleri `0040302` ile commit edildi (`ozet-tamamlama`).
- A1 sunucu tasarımı: planlanan uca isteğe bağlı `from` / `to`. Verilirse
  `daysAhead` yerine aralık kullanılır; cevap `period` özetini taşır. Eski
  istemci aynı cevabı alır.

### 22 Kasım 2026 (devam) — A1 ve A2

- **Sunucu:** planlanan uca `from`, `to`, `type`; cevapta `period` (üç ayrı
  toplam, tutarı belirsiz ödeme sayısı, seçimden önce gecikenler). Eski
  istemcinin okuması değişmedi. Sözleşme `financial-activity-api-contract.md`.
- **Ekran:** 04 panosundaki dönem kartı, gecikenler satırı, gün kartları ve
  satır dili; filtre paneli (tür menüsü, dönem, takvim); boş durumlar; yenileme
  uyarısı; bilinmeyen türün atlanması. Kart Hareketleri'nin takvim ızgarası ve
  dönem satırları ortak bileşene alındı (`AppRangeCalendarGrid`,
  `AppDateRangeDialog`, `AppFilterRadioRow`, `AppFilterRangeRow`).
- **Sapma:** satırdaki tutar renksiz (panodaki gibi); paranın yönü alttaki etki
  satırında yazılı. Gecikenler satırının başlığı `N geciken kayıt` (panoda
  `Geciken ödemeler`): seçimden önce gecikenler tahsilat da içerebiliyor.
- **Kalan (A3):** satırdan ödeme panellerinin kendisi (ekstre için kart ödeme
  formu, vergi için Ödedim paneli, taksit planı) ve `…görüntüle` bağlantıları;
  bugün ekstre ve taksit Kartlar'a, vergi Vergi takibi'ne gidiyor.
- **Not:** emülatördeki API (5284) bu değişiklikten önce açılmış; yeni aralık
  ve toplamlar API yeniden başlatılınca görünür.

### 22 Kasım 2026 (devam) — kendi değerlendirmem ve A3

- **Gözden geçirme (cihazda, gerçek API):** toplamlar Özet'le tutuyor. Düzeltilen
  beş yer: eylem rengi (S5), bilgi satırında tekrarlanan tür (S7), uzun
  belirsiz tutar metni (S3), eski yükümlülükte kişi simgesi (fatura simgesine
  döndü), gün adı (S4).
- **A3:** vergi satırı `Ödedim` panelini aynı ekranda açar (S11); ekstre satırı
  kartın ödeme seçimini, günü gelmemiş taksit taksit planını açar, kapanınca
  Planlananlar'a dönülür (S10). Kart sayfası yüklenirken artık
  `Kart artık bulunamıyor` yerine yükleniyor gösterir.
- **Kabul listesi** `KABUL.md` (Aşama 1–3) yazıldı.
- **Sıradaki:** B1 Tekrarlayanlar listesi.

### 22 Kasım 2026 (devam) — B1

- Tekrarlayanlar kendi sayfasında (`/more/recurring`, `RecurringPlansPage`);
  liste (`RecurringPlanList`) Planlama'nın sekmesiyle ortak. Satır, sıra,
  arama, sonuçsuz aramadan adla ekleme, ilk kullanım, duraklatılan ve
  bitenler ayrı sayfada. Diğer → Planlama'ya Planlananlar ve Tekrarlayanlar.
- Kendi değerlendirmem: gider simgeleri griye alındı (S18). Duraklatma
  metni bekleyen kalemlerin de listelerden kalktığını söylüyor (B9).
- Kabul: `KABUL.md` Aşama 4. Sıradaki: B2 plan ayrıntı sayfası.

### 22 Kasım 2026 (devam) — B2

- Plan ayrıntısı (`RecurringPlanDetailPage`, `/more/recurring/:planId`): özet,
  açık vadeler ve ödemesi, geçmiş (son iki + `Tümü`), duraklat / sürdür, sil.
  Planlananlar'ın gün panelinde `Planı görüntüle ›`. Eski duraklat/sil paneli
  kalktı; yönetim ayrıntı sayfasında.
- Kendi değerlendirmem (cihazda): özet kartında tutar görünmüyordu (S25),
  bölüm başlıkları kenara yapışıktı; ikisi düzeltildi.
- Kabul: `KABUL.md` Aşama 5. Sıradaki: B3 (sunucu: D1 dönemden itibaren
  tutar/kaynak, D2 planı sona erdir; ardından Düzenle formu).

