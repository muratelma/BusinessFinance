# Uygulama Gözlem Formu — Bluecoins

## P2 kapanış durumu — 15 Eylül 2026

P2-K tamamlandı: 90/90 görsel ve BC-Q01–BC-Q16 değerlendirmesi kayıtlı.
P2-T sonucuna göre ek canlı kontrol gerekmiyor. B10 kapsamı sınırlı, B16 kanıtla
kapalıdır; silme/geri yükleme ve ölçülmemiş otomasyon dalları bilinmiyor.
Wallet kıyasları B09/P3, nihai öneriler B19/P4 sonucunu bekler.
Dayanak: [doğrulama kaydı](../BULGU-DOGRULAMA-KAYDI.md) P2-T ve P2-K bölümleri.
Aşağıdaki koşum tarihleri, yöntem sınırları ve aday kararlar korunur.

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Bluecoins Finance & Budget / Mabuhay Software |
| Sürüm | 13.1.45 (`versionCode=33111`) · Faz 3'te aynı sürüm, kurulum 2026-09-07 |
| Test tarihi | 1 Eylül 2026 (Tur 1, eski PC) · **10 Eylül 2026 (Faz 3 boşluk koşumu, yeni emülatör)** · 11 Eylül 2026 (Faz 7, koşum kaydı) |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400) |
| Dil / para birimi | Türkçe ağırlıklı / TRY; Tur 1 mağazası İngilizce, Faz 3 formlarında İngilizce tarih ve Others/New Account görülüyor. Sistem dilini otomatik alma bu görsellerle doğrulanmadı |
| Hesap veya plan türü | Yerel ücretsiz sürüm; **bulut senkronizasyonu / giriş yok** → Faz 3'te yeni emülatörde Tur 1 verisi **yoktu**, tam yeniden koşum yapıldı (Money Manager Faz 1 gibi). Hazır gelen ₺0 örnek hesaplar (Birikimler, Çek, Cüzdan, Kredi Kartı, Ev İpoteği) silinmedi, net varlığa etkisi yok. |
| Erişim kısıtı | Kayıt/giriş yok; PDF/yazıcı, CSV ve HTML için Premium bilgisi önceki koşum notudur, P2-G04 yalnız seçenekleri gösterir ve son adımı doğrulamaz. **Kararsızlık:** transfer kaydından sonra "Varsayılan klasörü bulamıyor" diyaloğu UI'yi kilitledi; force-stop + yeniden başlatma ile kurtarıldı (yerel DB'de veri kaldı). |

## P2-G01 inceleme sınırı — 14 Eylül 2026

E0016–E0032 (17 görsel) tek tek yeniden açıldı; sonuçlar
`../BULGU-DOGRULAMA-KAYDI.md` P2-G01 bölümündedir.
Aşağıdaki görev tablosu Tur 1'e aittir; Faz 3 kanıtı ayrıca belirtilir.
Adım sayıları önceki koşum tahminidir; bu tur yeniden ölçülmedi.
13.1.45/versionCode bilgisi önceki koşum kaydıdır; mağaza karesi yalnız
v13.1 yenilik metnini gösterir. Cihazda sürüm yeniden kontrol edilmedi.
**15 Eylül P2-G02:** E0033–E0050 de 18/18 incelendi; Bluecoins toplamı
35/90. Taksit, tekrar, kart/cari alanları ve kısmi ödeme sonuçları aşağıda.
**15 Eylül P2-G03:** E0051–E0079 da 29/29 incelendi; toplam 64/90.
Başlangıç ve D1–D3 aşağıda doğrulandı; E0080–E0105 P2-G04'te incelendi.
**15 Eylül P2-G04:** E0080–E0105 de 26/26 incelendi; Bluecoins 90/90.
Arama, filtre, ayarlar, takvim ve çıktı seçenekleri aşağıda kanıt sınırlarıyla kapandı.

## Görev gözlemleri

| Görev | Sonuç | Yaklaşık adım | İyi çalışan | Sürtünme/belirsizlik | Kanıt |
|---|---|---:|---|---|---|
| K00 İlk açılış ve kayıt | Koşumda tamamlandı | ~2 (koşum notu) | Türkçe karşılama, Hadi Başlayalım ve dil seçici görünür | Kayıtsız ilerleme ve demo önerisi önceki koşum notudur; Tur 1 karesinde demo seçimi görünmez | `00-magaza.png`, `01-ilk-acilis.png` |
| K01 Ana ekran | Tamamlandı | 0 | Hesaplar / İşlemler / Hatırlatıcılar / Tümünü göster yatay geçişleri; günlük özet, bütçe ve takvim gibi kartlar; kart listesi kullanıcı tarafından düzenlenebiliyor | İlk bakış yoğun; ekrandaki kartların bir kısmı aynı dönemde “işlem yok” mesajını tekrar ediyor | `02-bos-ana-ekran.png`, `09-ozgun-ozellik.png` |
| K02 Hesap/cüzdan oluşturma | Koşumda tamamlandı | ~8/hesap (koşum notu) | Hesap seçicide Arama/Yeni, Banka/Nakit/Kredi Kartı grupları ve hesap bakiyeleri görünür | Sıfır bakiyeli hesaplar seçiciyi uzatıyor. Açılış tarihi/bakiyesi formu ve bütün hesap türü ağacı bu kareyle doğrulanamaz | `03-dolu-ana-ekran.png` |
| K03 İşletme geliri | Tamamlandı (kapsamsız) | ~9 | Tek formda ad, tarih/saat, planlama, tutar, para birimi, kategori, hesap, bölme, durum, etiket ve not var; ayrı `GELİR` türü | Form yoğun ve varsayılan `GİDER`; işletme hizmet geliri kategorisi/kapsamı yok, `Diğer` kullanıldı | `04-islem-formu.png` |
| K04 Şahsi gider | Kayıt listede görüldü | ~8 (koşum notu) | Market alisverisi −₺850; işlem adı, Diğer kategorisi, hesap ve işlem sonrası bakiye ayrı gösteriliyor | Bu listede kapsam alanı görünmüyor; yalnız görüntüden ürün genelinde desteklenmiyor hükmü çıkarılmaz | `06-islem-listesi.png` |
| K05 İşletme kart gideri | Kayıt listede görüldü | ~9 (koşum notu) | Tasarim yazilimi gideri −₺1.200 ve Is Karti −₺1.200; daha sonraki ödeme kartı sıfırlıyor | Tur 1 listesi taksit alanını göstermez; alan Faz 3 formunda doğrulandı. Ayrı kapsam/tedarikçi alanı incelenen formda görünmedi | `06-islem-listesi.png`; Faz 3: `12-kart-gideri-taksit-alani.png` |
| K06 Transfer | Listede doğrulandı | ~10 (koşum notu) | Hesaplar arası aktarım ve kart ödemesi ayrı transfer bacaklarıyla gösteriliyor; gün netleri sıfır | İki satır aynı olayın iki hesabını gösteriyor. Kaynak/hedef varsayılanı ve ilk seçim sayısı yalnız koşum notudur | `06-islem-listesi.png`; Faz 3 formu: `13-transfer-formu.png` |
| K07 Liste ve rapor | Görüntüde doğrulandı | ~4 (koşum notu) | Net Kazançlar raporunda Ağustos GİDER −₺2.050, Eylül ₺0; kategori/alt kategori hiyerarşisi ve dönem karşılaştırması; Faz 7'de üç çıktı seçeneği görünür | Yatay sekmeler ekran dışında kalıyor. Premium sınırı ve dosya üretimi doğrulanmadı | `06-islem-listesi.png`, `07-rapor.png`, `f7-54-print.png` |
| K08 Düzeltme/iptal | Sıfır tutarlı detay ve silme onayı görüldü | ~4 (koşum notu) | Detayda ₺0,00 ve Silmek istediğinize emin misiniz? onayı; İptal/TAMAM düğmeleri | Düzenleme ve sıfır tutarlı kaydın uyarısız oluşması koşum notu. Silmenin kalıcılığı, çöp kutusu ve geri yükleme bu kareyle doğrulanamadı (B10/BC-Q03) | `08-hata-veya-bos-durum.png` |

## Kontrol değeri doğrulaması

| Değer | Beklenen | Bluecoins | Durum |
|---|---:|---:|---|
| Ana Hesap bakiyesi | 40.800,00 | ₺40.800,00 | ✓ |
| Ortak Cüzdan bakiyesi | 4.150,00 | ₺4.150,00 | ✓ |
| İş Kartı bakiyesi/borcu | 0,00 | ₺0,00 | ✓ |
| Toplam net varlık | 44.950,00 | ₺44.950,00 | Hesap toplamıyla tutarlı; Tur 1 rapor karesinde doğrudan görünmüyor |
| Gelir işlemi | 25.000,00 | ₺25.000,00 | ✓ |
| Gider toplamı | 2.050,00 | −₺2.050,00 | ✓ |

