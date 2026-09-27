# Uygulama Gözlem Formu — Money Manager (Realbyte)

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Money Manager / Realbyte Apps |
| Sürüm | **4.12.8** (`versionCode=1197`; Ayarlar ekranında `4.12.8 AD` yazıyor — reklamlı sürüm), 7 Eyl 2026 yeni PC emülatörüne temiz kuruldu |
| Test tarihi | 1 Eylül 2026 (Tur 1, eski PC, İngilizce) · **10 Eylül 2026 (Faz 1 boşluk koşumu, yeni emülatör, Türkçe)** |
| ⚠ Sürüm/dil farkı | Tur 1 formu ve o turun 02–13 numaralı eski kareleri **eski PC / İngilizce arayüzden** (bu eski kareler bugün diskte yok). Yeni emülatörde uygulama sistem diliyle **Türkçe** açılıyor ve Tur 1 verisi yok (temiz kurulum). Faz 1'de veri Türkçe arayüzde **Ağustos 2026** tarihleriyle yeniden girildi; kareler Türkçe yenileriyle değiştirildi. Tur 1'in "01.09'a tek güne girildi" tarih sapması bu koşumda **düzeltildi**. |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400, Android 17) |
| Dil / para birimi | **Türkçe arayüz** (sistem dili) / TRY (₺) |
| Hesap veya plan türü | Ücretsiz sürüm (Ayarlar'da `4.12.8 AD`) — karelerde reklam alanı yalnız boş "Ad" yer tutucusu olarak görünüyor (Hesaplar altı, Ayarlar üstü); yüklenmiş bir reklam karede yok |
| Erişim kısıtı | Yok — kayıt/giriş/e-posta istemiyor, tamamen yerel. Açılışta SMS izni ("Gerekli Değil") ve bildirim izni sorar, ikisi de reddedildi |

## Görev gözlemleri

| Görev | Sonuç | Yaklaşık adım | İyi çalışan | Sürtünme/belirsizlik | Kanıt |
|---|---|---:|---|---|---|
| K00 İlk açılış ve kayıt | Tamamlandı | 0 | Kayıt/giriş/onboarding **hiç yok**; ilk açılışta doğrudan ana ekran, anında kullanılabilir | Para birimi/dil sorulmuyor; varsayılan USD | Kullanıcı doğruladı: hiçbir ekran görmedi |
| K01 Ana ekran | Tamamlandı | 0 | **Gün / Takvim / Ay / Toplam / Not** sekmeleri; üstte **Gelir / Gider / Toplam**; tek turuncu "+" | Ay gezgini + 5 sekme + 3 sütun ilk bakışta yoğun | `02-bos-ana-ekran.png` (veri girildikten sonra boş Eylül ayı, 10 Eyl 11:28; ilk açılış karesi değil), `03-dolu-ana-ekran.png` |
| K02 Hesap/cüzdan oluşturma | Tamamlandı (uyarlanarak) | ~6/hesap | 3 hazır hesap, grupları **Nakit / Banka Hesapları / Kredi Kartı**; grup + hesap iki katmanlı; kart hesabı zengin (Kaynak = ödeme hesabı, Hesap Kesim/Son Ödeme Tarihi, **Bu Ay** vs **Gelecek Ay**) | "Hesap oluştur" butonu yok — hazır hesaplar düzenleniyor; **para birimi hesap başına**; her hesap için 2-3 onay diyaloğu | `09-kart-hesap-ekstre-modeli.png`, `11-hesaplar-final-kontrol-degerleri.png` |
| K03 İşletme geliri | Tamamlandı (kapsamsız) | ~7 | Segment: **Gelir / Gider / Havale**; alanlar az ve net (Tarih · Tutar · Kategori · Hesap · Not · Detay); **Kaydet / Devam et** (seri giriş) | Gelir kategorisi listesi **doğrulanmadı** — Tur 1 (İngilizce) notunda `Allowance/Salary/Petty cash/Bonus/Other` yazıyor ama Türkçe koşumda gelir kategori ızgarasının karesi yok. Kaydedilen gelirin kategorisi **Diğer** (`06-islem-listesi-transfer-notr.png`). Müşteri adı yalnız **Not** alanına giriyor | `04-islem-formu-ve-kategori.png`, `08-hata-toast-hesap-sec.png` |
| K04 Şahsi gider | Tamamlandı (ama ayrım yok) | ~7 | Not girilince liste satırında **başlık = not**, kategori sola küçük etiket (BusinessFinance `title = açıklama ?? kategori` ile birebir) | **İşletme/şahsi (kapsam) ayrımı formda hiç yok.** Protokol gereği taklit edilmedi → bu boyut `Desteklenmiyor` | `06-islem-listesi-transfer-notr.png` (Market/Yiyecek satırı), `04-islem-formu-ve-kategori.png` (11 kişisel gider kategorisi) |
| K05 İşletme kart gideri | Tamamlandı | ~7 | Kart hesabı seçilince gider **aynı gün gider sayılıyor** VE kart borcu +₺1.200 oluyor (BusinessFinance CreditCardCharge kuralıyla aynı) | Kategori yine kişisel liste; **Diğer** seçildi | `06-islem-listesi-transfer-notr.png`, `12-hesaplar-kart-borcu-bu-ay.png` |
| K06 Transfer | Tamamlandı | ~7 | Ayrı **Havale** segmenti; **Kaynak / Giriş** alanları; liste satırı **nötr siyah** renkte, "Ana Hesap → Ortak Cuzdan"; gün başlığı ₺0,00/₺0,00 | Tur 1'de not edilen "swap oku + isteğe bağlı Fees" Türkçe koşumun karelerinde görünmüyor — `Doğrulanamadı` | `06-islem-listesi-transfer-notr.png`, `13-odeme-butonu-onfoldurulmus-havale.png` |
| K07 Liste ve rapor | Tamamlandı | ~3 | **Transfer ve kart ödemesi gelir/gidere 0 etki** (gün başlıkları ₺0,00); **açılış bakiyeleri gelire girmiyor** — Ağustos Gelir tam ₺25.000; kart gideri gidere giriyor; **İstatistik** sekmesinde aylık pasta (Gider: Diğer %58,5 ₺1.200 · Yiyecek %41,5 ₺850) | Rapor kapsam/işletme-şahsi bölmesi yok (zaten kapsam yok). *Tur 1'deki "Salary %100" notu düştü: Türkçe koşumda gelirin kategorisi **Diğer** ve `07-rapor-agustos.png` karesi gelir değil gider pastasını gösteriyor* | `07-rapor-agustos.png`, `03-dolu-ana-ekran.png`, `10-takvim-gorunumu.png` |
| K08 Düzeltme/iptal | Tamamlandı | ~2 | Satıra dokun → **aynı formda satır içi düzenleme**; alttan **Sil / Kopya / Hızlı erişim** | **Silme = tek onay → kalıcı.** "İptal/void" veya geri alma yok. Zorunlu alan hatası **toast**: "Lütfen hesabı seçiniz." | `08-hata-toast-hesap-sec.png` (yalnız toast; düzenleme formu, Sil/Kopya/Hızlı erişim ve silme onayı karede yok — koşum notu) |

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

| Alan | Gezildi | Kısa gözlem | Kanıt |
|---|---|---|---|
| Tüm ana sekmeler | ✓ | **Gün** (gün listesi), **Takvim** (aylık ızgara — hücre başına tek sayı, üç satır değil; Ağustos verisinde aynı günde iki yön olmadığı için net mi ayrı mı gösterildiği doğrulanamaz, havale günleri siyah 0,00), **Ay** (yıllık liste, ay→hafta açılır), **Toplam** (bütçe + hesap kırılımı), **Not** (işlemden ayrı memo, boş). Beşi de aynı 3 sütun başlığı + FAB. Hafta sonu başlıkları renkli (**Paz** kırmızı, **Cmt** mavi) | `10-takvim-gorunumu.png` |
| Rapor drill-down | ✓ | İstatistik pastası → kategoriye tıklanınca o kategorinin işlem listesi (drill-down karesi yok, koşum notu). **Toplam** sekmesi ayrıca gideri kaynağa göre bölüyor. Ağustos 2026 (12 Eyl çekimi, üst toplam Gider ₺3.050): **Gider (Nakit, Banka Hesapları)** ₺850,00 · **Gider (Kredi Kartı, Ödeme)** **₺2.200,00(₺1.200,00)** · **Havale (Nakit, Banka Hesapları →)** ₺0,00 · **Giderleri Karşılaştır (Son ay)** %100. Kart satırı dönemin kart harcamasının (kart defterinde Çekme ₺2.200) yanında **parantez içinde o dönem ödenen tutarı** (Para Yatırma ₺1.200) veriyor; ₺2.200 dönem sonu borcu değil (borç ₺1.000, `31-kart-defteri-agustos-hareketler.png`) | `26-toplam-sekmesi-agustos.png` |
| Bütçe / planlama | ✓ (yüzeysel) | **Toplam** sekmesinde **Bütçe** bloğu + **"Bütçe Ayarları >"** linki. Kategori bazlı aylık bütçe kurgusu; kurulum ekranı kapsam dışı (kullanıcı kararı) | `26-toplam-sekmesi-agustos.png` |
| Ayar derinliği | ✓ | **Daha** → **Ayarlar** ızgarası, dokuz kutu: **Ayarlar · Hesaplar · Giriş Kodu · CalcBox · PC'den Yönet · Yedekle · İletişim · Yardım · Tavsiye et**; altta **"Reklamlar Kaldır"**, sağ üstte sürüm `4.12.8 AD`. `Ayarlar` içinde (Tur 1 notu, Türkçe karesi yok → `Doğrulanamadı`): kategori ayarları, ana+alt para birimi, başlangıç ekranı, ay/hafta başlangıcı, devir (carry-over), kaydırma davranışı, gelir-gider renk seti. Dışa aktarma ayrı yerde: **Toplam** sekmesindeki "Excel(.xlsx) e-posta olarak gönder" | `30-ayarlar-izgarasi.png` |
| Arama ve filtre | ✓ | Filtre paneli ("Eğer filtre uygulamak istediğiniz öğeyi seçin"): **Gelir / Gider** donut yüzdeleri + Toplam; alt sekmeler **GELİR / GİDER / HESAP**. HESAP sekmesinde her hesap için dört değer, **"Her şey"** başlık satırı ve checkbox ile süzme. Ayrı büyüteç ile serbest metin arama. **Türkçe sürümde havale sütun başlıkları ters** — aşağıdaki Faz 7.5 bölümüne bakın | `27-filtre-paneli-agustos-hesap.png` |
| Boş durum | ✓ | Gri çizgi illüstrasyon + **"Veri yok."** Aynı Gün görünümünde iki farklı illüstrasyon görüldü (`02-bos-ana-ekran.png` Eylül'de astronot, `20-tekrarlayan-ekim-onizleme.png` Ekim'de kedi); neye göre değiştiği bilinmiyor. Yönlendirici değil (ne yapılacağını söylemiyor) | `02-bos-ana-ekran.png`, `20-tekrarlayan-ekim-onizleme.png` |
| Hata / uç durum | ✓ | Zorunlu alan eksikse **toast**: **"Lütfen hesabı seçiniz."** (kırmızı ₺ ikonu), inline alan hatası yok. Tutar 0,00 sessizce kabul (sıfır tutar engellenmiyor). Formdan "geri" **taslak uyarısı vermeden** çıkıyor (bu iki davranış koşum notu; karesi yok) | `08-hata-toast-hesap-sec.png` |
| Widget / hızlı giriş | Kısmen | Form içi "Continue" = kaydet + boş forma dön (seri giriş). Ana ekran widget'ı denenmedi | — |

## Arayüz incelemesi

| Başlık | Kısa gözlem |
|---|---|
| Bilgi hiyerarşisi | Zaman odaklı (ay → gün). Para özeti hep üç sütun: **Gelir** (mavi) / **Gider** (turuncu) / **Toplam** (siyah) |
| Alt/üst gezinme | Alt: **İşlemler / İstatistik / Hesaplar / Daha**. Üst: ay gezgini + favori + arama + filtre |
| Renklerin anlamı | Gelir mavi, gider turuncu-kırmızı, net/transfer nötr siyah; birincil eylem turuncu FAB. Tutarlı |
| Tipografi ve para | Para sağa hizalı, iki ondalık, ₺ ön ekli. Okunur. Hesaplar ekranında kart borcu işaretsiz, kırmızı (₺1.200,00, `12-hesaplar-kart-borcu-bu-ay.png`); eksi işaret yalnız kart defterinin yürüyen bakiyesinde (−1.200,00, `31-kart-defteri-agustos-hareketler.png`) |
| Kart/liste/grafik | **Hesaplar**: gruplu liste, grup alt-toplamı gri başlık. İşlem listesi: gün başlığı + satırlar. **İstatistik**: pasta + yüzdeli liste |
| Form alanları | Tek ekranda 5 alan (Tarih · Tutar · Kategori · Hesap · Not) + **Detay**/fiş(kamera). Kategori/hesap alt panelden ızgara seçim. **"Devam et"** ile arka arkaya giriş. Tutar için ayrı hesap-makinesi tuş takımı. Sağ üstte **"Tekrar/Taksit"** kısayolu |
| Loading/boş/hata/başarı | Boş: illüstrasyon + **"Veri yok."** (yönlendirici değil). Başarı sessiz (toast yok). Hata = toast, inline değil. Silme tek onaylı |
| Erişilebilirlik | Ücretsiz sürümde reklam alanı var; karelerde yalnız boş "Ad" yer tutucusu görünüyor, İşlemler ekranlarında reklam alanı görünmüyor (Korece reklam gözlemi koşum notu, karesi yok); tuş takımı büyük ve rahat. **Arayüz sistem diliyle tam Türkçe** (Tur 1'de "Türkçe yok" yazılmıştı; yeni emülatörde arayüzün tamamı Türkçe çıktı, tüm kareler bunu gösteriyor) |
| Ekran tutarlılığı | 5 ana sekme + Hesaplar + İstatistik hepsi aynı 3 sütun para başlığını taşıyor — güçlü tutarlılık. Ay gezgini her ekranda aynı yerde. Her ekranda farklı FAB ikonu (işlem +, not +, yok) — bağlama göre |

## Akış özeti

- En kısa ve güçlü akış: İşlem ekleme — "+" → tutar → kategori → hesap → **Kaydet** (~7 dokunuş); **"Devam et"** ile seri giriş
- En fazla sürtünme yaratan akış: Para birimi kurulumu — ana ayar + her hesap ayrı ayrı, hesap başına 2-3 onay diyaloğu
- Uygulamanın hedef kullanıcı varsayımı: Kişisel/gündelik bütçe takibi — işletme değil
- İşletme ve şahsi para yaklaşımı: **Yok.** Kapsam boyutu, mod veya işletme kategorisi bulunmuyor
- Transfer ve kart ödemesi yaklaşımı: **Havale** ayrı segment, rapora 0 etki. Kart hesabında ayrı **"Ödeme"** butonu = ön doldurulmuş Havale
- Planlama, borç ve tahsilat yaklaşımı: **"Tekrar/Taksit"** form içinde; kart hesabı **Hesap Kesim Tarihi** + **Son Ödeme Tarihi** taşıyor, **"Bu Ay"** (kesilmiş dönem) vs **"Gelecek Ay"** (kesilmemiş) ayrımı

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
| Diğer alanlar | **Tür** (Kredi Kartı) · **Ad** · **Tutar** · **Detay**; altta **Toplama Dahil Et** anahtarı ve **Göster/Gizle ayarı** (aşağıdaki Faz 7.5 bölümüne bakın) |

Hesaplar listesinde kart satırı **iki sütun** taşıyor: **Bu Ay** ve **Gelecek Ay**
(`12-hesaplar-kart-borcu-bu-ay.png`). 8 Ağu'daki ₺1.200 harcama kesim tarihi 1
olduğu için "Bu Ay" (01/08–31/08) ekstresine düştü (`12-hesaplar-kart-borcu-bu-ay.png`, ödemeden önce);
18 Ağu'daki ödeme onu kapattı → ikisi de ₺0 (`11-hesaplar-final-kontrol-degerleri.png`).

**BusinessFinance karşılığı:** Bu tam olarak bizim **ekstre projeksiyon modeli** —
"Bu Ay" = kesilmiş ve ödenecek tutar, "Gelecek Ay" = henüz kesilmemiş dönem.
Uygulama bu iki terimi kullanmıyor; ekranda yalnız "Bu Ay" / "Gelecek Ay"
yazıyor. Kullanıcıya iki rakamı ayrı göstermek bizim ekstre projeksiyonumuzla
aynı fikir.

### Kart hesabının kendi defteri (12 Eyl'de eklendi)

Hesaplar ekranı kartı yalnız **iki toplam** olarak gösteriyor (Bu Ay / Gelecek
Ay); kart harcamalarının kendisi **hesabın defterinde**. Bu ekran Faz 1'de
çekilmemişti, doğrulama turunda eklendi.

**Ağustos defteri** (`31-kart-defteri-agustos-hareketler.png`) — üç hareketin
üçü de görünüyor:

| | Para Yatırma | Çekme | Toplam | Bakiye |
|---|---:|---:|---:|---:|
| `1.08.2026 ~ 31.08` | ₺1.200,00 | ₺2.200,00 | −₺1.000,00 | **₺1.000,00** |

| Tarih | Satır | Tutar | Yürüyen bakiye |
|---|---|---:|---:|
| 08 Cmt | `Diğer` · **Mavi Yazilim** | ₺1.200,00 | (−1.200,00) |
| 15 Cmt | `Diğer` · **Tasarim ekipmani (1/6)** | ₺1.000,00 | (−2.200,00) |
| 18 Sal | `Havale` · **Ödeme Bilgisi** Ana Hesap → Is Karti | ₺1.200,00 | (−1.000,00) |

**Eylül defteri** (`32-kart-defteri-eylul-taksit-2-6.png`): `Tasarim ekipmani
(2/6)` ₺1.000 ve ₺400 kısmi ödeme; dönem Toplamı −₺600, Bakiye ₺1.600
(= Bu Ay ₺600 + Gelecek Ay ₺1.000).

Üç şey bu ekrandan okunuyor:

1. **Kart ödemesi defterde "Para Yatırma" sütununda** — gider değil, borcu
   azaltan bir giriş. Harcamalar "Çekme" sütununda. Aynı satır listesinde iki
   yön ayrı sütunlarla ayrılmış.
2. **Satır başına yürüyen bakiye** parantez içinde veriliyor (`(−2.200,00)`),
   yani kullanıcı her hareketin borcu nereye taşıdığını görüyor.
3. Nakit hesabında da aynı düzen var (`16-acilis-bakiye-farki-defter.png`) ama
   sütun adları aynı kalıyor — kart için "Para Yatırma/Çekme" biraz zorlama.

### Kart ödemesi — "Ödeme" butonu

Kart hesabı defterinde `+` FAB'ın **solunda** ayrı, çerçeveli bir **"Ödeme"**
butonu var (`31-kart-defteri-agustos-hareketler.png`, `32-kart-defteri-eylul-taksit-2-6.png` — bu buton yalnız kart hesaplarında çıkıyor). Basınca
**ön doldurulmuş bir Havale (transfer) formu** açılıyor
(`13-odeme-butonu-onfoldurulmus-havale.png`): Tutar = kart borcu tutarı, Kaynak =
kartın varsayılan hesabı, Giriş = kart, Not = "Ödeme Bilgisi". Yani **kart ödemesi
MM'de özel bir transfer** — hesap −, kart borcu −, gelir/gider 0. Bizim
`CreditCardPayment` modelimizle birebir.

### Tekrarlayan işlem (B1 — aylık ₺600 abonelik, canlı test 10 Eyl)

İşlem formunda sağ üstte **"Tekrar/Taksit"** → açılır menü: **Tekrarlama** ve
**Taksit** iki ayrı seçenek (bu açılır menünün karesi yok). Tekrarlama seçenekleri
(`14-tekrarlama-secenekleri.png`; karede alt gezinmede **Daha** seçili ve saat
tekrar testinden önce — liste formdan değil Daha altından açılmış görünüyor):
Hiçbiri / Günlük / Haftanın günleri / Haftasonu / Haftalık / İki Haftada Bir /
Dört Haftada Bir / Aylık / Ayın Son Günü / İki Ayda Bir / Üç Aylık / Her dört
ayda bir / Her 6 ayda bir / Yıllık.

**Kurulum:** "Aylık" seçilince form üst köşesinde "Aylık" rozeti belirir
(`17-tekrarlayan-aylik-form.png`; tekrarlı formda "Devam et" yok, yalnız Kaydet).
Kaydet'te **tek onay**: "Tarihte tekrar eden işlemler uygulanır. Tekrarlı olarak
kaydetmek istiyor musunuz?" → EVET (onay diyaloğu koşum notu, karesi yok).

**Üretim davranışı (kritik):**

| Dönem | Nasıl görünüyor | Bakiye/rapora etkisi |
|---|---|---|
| 10 Ağu (geçmiş, ilk kayıt) | Listede normal işlem, "Ana Hesap (Aylık)" etiketi (`18-tekrarlayan-agustos-liste.png`) | ✅ Sayılıyor — Ağustos gider +₺600, Ana Hesap −₺600 |
| 10 Eyl (= bugün) | Listede normal işlem, "Ana Hesap (Aylık)" (`19-tekrarlayan-eylul-otomatik.png`) | ✅ Sayılıyor — Eylül gider +₺600, Ana Hesap −₺600 |
| 10 Eki (gelecek) | Ayrı **"Tekrarlama" önizleme bölümü**, gün listesinde değil (`20-tekrarlayan-ekim-onizleme.png`) | ❌ Sayılmıyor — ayın Gelir/Gider/Toplam'ı 0 |

Yani MM: **tarihi ≤ bugün olan tekrarları otomatik gerçek işleme dönüştürür**
(onaysız), gelecek tekrarları önizleme olarak gösterir. BusinessFinance'te
`RecurringTransaction` tanımı **hiçbir** dönemde otomatik gerçekleşmez — geçmiş
vadesi geçmiş olsa bile açık `realize` gerekir. MM'nin "gelecek = önizleme"
kısmı bizim planlanan feed'e benzer; "geçmiş = otomatik" kısmı bizden farklı.

**Seri silme:** Bir occurrence'ı silmek (Sil → tek onay) **yalnız o ayı** siler;
seri devam eder (Ağu 10 silindi, Eyl 10 duruyor; silme anı karede yok, 12 Eyl
kareleri sonuçla tutarlı: Ağustos gideri ₺3.050 `26-toplam-sekmesi-agustos.png`, Ana Hesap Ağustos gideri
₺0 `27-filtre-paneli-agustos-hesap.png`, Ana Hesap ₺39.800 `29-toplama-dahil-kapali-net-varlik.png`). Seriyi tümden yönetmek ayrı
bir yol — Faz 1'de aranmadı.

### Taksit (B2 — tasarım ekipmanı ₺6.000 / 6 taksit, canlı test 10 Eyl)

"Tekrar/Taksit" → **Taksit** → tek alan: "Aylık taksit (Ay sayısı)" → 6 →
Kaydet (ay sayısı alanı koşum notu, karesi yok). Form köşesinde "6 Ay" rozeti
(`21-taksit-6ay-form.png`).

**Davranış:**

- İşlem listesinde **"Tasarim ekipmani (1/6)"** — taksit numarası başlıkta;
  15 Ağu'ya **yalnız ₺1.000** (bir taksit) düştü, ₺6.000 tamamı değil (`22-taksit-1-6-agustos.png`).
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
₺400'e indirildi (`24-kismi-kart-odemesi-400.png`; karede yalnız düzenlenmiş
₺400 var, ön dolu ₺1.000 görünmüyor — ön dolumun kanıtı `13-odeme-butonu-onfoldurulmus-havale.png`'teki o anki borç
₺1.200) → kaydedildi → "Bu Ay" borcu
₺1.000'den **₺600'e** düştü, "Gelecek Ay" ₺1.000 değişmedi (`25-kismi-odeme-sonrasi-borc.png`). Yani kısmi
ödeme **yalnız kesilmiş ekstreye** uygulanıyor. Bizim kısmi tahsilat/ödeme
modelimizle (D3) uyumlu.

### Fiş / kamera

İşlem formunda "Detay" satırında kamera ikonu var — fiş **fotoğrafı ekleme**
(depolama), OCR değil. MM'de otomatik fiş okuma yok (Tur 1 resmî kaynakta da
geçmiyordu). ADR 0011 öneri katmanının karşılığı değil, sade ek dosya.

