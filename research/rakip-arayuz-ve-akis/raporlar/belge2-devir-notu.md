# Belge 2 — oturum devir notu

**22 Eylül 2026 oturumu.** Bir sonraki oturumun okuyacağı özet: bugün ne yapıldı,
ne bekliyor. Karar gerekçeleri `belge2-yeniden-kurgu.md` içindedir — bugün eklenen
iki bölüm: **§11 · K13** (Bölüm 9'un kapsamı) ve **§12 · K14** (Bölüm 10'a ne eklenmeli).

---

## Bugünün sonucu

`belge2/tam/belge2.pdf` — **105 sayfa · 127 kare · hash 127/127 · kırık atıf yok ·
uyarı yok.** Gün başında 101 sayfa / 125 kareydi.

| Bölüm | Başlangıç | Kare |
|---|---:|---:|
| 1 Gelir ve gider | 3 | 18 |
| 2 Hesaplar | 13 | 13 |
| 3 Kart | 22 | 22 |
| 4 Borç ve cari | 34 | 18 |
| 5 Zaman | 44 | 18 |
| 6 Sınıflandırma | 54 | 11 |
| 7 Rapor | 63 | 16 |
| 8 Veri | 73 | 14 |
| **9 Ön muhasebe programları** | **82** | **6** |
| 10 Ürünlerin ayrıldığı yer | 94 | 0 |
| Kanıt eki | 102 | — |

---

## 1 · Motor: `soru` sayfalarında metin basılmıyordu

`soru` sayfa türü Belge 1 için yazıldı; orada bir şeklin iddiaları `isaretler` ile,
karenin üstüne çizilen numaralı baloncuklarla taşınır. Belge 2'nin sekiz kaynak
sayfası `yanyana`'nın anahtarlarını kullandı (`satirlar`, `sag_notlar`) ve
`kalip._soru` bu ikisini **sessizce düşürüyordu**: `satirlar` yalnız iddia tablosuna
giriyor, sayfaya çizilmiyordu; `sag_notlar` hiçbir yere gitmiyordu. Toplam
**52 ifade** PDF'te yoktu.

**Yapılan:** `belge2/ortak/b2.py`'ye kendi `_soru`'su yazıldı. `kalip.py`'ye
dokunulmadı (README kuralı: kalıp sayfa türleri değiştirilmez). Sağ sütun sırası:

    şekil satırları → kırpıntılar → işaretler → notlar + sag_notlar → ürünler

- `sag_notlar` ürün bloğunun **üstünde**, notların devamı olarak aynı madde
  listesinde. Altta tek madde kalınca "kaynaktan okunan üç ürün" başlığının altında
  asılı duruyordu.
- `Şekil N.M` alt başlığı tek kareli sayfalarda da basılıyor.

**Yan etki:** G4 kapısı Bölüm 7'de "matris" kelimesini yakaladı — o satırlar hiç
basılmadığı için yasak kelime taramasına görünmüyordu. İki cümle okurun diline
çevrildi.

## 2 · Kare boyutu ve kırpma

| Ne | Eski | Yeni |
|---|---|---|
| Çift kareli altı sayfa (4.5 · 6.3 · 7.6 · 8.3 · 9.2 · 9.3) | 330 | **350** |
| Tek kareli iki sayfa (3.1b · 5.6) | 470 | **420** |
| Şekil 4.17 kırpma | …865 (66px siyah bant) | …800 |
| Şekil 6.11 kırpma | …700 (içeriği kesiyordu) | …845 |