Tur 1 dayanakları: hesap bakiyeleri `03-dolu-ana-ekran.png`, gelir ve
hareketler `06-islem-listesi.png`, gider toplamı `07-rapor.png`.
Tur 1 net varlık 44.950 değeri bu hesapların toplamıyla tutarlı; bu paketteki
Tur 1 rapor karesinde doğrudan görünmez. Faz 3 toplamı ayrı karede görünür.

Transfer ve kart ödemesi listede kaynak/hedef için ayrı satırlar oluşturdu,
fakat tarih başlığındaki toplamı ve gider raporunu ikinci kez etkilemedi.

**Faz 3 (10 Eyl 2026)** — yeni emülatörde 3 hesap (Ana Hesap ₺20.000 Banka, Ortak
Cuzdan ₺2.000 Nakit, Is Karti ₺0 Kredi Kartı) + 5 çekirdek işlem Ağustos 2026
tarihleriyle yeniden girildi. Kontrol birebir tuttu: **Net Kazançlar Ağustos —
Gelir ₺25.000,00 / Gider −₺2.050,00 / Net ₺22.950,00**; **Net Kazanç Eylül —
Varlıklar ₺44.950,00 / Cari hesap ₺0,00**; Net Kazanç satırının sonu
`+` düğmesiyle örtülü, 44.950 sonucu görünür iki satırın farkıyla tutarlı (açılış
bakiyeleri "bugün" tarihli düştüğü için Ağustos satırı ₺22.950, güncel ₺44.950).
İş Kartı ₺0, Ana Hesap ₺40.800. Transfer + kart ödemesi gün toplamında ₺0.
Kareler `11-uc-hesap-kuruldu.png`, `15-cekirdek-5-islem-tamamlandi.png`, `16-kontrol-degerleri-net-kazanc-44950.png`. Not: açılış tarihi emülatörde içinde bulunulan aydan
öncesine çekilemedi (önceki koşum notu; neden doğrulanmadı); açılışlar 10 Eyl tarihli
kaldı. Güncel toplam doğru olsa da açılış tarihleri geçmiş dönem varlıklarını
ve işlem sonrası satır bakiyelerini etkiliyor: `14-islem-listesi-running-bakiye.png`
ara durumunda 12 Ağustos Ana Hesap ₺22.000, 10 Eylül satırında ₺42.000;
`15-cekirdek-5-islem-tamamlandi.png` içinde kart ödemesi sonrası
18 Ağustos ₺20.800, 10 Eylül ₺40.800. Bu yüzden dönem bakiyesi güncel bakiye
yerine kullanılamaz. Açılış tarihini değiştirmenin ürün kısıtı mı emülatör
etkileşimi mi olduğu BC-Q07'de açık.

## Arayüz taraması (görev dışı, ~10 dk)

| Alan | Gezildi mi | Kısa gözlem | Kanıt |
|---|---|---|---|
| Tüm ana sekmeler / alt görünümler | ✓ | Hesaplar, İşlemler, Hatırlatıcılar, Tümünü göster, Bütçe, Net Kazançlar, Öğeler Özeti ve Etiketler görüldü; yatay sekme listesi ekranı aşacak kadar geniş | — |
| Bir raporun içine tıklama | ✓ | Net Kazançlar dönem karşılaştırması ve kategori→alt kategori hiyerarşisi karede görülüyor; hesap/varlık raporuna geçiş ayrı koşum notudur | `07-rapor.png` |
| Bütçe / hedef / planlama ekranı | Kısmen | Bütçe Özeti, Hatırlatıcılar ve işlem formunda `Planlı İşlemler` alanı görüldü; derin oluşturma Tur 2'ye bırakıldı | — |
| Ayarların derinliği | Görüntüde doğrulandı | Ana ekran kartları yanında dil/para/tarih, veri, kategori/hesap, form ve liste sunumu tercihleri görülür; anahtarların sonuç davranışı ölçülmedi | `09-ozgun-ozellik.png`, `f7-39-ayarlar.png`, `f7-44-gelismis-ayarlar.png` |
| Arama ve filtre davranışı | Sonuç/yüzey doğrulandı | Ada araması üç eşleşmenin net toplamını 12.000 gösterir. Sekiz ana filtre ölçütü görünür; canlı güncelleme, çoklu seçim ve profil davranışı denenmedi | `f7-30-arama2.png`, `f7-31-filtre.png` |
| Boş durum ekranları | ✓ | Tur 1 karşılama Hadi Başlayalım diyor; kartlar Bu dönemde hiçbir işlem yok mesajı taşıyor. Faz 3 sıfır bakiye ekranında İlk İşlemi Ekle görünür; demo seçimi bu karelerde doğrulanmadı | `01-ilk-acilis.png`, `02-bos-ana-ekran.png`, `10-fresh-bos-ana-ekran.png` |
| Hata / uç durum | ✓ | Sıfır tutarlı detay, silme onayı ve açıklamasız boş Çöp Kutusu görünür. Uyarısız kayıt oluşması koşum notudur; silme sonucu ve geri yükleme doğrulanmadı | `08-hata-veya-bos-durum.png`, `f7-38-cop-kutusu.png` |
| Widget / hızlı giriş / kısayol | Kısmen | Ana ekranda sabit `+`; düzenlenebilir dashboard kartları var. Android ana ekran widget'ı denenmedi | `09-ozgun-ozellik.png` |

## Arayüz incelemesi

| Başlık | Kısa gözlem |
|---|---|
| Bilgi hiyerarşisi | Dashboard kart tabanlı; işlem listesi gün toplamı → işlem adı/tutar → kategori/hesap → işlem sonrası bakiye hiyerarşisi kuruyor |
| Alt/üst gezinme | Alt bar yerine ekran üstünde yatay ve kaydırılabilir çok sayıda sekme; sağ altta bağlama göre `+`. Çekmece ikincil hedefleri topluyor ancak iki ayrı `Hesaplar` etiketi yönü belirsizleştiriyor |
| Renklerin anlamı ve tutarlılığı | Gelir yeşil, gider pembe/kırmızı, transfer mavi; seçili tür dolu arka planla ve tutar alanındaki +/− işaretiyle destekleniyor |
| Tipografi ve para değerlerinin okunması | Para sağda, iki ondalık ve ₺ ile; günlük toplam ile satır tutarı ayrışıyor. Çok sayıda küçük ikincil metin yoğunluk yaratıyor |
| Kart, liste ve grafik kullanımı | Dashboard kartları; tarih gruplu işlem listesi; iki dönem sütunlu rapor. Varlık raporu tür ve hesap düzeyinde açılıyor |
| Form alanları ve varsayılanlar | Tek ekran çok güçlü ama yoğun. Gider varsayılanı koşum notudur; kart formunda taksit alanı, transfer formunda kaynak/hedef, yön değiştirme ikonu ve Transfer ücreti görülür |
| Loading, boş, hata ve başarı geri bildirimi | Ana kartların boş durumları açıklayıcı; Çöp Kutusu boşken açıklama veya eylem yok. Kayıt başarısı sessizce önceki ekrana dönüyor; silme onaylı, geri alınabilirliği doğrulanmadı |
| Erişilebilirlik / dokunma alanları / metin yoğunluğu | Yatay sekmelerin bir kısmı ekran dışında ve form çok alanlı; Yinelenmek etiketi detayda görülür. Dokunma alanı boyutları, büyük metin ve ekran okuyucu bu tur ölçülmedi |

## Akış özeti

- En kısa ve güçlü akış: Tek ekranda ayrıntılı işlem oluşturma; tarih, planlama, bölme ve etiket başka bir ayrıntı sayfasına dağılmıyor.
- En fazla sürtünme yaratan akış: Hesap seçimi — hazır gelen çok sayıda sıfır hesap arasında kaynak/hedefi iki ayrı bottom sheet'ten bulmak.
- Uygulamanın hedef kullanıcı varsayımı: Finansal model ayrıntısı isteyen ileri seviye kişisel bütçe kullanıcısı; çok hesap, kredi, ipotek ve rapor kullanıyor.
- İşletme ve şahsi para yaklaşımı: **Yok.** Kategori/etiket mevcut ama özel kapsam boyutu değil.
- Transfer ve kart ödemesi yaklaşımı: Ayrı transfer türü, çift bağlı satır, gün toplamında sıfır; kart ödemesi ikinci kez gider değil.
- Planlama, borç ve tahsilat yaklaşımı: Planlı işlem, hatırlatıcı, kredi/ipotek ve taksit alanları var; B1/B2 + bağımsız hatırlatıcı canlı kuruldu (hepsi tek Hatırlatıcılar listesi). Cari hesaba gelir ve bankaya transfer bu koşumda doğrulandı; belirli faturaya tahsis veya ürün genelinde fatura nesnesinin bulunmaması doğrulanmadı.