### Açılış bakiyesi — "Bakiye Farkı"

Hesap düzenleme ekranında **Tutar** alanı var. Alan açılışa özel değil, hesabın
güncel bakiyesini gösteriyor (12 Eyl'de Ortak Cuzdan ₺4.150, `28-toplama-dahil-et-anahtari.png`); boş hesapta
girilen değer açılış bakiyesi işlevi görüyor. Kaydedince:
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


## Faz 7.5 doğrulama turu (12 Eyl 2026) — canlı emülatör kontrolü

Form iddiaları 27 karenin tamamıyla karşılaştırıldı (12 Eyl kaydı; "27" sayısı
bugünkü 30 kareyle eşleştirilemiyor, bkz. 14 Eyl görsel incelemesi); kanıtı olmayan üç iddia
için uygulama emülatörde yeniden açıldı (veri **değiştirilmedi**, yalnız bir
ayar geçici olarak açılıp kapatıldı ve geri alındı).

### Kanıtsız kalan iki iddia doğrulandı

**Toplam sekmesi** (`26-toplam-sekmesi-agustos.png`, Ağustos 2026): gider
kaynağa göre gerçekten bölünüyor — **Gider (Nakit, Banka Hesapları)** ₺850,00
ve **Gider (Kredi Kartı, Ödeme)** ₺2.200,00, yanlarında **Havale** ₺0,00 ve
**Giderleri Karşılaştır (Son ay)** %100. Beklenmeyen ayrıntı: kart satırı dönemin
kart harcamasının yanında **parantez içinde o dönem ödenen tutarı** yazıyor —
`₺2.200,00(₺1.200,00)`: 2.200 = 1.200 Mavi Yazilim + 1.000 taksit (1/6), yani
kart defterinin Çekme sütunu; 1.200 = Para Yatırma (`31-kart-defteri-agustos-hareketler.png`). Dönem sonu borcu
₺1.000'dür, 2.200 değil. Tek satırda "ne harcadın / ne ödedin" okunuyor.

**Filtre paneli** (`27-filtre-paneli-agustos-hesap.png`, Ağustos 2026):
**GELİR / GİDER / HESAP** alt sekmeleri var; HESAP sekmesi hesap başına dört
değer gösteriyor.

**Doğrulanmış bulgu — Türkçe çeviride havale başlıkları ters.** Ağustos
verisiyle üç hesapta birden sınandı; yön tartışmasız:

| Hesap | Gerçekte olan | Ekranda hangi sütunda |
|---|---|---|
| Ana Hesap | ₺4.200 **gönderdi** (3.000 cüzdana + 1.200 karta) | sağ sütun, başlığı `Gelen Havale` |
| Ortak Cuzdan | ₺3.000 **aldı** | sol sütun, başlığı `Giden Havale` |
| Is Karti | ₺1.200 **aldı** | sol sütun, başlığı `Giden Havale` |

Yani sol sütun gerçekte **gelen**, sağ sütun **giden** havaleyi tutuyor; Türkçe
başlıklar ikisini yer değiştirmiş. Tur 1'in İngilizce notundaki sıralama
(`Income / Transfer-In` solda, `Expense / Transfer-Out` sağda) **veriyle
uyuşuyor** — bu yüzden hatanın Türkçe çeviriye özgü olduğu **çıkarımdır**:
İngilizce not eski PC'nin başka koşumundan, aynı sürümün İngilizce karesi yok.
Sayılar doğru, Türkçe arayüzdeki etiketler ters.

### Formda hiç yazılmamış bir ayar: "Toplama Dahil Et"

Hesap düzenleme ekranında (`28-toplama-dahil-et-anahtari.png`) her hesapta iki
anahtar var: **Toplama Dahil Et** ve **Göster/Gizle ayarı**. Anahtar görülen her
karede açık (10 Eyl kart `09-kart-hesap-ekstre-modeli.png` ve Ortak Cuzdan `15-acilis-bakiye-farki-dialog.png`, 12 Eyl `28-toplama-dahil-et-anahtari.png`); bu gözlenen
durumdur, kurulum varsayılanı ayrıca doğrulanmadı. Davranışı canlı ölçüldü:

| Durum | Varlıklar | Borçlar | Toplam | Nakit grubu | Ortak Cuzdan satırı |
|---|---:|---:|---:|---:|---|
| Anahtar **açık** | ₺43.950,00 | ₺1.600,00 | ₺42.350,00 | ₺4.150,00 | ₺4.150,00 (mavi) |
| Anahtar **kapalı** | ₺39.800,00 | ₺1.600,00 | **₺38.200,00** | **₺0,00** | ₺4.150,00 (**gri**) |

Kanıt: `29-toplama-dahil-kapali-net-varlik.png` (kapalı satır). Açık satırın
12 Eyl karesi yok; değerler kapalı kare + Ortak Cuzdan ₺4.150 aritmetiğidir
(39.800 + 4.150 = 43.950; 43.950 − 1.600 = 42.350). Hesap listeden **kaybolmuyor**
ve kendi bakiyesini göstermeye devam ediyor, ama grup alt-toplamından ve net
varlıktan **sessizce düşüyor**.

**Ne kazandırıyor:** kullanıcı kendi parası olmayan bir hesabı (ortak kasa,
emanet, yabancı para cüzdanı) defterde tutup net varlığına katmayabiliyor —
"kaydı görmek" ile "servetime saymak" ayrılıyor. **Ne kaybettiriyor:** ekranda
görünen bakiye ile toplamın ilişkisi kopuyor; kullanıcı anahtarın kapalı
olduğunu unutursa net varlığı sürekli eksik okur ve uygulama bunu hiçbir yerde
uyarmıyor.

**Çapraz bulgu — P4-tema-08 düzeltmesi (15 Eylül):** Bluecoins'te **Nakit Akım
Ayarı** hesap başına rapora katılım seçeneği gösteriyor (E0005 P2-G04).
Kullanılan hesapların yakalanan karede kapalı olması sıfır Nakit Akışı görünümüyle
tutarlı, fakat fabrika varsayılanı veya sıfırın kesin nedeni kanıtlanmadı;
anahtar değiştirilip rapor ölçülmedi. Bu, Money Manager'ın **net varlığa katılım**
anahtarıyla aynı ölçüyü değiştirdiği anlamına gelmez. İki yüzeyde de dahil edilen
hesapların görünür anlatılması bir Belge 3 adayıdır; sonuç eşdeğerliği değildir.

### Veri durumu

Emülatörde MM verisi Faz 1 sonundaki sapmış hâliyle duruyor (Eylül: taksit 2/6
₺1.000, tekrarlayan ₺600, kısmi ödeme ₺400 → Ana Hesap ₺39.800, Toplam
₺42.350). **Hiçbir kayıt silinmedi**; geçici olarak kapatılan anahtar aynı
oturumda geri açıldı ve toplamların döndüğü doğrulandı.

## 20 Eylül 2026 — Belge 2 koşumu (K1, K2)

Yalnız okuma yapıldı: **hiçbir kayıt eklenmedi, silinmedi veya düzenlenmedi**; hiçbir ayar
değiştirilmedi. Koşum listesi `raporlar/belge2-kosum-listesi.md`. Veri durumu 12 Eylül'deki
hâliyle aynı (Ana Hesap ₺39.800, Toplam ₺42.350).

### K1 — kısmi kart ödemesinin gider toplamına etkisi (kapandı)

Eylül'ün üç kaydı: 10 Eyl abonelik ₺600 (Ana Hesap), 10 Eyl kart ödemesi ₺400 (Havale,
Ana Hesap → Is Karti), 15 Eyl taksit 2/6 ₺1.000 (Is Karti).

