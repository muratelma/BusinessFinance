# Uygulama Gözlem Formu — Money Manager (Realbyte)

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Money Manager / Realbyte Apps |
| Sürüm | **4.12.8 GF** (`versionCode=1197`), 7 Eyl 2026 yeni PC emülatörüne temiz kuruldu |
| Test tarihi | 1 Eylül 2026 (Tur 1, eski PC, İngilizce) · **10 Eylül 2026 (Faz 1 boşluk koşumu, yeni emülatör, Türkçe)** |
| ⚠ Sürüm/dil farkı | Tur 1 formu ve `02`–`13` kareleri **eski PC / İngilizce arayüzden**. Yeni emülatörde uygulama sistem diliyle **Türkçe** açılıyor ve Tur 1 verisi yok (temiz kurulum). Faz 1'de veri Türkçe arayüzde **Ağustos 2026** tarihleriyle yeniden girildi; kareler Türkçe yenileriyle değiştirildi. Tur 1'in "01.09'a tek güne girildi" tarih sapması bu koşumda **düzeltildi**. |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400, Android 17) |
| Dil / para birimi | **Türkçe arayüz** (sistem dili) / TRY (₺) |
| Hesap veya plan türü | Ücretsiz sürüm — ekranda banner reklam ("Ad") |
| Erişim kısıtı | Yok — kayıt/giriş/e-posta istemiyor, tamamen yerel. Açılışta SMS izni ("Gerekli Değil") ve bildirim izni sorar, ikisi de reddedildi |

## Görev gözlemleri

| Görev | Sonuç | Yaklaşık adım | İyi çalışan | Sürtünme/belirsizlik | Kanıt |
|---|---|---:|---|---|---|
| K00 İlk açılış ve kayıt | Tamamlandı | 0 | Kayıt/giriş/onboarding **hiç yok**; ilk açılışta doğrudan ana ekran, anında kullanılabilir | Para birimi/dil sorulmuyor; varsayılan USD | Kullanıcı doğruladı: hiçbir ekran görmedi |
| K01 Ana ekran | Tamamlandı | 0 | Daily/Calendar/Monthly/Total/Note sekmeleri; üstte Income/Expenses/Total; tek turuncu "+" | Ay gezgini + 5 sekme + 3 sütun ilk bakışta yoğun | `02-bos-ana-ekran.png`, `03-dolu-ana-ekran.png` |
| K02 Hesap/cüzdan oluşturma | Tamamlandı (uyarlanarak) | ~6/hesap | 3 hazır hesap (Cash / Accounts / Card grupları); grup + hesap iki katmanlı; Card hesabı zengin (ödeme hesabı, kesim/ödeme günü, "Balance Payable" vs "Outst. Balance") | "Hesap oluştur" butonu yok — hazır hesaplar düzenleniyor; **para birimi hesap başına**; her hesap için 2-3 onay diyaloğu | `09-ozgun-ozellik.png` (Card takvimi) |
| K03 İşletme geliri | Tamamlandı (kapsamsız) | ~7 | Segmented Income/Expense/Transfer; alanlar az ve net; "Save / Continue" (seri giriş) | Gelir kategorileri: Allowance/Salary/Petty cash/Bonus/Other — **işletme geliri kategorisi yok**; müşteri ancak "Note" alanına | `04-islem-formu.png` |
| K04 Şahsi gider | Tamamlandı (ama ayrım yok) | ~7 | Not girilince liste satırında **başlık = not**, kategori sola küçük etiket (BusinessFinance `title = açıklama ?? kategori` ile birebir) | **İşletme/şahsi (kapsam) ayrımı formda hiç yok.** Protokol gereği taklit edilmedi → bu boyut `Desteklenmiyor` | `05-siniflandirma.png` |
| K05 İşletme kart gideri | Tamamlandı | ~7 | Kart hesabı seçilince gider **aynı gün gider sayılıyor** VE kart borcu +₺1.200 oluyor (BusinessFinance CreditCardCharge kuralıyla aynı) | Kategori yine kişisel liste; "Other" seçildi | `05-siniflandirma.png`, `06-islem-listesi.png` |
| K06 Transfer | Tamamlandı | ~7 | Ayrı "Transfer" sekmesi; From/To + swap oku + isteğe bağlı "Fees"; liste satırı **nötr siyah** renkte, "Ana Hesap → Ortak Cuzdan" | — | `06-islem-listesi.png` |
| K07 Liste ve rapor | Tamamlandı | ~3 | **Transfer gelir/gidere 0 etki**; **açılış bakiyeleri (₺22.000) gelire girmiyor** (Income = tam ₺25.000, Salary %100); kart gideri gidere giriyor; Stats aylık pasta grafiği | Rapor kapsam/işletme-şahsi bölmesi yok (zaten kapsam yok) | `07-rapor.png` |
| K08 Düzeltme/iptal | Tamamlandı | ~2 | Satıra dokun → **aynı formda satır içi düzenleme**; alttan Delete / Copy / Bookmark | **Silme = tek onay → kalıcı.** "İptal/void" veya geri alma yok; BusinessFinance'in "sil yerine iptal" kuralının tersi | — |