## Faz 3 boşluk koşumu (10 Eyl 2026) — taksit, tekrarlayan, kart, hatırlatıcı

Yeni emülatörde tam yeniden koşum (bulut yok). Kanıt havuzu E0026–E0044 (envanter kapsamı; tek bir iddiaya aralık atfı değildir).

### İşlem formu — tek yoğun ekran

İsim + tarih/saat + **Planlı İşlemler** (tekrar) düğmesi + tutar (kırmızı −
GİDER / yeşil + GELİR, hesap makinesi widget'ı) + kategori + hesap + (kart
seçilince) **Taksit şartlarını seçin** + Bölmek + Durum + Etiket + Not. Alt bar:
GİDER / GELİR / TRANSFER + yeşil kaydet. Kare `12-kart-gideri-taksit-alani.png`. Hesap makinesi widget'ı bazen
donuyor (emülatör tuzağı) — tutar alanına doğrudan dokunup klavyeyle yazmak çalışır.

### Kredi kartı modeli

Kart, hesap listesinde CARİ HESAP grubundadır. Kart gideri formunda
Taksit şartlarını seçin alanı görünür (`12-kart-gideri-taksit-alani.png`).
Bu koşumda kart ödemesi Ana Hesap → Is Karti transferiyle yapılmıştır;
ayrı ödeme akışının ürün genelinde bulunmadığı sonucu çıkarılmaz.
Kesim günü alanı aşağıdaki hesap formunda görülür; dönem/ekstre davranışı
bu görsellerle doğrulanmadı (BC-Q06).

### Taksit — B2, 6.000 TRY — P2-G02 doğrulaması

`17-b2-taksit-sartlari-sheet.png`: Taksit oranı 0,00; 2 ay; İlk ödeme
11 Eylül 2026. Açılır liste kapalıdır; eski 2/3/6/9/12/15/18/21/24/Özel
listesi bu pakette doğrulanmadı. Oran etiketinde yüzde işareti görünmez;
sıfır dışı değerin anlamı ve hesaplama etkisi denenmedi.

`18-b2-taksit-6ay-15agu.png`: 6 ay, 0,00 ve 15 Ağustos 2026 ilk ödeme
seçilmiş. `19-b2-6ay-hatirlatici-metni.png` kaydetme öncesi 1.000,00
tutarlı altı aylık hatırlatıcı özetini, Değiştir/Sıfırla eylemlerini gösterir.

- `20-b2-1-6-taksit-1000-kayit.png`: İşlemler listesinde 15 Ağustos
  Tasarim ekipmani −1.000 ve 1/6 vardır. O tarihsel satırda kart −2.200;
  18 Ağustos kart ödemesi satırında −1.000'dır. Güncel borç ile harcama
  anındaki bakiye aynı değer değildir.
- `21-b2-kalan-5-taksit-hatirlatici.png`: 2/6–6/6, 15 Eylül 2026'dan
  15 Ocak 2027'ye kadar beş aylık −1.000 hatırlatıcıdır.
- `22-b2-sonrasi-rapor-gider-3050.png`: Ağustos gideri −3.050 ve dönem
  neti 21.950; çekirdek gider −2.050'ye göre fark −1.000'dır.

İlk taksidin kayıttan hemen sonra oluştuğu önceki koşum notudur ve bu
görüntü zinciriyle tutarlıdır. Ancak ilk ödeme test gününden öncedir:
sonuç tarih etkisi mi taksit kuralı mı, geleceğe kurulunca da aynı mı,
ayrıştırılmadı. Kalan taksitlerin otomatik/onaylı gerçekleşmesi açık.
Bu nedenle ilk taksit her koşulda otomatik veya BF modeliyle tek fark
hükmü kurulmaz (B10/BC-Q04).

### Tekrarlayan işlem — B1, aylık 600 TRY — P2-G02 doğrulaması

`23-b1-planli-islem-aylik-sheet.png`: Bir Defa, Günlük, Haftalık, Aylık,
Yıllık; Aylık seçili. Her ay tekrarla, Ayın günü/Haftanın günü, Tarih ve
Son Ödeme Tarihi alanları görünür. Bitiş seçenekleri ekranda Asla,
1 etkinlik sonrar ve Son Tarih olarak yazılıdır. Vade tarihinde otomatik
olarak işlem olarak girin kutusu boştur. **Bu paneldeki tarih 11 Eylül 2026'dır.**

Geçmiş başlangıcın kanıtı `24-b1-yenilenen-islem-banner.png`:
Bulut yazilim aboneligi 600 TRY, Ana Hesap, 10 Ağustos 2026 ve aylık seri
özeti. Otomatik kutunun kapalı kaldığı önceki koşum kaydıyla belirtilir;
son form bu kutuyu tekrar göstermiyor.

- `25-b1-hatirlatici-gecikmeli-bugun.png`: 10 Ağustos kaydı
  31 gün gecikmeli; 10 Eylül kaydı Bugün süresi doluyor; görünen
  sonraki aylık kayıtlar ve taksitler aynı listede.
- `26-b1-hatirlatici-detay-kaydet.png`: −600 abonelik detayında
  Kaydet/Düzenle. `27-b1-islem-olarak-kaydet-bugun-mu.png`:
  İşlem Olarak Kaydet? altında Bugün / 10 Ağustos 2026 seçenekleri.
- `28-b1-onay-sonrasi-rapor-gider-3650.png`: Ağustos gideri −3.650,
  dönem neti 21.350. Önceki taksit sonrası rapora göre gider 600 artmış.
- Sonraki `31-hatirlatici-listesi-bagimsiz-tekrar-taksit.png`
  listesi 10 Eylül'den başlıyor; 10 Ağustos satırı üst bölümde artık yok.
  `34-kismi-kart-odemesi-500-transfer.png` içinde 10 Ağustos −600
  abonelik gerçek işlem olarak görünür. Geçmiş tarihli onay sonucu bu
  zincirle desteklenir; Bugün kolu bu turda doğrulanmadı.

**Kapsam:** Bu koşum otomatik kayıt kapalıyken bekletme ve elle kaydetme
yolunu gösterir. Tanımın onay öncesinde gider üretmediği önceki koşum
notudur; tanım sonrasına ve onay öncesine ait ayrı rapor karesi bu pakette
yok. Tanım hiçbir şey üretmez hükmü ürünün tüm modlarına genellenmez.
Otomatik seçenek ekranda mevcut; açık kolun sonucu bilinmiyor.
Wallet/Money Manager ile evrensel karşıtlık veya BusinessFinance ile
birebir eşdeğerlik kurulmaz (B10/BC-Q04/Q07).

### Bakiye zinciri — P2-G02

Aşağıdaki netler görünür hesap/grup bakiyelerinden hesaplanan kontrol
değerleridir; kırpılmış rapor hücresinin doğrudan okuması değildir.

| Durum | Ana Hesap | Ortak Cuzdan | Kart | Net | Kanıt / sınır |
|---|---:|---:|---:|---:|---|
| Çekirdek sonrası | 40.800 | 4.150 | 0 | 44.950 | `15-cekirdek-5-islem-tamamlandi.png`, `16-kontrol-degerleri-net-kazanc-44950.png` |
| İlk taksit sonrası | 40.800 | 4.150 | −1.000 | 43.950 | `20-b2-1-6-taksit-1000-kayit.png`; gider değişimi `22-b2-sonrasi-rapor-gider-3050.png` |
| B1 onayı sonrası / kısmi ödeme öncesi | 40.200 | 4.150 | −1.000 | 43.350 | `33-cari-hesap-olusturuldu.png` görünür hesap/grup tutarları; Ada Reklam cari 0 |
| 500 kart ödemesi sonrası | 39.700 | 4.150 | −500 | 43.350 | `34-kismi-kart-odemesi-500-transfer.png` |

**Düzeltme:** Eski 44.350 → 43.350 ifadesinin başlangıcı yanlıştı.
Doğru net zinciri 44.950 − 1.000 − 600 = 43.350'dir. 44.350, B1
sonrasında banka ve nakdin toplamıdır; −1.000 kartı içermez.
Kısmi ödeme banka/kart dağılımını değiştirir, neti değiştirmez.
B10'un aritmetik kısmı bu kanıtlarla düzeltildi; otomasyon ve silme
davranışları açık kaldığı için B10 bütünü kapanmadı.