| Ölçü | Değer | Kanıt |
|---|---|---|
| Eylül Gelir / Gider / Toplam | 0,00 / **1.600,00** / −1.600,00 | `33-eylul-islem-listesi.png` |
| 10 Eylül gün başlığı gideri | **600,00** — aynı günkü ₺400 havale bu başlığa girmiyor | `33-eylul-islem-listesi.png` |
| İstatistik, Eylül | Gider ₺1.600,00; tek kategori `Diğer` %100 | `34-istatistik-eylul-gider.png` |
| Toplam sekmesi, Eylül | `Gider (Nakit, Banka Hesapları)` ₺600,00 · `Gider (Kredi Kartı, Ödeme)` **₺1.000,00(₺400,00)** · `Havale (Nakit, Banka Hesapları →)` ₺0,00 | `35-toplam-sekmesi-eylul.png` |

**Sonuç:** Kısmi kart ödemesi gider toplamına **girmiyor**. Ağustos'ta gözlenen desen Eylül'de de
geçerli: kart satırı dönemin kart harcamasını verir, **parantez içinde o dönem ödenen tutarı**
gösterir; havale satırı ₺0,00 kalır. 1.600 = 600 abonelik + 1.000 taksit; ödeme incelenen Eylül işlem ve rapor toplamlarında
gider olarak görünmüyor.