## Kontrol değeri doğrulaması

**Faz 1 (10 Eyl 2026)** — 5 çekirdek işlemin tamamı, kart ödemesi (5. olay) dâhil,
Ağustos 2026 tarihleriyle. Kaynak: `11-hesaplar-final-kontrol-degerleri.png`,
`07-rapor-agustos.png`.

| Değer | `SENTETIK-TEST-VERISI` beklenen | Money Manager | Durum |
|---|---:|---:|---|
| Ortak Cuzdan bakiyesi | 4.150,00 | ₺4.150,00 | ✓ |
| Ana Hesap bakiyesi | 40.800,00 | ₺40.800,00 | ✓ |
| Kart borcu (kart ödemesinden sonra) | 0,00 | ₺0,00 ("Bu Ay" ve "Gelecek Ay") | ✓ |
| Net varlık (Toplam) | 44.950,00 | ₺44.950,00 | ✓ |
| Ağustos gelir toplamı | 25.000,00 | ₺25.000,00 | ✓ (açılış bakiyesi kirletmedi) |
| Ağustos gider toplamı | 2.050,00 | ₺2.050,00 | ✓ (kart harcaması dâhil, kart ödemesi/transfer hariç) |

Money Manager'ın para modeli BusinessFinance ile şaşırtıcı derecede uyumlu:
**transfer nötr, kart ödemesi nötr, açılış bakiyesi gelir değil, kart harcaması
harcandığı gün gider.** Takvim görünümünde 12 Ağu (transfer) ve 18 Ağu (kart
ödemesi) hücreleri "0,00" gösteriyor (`10-takvim-gorunumu.png`).

## Arayüz taraması (görev dışı)