### Transfer formu ve ara durum — P2-G01

`13-transfer-formu.png`: 3000 TRY, Ana Hesap → Ortak Cuzdan,
Transfer ücreti ve yön değiştirme ikonu; TRANSFER seçili.
`14-islem-listesi-running-bakiye.png`: kart ödemesi henüz yok; 12 Ağustos
iki transfer bacağı ve gün neti sıfır. Kart ödemesi
`15-cekirdek-5-islem-tamamlandi.png` içinde eklenmiş.

`05-siniflandirma.png` dosya adına rağmen Günlük Özet ayar ekranıdır:
Gelecek Tahmini, dönem seçicisi, Tablo ve Günlük İstatistikler görünür.
K04 sınıflandırmasına kanıt olarak kullanılmaz; giriş aracı soldaki metni örtüyor.

### Transfer detayı

Kayıt detayı: iki bağlı satır + her satırda işlem sonrası bakiye + **"Benzer
işlemler göster"** + **"Yinelenmek"** (transferi tekrarlı yapma) + Düzenle.

### Hatırlatıcılar sekmesi — birleşik bekleyen görünüm

`31-hatirlatici-listesi-bagimsiz-tekrar-taksit.png` içinde aylık abonelik,
tek seferlik kira ve taksitler tarih sıralı tek listede görünür.
Bugün/gecikme/yarın etiketleri tutarın yanında kullanıcıya vade durumunu
anlatır. Bu ortak görünüm, kayıtların aynı fiziksel tablo veya domain
modelinden geldiğini ve BF ile aynı davranışı taşıdığını kanıtlamaz.

### Kredi kartı hesabının alanları

`32-kart-hesap-kesim-gunu-limit-alanlari.png`: Yeni Hesap ekranında
Başlangıç bakiyesi ve Açılış tarihi aynı formda; Hesap Tipi Kredi Kartı.
Kredi Limiti, Hesap Kesim Günü ve Bitiş tarihi alanları var.
Son iki alanda Ayın 1. Günü seçili. Bitiş tarihi etiketinin iş anlamı,
kesim günü seçenekleri ve ekstreye etkisi bu kareden çıkmaz.
Bu, alan varlığının kanıtıdır; varsayılan kurulum veya dönem davranışı
kanıtı değildir (BC-Q06). Wallet ile karşılaştırma P3 doğrulamasını bekler.

### Bölmek

`29-bolmek-split-modu.png`: Hepsini temizle / + Ekle, Toplam tutar
ve tek alt satırda tutar, kategori, hesap, not, çöp ikonu var. Görünen
satır 0,00, Diğer/Others, Nakit/Cüzdan. Çoklu kalem girişine yönelik
arayüz doğrulanır; farklı hesaplara dolu satırların kaydı ve rapora
etkisi denenmiş sayılmaz.

### Taslak uyarısı — koşum notu

Önceki koşum, kaydetmeden çıkışta Değişiklik kaydetmeden işlem ekranından
çıkın mı? onayı gördüğünü söylüyor. Bu pakette o diyaloğun karesi yok.
Doğrulanmış görsel bulgusu veya diğer uygulamalarda bulunmadığının kanıtı
olarak kullanılmaz (BC-Q14).

### Bağımsız hatırlatıcı

`30-bagimsiz-hatirlatici-bir-kez-program.png`: Ofis kirasi 10000.0 TRY,
Ana Hesap, 11 Eylül 2026 günü bir kez program yapın özeti.
`31-hatirlatici-listesi-bagimsiz-tekrar-taksit.png`: −10.000 kira
11 Eylül altında Yarın borçlanacak etiketiyle; abonelik ve taksitlerle
aynı listede. Bir Defa seçeneği tekrar panelinde görünür; bu formun
tek seferlik özeti onunla tutarlıdır. + yolunun varsayılan tarihi ve
ürünün iç veri modeli yalnız bu karelerden türetilmez.

### Cari hesap — kanıtın sınırı

`33-cari-hesap-olusturuldu.png`: CARİ HESAP grubunda Cari hesap
başlığı altında Ada Reklam cari 0 satırı görünür. Hesabın listede
bulunması doğrulanır. Cari hesap oluşturma formu, özel alanları veya
fatura/tahsilat bağı bu karede gösterilmez.
`32-kart-hesap-kesim-gunu-limit-alanlari.png` kredi kartı formudur;
cari türünün özel alanı yok iddiasına kanıt olarak kullanılmaz.

Cari hesaba gelir ve oradan bankaya transfer, aşağıdaki P2-G03 D2/D3
zincirinde doğrulandı. Bu yolun görülmesi, ürün genelinde fatura nesnesi
yok veya cari hesap yalnız bir kasa hükmünü kanıtlamaz.

P1-B01 düzeltmesi korunur: BusinessFinance cari tahsilatı belirli
borçlandırmaya bağlamaz; bağ karşı taraf düzeyindedir. Belirli kayda
bağlı kapanış tek seferlik yükümlülükte tam tutarla ve borç taksitinde
vardır. Bu bilgi, rakipte görüntülenmeyen bir alanın bulunmadığını
kanıtlamak için kullanılamaz.

### Kısmi kart ödemesi

`33-cari-hesap-olusturuldu.png` ödeme öncesi Ana Hesap 40.200 ve
kart grubu −1.000'i gösterir.
`34-kismi-kart-odemesi-500-transfer.png`: Ana Hesap → Is Karti
transferi −500/+500; Ana Hesap 39.700, Is Karti −500.
Bu tek 500 TRY denemesi doğrulanır; herhangi bir tutar çalışır veya
ekstre dönemi hiç devreye girmez sonucu çıkarılmaz (BC-Q06).

10 Eylül gün toplamı **22.000**'dir; aynı gün iki açılış kaydı vardır.
Transferin iki bacağının neti sıfırdır; gün başlığının sıfır olması gerekmez.

### Fiş / kamera — bu pakette açık

Ataç ikonu işlem formlarında görünür. Önceki koşumda adb dokunuşuyla ek
akışı açılamadığı yazılıdır. Bu paket ek seçicisini veya OCR akışını
göstermez; OCR yok/pazarlanmıyor ve gerçek cihazda sorun olmaz hükümleri
doğrulanmış değildir. Emülatör etkileşimi ve ürün davranışı ayrıştırılmalı
(BC-Q14).

### Faz 3 sonrası veri durumu

Ana Hesap ₺39.700 · Ortak Cuzdan ₺4.150 · İş Kartı −₺500 · Ada Reklam cari ₺0 ·
net ₺43.350. Ek: B1 tekrarlayan serisi (10 Ağu occurrence onaylandı, sonraki
vade 10 Eyl), B2 taksit hatırlatıcıları 2/6–6/6, "Ofis kirasi" ₺10.000 bağımsız
hatırlatıcı (11 Eyl), ₺500 kısmi kart ödemesi. A: 500 kısmi ödeme, B1: otomatik kapalı elle onay ve B2: geçmiş ilk
ödemeli taksit sonuçları P2-G02'de mevcut görsellerle incelendi. B: ek/fiş
akışı doğrulanamadı; dört senaryonun tümü başarılı sayılmaz. Yeni canlı
testler P2-T kapsamına bağlıdır. Bluecoins Tur 2'ye seçildi (11 Eyl); bu veri **sıfırlanmadan**
Faz 7 kayıtları (D1 tamamlama + D2 + D3) üzerine eklenecek.

### Faz 7 — Tur 2 derin koşum (11 Eyl 2026, yapay zekâ)

**P2-G03 görsel doğrulaması — 15 Eylül:** E0051–E0079, 29 kare.
11 Eylül koşumunun kayıtlarıdır; yeni canlı test veya bugünkü cihaz durumu değildir.

**Başlangıç:** `f7-02-hesaplar-scroll2.png` Eylül netini 43.350 gösterir;
`f7-03-hesap-listesi.png` Ana Hesap 39.700, Ortak Cuzdan 4.150 ve
Is Karti −500'ü, `f7-04-tum-hesaplar.png` Ada Reklam cari 0'ı doğrular.
Ağustos akış neti 21.350 ile Eylül varlık neti aynı ölçü değildir.