*İzolasyon:* 12 Eylül'de **Ağustos'taki** ₺600 tekrarlayan occurrence silinmişti; Eylül'e
dokunulmadı. `33-eylul-islem-listesi.png` Eylül'de yalnız bu üç kaydın bulunduğunu gösterir, bu
yüzden toplam yorumlanabilir.

*Yan gözlem (bu koşumda görüldü):* Toplam sekmesinde bir **Bütçe** bloğu var — `Toplam ₺1.400,00`,
`Yiyecek ₺1.400,00`, %0 kullanılmış. Bütçenin ne zaman ve nasıl kurulduğu bu koşumda incelenmedi;
Eylül'ün 1.600'lük gideri `Diğer` kategorisindedir. Bunun `Yiyecek` bütçesindeki %0 ile
ilişkisi olası bir açıklamadır; bütçenin hesaplama/filtre kuralı bu koşumda sınanmadı.

### K2 — kalan taksitler nerede (MM-Q03 kapandı)

Beş yüzey tarandı:

| Yüzey | Sonuç | Kanıt |
|---|---|---|
| İşlem formu → `Tekrar/Taksit` açılır menüsü | Yalnız iki seçenek: `Tekrarlama`, `Taksit`. Liste değil | `36-tekrar-taksit-menusu.png` |
| Gelecek ayların İşlemler ekranı | **Taksitler burada.** 3/6 Eki, 4/6 Kas, 5/6 Ara, 6/6 Oca 2027; her biri gün listesinde görünen satır ve **o ayın Gider toplamında** (1.000,00). Şub 2027 işlem alanında "Veri yok"; −600 Tekrarlama önizlemesi sürüyor | `37-ekim-islemler-taksit-3-6.png`, `38-kasim-islemler-taksit-4-6.png`, `39-aralik-islemler-taksit-5-6.png`, `40-ocak-islemler-taksit-6-6.png`, `41-subat-islemler-veri-yok.png` |
| Is Karti kart defteri | **Yürüyen bakiye geleceğe uzuyor:** Eyl 1.600 → Eki 2.600 → Oca 2027 5.600 | `42-kart-defteri-ekim-bakiye-2600.png`, `43-kart-defteri-ocak-bakiye-5600.png` |
| `Daha` sekmesi | Taksit veya plan listesi girişi görülmedi (sürüm 4.12.8 AD) | `45-daha-sekmesi.png` |
| `Ayarlar` → `Kategori/Tekrar` → `Tekrarlayan İşlemler` | Yalnız tekrarlayan abonelik; **taksit planı bu listede yok** | `46-ayarlar-tekrarlayan-islemler.png` |

