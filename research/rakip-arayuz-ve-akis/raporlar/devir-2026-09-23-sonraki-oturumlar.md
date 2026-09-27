# Devir: Belge 1 düzeltme turu

23 Eylül 2026 oturumundan sonraki oturum için. Bu dosya iki şey taşır: Belge 1 turunun
malzemesi ve yöntemi (bölüm 1-4) ve oturumun başına yapıştırılacak istem (bölüm 5).

## 1. Başlarken durum

- Belge 2 tamamlandı. 22 Eylül ek koşumunun bulguları 23 Eylül'de işlendi; ne değiştiği
  `belge2-entegrasyon-plani.md` bölüm 10'da. Aynı oturumda Belge 2'nin kare kırpmaları
  da düzeltildi; kullanılan kırpma kuralı ve sonucu `belge2-entegrasyon-plani.md`
  bölüm 11'de yazılı. Belge 1 turuna başlamadan ikisi de okunur.
- Belge 2 23 Eylül'ün ikinci oturumunda kapandı: 105 sayfa, 129 kare, kapılar temiz. Son
  hâli ve kararları `belge2-son-kontrol.md`; o oturumun özeti `belge2-devir-2026-09-23.md` §5.
- Belge 1 (`belge1/tam/belge1.pdf`, 98 sayfa, 145 kare) 17 Eylül'den beri değişmedi.
- Envanter: 23 Eylül'de E0425-E0462 (38 kare) eklendi. 22 Eylül koşumunun kalan 74 karesi
  envanterde değil; Belge 1 turunun malzemesi.
- Git: 11 Eylül'den beri commit yok, `raporlar/` Git'te değil. Bu turun kapsamı dışında;
  commit yapılmaz.

## 2. Belge 1 turunun üç iş kaynağı

**a) Belge 2'den taşınacak kurallar.** `belge2-yeniden-kurgu.md` bölüm 9 tablosu:
A1-A5, B1-B3, C1-C4 ve D1-D4 "Belge 1'e: Evet"; E1-E4 Belge 2'ye özgü, taşınmaz. Aynı
yerdeki 7 maddelik iş listesi. **23 Eylül kararlarıyla daraldı:** 2 (görülmedi sayımı) temizlik
kuralına döndü, 6 (rozet) alınmadı, 7 (giriş bölümleri) Bölüm 1–2 birleşmesine dönüştü. Güncel
liste `belge1-hazirlik.md` karar 5–10'da:

1. Tarih taraması (kalıbın `TARIH = "16 Eylül 2026"` sabiti dahil).
2. "Görülmedi" sayımı, sonra eksik listesine indirme.
3. Kendi kavramlarımız taraması (G7 kapısı Belge 1 üretimine eklenir).
4. Kapsam ve gövde tutarlılığı (G8 kapısı).
5. Üç kaynak ürüne (Paraşüt, Logo, QuickBooks) Belge 2 Bölüm 9 göndermesi.
6. Rozet yoğunluğu (yalnız istisnada rozet).
7. Giriş bölümleri (okuma kılavuzu kesilmeden önce ölçülür).

**b) Dış göz inceleme notları.** `belge1/inceleme-notlari.md`, 20 madde. `DURUM.md`
bunları "hâlâ işlenmedi" diye kayıtlı. Her birinin hâlâ geçerli olduğu kaynağa bakılarak
doğrulanır.

**c) 22 Eylül ek koşumunun Belge 1 eksikleri.** `eksik-kosum-ortak-listesi.md` içinde
kaynağı "B1" olan kapananlar: MM-01, MM-02, MM-08, MM-11, WL-01, WL-02, WL-10, WL-12,
WL-14, WL-15, WL-17, WL-18, WL-19, WL-20, BC-01, BC-03, BC-06, BC-07, BC-13, BC-16, GB-01,
GB-03, GB-04 (hesap makinesi yarısı, kullanıcı gözlemi), HD-01, HD-03, WB-01. Açık
kalanlar: BC-X1, HD-04, WB-02, GB-04'ün tebrik mesajı. Bunların bir kısmı Belge 2'de de
kullanıldı; o kareler zaten envanterde (E0425-E0462) ve gözlem formlarında 23 Eylül
bölümleri yazılı.

**d) 23 Eylül ikinci oturumunda Belge 2'de alınan ve Belge 1'i ilgilendiren kararlar.**

1. **Kapakta kendi ürün adımız kalıyor** (kullanıcı kararı). Belge 1'e G7 kapısı
   eklenirken kapak yalnız bu ad için muaf tutulur. Örnek: `belge2/tam/uret.py` içindeki
   `ek_kapilar` (G9): kapak, içindekiler ve kanıt eki G4, G4b ve G7'den geçer.