**D1 — kira gerçekleştirme:** `f7-05-hatirlaticilar.png` içinde
11 Eylül kira −10.000 bugün vadeli, 10 Eylül abonelik hâlâ bekler.
`f7-09-ofis-kirasi-dialog.png` adına rağmen onay değil planlı gider
detayıdır; Kaydet/Düzenle görünür. Gerçek onay
`f7-11-kaydet-dogru.png`: İşlem Olarak Kaydet? → İptal/TAMAM.
`f7-12-ofis-kirasi-realized.png` listesinde kira yok;
`f7-14-islemler-guncel.png` içinde 11 Eylül −10.000 kira ve
Ana Hesap 29.700 var. Böylece 39.700→29.700 doğrulandı.
B1 geçmiş vade iki tarih seçimi ile D1 aynı gün tek TAMAM farkı görünür;
nedeninin tarih mi plan türü mü olduğu ayrıştırılmadı. Otomatik açık kol
hakkında sonuç çıkarılmaz (B10/BC-Q04).

**D2 — 12.000 cari gelir:** `f7-16-fab-tap.png` genel işlem formunda
**Durum** alanını gösterir; eski durum alanı yok ifadesi düzeltilmiştir.
Seçenekler açılmadığı için ödendi/kısmi/gecikmiş gibi fatura durumları
olduğu da olmadığı da söylenemez.
`f7-21-hesap-secici.png` içinde cari 0 ve banka 29.700;
`f7-23-hesap-dogru-secildi.png` içinde Ada Reklam hizmet faturasi
adı, GELİR 12000, Ada Reklam cari seçili.
`f7-24-d2-kaydedildi.png` +12.000 gelir ve cari 12.000'i doğrular;
gün toplamı 12.000−10.000=2.000. Fatura sözcüğü bu akışta işlem adıdır.
Ayrı fatura nesnesinin ürün genelinde bulunmadığı hükmü kaldırıldı.

**D3 — 5.000 aktarım:** `f7-27-transfer-hazir.png` içinde
Ada Reklam kismi tahsilat, 5000, Ada Reklam cari → Ana Hesap ve
TRANSFER seçili. `f7-28-d3-kaydedildi.png` iki bacağı gösterir:
−5.000 sonrası cari 7.000, +5.000 sonrası banka 34.700.
Banka+cari 41.700, gün toplamı 2.000 olarak korunur; D2 satırındaki
12.000 o işlemin tarihsel bakiyesidir. **7.000 cari hesap bakiyesidir;
belirli faturanın kalan tutarı olarak doğrulanmadı.** Formda belirli
fatura seçimi görünmüyor; ürünün başka yollarında bağ bulunmadığı
sonucu çıkarılamaz. D3 sonrası tüm hesaplar net raporu bu pakette yoktur.

P1-B01 düzeltmesi korunur: BF cari tahsilatı karşı taraf düzeyinde işler,
belirli borçlandırmaya bağlanmaz; bu yüzden BF için de fatura bazında
kalanı garantiler denmez. Tek seferlik yükümlülük kapanışı tam tutarlıdır.
Bu paket yeni ürün kararı oluşturmaz.

**Ara karelerin gerçek rolleri:** Aşağıdaki kayıtlar korunur; dosya adı
tek başına bir eylemin başarı kanıtı sayılmaz (BC-Q10).

| Kare | Görünen içerik ve kullanım |
|---|---|
| `f7-00-baslangic.png` | Hesaplar, Günlük Özet boş dönem; 7 gün ortalama 0, 30 gün −33,33; Test Reklamı; Bütçe Özeti; Kanıt eki — başlangıç ve reklam |
| `f7-01-hesaplar-scroll.png` | Eylül 2026 takvimi, 10/11/15 günlerinde işaretler; Ağu gelir 25.000, gider −3.650, net 21.350; Kanıt eki — takvim ve akış |
| `f7-06-ofis-kirasi-detay.png` | Aynı bekleyen liste; kira satırı hâlâ listede; Arşiv — gezinme ara karesi |
| `f7-07-tap-icon.png` | Bekleyen liste, kira satırı ve sonraki vadeler; Arşiv — sonuç üretmeyen ara kare |
| `f7-08-tap-retry.png` | Bekleyen liste saat 13:55; kira −10.000 bekliyor; Arşiv — tekrar kontrolü |
| `f7-10-ofis-kirasi-kaydet-sonuc.png` | Planlı kira detayı, −10.000, Ana Hesap, Kaydet/Düzenle; reklam değişmiş; Arşiv — ara kare |
| `f7-13-islemler.png` | İşlemler listesi; üstte kira satırı gezinme katmanının altında kısmen örtülü; Arşiv — kaydırma ara karesi |
| `f7-15-yeni-islem.png` | Kira sonrası işlem listesi ve artı düğmesi; Arşiv — form öncesi |
| `f7-17-isim-girildi.png` | Ada Reklam hizmet faturasi adı yazılı; gider 0,00, Cüzdan; Kanıt eki — ad girişi |
| `f7-18-gelir-tutar.png` | Tutar odakta; GİDER hâlâ seçili, 0,00; Arşiv — tutar odağı |
| `f7-19-tutar-girildi.png` | 12000 yazılmış; GİDER seçili, Cüzdan; Kanıt eki — tutar girişi |
| `f7-20-hesap-secim.png` | Diğer/Others, İşveren/Bonus/Salary seçenekleri; Kanıt eki — kategori seçicisi |
| `f7-22-hesap-secildi.png` | GELİR 12000, Ortak Cuzdan, Durum alanı; Arşiv — hesap düzeltmesi öncesi |
| `f7-25-transfer-formu.png` | TRANSFER seçili; 0,00; iki hesap Cüzdan; Transfer ücreti, Durum, Etiket; Kanıt eki — transfer başlangıcı |
| `f7-26-check.png` | Ada Reklam kismi tahsilat adı, tutar 0,00, hesaplar Cüzdan; Arşiv — ad girişi |

### Arama, filtre, ayarlar ve çıktı — P2-G04 doğrulaması

`f7-30-arama2.png` içindeki `Ada` sorgusu, D3'ün −5.000/+5.000
transfer bacaklarını ve D2'nin +12.000 gelirini listeler; Toplam 12.000'dir.
Eşleşen satırların net toplamı doğrulanır. Tek sonuç karesi, sorgunun her
tuşta canlı güncellendiğini veya başka alanlarda arama yaptığını kanıtlamaz.

`f7-31-filtre.png` metin/madde adı/alacaklı/not, başlangıç-bitiş tutarı,
tarih aralığı, işlem tipi, kategori, hesap, etiket ve durum alanlarını;
Satır tarzı ile sıfırla/kaydet/yükle ikonlarını gösterir. Çoklu seçim,
kayıtlı profil ve ikonların sonuç davranışı açılmadı. Bu yüzden test edilen
uygulamalar arasında en gelişmiş filtre hükmü kaldırıldı.

`f7-35-nakit-akim-ayari.png` hesap başına nakit akışı seçimini gösterir:
Ana Hesap ve Ortak Cüzdan kapalı; Birikimler, Çek ve Cüzdan açıktır.
Bunlar yakalanmış yapılandırmadır, fabrika varsayılanı değildir. Kullanılan
hesapların kapalı olması önceki sıfır Nakit Akışı görünümüyle tutarlı bir
açıklamadır; anahtar değiştirilip rapor ölçülmediği için nedensellik kanıtı
sayılmaz.

`f7-42-hesap-ayarlari.png` adına rağmen Kategori Ayarlarıdır: gider ve gelir
varsayılanları ayrı ayrı Others; simgeler ve kompakt seçici açıktır.
`f7-43-hesap-ayarlari2.png` Varsayılan Hesap Cüzdan'ı ve Gizli hesapların
seçimi anahtarını gösterir; hesap gizleme akışının kendisini göstermez.
`f7-44-gelismis-ayarlar.png` dövizli işlem için son kur ve son para birimi
tercihlerini taşır; gerçek kur dönüşümü denenmedi. `f7-45-gelismis-scroll.png`
liste görünümü ve doğrudan düzenleme gibi sunum tercihlerini gösterir.

`f7-36-kategoriler2.png` iki seviyeli kategori hiyerarşisini,
`f7-37-etiketler.png` Doğum günü/Film/İş/Kişisel/Tatil listesini gösterir.
Etiketlerin serbest oluşturulması denenmedi; İş/Kişisel adları ADR 0013'teki
kapsam boyutuna eşdeğer sayılmaz. `f7-38-cop-kutusu.png` ve
`f7-46-check-nav.png` açıklamasız boş Çöp Kutusu yüzeyidir; silinen kaydın
buraya düşmesi ve geri yüklenmesi doğrulanmadı.

`f7-47-takvim.png` ile `f7-48-takvim-ayarlari.png` 11 Eylül için gider
−10.000, gelir +12.000 ve net 2.000'i gösterir. Dosya adına rağmen ikincisinde
ayar açılmamıştır. Transfer bacakları akış toplamını şişirmemiştir.