**Sonuç:** Kalan taksitler **gelecek ayların işlem listesinde görünür satırlar** olarak duruyor
ve o ayın gider toplamına giriyor. Taranan yönetim listesinde taksit planı görülmedi;
arkada ayrı plan nesnesi bulunmadığı sonucu çıkarılamaz (B12). Tekrarlayan plan bunun tersi
davranıyor: her ayda ayrı bir `Tekrarlama` önizleme bölümünde (₺−600) görünüyor ve **ayın toplamına
girmiyor**. Yani aynı üründe iki farklı gelecek-kayıt davranışı var.

**Aynı koşumda iki farklı dönem kapsamı okunuyor** (20 Eylül, aynı kart; kareler 11:05 ve 11:07):

| Yüzey | Borç |
|---|---|
| Hesaplar → Borçlar (Bu Ay 600 + Gelecek Ay 1.000) | **₺1.600** (`44-hesaplar-borclar-1600.png`) |
| Is Karti defteri → Oca 2027 yürüyen Bakiye | **₺5.600** |

Fark ₺4.000; Ekim–Ocak arasındaki dört adet ₺1.000 taksitle aritmetik olarak tutarlıdır.
Hesaplar ekranındaki iki sütun ile Ocak defterinin yürüyen bakiyesi **aynı dönem kapsamını
ölçmez**; bu iki sayı çelişki diye sunulmaz. Saklama biçimi ve üretim eşiği bu taramayla belirlenmedi.

**Uyarı — bu koşumda iki ayrı ₺1.600 var; bileşimleri farklıdır ve yazımda birleştirilemez.**

| ₺1.600 | Bileşimi | Nerede |
|---|---|---|
| Eylül **gideri** | 600 abonelik (Ana Hesap) + 1.000 taksit 2/6 (Is Karti) | `33-eylul-islem-listesi.png`, `34-istatistik-eylul-gider.png` |
| Is Karti **borcu** | Bu Ay 600 (Ağustos taksiti 1.000 − 400 ödeme, E0249) + Gelecek Ay 1.000 (Eylül taksiti) | `44-hesaplar-borclar-1600.png` |