| Alan | Gezildi | Kısa gözlem |
|---|---|---|
| Tüm ana sekmeler | ✓ | **Daily** (gün listesi), **Calendar** (aylık ızgara, her hücrede gelir/gider/net üç satır), **Monthly** (yıllık liste, ay→hafta açılır), **Total** (bütçe + hesap kırılımı), **Note** (işlemden ayrı memo/checklist, boş). Beşi de aynı 3 sütun başlığı + FAB. Hafta sonu başlıkları renkli (Sun kırmızı, Sat mavi) | `10-arayuz-calendar.png` |
| Rapor drill-down | ✓ | Stats pastası → kategoriye tıklanınca o kategorinin işlem listesi. Total sekmesi ayrıca **"Expenses (Cash, Accounts) ₺850" vs "Expenses (Card) ₺1.200"** diye ayırıyor + "Transfer ₺0" satırı + "Compared Expenses (Last month) %" | `11-arayuz-total-butce.png` |
| Bütçe / planlama | ✓ (yüzeysel) | Total sekmesinde "Budget" bloğu + "Budget Setting" linki; Configuration'da "Budget Setting" ve "Repeat Setting" ayrı. Kategori bazlı aylık bütçe kurgusu | — |
| Ayar derinliği | ✓ | More → Settings ızgarası: Configuration / Accounts / Passcode / CalcBox / PC Manager / Backup / Feedback / Help / Recommend. Configuration: kategori ayarları, ana+alt para birimi, başlangıç ekranı, ay/hafta başlangıcı, devir (carry-over), swipe davranışı, gelir-gider renk seti (Set A/B). Yedek ayrı; dışa aktarma "Export data to Excel" (Total sekmesinde) | — |
| Arama ve filtre | ✓ | Filtre paneli: Income/Expense donut yüzdeleri + INCOME/EXPENSES/**ACCOUNT** alt sekmeleri; hesap bazında Income/Transfer-In ve Expense/Transfer-Out sütunları, checkbox ile süzme. Ayrı büyüteç ile serbest metin arama | `12-arayuz-filtre.png` |
| Boş durum | ✓ | Her boş ekranda aynı gri kedi/robot illüstrasyonu + "No data available." Tutarlı ama yönlendirici değil (ne yapılacağını söylemiyor) | `02-bos-ana-ekran.png` |
| Hata / uç durum | ✓ | Zorunlu alan eksikse **toast** ("Please select specific category." + domuz kumbara ikonu), inline alan hatası yok. Tutar 0.00 sessizce kabul (sıfır tutar engellenmiyor). Formdan "geri" **taslak uyarısı vermeden** çıkıyor | `13-hata-toast.png` |
| Widget / hızlı giriş | Kısmen | Form içi "Continue" = kaydet + boş forma dön (seri giriş). Ana ekran widget'ı denenmedi | — |

## Arayüz incelemesi

| Başlık | Kısa gözlem |
|---|---|
| Bilgi hiyerarşisi | Zaman odaklı (ay → gün). Para özeti hep üç sütun: Income (mavi) / Expenses (turuncu) / Total (siyah) |
| Alt/üst gezinme | Alt: Trans. / Stats / Accounts / More. Üst: ay gezgini + favori + arama + filtre |
| Renklerin anlamı | Gelir mavi, gider turuncu-kırmızı, net/transfer nötr siyah; birincil eylem turuncu FAB. Tutarlı |
| Tipografi ve para | Para sağa hizalı, iki ondalık, ₺ ön ekli. Okunur. Kart borcu negatif (₺ -1.200,00) gösteriliyor |
| Kart/liste/grafik | Accounts: gruplu liste, grup alt-toplamı gri başlık. İşlem listesi: gün başlığı + satırlar. Stats: pasta + yüzdeli liste |
| Form alanları | Tek ekranda 5 alan + açıklama/fiş(kamera). Kategori/hesap alt panelden ızgara seçim. "Continue" ile arka arkaya giriş. Tutar için ayrı hesap-makinesi tuş takımı |
| Loading/boş/hata/başarı | Boş: illüstrasyon + "No data available." (yönlendirici değil). Başarı sessiz (toast yok). Hata = toast, inline değil. Silme tek onaylı |
| Erişilebilirlik | Ücretsiz sürümde kalıcı banner reklam ekranın altını yer yer kaplıyor; tuş takımı büyük ve rahat; Türkçe yok (İngilizce/başka diller var) |
| Ekran tutarlılığı | 5 ana sekme + Accounts + Stats hepsi aynı 3 sütun para başlığını taşıyor — güçlü tutarlılık. Ay gezgini her ekranda aynı yerde. Her ekranda farklı FAB ikonu (işlem +, not +, yok) — bağlama göre |

## Akış özeti

- En kısa ve güçlü akış: İşlem ekleme — "+" → tutar → kategori → hesap → Save (~7 dokunuş); "Continue" ile seri giriş
- En fazla sürtünme yaratan akış: Para birimi kurulumu — ana ayar + her hesap ayrı ayrı, hesap başına 2-3 onay diyaloğu
- Uygulamanın hedef kullanıcı varsayımı: Kişisel/gündelik bütçe takibi — işletme değil
- İşletme ve şahsi para yaklaşımı: **Yok.** Kapsam boyutu, mod veya işletme kategorisi bulunmuyor
- Transfer ve kart ödemesi yaklaşımı: Transfer ayrı segment, rapora 0 etki. Card hesabında ayrı "Pay" butonu (Tur 2'de sınanacak)
- Planlama, borç ve tahsilat yaklaşımı: "Rep/Inst." (tekrar/taksit) form içinde; Card hesabı kesim (Settlement) + ödeme (Payment) takvimi taşıyor, "Balance Payable" (kesilmiş, ödenecek) vs "Outst. Balance" (kesilmemiş) ayrımı (Tur 2)

## Faz 1 boşluk koşumu (10 Eyl 2026) — kart, tekrarlayan, açılış bakiyesi

Tur 1'de sınanmayan alanlar Türkçe arayüzde canlı test edildi.

### Kredi kartı ekstre modeli — "Bu Ay" / "Gelecek Ay"

Kart hesabı düzenleme ekranı (`09-kart-hesap-ekstre-modeli.png`):

| Alan | Değer / davranış |
|---|---|
| **Kaynak** | Kartın varsayılan ödeme hesabı (Ana Hesap) — kart formunda seçili |
| **Hesap Kesim Tarihi** | Ayın günü (1) |
| **Son Ödeme Tarihi** | Ayın günü (1) |
| **Bu Ay** | `01/08 ~ 31/08 (Ödeme: 01/09)` — kesilmiş ekstre dönemi + ödeme vadesi |
| **Gelecek Ay** | `01/09 ~ 30/09 (Ödeme: 01/10)` — henüz kesilmemiş dönem |

Hesaplar listesinde kart satırı **iki sütun** taşıyor: **Bu Ay** ve **Gelecek Ay**
(`12-hesaplar-kart-borcu-bu-ay.png`). 8 Ağu'daki ₺1.200 harcama kesim tarihi 1
olduğu için "Bu Ay" (01/08–31/08) ekstresine düştü; 18 Ağu'daki ödeme onu kapattı
→ ikisi de ₺0.

**BusinessFinance karşılığı:** Bu tam olarak bizim **ekstre projeksiyon modeli** —
"Bu Ay" = kesilmiş/ödenecek (Balance Payable), "Gelecek Ay" = kesilmemiş
(Outstanding). Kullanıcıya iki rakamı ayrı göstermek doğrulayıcı referans.

### Kart ödemesi — "Ödeme" butonu

Kart hesabı ekranında `+` FAB'ın yanında ayrı bir **"Ödeme"** butonu var. Basınca
**ön doldurulmuş bir Havale (transfer) formu** açılıyor
(`13-odeme-butonu-onfoldurulmus-havale.png`): Tutar = kart borcu tutarı, Kaynak =
kartın varsayılan hesabı, Giriş = kart, Not = "Ödeme Bilgisi". Yani **kart ödemesi
MM'de özel bir transfer** — hesap −, kart borcu −, gelir/gider 0. Bizim
`CreditCardPayment` modelimizle birebir.

### Tekrarlayan işlem (B1 — aylık ₺600 abonelik, canlı test 10 Eyl)

İşlem formunda sağ üstte **"Tekrar/Taksit"** → açılır menü: **Tekrarlama** ve
**Taksit** iki ayrı seçenek. Tekrarlama seçenekleri (`14-tekrarlama-secenekleri.png`):
Hiçbiri / Günlük / Haftanın günleri / Haftasonu / Haftalık / İki Haftada Bir /
Dört Haftada Bir / Aylık / Ayın Son Günü / İki Ayda Bir / Üç Aylık / Her dört
ayda bir / Her 6 ayda bir / Yıllık.

**Kurulum:** "Aylık" seçilince form üst köşesinde "Aylık" rozeti belirir
(`17-tekrarlayan-aylik-form.png`). Kaydet'te **tek onay**: "Tarihte tekrar eden
işlemler uygulanır. Tekrarlı olarak kaydetmek istiyor musunuz?" → EVET.

**Üretim davranışı (kritik):**

| Dönem | Nasıl görünüyor | Bakiye/rapora etkisi |
|---|---|---|
| 10 Ağu (geçmiş, ilk kayıt) | Listede normal işlem, "Ana Hesap (Aylık)" etiketi (`18`) | ✅ Sayılıyor — Ağustos gider +₺600, Ana Hesap −₺600 |
| 10 Eyl (= bugün) | Listede normal işlem, "Ana Hesap (Aylık)" (`19`) | ✅ Sayılıyor — Eylül gider +₺600, Ana Hesap −₺600 |
| 10 Eki (gelecek) | Ayrı **"Tekrarlama" önizleme bölümü**, gün listesinde değil (`20`) | ❌ Sayılmıyor — ayın Gelir/Gider/Toplam'ı 0 |

Yani MM: **tarihi ≤ bugün olan tekrarları otomatik gerçek işleme dönüştürür**
(onaysız), gelecek tekrarları önizleme olarak gösterir. BusinessFinance'te
`RecurringTransaction` tanımı **hiçbir** dönemde otomatik gerçekleşmez — geçmiş
vadesi geçmiş olsa bile açık `realize` gerekir. MM'nin "gelecek = önizleme"
kısmı bizim planlanan feed'e benzer; "geçmiş = otomatik" kısmı bizden farklı.

**Seri silme:** Bir occurrence'ı silmek (Sil → tek onay) **yalnız o ayı** siler;
seri devam eder (Ağu 10 silindi, Eyl 10 duruyor). Seriyi tümden yönetmek ayrı
bir yol — Faz 1'de aranmadı.

### Taksit (B2 — tasarım ekipmanı ₺6.000 / 6 taksit, canlı test 10 Eyl)

"Tekrar/Taksit" → **Taksit** → tek alan: "Aylık taksit (Ay sayısı)" → 6 →
Kaydet. Form köşesinde "6 Ay" rozeti (`21-taksit-6ay-form.png`).

**Davranış:**

- İşlem listesinde **"Tasarim ekipmani (1/6)"** — taksit numarası başlıkta;
  15 Ağu'ya **yalnız ₺1.000** (bir taksit) düştü, ₺6.000 tamamı değil (`22`).
- Her taksit ayın kendi ekstresine giriyor: Ağustos taksiti "Bu Ay" ₺1.000,
  Eylül taksiti "Gelecek Ay" ₺1.000; kalan 4 taksit henüz borç toplamında yok
  (`23-taksit-kart-borcu-bu-gelecek-ay.png`). Borçlar = ₺2.000 (₺6.000 değil).
- Ağustos taksiti **gider olarak** sayılıyor (Ağustos gider 2.050 → 3.650 +
  ₺1.000 taksit + ₺600 abonelik).

**BusinessFinance karşılığı:** `InstallmentPlan` "yalnız niyettir; yalnız
`realize()` edilen item gerçek `CreditCardCharge` ve gider üretir" — MM de
taksiti ay ay gerçekleştiriyor (tam tutarı bir kerede borç yazmıyor), sadece
gerçekleşme onaysız/otomatik. Etki (taksit başına kendi ayının ekstresi + gider)
bizimkiyle aynı.

### Kısmi kart ödemesi (A — canlı test 10 Eyl)

"Ödeme" butonunun ön doldurduğu tutar (**Bu Ay** borcu = ₺1.000) **düzenlenebilir**:
₺400'e indirildi (`24-kismi-kart-odemesi-400.png`) → kaydedildi → "Bu Ay" borcu
₺1.000'den **₺600'e** düştü, "Gelecek Ay" ₺1.000 değişmedi (`25`). Yani kısmi
ödeme **yalnız kesilmiş ekstreye** uygulanıyor. Bizim kısmi tahsilat/ödeme
modelimizle (D3) uyumlu.

### Fiş / kamera

İşlem formunda "Detay" satırında kamera ikonu var — fiş **fotoğrafı ekleme**
(depolama), OCR değil. MM'de otomatik fiş okuma yok (Tur 1 resmî kaynakta da
geçmiyordu). ADR 0011 öneri katmanının karşılığı değil, sade ek dosya.

### Açılış bakiyesi — "Bakiye Farkı"

Hesap düzenleme ekranında **Tutar** alanı var (açılış bakiyesi). Kaydedince:
"Fark hesap detaylarına kaydedildi, 'İşlemler' bölümünde gösterilmesini ister
misiniz?" → **HAYIR** seçilince (`15-acilis-bakiye-farki-dialog.png`) açılış
bakiyesi **ana İşlemler feed'ine girmiyor** ama hesabın kendi defterinde bugünün
tarihiyle bir **"Bakiye Farkı" / "Bakiyeyi Düzelt"** hareketi olarak kalıyor
(`16-acilis-bakiye-farki-defter.png`). Gelir raporunu kirletmiyor (K07 doğruladı).
**Quirk:** hareket "bugün" tarihli — geçmiş bir döneme ait açılış istenen tarihe
konulamıyor; Ağustos raporu için sorun olmadı çünkü gelir/gider değil.

### Doğrulama (Faz 1)

- Zorunlu alan: hesap seçilmezse **toast** ("Lütfen hesabı seçiniz.",
  `08-hata-toast-hesap-sec.png`) — inline hata yok
- **Sıfır tutar sessizce kabul ediliyor** (kategori + hesap doluysa kaydediliyor);
  sonradan Sil ile temizlendi
- Silme: satır → düzenleme formu → alttan **Sil / Kopya / Hızlı erişim**; Sil →
  tek onay ("Silmek istediğinize emin misiniz?" HAYIR/EVET) → **kalıcı**, geri
  alma yok
- Formdan "geri" tuşuyla çıkınca **taslak uyarısı yok**, girilen veri kaybolur

## BusinessFinance için kararlar

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| İşlem listesinde başlık = not, kategori küçük etiket | Doğrudan al (zaten böyle) | BusinessFinance `title = açıklama ?? kategori` ile aynı; doğrulayıcı örnek | Aktivite feed satırı |
| ~7 dokunuşluk tek ekran işlem formu + "Continue" ile seri giriş | Uyarlayarak al | Hızlı; "Continue" (kaydet + yeni forma dön) mantığı bize de uyar; bizde ek olarak kapsam/KDV/indirilebilirlik alanları var | İşlem ekle formu |
| Gelir/gider/transfer segmented kontrolü form başında | Uyarlayarak al | Kayıt türünü en başta seçmek net; bizde transfer + kart ödemesi + cari tahsilat da var | İşlem ekle launcher |
| Hesap türü grupları (Cash/Bank/Card) + Card'da kesim/ödeme takvimi + "Balance Payable vs Outstanding" | Uyarlayarak al | Bizim kredi kartı + ekstre projeksiyon modelimizle birebir örtüşüyor; kullanıcıya iki rakamı ayrı göstermek iyi fikir | Kart ekranı, ekstre |
| Total sekmesinde ayın gideri "nakit/hesap" ve "kart" diye ayrı satır + "Transfer ₺0" satırı | Uyarlayarak al | Kullanıcıya "kart harcaman ayrı" demek bizim kart borcu modelimizi görünür kılar; transferi 0 olarak açıkça yazmak da öğretici | Özet / aylık rapor ekranı |
| Filtrede ACCOUNT alt sekmesi: hesap bazında Income / Transfer-In / Expense / Transfer-Out sütunları | Uyarlayarak al | Bizim birleşik feed'in hesap kırılımı için iyi model; transfer'i giriş/çıkış olarak ayrı göstermek net | Aktivite feed filtresi |
| Zorunlu alan hatası = toast, inline değil; sıfır tutar sessiz kabul; taslak uyarısı yok | Alma | Bizde ProblemDetails + alan bazlı hata var; toast tek başına erişilebilir değil ve hangi alan olduğunu göstermiyor | Form doğrulama |
| Boş durum ekranları yönlendirici değil (sadece "No data available.") | Alma / dikkat | Bizim proje kuralı: her ekranda görünür empty/loading/error/unauthorized/stale; boş durum ne yapılacağını söylemeli | Tüm liste ekranları |
| Para birimi hesap başına + çok adımlı değiştirme | Alma | Tek para birimi (TRY) varsayımımız var | — |
| Açılış bakiyesi = "Modified Balance / Difference" hareketi ("gelir olarak kaydet?" sorusu) | Uyarlayarak al / dikkat | Bizde `OpeningBalance` kalıcı kolon; ama MM'in çözümü de gelir raporunu kirletmiyor (K07 doğruladı). "Gelir mi?" sorusu gereksiz karmaşıklık — bizim sessiz yaklaşımımız daha iyi | Hesap oluşturma |
| Düzenleme = satır içi; silme = kalıcı, tek onay | Alma | BusinessFinance finansal geçmişi korur: sil yerine iptal, düzeltme = iptal + yeni. MM'in yaklaşımı ticari üründe veri bütünlüğünü bozar | İşlem düzenleme/iptal |
| İşletme/şahsi ayrımının hiç olmaması | Alma (kurucu kararımız tersi) | ADR 0013: ayrım bir raporlama boyutu; MM bu ihtiyacı hiç karşılamıyor — asıl farklılaşma noktamız, negatif referans | — |
| Tekrarlayan: geçmiş/bugünkü tekrarı **otomatik** gerçekleştirme (onaysız), geleceği önizleme | Uyarlayarak al (yalnız önizleme kısmı) | Gelecek tekrarı ayrı "önizleme" bölümünde göstermek = bizim planlanan feed. Ama geçmişi otomatik yazmak `RecurringTransaction` `realize` kuralımıza aykırı — geçmiş vadesi geçmiş olsa bile açık onay | Planlama / tekrarlayan |
| Taksit: tam tutarı bir kerede borç yazmama, ay ay ekstreye bölme, "(1/6)" başlık | Doğrudan al (aynı fikir) | `InstallmentPlan` yalnız niyettir; her `realize()` item kendi ayının `CreditCardCharge`'ını üretir. MM'nin bölme mantığı bizimkiyle aynı, sadece gerçekleşme otomatik | Taksit planı / kart ekstresi |
| Kısmi kart ödemesi: "Ödeme" tutarı düzenlenebilir, kısmi ödeme yalnız kesilmiş ekstreye | Doğrudan al | Bizim kısmi tahsilat/ödeme (D3) ile uyumlu; ödeme kaynağı ve tutarı serbest | Kart ödemesi |
| Fiş kamerası = ek dosya, OCR yok | Not | ADR 0011 öneri katmanı MM'de yok; sade fotoğraf saklama | — |

## Kanıt ve güven düzeyi

- Manuel gözlem (Tur 1, 1 Eyl): K00–K08 eski PC/İngilizce, sentetik veriyle
- Manuel gözlem (Faz 1, 10 Eyl): Türkçe arayüzde hesap kurulumu + 5 çekirdek
  işlem (Ağustos tarihli) + kart ekstre modeli + Ödeme butonu + Tekrar/Taksit +
  açılış bakiyesi + silme/doğrulama. Kareler `kanitlar/money-manager/02`–`16`
  (Türkçe; eski İngilizce kareler değiştirildi). Kontrol değerleri birebir tuttu
- Resmî kaynak: —
- Yorum: "MM para modeli BusinessFinance ile uyumlu" — Faz 1 kontrol değerleri +
  Takvim görünümündeki nötr transfer/ödeme hücreleriyle desteklendi
- Manuel gözlem (Faz 1 ek, 10 Eyl): B1 tekrarlayan gider + üretim davranışı
  (`17`–`20`), B2 taksit planı + ekstre bölünmesi (`21`–`23`), kısmi kart
  ödemesi (`24`–`25`), fiş kamerası (ek dosya, OCR yok). Kareler `02`–`25`
- Not: B1/B2 sonrası MM test verisi çekirdek durumdan **saptı** (Ana Hesap
  ₺39.200, taksit serisi + tekrarlayan seri aktif). Temiz ₺44.950 kontrol
  durumu `11-hesaplar-final-kontrol-degerleri.png` karesinde. **A/B/B1/B2'nin
  dördü de MM'de canlı koşuldu** (A: ₺400 kısmi kart ödemesi `24`–`25`; B: fiş
  kamerası ek dosya, OCR yok; B1: tekrarlayan `17`–`20`; B2: taksit `21`–`23`).
  MM Tur 2'ye seçilse bile veri sıfırlanmaz, üzerine eklenir (11 Eyl 2026'da
  genel kural olarak kesinleşti)
- Fatura→tahsilat MM'de yok. Tur 2'de D1–D4
- Kapsam dışı (kullanıcı kararı, 10 Eyl): bütçe kurulum ekranı, Excel export,
  yedek/geri yükleme

## Tek cümlelik sonuç

Money Manager hızlı ve alışkanlık kurduran bir kişisel bütçe defteri; işlem formu, liste hiyerarşisi ve kredi kartı ekstre modeli bize örnek olur, ama işletme/şahsi ayrımı ve "sil yerine iptal" hiç olmadığı için asıl farkımızı gösteren negatif referans.