`f7-54-print.png` üç seçenek gösterir: PDF veya Yazıcıya gönder,
Excel (.csv), HTML. Gerçek dosya üretimi, paylaşım ve Premium son adımı
denenmedi. `f7-53-export.png` adına rağmen dışa aktarma değil filtre
sayfasıdır. Veri Yönetimi ekranındaki Excel (.csv) ve QIF içe aktarma
girişleri `f7-40-diger-ayarlar.png` içindedir; şema, başarı ve banka
ekstresi ayrıştırma yeteneği doğrulanmadı.

### P2-G04 kare rolleri

| Kare | Görünen içerik / rol |
|---|---|
| `f7-29-arama.png` | Arama açılmadan önceki işlem listesi; arşiv |
| `f7-30-arama2.png` | Ada araması ve 12.000 toplam; ana |
| `f7-31-filtre.png` | Filtre yüzeyi; ana |
| `f7-32-menu.png` | Dosya adına rağmen filtre tekrarı; arşiv |
| `f7-33-menu2.png` | İki Hesaplar başlıklı gezinme çekmecesi; ana |
| `f7-34-kategoriler.png` | Dosya adına rağmen D3 sonrası Hesaplar; ana |
| `f7-35-nakit-akim-ayari.png` | Nakit akışı hesap seçimi; ana |
| `f7-36-kategoriler2.png` | Kategori hiyerarşisi; ana |
| `f7-37-etiketler.png` | Etiket listesi ve arama; ana |
| `f7-38-cop-kutusu.png` | Açıklamasız boş Çöp Kutusu; ana |
| `f7-39-ayarlar.png` | Ayar merkezi ve içerik arasındaki reklam; ana |
| `f7-40-diger-ayarlar.png` | Dosya adına rağmen Veri Yönetimi; ana |
| `f7-41-diger-ayarlar2.png` | Diğer Ayarlar alt hedefleri; ana |
| `f7-42-hesap-ayarlari.png` | Dosya adına rağmen Kategori Ayarları; ana |
| `f7-43-hesap-ayarlari2.png` | Hesap Ayarları; ana |
| `f7-44-gelismis-ayarlar.png` | Kur, hesap makinesi ve form tercihleri; ana |
| `f7-45-gelismis-scroll.png` | Liste sunumu tercihleri; kanıt eki |
| `f7-46-check-nav.png` | Boş Çöp Kutusu tekrarı; arşiv |
| `f7-47-takvim.png` | Günlük gelir/gider/net kırılımı; ana |
| `f7-48-takvim-ayarlari.png` | Dosya adına rağmen takvim toplamı; kanıt eki |
| `f7-49-seyahat-modu.png` | Çekmecede kapalı Seyahat Modu; kanıt eki |
| `f7-50-seyahat-toggle.png` | Dosya adına rağmen etiket seçim sayfası; arşiv |
| `f7-51-check.png` | Seyahat anahtarı kapalı çekmece tekrarı; arşiv |
| `f7-52-nav-check.png` | Dashboard'a dönüş ve reklam; kanıt eki |
| `f7-53-export.png` | Dosya adına rağmen filtre tekrarı; arşiv |
| `f7-54-print.png` | Üç çıktı seçeneği; ana |

QuickSync, Arkadaşa Öner, Geri Bildirim Gönder ve seyahat anahtarı yalnız
çekmecede görüldü; davranışları test edilmedi. Bu paket yeni canlı koşum veya
erişilebilirlik ölçümü değildir.

## BusinessFinance için kararlar

P2-G02/G03/G04'te düzeltilen satırlar kanıta dayalı aday yorumlardır; ürün
kapsamı veya Belge 3 için kabul kararı değildir.

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| İşlem satırında işlem sonrası hesap bakiyesi | Uyarlayarak al | Kullanıcı hareketin etkisini anında görüyor; birleşik feed'de isteğe bağlı ikincil bilgi olabilir | Aktivite feed'i |
| Gün başlığında o günün neti | Uyarlayarak al | Gelir/gider/transfer yoğunluğunu gün düzeyinde özetliyor; transferlerin sıfır etkisi görünür oluyor | Aktivite feed'i |
| Açılış bakiyesi + açılış tarihi aynı hesap formunda | Uyarlayarak al | Alanlar kart formunda birlikte görünür; tarihsel bakiyeye etkisi açık anlatılmalı. Önceki aya giriş kısıtının nedeni henüz doğrulanmadı | Hesap oluşturma |
| Dinamik kart alanı (`Taksit şartlarını seçin`) | Uyarlayarak al | Yalnız seçilen hesap türüne ilişkin alanları açmak form yoğunluğunu azaltır | İşlem/kart formu |
| Kullanıcı tarafından düzenlenen dashboard kartları | Uyarlayarak al | Farklı kullanıcı önceliklerine uyar; ancak ilk sürümde az sayıda anlamlı ön ayar yeterli | Ana ekran |
| Tek ekrana bütün ayrıntıları yığmak | Alma | Güçlü ama esnaf için fazla yoğun; temel alanlar üstte, vergi/etiket/not gibi ayrıntılar kontrollü açılmalı | İşlem formu |
| Çok geniş hesap türü ve hazır örnek hesap listesi | Alma | Kredi/ipotek/yatırım kapsamı çekirdek işi gömer; BusinessFinance yalnız kendi domain türlerini göstermeli | Hesap listesi ve oluşturma |
| Sıfır tutarı kabul etmek | Alma | Anlamsız finansal hareket oluşturuyor. BusinessFinance'te normal para hareketlerinde sıfır tutar istemcide ve sunucuda zaten reddediliyor. *(P1-B02-B04 düzeltmesi, 14 Eyl 2026: önceki gerekçe bunu yapılması gereken bir iş gibi anlatıyordu; sıfır reddi kasa sayımı gibi farklı kavramlara genellenmez.)* | Tüm para formları |
| Düzenleme ve silme onayı | Henüz karar verme | Sıfır tutarlı detay, silme onayı ve boş Çöp Kutusu görünür; silinen kaydın hedefi, kalıcılığı ve geri yüklenmesi kanıtlanmadı. B10 kapsamı sınırlı; BC-Q03 için karar ertelendi. Yeni bir öneri bu davranışa dayanacaksa soru yeniden değerlendirilir | Kayıt detayı |
| Transferi iki bağlı satır ve iki running balance ile gösterme | Uyarlayarak al | Kaynak/hedef etkisi güçlü; tek olay oldukları görsel bağla daha açık tutulmalı | Transfer detayı ve feed |
| İşletme/şahsi boyutunun olmaması | Alma | ADR 0013'ün temel ihtiyacını karşılamıyor | İşlem formu ve rapor filtresi |
| Otomatik kapalı tekrarlayanın bekleyen listeden elle kaydedilmesi | Uyarlayarak al | Kullanıcı gerçekleşme anını seçebiliyor; otomatik seçenek açıkken sonuç bilinmiyor. Bu koşuma ait arayüz örneğidir, modellerin eşdeğerliği değildir | Planlanan görünüm / tekrarlayan plan |
| Geçmiş vadeyi gecikmeli etiketle gösterme | Uyarlayarak al | İncelenen kapalı otomasyon koşumunda 31 gün gecikmeli etiketi görünür; kullanıcı bekleyen kalemi ayırt eder. Diğer modlar/ürünler hakkında hüküm vermez | Planlanan görünüm |
| Gerçekleştirmede Bugün / planlanan tarih seçimi | Uyarlayarak al | Tarihin hangi dönemi etkileyeceği kullanıcıya görünür; geçmiş tarih kolu doğrulandı, Bugün kolunun sonucu açık | Hatırlatıcı onayı |
| 6.000 tutarı altı 1.000 taksite ayıran kurulum | Uyarlayarak al | Toplam, ay sayısı ve plan özeti görülebiliyor; örnek ilk ödemesi geçmişte olan 6 aylık/sıfır oranlı koşumdur. Diğer sayılar ve zamanlama davranışı açık | Taksit planı kurulumu |
| Geçmiş ilk ödemeli kurulumda ilk taksidin işlem listesine düşmesi | Henüz karar verme | Görünen sonuç doğrulandı; tarih etkisi ile taksit kuralı ayrılmadığı için her kurulumda otomatik yazım kararı çıkarılmaz | Taksit planı kurulumu |
| Taksit oranı alanı | Henüz karar verme | Ekranda 0,00 var; yüzde işareti ve sıfır dışı hesaplama etkisi doğrulanmadı. Alanın anlamı araştırılmadan vergi alanlarıyla kıyaslanmaz | Taksit planı |
| Tekrar ve taksitlerin tarih sıralı ortak listesi | Uyarlayarak al | Ayrı vadeleri tek yerde gösteriyor; ekran benzerliği domain veya fiziksel şema eşdeğerliği kanıtı değildir | Planlanan görünüm |
| "Yinelenmek" (kaydı çoğaltma) transfer detayında | Henüz karar verme | Benzer transferi hızlı tekrar; küçük kolaylık | Kayıt detayı |
| İşlem formunda ataç ikonu | Henüz karar verme | Ek akışı açılamadığı için OCR varlığı/yokluğu ve son kayıt davranışı doğrulanmadı | İşlem eki |
| Kredi kartında Hesap Kesim Günü alanı | Henüz karar verme | Alan var; Bitiş tarihi anlamı ve ekstreye etkisi açık. Alanı görmek hesaplama davranışını doğrulamaz | Kart hesabı |
| Bölmek formunda alt satır tutar/kategori/hesap/not alanları | Henüz karar verme | Kalem giriş yüzeyi görülüyor; farklı hesaplara dolu çoklu satır kaydı ve rapor etkisi denenmedi | Kayıt detayı |
| Taslak çıkış uyarısı koşum notu | Henüz karar verme | Bu pakette diyalog karesi yok; diğer ürünlerle kıyas veya yeni BF ihtiyacı çıkarmadan önce doğrulanmalı | Tüm formlar |
| Cari hesap listesi ve gelir/transfer sonucu | Henüz karar verme | Cari 0→12.000→7.000 doğrulandı; özel cari alanları ve belirli faturaya tahsis doğrulanmadı. BF tahsilatı da belirli borçlandırmaya bağlı değildir (P1-B01) | Cari hesap |
| Tek seferlik kira, abonelik ve taksidin aynı listede gösterilmesi | Uyarlayarak al | Kullanıcı farklı vadeleri birlikte görüyor; tek liste ortak domain modelini kanıtlamaz | Planlanan görünüm |
| Cari hesaba gelir ve oradan bankaya transfer (D2/D3) | Henüz karar verme | Bu koşumda tutarlar ve gün toplamının korunması kanıtlı. Genel Durum alanı var; fatura durumları ve sistemsel bağ açılmadı. Ürün genelinde bağ yokluğu veya sıfır öğrenme maliyeti iddiası kurulamaz; BF karşı taraf bağı da fatura bazlı tahsis değildir (P1-B01) | Cari hesap / tahsilat akışı |
| Nakit akışı raporuna dahil edilecek hesapların manuel seçilmesi (hesap başına on/off) | Alma | Kayıtlı ayarda kullanılan iki hesap kapalı, sıfır hesaplar açıktır; bunun fabrika varsayılanı veya sıfır raporun kesin nedeni olduğu söylenmez. BF'de rapor kapsamı hesap türünden görünür biçimde türemeli | Rapor/ayarlar |
| Varsayılan gider/gelir kategorisi ve varsayılan hesap ayarı | Uyarlayarak al | Form sürtünmesini azaltan makul bir varsayılan; bizde de kategori/hesap seçimi için benzer bir varsayılan düşünülebilir | İşlem formu varsayılanları |
| Para birimi ve döviz kuru hatırlama ayarları | Alma | Ayar yüzeyi dövizli işlemi anıyor; kur hesabı denenmedi. BusinessFinance yalnız TRY işletiyor, bu yetenek ürün kapsamı dışında | — |
| Kapsamlı filtre yüzeyi (metin, tutar/tarih, tür, kategori, hesap, etiket, durum) | Uyarlayarak al | Alanlar doğrulandı; çoklu seçim ve kayıtlı profil davranışı doğrulanmadı. Aktivite feed'i için ölçülü alt küme adaydır | Aktivite feed'i / arama |
| Üç çıktı seçeneği (PDF/Yazıcı, Excel, HTML) | Uyarlayarak al | Seçenekler görünür; dosya üretimi ve Premium son adımı doğrulanmadı. Bizde CSV var, PDF/HTML muhasebeci paketiyle ayrıca değerlendirilebilir | Dışa aktarma |
| Takvim sekmesinde günlük gelir/gider/net drill-down | Uyarlayarak al | Günlük detay görünümü aktivite feed'inde zaten var; ayrı bir takvim görünümü küçük bir katma değer | Rapor/takvim |