Eşitlik rastlantıdır. İki kalem her iki toplamda da yoktur: ₺600 abonelik gidere girer, kart
borcuna hiç dokunmaz (Ana Hesap'tan ödendi); ₺400 havale borcu düşürür, gidere hiç girmez.
Bu koşumda ölçülen budur; 12 Eylül'de de `Bu Ay` sütununun yalnız 400 kadar düştüğü kayıtlıdır
(E0249). Aynı sayının iki kez kullanılması, **aynı şeyi** ölçtükleri anlamına gelmez.

**Ek gözlem:** `Tekrarlayan İşlemler` ekranının üstünde `Tekrar ne zaman uygulanır? → Tarihte`
ayarı var. Faz 1'de gözlenen "tarihi ≤ bugün olan tekrarlar otomatik gerçek işleme dönüşüyor"
davranışının ayar karşılığı bu olabilir; **ayar değiştirilip sınanmadı**, bu bağ çıkarımdır.

### Bu koşumda kapanmayanlar

- Kart borcunun tamamen ödenmesi denenmedi (mevcut bakiyeyi değiştirir).
- Gelecek taksit satırlarının arka planda gerçekten kayıt olarak mı tutulduğu, yoksa tarihten
  türetilen bir görünüm mü olduğu ekrandan çıkarılamaz (B12).
- Bütçe bloğunun kurulumu ve davranışı incelenmedi.

## BusinessFinance için kararlar

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| İşlem listesinde başlık = not, kategori küçük etiket | Doğrudan al (zaten böyle) | BusinessFinance `title = açıklama ?? kategori` ile aynı; doğrulayıcı örnek | Aktivite feed satırı |
| ~7 dokunuşluk tek ekran işlem formu + "Continue" ile seri giriş | Uyarlayarak al | Hızlı; "Continue" (kaydet + yeni forma dön) mantığı bize de uyar; bizde ek olarak kapsam/KDV/indirilebilirlik alanları var | İşlem ekle formu |
| Gelir/gider/transfer segmented kontrolü form başında | Uyarlayarak al | Kayıt türünü en başta seçmek net; bizde transfer + kart ödemesi + cari tahsilat da var | İşlem ekle launcher |
| Hesap grupları (**Nakit / Banka Hesapları / Kredi Kartı**) + kartta kesim/ödeme günü + **"Bu Ay" vs "Gelecek Ay"** | Uyarlayarak al | **Kazandırdığı:** kullanıcı "şu an ödemem gereken" ile "henüz kesilmemiş" tutarı iki ayrı sayıda görüyor; ekstre kavramı hiç anlatılmadan anlaşılıyor. **Kaybettirdiği:** iki sütun her kart satırını genişletiyor ve tek kartlı kullanıcıya fazla geliyor. Bizim ekstre projeksiyon modelimizle aynı fikir | Kart ekranı, ekstre |
| **Toplam** sekmesinde ayın gideri "Nakit, Banka Hesapları" ve "Kredi Kartı, Ödeme" diye ayrı satır + "Havale" satırı, kart satırında `borç(ödenen)` | Uyarlayarak al | **Kazandırdığı:** "kart harcaman ayrı" bilgisi tek bakışta okunuyor, havaleyi ₺0 olarak açıkça yazmak transferin gider olmadığını öğretiyor, `₺2.200,00(₺1.200,00)` biçimi (Ağustos; Eylül Toplam karesi çekilmedi) dönemin kart harcamasını ve ödemesini tek satırda veriyor. **Kaybettirdiği:** dört farklı toplam yan yana; kullanıcı hangisinin "ayın gideri" olduğunu seçmek zorunda | Özet / aylık rapor ekranı |
| Zorunlu alan hatası = toast ("Lütfen hesabı seçiniz."), inline değil; sıfır tutar sessiz kabul; taslak uyarısı yok | Alma | **Kazandırdığı:** form hiç kırmızıya boyanmıyor, hata tek satırda geçip gidiyor — hızlı girişte görsel gürültü yok. **Kaybettirdiği:** toast hangi alanın eksik olduğunu ekranda işaretlemiyor, ekran okuyucuyla kaçırılabiliyor, sıfır tutarlı kayıt sessizce oluşuyor ve terk edilen form uyarısız siliniyor | Form doğrulama |
| Boş durum ekranları yönlendirici değil (sadece "Veri yok.") | Alma | **Kazandırdığı:** ekran sessiz ve sade kalıyor, kullanıcıyı yönlendirme metniyle meşgul etmiyor. **Kaybettirdiği:** ilk kez açan kullanıcıya bir sonraki adımı söylemiyor; aynı görünümde farklı illüstrasyon çıktığı için tutarlı bir "boş" dili kurulmuyor | Tüm liste ekranları |
| Para birimi hesap başına + çok adımlı değiştirme | Alma | Tek para birimi (TRY) varsayımımız var | — |
| Açılış bakiyesi = **"Bakiye Farkı"** hareketi + "İşlemler'de gösterilsin mi?" sorusu | Uyarlayarak al | **Kazandırdığı:** açılış bakiyesi sihirli bir sayı değil, hesabın kendi defterinde görünen bir hareket; kullanıcı bakiyeyi sonradan düzeltince de aynı mekanizma çalışıyor ve fark izlenebilir kalıyor. **Kaybettirdiği:** hareket **bugünün tarihiyle** yazılıyor — geçmiş bir döneme açılış konulamıyor; ayrıca kullanıcıya "bunu feed'de görmek ister misin?" diye bir soru daha soruluyor. Bizde `OpeningBalance` kalıcı kolon: soru yok ama düzeltme izi de yok | Hesap oluşturma |
| Düzenleme = satır içi; silme = kalıcı, tek onay | Alma | **Kazandırdığı:** en hızlı düzeltme yolu — yanlış yazılan tutar iki dokunuşta düzeliyor, kullanıcı iptal/yeni kayıt kavramını hiç öğrenmiyor. **Kaybettirdiği:** finansal geçmiş yeniden yazılabilir hâle geliyor; geçmiş bir ayın raporu bugün değişebilir ve müşavire giden sayı ile ekrandaki sayı ayrışabilir. Bizim "sil yerine iptal, düzeltme = iptal + yeni" kuralımız hızı verip denetlenebilirliği alıyor | İşlem düzenleme/iptal |
| İşletme/şahsi ayrımının hiç olmaması | Alma | **Kazandırdığı:** form beş alanda kalıyor, kavram sayısı az, günlük kayıt hızlı — ürün kişisel bütçe kullanıcısına tam oturuyor. **Kaybettirdiği:** şahıs şirketi sahibi aynı defterde iki tür parayı ayıramıyor; MM'de bunun hiçbir dolaylı yolu da yok (etiket, ikinci defter, kapsam alanı — üçü de yok). ADR 0013 ters ödünleşimi seçiyor: bir alan fazla, ayrım mümkün | — |
| Tekrarlayan: geçmiş/bugünkü tekrarı **otomatik** gerçekleştirme (onaysız), geleceği önizleme | Uyarlayarak al (yalnız önizleme kısmı) | Gelecek tekrarı ayrı "önizleme" bölümünde göstermek = bizim planlanan feed. Ama geçmişi otomatik yazmak `RecurringTransaction` `realize` kuralımıza aykırı — geçmiş vadesi geçmiş olsa bile açık onay | Planlama / tekrarlayan |
| Taksit: tam tutarı bir kerede borç yazmama, ay ay ekstreye bölme, "(1/6)" başlık | Doğrudan al (aynı fikir) | `InstallmentPlan` yalnız niyettir; her `realize()` item kendi ayının `CreditCardCharge`'ını üretir. MM'nin bölme mantığı bizimkiyle aynı, sadece gerçekleşme otomatik | Taksit planı / kart ekstresi |
| Kısmi kart ödemesi: "Ödeme" tutarı düzenlenebilir, kısmi ödeme yalnız kesilmiş ekstreye | Doğrudan al | Bizim kısmi tahsilat/ödeme (D3) ile uyumlu; ödeme kaynağı ve tutarı serbest | Kart ödemesi |
| Fiş kamerası = ek dosya, OCR yok | Not | **Kazandırdığı:** hiçbir yanlış okuma riski yok, kullanıcı ne eklediğini bilir. **Kaybettirdiği:** tutar/tarih/satıcı yine elle giriliyor; ADR 0011'in öneri katmanının verdiği hız yok | — |
| **"Toplama Dahil Et"** — hesap başına net varlık anahtarı (`28-toplama-dahil-et-anahtari.png`, `29-toplama-dahil-kapali-net-varlik.png`) | Henüz karar verme | **Kazandırdığı:** kullanıcının kendi parası olmayan hesabı (emanet, ortak kasa) defterde tutup net varlığına saymama imkânı. **Kaybettirdiği:** kapalıyken hesap listede görünmeye devam ettiği için toplam sessizce eksik okunuyor ve uyarı yok — Bluecoins'te aynı desen ("Nakit Akım Ayarı") raporu ₺0 gösterecek kadar ileri gitmişti. Bizde net varlık her zaman bütün hesapları kapsıyor | Hesaplar / net varlık |
| Filtrede hesap başına **gelir / gider / gelen havale / giden havale** dört değeri — iki sütunda ikişer satır (`27-filtre-paneli-agustos-hesap.png`) | Uyarlayarak al | **Kazandırdığı:** bir hesabın ayını dört sayıda özetliyor ve havaleyi gelir-giderden ayrı tutuyor — birleşik feed'imizin hesap kırılımı için doğrudan model. **Kaybettirdiği:** her hücrede iki sayı üst üste, dar ekranda yoğun; ayrıca Türkçe sürümde iki havale başlığı **yer değiştirmiş** (doğrulandı) — sayı doğru, etiket yanlış, kullanıcı yönü ters okuyor. Yerelleştirmede yön kelimelerinin ne kadar kolay ters çevrildiğine dair somut uyarı | Aktivite feed filtresi |

## Kanıt ve güven düzeyi

- Manuel gözlem (Tur 1, 1 Eyl): K00–K08 eski PC/İngilizce, sentetik veriyle
- Manuel gözlem (Faz 1, 10 Eyl): Türkçe arayüzde hesap kurulumu + 5 çekirdek
  işlem (Ağustos tarihli) + kart ekstre modeli + Ödeme butonu + Tekrar/Taksit +
  açılış bakiyesi + silme/doğrulama. Kareler E0227–E0241 (tam adlar `KANIT-ENVANTERI.md`'de; Türkçe; eski İngilizce
  kareler değiştirildi — **05 numarası boş**, sınıflandırma karesi çekilmemiş).
  Kontrol değerleri birebir tuttu
- Resmî kaynak: —
- Yorum: "MM para modeli BusinessFinance ile uyumlu" — Faz 1 kontrol değerleri +
  Takvim görünümündeki nötr transfer/ödeme hücreleriyle desteklendi
- Manuel gözlem (Faz 1 ek, 10 Eyl): B1 tekrarlayan gider + üretim davranışı
  (`17-tekrarlayan-aylik-form.png`, `18-tekrarlayan-agustos-liste.png`,
  `19-tekrarlayan-eylul-otomatik.png`, `20-tekrarlayan-ekim-onizleme.png`), B2
  taksit planı + ekstre bölünmesi (`21-taksit-6ay-form.png`,
  `22-taksit-1-6-agustos.png`, `23-taksit-kart-borcu-bu-gelecek-ay.png`), kısmi
  kart ödemesi (`24-kismi-kart-odemesi-400.png`,
  `25-kismi-odeme-sonrasi-borc.png`), fiş kamerası (ek dosya, OCR yok). Kareler E0227–E0249
- Not: B1/B2/A sonrası MM test verisi çekirdek durumdan **saptı**. Zincir:
  B1/B2 sonrası Ana Hesap ₺39.600, net ₺41.750 (10 Eyl 12:10, `23-taksit-kart-borcu-bu-gelecek-ay.png`); A kısmi
  ödemesi sonrası Ana Hesap ₺39.200, net ₺41.750 (12:12, `25-kismi-odeme-sonrasi-borc.png`); Ağu 10 tekrarı
  silinmiş hâl Ana Hesap ₺39.800 (12 Eyl, `29-toplama-dahil-kapali-net-varlik.png`). Temiz ₺44.950 kontrol
  durumu `11-hesaplar-final-kontrol-degerleri.png` karesinde. **A/B/B1/B2'nin
  dördü de MM'de canlı koşuldu** (A: ₺400 kısmi kart ödemesi E0248–E0249; B: fiş
  kamerası ek dosya, OCR yok; B1: tekrarlayan E0241–E0244; B2: taksit E0245–E0247).
  MM Tur 2'ye seçilse bile veri sıfırlanmaz, üzerine eklenir (11 Eyl 2026'da
  genel kural olarak kesinleşti)