2. **Çıkarım denetiminin ölçütü.** Çıkarım rozetli metin, karede birebir yazmadığı için
   "fazla" sayılmaz. Önce karelerden bağımsız bir çıkarım yapılır, sonra metinle
   karşılaştırılır. Değişiklik yalnız görünenle ya da belgenin başka bir sayfasıyla
   çelişkide önerilir. Sayım hatası ise çıkarım değildir, düzeltilir.
3. **Bluecoins'in hesap bölümleri.** Hesaplar ekranında iki üst bölüm var: VARLIKLAR (Banka,
   Nakit) ve CARİ HESAP (Cari hesap, Kredi Kartı, İpotekler). Alacak kartla aynı bölümde,
   borç tarafında duruyor ve varlık toplamına girmiyor. Net Kazanç'taki "Cari hesap" satırı
   bu bölümün toplamı (E0049, E0055, E0053; gözlem formunda 23 Eylül satırı). Belge 1 bu
   başlıkları yazıyor (6.x, 7.x, 9.1); turda alacağın "varlıkta" durduğunu söyleyen bir
   cümle olup olmadığına bakılır.
4. **KolayBi masaüstü kutusu.** Belge 2'de `(96, 112, 1305, 700)` kutusu pencerelerin
   düğmelerini ve bir tablonun kolonlarını kesiyordu (8.10, 8.11 ve 7.16 düzeltildi).
   Belge 1'de yedi bölüm aynı kutu ailesini kullanıyor; alt sınırlar şeklin satırlarına göre
   kontrol edilir.
5. **Birleşik belge toplu üretilir.** Küçük düzeltmeden sonra yalnız bölümün `uret.py`'si
   çalışır ve değişen sayfaya bakılır. `tam/uret.py` düzeltmeler birikince bir kez çalışır.

## 3. Kırpma ve işaretler

Belge 2'de kareler üstten 55 piksel, alttan çok kesildiği için durum çubuğu (saat, pil)
kalıyor, iddianın kendisi ise alttan kesiliyordu. 23 Eylül'de düzeltildi: üst sınır durum
çubuğunu atar, alt sınır şeklin altında yazılı her şeyi içerir, sayfada yer varsa kare
yüksekliği artırılır.

Belge 1'de durum farklı: şekillerin çok azı kırpma taşıyor, çoğu tam ekran basılıyor.
Alttan kesilme sorunu orada küçük; durum çubuğu ise her karede duruyor. Ama Belge 1'de
**156 işaretli şekil** var ve işaret koordinatları kırpma sonrası uzayda yazılı
(`motor.isaretle`). Üstten kesmek her işareti kesilen piksel kadar kaydırır. Seçenekler:
motor işaretleri ham ekran koordinatında tutacak şekilde değişir, ya 156 şeklin
koordinatı güncellenir, ya da Belge 1'de çubuk kalır. **Karar verildi (23 Eylül):** kesme
uygulamanın başında, Belge 1'e özgü ayarla; ayrıntı `belge1-hazirlik.md` karar 4.

**Paylaşılan motor uyarısı:** `belge1/ortak/kalip.py` ve `motor.py` iki belgenin ortak
motorudur. Motorda yapılan her değişiklikten sonra Belge 2 de yeniden üretilir ve
kapıları temiz kalmalıdır.

**Karartma:** Goodbudget `39`, `41` (hane adı) ve Wallet `f7-64`, `f7-65` (içe aktarma
e-postası) basılacaksa `kalip.py` içindeki `KARARTMA_ORTAK` listesine girer. Money
Manager `61` Play hesap baş harfini taşıyor. 23 Eylül kare doğrulamasında eklenenler: MM `47`, `53` ve
Bluecoins `f7-55` (reklam, kırpılır) · Wallet `f7-64` (import e-postası) · Goodbudget `38`
(hane adı; `39` zaten listede).

## 4. 23 Eylül'de öğrenilen dersler

- Koşum özetine güvenme, kareyi aç. 22 Eylül koşumunun dört sonucu kareye bakınca
  çürük çıktı (WL-11, Wallet %2147483647, GB-02, BC-17).
- Kareden değil sayfadan başla. Yeni bulguların önemli kısmı belgedeki bir yanlışı
  düzeltti; eklemeden önce onları bul.
- Şekil satırı yalnız kendi karesinde görüneni anlatır.
- Karesiz kullanıcı gözlemi "kosum" türüyle ve cümlede "kullanıcı kontrolüyle kayıtlı,
  karesi yok" diye girer. Aritmetikle kurulan sonuç "cikarim" rozetiyle ve dayanağıyla.