408 denendi, kullanıcı "çok büyük" buldu; 350'de karar kılındı. **9.3 ve 9.4'ün
kırpmasına bilerek dokunulmadı** (kullanıcı: "görselin yarısını kesmek iyi
gözükmez").

## 3 · Kare taraması — kapandı, kare çıkarılmadı

134 şekil yuvası / 127 benzersiz kare tek tek tarandı. Sorulan tek soru: bu kare
sırf sayıyı doldurmak için mi eklendi?

- **Sıfır iddialı kare yok**; her kare 2–3 cümle taşıyor.
- Tekrar kullanılan 9 karenin hepsi meşru — ikinci kullanımda başka cümle taşıyor.
- `EN_AZ_KARE` kapıları fiilî sayıya eşit ya da altında; kapı içeriği sürüklemiyor.
- Tek aday **3.13** (E0409 · Taksit 5/6) idi — üç kare ay adı ve taksit sayacı
  dışında birebir aynı. **Kullanıcı kalmasına karar verdi.**
- Tarama bir **olgusal hata** buldu: 8.14 "Ayarlarda bir anahtar" diyordu; kare
  aslında **çekmece** (Ayarlar aynı listede ayrı bir kalem). Düzeltildi.

## 4 · Bölüm 9 yeniden kuruldu — K13 · seçenek B

**Eski:** "Kaydın yerine belge geçince ne değişiyor" · 9 sayfa · 4 kare
**Yeni:** "Ön muhasebe programları: belge, vergi ve müşavir" · 12 sayfa · 6 kare

Gerekçe §11'de. Özet: bölüm zaten başlığının dışına çıkmıştı (9.4 muhasebeci,
kayıt/belge sorusuyla ilgili değil) ve hiçbir bölüme girmeyen gerçek malzeme vardı —
ağırlığı **KDV**'de.

| # | Bölüm | Durum |
|---|---|---|
| 9.1 | Üç üründe aynı iskelet | + sesli fatura bloğu |
| 9.2 | Belgenin kendisi: formda ne var | aynı |
| 9.3 | Çek ve senet | eski 9.2b |
| **9.4** | **Vergi belgenin üstünde yaşıyor** | **YENİ** · E0214 + E0213 |
| **9.5** | **Belge kalem taşıyor: ürün, hizmet ve stok** | **YENİ** · karesiz |
| 9.6 | Bölüm 1–4'ün soruları | + vergi ve kalem satırları |
| 9.7 | Muhasebeci nerede duruyor | eski 9.4 |
| 9.8 | Ön muhasebe yolu | + vergi adımı (4 → 5 adım) |
| 9.9 | Neden ayrışıyorlar | + 2 çıkarım, + 2 Belge 3 sorusu |

`EN_AZ_KARE` 4 → 6. Klasör adı `bolum-09-belge` olarak **bırakıldı** (yol
değiştirmemek için). Eksik listesi 4 → **14 madde**.

**E0218 (Ürün ve Hizmetler) basılmadı** — karede sekme şeridi dışında içerik yok;
iddiası 9.5'te kaynak cümlesi olarak duruyor ve sayfada bu açıkça yazılı.

## 5 · Hedefli web taraması — yedi soru

Ayrı bir ajan resmî kaynakları taradı. **Bulgular önce gözlem formlarına işlendi**
(G2: formda yazılı olmayan çıkarım basılmaz), sonra bölüme taşındı:

- `gozlemler/kolaybi.md` → **Faz 8** bölümü
- `gozlemler/parasut.md` → **Faz 8** bölümü
- `gozlemler/logo-isbasi.md` → **Faz 8** bölümü

Kapanan başlıca sorular:

| Soru | Cevap |
|---|---|
| KolayBi KDV'yi hesaplıyor mu | **Tek kural yok.** Fatura kaleminde `vat_rate` zorunlu ve tutar alanı yok → oran belirliyor. Genel giderde `vat_type` = oran **veya** tutar |
| Paraşüt KDV ekranı | `AY / HESAPLANAN / İNDİRİLECEK / NET KDV`; Net KDV negatife düşebiliyor. Kalemde oran seçiliyor, tutarı ürün hesaplıyor |
| KolayBi depo/varyant | Ana Depo varsayılan; varyant en fazla 4 tür; **stok birimi varyant × depo çifti** (depo + raf kodu) |
| Sesli fatura | **Telefonu sallayarak**, kayıtlı hazır şablonlarla; hâlâ yayında. Onay adımı arandı, bulunamadı |
| Logo müşavir | "Müşteri adına işlemleri gerçekleştirebilir" — salt okunur değil. Yetki seviyesi ve iz katmanı arandı, bulunamadı |
| Stok | Paraşüt ek ücretsiz + çoklu depo; KolayBi depo bazlı maliyet/kâr kolonlu; Logo tek düzlem |
| e-Belge | Üçünde de **ek modül, kontörle** fiyatlanıyor; gelen fatura da kontör yakıyor |