## Kanıt ve güven düzeyi

- Manuel gözlem — mevcut görsel incelemesi: P2-G01–P2-G04 toplam
  E0016–E0105, 90 kare. İnceleme tarihleri 14–15 Eylül 2026;
  yeni canlı koşum yapılmadı.
- Faz 3 B2: 6 aylık, 0,00 oranlı, geçmiş ilk ödemeli plan; 1/6 gerçek
  işlem ve beş bekleyen taksit. İlk taksidin oluşma nedeni ayrıştırılmadı.
- Faz 3 B1: otomatik kutu kapalıyken bekleyenler ve geçmiş tarihli elle
  kaydetme yolu; onay sonrası gider +600. Açık otomasyon kolu bilinmiyor.
- Faz 3 ek: bölme formu, tek seferlik kira listesi, kart alanları, cari
  hesap satırı ve 500 kısmi transfer kanıtlı. Taslak uyarısı ve fiş ek
  akışı yalnız koşum notu / doğrulanamadı.
- Faz 7 başlangıcı ve D1/D2/D3: E0051–E0079 doğrulandı. Kira gideri,
  cari gelir ve bankaya transfer zinciri kanıtlı; fatura tahsisi doğrulanmadı.
- Faz 7 arama/filtre/ayar/takvim/çıktı yüzeyleri: E0080–E0105 doğrulandı;
  açılmayan alt seçenekler, anahtar sonuçları ve gerçek çıktı doğrulanmadı.
- Resmî kaynak: bu tur dış kaynak taraması yok.
- Çıkarım: farklı vadeleri ortak listede toplamak ve gerçekleşme tarihini
  görünür seçtirmek arayüz adayıdır. BF ile birebir/en yakın model hükmü
  çıkarılmaz; cari bağ, otomasyon ve ekstre davranışı eksik doğrulanmıştır.
- Doğrulanamadı: B1 otomatik açık kol, geleceğe kurulan ilk taksidin ve
  kalan taksitlerin gerçekleşmesi, silme/geri yükleme, ekstre dönem
  davranışı, cari oluşturma alanları, taslak uyarısı, fiş/OCR akışı,
  gerçek cihazda etkileşim, Premium dışa aktarmanın son adımı, Seyahat Modu.

## Tek cümlelik sonuç

Bluecoins'in 90 karesinin tamamı; taksit, farklı vade, transfer, cari hareket,
arama, filtre, ayar ve günlük takvim yüzeylerini doğruluyor; otomasyon, silme/
geri yükleme, fatura tahsisi, ekstre ve gerçek çıktı davranışı açık kalıyor.

## 15 Eylül kullanıcı kontrolü — BC-U01 (B10)

Kullanıcı, Cüzdan'a eklediği 10 TL “Silme denemesi” giderini silince satırın ve bakiye etkisinin kalktığını; Çöp Kutusu'ndan geri yükleyince −10 TL etkisinin dönmesine rağmen satırın görünmediğini bildirdi. Sonra eklediği 5 TL gider görünür, silinmemiştir. Geçiş adımları kullanıcı beyanıdır.

`işlemler.png`: 15 Eylül gün toplamı −15 TL; görünen tek satır “gider” −5 TL, Cüzdan satır bakiyesi −15 TL; ardından 11 Eylül başlığı gelir. `ögeler özeti.png`: Eylül raporunda “Silme denemesi” −10 TL ve “gider” −5 TL ayrı görünür. Rapor ve toplam geri yüklenen tutarı içerirken yakalanan listede satır görünmüyor. Yeniden açma/arama/filtre kontrolü bekliyor; kalıcı kayıp veya kök neden sonucu çıkarılmaz. Ortak Cüzdan'ın 4.150 TL tarihsel satırı Cüzdan hesabıyla karıştırılmaz; kullanıcının 4.140 TL Nakit beyanı bu karelerde görünmez.

Bu ek kayıt eski silme/geri yükleme bilinmiyor ifadelerini yalnız yukarıdaki kullanıcı kontrolü kadar günceller. Otomatik tekrar ve diğer denenmemiş dallar açık sınır olarak korunur. Ayrıntı: BULGU-DOGRULAMA-KAYDI.md BC-U01/HD-U01.