- Bir iddia kanıtın söylediği kadar yazılır ("listede var", "hazır geliyor" değil).
- Yeni bulgu önce ilgili `gozlemler/*.md` formuna, sonra belgeye (G2).
- Eksik listeleri `icerik.py` içindeki `EKSIKLER` listesinden üretilir.
- Koşum test verisi silinmez. Eski kontrol değeriyle çelişen toplam taşıyan kare basılmaz
  ya da cümle sayının koşum kaydından sonra okunduğunu söyler.
- Kabuk heredoc'u Türkçe tırnaklı metinde bozulabiliyor; `icerik.py` düzenlemeleri
  scratchpad'e yazılan küçük bir Python betiğiyle yapılır (eski ve yeni metin çiftleri,
  her eski metin dosyada tam bir kez bulunmalı).
- `tam/uret.py` 2 dakikayı geçer; arka planda çalıştırılır.

## 5. Oturumun başına yapıştırılacak istem

```
research/rakip-arayuz-ve-akis/raporlar/ üzerinde çalışıyoruz. Bu oturumun işi Belge 1'in
düzeltme turunun Aşama 1'i: Belge 1'i bölüm bölüm okuyup yeni kanıtın ve 23 Eylül
kararlarının her bölümde neyi değiştirdiğini çıkarmak. Belge 1'e henüz dokunulmaz.
Git'e commit yapılmaz.

Önce oku, sırayla:
1. raporlar/belge1-hazirlik.md (tamamı). Tek kaynak budur. Aşama 0 (kaynak doğrulama)
   bitti. Kullanıcının kararları "Kullanıcının verdiği kararlar" ve "İkinci karar turu"
   başlıklarında (1–10). Temizlik kuralı, Belge 2'ye dönük bekleyen iş ve Aşama 1'in
   yöntemi ("Aşama 1'e devir") aynı dosyada.
2. raporlar/devir-2026-09-23-sonraki-oturumlar.md bölüm 2c–2d, 3 ve 4 (malzeme, kırpma
   notları, dersler).
3. raporlar/belge1/README.md ve raporlar/belge1/inceleme-notlari.md (20 madde; doğrulama
   sonucu hazırlık dosyasında).
4. raporlar/eksik-kosum-ortak-listesi.md: kaynağı B1 olan satırlar (kare doğrulaması
   hazırlık dosyasında).

Belge 1'in doğası: arayüz belgesi. Belge 2'den yalnız bu turun amacına (yeni kanıtla
güncelleme ve hata düzeltme) hizmet eden kurallar alınır. Çıkarım katmanı ve Belge 3
soruları eklenmez. Rozet yapısına dokunulmaz. Ürün adımız kapakta ve sınır cümlelerinde
kalır.

Aşama 1'in işi (hazırlık dosyasındaki "Aşama 1'e devir"): yalnız tam/uret.py'nin kullandığı
13 klasör okunur (bolum-03-* altında yalnız bolum-03-ana-ekran). Her bölüm için:
1. Kanıt güncellemesi: "görülmedi · denenmedi · doğrulanmadı · Bilinmeyenler" yeni kanıta
   karşı; d1, d2 ve g1–g8 yerine yerleşir.
2. "Çıkarılmayan sonuç" notları üç yoldan birine atanır (karar 5).
3. Temizlik (a) ve iddia denetimi (b).
4. Yeni kare adayları (sayfa ihtiyacıyla; karar yok, liste).
5. O bölüme düşen dış göz notları.
Bölüm 1 ve 2 birleşeceği için (karar 6) bu iki bölüm yeni giriş bölümünün taslağı olarak
okunur.

Sonuçlar belge1-hazirlik.md'ye bölüm bölüm yazılır; tablo: yer (sayfa · öğe) · şimdiki
ifade · yeni · tür (D yanlış · G güncelleme · T temizlik) · kaynak · kare. Her bölüm
bitince dosyaya yazılır ki oturum kesilirse kayıp olmasın.

Aşama 1 bitince Aşama 2: raporlar/belge1-duzeltme-plani.md, belge2-entegrasyon-plani.md
yapısıyla. Açık kararlar seçenekli ve önerili olarak belgeye yazılır, sohbette seçim
ekranıyla sorulmaz. Uygulama (Aşama 3) kullanıcı onayından sonra; ilk adımı kesme
(karar 4).

Dersler:
- Koşum özetine güvenme, kareyi aç.
- Kareden değil sayfadan başla.
- Şekil satırı ve işaret yalnız kendi karesinde görüneni anlatır; ürünün sözü olgu gibi
  yazılmaz.
- Kullanıcı kararlarının kaydı hazırlık dosyasındadır; orada yazılı bir kararı yeniden
  sorma.
```
