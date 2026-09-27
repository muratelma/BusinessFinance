# Devir: Belge 1 düzeltme turu, Aşama 3 (uygulama) tamamlandı

> **Durum (24 Eylül, ikinci oturum sonu):** Kalanlar 1–8'in hepsi yapıldı. `belge1/tam/belge1.pdf`
> 98 sayfa, 12 bölüm, 154 basılı kare; Belge 2 yedi göndermeyle yeniden üretildi (105 sayfa).
> Sıradaki: kullanıcı incelemesi. Açık kalanlar dosyanın sonunda.

24 Eylül 2026. Kullanıcı tam yetki verdi ("Belge 1'in amacından çıkmadan … hatasız ve
profesyonel"). Açık kararlar K1–K11 planın önerileriyle uygulanıyor. Commit yok.
Yedek: `_yedek/2026-09-24-belge1-asama3-oncesi/` (belge1/, envanter, gözlem formları, ortak liste).

## Bitenler

1. **Envanter:** 34 kare eklendi, E0463–E0496 (`KANIT-ENVANTERI.md` son bölüm; her kare açılarak
   yazıldı). `kanit-dizini.json` yeniden üretildi (459 kare).
2. **Karartma:** `kalip.py` `KARARTMA_ORTAK`'a E0494 (GB işlem listesi) ve E0495 (GB ayarlar üstü).
3. **Gözlem formları (G2):** WL, BC, MM, GB, HD formlarına "24 Eylül 2026 — Belge 1 düzeltme turu:
   kare doğrulaması" bölümü; `bluecoins.md`'deki yanlış kimlik (E0444 → E0490) düzeltildi.
4. **Ortak liste:** WL-19, BC-16, WL-14, MM-01, MM-02, WL-08 satırlarına 24 Eylül doğrulama notu.
5. **Yeni üretim katmanı `belge1/ortak/b1.py`** (ortak motora dokunmadan):
   - kesme: telefon karesinde üst 120 px (durum çubuğu); E0376, E0026, E0171 için 88–90 px
     (`OZEL_KESME`). İşaret koordinatları özgün kare uzayında kalır, otomatik kaydırılır.
   - kapılar: kenara taşan işaret · G4 ("Çıkarılmayan sonuç", "matrisi", "koşum" — rozet hariç)
     · G4b · G7 · G8 · A1 (tarih). İhlalde üretim durur.
   - markdown: tarih yazılmaz, şekilsiz bölümde şekil dizini yazılmaz.
   - bütün bölüm `uret.py`'leri `b1.calistir` çağırıyor.
6. **Bölüm 3–13 içerik düzeltmeleri** (Aşama 1 tabloları + ek): hepsi uygulandı, her bölüm
   kapılardan geçti ve önizleme gözle incelendi. Bütün `cikarilmayan`/`CIKARILMAYAN` kalktı;
   gerekenler Belge 2 alt bölüm göndermesi ya da tam cümle oldu. Her bölüme `EK_KANITLAR` eklendi.
   Yeni basılan kareler: E0109 (3.1), E0494 (4.2 kırpıntı), E0448 (5.2), E0487 (5.3), E0463 (5.4),
   E0491 (7.2), E0469 (8.4, E0274'ün yerine), E0234 (10.1b), E0053 (10.2), E0476 (10.4, E0377'nin
   yerine), E0427 (11.1, E0175'in yerine), E0471 (11.2). 11.2 iki sayfaya bölündü (KolayBi ayrı).
   İşaret düzeltmeleri: not 13 (6.6), 8.6b Goodbudget işaret 1.
7. **Yapı (karar 6, K8) — başladı:**
   - `bolum-01-okuma-kilavuzu`, `bolum-02-urun-kimligi` → `belge1/_arsiv/`
   - üç deneme klasörü → `belge1/_arsiv/deneme-bolum-03-*`
   - 03…13 klasörleri 02…12 olarak yeniden adlandırıldı; `NO` ve bütün "N.M", "→ N" göndermeleri
     betikle kaydırıldı (`belge1/ortak/araclar/kaydir.py`; "5 MB" ve Belge 2 göndermeleri korundu).
     Docstring ve yorum satırları da güncellendi. Eski bölüm PDF/MD'leri silindi.

## Uygulama denetimi (24 Eylül, ikinci oturum)

Eski Bölüm 7–13'ün (yeni 6–12) uygulaması plana ve hazırlık tablolarına karşı satır satır denetlendi.
Her sapma, önceki oturumun dökümü (`8944cc3a…jsonl`), envanter açıklaması ve gerekirse karenin kendisiyle
"uygulamada bulunan düzeltme mi, atlama mı" diye ayrıldı.

**Uygulamada bulunan düzeltmeler (planın yerine geçer, dokunulmaz):**
- 9.1 Money Manager kategori ayrıntısı: plan "çubuk", karede **çizgi** grafik (E0466).
- 6.1 Bluecoins örneği: plan "Dining Out", metin "Others"; "Others" E0071'de, "Fuel, Grocery" E0087'de.
  Dayanak ikisini birlikte taşıyor.
- **6.2 kare seçimi (karar kaydı):** plan §6 #7 E0426'yı (etiket seçici) öngörüyordu. Basılan kare
  **E0491** (`f7-68`, listede İş çipi): "bir kayda İş etiketi verildi ve listede göründü" iddiasının
  karesi. E0426 dayanakta kalır; seçicinin beş değeri metne yazıldı.

**Biçim farkı (içerik korunmuş):** 9.4 Goodbudget iki satır (tek satır tek rozet taşır); 8.3 ve
9.4'te Bluecoins bilgisi işaret yerine notta, Durum alanı 4.2'de basılı. **Plana ek:** 12.3 Money
Manager "Tekrarın uygulanma ayarı" satırı (12.2'deki satırla aynı ayarı anar); 10.2'de kaynak
ürünlere "→ Belge 2 Bölüm 9".

**Atlanmış ya da hatalıydı; bu oturumda düzeltildi:**
- Kaymamış dört gönderme: 6.4 "→ 9." → 8 · 7.4 "→ 8.3." → 7.3 · 8.1 "→ 9.5." → 8.5 · 8.1 "→ 6." → 5.
  Neden: `kaydir.py` cümle sonu noktasını reddediyordu; desen düzeltildi (sürüm numarası, "5 MB",
  "1.400" ve Belge 2 göndermeleri korunuyor, sınandı).
- Bölüm 7 `KAPSAM` Money Manager `["canli", "kosum"]` → `["canli"]` (iki koşum satırı canlıya dönmüştü).
- Wallet `f7-79` ve `f7-101` hiç açılmamıştı. Açıldı, **E0497** ve **E0498** olarak envantere girdi
  (dizin 461 kare). 6.2'ye Wallet etiket formu (ad, renk, Auto assign to new records), 7.3'e
  sıralama (vadeye ya da ada göre) yazıldı.
- 7.3'e "süresi geçen hatırlatıcı kendiliğinden kayda dönmüyor (→ 7.2)" notu; 7.5'te "(7.1)" → "(→ 7.1)".
- Bölüm 6, 7 ve 8 yeni numaralarla üretildi; kapılar temiz, sayfa sayısı değişmedi, 6.2 ve 7.3
  önizlemede taşmıyor.

**Kapıların ilk taraması** (plan §8 adım 4; önceki oturum çalıştırmış, kaydetmemişti): 17 Eylül
bölüm PDF'lerinde **72** "Çıkarılmayan sonuç", **24** "koşum", **21** tarih, **2** "matrisi". Ayrıca 8
"BusinessFinance" geçişi (sınır cümlesinde serbest, ihlal değil). Eski bölüm numaralarıyla:
Çıkarılmayan 1·0·8·5·10·8·6·9·6·7·6·2·4 (Bölüm 1–13).

**Bilinen ve bırakılanlar:** Wallet `KAPSAM`'ındaki "koşum" (Bölüm 3, 6, 7, 8) turdan önce de
gövdede karşılıksızdı; G8 bunu yakalamıyor, çünkü yalnız ürün adına bakıyor. Balance Trend (7.4),
Yiyecek ₺1.400 (7.6) ve Include account transfers (10.1) metne girmedi. Planın sayı kuralıyla
uyumlu; bırakıldı.

**Kalanlar 4 için uyarı:** 39 kare metinde anılıyor ama ne basılı ne de bir `EK_KANITLAR`'da
(örnek E0037, E0126, E0238, E0317). Turdan önce de böyleydi. Kanıt ekinin "anılan ama basılmayan"
listesi `EK_KANITLAR`'dan değil, `icerik.py` metinlerindeki bütün E kimliklerinden üretilmeli
(basılanlar ve gözlem formları E0005–E0014 hariç).

## Kalanlar (sırayla) — hepsi yapıldı, ikinci oturum

Sonuçlar: 1 · `bolum-01-giris/` 6 sayfa (Goodbudget sayfası `b1.py` `yuva` ayarıyla) · 2 · eski
1.x/2.x göndermesi kalmadı · 3 · 12 bölüm yeniden üretildi, kapılar temiz · 4 · `tam/uret.py`
(`b1.Bolum`, kapakta tarih yok, içindekiler 6 + 6, anılanlar metinden, tam metin kapıları) · 5 ve 7 ·
Belge 2'de yalnız yedi satır değişti · 6 · KolayBi 17 / 31 · 8 · README, DURUM, plan §9 ve §12,
hazırlık aşama tablosu. Aşağıdaki liste kayıt için duruyor.

1. **Yeni `bolum-01-giris/`** (`icerik.py` + `uret.py`): açılış sayfası = giriş (K11; ana soru,
   amaç, test verisi, "görülmedi ≠ yok" ve nicelik cümlesi (K1), neyi ölçmez, sınır cümlesi; sağda
   `KAPSAM` kutusu). 1.1 canlı ürün kartları (MM+BC | WL+HD | GB), 1.2 kaynak ürünler (KolayBi+Paraşüt
   | Logo+QB); 1.2 girişinde "akış modeli Belge 2 Bölüm 9" (K10) ve Paraşüt kartında tanıtım karesi
   cümlesi (K2). Kart metinleri: hazırlık dosyası "Bölüm 1–2" tablosu. Kare sayıları envanterden
   (MM 60, BC 111, WL 141, HD 49, GB 42, KB 39, PS 9, LG 6, QB 4 = 461; E0497–E0498 eklendi). Tarih yok.
   **Numaralama notu:** karar 6'daki 1.1 giriş = bölüm açılış sayfası; kartlar 1.1 ve 1.2 oldu.
2. **Kalan ESKİ 1.x/2.x göndermeleri:** yeni 1.1/1.2'ye bağlanmalı; `grep "Bölüm 1\|Bölüm 2'"`
   ile kontrol (bulunmadı ama yeni bölüm yazılınca tekrar bakılır).
3. **Bölüm 2–12'yi yeniden üret** (numara değişti): her klasörde `uret.py`; kapılar temiz olmalı.
4. **`tam/uret.py`:** `BOLUMLER` yeni klasör adları; kapaktan tarih kalkar (A1); içindekiler sütun
   bölünmesi (`i < 7` → 12 bölüm için ayar); alt bant örneği "(→ 8.4)" → "(→ 7.4)"; kanıt eki
   `BASILMAYAN` listesi eskidi (WL-U02-A/U03-A artık E0399/E0400) → basılmayan anılanları
   `EK_KANITLAR`'dan otomatik üret; Belge 2'deki gibi kapak/içindekiler/ek kapıları (G4, G7 kapak
   muaf, tarih). Arka planda çalıştır (>2 dk).
5. **Kesmenin Belge 2'ye etkisi yok:** kesme yalnız `b1.py`'de; ortak motor değişmedi (kalip.py'de
   yalnız `KARARTMA_ORTAK`'a iki kimlik eklendi — Belge 2 bu karelere basmıyor). Yine de Belge 2
   `tam/uret.py` bir kez geçici klasöre üretilip sayfa sayısı ve kapılar kontrol edilmeli.
6. **Birleşik belge sonrası:** KolayBi basılan destek görseli sayısı sayılıp 1.2 kartına yazılır
   (dış göz notu 1); tam belge sayfa sayfa gözle incelenir.
7. **Belge 2'ye dönük iş (karar 7):** Belge 2 kapsam kutularındaki yedi "→ Belge 1 §N" yeni
   numaralara (Belge 1 §4, §5, §5, §8, §7, §6, §10 — yani eski −1); ilgili Belge 2 bölümleri ve
   `tam/` yeniden üretilir, yalnız o sayfalara bakılır. Belge 1'deki "→ Belge 2 X.Y"
   göndermeleri Belge 2'nin bugünkü numaralarıdır; kontrol edildi.
8. **Belgeler:** `belge1/README.md` (klasör düzeni, `_arsiv`, b1.py, 12 bölüm, sayılar),
   `DURUM.md`, `belge1-duzeltme-plani.md` §9'a "uygulandı: K1–K11 önerileriyle" ve §10 uygulama
   sonucu, `belge1-hazirlik.md` aşama tablosu.

## Dikkat

- `icerik.py` düzenlemeleri `belge1/ortak/araclar/duzenle.py` (eski/yeni çiftleri, tam bir eşleşme)
  ile yapıldı; kabuk heredoc'u Türkçe tırnakta bozuluyor.
- Yeni kare basmadan önce kareyi aç; ortak listenin özeti bu turda da beş yerde kareden fazla
  söyledi (plan §2).

## Açık kalanlar ve kullanıcı kararları (24 Eylül, üçüncü oturum)

Kullanıcı kararıyla yapıldı (bölüm dosyalarında; birleşik belge Bölüm 1 incelemesinden sonra üretilecek):

1. **Rozet denetimi ve G8.** `b1.py` G8'i genişletildi: kutu gövdedeki rozet kümesiyle birebir, Koşum
   kaydı / Görülmedi taşıyan üründe not zorunlu. 30 kutu satırı düzeldi (Bölüm 2–10): Wallet'ın eskimiş
   "koşum"u (3, 6, 7, 8), Hesap Defterim'inki (10); gövdedeki Görülmedi'nin kutuya işlenmesi (6, 7, 9, 10
   ağırlıklı); Bölüm 6'da Paraşüt/Logo "Kaynak beyanı" → Görülmedi; eksik notlar. Gövdede iki rozet
   karenin kendisiyle düzeldi: 4.3 Goodbudget Görülmedi → Canlı kare (E0133, boş formda tür Expense),
   4.5 Hesap Defterim Koşum kaydı → Canlı kare (E0178, Not önerileri ☑). Öteki "Görülmedi"ler yeni
   kanıtla (E0395–E0498, ortak liste) karşılaştırıldı; kapatan kanıt yok (erişim sınırı, ücretli paket
   ya da yüzey gerçekten yok). Bölüm 1 ve 11 kuralı zaten karşılıyordu.
2. **12.2 / 12.3:** kullanıcı sorun görmedi; iki satır kalır.
3. **Gözlem formlarının hash kaydı:** envanterin sonuna dokuz formun 24 Eylül bayt, satır ve SHA-256
   değerleri yazıldı.
4. **E0187 (KolayBi, 2026 videosu):** kullanıcı karenin 2026'da yayımlanan bir videodan olduğunu
   doğruladı. Kare açıldı: KolayBi Ofis markası, sol menüde ayrıca Fatura Ödeme, listede BiLink yerine
   Müşavirini Davet Et; iskelet destek görseliyle (E0211) aynı. 2.7'ye not ve kutu notu, 10.5'e
   muhasebeci daveti notu yazıldı. Video karesi kural gereği basılmadı.

Bölüm 2–10 yeniden üretildi: yeni G8 dahil kapılar temiz, sayfa sayıları değişmedi; değişen açılış
sayfaları ile 2.7, 4.3, 10.5 gözle incelendi.

**Kullanıcı geri bildirimi, aynı oturum:**
- **Bölüm 1 sayfa düzeni** eski Belge 1'deki gibi yapıldı (içerik değişmedi): 1.1 MM+BC · WL+HD ·
  "Canlı ve kaynakla incelenen ürünler" Goodbudget+KolayBi · 1.2 "Yalnız kaynaktan incelenen ürünler"
  Paraşüt+Logo+QuickBooks. 6 → 5 sayfa; `yuva` ayarı artık kullanılmıyor (b1'de duruyor). 1.2 girişi
  hâlâ KolayBi cümlesini taşıyor; içerik değişmesin dendiği için bırakıldı, kullanıcıya soruldu.
- **"Görülmedi" yoğunluğu:** kullanıcıya belge eksik izlenimi veriyordu. Yalnız Bölüm 6'da deneme:
  `KUTU_KURALI = "kanit"`: kutu yalnız kanıt türlerini listeler, Görülmedi yalnız ürünün bölümde hiç
  kanıtı yoksa; bölüm çapındaki bulgu kutuda ürün ürün tekrarlanmaz. 6.5'te sekiz üründe aynı olan
  "kayıt düzeyinde alan: Görülmedi" sütunu kalktı; bulgu girişte bir kez, tablo "Ayrıma en yakın araç".
  Gri rozet: kutuda 7 → 1, 6.5'te 12 → 2. Hiçbir iddia ve kanıt türü değişmedi. Onaylanırsa bütün
  bölümlere uygulanır ("kanit" kuralı b1'de varsayılan olur, öteki kutulardaki kısmi Görülmedi'ler kalkar).

**Kullanıcı geri bildirimi, ikinci tur:**
- Bölüm 6 kuralı onaylandı ve bütün belgeye uygulandı (b1 G8 varsayılanı "kanit"; bayrak kalktı).
  Ek kurallar: özet tablolarında yokluk hücresi
  düz metin (Bölüm 7: 11, Bölüm 10: 4); aynı cümleli satırlar birleşti (7.3, 7.4); kutu notları plan
  turundaki hâline döndü, zorunlu olanlar kaldı. Bölüm 11'in iki matrisi kullanıcı kararıyla Görülmedi rozetini korudu. 7.1'de E0205 alt kırpması 700 → 800 (kullanıcı: fazla kesilmişti).
- Bölüm 1: 1.2 girişi "Dört ürünün içine girilemedi. KolayBi yalnız destek sayfası görselleriyle
  görülüyor." + Belge 2 Bölüm 9 göndermesi (K10) oldu; kart başlıklarının altındaki kare satırları kalktı.
- KolayBi alt alta görseller, yalnız kullanıcının seçtiği sayfalarda 350: 8.5 (340 → 350), 7.1
  (330 → 350; alt kırpma 865 → 700, içerik kesilmedi), 4.9 (318 → 334; 350 sayfaya taşıyor, alt
  işaretler karenin dibinde olduğu için kırpılamıyor; kullanıcıya bildirildi).

**Bölüm 1 için öneri (kullanıcı incelemesinde):** KolayBi kartının "Kapsam ve bilinmeyenler"
alanı "aynı ekran tanıtım videosunda farklı düzenle görünüyor (→ 2.7)" diyor; 2026 videosuyla bu
"2026'da yayımlanan bir videoda panonun daha yeni bir sürümü görülüyor (→ 2.7)" olabilir. Bölüm 1'e
kullanıcı incelemesi sürerken dokunulmadı.

**Sonra:** birleşik belge (Bölüm 1 incelemesinden sonra), küçük işler: `inceleme-notlari.md` 20
notun durumu, ortak listede WL-07/WL-21'e E0497/E0498, plan §10 dış inceleme soruları, eski düzen
belgeleri (`belge1-bolum-plani.md`, `butun-harita.md`), commit (kullanıcı onayıyla).

**Birleşik belge üretildi (24 Eylül, üçüncü oturum sonu):** `belge1/tam/belge1.pdf` 97 sayfa (Bölüm 1
5 sayfa), 154 basılı ve 126 anılan kare, hash denetimi 280; kırık gönderme, bozuk karakter, uyarı yok.
88 bölüm sayfası tekil bölüm PDF'leriyle metin ve resim boyutu bakımından birebir aynı. Kullanıcı
başka sorun görmedi. Sıradaki: küçük işler (inceleme notlarının durumu, ortak listeye E0497/E0498,
plan §10, eski düzen belgeleri) ve kullanıcı onayıyla commit.

**Küçük işler bitti (24 Eylül, üçüncü oturum):** `belge1/inceleme-notlari.md`'ye 20 notun kapanış
tablosu (19 kapandı, 4. not kullanıcı kararıyla eski sayfa düzeni); 12. not bu sırada açık bulundu
ve kapatıldı (Şekil 4.1'de işaret 1, 2, 4 boş alana). Ortak listede WL-07 → E0497, WL-21 → E0498.
Plan §10'daki beş soruya uygulama notu. `belge1-bolum-plani.md` ve `belge1/butun-harita.md` başına
eski → yeni numara eşlemesi. Birleşik belge yeniden üretildi (97 sayfa, fark yok). **Tur kapandı;
kalan tek iş kullanıcı onayıyla commit.**