- Fatura→tahsilat MM'de yok. Tur 2'de D1–D4
- **Manuel gözlem (Faz 7.5 doğrulama, 12 Eyl):** Toplam sekmesi (`26-toplam-sekmesi-agustos.png`), filtre
  paneli HESAP sekmesi (`27-filtre-paneli-agustos-hesap.png`), hesap düzenleme anahtarları (`28-toplama-dahil-et-anahtari.png`), "Toplama
  Dahil Et" kapalıyken net varlık (`29-toplama-dahil-kapali-net-varlik.png`), Ayarlar ızgarası (`30-ayarlar-izgarasi.png`) ve
  **kart hesabının kendi defteri** (`31-kart-defteri-agustos-hareketler.png` Ağustos, `32-kart-defteri-eylul-taksit-2-6.png` Eylül). Bu yedi kare,
  daha önce **kanıtsız** olan dört iddiayı doğruladı ve kart harcamalarının
  hiç görünmediği boşluğu kapattı
- **Doğrulanamadı:** gelir kategori listesi (Türkçe koşumda karesi yok) ·
  transfer formundaki "swap oku / Fees" alanları
- **Kanıt dönemi:** kare `26-toplam-sekmesi-agustos.png`, `27-filtre-paneli-agustos-hesap.png` ve `31-kart-defteri-agustos-hareketler.png` **Ağustos 2026** dönemindedir —
  sentetik verinin çoğunluğu o aya girildi, dolayısıyla ekranlar dolu görünür
  (`MANUEL-TEST-PROTOKOLU.md` → "Kanıt dönemi Ağustos 2026")
- **14 Eyl 2026 görsel incelemesi:** 30/30 kare açıldı; tamamı Türkçe, E0227–E0249
  10 Eyl 11:08–12:12, E0250–E0256 12 Eyl 08:59–09:24 (durum çubuğu). Aşağıdaki
  "27 kare" sayısı 12 Eyl kaydıdır, bugünkü dosyalarla eşleştirilemiyor.
  Ayrıntı: `KANIT-ENVANTERI.md` P1-money-manager-G01
- **12 Eyl 2026 denetimi:** 27 karenin tamamı metinle karşılaştırıldı. Düzeltilen
  hatalar: 9 ölü kare atıfı (Faz 1'de İngilizce kareler değişirken form
  güncellenmemişti), 11 yerde İngilizce arayüz etiketi (ekran tamamen Türkçe),
  "Salary %100" (gelirin kategorisi **Diğer**), "Türkçe yok" (arayüz Türkçe),
  "takvimde üç satır" (hücre başına tek sayı), toast metni (kategori değil
  **hesap**). Kanıt atıfları tam dosya adına çevrildi