**En sert bulgu:** Logo kendi işlem rehberinde *"İşbaşı üzerinde oluşturduğunuz
fatura kayıtlarının resmî değeri yoktur"* diyor. 9.9'a yeni çıkarım bloğu olarak
girdi: "Üründeki belge ile resmî belge aynı şey değil."

## 6 · Cümle denetimi — on düzeltme

Bölüm 9 baştan okundu. Mutlak iddialar gözlenen kapsama indirildi ("dokuz üründe
benzeri görülmeyen" → "öteki sekiz üründe görülmedi"), yanlış alan adı düzeltildi
("belge numarası" → "seri numarası"), sayı tutarsızlıkları giderildi ("dört soru" →
tablo sekiz satır), yokluk iddiaları dayanağa bağlandı ("Bölüm 1'de beş ürünün
işlem formu tek tek okundu"), ve Hesap Defterim'de kategori bir seçici olmadığı için
"kategori" → "sınıflandırma".

**Sızıntı denetimi temiz:** `BusinessFinance` · `ADR` · `TransactionScope` ·
`Counterparty` · `bizde/bizim` — bölüm metinlerinde sıfır.

## 7 · Bölüm 10 — K14 · B uygulandı, D uygulanmadı

**Eski:** 5 bölüm · 7 sayfa · 0 kare → **Yeni:** 6 bölüm · 8 sayfa · 0 kare

| # | Bölüm | Durum |
|---|---|---|
| 10.1 | Dokuz bölüm, dokuz ayrım noktası | 9. satır yeni Bölüm 9'a göre yeniden yazıldı |
| 10.2 | Üç tekrar eden desen | değişmedi |
| **10.3** | **Beş ürünün motoru, aynı beş soruyla** | **YENİ** |
| 10.4 | Ürün ürün: neyi iyi yapıyor, neyi bırakıyor | kaynak üçlüsünün satırları güncellendi |
| 10.5 | Bir paranın belgedeki yolu | değişmedi |
| 10.6 | Belgenin bıraktığı yer | sorular 7 → 9 |

**10.3'ün işi:** beş soru beş ürüne aynı biçimde soruluyor — *para nerede yaşıyor ·
kayıt ile ödeme ayrılıyor mu · gelecek nasıl tutuluyor · ayın toplamı neyi sayıyor ·
toplamın tanımı ekranda yazıyor mu.* Sütun aşağı okununca tek ürünün modeli, satır
yan yana okununca aynı sorunun beş cevabı. Belgede ilk kez tek bir ürün uçtan uca
okunabiliyor — 10.4 "iyi mi kötü mü" der, bu "nasıl çalışıyor" der. Bu yüzden
10.4'ten **önce** duruyor: önce model, sonra bedeli.

**"Bu araştırmanın göremedikleri" sayfası yazıldı ve kullanıcı kararıyla
kaldırıldı** — kanıt düzeyi zaten her sayfada rozetli ve her bölümün kendi eksik
listesi var. 10.2'den oraya taşınmış olan "Bu belgenin sınırı" bloğu **10.2'ye geri
konuldu**; kaldırma yalnız yeni sayfayı kapsadı.

**v1'in "çözülmemiş yerler" listesi taşınmadı**, çünkü v3 onları çözmüş:
Goodbudget'ın v1'de "açıklanamayan ₺600 farkı" dediği şey 1.8'de tam olarak hesaba
yazılmayan kayıtların toplamı diye kanıtlı (2.050 → 3.284, tam 1.234 artış);
Wallet'ın birleşmeyen borç kartları 4.7'de duruyor; Money Manager'ın ters görünen
sütun başlıklarının v3'te hiç kanıtı yok.

## 8 · Motorda üç düzeltme, bir yeni kapı

1. **`_soru`** — yukarıda, madde 1.
2. **`_cikarim` sayfa kırılması** — blok yüksekliği hesaplanırken `Dayanağı:` satırı
   sayılmıyordu; blok sayfanın dibine sığınca dayanağı tek başına sonraki sayfaya
   düşüyordu. Artık blok gövdesiyle dayanağı ayrılmaz.
3. **G4b kapısı** — metinde `**` varsa üretim durur. Motor markdown kalınlığı
   basmaz, yıldızları olduğu gibi yazar; bu sessizce beş yerde oldu.

---

## Bekleyenler

### Karar bekleyen

1. **Kapakta kendi ürün adımız.** `BusinessFinance · Rakip araştırması` ve "Belge 3
   **BusinessFinance** önerisini kurar" — A3/G7 kuralına göre geçmemesi gerekir.
   **Belge 1'in kapağı da aynı**, yani baştan beri böyle, bu oturumda eklenmedi.
   Önerim: kalsın — kapak belgenin kimliği, gövdedeki bir karşılaştırma iddiası
   değil. Değiştirilecekse iki belgeye birden uygulanmalı.

2. **Diğer ajanın emülatör koşumu.** Belge 1 ve Belge 2'nin eksik listeleri için
   koşum yapılıyor. Sonuç gelince hangi maddelerin kapanacağına ve metnin nasıl
   değişeceğine karar verilecek. **Kural: mevcut test verisi silinmez veya kontrol
   değerine döndürülmez.**

### Kararı belli, küçük işler

3. **`tam/uret.py` kapıları koşmuyor.** G4/G7/G8 yalnız bölüm üretiminde çalışıyor;
   birleşik belgenin kendi sayfaları (kapak, içindekiler, kanıt eki) hiçbir
   taramadan geçmiyor. Kapaktaki bulgu bu yüzden ancak elle bakınca çıktı.
4. **`belge2/README.md` eski** — "94 sayfa, 127 kare, 1.197 iddia" yazıyor; gerçek
   **105 sayfa, 127 kare**. Bölüm tablosu da eski: Bölüm 9 için "9 sayfa, 4 kare"
   diyor (artık 12 sayfa, 6 kare), Bölüm 6 için "10 kare" diyor (11).

### Sıradaki büyük iş

5. **Belge 1 düzeltme turu.** `belge2-yeniden-kurgu.md` §9'da 15 kural ve 7 maddelik
   iş listesi hazır: tarih taraması, "görülmedi" sayımı, kendi kavramlarımız
   taraması, kapsam–gövde tutarlılığı, üç kaynak ürünün Bölüm 9'a göndermesi, rozet
   yoğunluğu, giriş bölümleri. Belge 2 beğenildikten sonra uygulanacaktı; Belge 2
   bugün itibarıyla üretilmiş durumda.
6. **Belge 3** — ondan sonra.

---

## Bir sonraki oturuma iki hatırlatma

- **Yeni bir kaynak bulgusu doğrudan bölüme yazılmaz.** Önce ilgili gözlem formuna
  (URL'siyle), sonra bölüme. G2 bunu zorluyor ve bugün de bu sırayla yapıldı.
- **Kapılar yapıyı korur, iddianın kanıttan çıktığını doğrulayamaz.** Her sayfa
  `onizleme/` üzerinden gözle incelenir. Bugün kapıların yakalamadığı dört şey
  gözle bulundu: öksüz `Dayanağı:` satırı, basılan `**` işaretleri, 8.14'ün yanlış
  konum cümlesi ve 6.11'in kesik kırpması.

---

## Değişen dosyalar

    belge2/ortak/b2.py                      _soru eklendi · _cikarim düzeltildi · G4b kapısı
    belge2/bolum-03-kart/icerik.py          3.1b 470→420
    belge2/bolum-04-borc-cari/icerik.py     4.5 350 · E0195 kırpma 800
    belge2/bolum-05-zaman/icerik.py         5.6 470→420
    belge2/bolum-06-siniflandirma/icerik.py 6.3 350 · E0190 kırpma 845
    belge2/bolum-07-rapor/icerik.py         7.6 350 · "matris" okurun diline çevrildi
    belge2/bolum-08-veri/icerik.py          8.3 350 · 8.14 "Çekmecede" düzeltmesi
    belge2/bolum-09-belge/icerik.py         tamamen yeniden kuruldu
    belge2/bolum-10-kapanis/icerik.py       10.3 eklendi · numaralar kaydı · satırlar güncellendi
    belge2-yeniden-kurgu.md                 §11 K13 · §12 K14
    gozlemler/kolaybi.md                    Faz 8
    gozlemler/parasut.md                    Faz 8
    gozlemler/logo-isbasi.md                Faz 8

Bütün bölümler ve `tam/` yeniden üretildi; hiçbirinde uyarı yok.