15 Eylül takip sonucu (kullanıcı beyanı, yeni görsel yok): Kullanıcı Bluecoins uygulamasını kapatıp yeniden açınca “Silme denemesi” satırının İşlemler listesine geldiğini bildirdi. BC-U01 yeniden açma kontrolü tamamlandı; bu olayda liste görünümü yeniden açılışla düzeldi. Önceki iki kare tutarın raporda/toplamda bulunduğunu gösterir. Liste yenilemesiyle ilgili geçici bir sorun olasılığı desteklenir; teknik kök neden veya bütün sürümlere genelleme yapılmaz. Yeni arama/filtre kontrolü gerekmiyor. B10 otomasyon ve diğer denenmemiş dallar nedeniyle genel olarak kapsamı sınırlı kalır.

## 23 Eylül 2026 — ek eksik koşumu (22 Eylül), kullanıcı kontrolü ve kare doğrulaması

Koşum listesi `raporlar/eksik-kosum-ortak-listesi.md` (BC-01…BC-19). Aşağıdakiler
kareler tek tek açılarak doğrulanan ve Belge 2'ye taşınan kısımdır.

**Test verisi (silinmedi):** ₺110 bölünmüş gider (50 Grocery + 60 Others), ₺56 `İş`
etiketli gider, ₺1 gelir, ₺500 Cüzdan → Ana Hesap aktarımı. Eylül gideri 11.015 → 11.181;
Ana Hesap 34.700 → 35.200; Cüzdan −180 → −680.

| Soru | Gözlem | Kanıt |
|---|---|---|
| Bölmek (BC-08) | Her parça kendi tutarı, kategorisi, hesabı, notu, **Durum**'u ve **Etiket**'iyle; üstte Toplam tutar. 50 + 60 = 110 yazıldı; ay gideri 11.015 → 11.125; listede **tek satır**, "2 Kategoriler" | `f7-58-bolmek-satir-yapisi.png` (E0443); liste `f7-65-…` (E0490; 24 Eylül düzeltmesi) |
| Etiket (BC-04) | Seçicide Doğum günü · Film · **İş** · **Kişisel** · Tatil; çoklu seçim. Etiketli kayıt listede çiple görünüyor. Bu etiketlerin ürünle mi geldiği, daha önce mi eklendiği görülmedi (E0088'in 15 Eylül açıklaması da kaynağı açık bırakıyor) | `f7-67-…` (E0426), E0444 |
| Durum alanı (BC-03) | Dört değer: Yok · Kontrol · Mutabık · İptal edildi. Fatura bağlama alanı yok; ek yalnız formun üstündeki ataç | `f7-66-durum-alani-dort-deger.png` (E0448) |
| Transfer ücreti (BC-10) | Ücret açılınca ayrı blok: kendi tutarı, kendi hesabı (Cüzdan), kendi kategorisi (Diğer/Others). Ücret aktarımın parçası değil, bağlı ayrı bir gider satırı | `f7-75-…` (E0435) |
| Nakit Akım Ayarı (BC-09) | Ekranın cümlesi: "Nakit akışı hesaplarken kullanılacak nakit hesapları seçiniz." Hesap başına anahtar (Ana Hesap kapalı · Birikimler · Çek · Cüzdan açık · Ortak Cuzdan kapalı). Anahtarı değiştirip raporu karşılaştırmak **ölçülmedi** | `f7-63-…` (E0446) |
| Cari hesapta vade (BC-11) | Cari hesap formu: Not · Başlangıç bakiyesi · Son Bakiye · Açılış tarihi · Hesap Tipi · iki anahtar · İşlem Listesi. **Vade alanı yok** | `f7-77-…` (E0449) |
| Bütçe Özeti (BC-18) | Others · Güncel ₺11.015 / Bütçe ₺0; İşlem tipi Gider · Bu Ay · Kategoriye Göre. Hiçbir kategoriye bütçe kurulmamış, tamamı Others | `f7-56-…` (E0442) |
| İçe / dışa aktarma (BC-14, BC-16) | Veri Yönetimi: içe aktarma yalnız **Excel (.csv)** ve **QIF**; yedek "Telefon hafızası". Yazıcı simgesi: PDF veya Yazıcıya gönder · Excel (.csv) · HTML. Dosya üretilmedi | `f7-71-…` (E0445), `f7-73-…` (E0444) |
| İki blok adı (BC-12) | Gelir-gider bloğu **Net Kazançlar**, varlık bloğu **Net Kazanç** — tek harf farkı. Kare üst şeritle örtülü, basılmaz | E0459; basılan kanıt E0053 |
| Seyahat modu (BC-17) | Anahtara dokununca **etiket seçici** açılıyor; seçim yapılmadan iptal edildi. Seçilen etiketin ne yaptığı görülmedi | `f7-78-…` (E0454) |
| Çöp kutusu (BC-15) | **Kullanıcı kontrolü (23 Eyl):** silinen kayıt çöp kutusuna düşüyor, oradan geri yükleniyor; geri yüklenen kayıt işlemler listesinde hemen görünmedi, uygulama kapatılıp açılınca geldi. **15 Eylül BC-U01 ile aynı sonuç** (E0401, E0402) — ikinci bağımsız gözlem | kullanıcı kontrolü; E0401, E0402 |
| Taksit ve tekrarların gerçekleşmesi (BC-X2) | **Kullanıcı kontrolü (23 Eyl):** yaklaşan tekrarlayan işlemler ve taksit ödemeleri Hatırlatıcılar'da duruyor, **kendiliğinden işlemlere girmiyor**. Hatırlatıcıya dokununca altta Kaydet ve Düzenle; Kaydet işlemi gerçekleştiriyor ve kayıt işlemler listesine geçiyor. İleri tarihli bir taksitte Kaydet'e basınca ödeme tarihi soruluyor: **bugün** veya **taksitin tarihi** (E0043 ile aynı soru). Planlı işlem sayfasındaki otomatik kolun açık olduğu bir plan izlenmedi (BC-19 açık) | kullanıcı kontrolü; E0043, E0047 |
| Hesap listesinin bölümleri | **Kullanıcı kontrolü (23 Eyl) + kare:** Hesaplar ekranında iki üst bölüm var: **VARLIKLAR** (Banka, Nakit) ve **CARİ HESAP** (Cari hesap, Kredi Kartı, İpotekler). Ada Reklam cari, kartla aynı CARİ HESAP bölümünde, yani borç tarafında duruyor; VARLIKLAR toplamına girmiyor. Net Kazanç bloğundaki "Cari hesap" satırı bu bölümün toplamı: E0053'teki −500 = cari 0 + kart −500; E0459'daki +5.500 = cari 7.000 + kart −1.500 (aritmetik). "Cari hesap" adı hem bölümün hem içindeki grubun adı | E0049, E0055, E0053, E0459; kullanıcı kontrolü |

## 24 Eylül 2026 — Belge 1 düzeltme turu: kare doğrulaması

Belge 1'e dayanak yapılmadan önce kareler tek tek açıldı. Aşağıdakiler ya ortak listedeki koşum
özetini kareye göre düzeltir ya da Belge 1'in kullanmadığı bir karede görülen arayüz ayrıntısıdır.
Yeni koşum yapılmadı.

| Soru | Gözlem | Kanıt |
|---|---|---|
| Bölünmüş kaydın listesi (düzeltme) | 23 Eylül satırı listeyi "`f7-73` (E0444)" diye gösteriyordu; E0444 çıktı seçimi karesi. Liste karesi `f7-65`: tek satır "Diğer · 2 Kategoriler" −110 | E0490 |
| CSV/HTML çıktısı (BC-16) | Seçenekler ve ürünün cümlesi kareli: "Tüm raporları PDF, Excel **(cvs)** veya Html'e aktarmak için soldaki yazıcı simgesinin olduğu her yerde bulunur." (karedeki yazım). **Dosya üretilmedi**; ortak listedeki "kapandı" Belge 1'in eksiği (dosyanın üretimi) için geçerli değil | E0493, E0444 |
| Hesaplar sekmesi (BC-06) | Kart panosu: Günlük Özet, test reklamı, Bütçe Özeti. Açılışın soğuk olduğu karede değil, koşum kaydında | E0486 |
| Çekmecedeki iki Hesaplar (BC-07) | İlki (ızgara simgesi) kart panosu; ikincisi (banka simgesi) hesap listesi: Nakit Akım Ayarı, VARLIKLAR (Banka, Nakit), CARI HESAP (Cari hesap 7.000, Kredi Kartı −1.500) | E0488, E0489 |
| Bütçe Kur | Kategoriler ekranının üstünde **Bütçe Kur** düğmesi; kurulum formu açılmadı. Grup adları Türkçe (Araba, Eve Ait, Eğlence), alt kategoriler İngilizce | E0087 |
| İki blok aynı karede | Net Kazançlar (Gelir, Gider, Net Kazançlar) ve Net Kazanç (Varlıklar, Cari hesap, Net Kazanç) alt alta, ikisi de okunaklı | E0053 |