- Kapsam dışı (kullanıcı kararı, 10 Eyl): bütçe kurulum ekranı, Excel export,
  yedek/geri yükleme

## Tek cümlelik sonuç

Money Manager hızlı ve alışkanlık kurduran bir kişisel bütçe defteri: az alan, az kavram, yedi dokunuşta kayıt — ve bu sadeliğin bedeli olarak işletme/şahsi ayrımı, denetlenebilir düzeltme izi ve yönlendirici boş durum yok; kredi kartı ekstre modeli ("Bu Ay" / "Gelecek Ay") ile gideri kaynağa bölen Toplam sekmesi bizim kart ve rapor tasarımımıza doğrudan girdi, "Toplama Dahil Et" anahtarı ise net varlığı sessizce eksilten bir desen olarak Belge 3'e taşınacak.

## 23 Eylül 2026 — ek eksik koşumu (22 Eylül) ve kare doğrulaması

Koşum listesi ve ham sonuçlar `raporlar/eksik-kosum-ortak-listesi.md` (MM-01…MM-11).
Aşağıdakiler 23 Eylül'de kareler tek tek açılarak doğrulanan ve Belge 2'ye taşınan
kısımdır; kimlikler `KANIT-ENVANTERI.md` E0425–E0461.

**Test verisi (silinmedi):** 22 Eylül'de tutarı boş bir kayıt yazıldı (₺0), sonra 580'e
düzenlendi; ₺300 Ana Hesap → Ortak Cuzdan havalesi; ₺150 aylık tekrarlayan plan
(Günlük Yaşam, 22.10.2026'dan). Eylül gideri 1.600 → 2.180, Ortak Cuzdan 4.150 → 4.450.

| Soru | Gözlem | Kanıt |
|---|---|---|
| Sıfır tutar kabul ediliyor mu (MM-07) | **Evet.** Tutarı boş bırakılan kayıt listeye ₺0,00 olarak girdi, uyarı yok; Eylül toplamı 1.600'de kaldı. Kaydet önce eksik **hesap** alanını istiyor; boş tutar eksik sayılmıyor | `50-sifir-tutarli-kayit-kabul-edildi-eylul-1600.png` (E0434); hesap uyarısı E0232 |
| Tekrarlayanın gerçekleşmesi onaylı mı (MM-04) | **Onaysız.** Kurulumda tek diyalog: "Tarihte tekrar eden işlemler uygulanır. Tekrarlı olarak kaydetmek istiyor musunuz?" (HAYIR/EVET). Ayarlar › Tekrarlayan İşlemler'de "Tekrar ne zaman uygulanır?" iki değer taşıyor: **Tarihte** · **Her ayın ilk günü**. Kayıt başına onay seçeneği yok | `66-tekrarli-kaydetme-onay-diyalogu.png` (E0430), `69-tekrar-ne-zaman-uygulanir-secenekleri.png` (E0429) |
| Bütçe bloğunun %0 görünmesi (MM-05) | Bütçe **kurulu**: tek satır Yiyecek ₺1.400; "Bütçe İşlemler > Toplam içerisinde gösterilecektir." Kategori başına varsayılan + ay ay değer; "Varsayılan bütçeyi değiştirirseniz, önümüzdeki aydan itibaren uygulanır." Ekim: Gider 1.000 (kart taksiti), Yiyecek harcaması 0 → %0. Eylül'ün gideri de tamamen `Diğer`'de (E0404). **20 Eylül yan gözlemindeki "olası açıklama" doğrulandı** | `71-…` (E0431), `72-…` (E0432), `70-…` (E0433) |
| Filtre toplamı yeniden hesaplıyor mu (MM-06) | Evet, anında: HESAP sekmesinde yalnız Is Karti seçilince Gider ₺1.000 (%45), Toplam −1.000; havale "Havale : ₺400,00" diye ayrı satır | `55-filtre-is-karti-secili-1000-havale-ayri.png` (E0457) |
| Havale formunda ücret alanı (MM-09) | Havale formunda Tutar'ın yanında **Harç** düğmesi ve Kaynak/Giriş arasında yer değiştirme oku var — yukarıdaki "Doğrulanamadı: transfer formundaki swap oku / Fees alanları" notu bu kareyle kapanır. ₺300 havale ay toplamını değiştirmedi (gider 2.180'de kaldı) | `57-havale-formu-harc-alani.png` (E0462) |
| Silinen kaydın geri alınması (MM-11) | Silme onayı: "Silmek istediğinize emin misiniz?" Daha ızgarasında çöp kutusu kalemi yok (Ayarlar · Hesaplar · Giriş Kodu · CalcBox · PC'den Yönet · Yedekle · İletişim · Yardım · Tavsiye et) | E0415 (aynı ızgara), koşum kaydı |
| CalcBox ve PC'den Yönet (MM-08) | İkisi de uygulama içi araç değil: CalcBox Google Play'de **ayrı bir ürünün** sayfasını açıyor (Realbyte Inc., "Cihazınız bu sürümle uyumlu değil"); PC'den Yönet **ücretli sürüm** ekranına gidiyor ("PC Manager'ı kullanmak için bir yönlendirici gereklidir") | E0455 (basılmaz), `62-pcden-yonet-ucretli-surum-ekrani.png` (E0456) |

Kart defteri Kasım 3.600 ve Aralık 4.600 kareleriyle (`63`, `64`) Ekim 2.600 → Ocak 5.600
zinciri kesintisiz; bu kareler envantere girmedi, E0412–E0414'ün aritmetiğini tekrar eder.

## 24 Eylül 2026 — Belge 1 düzeltme turu: kare doğrulaması

Belge 1'e dayanak yapılmadan önce kareler tek tek açıldı. Aşağıdakiler ya ortak listedeki koşum
özetini kareye göre düzeltir ya da Belge 1'in kullanmadığı bir karede görülen arayüz ayrıntısıdır.
Yeni koşum yapılmadı.

| Soru | Gözlem | Kanıt |
|---|---|---|
| Tutar girişi (MM-01) | Form açılınca altta rakam tuş takımı; hesap makinesi simgesi tuş takımının **sağ sütununda** (sağ alttaki düğme "Bitti"). Simge tam ekran hesap makinesi açıyor (AC ÷ × − + =, 00) | E0463, E0464 |
| Kategori ayrıntısı (MM-02) | Pastadan açılan Diğer ayrıntısı: Toplam 1.600, Şub–Eyl **çizgi** grafik (ortak listedeki "çubuk" değil), kategorinin kayıtları | E0466 |
| Sıfır tutar (MM-07) | Tutar boş form (hesap ve kategori dolu) → kayıt listeye ₺0,00 olarak girdi. "Kaydet önce hesap alanına atladı" bu karelerde görünmüyor; koşum kaydı | E0465, E0434 |
| Daha sekmesi | Ayarlar · Hesaplar · Giriş Kodu · CalcBox · PC'den Yönet · Yedekle · İletişim · Yardım · Tavsiye et tek ızgarada; çöp kutusu kalemi yok. E0254 ile aynı ekran | E0415, E0254 |
| Takvim görünümü | İşlemler › Takvim: gün hücresinde gelir mavi, gider kırmızı tutar; üstte ayın Gelir/Gider/Toplam'ı | E0234 |
| Tekrarlayan işlemler listesi | Ayarlar'da ayrı liste: plan, tarih, sıklık, hesap, kategori; üstünde "Tekrar ne zaman uygulanır? Tarihte" | E0416 |
